import { Component, forwardRef, input } from "@angular/core";
import { NG_VALUE_ACCESSOR } from "@angular/forms";
import { BaseSelectComponent, type SelectOption } from "../select";

/**
 * Reusable seats selection component
 * Provides min seats filter options (2+, 4+, 5+, 7+)
 */
@Component({
  selector: "ui-select-seats",
  standalone: true,
  template: `
    <select
      [id]="id()"
      [disabled]="disabled"
      [class]="cssClass()"
      (change)="onSelectChange($event)"
      (blur)="onTouched()"
    >
      <option [value]="null">{{ placeholder() }}</option>
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
      useExisting: forwardRef(() => SelectSeatsComponent),
      multi: true,
    },
  ],
})
export class SelectSeatsComponent extends BaseSelectComponent<number | null> {
  readonly id = input<string>("minSeats");
  readonly placeholder = input<string>("Alle");
  readonly cssClass = input<string>("form-input");

  override value: number | null = null;

  readonly options: SelectOption<number | null>[] = [
    { value: 2, label: "2+ Sitze" },
    { value: 4, label: "4+ Sitze" },
    { value: 5, label: "5+ Sitze" },
    { value: 7, label: "7+ Sitze" },
  ];

  protected parseValue(rawValue: string): number | null {
    return rawValue === "null" || rawValue === "" ? null : parseInt(rawValue, 10);
  }

  protected getDefaultValue(): number | null {
    return null;
  }
}
