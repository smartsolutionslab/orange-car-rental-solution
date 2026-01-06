import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { RegisterComponent } from './register.component';
import { AuthService } from '../../services/auth.service';
import {
  TEST_EMAILS,
  TEST_PASSWORDS,
  TEST_PERSONAL_INFO,
} from '@orange-car-rental/shared/testing';
import { UI_TIMING } from '../../constants/app.constants';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['register', 'loginWithPassword']);

    await TestBed.configureTestingModule({
      imports: [RegisterComponent, TranslateModule.forRoot()],
      providers: [{ provide: AuthService, useValue: authServiceSpy }, provideRouter([])],
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router);
    spyOn(router, 'navigate').and.returnValue(Promise.resolve(true));
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Initial State', () => {
    it('should start with loading as false', () => {
      expect(component.isLoading()).toBeFalsy();
    });

    it('should start with no error message', () => {
      expect(component.errorMessage()).toBeNull();
    });

    it('should start with no success message', () => {
      expect(component.successMessage()).toBeNull();
    });
  });

  describe('Labels Configuration', () => {
    it('should provide labels to the form', () => {
      const labels = component.labels();
      expect(labels.title).toBe('auth.register.title');
      expect(labels.subtitle).toBe('auth.register.subtitle');
      expect(labels.step1Title).toBe('auth.register.steps.account');
      expect(labels.step2Title).toBe('auth.register.steps.personal');
      expect(labels.step3Title).toBe('auth.register.steps.confirmation');
    });

    it('should provide validation labels', () => {
      const labels = component.labels();
      expect(labels.emailRequired).toBe('auth.validation.emailRequired');
      expect(labels.emailInvalid).toBe('auth.validation.emailInvalid');
      expect(labels.passwordRequired).toBe('auth.validation.passwordRequired');
      expect(labels.passwordMismatch).toBe('auth.register.validation.passwordMismatch');
    });

    it('should provide form field labels', () => {
      const labels = component.labels();
      expect(labels.emailLabel).toBe('common.labels.email');
      expect(labels.passwordLabel).toBe('common.labels.password');
      expect(labels.firstNameLabel).toBe('common.labels.firstName');
      expect(labels.lastNameLabel).toBe('common.labels.lastName');
      expect(labels.phoneLabel).toBe('common.labels.phone');
      expect(labels.dateOfBirthLabel).toBe('common.labels.dateOfBirth');
    });
  });

  describe('Auth Config', () => {
    it('should configure terms URL', () => {
      expect(component.authConfig.termsUrl).toBe('/terms');
    });

    it('should configure privacy URL', () => {
      expect(component.authConfig.privacyUrl).toBe('/privacy');
    });

    it('should configure minimum age', () => {
      expect(component.authConfig.minAge).toBeDefined();
    });

    it('should configure login route', () => {
      expect(component.authConfig.loginRoute).toBe('/login');
    });
  });

  describe('Form Submission', () => {
    const validFormData = {
      email: TEST_EMAILS.VALID,
      password: TEST_PASSWORDS.STRONG,
      firstName: TEST_PERSONAL_INFO.FIRST_NAME,
      lastName: TEST_PERSONAL_INFO.LAST_NAME,
      phoneNumber: TEST_PERSONAL_INFO.PHONE,
      dateOfBirth: TEST_PERSONAL_INFO.DATE_OF_BIRTH_VALID,
      acceptTerms: true,
      acceptPrivacy: true,
      acceptMarketing: false,
    };

    it('should call authService.register with correct data', async () => {
      authService.register.and.returnValue(Promise.resolve());
      authService.loginWithPassword.and.returnValue(Promise.resolve());

      await component.onRegister(validFormData);

      expect(authService.register).toHaveBeenCalledWith({
        email: TEST_EMAILS.VALID,
        password: TEST_PASSWORDS.STRONG,
        firstName: TEST_PERSONAL_INFO.FIRST_NAME,
        lastName: TEST_PERSONAL_INFO.LAST_NAME,
        phoneNumber: TEST_PERSONAL_INFO.PHONE,
        dateOfBirth: TEST_PERSONAL_INFO.DATE_OF_BIRTH_VALID,
        acceptMarketing: false,
      });
    });

    it('should show success message on successful registration', async () => {
      authService.register.and.returnValue(Promise.resolve());
      authService.loginWithPassword.and.returnValue(Promise.resolve());

      await component.onRegister(validFormData);

      expect(component.successMessage()).toBe('auth.register.success');
    });

    it('should set loading state during submission', async () => {
      let resolveRegister!: () => void;
      const deferredPromise = new Promise<void>((resolve) => {
        resolveRegister = resolve;
      });
      authService.register.and.returnValue(deferredPromise);

      const submitPromise = component.onRegister(validFormData);
      expect(component.isLoading()).toBeTruthy();

      resolveRegister();
      await submitPromise;
      expect(component.isLoading()).toBeFalsy();
    });

    it('should clear error message on new submission', async () => {
      component.errorMessage.set('Previous error');
      authService.register.and.returnValue(Promise.resolve());
      authService.loginWithPassword.and.returnValue(Promise.resolve());

      await component.onRegister(validFormData);

      expect(component.errorMessage()).toBeNull();
    });

    it('should auto-login after successful registration', fakeAsync(() => {
      authService.register.and.returnValue(Promise.resolve());
      authService.loginWithPassword.and.returnValue(Promise.resolve());

      component.onRegister(validFormData);
      tick(); // Wait for registration

      tick(UI_TIMING.REDIRECT_DELAY); // Wait for auto-login timeout

      expect(authService.loginWithPassword).toHaveBeenCalledWith(
        TEST_EMAILS.VALID,
        TEST_PASSWORDS.STRONG,
      );
      expect(router.navigate).toHaveBeenCalledWith(['/']);
    }));
  });

  describe('Error Handling', () => {
    const validFormData = {
      email: TEST_EMAILS.VALID,
      password: TEST_PASSWORDS.STRONG,
      firstName: TEST_PERSONAL_INFO.FIRST_NAME,
      lastName: TEST_PERSONAL_INFO.LAST_NAME,
      phoneNumber: TEST_PERSONAL_INFO.PHONE,
      dateOfBirth: TEST_PERSONAL_INFO.DATE_OF_BIRTH_VALID,
      acceptTerms: true,
      acceptPrivacy: true,
      acceptMarketing: false,
    };

    it('should show error message on email already exists (409)', async () => {
      const error = { status: 409, message: 'Email already exists' };
      authService.register.and.returnValue(Promise.reject(error));

      await component.onRegister(validFormData);

      expect(component.errorMessage()).toBe('auth.register.errors.emailExists');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show error message on password error', async () => {
      const error = { status: 400, message: 'password requirements not met' };
      authService.register.and.returnValue(Promise.reject(error));

      await component.onRegister(validFormData);

      expect(component.errorMessage()).toBe('auth.register.errors.passwordWeak');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show error message on email error', async () => {
      const error = { status: 400, message: 'email is invalid' };
      authService.register.and.returnValue(Promise.reject(error));

      await component.onRegister(validFormData);

      expect(component.errorMessage()).toBe('auth.register.errors.invalidEmail');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show generic error message on unknown error', async () => {
      const error = { status: 500, message: 'Server error' };
      authService.register.and.returnValue(Promise.reject(error));

      await component.onRegister(validFormData);

      expect(component.errorMessage()).toBe('auth.register.errors.generic');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should log errors to console', async () => {
      spyOn(console, 'error');
      const error = { status: 500, message: 'Server error' };
      authService.register.and.returnValue(Promise.reject(error));

      await component.onRegister(validFormData);

      expect(console.error).toHaveBeenCalledWith('[RegisterComponent] Registration error', error);
    });
  });

  describe('Component Template', () => {
    it('should render the register form component', () => {
      const formComponent = fixture.nativeElement.querySelector('lib-register-form');
      expect(formComponent).toBeTruthy();
    });
  });
});
