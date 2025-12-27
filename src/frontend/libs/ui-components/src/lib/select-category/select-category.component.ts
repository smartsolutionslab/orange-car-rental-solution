import { Component, forwardRef, input } from '@angular/core';
import { type ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import {
  VehicleCategory,
  VehicleCategoryLabel,
} from '@orange-car-rental/vehicle-api';

export type SelectOption<T> = {
  value: T;
  label: string;
};

@Component({
  selector: 'ui-select-category',
  standalone: true,
  template: `
    <select
      [id]="id()"
      [disabled]="disabled"
      [class]="cssClass()"
      (change)="onSelectChange($event)"
      (blur)="onTouched()"
    >
      <option value="">{{ placeholder() }}</option>
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
      useExisting: forwardRef(() => SelectCategoryComponent),
      multi: true,
    },
  ],
})
export class SelectCategoryComponent implements ControlValueAccessor {
  readonly id = input<string>('category');
  readonly placeholder = input<string>('Alle Kategorien');
  readonly cssClass = input<string>('form-input');

  value: string = '';
  disabled = false;

  readonly options: SelectOption<string>[] = Object.entries(VehicleCategory).map(
    ([, enumValue]) => ({
      value: enumValue,
      label: VehicleCategoryLabel[enumValue as VehicleCategory],
    })
  );

  private onChange: (value: string) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: string): void {
    this.value = value ?? '';
  }

  registerOnChange(fn: (value: string) => void): void {
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
    this.value = target.value;
    this.onChange(this.value);
  }
}
