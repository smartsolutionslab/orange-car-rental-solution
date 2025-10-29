# Layout Component Usage Guide

The `LayoutComponent` provides a flexible, responsive layout with navigation, main content, and optional sidebar areas.

## Features

- **Flexible Navigation**: Support for top (horizontal) or left (vertical sidebar) navigation
- **Optional Right Sidebar**: Show/hide sidebar as needed
- **Responsive**: Mobile-friendly with automatic layout adjustments
- **Customizable**: Configure max-width, full-width mode, and more
- **Content Projection**: Use Angular's content projection for complete flexibility

## Basic Usage

### Top Navigation Layout (Default)

```typescript
// app.component.ts
import { Component } from '@angular/core';
import { LayoutComponent } from './components/layout/layout.component';
import { NavigationComponent } from './components/navigation/navigation.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [LayoutComponent, NavigationComponent],
  template: `
    <app-layout>
      <app-navigation navigation />

      <div content>
        <h1>Welcome to Orange Car Rental</h1>
        <p>Find your perfect vehicle today!</p>
      </div>
    </app-layout>
  `
})
export class AppComponent {}
```

### With Right Sidebar

```typescript
@Component({
  template: `
    <app-layout [showSidebar]="true">
      <app-navigation navigation />

      <div content>
        <h1>Vehicle Search</h1>
        <!-- Main content here -->
      </div>

      <aside sidebar>
        <h3>Filters</h3>
        <div class="filter-group">
          <!-- Sidebar filters here -->
        </div>
      </aside>
    </app-layout>
  `
})
```

### Left Navigation Layout

```typescript
@Component({
  template: `
    <app-layout [navPosition]="'left'">
      <nav navigation>
        <div class="sidebar-nav">
          <h2>Menu</h2>
          <ul>
            <li><a routerLink="/">Dashboard</a></li>
            <li><a routerLink="/vehicles">Vehicles</a></li>
            <li><a routerLink="/bookings">Bookings</a></li>
          </ul>
        </div>
      </nav>

      <div content>
        <h1>Dashboard</h1>
        <!-- Main content here -->
      </div>
    </app-layout>
  `
})
```

### Full-Width Layout

```typescript
@Component({
  template: `
    <app-layout [fullWidth]="true">
      <app-navigation navigation />

      <div content>
        <h1>Full Width Content</h1>
        <!-- Content spans entire width -->
      </div>
    </app-layout>
  `
})
```

### Custom Max Width

```typescript
@Component({
  template: `
    <app-layout [maxWidth]="'1200px'">
      <app-navigation navigation />

      <div content>
        <h1>Custom Width Content</h1>
        <!-- Content constrained to 1200px -->
      </div>
    </app-layout>
  `
})
```

## Component Inputs

| Input | Type | Default | Description |
|-------|------|---------|-------------|
| `showSidebar` | `boolean` | `false` | Show/hide the right sidebar area |
| `navPosition` | `'top' \| 'left'` | `'top'` | Position of navigation (horizontal top or vertical left) |
| `fullWidth` | `boolean` | `false` | Whether content should be full-width |
| `maxWidth` | `string` | `'1400px'` | Maximum width for contained layout |

## Content Projection Slots

The layout uses Angular's content projection with named slots:

| Attribute | Required | Description |
|-----------|----------|-------------|
| `navigation` | Yes | Navigation content (header or sidebar nav) |
| `content` | Yes | Main content area |
| `sidebar` | No | Right sidebar content (only shown if `showSidebar` is true) |

## Complete Example with Router

```typescript
// app.component.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LayoutComponent } from './components/layout/layout.component';
import { NavigationComponent } from './components/navigation/navigation.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, LayoutComponent, NavigationComponent],
  template: `
    <app-layout>
      <app-navigation navigation />

      <div content>
        <router-outlet />
      </div>
    </app-layout>
  `
})
export class AppComponent {}
```

## Example: Dashboard with Sidebar

```typescript
// dashboard.component.ts
import { Component, signal } from '@angular/core';
import { LayoutComponent } from '../components/layout/layout.component';
import { NavigationComponent } from '../components/navigation/navigation.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [LayoutComponent, NavigationComponent],
  template: `
    <app-layout [showSidebar]="showFilters()">
      <app-navigation navigation />

      <div content>
        <div class="dashboard-header">
          <h1>Vehicle Dashboard</h1>
          <button (click)="toggleFilters()">
            {{ showFilters() ? 'Hide' : 'Show' }} Filters
          </button>
        </div>

        <div class="dashboard-content">
          <!-- Your dashboard content -->
        </div>
      </div>

      <aside sidebar>
        <h3>Filters</h3>
        <form class="filters-form">
          <div class="form-group">
            <label>Location</label>
            <select>
              <option>All Locations</option>
              <option>Berlin</option>
              <option>Munich</option>
            </select>
          </div>

          <div class="form-group">
            <label>Vehicle Type</label>
            <select>
              <option>All Types</option>
              <option>Compact</option>
              <option>SUV</option>
            </select>
          </div>

          <button type="submit">Apply Filters</button>
        </form>
      </aside>
    </app-layout>
  `,
  styles: [`
    .dashboard-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 2rem;
    }

    .filters-form {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .form-group label {
      font-weight: 500;
      font-size: 0.875rem;
    }

    .form-group select {
      padding: 0.5rem;
      border: 1px solid #ddd;
      border-radius: 4px;
    }

    button {
      padding: 0.625rem 1.25rem;
      background-color: #ff6b35;
      color: white;
      border: none;
      border-radius: 6px;
      cursor: pointer;
      font-weight: 500;
    }

    button:hover {
      background-color: #e55a28;
    }
  `]
})
export class DashboardComponent {
  protected showFilters = signal(true);

  protected toggleFilters() {
    this.showFilters.update(value => !value);
  }
}
```

## Responsive Behavior

The layout automatically adapts to different screen sizes:

- **Desktop (> 1024px)**: Full layout with sidebar
- **Tablet (768px - 1024px)**: Sidebar moves below main content
- **Mobile (< 768px)**: Single column layout, left nav becomes top nav

## Styling Tips

### Custom Content Styling

The main content area has default padding and background. Override as needed:

```css
/* In your component styles */
:host ::ng-deep .layout__main {
  padding: 0; /* Remove default padding */
  background: transparent; /* Remove background */
}
```

### Sticky Sidebar

The sidebar is already sticky by default. Adjust the position if needed:

```css
:host ::ng-deep .layout__sidebar {
  top: 100px; /* Adjust based on your header height */
}
```

### Custom Navigation Styles

Style your navigation content freely within the projection slot:

```html
<nav navigation class="my-custom-nav">
  <!-- Your navigation HTML -->
</nav>
```

```css
.my-custom-nav {
  background: linear-gradient(to right, #ff6b35, #f7931e);
  padding: 1rem 2rem;
}
```

## Advanced Patterns

### Conditional Sidebar

```typescript
@Component({
  template: `
    <app-layout [showSidebar]="hasActiveFilters()">
      <app-navigation navigation />

      <div content>
        <!-- Main content -->
      </div>

      @if (hasActiveFilters()) {
        <aside sidebar>
          <!-- Active filters display -->
        </aside>
      }
    </app-layout>
  `
})
export class SearchComponent {
  hasActiveFilters = signal(false);
}
```

### Dynamic Layout Switching

```typescript
@Component({
  template: `
    <app-layout [navPosition]="isMobile() ? 'top' : 'left'">
      <!-- Content -->
    </app-layout>
  `
})
export class ResponsiveComponent {
  isMobile = signal(window.innerWidth < 768);
}
```

## Browser Support

- Chrome, Edge, Firefox, Safari (latest 2 versions)
- CSS Grid and Flexbox required
- Sticky positioning support recommended