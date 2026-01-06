import { InjectionToken } from "@angular/core";

/**
 * Interface for API configuration
 * Implemented by each application's ConfigService
 */
export interface ApiConfig {
  readonly apiUrl: string;
}

/**
 * Default API URL used as fallback
 */
const DEFAULT_API_URL = "http://localhost:5000";

/**
 * Injection token for API configuration
 * Each application should provide their ConfigService using this token
 *
 * The token has a default factory that provides a fallback API URL,
 * ensuring services can be instantiated even before APP_INITIALIZER runs.
 *
 * @example
 * // In app.config.ts
 * providers: [
 *   { provide: API_CONFIG, useExisting: ConfigService }
 * ]
 */
export const API_CONFIG = new InjectionToken<ApiConfig>("API_CONFIG", {
  providedIn: "root",
  factory: () => ({ apiUrl: DEFAULT_API_URL }),
});
