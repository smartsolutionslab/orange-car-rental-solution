import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SimilarVehiclesComponent } from './similar-vehicles.component';
import type { Vehicle } from '@orange-car-rental/vehicle-api';
import { API_CONFIG } from '@orange-car-rental/shared';

describe('SimilarVehiclesComponent', () => {
  let component: SimilarVehiclesComponent;
  let fixture: ComponentFixture<SimilarVehiclesComponent>;

  const mockCurrentVehicle: Vehicle = {
    id: '123e4567-e89b-12d3-a456-426614174000',
    name: 'VW Golf',
    categoryCode: 'MITTEL',
    categoryName: 'Mittelklasse',
    locationCode: 'BER-HBF',
    city: 'Berlin',
    dailyRateNet: 50.00,
    dailyRateVat: 9.50,
    dailyRateGross: 59.50,
    currency: 'EUR',
    seats: 5,
    fuelType: 'Petrol',
    transmissionType: 'Manual',
    status: 'Available',
    licensePlate: 'B-AB 1234',
    manufacturer: 'Volkswagen',
    model: 'Golf 8',
    year: 2023,
    imageUrl: null
  };

  const mockSimilarVehicles: Vehicle[] = [
    {
      id: '987e6543-e89b-12d3-a456-426614174001',
      name: 'Opel Astra',
      categoryCode: 'MITTEL',
      categoryName: 'Mittelklasse',
      locationCode: 'BER-HBF',
      city: 'Berlin',
      dailyRateNet: 45.00,
      dailyRateVat: 8.55,
      dailyRateGross: 53.55,
      currency: 'EUR',
      seats: 5,
      fuelType: 'Petrol',
      transmissionType: 'Manual',
      status: 'Available',
      licensePlate: 'B-CD 5678',
      manufacturer: 'Opel',
      model: 'Astra',
      year: 2023,
      imageUrl: null
    },
    {
      id: '111e2222-e89b-12d3-a456-426614174002',
      name: 'Ford Focus',
      categoryCode: 'KOMPAKT',
      categoryName: 'Kompaktklasse',
      locationCode: 'BER-HBF',
      city: 'Berlin',
      dailyRateNet: 40.00,
      dailyRateVat: 7.60,
      dailyRateGross: 47.60,
      currency: 'EUR',
      seats: 5,
      fuelType: 'Diesel',
      transmissionType: 'Manual',
      status: 'Available',
      licensePlate: 'B-EF 9012',
      manufacturer: 'Ford',
      model: 'Focus',
      year: 2022,
      imageUrl: null
    }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SimilarVehiclesComponent],
      providers: [
        { provide: API_CONFIG, useValue: { apiUrl: 'http://localhost:5000' } }
      ]
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
      expect(result.text).toContain('günstiger');
    });

    it('should calculate price difference correctly for more expensive vehicle', () => {
      const expensive: Vehicle = {
        ...mockCurrentVehicle,
        dailyRateGross: 70.00
      };
      const result = component['getPriceDifference'](expensive);

      expect(result.amount).toBeLessThan(0);
      expect(result.text).toContain('teurer');
    });

    it('should handle same price', () => {
      const samePrice: Vehicle = {
        ...mockCurrentVehicle,
        id: 'different-id'
      };
      const result = component['getPriceDifference'](samePrice);

      expect(result.amount).toBe(0);
      expect(result.text).toBe('Gleicher Preis');
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

      expect(reason).toContain('Gleiche Kategorie');
    });

    it('should show "Ähnliche Kategorie" for different category', () => {
      const differentCategory = mockSimilarVehicles[1]!; // KOMPAKT vs MITTEL
      const reason = component['getSimilarityReason'](differentCategory);

      expect(reason).toContain('Ähnliche Kategorie');
    });

    it('should include "Günstiger" for cheaper vehicles', () => {
      const cheaper = mockSimilarVehicles[0]!;
      const reason = component['getSimilarityReason'](cheaper);

      expect(reason).toContain('Günstiger');
    });

    it('should include fuel type when matching', () => {
      const sameFuel = mockSimilarVehicles[0]!; // Petrol like current
      const reason = component['getSimilarityReason'](sameFuel);

      expect(reason).toContain('Petrol');
    });

    it('should include transmission type when matching', () => {
      const sameTransmission = mockSimilarVehicles[0]!; // Manual like current
      const reason = component['getSimilarityReason'](sameTransmission);

      expect(reason).toContain('Manual');
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
    it('should translate Petrol to Benzin', () => {
      expect(component['formatFuelType']('Petrol')).toBe('Benzin');
    });

    it('should translate Diesel to Diesel', () => {
      expect(component['formatFuelType']('Diesel')).toBe('Diesel');
    });

    it('should translate Electric to Elektro', () => {
      expect(component['formatFuelType']('Electric')).toBe('Elektro');
    });

    it('should translate Hybrid to Hybrid', () => {
      expect(component['formatFuelType']('Hybrid')).toBe('Hybrid');
    });

    it('should return original value for unknown fuel type', () => {
      expect(component['formatFuelType']('Unknown')).toBe('Unknown');
    });
  });

  describe('Transmission Type Formatting', () => {
    it('should translate Manual to Manuell', () => {
      expect(component['formatTransmissionType']('Manual')).toBe('Manuell');
    });

    it('should translate Automatic to Automatik', () => {
      expect(component['formatTransmissionType']('Automatic')).toBe('Automatik');
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
      expect(header.textContent).toContain('Ähnliche Fahrzeuge');
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
      expect(button.textContent).toContain('Stattdessen buchen');
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
