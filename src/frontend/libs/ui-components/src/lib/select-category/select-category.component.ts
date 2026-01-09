import { Component, forwardRef, input } from "@angular/core";
import { NG_VALUE_ACCESSOR } from "@angular/forms";
import {
  VehicleCategory,
  VehicleCategoryLabel,
} from "@orange-car-rental/vehicle-api";
import { BaseSelectComponent, type SelectOption } from "../select";

@Component({
  selector: "ui-select-category",
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
export class SelectCategoryComponent extends BaseSelectComponent<string> {
  readonly id = input<string>("category");
  readonly placeholder = input<string>("Alle Kategorien");
  readonly cssClass = input<string>("form-input");

  override value = "";

  readonly options: SelectOption<string>[] = Object.entries(
    VehicleCategory,
  ).map(([, enumValue]) => ({
    value: enumValue,
    label: VehicleCategoryLabel[enumValue as VehicleCategory],
  }));

  protected parseValue(rawValue: string): string {
    return rawValue;
  }

  protected getDefaultValue(): string {
    return "";
  }
}
