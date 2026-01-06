import { Component, inject, ChangeDetectionStrategy } from "@angular/core";
import { I18nService } from "../i18n.service";

/**
 * Language Switcher Component
 *
 * Displays buttons to switch between supported languages (DE/EN)
 * Highlights the currently active language
 *
 * Usage:
 * ```html
 * <lib-language-switcher />
 * ```
 */
@Component({
  selector: "lib-language-switcher",
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="language-switcher">
      @for (lang of i18n.supportedLanguages; track lang) {
        <button
          type="button"
          class="lang-btn"
          [class.active]="i18n.currentLanguage() === lang"
          [attr.aria-pressed]="i18n.currentLanguage() === lang"
          [attr.aria-label]="i18n.getLanguageName(lang)"
          (click)="i18n.setLanguage(lang)"
        >
          {{ lang.toUpperCase() }}
        </button>
      }
    </div>
  `,
  styles: `
    .language-switcher {
      display: flex;
      gap: 0.25rem;
      align-items: center;
    }

    .lang-btn {
      padding: 0.375rem 0.625rem;
      font-size: 0.75rem;
      font-weight: 600;
      letter-spacing: 0.025em;
      color: var(--color-text-secondary, #6b7280);
      background: transparent;
      border: 1px solid var(--color-border, #e5e7eb);
      border-radius: 0.375rem;
      cursor: pointer;
      transition: all 0.15s ease;
    }

    .lang-btn:hover {
      color: var(--color-text-primary, #374151);
      background: var(--color-bg-hover, #f3f4f6);
      border-color: var(--color-border-hover, #d1d5db);
    }

    .lang-btn:focus-visible {
      outline: 2px solid var(--color-primary, #f97316);
      outline-offset: 2px;
    }

    .lang-btn.active {
      color: var(--color-primary, #f97316);
      background: var(--color-primary-light, #fff7ed);
      border-color: var(--color-primary, #f97316);
    }
  `,
})
export class LanguageSwitcherComponent {
  protected readonly i18n = inject(I18nService);
}
