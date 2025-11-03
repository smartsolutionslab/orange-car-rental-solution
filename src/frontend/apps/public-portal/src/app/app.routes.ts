import { Routes } from '@angular/router';
import { VehicleListComponent } from './pages/vehicle-list/vehicle-list.component';
import { BookingComponent } from './pages/booking/booking.component';
import { ConfirmationComponent } from './pages/confirmation/confirmation.component';

export const routes: Routes = [
  {
    path: '',
    component: VehicleListComponent
  },
  {
    path: 'booking',
    component: BookingComponent
  },
  {
    path: 'confirmation',
    component: ConfirmationComponent
  }
];
