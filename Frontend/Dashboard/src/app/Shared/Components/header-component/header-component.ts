import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { SubjectService } from '../../../Features/Courses/Services/subject.service';
import { Subject } from '../../../Features/Courses/Models/subject.model';

@Component({
  selector: 'app-header-component',
  standalone: true,
  imports: [RouterLink, AsyncPipe],
  templateUrl: './header-component.html',
  styleUrl: './header-component.scss',
})
export class HeaderComponent implements OnInit {
  subjects$!: Observable<Subject[]>;

  constructor(private subjectService: SubjectService) { }

  ngOnInit(): void {
    this.subjects$ = this.subjectService.getAllSubjects();
  }
}
