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
    return firstValueFrom(
      http.get('/config.json').pipe(
        catchError(error => {
          console.warn('Failed to load config.json, using defaults:', error);
          return of({ apiUrl: 'http://localhost:5002' });
        })
      )
    ).then((config: any) => {
      console.log('Loaded configuration:', config);
      configService.setConfig(config);
    });
  };
}
