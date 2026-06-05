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

    // Create / Update / Delete — the effect re-dispatches loadSubjects on success,
    // so loading is driven by the load cycle. Only track errors here.
    on(SubjectActions.createSubjectFailure,
       SubjectActions.updateSubjectFailure,
       SubjectActions.deleteSubjectFailure,
       (state, { error }) => ({ ...state, error }))
);
