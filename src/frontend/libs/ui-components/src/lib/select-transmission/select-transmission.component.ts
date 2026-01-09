import { Component, forwardRef, input } from "@angular/core";
import { NG_VALUE_ACCESSOR } from "@angular/forms";
import {
  TransmissionType,
  TransmissionTypeLabel,
} from "@orange-car-rental/vehicle-api";
import { BaseSelectComponent, type SelectOption } from "../select";

@Component({
  selector: "ui-select-transmission",
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
      useExisting: forwardRef(() => SelectTransmissionComponent),
      multi: true,
    },
  ],
})
export class SelectTransmissionComponent extends BaseSelectComponent<string> {
  readonly id = input<string>("transmission");
  readonly placeholder = input<string>("Alle Getriebe");
  readonly cssClass = input<string>("form-input");

  override value = "";

  readonly options: SelectOption<string>[] = Object.entries(
    TransmissionType,
  ).map(([, enumValue]) => ({
    value: enumValue,
    label: TransmissionTypeLabel[enumValue as TransmissionType],
  }));

  protected parseValue(rawValue: string): string {
    return rawValue;
  }

  protected getDefaultValue(): string {
    return "";
  }
}
