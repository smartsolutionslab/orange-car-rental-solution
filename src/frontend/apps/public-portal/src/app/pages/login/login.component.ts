import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
import { logError } from '@orange-car-rental/util';
import { FormHelpers } from '@orange-car-rental/shared';
import { ErrorAlertComponent, IconComponent } from '@orange-car-rental/ui-components';

/**
 * Login Component
 *
 * Provides custom login UI that integrates with Keycloak.
 * Users can login with email and password.
 */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    TranslateModule,
    ErrorAlertComponent,
    IconComponent,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly translate = inject(TranslateService);

  loginForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  showPassword = signal(false);

  /** Signal to track if email field has been touched */
  private readonly emailTouched = signal(false);
  /** Signal to track if password field has been touched */
  private readonly passwordTouched = signal(false);

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      rememberMe: [false],
    });
  }

  async onSubmit(): Promise<void> {
    if (this.loginForm.invalid) {
      FormHelpers.markAllTouched(this.loginForm);
      this.emailTouched.set(true);
      this.passwordTouched.set(true);
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      const { email, password, rememberMe } = this.loginForm.value;

      await this.authService.loginWithPassword(email, password, rememberMe);

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

  togglePasswordVisibility(): void {
    this.showPassword.set(!this.showPassword());
  }

  private getReturnUrl(): string {
    // Check for return URL in query params
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('returnUrl') || '/';
  }

  // Convenience getters for template
  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  /** Computed signal for email validation error */
  readonly emailError = computed(() => {
    // Access signal to create reactivity
    this.emailTouched();
    const email = this.email;
    if (email?.hasError('required') && email?.touched) {
      return this.translate.instant('auth.validation.emailRequired');
    }
    if (email?.hasError('email') && email?.touched) {
      return this.translate.instant('auth.validation.emailInvalid');
    }
    return null;
  });

  /** Computed signal for password validation error */
  readonly passwordError = computed(() => {
    // Access signal to create reactivity
    this.passwordTouched();
    const password = this.password;
    if (password?.hasError('required') && password?.touched) {
      return this.translate.instant('auth.validation.passwordRequired');
    }
    if (password?.hasError('minlength') && password?.touched) {
      return this.translate.instant('auth.validation.passwordMinLength');
    }
    return null;
  });
}
