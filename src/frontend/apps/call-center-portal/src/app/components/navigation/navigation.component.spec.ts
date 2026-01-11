import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { NavigationComponent } from './navigation.component';
import { AuthService } from '../../services/auth.service';
import { CALL_CENTER_ROLES } from '../../constants/navigation.config';

describe('NavigationComponent', () => {
  let component: NavigationComponent;
  let fixture: ComponentFixture<NavigationComponent>;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', [
      'isAuthenticated',
      'getUsername',
      'getUserRoles',
      'login',
      'logout',
    ]);
    authServiceSpy.isAuthenticated.and.returnValue(false);
    authServiceSpy.getUsername.and.returnValue('');
    authServiceSpy.getUserRoles.and.returnValue([]);

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

    it('should have empty roles when not authenticated', () => {
      expect(component['userRoles']()).toEqual([]);
    });

    it('should update authentication state from service', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('agent1');
      authService.getUserRoles.and.returnValue([CALL_CENTER_ROLES.AGENT]);

      component.ngOnInit();

      expect(component['isAuthenticated']()).toBeTruthy();
      expect(component['username']()).toBe('agent1');
      expect(component['userRoles']()).toEqual([CALL_CENTER_ROLES.AGENT]);
    });
  });

  describe('Navigation Items Filtering', () => {
    it('should only show public items when not authenticated', () => {
      const items = component['navItems']();

      // Contact is public (no requiresAuth)
      expect(items.some((item) => item.path === 'contact')).toBeTruthy();

      // Auth-required items should be hidden
      expect(items.some((item) => item.path === '')).toBeFalsy();
      expect(items.some((item) => item.path === 'reservations')).toBeFalsy();
      expect(items.some((item) => item.path === 'customers')).toBeFalsy();
      expect(items.some((item) => item.path === 'locations')).toBeFalsy();
    });

    it('should show auth-required items when authenticated as agent', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('agent1');
      authService.getUserRoles.and.returnValue([CALL_CENTER_ROLES.AGENT]);
      component.ngOnInit();

      const items = component['navItems']();

      expect(items.some((item) => item.path === '')).toBeTruthy();
      expect(items.some((item) => item.path === 'reservations')).toBeTruthy();
      expect(items.some((item) => item.path === 'customers')).toBeTruthy();
    });

    it('should hide supervisor-only items for agents', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('agent1');
      authService.getUserRoles.and.returnValue([CALL_CENTER_ROLES.AGENT]);
      component.ngOnInit();

      const items = component['navItems']();

      // Locations requires supervisor or admin role
      expect(items.some((item) => item.path === 'locations')).toBeFalsy();
    });

    it('should show supervisor items for supervisors', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('supervisor1');
      authService.getUserRoles.and.returnValue([CALL_CENTER_ROLES.SUPERVISOR]);
      component.ngOnInit();

      const items = component['navItems']();

      expect(items.some((item) => item.path === 'locations')).toBeTruthy();
    });

    it('should show all items for admin', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('admin1');
      authService.getUserRoles.and.returnValue([CALL_CENTER_ROLES.ADMIN]);
      component.ngOnInit();

      const items = component['navItems']();

      expect(items.some((item) => item.path === '')).toBeTruthy();
      expect(items.some((item) => item.path === 'reservations')).toBeTruthy();
      expect(items.some((item) => item.path === 'customers')).toBeTruthy();
      expect(items.some((item) => item.path === 'locations')).toBeTruthy();
      expect(items.some((item) => item.path === 'contact')).toBeTruthy();
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

    it('should render brand title', () => {
      const title = fixture.nativeElement.querySelector('.nav__title');
      expect(title).toBeTruthy();
      expect(title.textContent).toContain('Call Center');
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
      authService.getUsername.and.returnValue('agent1');
      authService.getUserRoles.and.returnValue([CALL_CENTER_ROLES.AGENT]);
      component.ngOnInit();
      fixture.detectChanges();

      const username = fixture.nativeElement.querySelector('.nav__username');
      expect(username).toBeTruthy();
      expect(username.textContent).toContain('agent1');
    });

    it('should show logout button when authenticated', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('agent1');
      authService.getUserRoles.and.returnValue([CALL_CENTER_ROLES.AGENT]);
      component.ngOnInit();
      fixture.detectChanges();

      const logoutButton = fixture.nativeElement.querySelector('.nav__auth-button--logout');
      expect(logoutButton).toBeTruthy();
    });

    it('should render navigation links when authenticated', () => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUsername.and.returnValue('agent1');
      authService.getUserRoles.and.returnValue([CALL_CENTER_ROLES.AGENT]);
      component.ngOnInit();
      fixture.detectChanges();

      const links = fixture.nativeElement.querySelectorAll('.nav__link');
      expect(links.length).toBeGreaterThan(0);
    });
  });
});
