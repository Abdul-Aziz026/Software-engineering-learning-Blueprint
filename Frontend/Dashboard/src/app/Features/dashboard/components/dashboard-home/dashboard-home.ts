import { Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { PostService } from '../../../Posts/Services/post.service';
import { PostSummary } from '../../../Posts/Models/post.model';
import { AuthService } from '../../../Auth/Services/auth.service';
import { SubscribeFormComponent } from '../../../Subscribers/Components/subscribe-form/subscribe-form';

@Component({
  selector: 'app-dashboard-home',
  standalone: true,
  imports: [DatePipe, RouterLink, SubscribeFormComponent],
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss',
})
export class DashboardHome implements OnInit {
  private readonly postService = inject(PostService);
  readonly authService = inject(AuthService);

  posts: PostSummary[] = [];
  loading = false;

  ngOnInit(): void {
    this.loading = true;
    this.postService.getPosts(1, 9).subscribe({
      next: (posts) => {
        this.posts = posts;
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }
}
