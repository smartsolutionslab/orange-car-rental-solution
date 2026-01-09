import { Component, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { LanguageSwitcherComponent } from '@orange-car-rental/i18n';
import type { NavItem } from '@orange-car-rental/shared';
import { ICON_CAR, ICON_LOGIN, ICON_LOGOUT } from '@orange-car-rental/util';

/**
 * Shared Navigation Component
 *
 * Provides a consistent navigation bar across all portals.
 * Supports dynamic navigation items, auth state, and role-based filtering.
 *
 * @example
 * <ocr-navigation
 *   [title]="'My App'"
 *   [navItems]="filteredNavItems()"
 *   [isAuthenticated]="isAuthenticated()"
 *   [username]="username()"
 *   (loginClick)="login()"
 *   (logoutClick)="logout()"
 * />
 */
@Component({
  selector: 'ocr-navigation',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule, TranslateModule, LanguageSwitcherComponent],
  template: `
    <nav class="nav">
      <div class="nav__brand">
        <svg class="nav__logo" viewBox="0 0 24 24" fill="currentColor" [innerHTML]="logoIcon()"></svg>
        <span class="nav__title">{{ title() }}</span>
      </div>

      <ul class="nav__menu">
        @for (item of navItems(); track item.path) {
          <li class="nav__item">
            <a
              [routerLink]="item.path"
              routerLinkActive="nav__link--active"
              [routerLinkActiveOptions]="{ exact: item.exactMatch ?? false }"
              class="nav__link"
            >
              <svg
                class="nav__link-icon"
                viewBox="0 0 24 24"
                fill="currentColor"
                [innerHTML]="item.icon"
              ></svg>
              <span class="nav__link-text">{{ item.label }}</span>
            </a>
          </li>
        }
      </ul>

      <div class="nav__auth">
        <lib-language-switcher />
        @if (isAuthenticated()) {
          <span class="nav__username">{{ username() }}</span>
          <button (click)="logoutClick.emit()" class="nav__auth-button nav__auth-button--logout">
            <svg
              class="nav__auth-icon"
              viewBox="0 0 24 24"
              fill="currentColor"
              [innerHTML]="icons.logout"
            ></svg>
            <span>{{ 'common.actions.logout' | translate }}</span>
          </button>
        } @else {
          <button (click)="loginClick.emit()" class="nav__auth-button nav__auth-button--login">
            <svg
              class="nav__auth-icon"
              viewBox="0 0 24 24"
              fill="currentColor"
              [innerHTML]="icons.login"
            ></svg>
            <span>{{ 'common.actions.login' | translate }}</span>
          </button>
        }
      </div>
    </nav>
  `,
  styles: [
    `
      .nav {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 1rem 0;
      }

      .nav__brand {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        font-weight: 600;
        font-size: 1.25rem;
        color: #ff6b35;
      }

      .nav__logo {
        width: 32px;
        height: 32px;
      }

      .nav__title {
        color: #333;
      }

      .nav__menu {
        display: flex;
        gap: 0.5rem;
        list-style: none;
        margin: 0;
        padding: 0;
      }

      .nav__item {
        margin: 0;
      }

      .nav__link {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        padding: 0.625rem 1rem;
        border-radius: 6px;
        text-decoration: none;
        color: #666;
        font-weight: 500;
        transition: all 0.2s ease;
      }

      .nav__link:hover {
        background-color: #f5f5f5;
        color: #ff6b35;
      }

      .nav__link--active {
        background-color: #fff3ed;
        color: #ff6b35;
      }

      .nav__link-icon {
        width: 20px;
        height: 20px;
      }

      .nav__link-text {
        font-size: 0.9375rem;
      }

      .nav__auth {
        display: flex;
        align-items: center;
        gap: 1rem;
      }

      .nav__username {
        font-weight: 500;
        color: #333;
        font-size: 0.875rem;
      }

      .nav__auth-button {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        padding: 0.5rem 1rem;
        border: none;
        border-radius: 6px;
        font-weight: 500;
        font-size: 0.875rem;
        cursor: pointer;
        transition: all 0.2s ease;
      }

      .nav__auth-button--login {
        background-color: #ff6b35;
        color: white;
      }

      .nav__auth-button--login:hover {
        background-color: #e55a2b;
      }

      .nav__auth-button--logout {
        background-color: #f3f4f6;
        color: #666;
      }

      .nav__auth-button--logout:hover {
        background-color: #e5e7eb;
        color: #333;
      }

      .nav__auth-icon {
        width: 18px;
        height: 18px;
      }

      /* Responsive */
      @media (max-width: 768px) {
        .nav {
          flex-direction: column;
          gap: 1rem;
          padding: 0.75rem 0;
        }

        .nav__menu {
          flex-wrap: wrap;
          justify-content: center;
        }

        .nav__link {
          padding: 0.5rem 0.75rem;
          font-size: 0.875rem;
        }

        .nav__auth {
          flex-wrap: wrap;
          justify-content: center;
        }
      }
    `,
  ],
})
export class NavigationComponent {
  /** Application title displayed in the nav brand */
  readonly title = input.required<string>();

  /** Navigation items to display */
  readonly navItems = input.required<readonly NavItem[]>();

  /** Whether user is authenticated */
  readonly isAuthenticated = input(false);

  /** Username to display when authenticated */
  readonly username = input('');

  /** Custom logo icon SVG path (defaults to car icon) */
  readonly logoIcon = input(ICON_CAR);

  /** Emitted when login button is clicked */
  readonly loginClick = output<void>();

  /** Emitted when logout button is clicked */
  readonly logoutClick = output<void>();

  /** Icons for template */
  protected readonly icons = {
    login: ICON_LOGIN,
    logout: ICON_LOGOUT,
  };
}
