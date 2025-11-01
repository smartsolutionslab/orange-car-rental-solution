import { Routes } from '@angular/router';
import { VehiclesComponent } from './pages/vehicles/vehicles.component';
import { ReservationsComponent } from './pages/reservations/reservations.component';
import { LocationsComponent } from './pages/locations/locations.component';
import { ContactComponent } from './pages/contact/contact.component';

export const routes: Routes = [
  {
    path: '',
    component: VehiclesComponent
  },
  {
    path: 'reservations',
    component: ReservationsComponent
  },
  {
    path: 'locations',
    component: LocationsComponent
  },
  {
    path: 'contact',
    component: ContactComponent
  }
];
