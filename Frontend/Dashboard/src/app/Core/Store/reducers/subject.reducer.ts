import { createReducer, on } from "@ngrx/store";
import { Subject } from "../../../Features/Courses/Models/subject.model";
import * as SubjectActions from "../actions/subject.actions";

export interface SubjectState {
    subjects: Subject[];
    loading: boolean;
    error: string | null;
}

const initialState: SubjectState = {
    subjects: [],
    loading: false,
    error: null
};

export const subjectReducer = createReducer(
    initialState,

    // Load Subjects
    on(SubjectActions.loadSubjects, (state) => ({ ...state, loading: true, error: null })),
    on(SubjectActions.loadSubjectsSuccess, (state, { subjects }) => ({
        ...state,
        subjects,
        loading: false
    })),
    on(SubjectActions.loadSubjectsFailure, (state, { error }) => ({
        ...state,
        loading: false,
        error
    })),

    // Create Subject
    on(SubjectActions.createSubjectSuccess, (state) => ({
        ...state,
        loading: true
    })),
    on(SubjectActions.createSubjectFailure, (state, { error }) => ({
        ...state,
        error
    })),

    // Update Subject
    on(SubjectActions.updateSubjectSuccess, (state) => ({
        ...state,
        loading: true
    })),
    on(SubjectActions.updateSubjectFailure, (state, { error }) => ({
        ...state,
        error
    })),

    // Delete Subject
    on(SubjectActions.deleteSubjectSuccess, (state) => ({
        ...state,
        loading: true
    })),
    on(SubjectActions.deleteSubjectFailure, (state, { error }) => ({
        ...state,
        error
    }))
);