import { InjectionToken } from '@angular/core';

/**
 * Interface for API configuration
 * Implemented by each application's ConfigService
 */
export interface ApiConfig {
  readonly apiUrl: string;
}

/**
 * Injection token for API configuration
 * Each application must provide their ConfigService using this token
 *
 * @example
 * // In app.config.ts
 * providers: [
 *   { provide: API_CONFIG, useExisting: ConfigService }
 * ]
 */
export const API_CONFIG = new InjectionToken<ApiConfig>('API_CONFIG');
