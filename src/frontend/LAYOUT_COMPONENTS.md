# Layout Components

Reusable layout components for Orange Car Rental Angular applications.

## What's Included

### Layout Component
**Location**: `apps/*/src/app/components/layout/`

A flexible layout component with:
- **Navigation Area**: Top horizontal bar or left vertical sidebar
- **Main Content Area**: Primary content with proper spacing and styling
- **Optional Right Sidebar**: Conditionally shown sidebar for filters, widgets, etc.
- **Responsive Design**: Automatically adapts to mobile, tablet, and desktop
- **Customizable**: Control width, navigation position, and more

### Navigation Component
**Location**: `apps/*/src/app/components/navigation/`

Example navigation component with:
- Logo and branding
- Menu items with router integration
- Active link highlighting
- Responsive mobile menu

## Quick Start

### 1. Basic Layout

```typescript
import { LayoutComponent } from './components/layout/layout.component';
import { NavigationComponent } from './components/navigation/navigation.component';

@Component({
  imports: [LayoutComponent, NavigationComponent],
  template: `
    <app-layout>
      <app-navigation navigation />

      <div content>
        <h1>Your Content Here</h1>
      </div>
    </app-layout>
  `
})
export class MyComponent {}
```

### 2. Layout with Sidebar

```typescript
@Component({
  template: `
    <app-layout [showSidebar]="true">
      <app-navigation navigation />

      <div content>
        <!-- Main content -->
      </div>

      <aside sidebar>
        <!-- Sidebar content -->
      </aside>
    </app-layout>
  `
})
```

### 3. Left Navigation Layout

```typescript
@Component({
  template: `
    <app-layout [navPosition]="'left'">
      <nav navigation>
        <!-- Vertical sidebar navigation -->
      </nav>

      <div content>
        <!-- Main content -->
      </div>
    </app-layout>
  `
})
```

## Configuration Options

| Input | Type | Default | Description |
|-------|------|---------|-------------|
| `showSidebar` | `boolean` | `false` | Show right sidebar |
| `navPosition` | `'top' \| 'left'` | `'top'` | Navigation position |
| `fullWidth` | `boolean` | `false` | Full-width content |
| `maxWidth` | `string` | `'1400px'` | Max width for content |

## Content Slots

| Attribute | Description |
|-----------|-------------|
| `navigation` | Navigation content |
| `content` | Main page content (required) |
| `sidebar` | Right sidebar content (optional) |

## Examples

See detailed examples and patterns in:
- `apps/public-portal/src/app/components/layout/LAYOUT_USAGE.md`
- `apps/public-portal/src/app/components/layout/app-integration-example.ts`

## Usage Patterns

### Public Portal
- Top navigation for customer-facing interface
- Optional filters sidebar for vehicle search
- Clean, marketing-focused design

### Call Center Portal
- Left navigation for internal tools
- Persistent sidebar for quick actions
- Data-dense, productivity-focused layout

## Responsive Breakpoints

- **Desktop**: > 1024px (full layout)
- **Tablet**: 768px - 1024px (sidebar below content)
- **Mobile**: < 768px (single column, top nav)

## Customization

Override styles in your component:

```css
:host ::ng-deep .layout__main {
  padding: 0;
  background: transparent;
}

:host ::ng-deep .layout__sidebar {
  width: 350px;
}
```

## Browser Support

- Modern browsers (Chrome, Firefox, Edge, Safari)
- CSS Grid and Flexbox required
- Sticky positioning recommended

## Next Steps

1. Review the usage documentation
2. Customize the navigation component for your needs
3. Integrate into your app component
4. Add your content and routes