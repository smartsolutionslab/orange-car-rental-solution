import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

/**
 * Configuration service
 * Loads runtime configuration from public/config.json
 */
@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private readonly http = inject(HttpClient);
  private config: { apiUrl: string } | null = null;

  get apiUrl(): string {
    return this.config?.apiUrl || 'http://localhost:5002';
  }

  async loadConfig(): Promise<void> {
    try {
      this.config = await this.http.get<{ apiUrl: string }>('/config.json').toPromise() as { apiUrl: string };
    } catch (error) {
      console.error('Failed to load config, using defaults', error);
      this.config = { apiUrl: 'http://localhost:5002' };
    }
  }
}
