import { Routes } from '@angular/router';
import { loadRemoteModule } from '@angular-architects/native-federation';

const loadPublicPortalRoutes = async () => {
  try {
    const m = await loadRemoteModule('publicPortal', './Routes');
    return m.routes;
  } catch (err) {
    console.error('Failed to load public portal module:', err);
    const errorModule = await import('./error/module-load-error.component');
    return [{ path: '', component: errorModule.ModuleLoadErrorComponent }];
  }
};

const loadCallCenterPortalRoutes = async () => {
  try {
    const m = await loadRemoteModule('callCenterPortal', './Routes');
    return m.routes;
  } catch (err) {
    console.error('Failed to load call center portal module:', err);
    const errorModule = await import('./error/module-load-error.component');
    return [{ path: '', component: errorModule.ModuleLoadErrorComponent }];
  }
};

export const routes: Routes = [
  // Public portal routes at root (vehicle search is home page)
  {
    path: '',
    loadChildren: loadPublicPortalRoutes,
  },
  // Call center portal at /admin (requires agent role)
  {
    path: 'admin',
    loadChildren: loadCallCenterPortalRoutes,
  },
  // Catch-all redirect to home
  {
    path: '**',
    redirectTo: '',
  },
];
