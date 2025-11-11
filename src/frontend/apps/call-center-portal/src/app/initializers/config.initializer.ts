import { HttpClient } from '@angular/common/http';
import { ConfigService } from '../services/config.service';
import { firstValueFrom } from 'rxjs';
import { catchError, of } from 'rxjs';

/**
 * Factory function for APP_INITIALIZER
 * Loads configuration from config.json before app starts
 */
export function initializeApp(http: HttpClient, configService: ConfigService): () => Promise<void> {
  return () => {
    console.log('[APP_INITIALIZER] Starting configuration load...');
    return firstValueFrom(
      http.get('/config.json').pipe(
        catchError(error => {
          console.error('[APP_INITIALIZER] Failed to load config.json:', error);
          console.log('[APP_INITIALIZER] Using default configuration');
          return of({ apiUrl: 'http://localhost:5002' });
        })
      )
    ).then((config: any) => {
      console.log('[APP_INITIALIZER] Configuration loaded successfully:', config);
      configService.setConfig(config);
      console.log('[APP_INITIALIZER] Configuration applied to service');
    }).catch(error => {
      console.error('[APP_INITIALIZER] Unexpected error:', error);
      // Set default config even if something unexpected happens
      configService.setConfig({ apiUrl: 'http://localhost:5002' });
      console.log('[APP_INITIALIZER] Applied default configuration due to error');
    });
  };
}
