import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { VehicleSearchComponent } from './vehicle-search.component';
import { getFutureDate } from '@orange-car-rental/shared/testing';
import type { VehicleSearchQuery } from '@orange-car-rental/vehicle-api';

describe('VehicleSearchComponent', () => {
  let component: VehicleSearchComponent;
  let fixture: ComponentFixture<VehicleSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VehicleSearchComponent, ReactiveFormsModule, TranslateModule.forRoot()],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(VehicleSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Form Initialization', () => {
    it('should initialize form with all controls', () => {
      expect(component['searchForm'].get('pickupDate')).toBeDefined();
      expect(component['searchForm'].get('returnDate')).toBeDefined();
      expect(component['searchForm'].get('locationCode')).toBeDefined();
      expect(component['searchForm'].get('categoryCode')).toBeDefined();
      expect(component['searchForm'].get('fuelType')).toBeDefined();
      expect(component['searchForm'].get('transmissionType')).toBeDefined();
      expect(component['searchForm'].get('minSeats')).toBeDefined();
    });

    it('should set default pickup date to tomorrow', () => {
      const pickupDate = component['searchForm'].get('pickupDate')?.value;
      const tomorrow = getFutureDate(1);
      expect(pickupDate).toBe(tomorrow);
    });

    it('should set default return date to 4 days from now', () => {
      const returnDate = component['searchForm'].get('returnDate')?.value;
      const fourDaysFromNow = getFutureDate(4);
      expect(returnDate).toBe(fourDaysFromNow);
    });

    it('should have today as minimum date', () => {
      const today = new Date();
      const expectedToday = today.toISOString().split('T')[0];
      expect(component['today']).toBe(expectedToday);
    });
  });

  describe('Date Validation', () => {
    it('should update minReturnDate when pickup date changes', fakeAsync(() => {
      const newPickupDate = getFutureDate(5);
      component['searchForm'].patchValue({ pickupDate: newPickupDate });
      tick();

      const expectedMinReturn = getFutureDate(6);
      expect(component['minReturnDate']()).toBe(expectedMinReturn);
    }));

    it('should adjust return date if it is before or equal to pickup date', fakeAsync(() => {
      const pickupDate = getFutureDate(10);
      const returnDate = getFutureDate(8); // Before pickup

      component['searchForm'].patchValue({ returnDate });
      tick();

      component['searchForm'].patchValue({ pickupDate });
      tick();

      const actualReturnDate = component['searchForm'].get('returnDate')?.value;
      const expectedMinReturn = getFutureDate(11);
      expect(actualReturnDate).toBe(expectedMinReturn);
    }));

    it('should not adjust return date if it is after pickup date', fakeAsync(() => {
      const pickupDate = getFutureDate(5);
      const returnDate = getFutureDate(10);

      component['searchForm'].patchValue({ pickupDate, returnDate });
      tick();

      const actualReturnDate = component['searchForm'].get('returnDate')?.value;
      expect(actualReturnDate).toBe(returnDate);
    }));
  });

  describe('Search Submission', () => {
    it('should emit searchSubmit with query on valid form', () => {
      const emitSpy = spyOn(component.searchSubmit, 'emit');

      component['searchForm'].patchValue({
        pickupDate: getFutureDate(1),
        returnDate: getFutureDate(4),
        locationCode: 'MUC-FLG',
        categoryCode: 'KOMPAKT',
        fuelType: 'Petrol',
        transmissionType: 'Automatic',
        minSeats: 4,
      });

      component['onSearch']();

      expect(emitSpy).toHaveBeenCalledWith(
        jasmine.objectContaining({
          pickupDate: getFutureDate(1),
          returnDate: getFutureDate(4),
          locationCode: 'MUC-FLG',
          categoryCode: 'KOMPAKT',
          fuelType: 'Petrol',
          transmissionType: 'Automatic',
          minSeats: 4,
        }),
      );
    });

    it('should not include empty optional fields in query', () => {
      const emitSpy = spyOn(component.searchSubmit, 'emit');

      component['searchForm'].patchValue({
        pickupDate: getFutureDate(1),
        returnDate: getFutureDate(4),
        locationCode: '',
        categoryCode: '',
        fuelType: '',
        transmissionType: '',
        minSeats: null,
      });

      component['onSearch']();

      const emittedQuery = emitSpy.calls.mostRecent().args[0] as VehicleSearchQuery;
      expect(emittedQuery.locationCode).toBeUndefined();
      expect(emittedQuery.categoryCode).toBeUndefined();
      expect(emittedQuery.fuelType).toBeUndefined();
      expect(emittedQuery.transmissionType).toBeUndefined();
      expect(emittedQuery.minSeats).toBeUndefined();
    });

    it('should not emit if form is invalid', () => {
      const emitSpy = spyOn(component.searchSubmit, 'emit');

      // Mark form as invalid by setting a validator that fails
      component['searchForm'].setErrors({ invalid: true });

      component['onSearch']();

      expect(emitSpy).not.toHaveBeenCalled();
    });
  });

  describe('Form Reset', () => {
    it('should reset form to default values', () => {
      // First set some values
      component['searchForm'].patchValue({
        locationCode: 'BER-HBF',
        categoryCode: 'SUV',
        fuelType: 'Electric',
        transmissionType: 'Automatic',
        minSeats: 5,
      });

      // Reset the form
      component['onReset']();

      // Check that filter fields are cleared
      expect(component['searchForm'].get('locationCode')?.value).toBe('');
      expect(component['searchForm'].get('categoryCode')?.value).toBe('');
      expect(component['searchForm'].get('fuelType')?.value).toBe('');
      expect(component['searchForm'].get('transmissionType')?.value).toBe('');
      expect(component['searchForm'].get('minSeats')?.value).toBeNull();
    });

    it('should reinitialize default dates after reset', () => {
      component['onReset']();

      const pickupDate = component['searchForm'].get('pickupDate')?.value;
      const returnDate = component['searchForm'].get('returnDate')?.value;

      expect(pickupDate).toBe(getFutureDate(1));
      expect(returnDate).toBe(getFutureDate(4));
    });
  });

  describe('UI Elements', () => {
    it('should render search form', () => {
      const form = fixture.nativeElement.querySelector('form');
      expect(form).toBeTruthy();
    });

    it('should render pickup date input', () => {
      const input = fixture.nativeElement.querySelector('ocr-input[formControlName="pickupDate"]');
      expect(input).toBeTruthy();
    });

    it('should render return date input', () => {
      const input = fixture.nativeElement.querySelector('ocr-input[formControlName="returnDate"]');
      expect(input).toBeTruthy();
    });

    it('should render search button', () => {
      const button = fixture.nativeElement.querySelector('button[type="submit"]');
      expect(button).toBeTruthy();
    });

    it('should render reset button', () => {
      const resetButton = fixture.nativeElement.querySelector('button.btn-secondary');
      expect(resetButton).toBeTruthy();
    });
  });
});
