import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

/**
 * Forgot Password Component
 *
 * Allows users to request a password reset link via email.
 * Integrates with Keycloak password reset functionality.
 */
@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {
  forgotPasswordForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  emailSent = signal(false);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService
  ) {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
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
      this.successMessage.set(
        `Ein Link zum Zurücksetzen des Passworts wurde an ${email} gesendet. ` +
        `Bitte überprüfen Sie Ihren Posteingang und folgen Sie den Anweisungen.`
      );

      // Reset form
      this.forgotPasswordForm.reset();

    } catch (error: unknown) {
      console.error('Password reset error:', error);

      const httpError = error as { status?: number; message?: string };
      if (httpError.status === 404) {
        // Don't reveal if email exists or not for security
        this.emailSent.set(true);
        this.successMessage.set(
          `Wenn ein Konto mit dieser E-Mail-Adresse existiert, wurde ein Link zum Zurücksetzen des Passworts gesendet.`
        );
      } else if (httpError.message?.includes('Network')) {
        this.errorMessage.set('Netzwerkfehler. Bitte überprüfen Sie Ihre Internetverbindung.');
      } else {
        this.errorMessage.set(
          'Beim Senden der E-Mail ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.'
        );
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
      return 'E-Mail-Adresse ist erforderlich';
    }
    if (this.email?.hasError('email') && this.email?.touched) {
      return 'Bitte geben Sie eine gültige E-Mail-Adresse ein';
    }
    return null;
  }
}
