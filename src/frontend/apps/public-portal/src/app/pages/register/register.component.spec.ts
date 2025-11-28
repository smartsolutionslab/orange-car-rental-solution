import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, provideRouter } from '@angular/router';
import { RegisterComponent } from './register.component';
import { AuthService } from '../../services/auth.service';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['register']);

    await TestBed.configureTestingModule({
      imports: [RegisterComponent, ReactiveFormsModule],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        provideRouter([])
      ]
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router);
    navigateSpy = spyOn(router, 'navigate').and.returnValue(Promise.resolve(true));
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Form Initialization', () => {
    it('should initialize at step 1', () => {
      expect(component.currentStep()).toBe(1);
    });

    it('should have 3 total steps', () => {
      expect(component.totalSteps).toBe(3);
    });

    it('should initialize with all form controls', () => {
      expect(component.registerForm.get('email')).toBeDefined();
      expect(component.registerForm.get('password')).toBeDefined();
      expect(component.registerForm.get('confirmPassword')).toBeDefined();
      expect(component.registerForm.get('firstName')).toBeDefined();
      expect(component.registerForm.get('lastName')).toBeDefined();
      expect(component.registerForm.get('phoneNumber')).toBeDefined();
      expect(component.registerForm.get('dateOfBirth')).toBeDefined();
      expect(component.registerForm.get('acceptTerms')).toBeDefined();
      expect(component.registerForm.get('acceptPrivacy')).toBeDefined();
      expect(component.registerForm.get('acceptMarketing')).toBeDefined();
    });
  });

  describe('Step 1 - Account Information Validation', () => {
    it('should require email', () => {
      const emailControl = component.registerForm.get('email');
      emailControl?.setValue('');
      emailControl?.markAsTouched();
      expect(emailControl?.hasError('required')).toBeTruthy();
    });

    it('should validate email format', () => {
      const emailControl = component.registerForm.get('email');
      emailControl?.setValue('invalid-email');
      emailControl?.markAsTouched();
      expect(emailControl?.hasError('email')).toBeTruthy();
    });

    it('should require password with minimum length', () => {
      const passwordControl = component.registerForm.get('password');
      passwordControl?.setValue('short');
      expect(passwordControl?.hasError('minlength')).toBeTruthy();
    });

    it('should validate password strength', () => {
      const passwordControl = component.registerForm.get('password');
      passwordControl?.setValue('weakpassword');
      expect(passwordControl?.hasError('weakPassword')).toBeTruthy();
    });

    it('should accept strong password', () => {
      const passwordControl = component.registerForm.get('password');
      passwordControl?.setValue('StrongP@ssw0rd!');
      expect(passwordControl?.valid).toBeTruthy();
    });

    it('should validate password match', () => {
      component.registerForm.patchValue({
        password: 'StrongP@ssw0rd!',
        confirmPassword: 'DifferentP@ssw0rd!'
      });
      expect(component.registerForm.hasError('passwordMismatch')).toBeTruthy();
    });

    it('should pass validation when passwords match', () => {
      component.registerForm.patchValue({
        password: 'StrongP@ssw0rd!',
        confirmPassword: 'StrongP@ssw0rd!'
      });
      expect(component.registerForm.hasError('passwordMismatch')).toBeFalsy();
    });

    it('should show password mismatch error', () => {
      component.registerForm.patchValue({
        password: 'StrongP@ssw0rd!',
        confirmPassword: 'Different'
      });
      component.confirmPassword?.markAsTouched();
      expect(component.confirmPasswordError).toBe('Passwörter stimmen nicht überein');
    });
  });

  describe('Step 2 - Personal Information Validation', () => {
    it('should require first name', () => {
      const firstNameControl = component.registerForm.get('firstName');
      firstNameControl?.setValue('');
      firstNameControl?.markAsTouched();
      expect(firstNameControl?.hasError('required')).toBeTruthy();
    });

    it('should require last name', () => {
      const lastNameControl = component.registerForm.get('lastName');
      lastNameControl?.setValue('');
      lastNameControl?.markAsTouched();
      expect(lastNameControl?.hasError('required')).toBeTruthy();
    });

    it('should validate phone number format', () => {
      const phoneControl = component.registerForm.get('phoneNumber');
      phoneControl?.setValue('invalid');
      expect(phoneControl?.hasError('pattern')).toBeTruthy();
    });

    it('should accept valid German phone number', () => {
      const phoneControl = component.registerForm.get('phoneNumber');
      phoneControl?.setValue('+49 123 456789');
      expect(phoneControl?.valid).toBeTruthy();
    });

    it('should require date of birth', () => {
      const dobControl = component.registerForm.get('dateOfBirth');
      dobControl?.setValue('');
      dobControl?.markAsTouched();
      expect(dobControl?.hasError('required')).toBeTruthy();
    });

    it('should validate minimum age of 18', () => {
      const dobControl = component.registerForm.get('dateOfBirth');
      const today = new Date();
      const underageDate = new Date(today.getFullYear() - 17, today.getMonth(), today.getDate());
      const dateString = underageDate.toISOString().split('T')[0];
      dobControl?.setValue(dateString);
      expect(dobControl?.hasError('underage')).toBeTruthy();
    });

    it('should accept valid age (18+)', () => {
      const dobControl = component.registerForm.get('dateOfBirth');
      const today = new Date();
      const validDate = new Date(today.getFullYear() - 25, today.getMonth(), today.getDate());
      const dateString = validDate.toISOString().split('T')[0];
      dobControl?.setValue(dateString);
      expect(dobControl?.valid).toBeTruthy();
    });
  });

  describe('Step 3 - Terms and Conditions', () => {
    it('should require terms acceptance', () => {
      const termsControl = component.registerForm.get('acceptTerms');
      termsControl?.setValue(false);
      // Validators.requiredTrue produces 'required' error when value is false
      expect(termsControl?.hasError('required')).toBeTruthy();
    });

    it('should require privacy acceptance', () => {
      const privacyControl = component.registerForm.get('acceptPrivacy');
      privacyControl?.setValue(false);
      // Validators.requiredTrue produces 'required' error when value is false
      expect(privacyControl?.hasError('required')).toBeTruthy();
    });

    it('should not require marketing acceptance', () => {
      const marketingControl = component.registerForm.get('acceptMarketing');
      marketingControl?.setValue(false);
      expect(marketingControl?.valid).toBeTruthy();
    });
  });

  describe('Step Navigation', () => {
    it('should not advance to step 2 with invalid step 1', () => {
      component.nextStep();
      expect(component.currentStep()).toBe(1);
    });

    it('should advance to step 2 with valid step 1', () => {
      component.registerForm.patchValue({
        email: 'test@example.com',
        password: 'StrongP@ssw0rd!',
        confirmPassword: 'StrongP@ssw0rd!'
      });
      component.nextStep();
      expect(component.currentStep()).toBe(2);
    });

    it('should go back from step 2 to step 1', () => {
      component.currentStep.set(2);
      component.previousStep();
      expect(component.currentStep()).toBe(1);
    });

    it('should not go back below step 1', () => {
      component.currentStep.set(1);
      component.previousStep();
      expect(component.currentStep()).toBe(1);
    });

    it('should validate current step correctly', () => {
      component.registerForm.patchValue({
        email: 'test@example.com',
        password: 'StrongP@ssw0rd!',
        confirmPassword: 'StrongP@ssw0rd!'
      });
      expect(component.isCurrentStepValid()).toBeTruthy();
    });

    it('should not advance past total steps', () => {
      component.currentStep.set(3);
      component.nextStep();
      expect(component.currentStep()).toBe(3);
    });
  });

  describe('Password Visibility Toggle', () => {
    it('should start with passwords hidden', () => {
      expect(component.showPassword()).toBeFalsy();
      expect(component.showConfirmPassword()).toBeFalsy();
    });

    it('should toggle password visibility', () => {
      component.togglePasswordVisibility('password');
      expect(component.showPassword()).toBeTruthy();
      component.togglePasswordVisibility('password');
      expect(component.showPassword()).toBeFalsy();
    });

    it('should toggle confirm password visibility', () => {
      component.togglePasswordVisibility('confirmPassword');
      expect(component.showConfirmPassword()).toBeTruthy();
      component.togglePasswordVisibility('confirmPassword');
      expect(component.showConfirmPassword()).toBeFalsy();
    });
  });

  describe('Form Submission', () => {
    beforeEach(() => {
      // Fill in all required fields
      const today = new Date();
      const validDate = new Date(today.getFullYear() - 25, today.getMonth(), today.getDate());

      component.registerForm.patchValue({
        email: 'test@example.com',
        password: 'StrongP@ssw0rd!',
        confirmPassword: 'StrongP@ssw0rd!',
        firstName: 'Max',
        lastName: 'Mustermann',
        phoneNumber: '+49 123 456789',
        dateOfBirth: validDate.toISOString().split('T')[0],
        acceptTerms: true,
        acceptPrivacy: true,
        acceptMarketing: false
      });
    });

    it('should not submit if form is invalid regardless of step', async () => {
      // The component checks form.invalid, not step number
      component.currentStep.set(2);
      component.registerForm.patchValue({ acceptTerms: false }); // Make form invalid
      await component.onSubmit();
      expect(authService.register).not.toHaveBeenCalled();
    });

    it('should not submit if form is invalid', async () => {
      component.registerForm.patchValue({ acceptTerms: false });
      component.currentStep.set(3);
      await component.onSubmit();
      expect(authService.register).not.toHaveBeenCalled();
    });

    it('should call authService.register with correct data', async () => {
      component.currentStep.set(3);
      authService.register.and.returnValue(Promise.resolve());
      // navigateSpy already returns Promise.resolve(true) from beforeEach

      await component.onSubmit();

      expect(authService.register).toHaveBeenCalledWith(jasmine.objectContaining({
        email: 'test@example.com',
        firstName: 'Max',
        lastName: 'Mustermann',
        phoneNumber: '+49 123 456789',
        acceptMarketing: false
      }));
    });

    it('should show success message on successful registration', async () => {
      component.currentStep.set(3);
      authService.register.and.returnValue(Promise.resolve());

      await component.onSubmit();

      // Component shows German message on success
      expect(component.successMessage()).toContain('Registrierung erfolgreich');
    });

    it('should set loading state during submission', async () => {
      component.currentStep.set(3);
      // eslint-disable-next-line @typescript-eslint/no-empty-function
      let resolveRegister: () => void = () => {};
      authService.register.and.returnValue(new Promise((resolve) => {
        resolveRegister = resolve;
      }));

      const submitPromise = component.onSubmit();
      expect(component.isLoading()).toBeTruthy();

      resolveRegister();
      await submitPromise;
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show error message on email already exists', async () => {
      component.currentStep.set(3);
      const error = { status: 409, message: 'Email already exists' };
      authService.register.and.returnValue(Promise.reject(error));

      await component.onSubmit();

      expect(component.errorMessage()).toBe('Diese E-Mail-Adresse ist bereits registriert.');
    });

    it('should show error message on password error', async () => {
      component.currentStep.set(3);
      const error = { status: 400, message: 'password requirements not met' };
      authService.register.and.returnValue(Promise.reject(error));

      await component.onSubmit();

      expect(component.errorMessage()).toBe('Das Passwort erfüllt nicht die Sicherheitsanforderungen.');
    });

    it('should show generic error message on unknown error', async () => {
      component.currentStep.set(3);
      const error = { status: 500, message: 'Server error' };
      authService.register.and.returnValue(Promise.reject(error));

      await component.onSubmit();

      expect(component.errorMessage()).toBe('Registrierung fehlgeschlagen. Bitte versuchen Sie es erneut.');
    });
  });

  describe('UI State', () => {
    it('should show step 1 form fields initially', () => {
      expect(component.currentStep()).toBe(1);
      fixture.detectChanges();
      const emailInput = fixture.nativeElement.querySelector('#email');
      expect(emailInput).toBeTruthy();
    });

    it('should display progress steps', () => {
      fixture.detectChanges();
      const progressSteps = fixture.nativeElement.querySelectorAll('.progress-step');
      expect(progressSteps.length).toBe(3);
    });

    it('should disable submit button when loading', () => {
      component.currentStep.set(3);
      component.isLoading.set(true);
      fixture.detectChanges();
      const submitButton = fixture.nativeElement.querySelector('button[type="submit"]');
      expect(submitButton?.disabled).toBeTruthy();
    });
  });
});
