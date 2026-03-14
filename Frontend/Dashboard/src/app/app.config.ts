import { provideHttpClient } from '@angular/common/http';
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideStore } from '@ngrx/store';
import { counterReducer } from './Core/Store/reducers/counter.reducer';
import { subjectReducer } from './Core/Store/reducers/subject.reducer';
import { provideEffects } from '@ngrx/effects';
import { CounterEffects } from './Core/Store/effects/counter.effects';
import { SubjectEffects } from './Core/Store/effects/subject.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    provideStore({
      counter: counterReducer,
      subjects: subjectReducer
    }),
    provideEffects([
      CounterEffects,
      SubjectEffects
    ])
  ]
};
