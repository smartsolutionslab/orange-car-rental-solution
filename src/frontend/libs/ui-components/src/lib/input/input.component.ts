import {
  Component,
  input,
  output,
  signal,
  computed,
  forwardRef,
  ElementRef,
  viewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import type { ControlValueAccessor } from '@angular/forms';
import { NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { IconComponent } from '../icon';

export type InputSize = 'sm' | 'md' | 'lg';
export type InputType = 'text' | 'email' | 'password' | 'number' | 'tel' | 'url' | 'search';

/**
 * Styled Input Component
 *
 * A configurable input field with icon support, error states,
 * and reactive forms integration.
 *
 * @example
 * <ocr-input
 *   [label]="'Email'"
 *   [placeholder]="'Enter your email'"
 *   [leadingIcon]="'mail'"
 *   [error]="emailError()"
 *   [(ngModel)]="email"
 * />
 */
@Component({
  selector: 'ocr-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, IconComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true,
    },
  ],
  template: `
    <div class="input-wrapper" [class.disabled]="disabled()">
      @if (label()) {
        <label [for]="inputId()" class="input-label">
          {{ label() }}
          @if (required()) {
            <span class="required-marker">*</span>
          }
        </label>
      }

      <div
        class="input-container"
        [class.input-sm]="size() === 'sm'"
        [class.input-lg]="size() === 'lg'"
        [class.has-error]="error()"
        [class.has-leading-icon]="leadingIcon()"
        [class.has-trailing-icon]="trailingIcon() || (type() === 'password' && showPasswordToggle())"
        [class.focused]="isFocused()"
      >
        @if (leadingIcon()) {
          <span class="input-icon leading">
            <lib-icon [name]="leadingIcon()!" variant="outline" [size]="iconSize()" />
          </span>
        }

        <input
          #inputElement
          [id]="inputId()"
          [type]="actualType()"
          [placeholder]="placeholder()"
          [disabled]="disabled()"
          [readonly]="readonly()"
          [attr.aria-invalid]="error() ? 'true' : null"
          [attr.aria-describedby]="error() ? inputId() + '-error' : null"
          [autocomplete]="autocomplete()"
          class="input-field"
          [value]="value()"
          (input)="onInput($event)"
          (blur)="onBlur()"
          (focus)="onFocus()"
        />

        @if (type() === 'password' && showPasswordToggle()) {
          <button
            type="button"
            class="input-icon trailing password-toggle"
            (click)="togglePasswordVisibility()"
            [attr.aria-label]="showPassword() ? 'Hide password' : 'Show password'"
            tabindex="-1"
          >
            <lib-icon
              [name]="showPassword() ? 'eye-off' : 'eye'"
              variant="outline"
              [size]="iconSize()"
            />
          </button>
        } @else if (trailingIcon()) {
          <span class="input-icon trailing">
            <lib-icon [name]="trailingIcon()!" variant="outline" [size]="iconSize()" />
          </span>
        }
      </div>

      @if (error()) {
        <span [id]="inputId() + '-error'" class="input-error">{{ error() }}</span>
      } @else if (hint()) {
        <span class="input-hint">{{ hint() }}</span>
      }
    </div>
  `,
  styles: [`
    .input-wrapper {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
      width: 100%;
    }

    .input-wrapper.disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .input-label {
      font-size: 0.875rem;
      font-weight: 500;
      color: #374151;
    }

    .required-marker {
      color: #ef4444;
      margin-left: 0.125rem;
    }

    .input-container {
      position: relative;
      display: flex;
      align-items: center;
      width: 100%;
      background: white;
      border: 1px solid #d1d5db;
      border-radius: 0.375rem;
      transition: border-color 0.15s ease, box-shadow 0.15s ease;
    }

    .input-container.focused {
      border-color: #f97316;
      box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.1);
    }

    .input-container.has-error {
      border-color: #ef4444;
    }

    .input-container.has-error.focused {
      box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1);
    }

    .input-field {
      flex: 1;
      width: 100%;
      padding: 0.625rem 0.75rem;
      font-size: 0.875rem;
      color: #111827;
      background: transparent;
      border: none;
      outline: none;
    }

    .input-field::placeholder {
      color: #9ca3af;
    }

    .input-field:disabled {
      cursor: not-allowed;
    }

    /* Size variants */
    .input-sm .input-field {
      padding: 0.375rem 0.625rem;
      font-size: 0.75rem;
    }

    .input-lg .input-field {
      padding: 0.75rem 1rem;
      font-size: 1rem;
    }

    /* Icon adjustments */
    .input-icon {
      display: flex;
      align-items: center;
      justify-content: center;
      color: #6b7280;
      flex-shrink: 0;
    }

    .input-icon.leading {
      padding-left: 0.75rem;
    }

    .input-icon.trailing {
      padding-right: 0.75rem;
    }

    .has-leading-icon .input-field {
      padding-left: 0.375rem;
    }

    .has-trailing-icon .input-field {
      padding-right: 0.375rem;
    }

    .password-toggle {
      background: none;
      border: none;
      cursor: pointer;
      padding: 0.25rem;
      margin-right: 0.5rem;
      border-radius: 0.25rem;
      transition: background-color 0.15s ease;
    }

    .password-toggle:hover {
      background-color: #f3f4f6;
    }

    .password-toggle:focus {
      outline: none;
    }

    /* Error and hint messages */
    .input-error {
      font-size: 0.75rem;
      color: #ef4444;
      margin-top: 0.25rem;
    }

    .input-hint {
      font-size: 0.75rem;
      color: #6b7280;
      margin-top: 0.25rem;
    }
  `],
})
export class InputComponent implements ControlValueAccessor {
  private static idCounter = 0;

  /** Input label */
  readonly label = input<string>();

  /** Input placeholder */
  readonly placeholder = input<string>('');

  /** Input type */
  readonly type = input<InputType>('text');

  /** Input size */
  readonly size = input<InputSize>('md');

  /** Leading icon name */
  readonly leadingIcon = input<string>();

  /** Trailing icon name */
  readonly trailingIcon = input<string>();

  /** Error message */
  readonly error = input<string | null>(null);

  /** Hint text */
  readonly hint = input<string>();

  /** Whether input is required */
  readonly required = input(false);

  /** Whether input is disabled */
  readonly disabled = input(false);

  /** Whether input is readonly */
  readonly readonly = input(false);

  /** Autocomplete attribute */
  readonly autocomplete = input<string>('off');

  /** Show password toggle for password inputs */
  readonly showPasswordToggle = input(true);

  /** Blur event */
  readonly inputBlur = output<void>();

  /** Focus event */
  readonly inputFocus = output<void>();

  /** Reference to input element */
  private readonly inputElement = viewChild<ElementRef<HTMLInputElement>>('inputElement');

  /** Unique ID for input */
  readonly inputId = signal(`ocr-input-${++InputComponent.idCounter}`);

  /** Current value */
  readonly value = signal('');

  /** Focus state */
  readonly isFocused = signal(false);

  /** Password visibility state */
  readonly showPassword = signal(false);

  /** Computed actual type for password toggle */
  readonly actualType = computed(() => {
    if (this.type() === 'password' && this.showPassword()) {
      return 'text';
    }
    return this.type();
  });

  /** Computed icon size based on input size */
  readonly iconSize = computed(() => {
    const size = this.size();
    if (size === 'sm') return 'xs';
    if (size === 'lg') return 'md';
    return 'sm';
  });

  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string): void {
    this.value.set(value ?? '');
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(_isDisabled: boolean): void {
    // Disabled state is handled via input signal
  }

  onInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.value.set(value);
    this.onChange(value);
  }

  onBlur(): void {
    this.isFocused.set(false);
    this.onTouched();
    this.inputBlur.emit();
  }

  onFocus(): void {
    this.isFocused.set(true);
    this.inputFocus.emit();
  }

  togglePasswordVisibility(): void {
    this.showPassword.set(!this.showPassword());
  }

  /** Focus the input element programmatically */
  focus(): void {
    this.inputElement()?.nativeElement.focus();
  }

  /** Blur the input element programmatically */
  blur(): void {
    this.inputElement()?.nativeElement.blur();
  }
}
