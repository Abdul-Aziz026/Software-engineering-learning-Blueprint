import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CourseService } from '../../Services/course.service';
import { Subject } from '../../Models/subject.model';
import { ChapterService } from '../../Services/chapter.service';

@Component({
  selector: 'app-lesson-component',
  standalone: true,
  imports: [],
  templateUrl: './home-lesson-component.html',
  styleUrl: './home-lesson-component.scss',
})
export class HomeLessonComponent implements OnInit {
  subject?: Subject;
  courseId: string = '';

  constructor(private route: ActivatedRoute,
    private courseService: CourseService,
    private chapterService: ChapterService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.courseId = params['id'];
      this.loadSubjectData(this.courseId);
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
