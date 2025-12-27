import { FormGroup } from '@angular/forms';
import type { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Custom Validators
 *
 * Reusable form validators for common validation patterns.
 * All validators are pure functions that can be used with Angular Reactive Forms.
 *
 * @example
 * ```typescript
 * import { CustomValidators } from '@orange-car-rental/shared';
 *
 * this.form = this.fb.group({
 *   password: ['', [Validators.required, CustomValidators.passwordStrength()]],
 *   dateOfBirth: ['', [Validators.required, CustomValidators.minimumAge(18)]],
 * }, {
 *   validators: [CustomValidators.passwordMatch('password', 'confirmPassword')]
 * });
 * ```
 */
export const CustomValidators = {
  /**
   * Validates password strength (uppercase, lowercase, number, special char)
   * @returns ValidatorFn that returns { weakPassword: true } if invalid
   */
  passwordStrength(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (!value) return null;

      const hasUpperCase = /[A-Z]/.test(value);
      const hasLowerCase = /[a-z]/.test(value);
      const hasNumeric = /[0-9]/.test(value);
      const hasSpecial = /[!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?]/.test(value);

      const isValid = hasUpperCase && hasLowerCase && hasNumeric && hasSpecial;

      return isValid ? null : { weakPassword: true };
    };
  },

  /**
   * Validates that two form controls match (e.g., password confirmation)
   * @param controlName - Name of the first control
   * @param matchingControlName - Name of the control that should match
   * @returns ValidatorFn that returns { passwordMismatch: true } if values don't match
   */
  passwordMatch(controlName: string, matchingControlName: string): ValidatorFn {
    return (formGroup: AbstractControl): ValidationErrors | null => {
      const control = formGroup.get(controlName);
      const matchingControl = formGroup.get(matchingControlName);

      if (!control || !matchingControl) return null;

      return control.value === matchingControl.value
        ? null
        : { passwordMismatch: true };
    };
  },

  /**
   * Validates that a date of birth meets minimum age requirement
   * @param minAge - Minimum age in years
   * @returns ValidatorFn that returns { underage: { minAge } } if too young
   */
  minimumAge(minAge: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const birthDate = new Date(control.value);
      const today = new Date();
      let age = today.getFullYear() - birthDate.getFullYear();
      const monthDiff = today.getMonth() - birthDate.getMonth();

      if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
        age--;
      }

      return age >= minAge ? null : { underage: { minAge } };
    };
  },

  /**
   * Validates that a date is not in the past
   * @returns ValidatorFn that returns { pastDate: true } if date is in the past
   */
  futureDate(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const date = new Date(control.value);
      const today = new Date();
      today.setHours(0, 0, 0, 0);

      return date >= today ? null : { pastDate: true };
    };
  },

  /**
   * Validates that a date is before another date control
   * @param otherControlName - Name of the other date control
   * @returns ValidatorFn that returns { dateAfter: true } if not before
   */
  dateBefore(otherControlName: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const parent = control.parent;
      if (!parent) return null;

      const otherControl = parent.get(otherControlName);
      if (!otherControl?.value) return null;

      const thisDate = new Date(control.value);
      const otherDate = new Date(otherControl.value);

      return thisDate < otherDate ? null : { dateAfter: { other: otherControlName } };
    };
  },

  /**
   * Validates German phone number format
   * @returns ValidatorFn that returns { invalidPhone: true } if invalid
   */
  germanPhone(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      // German phone: optional +49 or 0, then digits (spaces/dashes allowed)
      const phoneRegex = /^(\+49|0)[1-9][0-9\s\-]{6,14}$/;
      const cleanValue = control.value.replace(/[\s\-]/g, '');

      return phoneRegex.test(cleanValue) ? null : { invalidPhone: true };
    };
  },

  /**
   * Validates German postal code (5 digits)
   * @returns ValidatorFn that returns { invalidPostalCode: true } if invalid
   */
  germanPostalCode(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const postalRegex = /^[0-9]{5}$/;
      return postalRegex.test(control.value) ? null : { invalidPostalCode: true };
    };
  },

  /**
   * Validates license plate format (German)
   * @returns ValidatorFn that returns { invalidLicensePlate: true } if invalid
   */
  germanLicensePlate(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      // German format: 1-3 letters, space, 1-2 letters, space, 1-4 digits
      const plateRegex = /^[A-Z]{1,3}\s?[A-Z]{1,2}\s?\d{1,4}$/i;
      return plateRegex.test(control.value) ? null : { invalidLicensePlate: true };
    };
  },

  /**
   * Validates that at least one checkbox is checked in a group
   * @returns ValidatorFn that returns { noneSelected: true } if none selected
   */
  atLeastOneChecked(): ValidatorFn {
    return (formGroup: AbstractControl): ValidationErrors | null => {
      const group = formGroup as FormGroup;
      const controls = group.controls;
      if (!controls) return null;

      const hasChecked = Object.keys(controls).some(key => controls[key].value === true);
      return hasChecked ? null : { noneSelected: true };
    };
  },

  /**
   * Validates a number is within a range
   * @param min - Minimum value
   * @param max - Maximum value
   * @returns ValidatorFn that returns { outOfRange: { min, max } } if outside range
   */
  numberRange(min: number, max: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (control.value === null || control.value === undefined || control.value === '') {
        return null;
      }

      const value = Number(control.value);
      if (isNaN(value)) return { notANumber: true };

      return value >= min && value <= max ? null : { outOfRange: { min, max, actual: value } };
    };
  },

  /**
   * Validates that a value is a positive number
   * @returns ValidatorFn that returns { notPositive: true } if not positive
   */
  positiveNumber(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (control.value === null || control.value === undefined || control.value === '') {
        return null;
      }

      const value = Number(control.value);
      return !isNaN(value) && value > 0 ? null : { notPositive: true };
    };
  }
};
