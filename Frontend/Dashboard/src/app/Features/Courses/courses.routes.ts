import { Routes } from "@angular/router";

export const CourseRoutes: Routes = [
    {
        path: ':id',
        loadComponent: () => import('./Components/lesson-component/lesson-component').then(o => o.LessonComponent)
    }
];
