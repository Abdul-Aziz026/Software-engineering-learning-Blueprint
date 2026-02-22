import { Routes } from "@angular/router";

export const CourseRoutes: Routes = [
    {
        path: 'lesson/create',
        loadComponent: () => import('./Components/lesson-create-component/lesson-create-component').then(o => o.LessonCreateComponent)
    },
    {
        path: ':id',
        loadComponent: () => import('./Components/home-lesson-component/home-lesson-component').then(o => o.HomeLessonComponent)
    },
    {
        path: ':id/lesson/:lessonId',
        loadComponent: () => import('./Components/lesson-component/lesson-component').then(o => o.LessonComponent)
    }
];
