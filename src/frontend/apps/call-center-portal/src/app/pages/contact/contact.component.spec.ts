import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { ContactComponent } from './contact.component';

describe('ContactComponent', () => {
  let component: ContactComponent;
  let fixture: ComponentFixture<ContactComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContactComponent, TranslateModule.forRoot()],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ContactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Page Header', () => {
    it('should render page title', () => {
      const title = fixture.nativeElement.querySelector('.page-header h1');
      expect(title).toBeTruthy();
    });

    it('should render page subtitle', () => {
      const subtitle = fixture.nativeElement.querySelector('.page-header .subtitle');
      expect(subtitle).toBeTruthy();
    });
  });

  describe('Contact Information', () => {
    it('should render contact grid', () => {
      const grid = fixture.nativeElement.querySelector('.contact-grid');
      expect(grid).toBeTruthy();
    });

    it('should render four contact cards', () => {
      const cards = fixture.nativeElement.querySelectorAll('.contact-card');
      expect(cards.length).toBe(4);
    });

    it('should render phone contact card', () => {
      const cards = fixture.nativeElement.querySelectorAll('.contact-card');
      const phoneCard = Array.from(cards).find((card) =>
        (card as HTMLElement).textContent?.includes('+49 30 1234567'),
      );
      expect(phoneCard).toBeTruthy();
    });

    it('should render email contact card', () => {
      const cards = fixture.nativeElement.querySelectorAll('.contact-card');
      const emailCard = Array.from(cards).find((card) =>
        (card as HTMLElement).textContent?.includes('callcenter@orangecarrental.de'),
      );
      expect(emailCard).toBeTruthy();
    });

    it('should render headquarters contact card', () => {
      const cards = fixture.nativeElement.querySelectorAll('.contact-card');
      const addressCard = Array.from(cards).find((card) =>
        (card as HTMLElement).textContent?.includes('Friedrichstraße 123'),
      );
      expect(addressCard).toBeTruthy();
    });

    it('should render support hotline contact card', () => {
      const cards = fixture.nativeElement.querySelectorAll('.contact-card');
      const supportCard = Array.from(cards).find((card) =>
        (card as HTMLElement).textContent?.includes('+49 30 9876543'),
      );
      expect(supportCard).toBeTruthy();
    });

    it('should render contact icons', () => {
      const icons = fixture.nativeElement.querySelectorAll('.contact-icon svg');
      expect(icons.length).toBe(4);
    });
  });

  describe('Quick Links', () => {
    it('should render quick links section', () => {
      const quickLinks = fixture.nativeElement.querySelector('.quick-links');
      expect(quickLinks).toBeTruthy();
    });

    it('should render four quick link buttons', () => {
      const buttons = fixture.nativeElement.querySelectorAll('.quick-link-btn');
      expect(buttons.length).toBe(4);
    });

    it('should render quick link icons', () => {
      const icons = fixture.nativeElement.querySelectorAll('.quick-link-btn svg');
      expect(icons.length).toBe(4);
    });
  });

  describe('Content Sections', () => {
    it('should render two content sections', () => {
      const sections = fixture.nativeElement.querySelectorAll('.content-section');
      expect(sections.length).toBe(2);
    });

    it('should render section headings', () => {
      const headings = fixture.nativeElement.querySelectorAll('.content-section h2');
      expect(headings.length).toBe(2);
    });
  });
});
