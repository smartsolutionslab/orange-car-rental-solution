import { Component, computed, inject, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
import { logError } from '@orange-car-rental/util';
import {
  ForgotPasswordFormComponent,
  type ForgotPasswordFormSubmitEvent,
  type ForgotPasswordFormLabels,
  type AuthFormConfig,
} from '@orange-car-rental/auth';

/**
 * Forgot Password Page Component
 *
 * Uses the shared ForgotPasswordFormComponent and handles password reset requests.
 * Integrates with Keycloak password reset functionality.
 */
@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [ForgotPasswordFormComponent],
  template: `
    <lib-forgot-password-form
      [config]="authConfig"
      [labels]="labels()"
      [loading]="isLoading()"
      [error]="errorMessage()"
      [success]="emailSent()"
      (formSubmit)="onSubmit($event)"
    />
  `,
})
export class ForgotPasswordComponent {
  private readonly authService = inject(AuthService);
  private readonly translate = inject(TranslateService);

  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly emailSent = signal(false);

  /**
   * Auth form configuration
   */
  readonly authConfig: AuthFormConfig = {
    showRememberMe: false,
    showSocialLogin: false,
    loginRoute: '/login',
    registerRoute: '/register',
    forgotPasswordRoute: '/forgot-password',
  };

  /**
   * Computed labels from translation service
   */
  readonly labels = computed<ForgotPasswordFormLabels>(() => ({
    title: this.translate.instant('auth.forgotPassword.title'),
    subtitle: this.translate.instant('auth.forgotPassword.subtitle'),
    emailLabel: this.translate.instant('common.labels.email'),
    emailPlaceholder: this.translate.instant('common.placeholders.email'),
    submitButton: this.translate.instant('auth.forgotPassword.submit'),
    submittingButton: this.translate.instant('auth.forgotPassword.submitting'),
    backToLoginLink: this.translate.instant('auth.forgotPassword.backToLogin'),
    successTitle: this.translate.instant('auth.forgotPassword.successTitle'),
    successMessage: this.translate.instant('auth.forgotPassword.successMessage'),
    emailRequired: this.translate.instant('auth.validation.emailRequired'),
    emailInvalid: this.translate.instant('auth.validation.emailInvalid'),
  }));

  /**
   * Handle forgot password form submission
   */
  async onSubmit(event: ForgotPasswordFormSubmitEvent): Promise<void> {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      await this.authService.resetPassword(event.email);

      // Always show success message (don't reveal if email exists)
      this.emailSent.set(true);
    } catch (error: unknown) {
      logError('ForgotPasswordComponent', 'Password reset error', error);

      const httpError = error as { status?: number; message?: string };
      if (httpError.status === 404) {
        // Don't reveal if email exists or not for security
        this.emailSent.set(true);
      } else if (httpError.message?.includes('Network')) {
        this.errorMessage.set(this.translate.instant('errors.network'));
      } else {
        this.errorMessage.set(this.translate.instant('auth.forgotPassword.errors.sendFailed'));
      }
    } finally {
      this.isLoading.set(false);
    }
  }
}
