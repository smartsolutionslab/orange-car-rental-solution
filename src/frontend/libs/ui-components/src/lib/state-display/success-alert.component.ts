import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Success Alert Component
 * Displays a success message with optional icon
 *
 * @example
 * <ui-success-alert message="Daten erfolgreich gespeichert"></ui-success-alert>
 */
@Component({
  selector: 'ui-success-alert',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="success-alert" role="alert">
      <svg class="alert-icon" viewBox="0 0 24 24" fill="currentColor">
        <path d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z"/>
      </svg>
      <span class="alert-message">{{ message }}</span>
    </div>
  `,
  styles: [`
    .success-alert {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.75rem 1rem;
      background-color: #dcfce7;
      border: 1px solid #86efac;
      border-radius: 0.375rem;
      color: #166534;
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
export class SuccessAlertComponent {
  @Input({ required: true }) message!: string;
}
