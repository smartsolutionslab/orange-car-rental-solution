import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { LoginComponent } from './login.component';
import { AuthService } from '../../services/auth.service';
import { TEST_EMAILS, TEST_PASSWORDS } from '@orange-car-rental/shared/testing';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: Router;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', [
      'loginWithPassword',
      'getPostLoginRedirect',
    ]);
    authServiceSpy.getPostLoginRedirect.and.returnValue('/my-bookings');

    await TestBed.configureTestingModule({
      imports: [LoginComponent, TranslateModule.forRoot()],
      providers: [{ provide: AuthService, useValue: authServiceSpy }, provideRouter([])],
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router);
    spyOn(router, 'navigate').and.returnValue(Promise.resolve(true));
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
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
  });

  describe('Labels Configuration', () => {
    it('should provide labels to the form', () => {
      const labels = component.labels();
      expect(labels.title).toBe('auth.login.title');
      expect(labels.subtitle).toBe('auth.login.subtitle');
      expect(labels.emailLabel).toBe('auth.login.email');
      expect(labels.passwordLabel).toBe('auth.login.password');
      expect(labels.submitButton).toBe('auth.login.submit');
    });

    it('should provide validation labels', () => {
      const labels = component.labels();
      expect(labels.emailRequired).toBe('auth.validation.emailRequired');
      expect(labels.emailInvalid).toBe('auth.validation.emailInvalid');
      expect(labels.passwordRequired).toBe('auth.validation.passwordRequired');
    });
  });

  describe('Auth Config', () => {
    it('should enable remember me', () => {
      expect(component.authConfig.showRememberMe).toBeTruthy();
    });

    it('should configure login route', () => {
      expect(component.authConfig.loginRoute).toBe('/login');
    });

    it('should configure register route', () => {
      expect(component.authConfig.registerRoute).toBe('/register');
    });
  });

  describe('Form Submission', () => {
    it('should call authService.loginWithPassword with correct parameters', async () => {
      authService.loginWithPassword.and.returnValue(Promise.resolve());

      await component.onLogin({
        email: TEST_EMAILS.VALID,
        password: TEST_PASSWORDS.VALID,
        rememberMe: true,
      });

      expect(authService.loginWithPassword).toHaveBeenCalledWith(
        TEST_EMAILS.VALID,
        TEST_PASSWORDS.VALID,
        true,
      );
    });

    it('should navigate to redirect URL from auth service on successful login', async () => {
      authService.loginWithPassword.and.returnValue(Promise.resolve());

      await component.onLogin({
        email: TEST_EMAILS.VALID,
        password: TEST_PASSWORDS.VALID,
        rememberMe: false,
      });

      expect(authService.getPostLoginRedirect).toHaveBeenCalled();
      expect(router.navigate).toHaveBeenCalledWith(['/my-bookings']);
    });

    it('should set loading state during submission', async () => {
      let resolveLogin!: () => void;
      const deferredPromise = new Promise<void>((resolve) => {
        resolveLogin = resolve;
      });
      authService.loginWithPassword.and.returnValue(deferredPromise);

      const submitPromise = component.onLogin({
        email: TEST_EMAILS.VALID,
        password: TEST_PASSWORDS.VALID,
        rememberMe: false,
      });
      expect(component.isLoading()).toBeTruthy();

      resolveLogin();
      await submitPromise;
      expect(component.isLoading()).toBeFalsy();
    });

    it('should clear error message on new submission', async () => {
      component.errorMessage.set('Previous error');
      authService.loginWithPassword.and.returnValue(Promise.resolve());

      await component.onLogin({
        email: TEST_EMAILS.VALID,
        password: TEST_PASSWORDS.VALID,
        rememberMe: false,
      });

      expect(component.errorMessage()).toBeNull();
    });
  });

  describe('Error Handling', () => {
    it('should show error message on 401 error', async () => {
      const error = { status: 401, message: 'Unauthorized' };
      authService.loginWithPassword.and.returnValue(Promise.reject(error));

      await component.onLogin({
        email: TEST_EMAILS.VALID,
        password: TEST_PASSWORDS.VALID,
        rememberMe: false,
      });

      expect(component.errorMessage()).toBe('auth.errors.invalidCredentials');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show error message on 403 error', async () => {
      const error = { status: 403, message: 'Forbidden' };
      authService.loginWithPassword.and.returnValue(Promise.reject(error));

      await component.onLogin({
        email: TEST_EMAILS.VALID,
        password: TEST_PASSWORDS.VALID,
        rememberMe: false,
      });

      expect(component.errorMessage()).toBe('auth.errors.accountLocked');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show network error message', async () => {
      const error = { message: 'Network error' };
      authService.loginWithPassword.and.returnValue(Promise.reject(error));

      await component.onLogin({
        email: TEST_EMAILS.VALID,
        password: TEST_PASSWORDS.VALID,
        rememberMe: false,
      });

      expect(component.errorMessage()).toBe('auth.errors.networkError');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show generic error message on unknown error', async () => {
      const error = { status: 500, message: 'Server error' };
      authService.loginWithPassword.and.returnValue(Promise.reject(error));

      await component.onLogin({
        email: TEST_EMAILS.VALID,
        password: TEST_PASSWORDS.VALID,
        rememberMe: false,
      });

      expect(component.errorMessage()).toBe('auth.errors.loginFailed');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should log errors to console', async () => {
      spyOn(console, 'error');
      const error = { status: 500, message: 'Server error' };
      authService.loginWithPassword.and.returnValue(Promise.reject(error));

      await component.onLogin({
        email: TEST_EMAILS.VALID,
        password: TEST_PASSWORDS.VALID,
        rememberMe: false,
      });

      expect(console.error).toHaveBeenCalledWith('[LoginComponent] Login error', error);
    });
  });

  describe('Component Template', () => {
    it('should render the login form component', () => {
      const formComponent = fixture.nativeElement.querySelector('lib-login-form');
      expect(formComponent).toBeTruthy();
    });
  });
});
