import { Routes } from '@angular/router';
import { MainLayoutComponent } from './Shared/Components/main-layout-component/main-layout-component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadChildren: () => import('./Features/dashboard/dashboard.routes').then(o => o.DashboardRoutes)
      },
      {
        path: '**',
        redirectTo: 'dashboard'
      }
    ]
  }
];
