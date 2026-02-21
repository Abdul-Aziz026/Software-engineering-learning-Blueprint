import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CourseService } from '../../Services/course.service';
import { Subject } from '../../Models/subject.model';

@Component({
  selector: 'app-lesson-component',
  standalone: true,
  imports: [],
  templateUrl: './home-lesson-component.html',
  styleUrl: './home-lesson-component.scss',
})
export class HomeLessonComponent implements OnInit {
  subject?: Subject;

  constructor(private route: ActivatedRoute,
              private courseService: CourseService,
              private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.loadSubjectData(params['id']);
    });
  }

  loadSubjectData(courseId: string) {
    this.courseService.getCourseById(courseId).subscribe({
      next: (res) => {
        this.subject = res;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('error ', err);
      }
    });
    // Your API call here
    this.subject = {
      id: "1",
      name: "Mathematics",
      description: "Mathematics is the study of numbers, quantities, and shapes. In this course, you will learn fundamental concepts including algebra, geometry, and calculus that form the foundation of mathematical thinking."
    };
  }

  startLearning(): void {

  }
}
