import { createAction, props } from "@ngrx/store";

export const init = createAction('[Counter Action] Init');

export const set = createAction(
    '[Counter Action] Set',
    props<{value: number}>()
);

export const increment = createAction(
    '[Counter Action] Increment',
    props<{value: number}>()
);

export const decrement = createAction(
    '[Counter Action] Decrement',
    props<{value: number}>()
)