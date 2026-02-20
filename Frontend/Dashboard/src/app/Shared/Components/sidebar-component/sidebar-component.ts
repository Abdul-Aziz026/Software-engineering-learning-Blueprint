import { ChangeDetectorRef, Component, Input, OnChanges, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CourseService } from '../../../Features/Courses/Services/course.service';
import { ChapterService } from '../../../Features/Courses/Services/chapter.service';
import { Chapter } from '../../../Features/Courses/Models/chapter.model';
// import { CourseService } from 'src/app/services/course.service';

@Component({
  selector: 'app-sidebar-component',
  standalone: true,
  imports: [
    RouterLink
  ],
  templateUrl: './sidebar-component.html',
  styleUrls: ['./sidebar-component.scss']
})
export class SidebarComponent implements OnChanges {
  courseData: any;
  @Input() courseId!: string;

  constructor(
    private route: ActivatedRoute,
    private chapterservice: ChapterService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnChanges() {
    if (this.courseId) {
      this.loadCourseData();
    }
  }

  loadCourseData() {
    this.chapterservice.getChaptersBySubject(this.courseId).subscribe({
      next: (data) => {
        // Initialize expanded property for each chapter
        this.courseData = data.map(chapter => ({
          ...chapter,
          expanded: false // or true if you want them expanded by default
        }));
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading course:', err);
      }
    });
  }

  toggleChapter(chapter: Chapter) {
    chapter.expanded = !chapter.expanded;
    this.cdr.detectChanges();
  }
}