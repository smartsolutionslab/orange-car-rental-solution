import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ForgotPasswordComponent } from './forgot-password.component';
import { AuthService } from '../../services/auth.service';
import { TEST_EMAILS } from '@orange-car-rental/shared/testing';

describe('ForgotPasswordComponent', () => {
  let component: ForgotPasswordComponent;
  let fixture: ComponentFixture<ForgotPasswordComponent>;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['resetPassword']);

    await TestBed.configureTestingModule({
      imports: [ForgotPasswordComponent, TranslateModule.forRoot()],
      providers: [{ provide: AuthService, useValue: authServiceSpy }, provideRouter([])],
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

  describe('Initial State', () => {
    it('should start with emailSent as false', () => {
      expect(component.emailSent()).toBeFalsy();
    });

    it('should start with no error message', () => {
      expect(component.errorMessage()).toBeNull();
    });

    it('should start with loading as false', () => {
      expect(component.isLoading()).toBeFalsy();
    });
  });

  describe('Labels Configuration', () => {
    it('should provide labels to the form', () => {
      const labels = component.labels();
      expect(labels.title).toBe('auth.forgotPassword.title');
      expect(labels.subtitle).toBe('auth.forgotPassword.subtitle');
      expect(labels.emailLabel).toBe('common.labels.email');
      expect(labels.submitButton).toBe('auth.forgotPassword.submit');
      expect(labels.backToLoginLink).toBe('auth.forgotPassword.backToLogin');
    });

    it('should provide validation labels', () => {
      const labels = component.labels();
      expect(labels.emailRequired).toBe('auth.validation.emailRequired');
      expect(labels.emailInvalid).toBe('auth.validation.emailInvalid');
    });
  });

  describe('Auth Config', () => {
    it('should configure login route', () => {
      expect(component.authConfig.loginRoute).toBe('/login');
    });

    it('should configure register route', () => {
      expect(component.authConfig.registerRoute).toBe('/register');
    });

    it('should disable remember me', () => {
      expect(component.authConfig.showRememberMe).toBeFalsy();
    });
  });

  describe('Form Submission', () => {
    it('should call authService.resetPassword with email', async () => {
      authService.resetPassword.and.returnValue(Promise.resolve());

      await component.onSubmit({ email: TEST_EMAILS.VALID });

      expect(authService.resetPassword).toHaveBeenCalledWith(TEST_EMAILS.VALID);
    });

    it('should set emailSent to true on success', async () => {
      authService.resetPassword.and.returnValue(Promise.resolve());

      await component.onSubmit({ email: TEST_EMAILS.VALID });

      expect(component.emailSent()).toBeTruthy();
    });

    it('should set loading state during submission', async () => {
      let resolveReset!: () => void;
      const deferredPromise = new Promise<void>((resolve) => {
        resolveReset = resolve;
      });
      authService.resetPassword.and.returnValue(deferredPromise);

      const submitPromise = component.onSubmit({ email: TEST_EMAILS.VALID });
      expect(component.isLoading()).toBeTruthy();

      resolveReset();
      await submitPromise;
      expect(component.isLoading()).toBeFalsy();
    });

    it('should clear previous error on new submission', async () => {
      component.errorMessage.set('Previous error');
      authService.resetPassword.and.returnValue(Promise.resolve());

      await component.onSubmit({ email: TEST_EMAILS.VALID });

      expect(component.errorMessage()).toBeNull();
    });
  });

  describe('Error Handling', () => {
    it('should handle 404 error (email not found) securely', async () => {
      const error = { status: 404, message: 'User not found' };
      authService.resetPassword.and.returnValue(Promise.reject(error));

      await component.onSubmit({ email: TEST_EMAILS.VALID });

      // Should not reveal if email exists for security
      expect(component.emailSent()).toBeTruthy();
      expect(component.errorMessage()).toBeNull();
    });

    it('should show network error message', async () => {
      const error = { message: 'Network error occurred' };
      authService.resetPassword.and.returnValue(Promise.reject(error));

      await component.onSubmit({ email: TEST_EMAILS.VALID });

      expect(component.errorMessage()).toBe('errors.network');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show generic error message on unknown error', async () => {
      const error = { status: 500, message: 'Server error' };
      authService.resetPassword.and.returnValue(Promise.reject(error));

      await component.onSubmit({ email: TEST_EMAILS.VALID });

      expect(component.errorMessage()).toBe('auth.forgotPassword.errors.sendFailed');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should log errors to console', async () => {
      spyOn(console, 'error');
      const error = { status: 500, message: 'Server error' };
      authService.resetPassword.and.returnValue(Promise.reject(error));

      await component.onSubmit({ email: TEST_EMAILS.VALID });

      expect(console.error).toHaveBeenCalledWith(
        '[ForgotPasswordComponent] Password reset error',
        error,
      );
    });
  });

  describe('Component Template', () => {
    it('should render the forgot password form component', () => {
      const formComponent = fixture.nativeElement.querySelector('lib-forgot-password-form');
      expect(formComponent).toBeTruthy();
    });
  });
});
