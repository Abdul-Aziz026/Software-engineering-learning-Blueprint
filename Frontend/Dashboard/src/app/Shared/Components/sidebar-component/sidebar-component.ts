import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CourseService } from '../../../Features/Courses/Services/course.service';
// import { CourseService } from 'src/app/services/course.service';

@Component({
  selector: 'app-sidebar-component',
  templateUrl: './sidebar-component.html',
  styleUrls: ['./sidebar-component.scss']
})
export class SidebarComponent implements OnInit {
  courseData: any;
  courseId: string = '';

  constructor(
    private route: ActivatedRoute,
    private courseService: CourseService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.courseId = params['id'];
      this.loadCourseData();
    });
  }

  loadCourseData() {
    this.courseService.getCourseStructure(this.courseId).subscribe({
      next: (data) => {
        this.courseData = data;
      },
      error: (err) => {
        console.error('Error loading course:', err);
      }
    });
  }
}