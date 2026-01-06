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
} from "@angular/forms";
import { RouterModule } from "@angular/router";
import {
  ErrorAlertComponent,
  SuccessAlertComponent,
  IconComponent,
  InputComponent,
  CheckboxComponent,
} from "@orange-car-rental/ui-components";
import { CustomValidators } from "@orange-car-rental/shared";
import type {
  AuthFormConfig,
  RegisterFormSubmitEvent,
  RegisterFormLabels,
} from "../auth-forms.types";
import {
  DEFAULT_AUTH_CONFIG,
  DEFAULT_REGISTER_LABELS_DE,
} from "../auth-forms.types";

/**
 * Reusable Register Form Component
 *
 * A configurable, presentational multi-step registration form that handles
 * validation and emits submit events. The parent component handles actual registration.
 *
 * @example
 * <lib-register-form
 *   [labels]="customLabels"
 *   [loading]="isRegistering()"
 *   [error]="registrationError()"
 *   [success]="registrationSuccess()"
 *   (formSubmit)="onRegister($event)"
 * />
 */
@Component({
  selector: "lib-register-form",
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ErrorAlertComponent,
    SuccessAlertComponent,
    IconComponent,
    InputComponent,
    CheckboxComponent,
  ],
  template: `
    <div class="register-container">
      <div class="register-card">
        <!-- Header -->
        <div class="register-header">
          @if (config().logoUrl) {
            <img
              [src]="config().logoUrl"
              [alt]="config().brandName || 'Logo'"
              class="register-logo"
            />
          }
          <h1 class="register-title">{{ labels().title }}</h1>
          <p class="register-subtitle">{{ labels().subtitle }}</p>
        </div>

        <!-- Progress Steps -->
        <div class="progress-steps">
          @for (step of [1, 2, 3]; track step) {
            <div
              class="progress-step"
              [class.active]="step <= currentStep()"
              [class.current]="step === currentStep()"
            >
              <div class="step-number">{{ step }}</div>
              <div class="step-label">
                @if (step === 1) {
                  <span>{{ labels().step1Title }}</span>
                }
                @if (step === 2) {
                  <span>{{ labels().step2Title }}</span>
                }
                @if (step === 3) {
                  <span>{{ labels().step3Title }}</span>
                }
              </div>
            </div>
          }
        </div>

        <!-- Error Message -->
        @if (error()) {
          <ui-error-alert [message]="error()!"></ui-error-alert>
        }

        <!-- Success Message -->
        @if (success()) {
          <ui-success-alert [message]="success()!"></ui-success-alert>
        }

        <!-- Registration Form -->
        <form
          [formGroup]="registerForm"
          (ngSubmit)="onSubmit()"
          class="register-form"
        >
          <!-- Step 1: Account Information -->
          @if (currentStep() === 1) {
            <div class="form-step">
              <ocr-input
                [label]="labels().emailLabel"
                type="email"
                formControlName="email"
                placeholder="ihre@email.de"
                autocomplete="email"
                leadingIcon="mail"
                [required]="true"
                [error]="emailError()"
              />

              <div class="password-field-with-hint">
                <ocr-input
                  [label]="labels().passwordLabel"
                  type="password"
                  formControlName="password"
                  placeholder="••••••••"
                  autocomplete="new-password"
                  leadingIcon="lock"
                  [showPasswordToggle]="true"
                  [required]="true"
                  [error]="passwordError()"
                />
                <span class="field-hint">{{ passwordHint() }}</span>
              </div>

              <ocr-input
                [label]="labels().confirmPasswordLabel"
                type="password"
                formControlName="confirmPassword"
                placeholder="••••••••"
                autocomplete="new-password"
                leadingIcon="lock"
                [showPasswordToggle]="true"
                [required]="true"
                [error]="confirmPasswordError()"
              />
            </div>
          }

          <!-- Step 2: Personal Information -->
          @if (currentStep() === 2) {
            <div class="form-step">
              <div class="form-row">
                <ocr-input
                  [label]="labels().firstNameLabel"
                  type="text"
                  formControlName="firstName"
                  placeholder="Max"
                  autocomplete="given-name"
                  leadingIcon="user"
                  [required]="true"
                  [error]="firstNameError()"
                />

                <ocr-input
                  [label]="labels().lastNameLabel"
                  type="text"
                  formControlName="lastName"
                  placeholder="Mustermann"
                  autocomplete="family-name"
                  leadingIcon="user"
                  [required]="true"
                  [error]="lastNameError()"
                />
              </div>

              <ocr-input
                [label]="labels().phoneLabel"
                type="tel"
                formControlName="phoneNumber"
                placeholder="+49 123 456789"
                autocomplete="tel"
                leadingIcon="phone"
                [required]="true"
                [error]="phoneNumberError()"
              />

              <div class="date-field-with-hint">
                <div class="form-group">
                  <label for="dateOfBirth" class="form-label">
                    {{ labels().dateOfBirthLabel }} *
                  </label>
                  <input
                    id="dateOfBirth"
                    type="date"
                    formControlName="dateOfBirth"
                    class="form-input"
                    [class.input-error]="dateOfBirthError()"
                    autocomplete="bday"
                  />
                  @if (dateOfBirthError()) {
                    <span class="field-error">{{ dateOfBirthError() }}</span>
                  }
                </div>
                <span class="field-hint">{{ ageHint() }}</span>
              </div>
            </div>
          }

          <!-- Step 3: Terms and Conditions -->
          @if (currentStep() === 3) {
            <div class="form-step">
              <ocr-checkbox formControlName="acceptTerms" [required]="true">
                {{ acceptTermsPrefix() }}
                @if (config().termsUrl) {
                  <a
                    [href]="config().termsUrl"
                    target="_blank"
                    class="terms-link"
                  >
                    {{ termsLinkText() }}
                  </a>
                }
              </ocr-checkbox>

              <ocr-checkbox formControlName="acceptPrivacy" [required]="true">
                {{ acceptPrivacyPrefix() }}
                @if (config().privacyUrl) {
                  <a
                    [href]="config().privacyUrl"
                    target="_blank"
                    class="terms-link"
                  >
                    {{ privacyLinkText() }}
                  </a>
                }
              </ocr-checkbox>

              <ocr-checkbox formControlName="acceptMarketing">
                {{ labels().acceptMarketingLabel }}
              </ocr-checkbox>

              <div class="info-box">
                <lib-icon
                  name="information-circle"
                  variant="outline"
                  size="sm"
                  class="info-icon"
                />
                <div>
                  <p class="info-title">{{ privacyInfoTitle() }}</p>
                  <p class="info-text">{{ privacyInfoText() }}</p>
                </div>
              </div>
            </div>
          }

          <!-- Navigation Buttons -->
          <div class="form-actions">
            @if (currentStep() > 1) {
              <button
                type="button"
                class="secondary-button"
                (click)="previousStep()"
              >
                {{ labels().previousButton }}
              </button>
            }

            @if (currentStep() < totalSteps) {
              <button
                type="button"
                class="primary-button"
                (click)="nextStep()"
                [class.full-width]="currentStep() === 1"
              >
                {{ labels().nextButton }}
              </button>
            }

            @if (currentStep() === totalSteps) {
              <button
                type="submit"
                class="primary-button"
                [disabled]="loading()"
              >
                @if (loading()) {
                  <span class="spinner"></span>
                  <span>{{ labels().submittingButton }}</span>
                } @else {
                  <span>{{ labels().submitButton }}</span>
                }
              </button>
            }
          </div>
        </form>

        <!-- Login Link -->
        @if (config().loginRoute) {
          <div class="login-section">
            <p class="login-text">
              {{ labels().hasAccountText }}
              <a [routerLink]="config().loginRoute" class="login-link">
                {{ labels().loginLink }}
              </a>
            </p>
          </div>
        }
      </div>
    </div>
  `,
  styles: [
    `
      .register-container {
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: 100%;
        padding: 2rem 1rem;
      }

      .register-card {
        width: 100%;
        max-width: 28rem;
        background: white;
        border-radius: 0.75rem;
        box-shadow:
          0 4px 6px -1px rgba(0, 0, 0, 0.1),
          0 2px 4px -1px rgba(0, 0, 0, 0.06);
        padding: 2rem;
      }

      .register-header {
        text-align: center;
        margin-bottom: 1.5rem;
      }

      .register-logo {
        height: 3rem;
        margin-bottom: 1rem;
      }

      .register-title {
        margin: 0;
        font-size: 1.5rem;
        font-weight: 700;
        color: #111827;
      }

      .register-subtitle {
        margin: 0.5rem 0 0;
        font-size: 0.875rem;
        color: #6b7280;
      }

      .progress-steps {
        display: flex;
        justify-content: space-between;
        margin-bottom: 1.5rem;
        position: relative;
      }

      .progress-steps::before {
        content: "";
        position: absolute;
        top: 1rem;
        left: 2rem;
        right: 2rem;
        height: 2px;
        background: #e5e7eb;
        z-index: 0;
      }

      .progress-step {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 0.5rem;
        position: relative;
        z-index: 1;
      }

      .step-number {
        width: 2rem;
        height: 2rem;
        border-radius: 50%;
        background: #e5e7eb;
        color: #6b7280;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 0.875rem;
        font-weight: 600;
        transition: all 0.2s ease;
      }

      .progress-step.active .step-number {
        background: #f97316;
        color: white;
      }

      .progress-step.current .step-number {
        box-shadow: 0 0 0 4px rgba(249, 115, 22, 0.2);
      }

      .step-label {
        font-size: 0.75rem;
        color: #6b7280;
        text-align: center;
      }

      .progress-step.active .step-label {
        color: #374151;
        font-weight: 500;
      }

      .register-form {
        display: flex;
        flex-direction: column;
        gap: 1rem;
      }

      .form-step {
        display: flex;
        flex-direction: column;
        gap: 1rem;
      }

      .form-row {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 1rem;
      }

      @media (max-width: 480px) {
        .form-row {
          grid-template-columns: 1fr;
        }
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

      .form-input {
        width: 100%;
        padding: 0.625rem 0.75rem;
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

      .field-error {
        font-size: 0.75rem;
        color: #ef4444;
        margin-top: 0.25rem;
      }

      .field-hint {
        font-size: 0.75rem;
        color: #6b7280;
        margin-top: 0.25rem;
      }

      .password-field-with-hint,
      .date-field-with-hint {
        display: flex;
        flex-direction: column;
      }

      .terms-link {
        color: #f97316;
        text-decoration: none;
      }

      .terms-link:hover {
        text-decoration: underline;
      }

      .info-box {
        display: flex;
        gap: 0.75rem;
        padding: 1rem;
        background: #f3f4f6;
        border-radius: 0.5rem;
        margin-top: 0.5rem;
      }

      .info-icon {
        color: #6b7280;
        flex-shrink: 0;
      }

      .info-title {
        margin: 0 0 0.25rem;
        font-size: 0.875rem;
        font-weight: 500;
        color: #374151;
      }

      .info-text {
        margin: 0;
        font-size: 0.75rem;
        color: #6b7280;
      }

      .form-actions {
        display: flex;
        gap: 0.75rem;
        margin-top: 0.5rem;
      }

      .primary-button,
      .secondary-button {
        flex: 1;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 0.5rem;
        padding: 0.625rem 1rem;
        font-size: 0.875rem;
        font-weight: 500;
        border-radius: 0.375rem;
        cursor: pointer;
        transition: all 0.15s ease;
      }

      .primary-button {
        background-color: #f97316;
        color: white;
        border: none;
      }

      .primary-button:hover:not(:disabled) {
        background-color: #ea580c;
      }

      .primary-button:disabled {
        opacity: 0.7;
        cursor: not-allowed;
      }

      .primary-button.full-width {
        flex: 1;
      }

      .secondary-button {
        background-color: white;
        color: #374151;
        border: 1px solid #d1d5db;
      }

      .secondary-button:hover {
        background-color: #f9fafb;
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

      .login-section {
        text-align: center;
        margin-top: 1.5rem;
      }

      .login-text {
        margin: 0;
        font-size: 0.875rem;
        color: #6b7280;
      }

      .login-link {
        color: #f97316;
        text-decoration: none;
        font-weight: 500;
      }

      .login-link:hover {
        text-decoration: underline;
      }
    `,
  ],
})
export class RegisterFormComponent {
  private readonly fb = inject(FormBuilder);

  /**
   * Form configuration
   */
  readonly config = input<AuthFormConfig>(DEFAULT_AUTH_CONFIG);

  /**
   * Form labels (for i18n)
   */
  readonly labels = input<RegisterFormLabels>(DEFAULT_REGISTER_LABELS_DE);

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
  readonly formSubmit = output<RegisterFormSubmitEvent>();

  /**
   * Form group
   */
  readonly registerForm: FormGroup;

  /**
   * Current step (1-3)
   */
  readonly currentStep = signal(1);

  /**
   * Total number of steps
   */
  readonly totalSteps = 3;

  constructor() {
    const minAge = DEFAULT_AUTH_CONFIG.minAge ?? 21;
    const minPasswordLength = DEFAULT_AUTH_CONFIG.minPasswordLength ?? 8;

    this.registerForm = this.fb.group(
      {
        // Step 1: Account Information
        email: ["", [Validators.required, Validators.email]],
        password: [
          "",
          [
            Validators.required,
            Validators.minLength(minPasswordLength),
            CustomValidators.passwordStrength(),
          ],
        ],
        confirmPassword: ["", [Validators.required]],

        // Step 2: Personal Information
        firstName: ["", [Validators.required, Validators.minLength(2)]],
        lastName: ["", [Validators.required, Validators.minLength(2)]],
        phoneNumber: [
          "",
          [Validators.required, CustomValidators.germanPhone()],
        ],
        dateOfBirth: [
          "",
          [Validators.required, CustomValidators.minimumAge(minAge)],
        ],

        // Step 3: Terms
        acceptTerms: [false, [Validators.requiredTrue]],
        acceptPrivacy: [false, [Validators.requiredTrue]],
        acceptMarketing: [false],
      },
      {
        validators: CustomValidators.passwordMatch(
          "password",
          "confirmPassword",
        ),
      },
    );
  }

  // Computed labels for display
  readonly passwordHint = computed(
    () =>
      `Mindestens ${this.config().minPasswordLength ?? 8} Zeichen mit Groß-/Kleinbuchstaben, Zahlen und Sonderzeichen`,
  );

  readonly ageHint = computed(
    () => `Sie müssen mindestens ${this.config().minAge ?? 21} Jahre alt sein`,
  );

  readonly acceptTermsPrefix = computed(() => "Ich akzeptiere die ");
  readonly termsLinkText = computed(() => "AGB");
  readonly acceptPrivacyPrefix = computed(() => "Ich akzeptiere die ");
  readonly privacyLinkText = computed(() => "Datenschutzerklärung");
  readonly privacyInfoTitle = computed(() => "Datenschutz");
  readonly privacyInfoText = computed(
    () =>
      "Ihre Daten werden gemäß DSGVO verarbeitet und nicht an Dritte weitergegeben.",
  );

  // Validation error computeds
  readonly emailError = computed(() => {
    const email = this.registerForm.get("email");
    if (email?.hasError("required") && email?.touched) {
      return this.labels().emailRequired;
    }
    if (email?.hasError("email") && email?.touched) {
      return this.labels().emailInvalid;
    }
    return null;
  });

  readonly passwordError = computed(() => {
    const password = this.registerForm.get("password");
    if (password?.hasError("required") && password?.touched) {
      return this.labels().passwordRequired;
    }
    if (password?.hasError("minlength") && password?.touched) {
      return this.labels().passwordMinLength;
    }
    if (password?.hasError("weakPassword") && password?.touched) {
      return this.labels().passwordWeak;
    }
    return null;
  });

  readonly confirmPasswordError = computed(() => {
    const confirmPassword = this.registerForm.get("confirmPassword");
    if (confirmPassword?.hasError("required") && confirmPassword?.touched) {
      return this.labels().confirmPasswordRequired;
    }
    if (
      this.registerForm.hasError("passwordMismatch") &&
      confirmPassword?.touched
    ) {
      return this.labels().passwordMismatch;
    }
    return null;
  });

  readonly firstNameError = computed(() => {
    const firstName = this.registerForm.get("firstName");
    if (firstName?.hasError("required") && firstName?.touched) {
      return this.labels().firstNameRequired;
    }
    return null;
  });

  readonly lastNameError = computed(() => {
    const lastName = this.registerForm.get("lastName");
    if (lastName?.hasError("required") && lastName?.touched) {
      return this.labels().lastNameRequired;
    }
    return null;
  });

  readonly phoneNumberError = computed(() => {
    const phoneNumber = this.registerForm.get("phoneNumber");
    if (phoneNumber?.hasError("required") && phoneNumber?.touched) {
      return this.labels().phoneRequired;
    }
    if (phoneNumber?.hasError("invalidPhone") && phoneNumber?.touched) {
      return this.labels().phoneInvalid;
    }
    return null;
  });

  readonly dateOfBirthError = computed(() => {
    const dateOfBirth = this.registerForm.get("dateOfBirth");
    if (dateOfBirth?.hasError("required") && dateOfBirth?.touched) {
      return this.labels().dateOfBirthRequired;
    }
    if (dateOfBirth?.hasError("underage") && dateOfBirth?.touched) {
      const minAge = this.config().minAge ?? 21;
      return this.labels().minAgeError.replace("{minAge}", String(minAge));
    }
    return null;
  });

  /**
   * Move to next step if current step is valid
   */
  nextStep(): void {
    if (this.currentStep() < this.totalSteps && this.isCurrentStepValid()) {
      this.currentStep.set(this.currentStep() + 1);
    } else {
      this.markCurrentStepTouched();
    }
  }

  /**
   * Move to previous step
   */
  previousStep(): void {
    if (this.currentStep() > 1) {
      this.currentStep.set(this.currentStep() - 1);
    }
  }

  /**
   * Check if current step fields are valid
   */
  private isCurrentStepValid(): boolean {
    switch (this.currentStep()) {
      case 1:
        return !!(
          this.registerForm.get("email")?.valid &&
          this.registerForm.get("password")?.valid &&
          this.registerForm.get("confirmPassword")?.valid &&
          !this.registerForm.hasError("passwordMismatch")
        );
      case 2:
        return !!(
          this.registerForm.get("firstName")?.valid &&
          this.registerForm.get("lastName")?.valid &&
          this.registerForm.get("phoneNumber")?.valid &&
          this.registerForm.get("dateOfBirth")?.valid
        );
      case 3:
        return !!(
          this.registerForm.get("acceptTerms")?.valid &&
          this.registerForm.get("acceptPrivacy")?.valid
        );
      default:
        return false;
    }
  }

  /**
   * Mark current step fields as touched
   */
  private markCurrentStepTouched(): void {
    const controls = this.getCurrentStepControls();
    controls.forEach((name) => this.registerForm.get(name)?.markAsTouched());
  }

  /**
   * Get control names for current step
   */
  private getCurrentStepControls(): string[] {
    switch (this.currentStep()) {
      case 1:
        return ["email", "password", "confirmPassword"];
      case 2:
        return ["firstName", "lastName", "phoneNumber", "dateOfBirth"];
      case 3:
        return ["acceptTerms", "acceptPrivacy"];
      default:
        return [];
    }
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const formValue = this.registerForm.value;
    this.formSubmit.emit({
      email: formValue.email,
      password: formValue.password,
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      phoneNumber: formValue.phoneNumber,
      dateOfBirth: formValue.dateOfBirth,
      acceptTerms: formValue.acceptTerms,
      acceptPrivacy: formValue.acceptPrivacy,
      acceptMarketing: formValue.acceptMarketing,
    });
  }

  /**
   * Reset the form
   */
  reset(): void {
    this.registerForm.reset();
    this.currentStep.set(1);
  }
}
