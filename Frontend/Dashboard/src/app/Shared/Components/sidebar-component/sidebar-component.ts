import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ChapterService } from '../../../Features/Courses/Services/chapter.service';
import { SubjectService } from '../../../Features/Courses/Services/subject.service';
import { Chapter } from '../../../Features/Courses/Models/chapter.model';
import { Subject } from '../../../Features/Courses/Models/subject.model';

@Component({
  selector: 'app-sidebar-component',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './sidebar-component.html',
  styleUrls: ['./sidebar-component.scss']
})
export class SidebarComponent implements OnChanges {
  @Input() courseId!: string;

  courseData$!: Observable<Chapter[]>;
  subject$!: Observable<Subject>;
  error: string | null = null;

  activeChapterMenuId: string | null = null;

  constructor(
    private chapterservice: ChapterService,
    private subjectService: SubjectService
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['courseId']?.currentValue) {
      this.loadCourseData();
    }
  }

  private loadCourseData(): void {
    if (!this.courseId) return;

    this.error = null;
    this.subject$ = this.subjectService.getSubjectById(this.courseId);

    this.error = null;
    this.courseData$ = this.chapterservice
      .getChaptersBySubject(this.courseId)
      .pipe(
        map(data => data.map(chapter => ({ ...chapter, expanded: false }))),
        catchError(err => {
          console.error('Error loading chapters:', err);
          this.error = 'Failed to load chapters. Please try again.';
          return of([]);
        })
      );
  }

  toggleChapter(chapter: Chapter): void {
    chapter.expanded = !chapter.expanded;
  }

  toggleChapterMenu(event: Event, chapterId: string) {
    event.stopPropagation();
    this.activeChapterMenuId = this.activeChapterMenuId === chapterId ? null : chapterId;
  }

  editChapter(chapter: Chapter) {
    this.activeChapterMenuId = null;
    const newName = window.prompt('Update Chapter Name:', chapter.chapterName);
    if (newName && newName !== chapter.chapterName) {
      const updated = { ...chapter, chapterName: newName };
      this.chapterservice.updateChapter(chapter.id, updated).subscribe({
        next: () => this.loadCourseData(),
        error: (err) => console.error('Failed to update chapter', err)
      });
    }
  }

  deleteChapter(chapter: Chapter) {
    this.activeChapterMenuId = null;
    if (window.confirm(`Are you sure you want to delete chapter "${chapter.chapterName}"?`)) {
      this.chapterservice.deleteChapter(chapter.id).subscribe({
        next: () => this.loadCourseData(),
        error: (err) => console.error('Failed to delete chapter', err)
      });
    }
  }
}
