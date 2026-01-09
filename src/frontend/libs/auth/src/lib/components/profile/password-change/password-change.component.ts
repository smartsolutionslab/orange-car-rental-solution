import {
  Component,
  input,
  output,
  signal,
  computed,
  inject,
} from "@angular/core";
import { CommonModule } from "@angular/common";
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
  AbstractControl,
  type ValidationErrors,
} from "@angular/forms";
import {
  ErrorAlertComponent,
  IconComponent,
  SuccessAlertComponent,
} from "@orange-car-rental/ui-components";
import type { PasswordChangeLabels, PasswordChangeEvent } from "../profile.types";
import { DEFAULT_PASSWORD_CHANGE_LABELS_DE } from "../profile.types";

/**
 * Password Change Component
 *
 * Form for changing user password with validation.
 *
 * @example
 * <lib-password-change
 *   [labels]="germanLabels"
 *   [loading]="isChanging()"
 *   [error]="changeError()"
 *   [success]="changeSuccess()"
 *   (formSubmit)="onChangePassword($event)"
 *   (cancel)="onCancel()"
 * />
 */
@Component({
  selector: "lib-password-change",
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ErrorAlertComponent,
    SuccessAlertComponent,
    IconComponent,
  ],
  template: `
    <div class="password-container">
      <div class="password-card">
        <!-- Header -->
        <div class="password-header">
          <h1 class="password-title">{{ labels().title }}</h1>
          <p class="password-subtitle">{{ labels().subtitle }}</p>
        </div>

        <!-- Success Message -->
        @if (success()) {
          <div class="alert-container">
            <ui-success-alert [message]="success()!"></ui-success-alert>
          </div>
        }

        <!-- Error Message -->
        @if (error()) {
          <div class="alert-container">
            <ui-error-alert [message]="error()!"></ui-error-alert>
          </div>
        }

        <!-- Password Change Form -->
        <form
          [formGroup]="passwordForm"
          (ngSubmit)="onSubmit()"
          class="password-form"
        >
          <!-- Current Password -->
          <div class="form-group">
            <label for="currentPassword" class="form-label">{{
              labels().currentPasswordLabel
            }}</label>
            <div class="password-wrapper">
              <input
                id="currentPassword"
                [type]="showCurrentPassword() ? 'text' : 'password'"
                formControlName="currentPassword"
                class="form-input"
                [class.input-error]="currentPasswordError()"
                autocomplete="current-password"
              />
              <button
                type="button"
                class="password-toggle"
                (click)="toggleCurrentPasswordVisibility()"
              >
                @if (showCurrentPassword()) {
                  <lib-icon name="eye-off" variant="outline" size="sm" />
                } @else {
                  <lib-icon name="eye" variant="outline" size="sm" />
                }
              </button>
            </div>
            @if (currentPasswordError()) {
              <span class="field-error">{{ currentPasswordError() }}</span>
            }
          </div>

          <!-- New Password -->
          <div class="form-group">
            <label for="newPassword" class="form-label">{{
              labels().newPasswordLabel
            }}</label>
            <div class="password-wrapper">
              <input
                id="newPassword"
                [type]="showNewPassword() ? 'text' : 'password'"
                formControlName="newPassword"
                class="form-input"
                [class.input-error]="newPasswordError()"
                autocomplete="new-password"
              />
              <button
                type="button"
                class="password-toggle"
                (click)="toggleNewPasswordVisibility()"
              >
                @if (showNewPassword()) {
                  <lib-icon name="eye-off" variant="outline" size="sm" />
                } @else {
                  <lib-icon name="eye" variant="outline" size="sm" />
                }
              </button>
            </div>
            @if (newPasswordError()) {
              <span class="field-error">{{ newPasswordError() }}</span>
            }
            <p class="password-hint">{{ labels().passwordHint }}</p>
          </div>

          <!-- Confirm New Password -->
          <div class="form-group">
            <label for="confirmPassword" class="form-label">{{
              labels().confirmPasswordLabel
            }}</label>
            <div class="password-wrapper">
              <input
                id="confirmPassword"
                [type]="showConfirmPassword() ? 'text' : 'password'"
                formControlName="confirmPassword"
                class="form-input"
                [class.input-error]="confirmPasswordError()"
                autocomplete="new-password"
              />
              <button
                type="button"
                class="password-toggle"
                (click)="toggleConfirmPasswordVisibility()"
              >
                @if (showConfirmPassword()) {
                  <lib-icon name="eye-off" variant="outline" size="sm" />
                } @else {
                  <lib-icon name="eye" variant="outline" size="sm" />
                }
              </button>
            </div>
            @if (confirmPasswordError()) {
              <span class="field-error">{{ confirmPasswordError() }}</span>
            }
          </div>

          <!-- Form Actions -->
          <div class="form-actions">
            <button
              type="button"
              class="cancel-button"
              (click)="cancel.emit()"
              [disabled]="loading()"
            >
              {{ labels().cancelButton }}
            </button>
            <button
              type="submit"
              class="submit-button"
              [disabled]="loading()"
            >
              @if (loading()) {
                <span class="spinner"></span>
                <span>{{ labels().savingButton }}</span>
              } @else {
                <span>{{ labels().saveButton }}</span>
              }
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [
    `
      .password-container {
        padding: 1.5rem;
        max-width: 32rem;
        margin: 0 auto;
      }

      .password-card {
        background: white;
        border-radius: 0.75rem;
        box-shadow:
          0 4px 6px -1px rgba(0, 0, 0, 0.1),
          0 2px 4px -1px rgba(0, 0, 0, 0.06);
        overflow: hidden;
      }

      .password-header {
        padding: 1.5rem;
        border-bottom: 1px solid #e5e7eb;
      }

      .password-title {
        margin: 0;
        font-size: 1.25rem;
        font-weight: 600;
        color: #111827;
      }

      .password-subtitle {
        margin: 0.5rem 0 0;
        font-size: 0.875rem;
        color: #6b7280;
      }

      .alert-container {
        padding: 1rem 1.5rem 0;
      }

      .password-form {
        padding: 1.5rem;
        display: flex;
        flex-direction: column;
        gap: 1.25rem;
      }

      .form-group {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
      }

      .form-label {
        font-size: 0.875rem;
        font-weight: 500;
        color: #374151;
      }

      .password-wrapper {
        position: relative;
      }

      .form-input {
        width: 100%;
        padding: 0.625rem 2.5rem 0.625rem 0.75rem;
        border: 1px solid #d1d5db;
        border-radius: 0.375rem;
        font-size: 0.875rem;
        transition:
          border-color 0.15s ease,
          box-shadow 0.15s ease;
      }

      .form-input:focus {
        outline: none;
        border-color: #f97316;
        box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.1);
      }

      .form-input.input-error {
        border-color: #ef4444;
      }

      .password-toggle {
        position: absolute;
        right: 0.5rem;
        top: 50%;
        transform: translateY(-50%);
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 0.25rem;
        background: none;
        border: none;
        cursor: pointer;
        color: #6b7280;
        border-radius: 0.25rem;
      }

      .password-toggle:hover {
        color: #374151;
        background-color: #f3f4f6;
      }

      .field-error {
        font-size: 0.75rem;
        color: #ef4444;
        margin-top: 0.25rem;
      }

      .password-hint {
        margin: 0.5rem 0 0;
        font-size: 0.75rem;
        color: #6b7280;
      }

      .form-actions {
        display: flex;
        justify-content: flex-end;
        gap: 0.75rem;
        padding-top: 1rem;
        border-top: 1px solid #e5e7eb;
        margin-top: 0.5rem;
      }

      .cancel-button {
        padding: 0.625rem 1.25rem;
        background: white;
        color: #374151;
        font-size: 0.875rem;
        font-weight: 500;
        border: 1px solid #d1d5db;
        border-radius: 0.375rem;
        cursor: pointer;
        transition: all 0.15s ease;
      }

      .cancel-button:hover:not(:disabled) {
        background: #f9fafb;
      }

      .cancel-button:disabled {
        opacity: 0.7;
        cursor: not-allowed;
      }

      .submit-button {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 0.5rem;
        padding: 0.625rem 1.25rem;
        background-color: #f97316;
        color: white;
        font-size: 0.875rem;
        font-weight: 500;
        border: none;
        border-radius: 0.375rem;
        cursor: pointer;
        transition: background-color 0.15s ease;
      }

      .submit-button:hover:not(:disabled) {
        background-color: #ea580c;
      }

      .submit-button:disabled {
        opacity: 0.7;
        cursor: not-allowed;
      }

      .spinner {
        width: 1rem;
        height: 1rem;
        border: 2px solid rgba(255, 255, 255, 0.3);
        border-top-color: white;
        border-radius: 50%;
        animation: spin 0.8s linear infinite;
      }

      @keyframes spin {
        to {
          transform: rotate(360deg);
        }
      }
    `,
  ],
})
export class PasswordChangeComponent {
  private readonly fb = inject(FormBuilder);

  /**
   * Component labels (for i18n)
   */
  readonly labels = input<PasswordChangeLabels>(
    DEFAULT_PASSWORD_CHANGE_LABELS_DE
  );

  /**
   * Minimum password length
   */
  readonly minPasswordLength = input(8);

  /**
   * External loading state
   */
  readonly loading = input(false);

  /**
   * External error message
   */
  readonly error = input<string | null>(null);

  /**
   * External success message
   */
  readonly success = input<string | null>(null);

  /**
   * Emitted when form is submitted with valid data
   */
  readonly formSubmit = output<PasswordChangeEvent>();

  /**
   * Emitted when cancel button is clicked
   */
  readonly cancel = output<void>();

  /**
   * Form group
   */
  readonly passwordForm: FormGroup;

  /**
   * Password visibility states
   */
  readonly showCurrentPassword = signal(false);
  readonly showNewPassword = signal(false);
  readonly showConfirmPassword = signal(false);

  /**
   * Form touched state for validation
   */
  private readonly formTouched = signal(false);

  constructor() {
    this.passwordForm = this.fb.group(
      {
        currentPassword: ["", [Validators.required]],
        newPassword: [
          "",
          [
            Validators.required,
            Validators.minLength(8),
            this.passwordStrengthValidator,
          ],
        ],
        confirmPassword: ["", [Validators.required]],
      },
      {
        validators: [this.passwordMatchValidator, this.notSameAsCurrentValidator],
      }
    );
  }

  /**
   * Password strength validator
   */
  private passwordStrengthValidator(
    control: AbstractControl
  ): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;

    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumeric = /[0-9]/.test(value);
    const hasSpecial = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(value);

    const isStrong = hasUpperCase && hasLowerCase && hasNumeric && hasSpecial;

    return isStrong ? null : { weakPassword: true };
  }

  /**
   * Password match validator
   */
  private passwordMatchValidator(group: AbstractControl): ValidationErrors | null {
    const newPassword = group.get("newPassword")?.value;
    const confirmPassword = group.get("confirmPassword")?.value;

    if (newPassword && confirmPassword && newPassword !== confirmPassword) {
      return { passwordMismatch: true };
    }
    return null;
  }

  /**
   * Not same as current password validator
   */
  private notSameAsCurrentValidator(
    group: AbstractControl
  ): ValidationErrors | null {
    const currentPassword = group.get("currentPassword")?.value;
    const newPassword = group.get("newPassword")?.value;

    if (currentPassword && newPassword && currentPassword === newPassword) {
      return { sameAsCurrentPassword: true };
    }
    return null;
  }

  // Validation error computed signals
  readonly currentPasswordError = computed(() => {
    this.formTouched();
    const ctrl = this.passwordForm.get("currentPassword");
    if (ctrl?.hasError("required") && ctrl?.touched) {
      return this.labels().currentPasswordRequired;
    }
    return null;
  });

  readonly newPasswordError = computed(() => {
    this.formTouched();
    const ctrl = this.passwordForm.get("newPassword");
    if (ctrl?.hasError("required") && ctrl?.touched) {
      return this.labels().newPasswordRequired;
    }
    if (ctrl?.hasError("minlength") && ctrl?.touched) {
      return this.labels().newPasswordMinLength;
    }
    if (ctrl?.hasError("weakPassword") && ctrl?.touched) {
      return this.labels().newPasswordWeak;
    }
    if (
      this.passwordForm.hasError("sameAsCurrentPassword") &&
      ctrl?.touched
    ) {
      return this.labels().sameAsCurrentPassword;
    }
    return null;
  });

  readonly confirmPasswordError = computed(() => {
    this.formTouched();
    const ctrl = this.passwordForm.get("confirmPassword");
    if (ctrl?.hasError("required") && ctrl?.touched) {
      return this.labels().confirmPasswordRequired;
    }
    if (
      this.passwordForm.hasError("passwordMismatch") &&
      ctrl?.touched
    ) {
      return this.labels().passwordMismatch;
    }
    return null;
  });

  /**
   * Toggle password visibility
   */
  toggleCurrentPasswordVisibility(): void {
    this.showCurrentPassword.set(!this.showCurrentPassword());
  }

  toggleNewPasswordVisibility(): void {
    this.showNewPassword.set(!this.showNewPassword());
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword.set(!this.showConfirmPassword());
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      this.formTouched.set(true);
      return;
    }

    const { currentPassword, newPassword } = this.passwordForm.value;
    this.formSubmit.emit({ currentPassword, newPassword });
  }

  /**
   * Reset the form
   */
  reset(): void {
    this.passwordForm.reset();
    this.formTouched.set(false);
    this.showCurrentPassword.set(false);
    this.showNewPassword.set(false);
    this.showConfirmPassword.set(false);
  }
}
