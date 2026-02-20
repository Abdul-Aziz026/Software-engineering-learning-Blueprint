import { Routes } from "@angular/router";

export const DashboardRoutes: Routes = [
    {
        path: '',
        loadComponent: () => import("./components/dashboard-home/dashboard-home").then(o => o.DashboardHome)
    },
    {
        path: 'course',
        loadComponent: () => import('../Courses/Components/subjects-component/subjects-component').then(o => o.SubjectsComponent)
    },
];