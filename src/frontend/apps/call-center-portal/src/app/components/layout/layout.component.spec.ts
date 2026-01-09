import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LayoutComponent } from './layout.component';

@Component({
  standalone: true,
  imports: [LayoutComponent],
  template: `
    <app-layout
      [showSidebar]="showSidebar"
      [navPosition]="navPosition"
      [fullWidth]="fullWidth"
      [maxWidth]="maxWidth"
    >
      <div navigation class="layout-navigation">Navigation Content</div>
      <div content class="layout-content">Main Content</div>
      <div sidebar class="layout-sidebar">Sidebar Content</div>
    </app-layout>
  `,
})
class TestHostComponent {
  showSidebar = false;
  navPosition: 'top' | 'left' = 'top';
  fullWidth = false;
  maxWidth = '1400px';
}

describe('LayoutComponent', () => {
  let hostComponent: TestHostComponent;
  let fixture: ComponentFixture<TestHostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestHostComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TestHostComponent);
    hostComponent = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(hostComponent).toBeTruthy();
  });

  describe('Default State', () => {
    it('should render layout container', () => {
      const layout = fixture.nativeElement.querySelector('.layout');
      expect(layout).toBeTruthy();
    });

    it('should have top navigation by default', () => {
      const layout = fixture.nativeElement.querySelector('.layout');
      expect(layout.classList.contains('layout--nav-top')).toBeTruthy();
      expect(layout.classList.contains('layout--nav-left')).toBeFalsy();
    });

    it('should not show sidebar by default', () => {
      const layout = fixture.nativeElement.querySelector('.layout');
      expect(layout.classList.contains('layout--with-sidebar')).toBeFalsy();
    });

    it('should not be full width by default', () => {
      const layout = fixture.nativeElement.querySelector('.layout');
      expect(layout.classList.contains('layout--full-width')).toBeFalsy();
    });
  });

  describe('Top Navigation Layout', () => {
    it('should render header element', () => {
      const header = fixture.nativeElement.querySelector('.layout__header');
      expect(header).toBeTruthy();
    });

    it('should render main content area', () => {
      const main = fixture.nativeElement.querySelector('.layout__main');
      expect(main).toBeTruthy();
    });

    it('should project navigation content', () => {
      const navigation = fixture.nativeElement.querySelector('.layout-navigation');
      expect(navigation).toBeTruthy();
      expect(navigation.textContent).toContain('Navigation Content');
    });

    it('should project main content', () => {
      const content = fixture.nativeElement.querySelector('.layout-content');
      expect(content).toBeTruthy();
      expect(content.textContent).toContain('Main Content');
    });

    it('should not render sidebar when showSidebar is false', () => {
      const sidebar = fixture.nativeElement.querySelector('.layout__sidebar');
      expect(sidebar).toBeFalsy();
    });
  });

  describe('Left Navigation Layout', () => {
    beforeEach(() => {
      hostComponent.navPosition = 'left';
      fixture.detectChanges();
    });

    it('should add nav-left class', () => {
      const layout = fixture.nativeElement.querySelector('.layout');
      expect(layout.classList.contains('layout--nav-left')).toBeTruthy();
      expect(layout.classList.contains('layout--nav-top')).toBeFalsy();
    });

    it('should render nav sidebar element', () => {
      const navSidebar = fixture.nativeElement.querySelector('.layout__nav-sidebar');
      expect(navSidebar).toBeTruthy();
    });

    it('should not render header element', () => {
      const header = fixture.nativeElement.querySelector('.layout__header');
      expect(header).toBeFalsy();
    });
  });

  describe('Sidebar Toggle', () => {
    it('should show sidebar when showSidebar is true', () => {
      hostComponent.showSidebar = true;
      fixture.detectChanges();

      const layout = fixture.nativeElement.querySelector('.layout');
      expect(layout.classList.contains('layout--with-sidebar')).toBeTruthy();

      const sidebar = fixture.nativeElement.querySelector('.layout__sidebar');
      expect(sidebar).toBeTruthy();
    });

    it('should project sidebar content when visible', () => {
      hostComponent.showSidebar = true;
      fixture.detectChanges();

      const sidebarContent = fixture.nativeElement.querySelector('.layout-sidebar');
      expect(sidebarContent.textContent).toContain('Sidebar Content');
    });
  });

  describe('Full Width Mode', () => {
    it('should add full-width class when enabled', () => {
      hostComponent.fullWidth = true;
      fixture.detectChanges();

      const layout = fixture.nativeElement.querySelector('.layout');
      expect(layout.classList.contains('layout--full-width')).toBeTruthy();
    });

    it('should set max-width to 100% when full width', () => {
      hostComponent.fullWidth = true;
      fixture.detectChanges();

      const body = fixture.nativeElement.querySelector('.layout__body');
      expect(body.style.maxWidth).toBe('100%');
    });

    it('should use custom max-width when not full width', () => {
      hostComponent.maxWidth = '1200px';
      fixture.detectChanges();

      const body = fixture.nativeElement.querySelector('.layout__body');
      expect(body.style.maxWidth).toBe('1200px');
    });
  });

  describe('Content Projection', () => {
    it('should render all projected content slots', () => {
      hostComponent.showSidebar = true;
      fixture.detectChanges();

      const navigation = fixture.nativeElement.querySelector('.layout-navigation');
      const content = fixture.nativeElement.querySelector('.layout-content');
      const sidebar = fixture.nativeElement.querySelector('.layout-sidebar');

      expect(navigation).toBeTruthy();
      expect(content).toBeTruthy();
      expect(sidebar).toBeTruthy();
    });
  });
});
