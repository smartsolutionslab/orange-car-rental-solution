import {
  Component,
  input,
  output,
  signal,
  computed,
  forwardRef,
  ElementRef,
  viewChild,
} from "@angular/core";
import { CommonModule } from "@angular/common";
import type { ControlValueAccessor } from "@angular/forms";
import { NG_VALUE_ACCESSOR, ReactiveFormsModule } from "@angular/forms";
import { IconComponent } from "../icon";

export type SelectSize = "sm" | "md" | "lg";

export interface SelectOption<T = string> {
  value: T;
  label: string;
  disabled?: boolean;
}

/**
 * Generic Select Component
 *
 * A configurable select field with icon support, error states,
 * and reactive forms integration.
 *
 * @example
 * <ocr-select
 *   [label]="'Sort By'"
 *   [options]="sortOptions"
 *   [placeholder]="'Select an option'"
 *   [(ngModel)]="selectedValue"
 * />
 */
@Component({
  selector: "ocr-select",
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, IconComponent],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectComponent),
      multi: true,
    },
  ],
  template: `
    <div class="select-wrapper" [class.disabled]="disabled()">
      @if (label()) {
        <label [for]="selectId()" class="select-label">
          {{ label() }}
          @if (required()) {
            <span class="required-marker">*</span>
          }
        </label>
      }

      <div
        class="select-container"
        [class.select-sm]="size() === 'sm'"
        [class.select-lg]="size() === 'lg'"
        [class.has-error]="error()"
        [class.has-leading-icon]="leadingIcon()"
        [class.focused]="isFocused()"
      >
        @if (leadingIcon()) {
          <span class="select-icon leading">
            <lib-icon
              [name]="leadingIcon()!"
              variant="outline"
              [size]="iconSize()"
            />
          </span>
        }

        <select
          #selectElement
          [id]="selectId()"
          [disabled]="disabled()"
          [attr.aria-invalid]="error() ? 'true' : null"
          [attr.aria-describedby]="error() ? selectId() + '-error' : null"
          class="select-field"
          (change)="onSelectChange($event)"
          (blur)="onBlur()"
          (focus)="onFocus()"
        >
          @if (placeholder()) {
            <option value="" [selected]="!value()">{{ placeholder() }}</option>
          }
          @for (option of options(); track option.value) {
            <option
              [value]="option.value"
              [selected]="option.value === value()"
              [disabled]="option.disabled"
            >
              {{ option.label }}
            </option>
          }
        </select>

        <span class="select-icon trailing chevron">
          <lib-icon name="chevron-down" variant="outline" [size]="iconSize()" />
        </span>
      </div>

      @if (error()) {
        <span [id]="selectId() + '-error'" class="select-error">{{
          error()
        }}</span>
      } @else if (hint()) {
        <span class="select-hint">{{ hint() }}</span>
      }
    </div>
  `,
  styles: [
    `
      .select-wrapper {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
        width: 100%;
      }

      .select-wrapper.disabled {
        opacity: 0.6;
        cursor: not-allowed;
      }

      .select-label {
        font-size: 0.875rem;
        font-weight: 500;
        color: #374151;
      }

      .required-marker {
        color: #ef4444;
        margin-left: 0.125rem;
      }

      .select-container {
        position: relative;
        display: flex;
        align-items: center;
        width: 100%;
        background: white;
        border: 1px solid #d1d5db;
        border-radius: 0.375rem;
        transition:
          border-color 0.15s ease,
          box-shadow 0.15s ease;
      }

      .select-container.focused {
        border-color: #f97316;
        box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.1);
      }

      .select-container.has-error {
        border-color: #ef4444;
      }

      .select-container.has-error.focused {
        box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1);
      }

      .select-field {
        flex: 1;
        width: 100%;
        padding: 0.625rem 0.75rem;
        padding-right: 2.5rem;
        font-size: 0.875rem;
        color: #111827;
        background: transparent;
        border: none;
        outline: none;
        cursor: pointer;
        appearance: none;
        -webkit-appearance: none;
        -moz-appearance: none;
      }

      .select-field:disabled {
        cursor: not-allowed;
      }

      /* Size variants */
      .select-sm .select-field {
        padding: 0.375rem 0.625rem;
        padding-right: 2rem;
        font-size: 0.75rem;
      }

      .select-lg .select-field {
        padding: 0.75rem 1rem;
        padding-right: 3rem;
        font-size: 1rem;
      }

      /* Icon adjustments */
      .select-icon {
        display: flex;
        align-items: center;
        justify-content: center;
        color: #6b7280;
        flex-shrink: 0;
        pointer-events: none;
      }

      .select-icon.leading {
        padding-left: 0.75rem;
      }

      .select-icon.trailing {
        position: absolute;
        right: 0.75rem;
      }

      .select-icon.chevron {
        color: #9ca3af;
      }

      .has-leading-icon .select-field {
        padding-left: 0.375rem;
      }

      /* Error and hint messages */
      .select-error {
        font-size: 0.75rem;
        color: #ef4444;
        margin-top: 0.25rem;
      }

      .select-hint {
        font-size: 0.75rem;
        color: #6b7280;
        margin-top: 0.25rem;
      }
    `,
  ],
})
export class SelectComponent<T = string> implements ControlValueAccessor {
  private static idCounter = 0;

  /** Select label */
  readonly label = input<string>();

  /** Select placeholder */
  readonly placeholder = input<string>();

  /** Select options */
  readonly options = input<SelectOption<T>[]>([]);

  /** Select size */
  readonly size = input<SelectSize>("md");

  /** Leading icon name */
  readonly leadingIcon = input<string>();

  /** Error message */
  readonly error = input<string | null>(null);

  /** Hint text */
  readonly hint = input<string>();

  /** Whether select is required */
  readonly required = input(false);

  /** Whether select is disabled */
  readonly disabled = input(false);

  /** Change event */
  readonly selectChange = output<T>();

  /** Blur event */
  readonly selectBlur = output<void>();

  /** Focus event */
  readonly selectFocus = output<void>();

  /** Reference to select element */
  private readonly selectElement =
    viewChild<ElementRef<HTMLSelectElement>>("selectElement");

  /** Unique ID for select */
  readonly selectId = signal(`ocr-select-${++SelectComponent.idCounter}`);

  /** Current value */
  readonly value = signal<T | null>(null);

  /** Focus state */
  readonly isFocused = signal(false);

  /** Computed icon size based on select size */
  readonly iconSize = computed(() => {
    const size = this.size();
    if (size === "sm") return "xs";
    if (size === "lg") return "md";
    return "sm";
  });

  private onChange: (value: T | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: T | null): void {
    this.value.set(value);
  }

  registerOnChange(fn: (value: T | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(_isDisabled: boolean): void {
    // Disabled state is handled via input signal
  }

  onSelectChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    const selectedValue = target.value as unknown as T;
    const newValue = selectedValue === ("" as unknown as T) ? null : selectedValue;
    this.value.set(newValue);
    this.onChange(newValue);
    this.selectChange.emit(newValue as T);
  }

  onBlur(): void {
    this.isFocused.set(false);
    this.onTouched();
    this.selectBlur.emit();
  }

  onFocus(): void {
    this.isFocused.set(true);
    this.selectFocus.emit();
  }

  /** Focus the select element programmatically */
  focus(): void {
    this.selectElement()?.nativeElement.focus();
  }

  /** Blur the select element programmatically */
  blur(): void {
    this.selectElement()?.nativeElement.blur();
  }
}
