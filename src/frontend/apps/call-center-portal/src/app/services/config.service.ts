import { Injectable } from '@angular/core';
import type { AppConfig } from '../types';
export type { AppConfig } from '../types';

/**
 * Configuration service that holds runtime configuration
 */
@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private config: AppConfig = { apiUrl: 'http://localhost:5002' };

  get apiUrl(): string {
    return this.config.apiUrl;
  }

  setConfig(config: AppConfig): void {
    this.config = config;
  }
}
