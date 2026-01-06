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

export type TextareaSize = 'sm' | 'md' | 'lg';
export type TextareaResize = 'none' | 'vertical' | 'horizontal' | 'both';

/**
 * Styled Textarea Component
 *
 * A configurable textarea with error states, character count,
 * and reactive forms integration.
 *
 * @example
 * <ocr-textarea
 *   [label]="'Description'"
 *   [placeholder]="'Enter a description...'"
 *   [maxLength]="500"
 *   [showCharCount]="true"
 *   [(ngModel)]="description"
 * />
 */
@Component({
  selector: 'ocr-textarea',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TextareaComponent),
      multi: true,
    },
  ],
  template: `
    <div class="textarea-wrapper" [class.disabled]="disabled()">
      @if (label()) {
        <label [for]="textareaId()" class="textarea-label">
          {{ label() }}
          @if (required()) {
            <span class="required-marker">*</span>
          }
        </label>
      }

      <div
        class="textarea-container"
        [class.textarea-sm]="size() === 'sm'"
        [class.textarea-lg]="size() === 'lg'"
        [class.has-error]="error()"
        [class.focused]="isFocused()"
      >
        <textarea
          #textareaElement
          [id]="textareaId()"
          [placeholder]="placeholder()"
          [disabled]="disabled()"
          [readonly]="readonly()"
          [rows]="rows()"
          [attr.maxlength]="maxLength()"
          [attr.aria-invalid]="error() ? 'true' : null"
          [attr.aria-describedby]="error() ? textareaId() + '-error' : null"
          class="textarea-field"
          [class.resize-none]="resize() === 'none'"
          [class.resize-vertical]="resize() === 'vertical'"
          [class.resize-horizontal]="resize() === 'horizontal'"
          [class.resize-both]="resize() === 'both'"
          [value]="value()"
          (input)="onInput($event)"
          (blur)="onBlur()"
          (focus)="onFocus()"
        ></textarea>
      </div>

      <div class="textarea-footer">
        @if (error()) {
          <span [id]="textareaId() + '-error'" class="textarea-error">{{ error() }}</span>
        } @else if (hint()) {
          <span class="textarea-hint">{{ hint() }}</span>
        } @else {
          <span></span>
        }

        @if (showCharCount() && maxLength()) {
          <span class="char-count" [class.at-limit]="isAtLimit()">
            {{ charCount() }} / {{ maxLength() }}
          </span>
        }
      </div>
    </div>
  `,
  styles: [`
    .textarea-wrapper {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
      width: 100%;
    }

    .textarea-wrapper.disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .textarea-label {
      font-size: 0.875rem;
      font-weight: 500;
      color: #374151;
    }

    .required-marker {
      color: #ef4444;
      margin-left: 0.125rem;
    }

    .textarea-container {
      position: relative;
      width: 100%;
      background: white;
      border: 1px solid #d1d5db;
      border-radius: 0.375rem;
      transition: border-color 0.15s ease, box-shadow 0.15s ease;
    }

    .textarea-container.focused {
      border-color: #f97316;
      box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.1);
    }

    .textarea-container.has-error {
      border-color: #ef4444;
    }

    .textarea-container.has-error.focused {
      box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1);
    }

    .textarea-field {
      display: block;
      width: 100%;
      padding: 0.625rem 0.75rem;
      font-size: 0.875rem;
      font-family: inherit;
      color: #111827;
      background: transparent;
      border: none;
      outline: none;
      line-height: 1.5;
    }

    .textarea-field::placeholder {
      color: #9ca3af;
    }

    .textarea-field:disabled {
      cursor: not-allowed;
    }

    /* Size variants */
    .textarea-sm .textarea-field {
      padding: 0.375rem 0.625rem;
      font-size: 0.75rem;
    }

    .textarea-lg .textarea-field {
      padding: 0.75rem 1rem;
      font-size: 1rem;
    }

    /* Resize options */
    .resize-none {
      resize: none;
    }

    .resize-vertical {
      resize: vertical;
    }

    .resize-horizontal {
      resize: horizontal;
    }

    .resize-both {
      resize: both;
    }

    /* Footer */
    .textarea-footer {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      gap: 1rem;
      min-height: 1.25rem;
    }

    .textarea-error {
      font-size: 0.75rem;
      color: #ef4444;
    }

    .textarea-hint {
      font-size: 0.75rem;
      color: #6b7280;
    }

    .char-count {
      font-size: 0.75rem;
      color: #6b7280;
      margin-left: auto;
      flex-shrink: 0;
    }

    .char-count.at-limit {
      color: #ef4444;
      font-weight: 500;
    }
  `],
})
export class TextareaComponent implements ControlValueAccessor {
  private static idCounter = 0;

  /** Textarea label */
  readonly label = input<string>();

  /** Textarea placeholder */
  readonly placeholder = input<string>('');

  /** Textarea size */
  readonly size = input<TextareaSize>('md');

  /** Number of visible rows */
  readonly rows = input(4);

  /** Maximum character length */
  readonly maxLength = input<number>();

  /** Show character count */
  readonly showCharCount = input(false);

  /** Resize behavior */
  readonly resize = input<TextareaResize>('vertical');

  /** Error message */
  readonly error = input<string | null>(null);

  /** Hint text */
  readonly hint = input<string>();

  /** Whether textarea is required */
  readonly required = input(false);

  /** Whether textarea is disabled */
  readonly disabled = input(false);

  /** Whether textarea is readonly */
  readonly readonly = input(false);

  /** Blur event */
  readonly textareaBlur = output<void>();

  /** Focus event */
  readonly textareaFocus = output<void>();

  /** Reference to textarea element */
  private readonly textareaElement = viewChild<ElementRef<HTMLTextAreaElement>>('textareaElement');

  /** Unique ID for textarea */
  readonly textareaId = signal(`ocr-textarea-${++TextareaComponent.idCounter}`);

  /** Current value */
  readonly value = signal('');

  /** Focus state */
  readonly isFocused = signal(false);

  /** Computed character count */
  readonly charCount = computed(() => this.value().length);

  /** Computed whether at character limit */
  readonly isAtLimit = computed(() => {
    const max = this.maxLength();
    return max ? this.charCount() >= max : false;
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
    const value = (event.target as HTMLTextAreaElement).value;
    this.value.set(value);
    this.onChange(value);
  }

  onBlur(): void {
    this.isFocused.set(false);
    this.onTouched();
    this.textareaBlur.emit();
  }

  onFocus(): void {
    this.isFocused.set(true);
    this.textareaFocus.emit();
  }

  /** Focus the textarea element programmatically */
  focus(): void {
    this.textareaElement()?.nativeElement.focus();
  }

  /** Blur the textarea element programmatically */
  blur(): void {
    this.textareaElement()?.nativeElement.blur();
  }
}
