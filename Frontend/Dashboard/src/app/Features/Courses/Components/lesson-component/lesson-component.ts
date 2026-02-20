import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CourseService } from '../../Services/course.service';

@Component({
  selector: 'app-lesson-component',
  standalone: true,
  imports: [],
  templateUrl: './lesson-component.html',
  styleUrl: './lesson-component.scss',
})
export class LessonComponent implements OnInit {
  course: any;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private courseService: CourseService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.loadCourse(params['id']);
    });
  }

  loadCourse(id: string) {
    this.courseService.getCourseById(id).subscribe({
      next: (data) => {
        this.course = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading course:', err);
        this.loading = false;
      }
    });
  }
}
