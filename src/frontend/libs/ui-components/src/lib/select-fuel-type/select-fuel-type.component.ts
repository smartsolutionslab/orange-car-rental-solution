import { Component, forwardRef, input } from "@angular/core";
import { type ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";
import { FuelType, FuelTypeLabel } from "@orange-car-rental/vehicle-api";

type SelectOption<T> = {
  value: T;
  label: string;
};

@Component({
  selector: "ui-select-fuel-type",
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
      useExisting: forwardRef(() => SelectFuelTypeComponent),
      multi: true,
    },
  ],
})
export class SelectFuelTypeComponent implements ControlValueAccessor {
  readonly id = input<string>("fuelType");
  readonly placeholder = input<string>("Alle Kraftstoffe");
  readonly cssClass = input<string>("form-input");

  value: string = "";
  disabled = false;

  readonly options: SelectOption<string>[] = Object.entries(FuelType).map(
    ([, enumValue]) => ({
      value: enumValue,
      label: FuelTypeLabel[enumValue as FuelType],
    }),
  );

  private onChange: (value: string) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: string): void {
    this.value = value ?? "";
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
