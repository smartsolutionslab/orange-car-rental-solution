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

export type SwitchSize = 'sm' | 'md' | 'lg';

/**
 * Toggle Switch Component
 *
 * A styled toggle switch with label support and reactive forms integration.
 *
 * @example
 * <ocr-switch
 *   [label]="'Enable notifications'"
 *   [(ngModel)]="notificationsEnabled"
 * />
 */
@Component({
  selector: 'ocr-switch',
  standalone: true,
  imports: [CommonModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SwitchComponent),
      multi: true,
    },
  ],
  template: `
    <label
      class="switch-wrapper"
      [class.switch-sm]="size() === 'sm'"
      [class.switch-lg]="size() === 'lg'"
      [class.disabled]="disabled()"
      [class.label-left]="labelPosition() === 'left'"
    >
      @if (label() && labelPosition() === 'left') {
        <span class="switch-label">{{ label() }}</span>
      }

      <button
        type="button"
        role="switch"
        class="switch-track"
        [class.checked]="checked()"
        [attr.aria-checked]="checked()"
        [attr.aria-label]="ariaLabel() || label()"
        [disabled]="disabled()"
        (click)="toggle()"
        (blur)="onBlur()"
      >
        <span class="switch-thumb"></span>
      </button>

      @if (label() && labelPosition() === 'right') {
        <span class="switch-label">{{ label() }}</span>
      }

      @if (description()) {
        <span class="switch-description">{{ description() }}</span>
      }
    </label>
  `,
  styles: [`
    :host {
      display: inline-block;
    }

    .switch-wrapper {
      display: inline-flex;
      align-items: center;
      gap: 0.75rem;
      cursor: pointer;
      user-select: none;
    }

    .switch-wrapper.disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .switch-wrapper.label-left {
      flex-direction: row;
    }

    .switch-track {
      position: relative;
      display: inline-flex;
      align-items: center;
      width: 2.75rem;
      height: 1.5rem;
      flex-shrink: 0;
      background-color: #d1d5db;
      border: none;
      border-radius: 9999px;
      cursor: pointer;
      transition: background-color 0.2s ease;
      padding: 0;
    }

    .switch-track:focus {
      outline: none;
      box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.2);
    }

    .switch-track:disabled {
      cursor: not-allowed;
    }

    .switch-track.checked {
      background-color: #f97316;
    }

    .switch-thumb {
      position: absolute;
      left: 0.125rem;
      width: 1.25rem;
      height: 1.25rem;
      background-color: white;
      border-radius: 50%;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1), 0 1px 2px rgba(0, 0, 0, 0.06);
      transition: transform 0.2s ease;
    }

    .switch-track.checked .switch-thumb {
      transform: translateX(1.25rem);
    }

    .switch-label {
      font-size: 0.875rem;
      font-weight: 500;
      color: #374151;
    }

    .switch-description {
      display: block;
      font-size: 0.75rem;
      color: #6b7280;
      margin-left: auto;
      width: 100%;
      margin-top: 0.25rem;
    }

    /* Size variants */
    .switch-sm .switch-track {
      width: 2rem;
      height: 1.125rem;
    }

    .switch-sm .switch-thumb {
      width: 0.875rem;
      height: 0.875rem;
    }

    .switch-sm .switch-track.checked .switch-thumb {
      transform: translateX(0.875rem);
    }

    .switch-sm .switch-label {
      font-size: 0.75rem;
    }

    .switch-lg .switch-track {
      width: 3.5rem;
      height: 2rem;
    }

    .switch-lg .switch-thumb {
      width: 1.75rem;
      height: 1.75rem;
    }

    .switch-lg .switch-track.checked .switch-thumb {
      transform: translateX(1.5rem);
    }

    .switch-lg .switch-label {
      font-size: 1rem;
    }
  `],
})
export class SwitchComponent implements ControlValueAccessor {
  /** Switch label */
  readonly label = input<string>();

  /** Label position */
  readonly labelPosition = input<'left' | 'right'>('right');

  /** Description text */
  readonly description = input<string>();

  /** Switch size */
  readonly size = input<SwitchSize>('md');

  /** Aria label for accessibility */
  readonly ariaLabel = input<string>();

  /** Whether switch is disabled */
  readonly disabled = input(false);

  /** Change event */
  readonly checkedChange = output<boolean>();

  /** Checked state */
  readonly checked = signal(false);

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

  toggle(): void {
    if (!this.disabled()) {
      const newValue = !this.checked();
      this.checked.set(newValue);
      this.onChange(newValue);
      this.checkedChange.emit(newValue);
    }
  }

  onBlur(): void {
    this.onTouched();
  }
}
