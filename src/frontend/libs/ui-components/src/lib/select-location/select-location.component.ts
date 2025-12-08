import {
  Component,
  forwardRef,
  input,
  signal,
  inject,
  type OnInit,
  DestroyRef,
  output,
} from '@angular/core';
import { type ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LocationService, type Location } from '@orange-car-rental/location-api';

@Component({
  selector: 'ui-select-location',
  standalone: true,
  template: `
    <select
      [id]="id()"
      [disabled]="disabled || loading()"
      [class]="cssClass()"
      (change)="onSelectChange($event)"
      (blur)="onTouched()"
    >
      <option value="">
        @if (loading()) {
          {{ loadingText() }}
        } @else if (error()) {
          {{ errorText() }}
        } @else {
          {{ placeholder() }}
        }
      </option>
      @for (location of locations(); track location.code) {
        <option [value]="location.code" [selected]="location.code === value">
          @if (showCity()) {
            {{ location.city }} - {{ location.name }}
          } @else {
            {{ location.name }}
          }
        </option>
      }
    </select>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectLocationComponent),
      multi: true,
    },
  ],
})
export class SelectLocationComponent implements ControlValueAccessor, OnInit {
  private readonly locationService = inject(LocationService);
  private readonly destroyRef = inject(DestroyRef);

  readonly id = input<string>('location');
  readonly placeholder = input<string>('Alle Standorte');
  readonly loadingText = input<string>('Lade Standorte...');
  readonly errorText = input<string>('Fehler beim Laden');
  readonly cssClass = input<string>('form-input');
  readonly showCity = input<boolean>(false);

  readonly locationLoaded = output<Location[]>();

  readonly locations = signal<Location[]>([]);
  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  value: string = '';
  disabled = false;

  private onChange: (value: string) => void = () => {};
  onTouched: () => void = () => {};

  ngOnInit(): void {
    this.loadLocations();
  }

  writeValue(value: string): void {
    this.value = value ?? '';
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

  private loadLocations(): void {
    this.loading.set(true);
    this.error.set(null);

    this.locationService
      .getAllLocations()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (locations) => {
          this.locations.set(locations);
          this.loading.set(false);
          this.locationLoaded.emit(locations);
        },
        error: (err) => {
          console.error('[SelectLocationComponent] Error loading locations:', err);
          this.error.set('Failed to load locations');
          this.loading.set(false);
        },
      });
  }

  refresh(): void {
    this.loadLocations();
  }
}
