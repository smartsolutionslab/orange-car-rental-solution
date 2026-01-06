import { Component, input } from "@angular/core";

/**
 * Loading State Component
 * Displays a spinner with customizable loading message
 *
 * @example
 * <ui-loading-state message="Lade Fahrzeuge..."></ui-loading-state>
 */
@Component({
  selector: "ui-loading-state",
  standalone: true,
  template: `
    <div class="loading-state">
      <div
        class="loading-spinner"
        [class.spinner-sm]="size() === 'sm'"
        [class.spinner-lg]="size() === 'lg'"
      ></div>
      @if (message()) {
        <p class="loading-message">{{ message() }}</p>
      }
    </div>
  `,
  styles: [
    `
      .loading-state {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        padding: 2rem;
        gap: 1rem;
      }

      .loading-spinner {
        width: 2.5rem;
        height: 2.5rem;
        border: 3px solid #e5e7eb;
        border-top-color: #f97316;
        border-radius: 50%;
        animation: spin 0.8s linear infinite;
      }

      .spinner-sm {
        width: 1.5rem;
        height: 1.5rem;
        border-width: 2px;
      }

      .spinner-lg {
        width: 3.5rem;
        height: 3.5rem;
        border-width: 4px;
      }

      @keyframes spin {
        to {
          transform: rotate(360deg);
        }
      }

      .loading-message {
        margin: 0;
        color: #6b7280;
        font-size: 0.875rem;
      }
    `,
  ],
})
export class LoadingStateComponent {
  readonly message = input("Laden...");
  readonly size = input<"sm" | "md" | "lg">("md");
}
