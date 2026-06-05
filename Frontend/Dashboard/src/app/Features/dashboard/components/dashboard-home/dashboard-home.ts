import { Component, OnInit, computed } from '@angular/core';
import { CommonModule, AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, map } from 'rxjs';
import { Subject } from '../../../Courses/Models/subject.model';
import { selectAllSubjects } from '../../../../Core/Store/selectors/subject.selectors';
import { loadSubjects } from '../../../../Core/Store/actions/subject.actions';
import { AuthService } from '../../../Auth/Services/auth.service';
import { AuthModalService } from '../../../Auth/Services/auth-modal.service';

@Component({
  selector: 'app-dashboard-home',
  standalone: true,
  imports: [CommonModule, AsyncPipe, RouterLink],
  templateUrl: './dashboard-home.html',
  styleUrl: './dashboard-home.scss',
})
export class DashboardHome implements OnInit {
  subjects$!: Observable<Subject[]>;
  featuredSubjects$!: Observable<Subject[]>;
  subjectCount$!: Observable<number>;

  greeting = computed(() => {
    const user = this.authService.currentUser();
    const hour = new Date().getHours();
    const tod = hour < 12 ? 'morning' : hour < 18 ? 'afternoon' : 'evening';
    return user ? `Good ${tod}, ${user.username}` : `Good ${tod}, learner`;
  });

  constructor(
    private store: Store,
    public authService: AuthService,
    private authModal: AuthModalService
  ) {
    this.subjects$ = this.store.select(selectAllSubjects);
    this.featuredSubjects$ = this.subjects$.pipe(map((list) => list.slice(0, 6)));
    this.subjectCount$ = this.subjects$.pipe(map((list) => list.length));
  }

  ngOnInit(): void {
    this.store.dispatch(loadSubjects());
  }

  openSignup(): void {
    this.authModal.open('signup');
  }

  openLogin(): void {
    this.authModal.open('login');
  }
}
