import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { BlogPostSummary } from '../../Models/blog.model';

/**
 * Presentational ("dumb") component.
 * Knows nothing about HttpClient, routing, or auth.
 * It only receives a post via input() and emits intent via output().
 */
@Component({
  selector: 'app-blog-card',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './blog-card.html',
  styleUrl: './blog-card.scss'
})
export class BlogCardComponent {
  // Required signal input — the parent must provide a post.
  post = input.required<BlogPostSummary>();

  // Output signal — the card asks the parent to open it, but doesn't navigate itself.
  open = output<string>();
}
