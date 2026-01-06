import {
  Component,
  input,
  output,
  signal,
  forwardRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import type { ControlValueAccessor } from '@angular/forms';
import { NG_VALUE_ACCESSOR } from '@angular/forms';

export type RadioGroupSize = 'sm' | 'md' | 'lg';
export type RadioGroupOrientation = 'horizontal' | 'vertical';

export interface RadioOption<T = string> {
  /** The value of the option */
  value: T;
  /** Display label */
  label: string;
  /** Whether option is disabled */
  disabled?: boolean;
  /** Optional description */
  description?: string;
}

/**
 * Radio Group Component
 *
 * A styled radio button group with options, label support,
 * and reactive forms integration.
 *
 * @example
 * <ocr-radio-group
 *   [label]="'Select size'"
 *   [options]="sizeOptions"
 *   [(ngModel)]="selectedSize"
 * />
 */
@Component({
  selector: 'ocr-radio-group',
  standalone: true,
  imports: [CommonModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RadioGroupComponent),
      multi: true,
    },
  ],
  template: `
    <fieldset class="radio-group" [class.disabled]="disabled()">
      @if (label()) {
        <legend class="radio-group-label">
          {{ label() }}
          @if (required()) {
            <span class="required-marker">*</span>
          }
        </legend>
      }

      <div
        class="radio-options"
        [class.horizontal]="orientation() === 'horizontal'"
        [class.vertical]="orientation() === 'vertical'"
        role="radiogroup"
        [attr.aria-invalid]="error() ? 'true' : null"
        [attr.aria-describedby]="error() ? groupId() + '-error' : null"
      >
        @for (option of options(); track option.value) {
          <label
            class="radio-option"
            [class.radio-sm]="size() === 'sm'"
            [class.radio-lg]="size() === 'lg'"
            [class.disabled]="option.disabled || disabled()"
            [class.selected]="isSelected(option.value)"
          >
            <input
              type="radio"
              class="radio-input"
              [name]="name()"
              [value]="option.value"
              [checked]="isSelected(option.value)"
              [disabled]="option.disabled || disabled()"
              (change)="onRadioChange(option.value)"
              (blur)="onBlur()"
            />
            <span class="radio-circle">
              <span class="radio-dot"></span>
            </span>
            <span class="radio-content">
              <span class="radio-label">{{ option.label }}</span>
              @if (option.description) {
                <span class="radio-description">{{ option.description }}</span>
              }
            </span>
          </label>
        }
      </div>

      @if (error()) {
        <span [id]="groupId() + '-error'" class="radio-error">{{ error() }}</span>
      } @else if (hint()) {
        <span class="radio-hint">{{ hint() }}</span>
      }
    </fieldset>
  `,
  styles: [`
    .radio-group {
      border: none;
      padding: 0;
      margin: 0;
    }

    .radio-group.disabled {
      opacity: 0.6;
    }

    .radio-group-label {
      display: block;
      font-size: 0.875rem;
      font-weight: 500;
      color: #374151;
      margin-bottom: 0.5rem;
    }

    .required-marker {
      color: #ef4444;
      margin-left: 0.125rem;
    }

    .radio-options {
      display: flex;
      gap: 0.75rem;
    }

    .radio-options.vertical {
      flex-direction: column;
    }

    .radio-options.horizontal {
      flex-direction: row;
      flex-wrap: wrap;
    }

    .radio-option {
      display: flex;
      align-items: flex-start;
      gap: 0.5rem;
      cursor: pointer;
      user-select: none;
    }

    .radio-option.disabled {
      cursor: not-allowed;
    }

    .radio-input {
      position: absolute;
      opacity: 0;
      width: 0;
      height: 0;
    }

    .radio-circle {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 1.25rem;
      height: 1.25rem;
      flex-shrink: 0;
      background: white;
      border: 2px solid #d1d5db;
      border-radius: 50%;
      transition: all 0.15s ease;
    }

    .radio-dot {
      width: 0.5rem;
      height: 0.5rem;
      background-color: transparent;
      border-radius: 50%;
      transition: all 0.15s ease;
    }

    .radio-option.selected .radio-circle {
      border-color: #f97316;
    }

    .radio-option.selected .radio-dot {
      background-color: #f97316;
    }

    .radio-option:hover:not(.disabled) .radio-circle {
      border-color: #f97316;
    }

    .radio-input:focus + .radio-circle {
      box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.2);
    }

    .radio-content {
      display: flex;
      flex-direction: column;
      gap: 0.125rem;
    }

    .radio-label {
      font-size: 0.875rem;
      color: #374151;
      line-height: 1.25rem;
    }

    .radio-description {
      font-size: 0.75rem;
      color: #6b7280;
      line-height: 1rem;
    }

    /* Size variants */
    .radio-sm .radio-circle {
      width: 1rem;
      height: 1rem;
    }

    .radio-sm .radio-dot {
      width: 0.375rem;
      height: 0.375rem;
    }

    .radio-sm .radio-label {
      font-size: 0.75rem;
      line-height: 1rem;
    }

    .radio-sm .radio-description {
      font-size: 0.625rem;
    }

    .radio-lg .radio-circle {
      width: 1.5rem;
      height: 1.5rem;
    }

    .radio-lg .radio-dot {
      width: 0.625rem;
      height: 0.625rem;
    }

    .radio-lg .radio-label {
      font-size: 1rem;
      line-height: 1.5rem;
    }

    .radio-lg .radio-description {
      font-size: 0.875rem;
    }

    /* Error and hint */
    .radio-error {
      display: block;
      font-size: 0.75rem;
      color: #ef4444;
      margin-top: 0.5rem;
    }

    .radio-hint {
      display: block;
      font-size: 0.75rem;
      color: #6b7280;
      margin-top: 0.5rem;
    }
  `],
})
export class RadioGroupComponent<T = string> implements ControlValueAccessor {
  private static idCounter = 0;

  /** Group label */
  readonly label = input<string>();

  /** Radio options */
  readonly options = input.required<RadioOption<T>[]>();

  /** Group name (for native radio behavior) */
  readonly name = input<string>(`ocr-radio-${++RadioGroupComponent.idCounter}`);

  /** Radio size */
  readonly size = input<RadioGroupSize>('md');

  /** Orientation */
  readonly orientation = input<RadioGroupOrientation>('vertical');

  /** Error message */
  readonly error = input<string | null>(null);

  /** Hint text */
  readonly hint = input<string>();

  /** Whether group is required */
  readonly required = input(false);

  /** Whether group is disabled */
  readonly disabled = input(false);

  /** Value change event */
  readonly valueChange = output<T>();

  /** Unique ID for group */
  readonly groupId = signal(`ocr-radio-group-${RadioGroupComponent.idCounter}`);

  /** Selected value */
  readonly selectedValue = signal<T | null>(null);

  private onChange: (value: T) => void = () => {};
  private onTouched: () => void = () => {};

  /** Check if option is selected */
  isSelected(value: T): boolean {
    return this.selectedValue() === value;
  }

  writeValue(value: T): void {
    this.selectedValue.set(value);
  }

  registerOnChange(fn: (value: T) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(_isDisabled: boolean): void {
    // Disabled state is handled via input signal
  }

  onRadioChange(value: T): void {
    this.selectedValue.set(value);
    this.onChange(value);
    this.valueChange.emit(value);
  }

  onBlur(): void {
    this.onTouched();
  }
}
