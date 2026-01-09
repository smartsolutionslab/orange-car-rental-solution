import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

/** Navigation position options */
export type NavPosition = 'top' | 'left';

/**
 * Shared Layout Component
 *
 * Main layout component with navigation, content, and optional sidebar areas.
 * Provides consistent page structure across all Orange Car Rental portals.
 *
 * @example
 * ```html
 * <ocr-layout>
 *   <nav navigation>
 *     <app-navigation />
 *   </nav>
 *
 *   <main content>
 *     <router-outlet />
 *   </main>
 *
 *   <aside sidebar>
 *     <!-- Optional sidebar content -->
 *   </aside>
 * </ocr-layout>
 * ```
 */
@Component({
  selector: 'ocr-layout',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      class="layout"
      [class.layout--nav-top]="navPosition() === 'top'"
      [class.layout--nav-left]="navPosition() === 'left'"
      [class.layout--with-sidebar]="showSidebar()"
      [class.layout--full-width]="fullWidth()"
    >
      @if (navPosition() === 'top') {
        <!-- Top Navigation Layout -->
        <header class="layout__header">
          <div class="layout__header-content" [style.max-width]="fullWidth() ? '100%' : maxWidth()">
            <ng-content select="[navigation]" />
          </div>
        </header>

        <div class="layout__body" [style.max-width]="fullWidth() ? '100%' : maxWidth()">
          <main class="layout__main">
            <ng-content select="[content]" />
          </main>

          @if (showSidebar()) {
            <aside class="layout__sidebar">
              <ng-content select="[sidebar]" />
            </aside>
          }
        </div>
      } @else {
        <!-- Left Navigation Layout -->
        <aside class="layout__nav-sidebar">
          <ng-content select="[navigation]" />
        </aside>

        <div class="layout__body" [style.max-width]="fullWidth() ? '100%' : maxWidth()">
          <main class="layout__main">
            <ng-content select="[content]" />
          </main>

          @if (showSidebar()) {
            <aside class="layout__sidebar">
              <ng-content select="[sidebar]" />
            </aside>
          }
        </div>
      }
    </div>
  `,
  styles: [
    `
      /* Layout Container */
      .layout {
        display: flex;
        flex-direction: column;
        min-height: 100vh;
        background-color: #f5f5f5;
      }

      /* Top Navigation Layout */
      .layout--nav-top {
        flex-direction: column;
      }

      .layout__header {
        background-color: #fff;
        border-bottom: 1px solid #e0e0e0;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
        position: sticky;
        top: 0;
        z-index: 100;
      }

      .layout__header-content {
        margin: 0 auto;
        padding: 0 1rem;
      }

      /* Left Navigation Layout */
      .layout--nav-left {
        flex-direction: row;
      }

      .layout__nav-sidebar {
        width: 250px;
        min-height: 100vh;
        background-color: #fff;
        border-right: 1px solid #e0e0e0;
        box-shadow: 2px 0 4px rgba(0, 0, 0, 0.05);
        position: sticky;
        top: 0;
        overflow-y: auto;
        flex-shrink: 0;
      }

      /* Body Container */
      .layout__body {
        display: flex;
        flex: 1;
        gap: 1.5rem;
        margin: 0 auto;
        padding: 1.5rem 1rem;
        width: 100%;
      }

      .layout--full-width .layout__body {
        max-width: none;
      }

      /* Main Content Area */
      .layout__main {
        flex: 1;
        min-width: 0; /* Prevents flex item from overflowing */
        background-color: #fff;
        border-radius: 8px;
        box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
        padding: 1.5rem;
      }

      /* Right Sidebar */
      .layout__sidebar {
        width: 300px;
        background-color: #fff;
        border-radius: 8px;
        box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
        padding: 1.5rem;
        flex-shrink: 0;
        position: sticky;
        top: 1.5rem;
        align-self: flex-start;
        max-height: calc(100vh - 3rem);
        overflow-y: auto;
      }

      /* When in left nav mode, adjust sticky position */
      .layout--nav-left .layout__sidebar {
        top: 1.5rem;
      }

      .layout--nav-top .layout__sidebar {
        top: calc(60px + 1.5rem); /* Header height + gap */
      }

      /* Without sidebar, main content takes full width */
      .layout__body:not(.layout--with-sidebar) .layout__main {
        max-width: 100%;
      }

      /* Responsive Design */
      @media (max-width: 1024px) {
        /* Stack sidebar below main content on tablets */
        .layout__body {
          flex-direction: column;
        }

        .layout__sidebar {
          width: 100%;
          position: static;
          max-height: none;
        }

        /* On small screens, switch left nav to top */
        .layout--nav-left {
          flex-direction: column;
        }

        .layout__nav-sidebar {
          width: 100%;
          min-height: auto;
          position: static;
          border-right: none;
          border-bottom: 1px solid #e0e0e0;
        }
      }

      @media (max-width: 768px) {
        .layout__body {
          padding: 1rem 0.5rem;
          gap: 1rem;
        }

        .layout__main,
        .layout__sidebar {
          padding: 1rem;
          border-radius: 0;
        }

        .layout__header-content {
          padding: 0 0.5rem;
        }
      }

      /* Scrollbar styling for sidebar */
      .layout__sidebar::-webkit-scrollbar,
      .layout__nav-sidebar::-webkit-scrollbar {
        width: 6px;
      }

      .layout__sidebar::-webkit-scrollbar-track,
      .layout__nav-sidebar::-webkit-scrollbar-track {
        background: #f1f1f1;
        border-radius: 3px;
      }

      .layout__sidebar::-webkit-scrollbar-thumb,
      .layout__nav-sidebar::-webkit-scrollbar-thumb {
        background: #ccc;
        border-radius: 3px;
      }

      .layout__sidebar::-webkit-scrollbar-thumb:hover,
      .layout__nav-sidebar::-webkit-scrollbar-thumb:hover {
        background: #999;
      }
    `,
  ],
})
export class LayoutComponent {
  /**
   * Whether to show the sidebar area.
   * @default false
   */
  readonly showSidebar = input<boolean>(false);

  /**
   * Navigation position: 'top' (horizontal) or 'left' (vertical sidebar).
   * @default 'top'
   */
  readonly navPosition = input<NavPosition>('top');

  /**
   * Whether the layout should be full-width or contained.
   * @default false
   */
  readonly fullWidth = input<boolean>(false);

  /**
   * Maximum width for contained layout (CSS value).
   * @default '1400px'
   */
  readonly maxWidth = input<string>('1400px');
}
