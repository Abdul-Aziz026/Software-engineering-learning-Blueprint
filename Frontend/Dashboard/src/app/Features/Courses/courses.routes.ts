import { Routes } from "@angular/router";

export const CourseRoutes: Routes = [
    {
        path: ':id',
        loadComponent: () => import('./Components/home-lesson-component/home-lesson-component').then(o => o.HomeLessonComponent)
    },
    {
        path: ':id/lesson/:lessonId',
        loadComponent: () => import('./Components/lesson-component/lesson-component').then(o => o.LessonComponent)
    }
];
