import { Routes } from '@angular/router';
import { loadRemoteModule } from '@angular-architects/native-federation';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'vehicles',
    loadChildren: () =>
      loadRemoteModule('publicPortal', './Routes')
        .then(m => m.routes)
        .catch(err => {
          console.error('Failed to load public portal module:', err);
          return import('./error/module-load-error.component').then(m => [{
            path: '',
            component: m.ModuleLoadErrorComponent
          }]);
        })
  },
  {
    path: 'booking',
    loadChildren: () =>
      loadRemoteModule('publicPortal', './Routes')
        .then(m => m.routes)
        .catch(err => {
          console.error('Failed to load public portal module:', err);
          return import('./error/module-load-error.component').then(m => [{
            path: '',
            component: m.ModuleLoadErrorComponent
          }]);
        })
  },
  {
    path: 'admin',
    loadChildren: () =>
      loadRemoteModule('callCenterPortal', './Routes')
        .then(m => m.routes)
        .catch(err => {
          console.error('Failed to load call center portal module:', err);
          return import('./error/module-load-error.component').then(m => [{
            path: '',
            component: m.ModuleLoadErrorComponent
          }]);
        })
  },
  {
    path: '**',
    redirectTo: ''
  }
];
