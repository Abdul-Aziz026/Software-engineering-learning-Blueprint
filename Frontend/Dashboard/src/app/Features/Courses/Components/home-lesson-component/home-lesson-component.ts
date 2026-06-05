import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, switchMap, tap } from 'rxjs';
import { SubjectService } from '../../Services/subject.service';
import { Subject } from '../../Models/subject.model';
import { ChapterService } from '../../Services/chapter.service';

@Component({
  selector: 'app-lesson-component',
  standalone: true,
  imports: [AsyncPipe],
  templateUrl: './home-lesson-component.html',
  styleUrl: './home-lesson-component.scss',
})
export class HomeLessonComponent implements OnInit {
  subject$!: Observable<Subject>;
  courseId: string = '';

  constructor(
    private route: ActivatedRoute,
    private subjectService: SubjectService,
    private chapterService: ChapterService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.subject$ = this.route.params.pipe(
      tap(params => this.courseId = params['id']),
      switchMap(params => this.subjectService.getSubjectById(params['id']))
    );
  }

  startLearning(): void {
    if (!this.courseId) return;

    this.chapterService.getChaptersBySubject(this.courseId).subscribe({
      next: (chapters) => {
        if (chapters && chapters.length > 0) {
          const firstChapter = chapters[0];
          if (firstChapter.lessons && firstChapter.lessons.length > 0) {
            const firstLessonId = firstChapter.lessons[0].id;
            this.router.navigate(['/course', this.courseId, 'lesson', firstLessonId]);
          } else {
            console.warn('No lessons found in the first chapter.');
          }
        } else {
          console.warn('No chapters found for this subject.');
        }
      },
      error: (err) => {
        console.error('Error fetching chapters:', err);
      }
    });
  }
}
