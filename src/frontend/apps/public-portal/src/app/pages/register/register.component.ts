import { Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
import { logError } from '@orange-car-rental/util';
import {
  RegisterFormComponent,
  type RegisterFormSubmitEvent,
  type RegisterFormLabels,
  type AuthFormConfig,
} from '@orange-car-rental/auth';
import { UI_TIMING, BUSINESS_RULES } from '../../constants/app.constants';

/**
 * Register Page Component
 *
 * Uses the shared RegisterFormComponent and handles user registration.
 */
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RegisterFormComponent],
  template: `
    <lib-register-form
      [config]="authConfig"
      [labels]="labels()"
      [loading]="isLoading()"
      [error]="errorMessage()"
      [success]="successMessage()"
      (formSubmit)="onRegister($event)"
    />
  `,
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly translate = inject(TranslateService);

  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  /**
   * Auth form configuration
   */
  readonly authConfig: AuthFormConfig = {
    showRememberMe: false,
    showSocialLogin: false,
    loginRoute: '/login',
    registerRoute: '/register',
    forgotPasswordRoute: '/forgot-password',
    termsUrl: '/terms',
    privacyUrl: '/privacy',
    minPasswordLength: 8,
    minAge: BUSINESS_RULES.MINIMUM_RENTAL_AGE,
  };

  /**
   * Computed labels from translation service
   */
  readonly labels = computed<RegisterFormLabels>(() => ({
    title: this.translate.instant('auth.register.title'),
    subtitle: this.translate.instant('auth.register.subtitle'),
    step1Title: this.translate.instant('auth.register.steps.account'),
    step2Title: this.translate.instant('auth.register.steps.personal'),
    step3Title: this.translate.instant('auth.register.steps.confirmation'),
    emailLabel: this.translate.instant('common.labels.email'),
    passwordLabel: this.translate.instant('common.labels.password'),
    confirmPasswordLabel: this.translate.instant('auth.register.confirmPassword'),
    firstNameLabel: this.translate.instant('common.labels.firstName'),
    lastNameLabel: this.translate.instant('common.labels.lastName'),
    phoneLabel: this.translate.instant('common.labels.phone'),
    dateOfBirthLabel: this.translate.instant('common.labels.dateOfBirth'),
    acceptTermsLabel: this.translate.instant('auth.register.acceptTermsPrefix'),
    acceptPrivacyLabel: this.translate.instant('auth.register.acceptPrivacyPrefix'),
    acceptMarketingLabel: this.translate.instant('auth.register.acceptMarketing'),
    nextButton: this.translate.instant('common.actions.next'),
    previousButton: this.translate.instant('common.actions.back'),
    submitButton: this.translate.instant('auth.register.submit'),
    submittingButton: this.translate.instant('auth.register.submitting'),
    hasAccountText: this.translate.instant('auth.register.hasAccount'),
    loginLink: this.translate.instant('auth.register.loginNow'),
    // Validation messages
    emailRequired: this.translate.instant('auth.validation.emailRequired'),
    emailInvalid: this.translate.instant('auth.validation.emailInvalid'),
    passwordRequired: this.translate.instant('auth.validation.passwordRequired'),
    passwordMinLength: this.translate.instant('auth.validation.passwordMinLength'),
    passwordWeak: this.translate.instant('auth.register.validation.passwordWeak'),
    confirmPasswordRequired: this.translate.instant(
      'auth.register.validation.confirmPasswordRequired',
    ),
    passwordMismatch: this.translate.instant('auth.register.validation.passwordMismatch'),
    firstNameRequired: this.translate.instant('auth.register.validation.firstNameRequired'),
    lastNameRequired: this.translate.instant('auth.register.validation.lastNameRequired'),
    phoneRequired: this.translate.instant('auth.register.validation.phoneRequired'),
    phoneInvalid: this.translate.instant('auth.register.validation.phoneInvalid'),
    dateOfBirthRequired: this.translate.instant('auth.register.validation.dateOfBirthRequired'),
    minAgeError: this.translate.instant('auth.register.validation.minAge', {
      minAge: BUSINESS_RULES.MINIMUM_RENTAL_AGE,
    }),
    termsRequired: this.translate.instant('auth.register.validation.termsRequired'),
    privacyRequired: this.translate.instant('auth.register.validation.privacyRequired'),
  }));

  /**
   * Handle registration form submission
   */
  async onRegister(event: RegisterFormSubmitEvent): Promise<void> {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      await this.authService.register({
        email: event.email,
        password: event.password,
        firstName: event.firstName,
        lastName: event.lastName,
        phoneNumber: event.phoneNumber,
        dateOfBirth: event.dateOfBirth,
        acceptMarketing: event.acceptMarketing,
      });

      this.successMessage.set(this.translate.instant('auth.register.success'));

      // Auto-login after registration
      setTimeout(async () => {
        await this.authService.loginWithPassword(event.email, event.password);
        this.router.navigate(['/']);
      }, UI_TIMING.REDIRECT_DELAY);
    } catch (error: unknown) {
      logError('RegisterComponent', 'Registration error', error);

      const httpError = error as { status?: number; message?: string };
      if (httpError.status === 409) {
        this.errorMessage.set(this.translate.instant('auth.register.errors.emailExists'));
      } else if (httpError.message?.includes('password')) {
        this.errorMessage.set(this.translate.instant('auth.register.errors.passwordWeak'));
      } else if (httpError.message?.includes('email')) {
        this.errorMessage.set(this.translate.instant('auth.register.errors.invalidEmail'));
      } else {
        this.errorMessage.set(this.translate.instant('auth.register.errors.generic'));
      }
    } finally {
      this.isLoading.set(false);
    }
  }
}
