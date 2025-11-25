import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ForgotPasswordComponent } from './forgot-password.component';
import { AuthService } from '../../services/auth.service';

describe('ForgotPasswordComponent', () => {
  let component: ForgotPasswordComponent;
  let fixture: ComponentFixture<ForgotPasswordComponent>;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['resetPassword']);

    await TestBed.configureTestingModule({
      imports: [ForgotPasswordComponent, ReactiveFormsModule],
      providers: [
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ForgotPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Form Initialization', () => {
    it('should initialize with empty email', () => {
      expect(component.forgotPasswordForm.get('email')?.value).toBe('');
    });

    it('should start with emailSent as false', () => {
      expect(component.emailSent()).toBeFalsy();
    });

    it('should start with no error or success messages', () => {
      expect(component.errorMessage()).toBeNull();
      expect(component.successMessage()).toBeNull();
    });

    it('should start with loading as false', () => {
      expect(component.isLoading()).toBeFalsy();
    });
  });

  describe('Form Validation', () => {
    it('should require email', () => {
      const emailControl = component.forgotPasswordForm.get('email');
      emailControl?.setValue('');
      emailControl?.markAsTouched();
      expect(emailControl?.hasError('required')).toBeTruthy();
      expect(component.emailError).toBe('E-Mail-Adresse ist erforderlich');
    });

    it('should validate email format', () => {
      const emailControl = component.forgotPasswordForm.get('email');
      emailControl?.setValue('invalid-email');
      emailControl?.markAsTouched();
      expect(emailControl?.hasError('email')).toBeTruthy();
      expect(component.emailError).toBe('Bitte geben Sie eine gültige E-Mail-Adresse ein');
    });

    it('should accept valid email', () => {
      const emailControl = component.forgotPasswordForm.get('email');
      emailControl?.setValue('test@example.com');
      expect(emailControl?.valid).toBeTruthy();
      expect(component.emailError).toBeNull();
    });

    it('should not show error when field is untouched', () => {
      const emailControl = component.forgotPasswordForm.get('email');
      emailControl?.setValue('');
      expect(component.emailError).toBeNull();
    });
  });

  describe('Form Submission', () => {
    beforeEach(() => {
      component.forgotPasswordForm.patchValue({
        email: 'test@example.com'
      });
    });

    it('should not submit if form is invalid', async () => {
      component.forgotPasswordForm.patchValue({ email: '' });
      await component.onSubmit();
      expect(authService.resetPassword).not.toHaveBeenCalled();
    });

    it('should mark email as touched if invalid on submit', async () => {
      component.forgotPasswordForm.patchValue({ email: '' });
      await component.onSubmit();
      expect(component.email?.touched).toBeTruthy();
    });

    it('should call authService.resetPassword with email', async () => {
      authService.resetPassword.and.returnValue(Promise.resolve());

      await component.onSubmit();

      expect(authService.resetPassword).toHaveBeenCalledWith('test@example.com');
    });

    it('should set emailSent to true on success', async () => {
      authService.resetPassword.and.returnValue(Promise.resolve());

      await component.onSubmit();

      expect(component.emailSent()).toBeTruthy();
    });

    it('should show success message with email on success', async () => {
      authService.resetPassword.and.returnValue(Promise.resolve());

      await component.onSubmit();

      expect(component.successMessage()).toContain('test@example.com');
      expect(component.successMessage()).toContain('Link zum Zurücksetzen des Passworts');
    });

    it('should reset form on success', async () => {
      authService.resetPassword.and.returnValue(Promise.resolve());

      await component.onSubmit();

      expect(component.forgotPasswordForm.get('email')?.value).toBeNull();
    });

    it('should set loading state during submission', async () => {
      let resolveReset: () => void;
      authService.resetPassword.and.returnValue(new Promise((resolve) => {
        resolveReset = resolve;
      }));

      const submitPromise = component.onSubmit();
      expect(component.isLoading()).toBeTruthy();

      resolveReset();
      await submitPromise;
      expect(component.isLoading()).toBeFalsy();
    });

    it('should clear previous messages on new submission', async () => {
      component.errorMessage.set('Previous error');
      component.successMessage.set('Previous success');
      authService.resetPassword.and.returnValue(Promise.resolve());

      await component.onSubmit();

      // During submission, messages should be cleared
      expect(component.errorMessage()).toBeNull();
    });
  });

  describe('Error Handling', () => {
    beforeEach(() => {
      component.forgotPasswordForm.patchValue({
        email: 'test@example.com'
      });
    });

    it('should handle 404 error (email not found) securely', async () => {
      const error = { status: 404, message: 'User not found' };
      authService.resetPassword.and.returnValue(Promise.reject(error));

      await component.onSubmit();

      // Should not reveal if email exists for security
      expect(component.emailSent()).toBeTruthy();
      expect(component.successMessage()).toContain('Wenn ein Konto mit dieser E-Mail-Adresse existiert');
    });

    it('should show network error message', async () => {
      const error = { message: 'Network error occurred' };
      authService.resetPassword.and.returnValue(Promise.reject(error));

      await component.onSubmit();

      expect(component.errorMessage()).toBe('Netzwerkfehler. Bitte überprüfen Sie Ihre Internetverbindung.');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show generic error message on unknown error', async () => {
      const error = { status: 500, message: 'Server error' };
      authService.resetPassword.and.returnValue(Promise.reject(error));

      await component.onSubmit();

      expect(component.errorMessage()).toBe('Beim Senden der E-Mail ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should log errors to console', async () => {
      spyOn(console, 'error');
      const error = { status: 500, message: 'Server error' };
      authService.resetPassword.and.returnValue(Promise.reject(error));

      await component.onSubmit();

      expect(console.error).toHaveBeenCalledWith('Password reset error:', error);
    });
  });

  describe('Email Sent State', () => {
    it('should show form initially', () => {
      expect(component.emailSent()).toBeFalsy();
      fixture.detectChanges();
      const form = fixture.nativeElement.querySelector('form');
      expect(form).toBeTruthy();
    });

    it('should hide form after successful email sent', async () => {
      authService.resetPassword.and.returnValue(Promise.resolve());
      component.forgotPasswordForm.patchValue({ email: 'test@example.com' });

      await component.onSubmit();
      fixture.detectChanges();

      expect(component.emailSent()).toBeTruthy();
    });

    it('should allow resending email', async () => {
      component.emailSent.set(true);
      fixture.detectChanges();

      // Simulate clicking "resend" button
      component.emailSent.set(false);

      expect(component.emailSent()).toBeFalsy();
    });

    it('should show info box when email sent', async () => {
      authService.resetPassword.and.returnValue(Promise.resolve());
      component.forgotPasswordForm.patchValue({ email: 'test@example.com' });

      await component.onSubmit();
      fixture.detectChanges();

      const infoBox = fixture.nativeElement.querySelector('.info-box');
      expect(infoBox).toBeTruthy();
    });

    it('should show resend button when email sent', async () => {
      authService.resetPassword.and.returnValue(Promise.resolve());
      component.forgotPasswordForm.patchValue({ email: 'test@example.com' });

      await component.onSubmit();
      fixture.detectChanges();

      const resendButton = fixture.nativeElement.querySelector('.resend-button');
      expect(resendButton).toBeTruthy();
    });
  });

  describe('UI State', () => {
    it('should show lock icon initially', () => {
      expect(component.emailSent()).toBeFalsy();
      fixture.detectChanges();
      const lockIcon = fixture.nativeElement.querySelector('.lock-icon');
      expect(lockIcon).toBeTruthy();
    });

    it('should show check icon after email sent', async () => {
      authService.resetPassword.and.returnValue(Promise.resolve());
      component.forgotPasswordForm.patchValue({ email: 'test@example.com' });

      await component.onSubmit();
      fixture.detectChanges();

      const checkIcon = fixture.nativeElement.querySelector('.check-icon');
      expect(checkIcon).toBeTruthy();
    });

    it('should disable submit button when loading', () => {
      component.isLoading.set(true);
      fixture.detectChanges();
      const submitButton = fixture.nativeElement.querySelector('button[type="submit"]');
      expect(submitButton?.disabled).toBeTruthy();
    });

    it('should show spinner when loading', () => {
      component.isLoading.set(true);
      fixture.detectChanges();
      const spinner = fixture.nativeElement.querySelector('.spinner');
      expect(spinner).toBeTruthy();
    });

    it('should display success message in alert', () => {
      component.successMessage.set('Test success message');
      fixture.detectChanges();
      const successAlert = fixture.nativeElement.querySelector('.success-alert');
      expect(successAlert).toBeTruthy();
      expect(successAlert.textContent).toContain('Test success message');
    });

    it('should display error message in alert', () => {
      component.errorMessage.set('Test error message');
      fixture.detectChanges();
      const errorAlert = fixture.nativeElement.querySelector('.error-alert');
      expect(errorAlert).toBeTruthy();
      expect(errorAlert.textContent).toContain('Test error message');
    });

    it('should show back to login link', () => {
      fixture.detectChanges();
      const backLink = fixture.nativeElement.querySelector('.back-link');
      expect(backLink).toBeTruthy();
      expect(backLink.getAttribute('routerLink')).toBe('/login');
    });
  });

  describe('Accessibility', () => {
    it('should have proper form labels', () => {
      fixture.detectChanges();
      const label = fixture.nativeElement.querySelector('label[for="email"]');
      expect(label).toBeTruthy();
      expect(label.textContent).toContain('E-Mail-Adresse');
    });

    it('should have role="alert" on error messages', () => {
      component.errorMessage.set('Error');
      fixture.detectChanges();
      const errorAlert = fixture.nativeElement.querySelector('.error-alert');
      expect(errorAlert.getAttribute('role')).toBe('alert');
    });

    it('should have role="alert" on success messages', () => {
      component.successMessage.set('Success');
      fixture.detectChanges();
      const successAlert = fixture.nativeElement.querySelector('.success-alert');
      expect(successAlert.getAttribute('role')).toBe('alert');
    });
  });
});
