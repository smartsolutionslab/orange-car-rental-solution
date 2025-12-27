import { TestBed } from '@angular/core/testing';
import { ConfigService } from './config.service';

describe('ConfigService', () => {
  let service: ConfigService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ConfigService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should have empty apiUrl by default', () => {
    expect(service.apiUrl).toBe('');
  });

  it('should set and get config', () => {
    const config = { apiUrl: 'https://api.example.com' };
    service.setConfig(config);
    expect(service.apiUrl).toBe('https://api.example.com');
  });

  it('should update config when setConfig is called again', () => {
    service.setConfig({ apiUrl: 'https://api.example.com' });
    expect(service.apiUrl).toBe('https://api.example.com');

    service.setConfig({ apiUrl: 'https://new-api.example.com' });
    expect(service.apiUrl).toBe('https://new-api.example.com');
  });
});
