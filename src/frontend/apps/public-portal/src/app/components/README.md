# Orange Car Rental - Shared Components

Reusable UI components for the Angular frontend applications.

## Components Overview

### 1. Layout Component ✨

**Path**: `components/layout/`

A flexible, production-ready layout system with multiple configuration options.

**Features:**
- ✅ Top or Left navigation positioning
- ✅ Optional right sidebar
- ✅ Responsive design (mobile, tablet, desktop)
- ✅ Customizable max-width
- ✅ Full-width mode option
- ✅ Sticky navigation and sidebar
- ✅ Clean, modern styling

**Files:**
- `layout.component.ts` - Main component logic
- `layout.component.html` - Template with content projection
- `layout.component.css` - Responsive styles
- `LAYOUT_USAGE.md` - Comprehensive documentation
- `app-integration-example.ts` - Integration examples
- `layout-demo.component.ts` - Interactive demo

### 2. Navigation Component 🧭

**Path**: `components/navigation/`

Example navigation component with router integration.

**Features:**
- ✅ Logo and branding
- ✅ Router link integration
- ✅ Active link highlighting
- ✅ Responsive design
- ✅ Customizable menu items

**Files:**
- `navigation.component.ts` - Navigation logic
- `navigation.component.html` - Navigation template
- `navigation.component.css` - Navigation styles

## Quick Start

### Basic Setup

1. **Import the components:**

```typescript
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
        <h1>Your Content</h1>
      </div>
    </app-layout>
  `
})
export class AppComponent {}
```

2. **Customize navigation items:**

Edit `components/navigation/navigation.component.ts`:

```typescript
protected readonly navItems = [
  { path: '/', label: 'Home', icon: 'home' },
  { path: '/vehicles', label: 'Vehicles', icon: 'car' },
  // Add your routes here
];
```

## Usage Examples

### Example 1: Simple Top Navigation

```html
<app-layout>
  <app-navigation navigation />

  <div content>
    <!-- Your content -->
  </div>
</app-layout>
```

### Example 2: With Right Sidebar

```html
<app-layout [showSidebar]="true">
  <app-navigation navigation />

  <div content>
    <!-- Main content -->
  </div>

  <aside sidebar>
    <!-- Filters, widgets, etc. -->
  </aside>
</app-layout>
```

### Example 3: Left Navigation (Dashboard Style)

```html
<app-layout [navPosition]="'left'">
  <nav navigation>
    <!-- Vertical sidebar navigation -->
  </nav>

  <div content>
    <!-- Dashboard content -->
  </div>
</app-layout>
```

### Example 4: Full Width Layout

```html
<app-layout [fullWidth]="true">
  <app-navigation navigation />

  <div content>
    <!-- Full-width content -->
  </div>
</app-layout>
```

## Layout Configuration

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `showSidebar` | `boolean` | `false` | Show/hide right sidebar |
| `navPosition` | `'top' \| 'left'` | `'top'` | Navigation position |
| `fullWidth` | `boolean` | `false` | Enable full-width content |
| `maxWidth` | `string` | `'1400px'` | Maximum content width |

## Content Projection Slots

Use these attributes to project content into specific areas:

| Attribute | Required | Description |
|-----------|----------|-------------|
| `navigation` | ✅ Yes | Navigation content |
| `content` | ✅ Yes | Main content area |
| `sidebar` | ❌ No | Right sidebar content |

## Testing the Layout

An interactive demo component is available to test all layout variations:

1. Add to your routes:

```typescript
import { LayoutDemoComponent } from './components/layout/layout-demo.component';

export const routes: Routes = [
  { path: 'demo', component: LayoutDemoComponent },
  // ... other routes
];
```

2. Navigate to `/demo` to see interactive examples

## Responsive Breakpoints

The layout automatically adapts at these breakpoints:

- **Desktop** (> 1024px): Full layout with all features
- **Tablet** (768px - 1024px): Sidebar moves below content
- **Mobile** (< 768px): Single column, stacked layout

## Styling Customization

### Override Default Styles

In your component:

```css
:host ::ng-deep .layout__main {
  padding: 2rem;
  background: #f9f9f9;
}

:host ::ng-deep .layout__sidebar {
  width: 350px;
}
```

### Custom Navigation

Replace the `NavigationComponent` with your own:

```html
<app-layout>
  <nav navigation class="my-nav">
    <!-- Your custom navigation -->
  </nav>

  <div content>
    <!-- Content -->
  </div>
</app-layout>
```

## Best Practices

### ✅ Do

- Use content projection for flexibility
- Customize navigation items for your app
- Test responsive behavior on different screen sizes
- Use the demo component during development
- Keep sidebar content lightweight

### ❌ Don't

- Put heavy content in the sidebar
- Override core layout structure
- Nest layout components
- Forget to test mobile view

## File Structure

```
components/
├── layout/
│   ├── layout.component.ts
│   ├── layout.component.html
│   ├── layout.component.css
│   ├── LAYOUT_USAGE.md
│   ├── app-integration-example.ts
│   └── layout-demo.component.ts
└── navigation/
    ├── navigation.component.ts
    ├── navigation.component.html
    └── navigation.component.css
```

## Documentation

- **Full Documentation**: `components/layout/LAYOUT_USAGE.md`
- **Integration Examples**: `components/layout/app-integration-example.ts`
- **Interactive Demo**: `components/layout/layout-demo.component.ts`
- **Quick Reference**: `../../LAYOUT_COMPONENTS.md`

## Browser Support

- Chrome, Edge, Firefox, Safari (latest 2 versions)
- Requires CSS Grid and Flexbox support
- Sticky positioning for navigation and sidebar

## Contributing

When modifying these components:

1. Update documentation
2. Test all layout variations
3. Check mobile responsiveness
4. Verify in both apps (public-portal, call-center-portal)

## Need Help?

- Read `LAYOUT_USAGE.md` for detailed examples
- Check `app-integration-example.ts` for integration patterns
- Run the demo component (`/demo`) to see live examples
- Review your browser console for any errors

## Next Steps

1. ✅ Components are ready to use in both apps
2. 📝 Customize navigation items for your routes
3. 🎨 Adjust colors and styling to match your brand
4. 🧪 Test with your actual content
5. 📱 Verify responsive behavior