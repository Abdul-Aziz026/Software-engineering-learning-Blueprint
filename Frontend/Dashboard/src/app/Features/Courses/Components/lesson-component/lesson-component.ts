import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, switchMap } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { LessonDetailsService } from '../../Services/lesson-details.service';
import { LessonDetails } from '../../Models/lesson-details.model';

@Component({
  selector: 'app-lesson-component',
  standalone: true,
  imports: [AsyncPipe, FormsModule],
  templateUrl: './lesson-component.html',
  styleUrl: './lesson-component.scss',
})
export class LessonComponent implements OnInit {
  lessonDetails$!: Observable<LessonDetails>;
  isEditing = false;
  editingLesson: LessonDetails | null = null;
  courseId = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private lessonDetailsService: LessonDetailsService
  ) { }

  ngOnInit(): void {
    this.lessonDetails$ = this.route.params.pipe(
      switchMap(params => {
        this.courseId = params['id'];
        return this.lessonDetailsService.getLessonDetailsByLessonId(params['id'], params['lessonId']);
      })
    );
  }

  editLesson(lesson: LessonDetails) {
    this.isEditing = true;
    this.editingLesson = { ...lesson };
    // Shallow copy the array so we can edit it
    this.editingLesson.referenceUrls = lesson.referenceUrls ? [...lesson.referenceUrls] : [];
  }

  cancelEdit() {
    this.isEditing = false;
    this.editingLesson = null;
  }

  addUrl() {
    if (this.editingLesson) {
      if (!this.editingLesson.referenceUrls) {
        this.editingLesson.referenceUrls = [];
      }
      this.editingLesson.referenceUrls.push('');
    }
  }

  removeUrl(index: number) {
    if (this.editingLesson && this.editingLesson.referenceUrls) {
      this.editingLesson.referenceUrls.splice(index, 1);
    }
  }

  saveLesson() {
    if (!this.editingLesson) return;

    // Filter empty URLs
    if (this.editingLesson.referenceUrls) {
      this.editingLesson.referenceUrls = this.editingLesson.referenceUrls
        .map(s => s.trim())
        .filter(s => s.length > 0);
    }

    this.lessonDetailsService.updateLessonDetails(this.editingLesson.id, this.editingLesson).subscribe({
      next: () => {
        this.isEditing = false;
        // Reload data
        this.ngOnInit();
      },
      error: (err) => console.error('Failed to update lesson details', err)
    });
  }

  deleteLesson(lesson: LessonDetails) {
    if (window.confirm(`Are you sure you want to delete "${lesson.title}"? This action cannot be undone.`)) {
      this.lessonDetailsService.deleteLessonDetails(lesson.id).subscribe({
        next: () => {
          // Route back to the course dashboard
          this.router.navigate(['/course', this.courseId]);
        },
        error: (err) => console.error('Failed to delete lesson', err)
      });
    }
  }
}
