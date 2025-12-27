import { initFederation } from '@angular-architects/native-federation';

(async () => {
  try {
    await initFederation('/assets/federation.manifest.json');
  } catch (err) {
    console.error('Federation initialization error:', err);
  }

  try {
    await import('./bootstrap');
  } catch (err) {
    console.error('Bootstrap error:', err);
  }
})();
