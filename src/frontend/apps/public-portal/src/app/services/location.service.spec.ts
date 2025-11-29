import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LocationService } from './location.service';
import { ConfigService } from './config.service';
import type { Location } from '@orange-car-rental/data-access';

describe('LocationService', () => {
  let service: LocationService;
  let httpMock: HttpTestingController;
  let configService: ConfigService;

  const mockApiUrl = 'https://api.example.com';
  const mockLocations: Location[] = [
    { code: 'BER-HBF', name: 'Berlin Hauptbahnhof', street: 'Europaplatz 1', city: 'Berlin', postalCode: '10557', fullAddress: 'Europaplatz 1, 10557 Berlin' },
    { code: 'MUC-FLG', name: 'Munich Airport', street: 'Nordallee 25', city: 'Munich', postalCode: '85356', fullAddress: 'Nordallee 25, 85356 Munich' }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [LocationService, ConfigService]
    });

    service = TestBed.inject(LocationService);
    httpMock = TestBed.inject(HttpTestingController);
    configService = TestBed.inject(ConfigService);
    configService.setConfig({ apiUrl: mockApiUrl });
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAllLocations', () => {
    it('should return all locations', () => {
      service.getAllLocations().subscribe(locations => {
        expect(locations).toEqual(mockLocations);
        expect(locations.length).toBe(2);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/locations`);
      expect(req.request.method).toBe('GET');
      req.flush(mockLocations);
    });

    it('should handle empty locations array', () => {
      service.getAllLocations().subscribe(locations => {
        expect(locations).toEqual([]);
        expect(locations.length).toBe(0);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/locations`);
      req.flush([]);
    });

    it('should handle HTTP error', () => {
      const errorMessage = 'Server error';

      service.getAllLocations().subscribe({
        next: () => fail('should have failed with 500 error'),
        error: (error) => {
          expect(error.status).toBe(500);
        }
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/locations`);
      req.flush(errorMessage, { status: 500, statusText: 'Server Error' });
    });
  });

  describe('getLocationByCode', () => {
    it('should return location by code', () => {
      const mockLocation = mockLocations[0];

      service.getLocationByCode('BER-HBF').subscribe(location => {
        expect(location).toEqual(mockLocation);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/locations/BER-HBF`);
      expect(req.request.method).toBe('GET');
      req.flush(mockLocation);
    });

    it('should handle 404 when location not found', () => {
      service.getLocationByCode('INVALID').subscribe({
        next: () => fail('should have failed with 404 error'),
        error: (error) => {
          expect(error.status).toBe(404);
        }
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/locations/INVALID`);
      req.flush('Not found', { status: 404, statusText: 'Not Found' });
    });
  });
});
