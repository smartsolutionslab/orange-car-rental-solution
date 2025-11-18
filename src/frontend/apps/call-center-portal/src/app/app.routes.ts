import { Routes } from '@angular/router';
import { VehiclesComponent } from './pages/vehicles/vehicles.component';
import { ReservationsComponent } from './pages/reservations/reservations.component';
import { CustomersComponent } from './pages/customers/customers.component';
import { LocationsComponent } from './pages/locations/locations.component';
import { ContactComponent } from './pages/contact/contact.component';
import { authGuard } from './guards/auth.guard';

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
