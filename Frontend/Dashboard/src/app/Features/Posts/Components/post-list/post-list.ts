import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PostService } from '../../Services/post.service';
import { PostSummary } from '../../Models/post.model';
import { AuthService } from '../../../Auth/Services/auth.service';
import { AuthModalService } from '../../../Auth/Services/auth-modal.service';
import { PostCardComponent } from '../post-card/post-card';

@Component({
  selector: 'app-post-list',
  standalone: true,
  imports: [PostCardComponent],
  templateUrl: './post-list.html',
  styleUrl: './post-list.scss'
})
export class PostListComponent implements OnInit {
  posts: PostSummary[] = [];
  loading = false;
  error = '';

  constructor(
    private postService: PostService,
    private router: Router,
    public authService: AuthService,
    private authModal: AuthModalService
  ) {}

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(): void {
    this.loading = true;
    this.error = '';
    this.postService.getPosts().subscribe({
      next: (posts) => {
        this.posts = posts;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message ?? 'Failed to load posts.';
        this.loading = false;
      }
    });
  }

  openPost(id: string): void {
    this.router.navigate(['/posts', id]);
  }

  startCreate(): void {
    // Creating requires a logged-in author; prompt sign-in otherwise.
    if (!this.authService.currentUser()) {
      this.authModal.open('login');
      return;
    }
    this.router.navigate(['/posts/create']);
  }
}
