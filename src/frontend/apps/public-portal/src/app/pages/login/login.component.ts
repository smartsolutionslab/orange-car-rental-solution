import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

/**
 * Login Component
 *
 * Provides custom login UI that integrates with Keycloak.
 * Users can login with email and password.
 */
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  showPassword = signal(false);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      rememberMe: [false]
    });
  }

  async onSubmit(): Promise<void> {
    if (this.loginForm.invalid) {
      this.markFormGroupTouched(this.loginForm);
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      const { email, password, rememberMe } = this.loginForm.value;

      await this.authService.login(email, password, rememberMe);

      // Redirect to intended page or home
      const returnUrl = this.getReturnUrl();
      this.router.navigate([returnUrl]);
    } catch (error: unknown) {
      console.error('Login error:', error);

      // Handle specific error cases
      const httpError = error as { status?: number; message?: string };
      if (httpError.status === 401) {
        this.errorMessage.set('Ungültige E-Mail-Adresse oder Passwort');
      } else if (httpError.status === 403) {
        this.errorMessage.set('Ihr Konto wurde gesperrt. Bitte kontaktieren Sie den Support.');
      } else if (httpError.message?.includes('Network')) {
        this.errorMessage.set('Netzwerkfehler. Bitte überprüfen Sie Ihre Internetverbindung.');
      } else {
        this.errorMessage.set('Anmeldung fehlgeschlagen. Bitte versuchen Sie es erneut.');
      }
    } finally {
      this.isLoading.set(false);
    }
  }

  togglePasswordVisibility(): void {
    this.showPassword.set(!this.showPassword());
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
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

  get emailError(): string | null {
    if (this.email?.hasError('required') && this.email?.touched) {
      return 'E-Mail-Adresse ist erforderlich';
    }
    if (this.email?.hasError('email') && this.email?.touched) {
      return 'Bitte geben Sie eine gültige E-Mail-Adresse ein';
    }
    return null;
  }

  get passwordError(): string | null {
    if (this.password?.hasError('required') && this.password?.touched) {
      return 'Passwort ist erforderlich';
    }
    if (this.password?.hasError('minlength') && this.password?.touched) {
      return 'Passwort muss mindestens 8 Zeichen lang sein';
    }
    return null;
  }
}
