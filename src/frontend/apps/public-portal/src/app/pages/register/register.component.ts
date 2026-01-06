import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
import { logError } from '@orange-car-rental/util';
import { FormHelpers, CustomValidators } from '@orange-car-rental/shared';
import {
  SuccessAlertComponent,
  ErrorAlertComponent,
  IconComponent,
  InputComponent,
  CheckboxComponent,
} from '@orange-car-rental/ui-components';
import { UI_TIMING, BUSINESS_RULES } from '../../constants/app.constants';

/**
 * Register Component
 *
 * Provides custom registration UI that integrates with Keycloak.
 * Includes validation for password strength and German data requirements.
 */
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    SuccessAlertComponent,
    ErrorAlertComponent,
    IconComponent,
    InputComponent,
    CheckboxComponent,
    TranslateModule,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private readonly translate = inject(TranslateService);

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
    private router: Router,
  ) {
    this.registerForm = this.fb.group(
      {
        // Step 1: Account Information
        email: ['', [Validators.required, Validators.email]],
        password: [
          '',
          [Validators.required, Validators.minLength(8), CustomValidators.passwordStrength()],
        ],
        confirmPassword: ['', [Validators.required]],

        // Step 2: Personal Information
        firstName: ['', [Validators.required, Validators.minLength(2)]],
        lastName: ['', [Validators.required, Validators.minLength(2)]],
        phoneNumber: ['', [Validators.required, CustomValidators.germanPhone()]],
        dateOfBirth: [
          '',
          [Validators.required, CustomValidators.minimumAge(BUSINESS_RULES.MINIMUM_RENTAL_AGE)],
        ],

        // Step 3: Terms
        acceptTerms: [false, [Validators.requiredTrue]],
        acceptPrivacy: [false, [Validators.requiredTrue]],
        acceptMarketing: [false],
      },
      { validators: CustomValidators.passwordMatch('password', 'confirmPassword') },
    );
  }

  async onSubmit(): Promise<void> {
    if (this.registerForm.invalid) {
      FormHelpers.markAllTouched(this.registerForm);
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
        acceptMarketing: formValue.acceptMarketing,
      });

      this.successMessage.set(this.translate.instant('auth.register.success'));

      // Auto-login after registration
      setTimeout(async () => {
        await this.authService.loginWithPassword(formValue.email, formValue.password);
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
        return !!(
          this.email?.valid &&
          this.password?.valid &&
          this.confirmPassword?.valid &&
          !this.registerForm.hasError('passwordMismatch')
        );
      case 2:
        return !!(
          this.firstName?.valid &&
          this.lastName?.valid &&
          this.phoneNumber?.valid &&
          this.dateOfBirth?.valid
        );
      case 3:
        return !!(this.acceptTerms?.valid && this.acceptPrivacy?.valid);
      default:
        return false;
    }
  }

  markCurrentStepTouched(): void {
    const controls = this.getCurrentStepControls();
    controls.forEach((name) => this.registerForm.get(name)?.markAsTouched());
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

  // Convenience getters
  get email() {
    return this.registerForm.get('email');
  }
  get password() {
    return this.registerForm.get('password');
  }
  get confirmPassword() {
    return this.registerForm.get('confirmPassword');
  }
  get firstName() {
    return this.registerForm.get('firstName');
  }
  get lastName() {
    return this.registerForm.get('lastName');
  }
  get phoneNumber() {
    return this.registerForm.get('phoneNumber');
  }
  get dateOfBirth() {
    return this.registerForm.get('dateOfBirth');
  }
  get acceptTerms() {
    return this.registerForm.get('acceptTerms');
  }
  get acceptPrivacy() {
    return this.registerForm.get('acceptPrivacy');
  }

  // Error getters
  get emailError(): string | null {
    if (this.email?.hasError('required') && this.email?.touched) {
      return this.translate.instant('auth.validation.emailRequired');
    }
    if (this.email?.hasError('email') && this.email?.touched) {
      return this.translate.instant('auth.validation.emailInvalid');
    }
    return null;
  }

  get passwordError(): string | null {
    if (this.password?.hasError('required') && this.password?.touched) {
      return this.translate.instant('auth.validation.passwordRequired');
    }
    if (this.password?.hasError('minlength') && this.password?.touched) {
      return this.translate.instant('auth.validation.passwordMinLength');
    }
    if (this.password?.hasError('weakPassword') && this.password?.touched) {
      return this.translate.instant('auth.register.validation.passwordWeak');
    }
    return null;
  }

  get confirmPasswordError(): string | null {
    if (this.confirmPassword?.hasError('required') && this.confirmPassword?.touched) {
      return this.translate.instant('auth.register.validation.confirmPasswordRequired');
    }
    if (this.registerForm.hasError('passwordMismatch') && this.confirmPassword?.touched) {
      return this.translate.instant('auth.register.validation.passwordMismatch');
    }
    return null;
  }

  get firstNameError(): string | null {
    if (this.firstName?.hasError('required') && this.firstName?.touched) {
      return this.translate.instant('auth.register.validation.firstNameRequired');
    }
    if (this.firstName?.hasError('minlength') && this.firstName?.touched) {
      return this.translate.instant('auth.register.validation.firstNameMinLength');
    }
    return null;
  }

  get lastNameError(): string | null {
    if (this.lastName?.hasError('required') && this.lastName?.touched) {
      return this.translate.instant('auth.register.validation.lastNameRequired');
    }
    if (this.lastName?.hasError('minlength') && this.lastName?.touched) {
      return this.translate.instant('auth.register.validation.lastNameMinLength');
    }
    return null;
  }

  get phoneNumberError(): string | null {
    if (this.phoneNumber?.hasError('required') && this.phoneNumber?.touched) {
      return this.translate.instant('auth.register.validation.phoneRequired');
    }
    if (this.phoneNumber?.hasError('invalidPhone') && this.phoneNumber?.touched) {
      return this.translate.instant('auth.register.validation.phoneInvalid');
    }
    return null;
  }

  get dateOfBirthError(): string | null {
    if (this.dateOfBirth?.hasError('required') && this.dateOfBirth?.touched) {
      return this.translate.instant('auth.register.validation.dateOfBirthRequired');
    }
    if (this.dateOfBirth?.hasError('underage') && this.dateOfBirth?.touched) {
      const minAge = this.dateOfBirth.errors?.['underage'].minAge;
      return this.translate.instant('auth.register.validation.minAge', { minAge });
    }
    return null;
  }
}
