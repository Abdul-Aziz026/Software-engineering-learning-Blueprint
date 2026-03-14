import { Actions, createEffect, ofType } from "@ngrx/effects";
import { decrement, increment, init, set } from "../actions/counter.actions";
import { of, switchMap, tap, withLatestFrom } from "rxjs";
import { inject, Injectable } from "@angular/core";
import { Store } from "@ngrx/store";
import { selectCount } from "../selectors/counter.selectors";

@Injectable()
export class CounterEffects {
    private action$ = inject(Actions);
    private store = inject(Store<{counter: number}>);

    saveCount = createEffect(
        () => this.action$.pipe(
            ofType(increment, decrement),
            withLatestFrom(this.store.select(selectCount)),
            tap(([action, counter]) => {
                console.log('counter effect action => ' + action + ' = ' + counter)
                localStorage.setItem('counter', counter.toString());
            })
        ),
        { dispatch: false }
    );

    loadCount = createEffect(
        () => this.action$.pipe(
            ofType(init),
            switchMap(() => {
                const storedCounter = localStorage.getItem('counter');
                if (storedCounter) {
                    return of(set({value: +storedCounter}))
                }
                return of(set({value: 0}));
            })
        )
    );

    // constructor() {}
}