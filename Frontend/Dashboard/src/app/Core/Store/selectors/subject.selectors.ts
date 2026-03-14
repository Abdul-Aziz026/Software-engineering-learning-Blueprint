import { createFeatureSelector, createSelector } from '@ngrx/store';
import { SubjectState } from '../reducers/subject.reducer';

export const selectSubjectState = createFeatureSelector<SubjectState>('subjects');

export const selectAllSubjects = createSelector(
    selectSubjectState,
    (state: SubjectState) => state.subjects
);

export const selectSubjectsLoading = createSelector(
    selectSubjectState,
    (state: SubjectState) => state.loading
);

export const selectSubjectsError = createSelector(
    selectSubjectState,
    (state: SubjectState) => state.error
);
