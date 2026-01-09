import { Component, forwardRef, input } from "@angular/core";
import { NG_VALUE_ACCESSOR } from "@angular/forms";
import { BaseSelectComponent, type SelectOption } from "../select";

/**
 * Reusable page size selection component
 * Provides common page size options (10, 25, 50, 100)
 */
@Component({
  selector: "ui-select-page-size",
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
export class SelectPageSizeComponent extends BaseSelectComponent<number> {
  readonly id = input<string>("pageSize");
  readonly cssClass = input<string>("form-input");

  override value = 10;

  readonly options: SelectOption<number>[] = [
    { value: 10, label: "10" },
    { value: 25, label: "25" },
    { value: 50, label: "50" },
    { value: 100, label: "100" },
  ];

  protected parseValue(rawValue: string): number {
    return parseInt(rawValue, 10);
  }

  protected getDefaultValue(): number {
    return 10;
  }
}
