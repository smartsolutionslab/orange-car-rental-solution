import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { NavigationComponent } from './navigation.component';
import { AuthService } from '../../services/auth.service';

describe('NavigationComponent', () => {
  let component: NavigationComponent;
  let fixture: ComponentFixture<NavigationComponent>;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', [
      'isAuthenticated',
      'getUsername',
      'login',
      'logout',
    ]);
    authServiceSpy.isAuthenticated.and.returnValue(false);
    authServiceSpy.getUsername.and.returnValue('');

    await TestBed.configureTestingModule({
      imports: [NavigationComponent, TranslateModule.forRoot()],
      providers: [{ provide: AuthService, useValue: authServiceSpy }, provideRouter([])],
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NavigationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Authentication State', () => {
    it('should start as not authenticated', () => {
      expect(component['isAuthenticated']()).toBeFalsy();
    });

    it('should have empty username when not authenticated', () => {
      expect(component['username']()).toBe('');
    });

    it('should update authentication state from service', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('testuser');

      component.ngOnInit();

      expect(component['isAuthenticated']()).toBeTruthy();
      expect(component['username']()).toBe('testuser');
    });
  });

  describe('Navigation Items Filtering', () => {
    it('should show public items when not authenticated', () => {
      const items = component['navItems']();

      // Should include public items
      expect(items.some((item) => item.path === '/')).toBeTruthy();
      expect(items.some((item) => item.path === '/locations')).toBeTruthy();
      expect(items.some((item) => item.path === '/contact')).toBeTruthy();
    });

    it('should hide auth-required items when not authenticated', () => {
      const items = component['navItems']();

      // Should not include auth-required items
      expect(items.some((item) => item.path === '/my-bookings')).toBeFalsy();
    });

    it('should show auth-required items when authenticated', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('testuser');
      component.ngOnInit();

      const items = component['navItems']();

      // Should include auth-required items
      expect(items.some((item) => item.path === '/my-bookings')).toBeTruthy();
    });

    it('should show all public items when authenticated', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('testuser');
      component.ngOnInit();

      const items = component['navItems']();

      expect(items.some((item) => item.path === '/')).toBeTruthy();
      expect(items.some((item) => item.path === '/locations')).toBeTruthy();
      expect(items.some((item) => item.path === '/contact')).toBeTruthy();
    });
  });

  describe('Login and Logout', () => {
    it('should call authService.login on login', () => {
      component['login']();

      expect(authService.login).toHaveBeenCalled();
    });

    it('should call authService.logout on logout', () => {
      component['logout']();

      expect(authService.logout).toHaveBeenCalled();
    });
  });

  describe('UI Elements', () => {
    it('should render navigation element', () => {
      const nav = fixture.nativeElement.querySelector('nav');
      expect(nav).toBeTruthy();
    });

    it('should render navigation links', () => {
      fixture.detectChanges();
      const links = fixture.nativeElement.querySelectorAll('.nav__link');
      expect(links.length).toBeGreaterThan(0);
    });

    it('should render language switcher', () => {
      const languageSwitcher = fixture.nativeElement.querySelector('lib-language-switcher');
      expect(languageSwitcher).toBeTruthy();
    });

    it('should show login button when not authenticated', () => {
      fixture.detectChanges();
      const loginButton = fixture.nativeElement.querySelector('.nav__auth-button--login');
      expect(loginButton).toBeTruthy();
    });

    it('should show username when authenticated', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('testuser');
      component.ngOnInit();
      fixture.detectChanges();

      const username = fixture.nativeElement.querySelector('.nav__username');
      expect(username).toBeTruthy();
      expect(username.textContent).toContain('testuser');
    });

    it('should show logout button when authenticated', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('testuser');
      component.ngOnInit();
      fixture.detectChanges();

      const logoutButton = fixture.nativeElement.querySelector('.nav__auth-button--logout');
      expect(logoutButton).toBeTruthy();
    });
  });

});
