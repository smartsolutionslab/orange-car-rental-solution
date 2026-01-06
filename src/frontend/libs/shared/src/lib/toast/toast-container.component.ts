import { Component, inject } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ToastService } from "./toast.service";

/**
 * Toast Container Component
 *
 * Displays toast notifications from the ToastService.
 * Add this component once in your root app component.
 *
 * @example
 * // In app.component.html
 * <router-outlet></router-outlet>
 * <lib-toast-container></lib-toast-container>
 */
@Component({
  selector: "lib-toast-container",
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="toast-container" aria-live="polite" aria-atomic="true">
      @for (toast of toastService.toasts(); track toast.id) {
        <div
          class="toast"
          [class.toast-success]="toast.type === 'success'"
          [class.toast-error]="toast.type === 'error'"
          [class.toast-info]="toast.type === 'info'"
          [class.toast-warning]="toast.type === 'warning'"
          role="alert"
        >
          <div class="toast-icon">
            @switch (toast.type) {
              @case ("success") {
                <svg viewBox="0 0 24 24" fill="currentColor">
                  <path d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z" />
                </svg>
              }
              @case ("error") {
                <svg viewBox="0 0 24 24" fill="currentColor">
                  <path
                    d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"
                  />
                </svg>
              }
              @case ("warning") {
                <svg viewBox="0 0 24 24" fill="currentColor">
                  <path
                    d="M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z"
                  />
                </svg>
              }
              @default {
                <svg viewBox="0 0 24 24" fill="currentColor">
                  <path
                    d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-6h2v6zm0-8h-2V7h2v2z"
                  />
                </svg>
              }
            }
          </div>
          <span class="toast-message">{{ toast.message }}</span>
          <button
            class="toast-close"
            (click)="toastService.remove(toast.id)"
            aria-label="Schließen"
          >
            <svg viewBox="0 0 24 24" fill="currentColor">
              <path
                d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"
              />
            </svg>
          </button>
        </div>
      }
    </div>
  `,
  styles: [
    `
      .toast-container {
        position: fixed;
        top: 1rem;
        right: 1rem;
        z-index: 9999;
        display: flex;
        flex-direction: column;
        gap: 0.5rem;
        max-width: 24rem;
      }

      .toast {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        padding: 0.75rem 1rem;
        border-radius: 0.5rem;
        box-shadow:
          0 4px 6px -1px rgba(0, 0, 0, 0.1),
          0 2px 4px -1px rgba(0, 0, 0, 0.06);
        animation: slideIn 0.3s ease-out;
      }

      @keyframes slideIn {
        from {
          transform: translateX(100%);
          opacity: 0;
        }
        to {
          transform: translateX(0);
          opacity: 1;
        }
      }

      .toast-success {
        background-color: #dcfce7;
        border: 1px solid #86efac;
        color: #166534;
      }

      .toast-error {
        background-color: #fef2f2;
        border: 1px solid #fecaca;
        color: #991b1b;
      }

      .toast-warning {
        background-color: #fef3c7;
        border: 1px solid #fcd34d;
        color: #92400e;
      }

      .toast-info {
        background-color: #dbeafe;
        border: 1px solid #93c5fd;
        color: #1e40af;
      }

      .toast-icon {
        flex-shrink: 0;
        width: 1.25rem;
        height: 1.25rem;
      }

      .toast-icon svg {
        width: 100%;
        height: 100%;
      }

      .toast-message {
        flex: 1;
        font-size: 0.875rem;
        font-weight: 500;
        line-height: 1.4;
      }

      .toast-close {
        flex-shrink: 0;
        width: 1.25rem;
        height: 1.25rem;
        padding: 0;
        border: none;
        background: transparent;
        cursor: pointer;
        opacity: 0.6;
        transition: opacity 0.2s;
      }

      .toast-close:hover {
        opacity: 1;
      }

      .toast-close svg {
        width: 100%;
        height: 100%;
        fill: currentColor;
      }
    `,
  ],
})
export class ToastContainerComponent {
  protected readonly toastService = inject(ToastService);
}
