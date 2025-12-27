import {
  Component,
  forwardRef,
  input,
  signal,
  computed,
  effect,
  output,
} from '@angular/core';
import {
  type ControlValueAccessor,
  NG_VALUE_ACCESSOR,
  FormsModule,
} from '@angular/forms';

export interface DateRange {
  startDate: string;
  endDate: string;
}

@Component({
  selector: 'ui-date-range-picker',
  standalone: true,
  imports: [FormsModule],
  template: `
    <div class="date-range-container" [class]="containerClass()">
      <div class="date-field">
        @if (showLabels()) {
          <label [for]="startDateId()">{{ startDateLabel() }}</label>
        }
        <input
          type="date"
          [id]="startDateId()"
          [class]="inputClass()"
          [min]="minStartDate()"
          [max]="maxStartDate() || undefined"
          [disabled]="disabled"
          [value]="startDate()"
          (change)="onStartDateChange($event)"
          (blur)="onTouched()"
          [attr.required]="required() || null"
        />
      </div>

      <div class="date-field">
        @if (showLabels()) {
          <label [for]="endDateId()">{{ endDateLabel() }}</label>
        }
        <input
          type="date"
          [id]="endDateId()"
          [class]="inputClass()"
          [min]="minEndDate()"
          [max]="maxEndDate() || undefined"
          [disabled]="disabled"
          [value]="endDate()"
          (change)="onEndDateChange($event)"
          (blur)="onTouched()"
          [attr.required]="required() || null"
        />
      </div>
    </div>
  `,
  styles: [
    `
      .date-range-container {
        display: flex;
        gap: 1rem;
        flex-wrap: wrap;
      }

      .date-field {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
        flex: 1;
        min-width: 150px;
      }

      .date-field label {
        font-size: 0.875rem;
        font-weight: 500;
        color: #374151;
      }
    `,
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DateRangePickerComponent),
      multi: true,
    },
  ],
})
export class DateRangePickerComponent implements ControlValueAccessor {
  readonly startDateId = input<string>('startDate');
  readonly endDateId = input<string>('endDate');
  readonly startDateLabel = input<string>('Startdatum');
  readonly endDateLabel = input<string>('Enddatum');
  readonly showLabels = input<boolean>(true);
  readonly inputClass = input<string>('form-input date-input');
  readonly containerClass = input<string>('');
  readonly required = input<boolean>(false);
  readonly minStartDate = input<string>(this.getToday());
  readonly maxStartDate = input<string | null>(null);
  readonly maxEndDate = input<string | null>(null);
  readonly minDaysApart = input<number>(0);

  readonly dateRangeChange = output<DateRange>();

  readonly startDate = signal<string>('');
  readonly endDate = signal<string>('');

  readonly minEndDate = computed(() => {
    const start = this.startDate();
    if (!start) return this.minStartDate();

    const minDays = this.minDaysApart();
    if (minDays <= 0) return start;

    const date = new Date(start);
    date.setDate(date.getDate() + minDays);
    return this.formatDate(date);
  });

  disabled = false;

  private onChange: (value: DateRange) => void = () => {};
  onTouched: () => void = () => {};

  constructor() {
    effect(() => {
      const start = this.startDate();
      const end = this.endDate();
      const minEnd = this.minEndDate();

      if (end && start && end < minEnd) {
        this.endDate.set(minEnd);
        this.emitChange();
      }
    });
  }

  writeValue(value: DateRange | null): void {
    if (value) {
      this.startDate.set(value.startDate ?? '');
      this.endDate.set(value.endDate ?? '');
    } else {
      this.startDate.set('');
      this.endDate.set('');
    }
  }

  registerOnChange(fn: (value: DateRange) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onStartDateChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.startDate.set(target.value);
    this.emitChange();
  }

  onEndDateChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.endDate.set(target.value);
    this.emitChange();
  }

  private emitChange(): void {
    const range: DateRange = {
      startDate: this.startDate(),
      endDate: this.endDate(),
    };
    this.onChange(range);
    this.dateRangeChange.emit(range);
  }

  private getToday(): string {
    return this.formatDate(new Date());
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}
