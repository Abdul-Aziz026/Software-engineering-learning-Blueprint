import { Routes } from '@angular/router';
import { MainLayoutComponent } from './Layouts/main-layout-component/main-layout-component';

export const routes: Routes = [
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./Features/Auth/Pages/reset-password/reset-password.component')
        .then(m => m.ResetPasswordComponent)
  },
  {
    path: 'confirm-subscription',
    loadComponent: () =>
      import('./Features/Subscribers/Pages/confirm-subscription/confirm-subscription.component')
        .then(m => m.ConfirmSubscriptionComponent)
  },
  {
    path: 'unsubscribe',
    loadComponent: () =>
      import('./Features/Subscribers/Pages/unsubscribe/unsubscribe.component')
        .then(m => m.UnsubscribeComponent)
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
  }
];
