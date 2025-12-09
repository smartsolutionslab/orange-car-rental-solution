import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Error Alert Component
 * Displays an inline error message for forms
 *
 * @example
 * <ui-error-alert message="Ungültige E-Mail-Adresse"></ui-error-alert>
 */
@Component({
  selector: 'ui-error-alert',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="error-alert" role="alert">
      <svg class="alert-icon" viewBox="0 0 24 24" fill="currentColor">
        <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"/>
      </svg>
      <span class="alert-message">{{ message }}</span>
    </div>
  `,
  styles: [`
    .error-alert {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.75rem 1rem;
      background-color: #fef2f2;
      border: 1px solid #fecaca;
      border-radius: 0.375rem;
      color: #991b1b;
      margin-bottom: 1rem;
    }

    .alert-icon {
      flex-shrink: 0;
      width: 1.25rem;
      height: 1.25rem;
    }

    .alert-message {
      font-size: 0.875rem;
      font-weight: 500;
    }
  `]
})
export class ErrorAlertComponent {
  @Input({ required: true }) message!: string;
}
