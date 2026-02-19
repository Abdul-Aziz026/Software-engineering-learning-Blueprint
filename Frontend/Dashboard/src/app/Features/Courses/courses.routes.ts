import { Routes } from "@angular/router";

export const CourseRoutes: Routes = [
    {
        path: 'course/:id',
        loadComponent: () => import('./Components/course-component/course-component').then(o => o.CourseComponent)
    }
];