import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Reusable form field wrapper component
 * Provides consistent styling for label and input groups
 */
@Component({
  selector: 'ui-form-field',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="form-field" [class.has-error]="hasError">
      <label [for]="inputId" class="form-label">
        {{ label }}
        @if (required) {
          <span class="required">*</span>
        }
      </label>
      <ng-content></ng-content>
      @if (hasError && errorMessage) {
        <span class="error-text">{{ errorMessage }}</span>
      }
      @if (hint && !hasError) {
        <span class="hint-text">{{ hint }}</span>
      }
    </div>
  `,
  styles: [`
    .form-field {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .form-label {
      font-size: 0.875rem;
      font-weight: 500;
      color: #374151;
    }

    .required {
      color: #dc3545;
      margin-left: 2px;
    }

    .error-text {
      font-size: 0.75rem;
      color: #dc3545;
    }

    .hint-text {
      font-size: 0.75rem;
      color: #6c757d;
    }

    .form-field.has-error :ng-deep input,
    .form-field.has-error :ng-deep select,
    .form-field.has-error :ng-deep textarea {
      border-color: #dc3545;
    }

    /* Default input styling through ng-deep */
    :ng-deep .form-field input,
    :ng-deep .form-field select,
    :ng-deep .form-field textarea {
      width: 100%;
      padding: 0.5rem 0.75rem;
      font-size: 1rem;
      border: 1px solid #ced4da;
      border-radius: 4px;
      transition: border-color 0.2s, box-shadow 0.2s;
    }

    :ng-deep .form-field input:focus,
    :ng-deep .form-field select:focus,
    :ng-deep .form-field textarea:focus {
      outline: none;
      border-color: #ff7f00;
      box-shadow: 0 0 0 3px rgba(255, 127, 0, 0.1);
    }

    :ng-deep .form-field input:disabled,
    :ng-deep .form-field select:disabled,
    :ng-deep .form-field textarea:disabled {
      background-color: #e9ecef;
      cursor: not-allowed;
    }
  `]
})
export class FormFieldComponent {
  @Input({ required: true }) label!: string;
  @Input() inputId?: string;
  @Input() required = false;
  @Input() hasError = false;
  @Input() errorMessage?: string;
  @Input() hint?: string;
}
