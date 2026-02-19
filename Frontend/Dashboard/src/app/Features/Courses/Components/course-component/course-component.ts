import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CourseService } from '../../Services/course.service';

@Component({
  selector: 'app-course-component',
  standalone: true,
  imports: [],
  templateUrl: './course-component.html',
  styleUrl: './course-component.scss',
})
export class CourseComponent implements OnInit {
  course: any;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private courseService: CourseService
  ) { }

  ngOnInit() {
    const courseId = this.route.parent?.snapshot.params['id'];
    this.loadCourse(courseId);
  }

  loadCourse(id: string) {
    this.courseService.getCourseById(id).subscribe({
      next: (data) => {
        this.course = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading course:', err);
        this.loading = false;
      }
    });
  }
}
