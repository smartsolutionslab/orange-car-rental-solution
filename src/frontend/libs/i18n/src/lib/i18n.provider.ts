import { APP_INITIALIZER, importProvidersFrom, type EnvironmentProviders, type Provider } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { I18nService } from './i18n.service';

/**
 * Factory function to initialize the I18nService
 */
function initializeI18n(i18nService: I18nService): () => void {
  return () => i18nService.initialize();
}

/**
 * Provides i18n configuration for the application
 *
 * Usage in app.config.ts:
 * ```typescript
 * export const appConfig: ApplicationConfig = {
 *   providers: [
 *     provideHttpClient(),
 *     ...provideI18n(),
 *   ],
 * };
 * ```
 */
export function provideI18n(): (Provider | EnvironmentProviders)[] {
  return [
    importProvidersFrom(
      TranslateModule.forRoot({
        defaultLanguage: 'de',
      })
    ),
    ...provideTranslateHttpLoader({
      prefix: './assets/i18n/',
      suffix: '.json',
    }),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeI18n,
      deps: [I18nService],
      multi: true,
    },
  ];
}
