import { Component, input } from "@angular/core";
import { CommonModule } from "@angular/common";

/**
 * Shared layout component with navigation, content, and optional sidebar areas.
 *
 * Usage:
 * ```html
 * <ocr-layout>
 *   <nav navigation>
 *     <!-- Navigation content here -->
 *   </nav>
 *
 *   <main content>
 *     <!-- Main content here -->
 *   </main>
 *
 *   <aside sidebar>
 *     <!-- Optional sidebar content here -->
 *   </aside>
 * </ocr-layout>
 * ```
 */
@Component({
  selector: "ocr-layout",
  standalone: true,
  imports: [CommonModule],
  templateUrl: "./layout.component.html",
  styleUrl: "./layout.component.css",
})
export class LayoutComponent {
  /**
   * Whether to show the sidebar area.
   * Default: false
   */
  showSidebar = input<boolean>(false);

  /**
   * Navigation position: 'top' (horizontal) or 'left' (vertical sidebar).
   * Default: 'top'
   */
  navPosition = input<"top" | "left">("top");

  /**
   * Whether the layout should be full-width or contained.
   * Default: false
   */
  fullWidth = input<boolean>(false);

  /**
   * Maximum width for contained layout (in pixels or CSS value).
   * Default: '1400px'
   */
  maxWidth = input<string>("1400px");

  /**
   * Theme variant for different portals.
   * Default: 'default'
   */
  theme = input<"default" | "admin">("default");
}
