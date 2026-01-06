import { Component, forwardRef, input } from "@angular/core";
import { type ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";
import {
  VehicleStatus,
  VehicleStatusLabel,
} from "@orange-car-rental/vehicle-api";

type SelectOption<T> = {
  value: T;
  label: string;
};

@Component({
  selector: "ui-select-vehicle-status",
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
      useExisting: forwardRef(() => SelectVehicleStatusComponent),
      multi: true,
    },
  ],
})
export class SelectVehicleStatusComponent implements ControlValueAccessor {
  readonly id = input<string>("vehicleStatus");
  readonly placeholder = input<string>("Alle Status");
  readonly cssClass = input<string>("form-input");

  value: string = "";
  disabled = false;

  readonly options: SelectOption<string>[] = Object.entries(VehicleStatus).map(
    ([, enumValue]) => ({
      value: enumValue,
      label: VehicleStatusLabel[enumValue as VehicleStatus],
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
