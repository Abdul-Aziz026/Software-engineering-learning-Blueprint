import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { SubjectService } from '../../../Features/Courses/Services/subject.service';
import { Subject } from '../../../Features/Courses/Models/subject.model';
import { Store } from '@ngrx/store';
import { selectCount } from '../../../Core/Store/selectors/counter.selectors';
import { selectAllSubjects } from '../../../Core/Store/selectors/subject.selectors';

@Component({
  selector: 'app-header-component',
  standalone: true,
  imports: [RouterLink, AsyncPipe],
  templateUrl: './header-component.html',
  styleUrl: './header-component.scss',
})
export class HeaderComponent {
  subjects$!: Observable<Subject[]>;

  constructor(private subjectService: SubjectService,
              private store: Store<{counter: number}>)
  {
      this.subjects$ = this.store.select(selectAllSubjects);
  }
}
