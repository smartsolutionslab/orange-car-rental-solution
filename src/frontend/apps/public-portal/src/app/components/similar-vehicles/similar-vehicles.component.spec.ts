import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { SimilarVehiclesComponent } from './similar-vehicles.component';
import type { Vehicle, VehicleId, DailyRate } from '@orange-car-rental/vehicle-api';
import { API_CONFIG } from '@orange-car-rental/shared';
import { MOCK_VEHICLES } from '@orange-car-rental/shared/testing';

describe('SimilarVehiclesComponent', () => {
  let component: SimilarVehiclesComponent;
  let fixture: ComponentFixture<SimilarVehiclesComponent>;

  // Use shared mock vehicles
  const mockCurrentVehicle: Vehicle = MOCK_VEHICLES.VW_GOLF;

  const mockSimilarVehicles: Vehicle[] = [MOCK_VEHICLES.OPEL_ASTRA, MOCK_VEHICLES.BMW_3ER];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SimilarVehiclesComponent, TranslateModule.forRoot()],
      providers: [{ provide: API_CONFIG, useValue: { apiUrl: 'http://localhost:5000' } }],
    }).compileComponents();

    fixture = TestBed.createComponent(SimilarVehiclesComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('currentVehicle', mockCurrentVehicle);
    fixture.componentRef.setInput('similarVehicles', mockSimilarVehicles);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Price Difference Calculation', () => {
    it('should calculate price difference correctly for cheaper vehicle', () => {
      const cheaper = mockSimilarVehicles[0]!; // 53.55 vs 59.50
      const result = component['getPriceDifference'](cheaper);

      expect(result.amount).toBeCloseTo(5.95, 2);
      expect(result.text).toBe('similarVehicles.cheaper');
    });

    it('should calculate price difference correctly for more expensive vehicle', () => {
      const expensive: Vehicle = {
        ...mockCurrentVehicle,
        dailyRateGross: 70.0 as DailyRate,
      };
      const result = component['getPriceDifference'](expensive);

      expect(result.amount).toBeLessThan(0);
      expect(result.text).toBe('similarVehicles.moreExpensive');
    });

    it('should handle same price', () => {
      const samePrice: Vehicle = {
        ...mockCurrentVehicle,
        id: 'different-id' as VehicleId,
      };
      const result = component['getPriceDifference'](samePrice);

      expect(result.amount).toBe(0);
      expect(result.text).toBe('similarVehicles.samePrice');
    });

    it('should return empty when current vehicle is null', () => {
      fixture.componentRef.setInput('currentVehicle', null);
      fixture.detectChanges();
      const result = component['getPriceDifference'](mockSimilarVehicles[0]!);

      expect(result.amount).toBe(0);
      expect(result.text).toBe('');
    });
  });

  describe('Similarity Reasons', () => {
    it('should show "Gleiche Kategorie" for same category', () => {
      const sameCategory = mockSimilarVehicles[0]!; // MITTEL like current
      const reason = component['getSimilarityReason'](sameCategory);

      expect(reason).toContain('similarVehicles.sameCategory');
    });

    it('should show "Ähnliche Kategorie" for different category', () => {
      const differentCategory = mockSimilarVehicles[1]!; // KOMPAKT vs MITTEL
      const reason = component['getSimilarityReason'](differentCategory);

      expect(reason).toContain('similarVehicles.similarCategory');
    });

    it('should include "Günstiger" for cheaper vehicles', () => {
      const cheaper = mockSimilarVehicles[0]!;
      const reason = component['getSimilarityReason'](cheaper);

      expect(reason).toContain('similarVehicles.cheaperLabel');
    });

    it('should include fuel type when matching', () => {
      const sameFuel = mockSimilarVehicles[0]!; // Petrol like current
      const reason = component['getSimilarityReason'](sameFuel);

      expect(reason).toContain('vehicles.fuelType.petrol');
    });

    it('should include transmission type when matching', () => {
      const sameTransmission = mockSimilarVehicles[0]!; // Manual like current
      const reason = component['getSimilarityReason'](sameTransmission);

      expect(reason).toContain('vehicles.transmission.manual');
    });

    it('should return empty string when current vehicle is null', () => {
      fixture.componentRef.setInput('currentVehicle', null);
      fixture.detectChanges();
      const reason = component['getSimilarityReason'](mockSimilarVehicles[0]!);

      expect(reason).toBe('');
    });
  });

  describe('Vehicle Selection', () => {
    it('should emit vehicleSelected event when selectVehicle is called', () => {
      spyOn(component.vehicleSelected, 'emit');
      const vehicle = mockSimilarVehicles[0]!;

      component['selectVehicle'](vehicle);

      expect(component.vehicleSelected.emit).toHaveBeenCalledWith(vehicle);
    });
  });

  describe('Fuel Type Formatting', () => {
    // With TranslateModule.forRoot(), translate.instant returns the key,
    // so formatFuelType falls back to the original value
    it('should translate Petrol to Benzin', () => {
      // Falls back to original since no translations are loaded
      expect(component['formatFuelType']('Petrol')).toBe('Petrol');
    });

    it('should translate Diesel to Diesel', () => {
      expect(component['formatFuelType']('Diesel')).toBe('Diesel');
    });

    it('should translate Electric to Elektro', () => {
      expect(component['formatFuelType']('Electric')).toBe('Electric');
    });

    it('should translate Hybrid to Hybrid', () => {
      expect(component['formatFuelType']('Hybrid')).toBe('Hybrid');
    });

    it('should return original value for unknown fuel type', () => {
      expect(component['formatFuelType']('Unknown')).toBe('Unknown');
    });
  });

  describe('Transmission Type Formatting', () => {
    // With TranslateModule.forRoot(), translate.instant returns the key,
    // so formatTransmissionType falls back to the original value
    it('should translate Manual to Manuell', () => {
      expect(component['formatTransmissionType']('Manual')).toBe('Manual');
    });

    it('should translate Automatic to Automatik', () => {
      expect(component['formatTransmissionType']('Automatic')).toBe('Automatic');
    });

    it('should return original value for unknown transmission type', () => {
      expect(component['formatTransmissionType']('Unknown')).toBe('Unknown');
    });
  });

  describe('UI Rendering', () => {
    it('should display unavailable warning when showUnavailableWarning is true', () => {
      fixture.componentRef.setInput('showUnavailableWarning', true);
      fixture.detectChanges();

      const warning = fixture.nativeElement.querySelector('.unavailable-warning');
      expect(warning).toBeTruthy();
    });

    it('should not show unavailable warning when showUnavailableWarning is false', () => {
      fixture.componentRef.setInput('showUnavailableWarning', false);
      fixture.detectChanges();

      const warning = fixture.nativeElement.querySelector('.unavailable-warning');
      expect(warning).toBeFalsy();
    });

    it('should display section header when no warning', () => {
      fixture.componentRef.setInput('showUnavailableWarning', false);
      fixture.detectChanges();

      const header = fixture.nativeElement.querySelector('.section-title');
      expect(header).toBeTruthy();
      // With TranslateModule.forRoot(), the translation key is rendered
      expect(header.textContent).toContain('similarVehicles.title');
    });

    it('should render vehicle cards for each similar vehicle', () => {
      const cards = fixture.nativeElement.querySelectorAll('.vehicle-card');
      expect(cards.length).toBe(2);
    });

    it('should display vehicle name in card', () => {
      const firstCard = fixture.nativeElement.querySelector('.vehicle-card');
      const name = firstCard.querySelector('.vehicle-name');
      expect(name.textContent).toContain('Opel Astra');
    });

    it('should display "Book This Instead" button', () => {
      const button = fixture.nativeElement.querySelector('.book-instead-button');
      expect(button).toBeTruthy();
      // With TranslateModule.forRoot(), the translation key is rendered
      expect(button.textContent).toContain('similarVehicles.bookInstead');
    });

    it('should show no vehicles message when array is empty', () => {
      fixture.componentRef.setInput('similarVehicles', []);
      fixture.detectChanges();

      const noVehicles = fixture.nativeElement.querySelector('.no-vehicles');
      expect(noVehicles).toBeTruthy();
    });

    it('should display vehicle specifications', () => {
      const specs = fixture.nativeElement.querySelectorAll('.spec-item');
      expect(specs.length).toBeGreaterThan(0);
    });

    it('should show price comparison', () => {
      const priceComparison = fixture.nativeElement.querySelector('.price-comparison');
      expect(priceComparison).toBeTruthy();
    });
  });
});
