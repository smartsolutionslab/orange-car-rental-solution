import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ICONS, type IconName, isValidIconName } from './icons';

/**
 * Reusable SVG Icon Component
 *
 * Renders SVG icons from the centralized icon registry.
 * Icons are rendered inline as SVG elements for easy styling with CSS.
 *
 * @example
 * Basic usage:
 * ```html
 * <lib-icon name="eye" />
 * <lib-icon name="check-circle" class="text-green-600" />
 * ```
 *
 * With custom size:
 * ```html
 * <lib-icon name="user" size="lg" />
 * <lib-icon name="car" [customSize]="32" />
 * ```
 *
 * Filled vs outline style:
 * ```html
 * <lib-icon name="star" variant="filled" />
 * <lib-icon name="heart" variant="outline" />
 * ```
 */
@Component({
  selector: 'lib-icon',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (paths().length > 0) {
      <svg
        [attr.class]="svgClass()"
        [attr.width]="computedSize()"
        [attr.height]="computedSize()"
        [attr.viewBox]="'0 0 24 24'"
        [attr.fill]="variant() === 'filled' ? 'currentColor' : 'none'"
        [attr.stroke]="variant() === 'filled' ? 'none' : 'currentColor'"
        [attr.stroke-width]="strokeWidth()"
        [attr.stroke-linecap]="'round'"
        [attr.stroke-linejoin]="'round'"
        [attr.aria-hidden]="ariaHidden()"
        [attr.aria-label]="ariaLabel()"
        [attr.role]="ariaLabel() ? 'img' : undefined"
      >
        @for (path of paths(); track $index) {
          <path [attr.d]="path" />
        }
      </svg>
    }
  `,
  styles: [`
    :host {
      display: inline-flex;
      align-items: center;
      justify-content: center;
    }

    svg {
      flex-shrink: 0;
    }
  `]
})
export class IconComponent {
  /**
   * The name of the icon to display.
   * Must be a valid key from the ICONS registry.
   */
  readonly name = input.required<IconName | string>();

  /**
   * Predefined size preset.
   * - xs: 12px
   * - sm: 16px
   * - md: 20px (default)
   * - lg: 24px
   * - xl: 32px
   */
  readonly size = input<'xs' | 'sm' | 'md' | 'lg' | 'xl'>('md');

  /**
   * Custom size in pixels. Overrides the size preset.
   */
  readonly customSize = input<number | undefined>(undefined);

  /**
   * Icon rendering variant.
   * - outline: Stroke-based icons (default)
   * - filled: Fill-based icons
   */
  readonly variant = input<'outline' | 'filled'>('outline');

  /**
   * Stroke width for outline icons.
   * Default is 1.5 for a balanced look.
   */
  readonly strokeWidth = input<number>(1.5);

  /**
   * Additional CSS classes for the SVG element.
   */
  readonly class = input<string>('');

  /**
   * Accessibility: Hide icon from screen readers.
   * Set to false and provide ariaLabel for meaningful icons.
   */
  readonly ariaHidden = input<boolean>(true);

  /**
   * Accessibility: Label for screen readers.
   * When provided, ariaHidden is ignored and icon becomes accessible.
   */
  readonly ariaLabel = input<string | undefined>(undefined);

  /**
   * Size mapping from presets to pixels.
   */
  private readonly sizeMap: Record<string, number> = {
    xs: 12,
    sm: 16,
    md: 20,
    lg: 24,
    xl: 32,
  };

  /**
   * Computed SVG paths from the icon registry.
   */
  readonly paths = computed<string[]>(() => {
    const iconName = this.name();
    if (isValidIconName(iconName)) {
      return ICONS[iconName];
    }
    console.warn(`Icon "${iconName}" not found in registry`);
    return [];
  });

  /**
   * Computed pixel size for the icon.
   */
  readonly computedSize = computed<number>(() => {
    const custom = this.customSize();
    if (custom !== undefined) {
      return custom;
    }
    return this.sizeMap[this.size()] ?? 20;
  });

  /**
   * Computed CSS class string for the SVG.
   */
  readonly svgClass = computed<string>(() => {
    const classes: string[] = [];
    const inputClass = this.class();
    if (inputClass) {
      classes.push(inputClass);
    }
    return classes.join(' ');
  });
}
