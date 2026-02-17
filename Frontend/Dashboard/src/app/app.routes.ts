import { Routes } from '@angular/router';
import { LayoutComponent } from './Shared/Components/layout-component/layout-component';
import { DashboardHome } from './Features/dashboard/components/dashboard-home/dashboard-home';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
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
