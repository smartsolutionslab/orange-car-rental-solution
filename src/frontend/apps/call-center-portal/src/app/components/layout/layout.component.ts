import { Component, input } from '@angular/core';
import { LayoutComponent as SharedLayoutComponent, type NavPosition } from '@orange-car-rental/ui-components';

/**
 * Layout wrapper component for call center portal.
 * Re-exports the shared LayoutComponent with the app-layout selector for backward compatibility.
 *
 * @see SharedLayoutComponent from @orange-car-rental/ui-components
 */
@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [SharedLayoutComponent],
  template: `
    <ocr-layout
      [showSidebar]="showSidebar()"
      [navPosition]="navPosition()"
      [fullWidth]="fullWidth()"
      [maxWidth]="maxWidth()"
    >
      <ng-content select="[navigation]" navigation />
      <ng-content select="[content]" content />
      <ng-content select="[sidebar]" sidebar />
    </ocr-layout>
  `,
})
export class LayoutComponent {
  /** Whether to show the sidebar area. */
  readonly showSidebar = input<boolean>(false);

  /** Navigation position: 'top' (horizontal) or 'left' (vertical sidebar). */
  readonly navPosition = input<NavPosition>('top');

  /** Whether the layout should be full-width or contained. */
  readonly fullWidth = input<boolean>(false);

  /** Maximum width for contained layout (CSS value). */
  readonly maxWidth = input<string>('1400px');
}
