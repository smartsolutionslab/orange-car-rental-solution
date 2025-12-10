import type { Routes } from '@angular/router';
// Import locale registration as side effect - runs when routes are loaded
import './locale-de';
import { VehiclesComponent } from './pages/vehicles/vehicles.component';
import { ReservationsComponent } from './pages/reservations/reservations.component';
import { CustomersComponent } from './pages/customers/customers.component';
import { LocationsComponent } from './pages/locations/locations.component';
import { ContactComponent } from './pages/contact/contact.component';
import { agentGuard } from '@orange-car-rental/auth';

/**
 * Call Center Portal Routes
 * All routes require agent role (call-center-agent, call-center-supervisor, or admin)
 */
export const routes: Routes = [
  {
    path: '',
    component: VehiclesComponent,
    canActivate: [agentGuard]
  },
  {
    path: 'reservations',
    component: ReservationsComponent,
    canActivate: [agentGuard]
  },
  {
    path: 'customers',
    component: CustomersComponent,
    canActivate: [agentGuard]
  },
  {
    path: 'locations',
    component: LocationsComponent,
    canActivate: [agentGuard]
  },
  {
    path: 'contact',
    component: ContactComponent,
    canActivate: [agentGuard]
  }
];
