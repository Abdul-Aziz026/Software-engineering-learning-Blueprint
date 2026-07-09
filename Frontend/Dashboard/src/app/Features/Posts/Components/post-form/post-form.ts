import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { marked } from 'marked';
import { PostService } from '../../Services/post.service';
import { AuthService } from '../../../Auth/Services/auth.service';

@Component({
  selector: 'app-post-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './post-form.html',
  styleUrl: './post-form.scss'
})
export class PostFormComponent implements OnInit {
  isEditing = false;
  postId: string | null = null;
  errorMessage = '';

  form = {
    title: '',
    summary: '',
    content: ''
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private postService: PostService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    // Creating/editing requires authentication.
    if (!this.authService.currentUser()) {
      this.router.navigate(['/posts']);
      return;
    }

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditing = true;
      this.postId = id;
      this.postService.getPost(id).subscribe({
        next: (post) => {
          // Only the author may edit; otherwise bounce back to the post.
          if (post.authorId !== this.authService.currentUser()?.userId) {
            this.router.navigate(['/posts', id]);
            return;
          }
          this.form = { title: post.title, summary: post.summary, content: post.content };
        },
        error: () => this.router.navigate(['/posts'])
      });
    }
  }

  get previewHtml(): string {
    return this.form.content ? (marked.parse(this.form.content) as string) : '';
  }

  submit(): void {
    if (!this.form.title.trim() || !this.form.content.trim()) {
      this.errorMessage = 'Title and content are required.';
      return;
    }

    const payload = {
      title: this.form.title.trim(),
      content: this.form.content,
      summary: this.form.summary.trim() || undefined
    };

    if (this.isEditing && this.postId) {
      const id = this.postId;
      this.postService.updatePost(id, payload).subscribe({
        next: () => this.router.navigate(['/posts', id]),
        error: (err) => (this.errorMessage = err.error?.message ?? 'Failed to save post.')
      });
    } else {
      this.postService.createPost(payload).subscribe({
        next: () => this.router.navigate(['/posts']),
        error: (err) => (this.errorMessage = err.error?.message ?? 'Failed to create post.')
      });
    }
  }

  cancel(): void {
    if (this.isEditing && this.postId) {
      this.router.navigate(['/posts', this.postId]);
    } else {
      this.router.navigate(['/posts']);
    }
  }
}
