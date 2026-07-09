import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { PostSummary } from '../../Models/post.model';

/**
 * Presentational ("dumb") component.
 * Knows nothing about HttpClient, routing, or auth.
 * It only receives a post via input() and emits intent via output().
 */
@Component({
  selector: 'app-post-card',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './post-card.html',
  styleUrl: './post-card.scss'
})
export class PostCardComponent {
  // Required signal input — the parent must provide a post.
  post = input.required<PostSummary>();

  // Output signal — the card asks the parent to open it, but doesn't navigate itself.
  open = output<string>();
}
