import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastContainerComponent } from '@orange-car-rental/shared';

/**
 * Root application component
 * Simple router outlet - shell provides main navigation and layout
 */
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToastContainerComponent],
  template: `
    <router-outlet />
    <lib-toast-container />
  `,
  styles: [
    `
      :host {
        display: block;
      }
    `,
  ],
})
export class App {}
