import { inject, Injectable } from "@angular/core";
import { Actions, createEffect, ofType } from "@ngrx/effects";
import { catchError, map, mergeMap, of, switchMap } from "rxjs";
import { SubjectService } from "../../../Features/Courses/Services/subject.service";
import * as SubjectActions from "../actions/subject.actions";

@Injectable()
export class SubjectEffects {
    private actions$ = inject(Actions);
    private subjectService = inject(SubjectService);

    loadSubjects$ = createEffect(() =>
        this.actions$.pipe(
            ofType(SubjectActions.loadSubjects),
            mergeMap(() =>
                this.subjectService.getAllSubjects().pipe(
                    map((subjects) => SubjectActions.loadSubjectsSuccess({ subjects })),
                    catchError((error) => of(SubjectActions.loadSubjectsFailure({ error: error.message })))
                )
            )
        )
    );

    createSubject$ = createEffect(() =>
        this.actions$.pipe(
            ofType(SubjectActions.createSubject),
            mergeMap(({ subject }) =>
                this.subjectService.createSubject(subject).pipe(
                    switchMap(() => [
                        SubjectActions.createSubjectSuccess(),
                        SubjectActions.loadSubjects()
                    ]),
                    catchError((error) => of(SubjectActions.createSubjectFailure({ error: error.message })))
                )
            )
        )
    );

    updateSubject$ = createEffect(() =>
        this.actions$.pipe(
            ofType(SubjectActions.updateSubject),
            mergeMap(({ id, subject }) =>
                this.subjectService.updateSubject(id, subject).pipe(
                    switchMap(() => [
                        SubjectActions.updateSubjectSuccess(),
                        SubjectActions.loadSubjects()
                    ]),
                    catchError((error) => of(SubjectActions.updateSubjectFailure({ error: error.message })))
                )
            )
        )
    );

    deleteSubject$ = createEffect(() =>
        this.actions$.pipe(
            ofType(SubjectActions.deleteSubject),
            mergeMap(({ id }) =>
                this.subjectService.deleteSubject(id).pipe(
                    switchMap(() => [
                        SubjectActions.deleteSubjectSuccess(),
                        SubjectActions.loadSubjects()
                    ]),
                    catchError((error) => of(SubjectActions.deleteSubjectFailure({ error: error.message })))
                )
            )
        )
    );
}