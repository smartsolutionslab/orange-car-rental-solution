import { HttpClient } from '@angular/common/http';
import type { ApiConfig } from '@orange-car-rental/shared';
import { ConfigService } from '../services/config.service';
import { firstValueFrom, catchError, of } from 'rxjs';

const DEFAULT_API_URL = 'http://localhost:5002';

/**
 * Factory function for APP_INITIALIZER
 * Loads configuration from config.json before app starts
 */
export function initializeApp(http: HttpClient, configService: ConfigService): () => Promise<void> {
  return async () => {
    try {
      const config = await firstValueFrom(
        http.get<ApiConfig>('/config.json').pipe(
          catchError(() => of({ apiUrl: DEFAULT_API_URL }))
        )
      );
      configService.setConfig(config);
    } catch {
      configService.setConfig({ apiUrl: DEFAULT_API_URL });
    }
  };
}
