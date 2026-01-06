import { Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
import { logError } from '@orange-car-rental/util';
import {
  LoginFormComponent,
  type LoginFormSubmitEvent,
  type LoginFormLabels,
  type AuthFormConfig,
} from '@orange-car-rental/auth';

/**
 * Login Page Component
 *
 * Uses the shared LoginFormComponent and handles authentication.
 */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [LoginFormComponent],
  template: `
    <lib-login-form
      [config]="authConfig"
      [labels]="labels()"
      [loading]="isLoading()"
      [error]="errorMessage()"
      (formSubmit)="onLogin($event)"
    />
  `,
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly translate = inject(TranslateService);

  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  /**
   * Auth form configuration
   */
  readonly authConfig: AuthFormConfig = {
    showRememberMe: true,
    showSocialLogin: false,
    loginRoute: '/login',
    registerRoute: '/register',
    forgotPasswordRoute: '/forgot-password',
    minPasswordLength: 8,
  };

  /**
   * Computed labels from translation service
   */
  readonly labels = computed<LoginFormLabels>(() => ({
    title: this.translate.instant('auth.login.title'),
    subtitle: this.translate.instant('auth.login.subtitle'),
    emailLabel: this.translate.instant('auth.login.email'),
    emailPlaceholder: this.translate.instant('common.placeholders.email'),
    passwordLabel: this.translate.instant('auth.login.password'),
    forgotPasswordLink: this.translate.instant('auth.login.forgotPassword'),
    rememberMeLabel: this.translate.instant('auth.login.rememberMe'),
    submitButton: this.translate.instant('auth.login.submit'),
    submittingButton: this.translate.instant('auth.login.submitting'),
    noAccountText: this.translate.instant('auth.login.noAccount'),
    registerLink: this.translate.instant('auth.login.registerNow'),
    orDivider: this.translate.instant('auth.login.or'),
    googleLoginButton: this.translate.instant('auth.login.loginWithGoogle'),
    showPassword: this.translate.instant('common.actions.showPassword'),
    hidePassword: this.translate.instant('common.actions.hidePassword'),
    emailRequired: this.translate.instant('auth.validation.emailRequired'),
    emailInvalid: this.translate.instant('auth.validation.emailInvalid'),
    passwordRequired: this.translate.instant('auth.validation.passwordRequired'),
    passwordMinLength: this.translate.instant('auth.validation.passwordMinLength'),
  }));

  /**
   * Handle login form submission
   */
  async onLogin(event: LoginFormSubmitEvent): Promise<void> {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      await this.authService.loginWithPassword(event.email, event.password, event.rememberMe);

      // Get role-based redirect URL
      const returnUrl = this.getReturnUrl();
      const redirectUrl = this.authService.getPostLoginRedirect(returnUrl);
      this.router.navigate([redirectUrl]);
    } catch (error: unknown) {
      logError('LoginComponent', 'Login error', error);

      // Handle specific error cases
      const httpError = error as { status?: number; message?: string };
      if (httpError.status === 401) {
        this.errorMessage.set(this.translate.instant('auth.errors.invalidCredentials'));
      } else if (httpError.status === 403) {
        this.errorMessage.set(this.translate.instant('auth.errors.accountLocked'));
      } else if (httpError.message?.includes('Network')) {
        this.errorMessage.set(this.translate.instant('auth.errors.networkError'));
      } else {
        this.errorMessage.set(this.translate.instant('auth.errors.loginFailed'));
      }
    } finally {
      this.isLoading.set(false);
    }
  }

  private getReturnUrl(): string {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('returnUrl') || '/';
  }
}
