import { Component, forwardRef, input } from '@angular/core';
import { type ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

type PageSizeOption = {
  value: number;
  label: string;
};

/**
 * Reusable page size selection component
 * Provides common page size options (10, 25, 50, 100)
 */
@Component({
  selector: 'ui-select-page-size',
  standalone: true,
  template: `
    <select
      [id]="id()"
      [disabled]="disabled"
      [class]="cssClass()"
      (change)="onSelectChange($event)"
      (blur)="onTouched()"
    >
      @for (option of options; track option.value) {
        <option [value]="option.value" [selected]="option.value === value">
          {{ option.label }}
        </option>
      }
    </select>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectPageSizeComponent),
      multi: true,
    },
  ],
})
export class SelectPageSizeComponent implements ControlValueAccessor {
  readonly id = input<string>('pageSize');
  readonly cssClass = input<string>('form-input');

  value: number = 10;
  disabled = false;

  readonly options: PageSizeOption[] = [
    { value: 10, label: '10' },
    { value: 25, label: '25' },
    { value: 50, label: '50' },
    { value: 100, label: '100' },
  ];

  private onChange: (value: number) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: number): void {
    this.value = value ?? 10;
  }

  registerOnChange(fn: (value: number) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onSelectChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.value = parseInt(target.value, 10);
    this.onChange(this.value);
  }
}
