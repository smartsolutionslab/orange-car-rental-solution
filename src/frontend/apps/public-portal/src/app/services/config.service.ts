import { Injectable } from '@angular/core';

export interface AppConfig {
  apiUrl: string;
}

/**
 * Configuration service that holds runtime configuration
 * Loaded via APP_INITIALIZER before the app starts
 */
@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private config: AppConfig = { apiUrl: '' };

  get apiUrl(): string {
    return this.config.apiUrl;
  }

  setConfig(config: AppConfig): void {
    this.config = config;
  }
}
