import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../icon/icon.component';
import type { IconName } from '../icon/icons';

/**
 * Stat card variant types for styling
 */
export type StatCardVariant = 'default' | 'success' | 'warning' | 'info' | 'error';

/**
 * Reusable statistic card component for dashboards
 * Displays a value with label and optional icon
 */
@Component({
  selector: 'ui-stat-card',
  standalone: true,
  imports: [CommonModule, IconComponent],
  templateUrl: './stat-card.component.html',
  styleUrl: './stat-card.component.css'
})
export class StatCardComponent {
  /**
   * The title/label for the statistic
   */
  readonly label = input.required<string>();

  /**
   * The value to display (number or string)
   */
  readonly value = input.required<string | number>();

  /**
   * Optional subtitle or additional info
   */
  readonly subtitle = input<string | undefined>(undefined);

  /**
   * Visual variant for the card icon
   */
  readonly variant = input<StatCardVariant>('default');

  /**
   * SVG icon path (d attribute for path element)
   * If not provided, a default icon is used based on variant
   * @deprecated Use iconName instead for consistency with IconComponent
   */
  readonly iconPath = input<string | undefined>(undefined);

  /**
   * Icon name from the icon registry
   * Preferred over iconPath for consistency
   */
  readonly iconName = input<IconName | undefined>(undefined);

  /**
   * Whether to show loading state
   */
  readonly loading = input(false);

  /**
   * Get the icon class based on variant
   */
  protected readonly iconClass = computed(() => {
    if (this.variant() === 'default') return 'stat-icon';
    return `stat-icon stat-icon--${this.variant()}`;
  });

  /**
   * Get default icon path based on variant
   */
  protected readonly defaultIconPath = computed(() => {
    const path = this.iconPath();
    if (path) return path;
    
    switch (this.variant()) {
      case 'success':
        return 'M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z';
      case 'warning':
        return 'M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z';
      case 'error':
        return 'M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z';
      case 'info':
        return 'M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-6h2v6zm0-8h-2V7h2v2z';
      default:
        return 'M19 3H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-5 14H7v-2h7v2zm3-4H7v-2h10v2zm0-4H7V7h10v2z';
    }
  });
}
