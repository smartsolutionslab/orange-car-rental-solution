import {
  Component,
  input,
  output,
  signal,
  forwardRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ControlValueAccessor,
  NG_VALUE_ACCESSOR,
} from '@angular/forms';
import { IconComponent } from '../icon';

export type CheckboxSize = 'sm' | 'md' | 'lg';

/**
 * Custom Checkbox Component
 *
 * A styled checkbox with custom checkmark icon, label support,
 * and reactive forms integration.
 *
 * @example
 * <ocr-checkbox
 *   [label]="'Accept terms and conditions'"
 *   [(ngModel)]="acceptTerms"
 * />
 */
@Component({
  selector: 'ocr-checkbox',
  standalone: true,
  imports: [CommonModule, IconComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CheckboxComponent),
      multi: true,
    },
  ],
  template: `
    <label
      class="checkbox-wrapper"
      [class.checkbox-sm]="size() === 'sm'"
      [class.checkbox-lg]="size() === 'lg'"
      [class.disabled]="disabled()"
      [class.has-error]="error()"
    >
      <input
        type="checkbox"
        class="checkbox-input"
        [checked]="checked()"
        [disabled]="disabled()"
        [attr.aria-invalid]="error() ? 'true' : null"
        [attr.aria-describedby]="error() ? checkboxId() + '-error' : null"
        (change)="onCheckboxChange($event)"
        (blur)="onBlur()"
      />
      <span class="checkbox-box" [class.checked]="checked()">
        @if (checked()) {
          <lib-icon name="check" variant="outline" [size]="iconSize()" />
        }
      </span>
      @if (label()) {
        <span class="checkbox-label">
          {{ label() }}
          @if (required()) {
            <span class="required-marker">*</span>
          }
        </span>
      }
      <ng-content></ng-content>
    </label>
    @if (error()) {
      <span [id]="checkboxId() + '-error'" class="checkbox-error">{{ error() }}</span>
    } @else if (hint()) {
      <span class="checkbox-hint">{{ hint() }}</span>
    }
  `,
  styles: [`
    :host {
      display: block;
    }

    .checkbox-wrapper {
      display: inline-flex;
      align-items: flex-start;
      gap: 0.5rem;
      cursor: pointer;
      user-select: none;
    }

    .checkbox-wrapper.disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .checkbox-input {
      position: absolute;
      opacity: 0;
      width: 0;
      height: 0;
    }

    .checkbox-box {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 1.25rem;
      height: 1.25rem;
      flex-shrink: 0;
      background: white;
      border: 2px solid #d1d5db;
      border-radius: 0.25rem;
      transition: all 0.15s ease;
      color: white;
    }

    .checkbox-box.checked {
      background-color: #f97316;
      border-color: #f97316;
    }

    .checkbox-wrapper:hover:not(.disabled) .checkbox-box:not(.checked) {
      border-color: #f97316;
    }

    .checkbox-input:focus + .checkbox-box {
      box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.2);
    }

    .checkbox-wrapper.has-error .checkbox-box {
      border-color: #ef4444;
    }

    .checkbox-wrapper.has-error .checkbox-box.checked {
      background-color: #ef4444;
      border-color: #ef4444;
    }

    .checkbox-label {
      font-size: 0.875rem;
      color: #374151;
      line-height: 1.25rem;
    }

    .required-marker {
      color: #ef4444;
      margin-left: 0.125rem;
    }

    /* Size variants */
    .checkbox-sm .checkbox-box {
      width: 1rem;
      height: 1rem;
    }

    .checkbox-sm .checkbox-label {
      font-size: 0.75rem;
      line-height: 1rem;
    }

    .checkbox-lg .checkbox-box {
      width: 1.5rem;
      height: 1.5rem;
    }

    .checkbox-lg .checkbox-label {
      font-size: 1rem;
      line-height: 1.5rem;
    }

    /* Error and hint */
    .checkbox-error {
      display: block;
      font-size: 0.75rem;
      color: #ef4444;
      margin-top: 0.25rem;
      margin-left: 1.75rem;
    }

    .checkbox-hint {
      display: block;
      font-size: 0.75rem;
      color: #6b7280;
      margin-top: 0.25rem;
      margin-left: 1.75rem;
    }

    .checkbox-sm .checkbox-error,
    .checkbox-sm .checkbox-hint {
      margin-left: 1.5rem;
    }

    .checkbox-lg .checkbox-error,
    .checkbox-lg .checkbox-hint {
      margin-left: 2rem;
    }
  `],
})
export class CheckboxComponent implements ControlValueAccessor {
  private static idCounter = 0;

  /** Checkbox label */
  readonly label = input<string>();

  /** Checkbox size */
  readonly size = input<CheckboxSize>('md');

  /** Error message */
  readonly error = input<string | null>(null);

  /** Hint text */
  readonly hint = input<string>();

  /** Whether checkbox is required */
  readonly required = input(false);

  /** Whether checkbox is disabled */
  readonly disabled = input(false);

  /** Change event */
  readonly checkedChange = output<boolean>();

  /** Unique ID for checkbox */
  readonly checkboxId = signal(`ocr-checkbox-${++CheckboxComponent.idCounter}`);

  /** Checked state */
  readonly checked = signal(false);

  /** Computed icon size based on checkbox size */
  readonly iconSize = () => {
    const size = this.size();
    if (size === 'sm') return 'xs';
    if (size === 'lg') return 'md';
    return 'sm';
  };

  private onChange: (value: boolean) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: boolean): void {
    this.checked.set(!!value);
  }

  registerOnChange(fn: (value: boolean) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    // Disabled state is handled via input signal
  }

  onCheckboxChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.checked.set(checked);
    this.onChange(checked);
    this.checkedChange.emit(checked);
  }

  onBlur(): void {
    this.onTouched();
  }

  /** Toggle the checkbox programmatically */
  toggle(): void {
    if (!this.disabled()) {
      const newValue = !this.checked();
      this.checked.set(newValue);
      this.onChange(newValue);
      this.checkedChange.emit(newValue);
    }
  }
}
