import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { marked } from 'marked';
import { PostService } from '../../Services/post.service';
import { PostDetail } from '../../Models/post.model';
import { AuthService } from '../../../Auth/Services/auth.service';
import { AuthModalService } from '../../../Auth/Services/auth-modal.service';
import { ConfirmDialogComponent } from '../../../../Shared/Components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-post-detail',
  standalone: true,
  imports: [DatePipe, FormsModule, ConfirmDialogComponent],
  templateUrl: './post-detail.html',
  styleUrl: './post-detail.scss'
})
export class PostDetailComponent implements OnInit {
  post: PostDetail | null = null;
  loading = true;
  error = '';

  newComment = '';
  pendingDeletePost = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private postService: PostService,
    public authService: AuthService,
    private authModal: AuthModalService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigate(['/posts']);
      return;
    }
    this.loadPost(id);
  }

  private loadPost(id: string): void {
    this.loading = true;
    this.postService.getPost(id).subscribe({
      next: (post) => {
        this.post = post;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message ?? 'Failed to load this post.';
        this.loading = false;
      }
    });
  }

  get contentHtml(): string {
    return this.post ? (marked.parse(this.post.content) as string) : '';
  }

  get isAuthor(): boolean {
    return !!this.post && this.authService.currentUser()?.userId === this.post.authorId;
  }

  private requireAuth(): boolean {
    if (!this.authService.currentUser()) {
      this.authModal.open('login');
      return false;
    }
    return true;
  }

  toggleLike(): void {
    if (!this.post || !this.requireAuth()) return;
    this.postService.toggleLike(this.post.id).subscribe({
      next: (res) => {
        if (!this.post) return;
        this.post.likedByCurrentUser = res.liked;
        this.post.likeCount = res.likeCount;
      }
    });
  }

  addComment(): void {
    if (!this.post || !this.newComment.trim() || !this.requireAuth()) return;
    const id = this.post.id;
    this.postService.addComment(id, { content: this.newComment.trim() }).subscribe({
      next: () => {
        this.newComment = '';
        this.loadPost(id); // refresh comments + count
      }
    });
  }

  canDeleteComment(authorId: string): boolean {
    return this.authService.currentUser()?.userId === authorId;
  }

  deleteComment(commentId: string): void {
    if (!this.post) return;
    const id = this.post.id;
    this.postService.deleteComment(id, commentId).subscribe({
      next: () => this.loadPost(id)
    });
  }

  goBack(): void {
    this.router.navigate(['/posts']);
  }

  editPost(): void {
    if (this.post) this.router.navigate(['/posts', this.post.id, 'edit']);
  }

  requestDeletePost(): void {
    this.pendingDeletePost = true;
  }

  confirmDeletePost(): void {
    if (!this.post) return;
    this.postService.deletePost(this.post.id).subscribe({
      next: () => {
        this.pendingDeletePost = false;
        this.router.navigate(['/posts']);
      }
    });
  }

  cancelDeletePost(): void {
    this.pendingDeletePost = false;
  }
}
