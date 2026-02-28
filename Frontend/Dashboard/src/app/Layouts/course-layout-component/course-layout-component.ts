import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter, Subscription } from 'rxjs';
import { HeaderComponent } from '../../Shared/Components/header-component/header-component';
import { FooterComponent } from '../../Shared/Components/footer-component/footer-component';
import { SidebarComponent } from '../../Shared/Components/sidebar-component/sidebar-component';

@Component({
  selector: 'app-course-layout-component',
  imports: [
    HeaderComponent,
    FooterComponent,
    SidebarComponent,
    RouterOutlet
  ],
  templateUrl: './course-layout-component.html',
  styleUrl: './course-layout-component.scss',
})
export class CourseLayoutComponent implements OnInit, OnDestroy {
  courseId: string = '';
  private sub!: Subscription;

  constructor(private route: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    // Extract courseId on initial load
    this.courseId = this.extractCourseId(this.route);

    // Re-extract on every subsequent navigation (handles /course/:id/lesson/:lessonId too)
    this.sub = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.courseId = this.extractCourseId(this.route);
      });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  /** Walks the activated route tree and returns the first 'id' param found */
  private extractCourseId(route: ActivatedRoute): string {
    let current: ActivatedRoute | null = route;
    while (current) {
      const id = current.snapshot.paramMap.get('id');
      if (id) return id;
      current = current.firstChild;
    }
    return this.courseId; // keep previous value if not found
  }
}
