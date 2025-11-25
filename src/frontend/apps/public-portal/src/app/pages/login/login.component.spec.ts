import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginComponent } from './login.component';
import { AuthService } from '../../services/auth.service';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['login']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [LoginComponent, ReactiveFormsModule],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Form Validation', () => {
    it('should initialize with empty form', () => {
      expect(component.loginForm.get('email')?.value).toBe('');
      expect(component.loginForm.get('password')?.value).toBe('');
      expect(component.loginForm.get('rememberMe')?.value).toBe(false);
    });

    it('should require email', () => {
      const emailControl = component.loginForm.get('email');
      emailControl?.setValue('');
      expect(emailControl?.hasError('required')).toBeTruthy();
    });

    it('should validate email format', () => {
      const emailControl = component.loginForm.get('email');
      emailControl?.setValue('invalid-email');
      emailControl?.markAsTouched();
      expect(emailControl?.hasError('email')).toBeTruthy();
      expect(component.emailError).toBe('Bitte geben Sie eine gültige E-Mail-Adresse ein');
    });

    it('should accept valid email', () => {
      const emailControl = component.loginForm.get('email');
      emailControl?.setValue('test@example.com');
      expect(emailControl?.hasError('email')).toBeFalsy();
    });

    it('should require password', () => {
      const passwordControl = component.loginForm.get('password');
      passwordControl?.setValue('');
      expect(passwordControl?.hasError('required')).toBeTruthy();
    });

    it('should show email required error when touched', () => {
      const emailControl = component.loginForm.get('email');
      emailControl?.setValue('');
      emailControl?.markAsTouched();
      expect(component.emailError).toBe('E-Mail-Adresse ist erforderlich');
    });

    it('should show password required error when touched', () => {
      const passwordControl = component.loginForm.get('password');
      passwordControl?.setValue('');
      passwordControl?.markAsTouched();
      expect(component.passwordError).toBe('Passwort ist erforderlich');
    });

    it('should mark form as invalid when fields are empty', () => {
      expect(component.loginForm.valid).toBeFalsy();
    });

    it('should mark form as valid when all fields are filled correctly', () => {
      component.loginForm.patchValue({
        email: 'test@example.com',
        password: 'password123'
      });
      expect(component.loginForm.valid).toBeTruthy();
    });
  });

  describe('Password Visibility Toggle', () => {
    it('should start with password hidden', () => {
      expect(component.showPassword()).toBeFalsy();
    });

    it('should toggle password visibility', () => {
      expect(component.showPassword()).toBeFalsy();
      component.togglePasswordVisibility();
      expect(component.showPassword()).toBeTruthy();
      component.togglePasswordVisibility();
      expect(component.showPassword()).toBeFalsy();
    });
  });

  describe('Form Submission', () => {
    beforeEach(() => {
      component.loginForm.patchValue({
        email: 'test@example.com',
        password: 'password123',
        rememberMe: true
      });
    });

    it('should not submit if form is invalid', async () => {
      component.loginForm.patchValue({ email: '', password: '' });
      await component.onSubmit();
      expect(authService.login).not.toHaveBeenCalled();
    });

    it('should call authService.login with correct parameters on valid form', async () => {
      authService.login.and.returnValue(Promise.resolve());
      router.navigate.and.returnValue(Promise.resolve(true));

      await component.onSubmit();

      expect(authService.login).toHaveBeenCalledWith('test@example.com', 'password123', true);
    });

    it('should navigate to home on successful login', async () => {
      authService.login.and.returnValue(Promise.resolve());
      router.navigate.and.returnValue(Promise.resolve(true));

      await component.onSubmit();

      expect(router.navigate).toHaveBeenCalledWith(['/']);
    });

    it('should set loading state during submission', async () => {
      let resolveLogin: () => void;
      authService.login.and.returnValue(new Promise((resolve) => {
        resolveLogin = resolve;
      }));

      const submitPromise = component.onSubmit();
      expect(component.isLoading()).toBeTruthy();

      resolveLogin();
      await submitPromise;
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show error message on 401 error', async () => {
      const error = { status: 401, message: 'Unauthorized' };
      authService.login.and.returnValue(Promise.reject(error));

      await component.onSubmit();

      expect(component.errorMessage()).toBe('Ungültige E-Mail-Adresse oder Passwort');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show error message on 403 error', async () => {
      const error = { status: 403, message: 'Forbidden' };
      authService.login.and.returnValue(Promise.reject(error));

      await component.onSubmit();

      expect(component.errorMessage()).toContain('Ihr Konto wurde gesperrt');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show network error message', async () => {
      const error = { message: 'Network error' };
      authService.login.and.returnValue(Promise.reject(error));

      await component.onSubmit();

      expect(component.errorMessage()).toBe('Netzwerkfehler. Bitte überprüfen Sie Ihre Internetverbindung.');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should show generic error message on unknown error', async () => {
      const error = { status: 500, message: 'Server error' };
      authService.login.and.returnValue(Promise.reject(error));

      await component.onSubmit();

      expect(component.errorMessage()).toBe('Ein unerwarteter Fehler ist aufgetreten. Bitte versuchen Sie es erneut.');
      expect(component.isLoading()).toBeFalsy();
    });

    it('should clear error message on new submission', async () => {
      component.errorMessage.set('Previous error');
      authService.login.and.returnValue(Promise.resolve());
      router.navigate.and.returnValue(Promise.resolve(true));

      await component.onSubmit();

      expect(component.errorMessage()).toBeNull();
    });
  });

  describe('Social Login', () => {
    it('should have loginWithGoogle method', () => {
      expect(component.loginWithGoogle).toBeDefined();
    });

    it('should log "Google login not implemented" when called', () => {
      spyOn(console, 'log');
      component.loginWithGoogle();
      expect(console.log).toHaveBeenCalledWith('Google login not implemented yet');
    });
  });

  describe('UI State', () => {
    it('should disable submit button when loading', () => {
      component.isLoading.set(true);
      fixture.detectChanges();
      const submitButton = fixture.nativeElement.querySelector('button[type="submit"]');
      expect(submitButton.disabled).toBeTruthy();
    });

    it('should show spinner when loading', () => {
      component.isLoading.set(true);
      fixture.detectChanges();
      const spinner = fixture.nativeElement.querySelector('.spinner');
      expect(spinner).toBeTruthy();
    });

    it('should display error message in alert', () => {
      component.errorMessage.set('Test error message');
      fixture.detectChanges();
      const errorAlert = fixture.nativeElement.querySelector('.error-alert');
      expect(errorAlert).toBeTruthy();
      expect(errorAlert.textContent).toContain('Test error message');
    });
  });
});
