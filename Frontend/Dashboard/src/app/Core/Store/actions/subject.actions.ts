import { createAction, props } from '@ngrx/store';
import { Subject } from '../../../Features/Courses/Models/subject.model';

// Load Subjects
export const loadSubjects = createAction('[Subject] Load Subjects');
export const loadSubjectsSuccess = createAction(
    '[Subject] Load Subjects Success',
    props<{ subjects: Subject[] }>()
);
export const loadSubjectsFailure = createAction(
    '[Subject] Load Subjects Failure',
    props<{ error: string }>()
);

// Create Subject
export const createSubject = createAction(
    '[Subject] Create Subject',
    props<{ subject: Subject }>()
);
export const createSubjectSuccess = createAction(
    '[Subject] Create Subject Success'
);
export const createSubjectFailure = createAction(
    '[Subject] Create Subject Failure',
    props<{ error: string }>()
);

// Update Subject
export const updateSubject = createAction(
    '[Subject] Update Subject',
    props<{ id: string; subject: Subject }>()
);
export const updateSubjectSuccess = createAction(
    '[Subject] Update Subject Success'
);
export const updateSubjectFailure = createAction(
    '[Subject] Update Subject Failure',
    props<{ error: string }>()
);

// Delete Subject
export const deleteSubject = createAction(
    '[Subject] Delete Subject',
    props<{ id: string }>()
);
export const deleteSubjectSuccess = createAction(
    '[Subject] Delete Subject Success'
);
export const deleteSubjectFailure = createAction(
    '[Subject] Delete Subject Failure',
    props<{ error: string }>()
);