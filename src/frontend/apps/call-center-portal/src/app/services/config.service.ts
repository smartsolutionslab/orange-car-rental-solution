import { Injectable } from '@angular/core';
import type { ApiConfig } from '@orange-car-rental/shared';

/**
 * Configuration service that holds runtime configuration
 * Implements ApiConfig interface for use with shared services
 */
@Injectable({
  providedIn: 'root',
})
export class ConfigService implements ApiConfig {
  private config: ApiConfig = { apiUrl: 'http://localhost:5002' };

  get apiUrl(): string {
    return this.config.apiUrl;
  }

  setConfig(config: ApiConfig): void {
    this.config = config;
  }
}
