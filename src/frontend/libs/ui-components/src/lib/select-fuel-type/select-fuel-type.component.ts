import { Component, forwardRef, input } from "@angular/core";
import { NG_VALUE_ACCESSOR } from "@angular/forms";
import { FuelType, FuelTypeLabel } from "@orange-car-rental/vehicle-api";
import { BaseSelectComponent, type SelectOption } from "../select";

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
export class SelectFuelTypeComponent extends BaseSelectComponent<string> {
  readonly id = input<string>("fuelType");
  readonly placeholder = input<string>("Alle Kraftstoffe");
  readonly cssClass = input<string>("form-input");

  override value = "";

  readonly options: SelectOption<string>[] = Object.entries(FuelType).map(
    ([, enumValue]) => ({
      value: enumValue,
      label: FuelTypeLabel[enumValue as FuelType],
    }),
  );

  protected parseValue(rawValue: string): string {
    return rawValue;
  }

  protected getDefaultValue(): string {
    return "";
  }
}
