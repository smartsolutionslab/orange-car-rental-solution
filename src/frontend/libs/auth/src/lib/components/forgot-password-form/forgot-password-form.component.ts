import { Component, input, output, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ErrorAlertComponent, SuccessAlertComponent, IconComponent } from '@orange-car-rental/ui-components';
import type {
  AuthFormConfig,
  ForgotPasswordFormSubmitEvent,
  ForgotPasswordFormLabels,
} from '../auth-forms.types';
import {
  DEFAULT_AUTH_CONFIG,
  DEFAULT_FORGOT_PASSWORD_LABELS_DE,
} from '../auth-forms.types';

/**
 * Reusable Forgot Password Form Component
 *
 * A configurable, presentational forgot password form that handles validation
 * and emits submit events. The parent component handles actual password reset.
 *
 * @example
 * <lib-forgot-password-form
 *   [labels]="customLabels"
 *   [loading]="isSending()"
 *   [error]="sendError()"
 *   [success]="emailSent()"
 *   (formSubmit)="onResetPassword($event)"
 * />
 */
@Component({
  selector: 'lib-forgot-password-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ErrorAlertComponent,
    SuccessAlertComponent,
    IconComponent,
  ],
  template: `
    <div class="forgot-password-container">
      <div class="forgot-password-card">
        <!-- Header -->
        <div class="header">
          @if (config().logoUrl) {
            <img [src]="config().logoUrl" [alt]="config().brandName || 'Logo'" class="logo" />
          }
          <div class="icon-wrapper">
            <lib-icon name="mail" variant="outline" size="lg" />
          </div>
          <h1 class="title">{{ labels().title }}</h1>
          <p class="subtitle">{{ labels().subtitle }}</p>
        </div>

        <!-- Success State -->
        @if (success()) {
          <div class="success-state">
            <ui-success-alert [message]="labels().successMessage"></ui-success-alert>
            <div class="back-link-container">
              <a [routerLink]="config().loginRoute" class="back-link">
                <lib-icon name="arrow-left" variant="outline" size="sm" />
                <span>{{ labels().backToLoginLink }}</span>
              </a>
            </div>
          </div>
        } @else {
          <!-- Error Message -->
          @if (error()) {
            <ui-error-alert [message]="error()!"></ui-error-alert>
          }

          <!-- Forgot Password Form -->
          <form [formGroup]="forgotPasswordForm" (ngSubmit)="onSubmit()" class="form">
            <!-- Email Field -->
            <div class="form-group">
              <label for="forgot-email" class="form-label">{{ labels().emailLabel }}</label>
              <input
                id="forgot-email"
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

          <!-- Back to Login -->
          <div class="back-link-container">
            <a [routerLink]="config().loginRoute" class="back-link">
              <lib-icon name="arrow-left" variant="outline" size="sm" />
              <span>{{ labels().backToLoginLink }}</span>
            </a>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .forgot-password-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100%;
      padding: 2rem 1rem;
    }

    .forgot-password-card {
      width: 100%;
      max-width: 24rem;
      background: white;
      border-radius: 0.75rem;
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
      padding: 2rem;
    }

    .header {
      text-align: center;
      margin-bottom: 1.5rem;
    }

    .logo {
      height: 3rem;
      margin-bottom: 1rem;
    }

    .icon-wrapper {
      display: flex;
      justify-content: center;
      margin-bottom: 1rem;
      color: #f97316;
    }

    .title {
      margin: 0;
      font-size: 1.5rem;
      font-weight: 700;
      color: #111827;
    }

    .subtitle {
      margin: 0.5rem 0 0;
      font-size: 0.875rem;
      color: #6b7280;
      line-height: 1.5;
    }

    .form {
      display: flex;
      flex-direction: column;
      gap: 1rem;
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
      transition: border-color 0.15s ease, box-shadow 0.15s ease;
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
      to { transform: rotate(360deg); }
    }

    .success-state {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .back-link-container {
      text-align: center;
      margin-top: 1.5rem;
    }

    .back-link {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.875rem;
      color: #6b7280;
      text-decoration: none;
      transition: color 0.15s ease;
    }

    .back-link:hover {
      color: #f97316;
    }
  `]
})
export class ForgotPasswordFormComponent {
  private readonly fb = inject(FormBuilder);

  /**
   * Form configuration
   */
  readonly config = input<AuthFormConfig>(DEFAULT_AUTH_CONFIG);

  /**
   * Form labels (for i18n)
   */
  readonly labels = input<ForgotPasswordFormLabels>(DEFAULT_FORGOT_PASSWORD_LABELS_DE);

  /**
   * External loading state
   */
  readonly loading = input(false);

  /**
   * External error message
   */
  readonly error = input<string | null>(null);

  /**
   * Whether email was sent successfully
   */
  readonly success = input(false);

  /**
   * Emitted when form is submitted with valid data
   */
  readonly formSubmit = output<ForgotPasswordFormSubmitEvent>();

  /**
   * Form group
   */
  readonly forgotPasswordForm: FormGroup;

  /**
   * Email touched state for validation display
   */
  private readonly emailTouched = signal(false);

  constructor() {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
    });
  }

  /**
   * Email validation error
   */
  readonly emailError = computed(() => {
    this.emailTouched();
    const email = this.forgotPasswordForm.get('email');
    if (email?.hasError('required') && email?.touched) {
      return this.labels().emailRequired;
    }
    if (email?.hasError('email') && email?.touched) {
      return this.labels().emailInvalid;
    }
    return null;
  });

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.forgotPasswordForm.invalid) {
      this.forgotPasswordForm.markAllAsTouched();
      this.emailTouched.set(true);
      return;
    }

    const { email } = this.forgotPasswordForm.value;
    this.formSubmit.emit({ email });
  }

  /**
   * Reset the form
   */
  reset(): void {
    this.forgotPasswordForm.reset();
    this.emailTouched.set(false);
  }
}
