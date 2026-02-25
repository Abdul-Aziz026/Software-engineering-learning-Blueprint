import { Component, OnDestroy, OnInit } from '@angular/core';
import { HeaderComponent } from '../header-component/header-component';
import { FooterComponent } from '../footer-component/footer-component';
import { SidebarComponent } from '../sidebar-component/sidebar-component';
import { ActivatedRoute, RouterOutlet } from '@angular/router';
import { Subscription } from 'rxjs';

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

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    // Subscribe to the live paramMap so courseId updates on every navigation
    // without requiring a full browser reload
    this.sub = this.route.firstChild!.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.courseId = id;
      }
    });
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
  }
}
