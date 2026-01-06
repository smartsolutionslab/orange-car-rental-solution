import { Injectable, inject, signal } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";

/**
 * Supported languages in the application
 */
export type SupportedLanguage = "de" | "en";

/**
 * Language storage key for localStorage
 */
const LANGUAGE_STORAGE_KEY = "ocr-preferred-language";

/**
 * Default language (German)
 */
const DEFAULT_LANGUAGE: SupportedLanguage = "de";

/**
 * List of supported languages
 */
const SUPPORTED_LANGUAGES: readonly SupportedLanguage[] = ["de", "en"] as const;

/**
 * Language display names
 */
export const LANGUAGE_NAMES: Readonly<Record<SupportedLanguage, string>> = {
  de: "Deutsch",
  en: "English",
} as const;

/**
 * I18nService - Wrapper around ngx-translate with Angular signals
 *
 * Provides:
 * - Language initialization from localStorage or browser preference
 * - Reactive language state via signals
 * - Language persistence to localStorage
 * - Type-safe language switching
 */
@Injectable({ providedIn: "root" })
export class I18nService {
  private readonly translate = inject(TranslateService);

  private readonly _currentLanguage =
    signal<SupportedLanguage>(DEFAULT_LANGUAGE);

  /**
   * Current language as a readonly signal
   */
  readonly currentLanguage = this._currentLanguage.asReadonly();

  /**
   * List of supported languages
   */
  readonly supportedLanguages = SUPPORTED_LANGUAGES;

  /**
   * Default language
   */
  readonly defaultLanguage = DEFAULT_LANGUAGE;

  /**
   * Initialize the i18n service
   * Should be called once during app initialization
   */
  initialize(): void {
    this.translate.setDefaultLang(DEFAULT_LANGUAGE);

    // Try to get stored preference
    const stored = localStorage.getItem(
      LANGUAGE_STORAGE_KEY,
    ) as SupportedLanguage | null;

    // Try browser language as fallback
    const browserLang = this.translate.getBrowserLang() as
      | SupportedLanguage
      | undefined;

    // Determine language: stored > browser (if supported) > default
    let lang: SupportedLanguage = DEFAULT_LANGUAGE;
    if (stored && SUPPORTED_LANGUAGES.includes(stored)) {
      lang = stored;
    } else if (browserLang && SUPPORTED_LANGUAGES.includes(browserLang)) {
      lang = browserLang;
    }

    this.setLanguage(lang);
  }

  /**
   * Set the current language
   * @param lang - The language code to switch to
   */
  setLanguage(lang: SupportedLanguage): void {
    if (!SUPPORTED_LANGUAGES.includes(lang)) {
      console.warn(
        `Unsupported language: ${lang}. Falling back to ${DEFAULT_LANGUAGE}`,
      );
      lang = DEFAULT_LANGUAGE;
    }

    this.translate.use(lang);
    this._currentLanguage.set(lang);
    localStorage.setItem(LANGUAGE_STORAGE_KEY, lang);
    document.documentElement.lang = lang;
  }

  /**
   * Toggle between German and English
   */
  toggleLanguage(): void {
    const current = this._currentLanguage();
    const next: SupportedLanguage = current === "de" ? "en" : "de";
    this.setLanguage(next);
  }

  /**
   * Get instant translation for a key
   * @param key - Translation key
   * @param params - Optional interpolation parameters
   */
  instant(key: string, params?: Record<string, unknown>): string {
    return this.translate.instant(key, params);
  }

  /**
   * Get the display name for a language
   */
  getLanguageName(lang: SupportedLanguage): string {
    return LANGUAGE_NAMES[lang];
  }
}
