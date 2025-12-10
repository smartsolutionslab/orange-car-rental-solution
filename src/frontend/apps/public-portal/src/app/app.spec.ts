import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';
import { App } from './app';
import { AuthService } from './services/auth.service';

describe('App', () => {
  beforeEach(async () => {
    const keycloakServiceSpy = jasmine.createSpyObj('KeycloakService', [
      'isLoggedIn',
      'login',
      'logout',
      'getUsername',
      'loadUserProfile',
      'getToken',
      'getUserRoles',
      'isUserInRole'
    ]);
    keycloakServiceSpy.isLoggedIn.and.returnValue(false);

    const authServiceSpy = jasmine.createSpyObj('AuthService', [
      'isAuthenticated',
      'getUsername',
      'login',
      'logout'
    ]);
    authServiceSpy.isAuthenticated.and.returnValue(false);
    authServiceSpy.getUsername.and.returnValue('');

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideRouter([]),
        { provide: KeycloakService, useValue: keycloakServiceSpy },
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render router outlet', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('router-outlet')).toBeTruthy();
  });
});
