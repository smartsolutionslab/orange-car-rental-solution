import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl } from '@angular/forms';
import { IconComponent } from '../icon';

/**
 * Custom validation message configuration
 */
export interface ValidationMessageConfig {
  /** The error key (e.g., 'required', 'email', 'minlength') */
  key: string;
  /** The message to display, or a function that receives error params */
  message: string | ((params: Record<string, unknown>) => string);
}

/**
 * Default German validation messages
 */
export const DEFAULT_VALIDATION_MESSAGES: ValidationMessageConfig[] = [
  { key: 'required', message: 'Dieses Feld ist erforderlich' },
  { key: 'email', message: 'Bitte geben Sie eine gültige E-Mail-Adresse ein' },
  { key: 'minlength', message: (p) => `Mindestens ${p['requiredLength']} Zeichen erforderlich` },
  { key: 'maxlength', message: (p) => `Maximal ${p['requiredLength']} Zeichen erlaubt` },
  { key: 'min', message: (p) => `Der Wert muss mindestens ${p['min']} sein` },
  { key: 'max', message: (p) => `Der Wert darf maximal ${p['max']} sein` },
  { key: 'pattern', message: 'Ungültiges Format' },
  { key: 'weakPassword', message: 'Passwort muss Groß-/Kleinbuchstaben, Zahlen und Sonderzeichen enthalten' },
  { key: 'passwordMismatch', message: 'Passwörter stimmen nicht überein' },
  { key: 'underage', message: (p) => `Sie müssen mindestens ${p['minAge']} Jahre alt sein` },
  { key: 'pastDate', message: 'Das Datum darf nicht in der Vergangenheit liegen' },
  { key: 'invalidPhone', message: 'Bitte geben Sie eine gültige Telefonnummer ein' },
  { key: 'invalidPostalCode', message: 'Bitte geben Sie eine gültige Postleitzahl ein' },
];

/**
 * Validation Messages Component
 *
 * Displays validation error messages for a form control.
 * Automatically detects errors and shows appropriate messages.
 *
 * @example
 * <ocr-validation-messages [control]="form.get('email')" />
 *
 * @example
 * <!-- With custom messages -->
 * <ocr-validation-messages
 *   [control]="form.get('password')"
 *   [messages]="customMessages"
 *   [showOnlyWhenTouched]="true"
 * />
 */
@Component({
  selector: 'ocr-validation-messages',
  standalone: true,
  imports: [CommonModule, IconComponent],
  template: `
    @if (visibleErrors().length > 0) {
      <div
        class="validation-messages"
        [class.validation-messages--single]="showFirst()"
        role="alert"
        aria-live="polite"
      >
        @for (error of visibleErrors(); track error.key) {
          <div class="validation-message">
            @if (showIcon()) {
              <lib-icon name="alert-circle" size="xs" class="validation-icon" />
            }
            <span class="validation-text">{{ error.message }}</span>
          </div>
        }
      </div>
    }
  `,
  styles: [`
    .validation-messages {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
      margin-top: 0.25rem;
    }

    .validation-message {
      display: flex;
      align-items: flex-start;
      gap: 0.375rem;
      font-size: 0.75rem;
      color: #ef4444;
      line-height: 1.25;
    }

    .validation-icon {
      flex-shrink: 0;
      margin-top: 0.0625rem;
    }

    .validation-text {
      flex: 1;
    }

    /* Animation for appearing */
    .validation-messages {
      animation: fadeIn 0.15s ease-out;
    }

    @keyframes fadeIn {
      from {
        opacity: 0;
        transform: translateY(-4px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }
  `]
})
export class ValidationMessagesComponent {
  /** The form control to display errors for */
  readonly control = input<AbstractControl | null>(null);

  /** Custom validation messages (merged with defaults) */
  readonly messages = input<ValidationMessageConfig[]>([]);

  /** Only show errors when control is touched */
  readonly showOnlyWhenTouched = input(true);

  /** Only show errors when control is dirty */
  readonly showOnlyWhenDirty = input(false);

  /** Show only the first error */
  readonly showFirst = input(true);

  /** Show error icon */
  readonly showIcon = input(true);

  /** Computed visible errors */
  readonly visibleErrors = computed(() => {
    const ctrl = this.control();
    if (!ctrl) return [];

    // Check visibility conditions
    if (this.showOnlyWhenTouched() && !ctrl.touched) return [];
    if (this.showOnlyWhenDirty() && !ctrl.dirty) return [];
    if (!ctrl.errors) return [];

    // Get all error keys
    const errorKeys = Object.keys(ctrl.errors);
    const allMessages = [...DEFAULT_VALIDATION_MESSAGES, ...this.messages()];

    // Map errors to messages
    const errors = errorKeys.map(key => {
      const errorValue = ctrl.errors![key];
      const config = allMessages.find(m => m.key === key);

      let message: string;
      if (config) {
        message = typeof config.message === 'function'
          ? config.message(errorValue)
          : config.message;
      } else {
        // Fallback for unknown errors
        message = `Validierungsfehler: ${key}`;
      }

      return { key, message };
    });

    // Return first or all errors
    return this.showFirst() ? errors.slice(0, 1) : errors;
  });
}
