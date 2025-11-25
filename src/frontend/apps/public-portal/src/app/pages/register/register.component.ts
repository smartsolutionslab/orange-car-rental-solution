import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

/**
 * Register Component
 *
 * Provides custom registration UI that integrates with Keycloak.
 * Includes validation for password strength and German data requirements.
 */
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm: FormGroup;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  showPassword = signal(false);
  showConfirmPassword = signal(false);
  currentStep = signal(1);
  totalSteps = 3;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      // Step 1: Account Information
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8), this.passwordStrengthValidator]],
      confirmPassword: ['', [Validators.required]],

      // Step 2: Personal Information
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[0-9\s\-()]{10,}$/)]],
      dateOfBirth: ['', [Validators.required, this.ageValidator(18)]],

      // Step 3: Terms
      acceptTerms: [false, [Validators.requiredTrue]],
      acceptPrivacy: [false, [Validators.requiredTrue]],
      acceptMarketing: [false]
    }, { validators: this.passwordMatchValidator });
  }

  async onSubmit(): Promise<void> {
    if (this.registerForm.invalid) {
      this.markFormGroupTouched(this.registerForm);
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      const formValue = this.registerForm.value;

      await this.authService.register({
        email: formValue.email,
        password: formValue.password,
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        phoneNumber: formValue.phoneNumber,
        dateOfBirth: formValue.dateOfBirth,
        acceptMarketing: formValue.acceptMarketing
      });

      this.successMessage.set('Registrierung erfolgreich! Sie werden in Kürze weitergeleitet...');

      // Auto-login after registration
      setTimeout(async () => {
        await this.authService.login(formValue.email, formValue.password);
        this.router.navigate(['/']);
      }, 2000);

    } catch (error: unknown) {
      console.error('Registration error:', error);

      const httpError = error as { status?: number; message?: string };
      if (httpError.status === 409) {
        this.errorMessage.set('Diese E-Mail-Adresse ist bereits registriert.');
      } else if (httpError.message?.includes('password')) {
        this.errorMessage.set('Das Passwort erfüllt nicht die Sicherheitsanforderungen.');
      } else if (httpError.message?.includes('email')) {
        this.errorMessage.set('Bitte geben Sie eine gültige E-Mail-Adresse ein.');
      } else {
        this.errorMessage.set('Registrierung fehlgeschlagen. Bitte versuchen Sie es erneut.');
      }
    } finally {
      this.isLoading.set(false);
    }
  }

  nextStep(): void {
    if (this.currentStep() < this.totalSteps) {
      // Validate current step before moving forward
      if (this.isCurrentStepValid()) {
        this.currentStep.set(this.currentStep() + 1);
      } else {
        this.markCurrentStepTouched();
      }
    }
  }

  previousStep(): void {
    if (this.currentStep() > 1) {
      this.currentStep.set(this.currentStep() - 1);
    }
  }

  isCurrentStepValid(): boolean {
    switch (this.currentStep()) {
      case 1:
        return !!(this.email?.valid && this.password?.valid && this.confirmPassword?.valid &&
                  !this.registerForm.hasError('passwordMismatch'));
      case 2:
        return !!(this.firstName?.valid && this.lastName?.valid &&
                  this.phoneNumber?.valid && this.dateOfBirth?.valid);
      case 3:
        return !!(this.acceptTerms?.valid && this.acceptPrivacy?.valid);
      default:
        return false;
    }
  }

  markCurrentStepTouched(): void {
    const controls = this.getCurrentStepControls();
    controls.forEach(name => this.registerForm.get(name)?.markAsTouched());
  }

  getCurrentStepControls(): string[] {
    switch (this.currentStep()) {
      case 1:
        return ['email', 'password', 'confirmPassword'];
      case 2:
        return ['firstName', 'lastName', 'phoneNumber', 'dateOfBirth'];
      case 3:
        return ['acceptTerms', 'acceptPrivacy'];
      default:
        return [];
    }
  }

  togglePasswordVisibility(field: 'password' | 'confirmPassword'): void {
    if (field === 'password') {
      this.showPassword.set(!this.showPassword());
    } else {
      this.showConfirmPassword.set(!this.showConfirmPassword());
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      formGroup.get(key)?.markAsTouched();
    });
  }

  // Custom Validators
  private passwordStrengthValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;

    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumeric = /[0-9]/.test(value);
    const hasSpecial = /[!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?]/.test(value);

    const passwordValid = hasUpperCase && hasLowerCase && hasNumeric && hasSpecial;

    return !passwordValid ? { weakPassword: true } : null;
  }

  private passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) return null;

    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
  }

  private ageValidator(minAge: number) {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const birthDate = new Date(control.value);
      const today = new Date();
      const age = today.getFullYear() - birthDate.getFullYear();
      const monthDiff = today.getMonth() - birthDate.getMonth();

      if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
        return age - 1 >= minAge ? null : { underage: { minAge } };
      }

      return age >= minAge ? null : { underage: { minAge } };
    };
  }

  // Convenience getters
  get email() { return this.registerForm.get('email'); }
  get password() { return this.registerForm.get('password'); }
  get confirmPassword() { return this.registerForm.get('confirmPassword'); }
  get firstName() { return this.registerForm.get('firstName'); }
  get lastName() { return this.registerForm.get('lastName'); }
  get phoneNumber() { return this.registerForm.get('phoneNumber'); }
  get dateOfBirth() { return this.registerForm.get('dateOfBirth'); }
  get acceptTerms() { return this.registerForm.get('acceptTerms'); }
  get acceptPrivacy() { return this.registerForm.get('acceptPrivacy'); }

  // Error getters
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
    if (this.password?.hasError('weakPassword') && this.password?.touched) {
      return 'Passwort muss Groß- und Kleinbuchstaben, Zahlen und Sonderzeichen enthalten';
    }
    return null;
  }

  get confirmPasswordError(): string | null {
    if (this.confirmPassword?.hasError('required') && this.confirmPassword?.touched) {
      return 'Passwortbestätigung ist erforderlich';
    }
    if (this.registerForm.hasError('passwordMismatch') && this.confirmPassword?.touched) {
      return 'Passwörter stimmen nicht überein';
    }
    return null;
  }

  get firstNameError(): string | null {
    if (this.firstName?.hasError('required') && this.firstName?.touched) {
      return 'Vorname ist erforderlich';
    }
    if (this.firstName?.hasError('minlength') && this.firstName?.touched) {
      return 'Vorname muss mindestens 2 Zeichen lang sein';
    }
    return null;
  }

  get lastNameError(): string | null {
    if (this.lastName?.hasError('required') && this.lastName?.touched) {
      return 'Nachname ist erforderlich';
    }
    if (this.lastName?.hasError('minlength') && this.lastName?.touched) {
      return 'Nachname muss mindestens 2 Zeichen lang sein';
    }
    return null;
  }

  get phoneNumberError(): string | null {
    if (this.phoneNumber?.hasError('required') && this.phoneNumber?.touched) {
      return 'Telefonnummer ist erforderlich';
    }
    if (this.phoneNumber?.hasError('pattern') && this.phoneNumber?.touched) {
      return 'Bitte geben Sie eine gültige Telefonnummer ein';
    }
    return null;
  }

  get dateOfBirthError(): string | null {
    if (this.dateOfBirth?.hasError('required') && this.dateOfBirth?.touched) {
      return 'Geburtsdatum ist erforderlich';
    }
    if (this.dateOfBirth?.hasError('underage') && this.dateOfBirth?.touched) {
      return `Sie müssen mindestens ${this.dateOfBirth.errors?.['underage'].minAge} Jahre alt sein`;
    }
    return null;
  }
}
