import { ChangeDetectorRef, Component, Input, OnChanges, OnInit } from '@angular/core';
import { LessonDetailsService } from '../../Services/lesson-details.service';
import { LessonDetails } from '../../Models/lesson-details.model';

@Component({
  selector: 'app-lesson-component',
  standalone: true,
  imports: [],
  templateUrl: './lesson-component.html',
  styleUrl: './lesson-component.scss',
})
export class LessonComponent implements OnChanges{
  @Input() id: string = '';
  @Input() lessonId: string = '';

  lessonDetails: LessonDetails | undefined;

  constructor(private lessonDetailsService: LessonDetailsService,
              private cdr: ChangeDetectorRef
  ) { }

  ngOnChanges(): void {
    this.lessonDetailsService.getLessonDetailsByLessonId(this.id, this.lessonId).subscribe({
      next: (res) => {
        this.lessonDetails = res;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("error: ", err);
      }
    });
  }

}
