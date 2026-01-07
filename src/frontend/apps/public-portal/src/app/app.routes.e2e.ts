import type { Routes } from '@angular/router';
// Import locale registration as side effect - runs when routes are loaded
import './locale-de';
import { authGuard } from '@orange-car-rental/auth';

/**
 * E2E Routes Configuration
 *
 * Uses lazy loading for ALL routes to avoid class field initializer issues.
 * When components are eagerly imported, their inject() calls in class field
 * initializers execute at module load time, before Angular's injection context
 * is available, causing NG0201 errors.
 *
 * Lazy loading defers component class definition until route navigation,
 * when the injection context is ready.
 */
export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/vehicle-list/vehicle-list.component').then((m) => m.VehicleListComponent),
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/register/register.component').then((m) => m.RegisterComponent),
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./pages/forgot-password/forgot-password.component').then(
        (m) => m.ForgotPasswordComponent,
      ),
  },
  {
    path: 'booking',
    loadComponent: () =>
      import('./pages/booking/booking.component').then((m) => m.BookingComponent),
    canActivate: [authGuard],
  },
  {
    path: 'confirmation',
    loadComponent: () =>
      import('./pages/confirmation/confirmation.component').then((m) => m.ConfirmationComponent),
    canActivate: [authGuard],
  },
  {
    path: 'my-bookings',
    loadComponent: () =>
      import('./pages/booking-history/booking-history.component').then(
        (m) => m.BookingHistoryComponent,
      ),
    canActivate: [authGuard],
  },
];
