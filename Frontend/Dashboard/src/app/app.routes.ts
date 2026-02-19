import { Routes } from '@angular/router';
import { MainLayoutComponent } from './Shared/Components/main-layout-component/main-layout-component';
import { CourseLayoutComponent } from './Shared/Components/Course-layout-component/Course-layout-component';

export const routes: Routes = [
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
    path: 'course/:id',
    component: CourseLayoutComponent,
    children: [
      {
        path: '',
        loadChildren: () => import('./Features/Courses/courses.routes').then(o => o.CourseRoutes)
      }
    ]
  }
];
