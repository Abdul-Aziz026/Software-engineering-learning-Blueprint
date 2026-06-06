import { Routes } from '@angular/router';
import { MainLayoutComponent } from './Layouts/main-layout-component/main-layout-component';
import { CourseLayoutComponent } from './Layouts/course-layout-component/course-layout-component'; 

export const routes: Routes = [
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./Features/Auth/Pages/reset-password/reset-password.component')
        .then(m => m.ResetPasswordComponent)
  },
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        loadChildren: () => import('./Features/dashboard/dashboard.routes').then(o => o.DashboardRoutes)
      }
    ]
  },
  {
    path: 'course',
    component: CourseLayoutComponent,
    children: [
      {
        path: '',
        loadChildren: () => import('./Features/Courses/courses.routes').then(o => o.CourseRoutes)
      }
    ]
  }
];
