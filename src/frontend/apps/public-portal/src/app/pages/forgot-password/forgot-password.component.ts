import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
import { logError } from '@orange-car-rental/util';
import {
  FormFieldComponent,
  SuccessAlertComponent,
  ErrorAlertComponent,
  IconComponent,
  InputComponent,
} from '@orange-car-rental/ui-components';

/**
 * Forgot Password Component
 *
 * Allows users to request a password reset link via email.
 * Integrates with Keycloak password reset functionality.
 */
@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    FormFieldComponent,
    SuccessAlertComponent,
    ErrorAlertComponent,
    IconComponent,
    InputComponent,
    TranslateModule,
  ],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css',
})
export class ForgotPasswordComponent {
  private readonly translate = inject(TranslateService);

  forgotPasswordForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  emailSent = signal(false);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
  ) {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
    });
  }

  async onSubmit(): Promise<void> {
    if (this.forgotPasswordForm.invalid) {
      this.email?.markAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    try {
      const email = this.forgotPasswordForm.value.email;

      await this.authService.resetPassword(email);

      this.emailSent.set(true);
      this.successMessage.set(this.translate.instant('auth.forgotPassword.success', { email }));

      // Reset form
      this.forgotPasswordForm.reset();
    } catch (error: unknown) {
      logError('ForgotPasswordComponent', 'Password reset error', error);

      const httpError = error as { status?: number; message?: string };
      if (httpError.status === 404) {
        // Don't reveal if email exists or not for security
        this.emailSent.set(true);
        this.successMessage.set(this.translate.instant('auth.forgotPassword.successGeneric'));
      } else if (httpError.message?.includes('Network')) {
        this.errorMessage.set(this.translate.instant('errors.network'));
      } else {
        this.errorMessage.set(this.translate.instant('auth.forgotPassword.errors.sendFailed'));
      }
    } finally {
      this.isLoading.set(false);
    }
  }

  get email() {
    return this.forgotPasswordForm.get('email');
  }

  get emailError(): string | null {
    if (this.email?.hasError('required') && this.email?.touched) {
      return this.translate.instant('auth.validation.emailRequired');
    }
    if (this.email?.hasError('email') && this.email?.touched) {
      return this.translate.instant('auth.validation.emailInvalid');
    }
    return null;
  }
}
