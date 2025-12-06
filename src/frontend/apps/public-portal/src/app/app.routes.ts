import type { Routes } from '@angular/router';
// Import locale registration as side effect - runs when routes are loaded
import './locale-de';
import { VehicleListComponent } from './pages/vehicle-list/vehicle-list.component';
import { BookingComponent } from './pages/booking/booking.component';
import { ConfirmationComponent } from './pages/confirmation/confirmation.component';
import { BookingHistoryComponent } from './pages/booking-history/booking-history.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: VehicleListComponent
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'register',
    component: RegisterComponent
  },
  {
    path: 'forgot-password',
    component: ForgotPasswordComponent
  },
  {
    path: 'booking',
    component: BookingComponent,
    canActivate: [authGuard]
  },
  {
    path: 'confirmation',
    component: ConfirmationComponent,
    canActivate: [authGuard]
  },
  {
    path: 'my-bookings',
    component: BookingHistoryComponent
  }
];
