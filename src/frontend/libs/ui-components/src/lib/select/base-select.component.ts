import type { ControlValueAccessor } from "@angular/forms";

/**
 * Abstract base class for specialized select components
 * Provides common ControlValueAccessor implementation to reduce boilerplate
 *
 * @example
 * export class SelectCategoryComponent extends BaseSelectComponent<string> {
 *   parseValue(rawValue: string): string {
 *     return rawValue;
 *   }
 * }
 */
export abstract class BaseSelectComponent<T> implements ControlValueAccessor {
  value!: T;
  disabled = false;

  protected onChange: (value: T) => void = () => {};
  onTouched: () => void = () => {};

  /**
   * Parse the raw string value from the select element to the component's value type
   * Override this method to handle type conversion (e.g., string to number)
   */
  protected abstract parseValue(rawValue: string): T;

  /**
   * Get the default value when null/undefined is written
   * Override this to provide a custom default
   */
  protected abstract getDefaultValue(): T;

  writeValue(value: T): void {
    this.value = value ?? this.getDefaultValue();
  }

  registerOnChange(fn: (value: T) => void): void {
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
    this.value = this.parseValue(target.value);
    this.onChange(this.value);
  }
}
