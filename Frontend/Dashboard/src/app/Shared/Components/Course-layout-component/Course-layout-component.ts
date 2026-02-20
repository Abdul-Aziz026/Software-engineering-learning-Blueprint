import { Component, OnInit } from '@angular/core';
import { HeaderComponent } from '../header-component/header-component';
import { FooterComponent } from '../footer-component/footer-component';
import { SidebarComponent } from '../sidebar-component/sidebar-component';
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';

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
export class CourseLayoutComponent implements OnInit {
  courseId: string = '';

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    let childRoute = this.route.firstChild?.firstChild;
    if (childRoute) {
      this.courseId = childRoute.snapshot.paramMap.get('id')!;
    }
  }
}
