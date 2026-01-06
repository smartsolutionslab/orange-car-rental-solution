import { Component, input, output } from "@angular/core";
import { CommonModule } from "@angular/common";

/**
 * Error State Component
 * Displays an error message with optional retry action
 */
@Component({
  selector: "ui-error-state",
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      class="error-state"
      [class.error-inline]="variant() === 'inline'"
      [class.error-banner]="variant() === 'banner'"
    >
      <div class="error-icon">
        <svg viewBox="0 0 24 24" fill="currentColor">
          <path
            d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"
          />
        </svg>
      </div>
      <div class="error-content">
        @if (title() && variant() !== "inline") {
          <h3 class="error-title">{{ title() }}</h3>
        }
        <p class="error-message">{{ message() }}</p>
      </div>
      @if (showRetry()) {
        <button class="error-retry" (click)="retry.emit()">
          <svg viewBox="0 0 24 24" fill="currentColor">
            <path
              d="M17.65 6.35C16.2 4.9 14.21 4 12 4c-4.42 0-7.99 3.58-7.99 8s3.57 8 7.99 8c3.73 0 6.84-2.55 7.73-6h-2.08c-.82 2.33-3.04 4-5.65 4-3.31 0-6-2.69-6-6s2.69-6 6-6c1.66 0 3.14.69 4.22 1.78L13 11h7V4l-2.35 2.35z"
            />
          </svg>
          {{ retryLabel() }}
        </button>
      }
    </div>
  `,
  styles: [
    `
      .error-state {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        padding: 2rem;
        text-align: center;
        gap: 0.75rem;
      }
      .error-inline {
        flex-direction: row;
        padding: 0.75rem 1rem;
        background-color: #fef2f2;
        border: 1px solid #fecaca;
        border-radius: 0.375rem;
        text-align: left;
      }
      .error-banner {
        flex-direction: row;
        padding: 1rem 1.5rem;
        background-color: #fef2f2;
        border-left: 4px solid #ef4444;
        text-align: left;
      }
      .error-icon {
        flex-shrink: 0;
        width: 2.5rem;
        height: 2.5rem;
        color: #ef4444;
      }
      .error-inline .error-icon,
      .error-banner .error-icon {
        width: 1.25rem;
        height: 1.25rem;
      }
      .error-icon svg {
        width: 100%;
        height: 100%;
      }
      .error-content {
        flex: 1;
      }
      .error-inline .error-content,
      .error-banner .error-content {
        margin-left: 0.75rem;
      }
      .error-title {
        margin: 0 0 0.25rem 0;
        font-size: 1rem;
        font-weight: 600;
        color: #991b1b;
      }
      .error-message {
        margin: 0;
        font-size: 0.875rem;
        color: #991b1b;
      }
      .error-retry {
        display: inline-flex;
        align-items: center;
        gap: 0.375rem;
        padding: 0.5rem 1rem;
        background-color: white;
        color: #ef4444;
        border: 1px solid #ef4444;
        border-radius: 0.375rem;
        font-size: 0.875rem;
        font-weight: 500;
        cursor: pointer;
        transition:
          background-color 0.15s ease,
          color 0.15s ease;
      }
      .error-inline .error-retry,
      .error-banner .error-retry {
        margin-left: auto;
      }
      .error-retry:hover {
        background-color: #ef4444;
        color: white;
      }
      .error-retry svg {
        width: 1rem;
        height: 1rem;
      }
    `,
  ],
})
export class ErrorStateComponent {
  readonly message = input.required<string>();
  readonly title = input("Fehler");
  readonly variant = input<"default" | "inline" | "banner">("default");
  readonly showRetry = input(false);
  readonly retryLabel = input("Erneut versuchen");

  readonly retry = output<void>();
}
