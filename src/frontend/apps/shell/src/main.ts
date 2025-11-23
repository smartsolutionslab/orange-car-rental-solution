import { initFederation } from '@angular-architects/native-federation';

initFederation('/assets/federation.manifest.json')
  .catch(err => console.error('Federation initialization error:', err))
  .then(_ => import('./bootstrap'))
  .catch(err => console.error('Bootstrap error:', err));
