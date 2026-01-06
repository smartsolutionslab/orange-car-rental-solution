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
  IconComponent,
} from "@orange-car-rental/ui-components";
import type {
  AuthFormConfig,
  LoginFormSubmitEvent,
  LoginFormLabels,
} from "../auth-forms.types";
import {
  DEFAULT_AUTH_CONFIG,
  DEFAULT_LOGIN_LABELS_DE,
} from "../auth-forms.types";

/**
 * Reusable Login Form Component
 *
 * A configurable, presentational login form that handles validation
 * and emits submit events. The parent component handles actual authentication.
 *
 * @example
 * <lib-login-form
 *   [labels]="customLabels"
 *   [loading]="isAuthenticating()"
 *   [error]="authError()"
 *   (formSubmit)="onLogin($event)"
 * />
 */
@Component({
  selector: "lib-login-form",
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ErrorAlertComponent,
    IconComponent,
  ],
  template: `
    <div class="login-container">
      <div class="login-card">
        <!-- Header -->
        <div class="login-header">
          @if (config().logoUrl) {
            <img
              [src]="config().logoUrl"
              [alt]="config().brandName || 'Logo'"
              class="login-logo"
            />
          }
          <h1 class="login-title">{{ labels().title }}</h1>
          <p class="login-subtitle">{{ labels().subtitle }}</p>
        </div>

        <!-- Error Message -->
        @if (error()) {
          <ui-error-alert [message]="error()!"></ui-error-alert>
        }

        <!-- Login Form -->
        <form
          [formGroup]="loginForm"
          (ngSubmit)="onSubmit()"
          class="login-form"
        >
          <!-- Email Field -->
          <div class="form-group">
            <label for="login-email" class="form-label">{{
              labels().emailLabel
            }}</label>
            <input
              id="login-email"
              type="email"
              formControlName="email"
              class="form-input"
              [class.input-error]="emailError()"
              [placeholder]="labels().emailPlaceholder"
              autocomplete="email"
            />
            @if (emailError()) {
              <span class="field-error">{{ emailError() }}</span>
            }
          </div>

          <!-- Password Field -->
          <div class="form-group">
            <div class="label-row">
              <label for="login-password" class="form-label">{{
                labels().passwordLabel
              }}</label>
              @if (config().forgotPasswordRoute) {
                <a
                  [routerLink]="config().forgotPasswordRoute"
                  class="forgot-link"
                >
                  {{ labels().forgotPasswordLink }}
                </a>
              }
            </div>
            <div class="password-wrapper">
              <input
                id="login-password"
                [type]="showPassword() ? 'text' : 'password'"
                formControlName="password"
                class="form-input"
                [class.input-error]="passwordError()"
                placeholder="••••••••"
                autocomplete="current-password"
              />
              <button
                type="button"
                class="password-toggle"
                (click)="togglePasswordVisibility()"
                [attr.aria-label]="
                  showPassword() ? labels().hidePassword : labels().showPassword
                "
              >
                @if (showPassword()) {
                  <lib-icon name="eye-off" variant="outline" size="sm" />
                } @else {
                  <lib-icon name="eye" variant="outline" size="sm" />
                }
              </button>
            </div>
            @if (passwordError()) {
              <span class="field-error">{{ passwordError() }}</span>
            }
          </div>

          <!-- Remember Me -->
          @if (config().showRememberMe) {
            <div class="form-group">
              <label class="checkbox-label">
                <input
                  type="checkbox"
                  formControlName="rememberMe"
                  class="checkbox-input"
                />
                <span>{{ labels().rememberMeLabel }}</span>
              </label>
            </div>
          }

          <!-- Submit Button -->
          <button type="submit" class="submit-button" [disabled]="loading()">
            @if (loading()) {
              <span class="spinner"></span>
              <span>{{ labels().submittingButton }}</span>
            } @else {
              <span>{{ labels().submitButton }}</span>
            }
          </button>
        </form>

        <!-- Register Link -->
        @if (config().registerRoute) {
          <div class="register-section">
            <p class="register-text">
              {{ labels().noAccountText }}
              <a [routerLink]="config().registerRoute" class="register-link">
                {{ labels().registerLink }}
              </a>
            </p>
          </div>
        }

        <!-- Social Login -->
        @if (config().showSocialLogin) {
          <div class="divider">
            <span>{{ labels().orDivider }}</span>
          </div>
          <div class="social-login">
            <button
              type="button"
              class="social-button"
              (click)="onGoogleLogin()"
            >
              <svg class="social-icon" viewBox="0 0 24 24">
                <path
                  fill="currentColor"
                  d="M12.545,10.239v3.821h5.445c-0.712,2.315-2.647,3.972-5.445,3.972c-3.332,0-6.033-2.701-6.033-6.032s2.701-6.032,6.033-6.032c1.498,0,2.866,0.549,3.921,1.453l2.814-2.814C17.503,2.988,15.139,2,12.545,2C7.021,2,2.543,6.477,2.543,12s4.478,10,10.002,10c8.396,0,10.249-7.85,9.426-11.748L12.545,10.239z"
                />
              </svg>
              <span>{{ labels().googleLoginButton }}</span>
            </button>
          </div>
        }
      </div>
    </div>
  `,
  styles: [
    `
      .login-container {
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: 100%;
        padding: 2rem 1rem;
      }

      .login-card {
        width: 100%;
        max-width: 24rem;
        background: white;
        border-radius: 0.75rem;
        box-shadow:
          0 4px 6px -1px rgba(0, 0, 0, 0.1),
          0 2px 4px -1px rgba(0, 0, 0, 0.06);
        padding: 2rem;
      }

      .login-header {
        text-align: center;
        margin-bottom: 1.5rem;
      }

      .login-logo {
        height: 3rem;
        margin-bottom: 1rem;
      }

      .login-title {
        margin: 0;
        font-size: 1.5rem;
        font-weight: 700;
        color: #111827;
      }

      .login-subtitle {
        margin: 0.5rem 0 0;
        font-size: 0.875rem;
        color: #6b7280;
      }

      .login-form {
        display: flex;
        flex-direction: column;
        gap: 1rem;
      }

      .form-group {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
      }

      .label-row {
        display: flex;
        justify-content: space-between;
        align-items: center;
      }

      .form-label {
        font-size: 0.875rem;
        font-weight: 500;
        color: #374151;
      }

      .forgot-link {
        font-size: 0.75rem;
        color: #f97316;
        text-decoration: none;
      }

      .forgot-link:hover {
        text-decoration: underline;
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

      .password-wrapper {
        position: relative;
      }

      .password-wrapper .form-input {
        padding-right: 2.5rem;
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

      .checkbox-label {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        font-size: 0.875rem;
        color: #374151;
        cursor: pointer;
      }

      .checkbox-input {
        width: 1rem;
        height: 1rem;
        accent-color: #f97316;
        cursor: pointer;
      }

      .submit-button {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 0.5rem;
        width: 100%;
        padding: 0.625rem 1rem;
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

      .register-section {
        text-align: center;
        margin-top: 1.5rem;
      }

      .register-text {
        margin: 0;
        font-size: 0.875rem;
        color: #6b7280;
      }

      .register-link {
        color: #f97316;
        text-decoration: none;
        font-weight: 500;
      }

      .register-link:hover {
        text-decoration: underline;
      }

      .divider {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        margin: 1.5rem 0;
        color: #9ca3af;
        font-size: 0.75rem;
      }

      .divider::before,
      .divider::after {
        content: "";
        flex: 1;
        height: 1px;
        background-color: #e5e7eb;
      }

      .social-login {
        display: flex;
        flex-direction: column;
        gap: 0.75rem;
      }

      .social-button {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 0.5rem;
        width: 100%;
        padding: 0.625rem 1rem;
        background-color: white;
        color: #374151;
        font-size: 0.875rem;
        font-weight: 500;
        border: 1px solid #d1d5db;
        border-radius: 0.375rem;
        cursor: pointer;
        transition: background-color 0.15s ease;
      }

      .social-button:hover {
        background-color: #f9fafb;
      }

      .social-icon {
        width: 1.25rem;
        height: 1.25rem;
      }
    `,
  ],
})
export class LoginFormComponent {
  private readonly fb = inject(FormBuilder);

  /**
   * Form configuration
   */
  readonly config = input<AuthFormConfig>(DEFAULT_AUTH_CONFIG);

  /**
   * Form labels (for i18n)
   */
  readonly labels = input<LoginFormLabels>(DEFAULT_LOGIN_LABELS_DE);

  /**
   * External loading state
   */
  readonly loading = input(false);

  /**
   * External error message
   */
  readonly error = input<string | null>(null);

  /**
   * Emitted when form is submitted with valid data
   */
  readonly formSubmit = output<LoginFormSubmitEvent>();

  /**
   * Emitted when Google login button is clicked
   */
  readonly googleLogin = output<void>();

  /**
   * Form group
   */
  readonly loginForm: FormGroup;

  /**
   * Password visibility state
   */
  readonly showPassword = signal(false);

  /**
   * Email touched state for validation display
   */
  private readonly emailTouched = signal(false);

  /**
   * Password touched state for validation display
   */
  private readonly passwordTouched = signal(false);

  constructor() {
    this.loginForm = this.fb.group({
      email: ["", [Validators.required, Validators.email]],
      password: ["", [Validators.required, Validators.minLength(8)]],
      rememberMe: [false],
    });
  }

  /**
   * Email validation error
   */
  readonly emailError = computed(() => {
    this.emailTouched();
    const email = this.loginForm.get("email");
    if (email?.hasError("required") && email?.touched) {
      return this.labels().emailRequired;
    }
    if (email?.hasError("email") && email?.touched) {
      return this.labels().emailInvalid;
    }
    return null;
  });

  /**
   * Password validation error
   */
  readonly passwordError = computed(() => {
    this.passwordTouched();
    const password = this.loginForm.get("password");
    if (password?.hasError("required") && password?.touched) {
      return this.labels().passwordRequired;
    }
    if (password?.hasError("minlength") && password?.touched) {
      return this.labels().passwordMinLength;
    }
    return null;
  });

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      this.emailTouched.set(true);
      this.passwordTouched.set(true);
      return;
    }

    const { email, password, rememberMe } = this.loginForm.value;
    this.formSubmit.emit({ email, password, rememberMe });
  }

  /**
   * Toggle password visibility
   */
  togglePasswordVisibility(): void {
    this.showPassword.set(!this.showPassword());
  }

  /**
   * Handle Google login click
   */
  onGoogleLogin(): void {
    this.googleLogin.emit();
  }

  /**
   * Reset the form
   */
  reset(): void {
    this.loginForm.reset({ rememberMe: false });
    this.emailTouched.set(false);
    this.passwordTouched.set(false);
  }
}
