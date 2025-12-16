import { FormGroup } from '@angular/forms';
import type { AbstractControl } from '@angular/forms';

/**
 * Validation message function type.
 * Uses Record<string, unknown> for params since Angular validators
 * return different param shapes for different error types.
 */
type ValidationMessageFn = (params: Record<string, unknown>) => string;

/**
 * German error messages for common validation errors.
 * Used by getFieldError to provide localized error messages.
 */
export const ValidationMessages: Record<string, string | ValidationMessageFn> = {
  required: 'Dieses Feld ist erforderlich',
  email: 'Bitte geben Sie eine gültige E-Mail-Adresse ein',
  minlength: (params) =>
    `Mindestens ${params['requiredLength']} Zeichen erforderlich`,
  maxlength: (params) =>
    `Maximal ${params['requiredLength']} Zeichen erlaubt`,
  min: (params) => `Der Wert muss mindestens ${params['min']} sein`,
  max: (params) => `Der Wert darf maximal ${params['max']} sein`,
  pattern: 'Ungültiges Format',
  weakPassword: 'Passwort muss Groß- und Kleinbuchstaben, Zahlen und Sonderzeichen enthalten',
  passwordMismatch: 'Passwörter stimmen nicht überein',
  underage: (params) =>
    `Sie müssen mindestens ${params['minAge']} Jahre alt sein`,
  pastDate: 'Das Datum darf nicht in der Vergangenheit liegen',
  dateAfter: 'Das Startdatum muss vor dem Enddatum liegen',
  invalidPhone: 'Bitte geben Sie eine gültige Telefonnummer ein',
  invalidPostalCode: 'Bitte geben Sie eine gültige Postleitzahl ein (5 Ziffern)',
  invalidLicensePlate: 'Bitte geben Sie ein gültiges Kennzeichen ein',
  noneSelected: 'Bitte wählen Sie mindestens eine Option',
  outOfRange: (params) =>
    `Der Wert muss zwischen ${params['min']} und ${params['max']} liegen`,
  notPositive: 'Bitte geben Sie einen positiven Wert ein',
  notANumber: 'Bitte geben Sie eine gültige Zahl ein',
};

/**
 * Form Helper Utilities
 *
 * Reusable functions for form manipulation and error handling.
 *
 * @example
 * ```typescript
 * import { FormHelpers, getFieldError } from '@orange-car-rental/shared';
 *
 * // Get error message for a field
 * const error = getFieldError(this.form.get('email'));
 *
 * // Mark all fields as touched
 * FormHelpers.markAllTouched(this.form);
 * ```
 */
export const FormHelpers = {
  /**
   * Mark all form controls as touched to trigger validation display
   * @param formGroup - The form group to mark
   */
  markAllTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      if (control instanceof FormGroup) {
        FormHelpers.markAllTouched(control);
      } else {
        control?.markAsTouched();
      }
    });
  },

  /**
   * Mark all form controls as untouched (pristine)
   * @param formGroup - The form group to reset
   */
  markAllUntouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      if (control instanceof FormGroup) {
        FormHelpers.markAllUntouched(control);
      } else {
        control?.markAsUntouched();
        control?.markAsPristine();
      }
    });
  },

  /**
   * Reset form to initial values and mark as pristine
   * @param formGroup - The form group to reset
   * @param initialValues - Optional initial values to reset to
   */
  resetForm(formGroup: FormGroup, initialValues?: Record<string, any>): void {
    if (initialValues) {
      formGroup.reset(initialValues);
    } else {
      formGroup.reset();
    }
    FormHelpers.markAllUntouched(formGroup);
  },

  /**
   * Check if a form has any dirty (modified) controls
   * @param formGroup - The form group to check
   */
  isDirty(formGroup: FormGroup): boolean {
    return Object.keys(formGroup.controls).some(key => {
      const control = formGroup.get(key);
      if (control instanceof FormGroup) {
        return FormHelpers.isDirty(control);
      }
      return control?.dirty ?? false;
    });
  },

  /**
   * Get all form values, optionally filtering out empty strings
   * @param formGroup - The form group
   * @param excludeEmpty - Whether to exclude empty string values
   */
  getValues<T extends Record<string, any>>(
    formGroup: FormGroup,
    excludeEmpty = false
  ): T {
    const values = formGroup.value;
    if (!excludeEmpty) return values;

    return Object.fromEntries(
      Object.entries(values).filter(([_, v]) => v !== '' && v !== null && v !== undefined)
    ) as T;
  },

  /**
   * Disable all controls in a form group
   * @param formGroup - The form group to disable
   * @param except - Optional array of control names to leave enabled
   */
  disableAll(formGroup: FormGroup, except: string[] = []): void {
    Object.keys(formGroup.controls).forEach(key => {
      if (!except.includes(key)) {
        formGroup.get(key)?.disable();
      }
    });
  },

  /**
   * Enable all controls in a form group
   * @param formGroup - The form group to enable
   * @param except - Optional array of control names to leave disabled
   */
  enableAll(formGroup: FormGroup, except: string[] = []): void {
    Object.keys(formGroup.controls).forEach(key => {
      if (!except.includes(key)) {
        formGroup.get(key)?.enable();
      }
    });
  },

  /**
   * Get error count for a form group
   * @param formGroup - The form group to check
   */
  getErrorCount(formGroup: FormGroup): number {
    let count = 0;
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      if (control instanceof FormGroup) {
        count += FormHelpers.getErrorCount(control);
      } else if (control?.errors) {
        count += Object.keys(control.errors).length;
      }
    });
    return count;
  }
};

/**
 * Get the first error message for a form control
 * @param control - The form control to check
 * @param onlyWhenTouched - Only return error if field was touched (default: true)
 * @returns Error message string or null
 *
 * @example
 * ```typescript
 * // In template
 * @if (getFieldError(form.get('email')); as error) {
 *   <span class="error">{{ error }}</span>
 * }
 *
 * // Or in getter
 * get emailError(): string | null {
 *   return getFieldError(this.email);
 * }
 * ```
 */
export function getFieldError(
  control: AbstractControl | null,
  onlyWhenTouched = true
): string | null {
  if (!control?.errors) return null;
  if (onlyWhenTouched && !control.touched) return null;

  const errorKey = Object.keys(control.errors)[0];
  const errorValue = control.errors[errorKey];
  const message = ValidationMessages[errorKey];

  if (!message) {
    return `Validierungsfehler: ${errorKey}`;
  }

  return typeof message === 'function' ? message(errorValue) : message;
}

/**
 * Get all error messages for a form control
 * @param control - The form control to check
 * @param onlyWhenTouched - Only return errors if field was touched (default: true)
 * @returns Array of error messages
 */
export function getAllFieldErrors(
  control: AbstractControl | null,
  onlyWhenTouched = true
): string[] {
  if (!control?.errors) return [];
  if (onlyWhenTouched && !control.touched) return [];

  return Object.keys(control.errors).map(errorKey => {
    const errorValue = control.errors![errorKey];
    const message = ValidationMessages[errorKey];

    if (!message) {
      return `Validierungsfehler: ${errorKey}`;
    }

    return typeof message === 'function' ? message(errorValue) : message;
  });
}

/**
 * Check if a form control has a specific error
 * @param control - The form control to check
 * @param errorKey - The error key to check for
 * @param onlyWhenTouched - Only check if field was touched (default: true)
 */
export function hasError(
  control: AbstractControl | null,
  errorKey: string,
  onlyWhenTouched = true
): boolean {
  if (!control) return false;
  if (onlyWhenTouched && !control.touched) return false;
  return control.hasError(errorKey);
}
