import { Component, forwardRef, input } from "@angular/core";
import { type ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";

type SelectOption = {
  value: number | null;
  label: string;
};

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
export class SelectSeatsComponent implements ControlValueAccessor {
  readonly id = input<string>("minSeats");
  readonly placeholder = input<string>("Alle");
  readonly cssClass = input<string>("form-input");

  value: number | null = null;
  disabled = false;

  readonly options: SelectOption[] = [
    { value: 2, label: "2+ Sitze" },
    { value: 4, label: "4+ Sitze" },
    { value: 5, label: "5+ Sitze" },
    { value: 7, label: "7+ Sitze" },
  ];

  private onChange: (value: number | null) => void = () => {};
  onTouched: () => void = () => {};

  writeValue(value: number | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: number | null) => void): void {
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
    const val = target.value;
    this.value = val === "null" || val === "" ? null : parseInt(val, 10);
    this.onChange(this.value);
  }
}
