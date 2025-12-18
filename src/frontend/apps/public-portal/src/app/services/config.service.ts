import { Injectable } from '@angular/core';
import type { ApiConfig } from '@orange-car-rental/shared';

/**
 * Configuration service that holds runtime configuration
 * Loaded via APP_INITIALIZER before the app starts
 * Implements ApiConfig interface for use with shared services
 */
@Injectable({
  providedIn: 'root',
})
export class ConfigService implements ApiConfig {
  private config: ApiConfig = { apiUrl: '' };

  get apiUrl(): string {
    return this.config.apiUrl;
  }

  setConfig(config: ApiConfig): void {
    this.config = config;
  }
}
