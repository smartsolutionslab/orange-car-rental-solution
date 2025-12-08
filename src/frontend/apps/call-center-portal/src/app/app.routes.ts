import type { Routes } from '@angular/router';
// Import locale registration as side effect - runs when routes are loaded
import './locale-de';
import { VehiclesComponent } from './pages/vehicles/vehicles.component';
import { ReservationsComponent } from './pages/reservations/reservations.component';
import { CustomersComponent } from './pages/customers/customers.component';
import { LocationsComponent } from './pages/locations/locations.component';
import { ContactComponent } from './pages/contact/contact.component';
import { authGuard } from '@orange-car-rental/auth';

export const routes: Routes = [
  {
    path: '',
    component: VehiclesComponent,
    canActivate: [authGuard]
  },
  {
    path: 'reservations',
    component: ReservationsComponent,
    canActivate: [authGuard]
  },
  {
    path: 'customers',
    component: CustomersComponent,
    canActivate: [authGuard]
  },
  {
    path: 'locations',
    component: LocationsComponent,
    canActivate: [authGuard]
  },
  {
    path: 'contact',
    component: ContactComponent,
    canActivate: [authGuard]
  }
];
