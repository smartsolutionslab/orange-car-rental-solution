import { HttpClient } from '@angular/common/http';
import { ConfigService, AppConfig } from '../services/config.service';
import { firstValueFrom } from 'rxjs';

/**
 * Factory function for APP_INITIALIZER
 * Loads configuration from config.json before app starts
 */
export function initializeApp(http: HttpClient, configService: ConfigService): () => Promise<void> {
  return () => {
    return firstValueFrom(
      http.get<AppConfig>('/config.json')
    ).then((config: AppConfig) => {
      console.log('Loaded configuration:', config);
      configService.setConfig(config);
    }).catch(error => {
      console.error('Failed to load configuration:', error);
      // Set default config on error
      configService.setConfig({ apiUrl: 'http://localhost:5002' });
    });
  };
}
