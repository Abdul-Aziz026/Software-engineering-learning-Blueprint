import { Routes } from "@angular/router";

export const DashboardRoutes: Routes = [
    {
        path: '',
        loadComponent: () => import("./components/dashboard-home/dashboard-home").then(o => o.DashboardHome)
    }
];