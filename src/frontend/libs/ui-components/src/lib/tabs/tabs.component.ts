import {
  Component,
  input,
  output,
  signal,
  computed,
  contentChildren,
  effect,
  HostListener,
} from "@angular/core";
import type { AfterContentInit } from "@angular/core";
import { CommonModule, NgTemplateOutlet } from "@angular/common";
import { TabComponent } from "./tab.component";

/**
 * Event emitted when the active tab changes
 */
export interface TabChangeEvent {
  /** Previous tab ID */
  previousTabId: string | null;
  /** Current tab ID */
  currentTabId: string;
  /** Index of the current tab */
  currentIndex: number;
}

/**
 * Tabs Component
 *
 * A tabbed interface component with keyboard navigation and lazy loading.
 * Follows WAI-ARIA tabs pattern for accessibility.
 *
 * @example
 * <ocr-tabs (tabChange)="onTabChange($event)">
 *   <ocr-tab tabId="info" label="Information">
 *     <ng-template>Info content</ng-template>
 *   </ocr-tab>
 *   <ocr-tab tabId="settings" label="Settings">
 *     <ng-template>Settings content</ng-template>
 *   </ocr-tab>
 * </ocr-tabs>
 */
@Component({
  selector: "ocr-tabs",
  standalone: true,
  imports: [CommonModule, NgTemplateOutlet],
  template: `
    <div class="tabs" [class.tabs--vertical]="orientation() === 'vertical'">
      <!-- Tab list -->
      <div
        class="tabs__list"
        role="tablist"
        [attr.aria-label]="ariaLabel()"
        [attr.aria-orientation]="orientation()"
      >
        @for (tab of tabs(); track tab.tabId(); let i = $index) {
          <button
            class="tabs__tab"
            [class.tabs__tab--active]="tab.tabId() === activeTabId()"
            [class.tabs__tab--disabled]="tab.disabled()"
            [disabled]="tab.disabled()"
            role="tab"
            [id]="'tab-' + tab.tabId()"
            [attr.aria-selected]="tab.tabId() === activeTabId()"
            [attr.aria-controls]="'panel-' + tab.tabId()"
            [attr.tabindex]="tab.tabId() === activeTabId() ? 0 : -1"
            (click)="selectTab(tab.tabId())"
            (keydown)="onTabKeydown($event, i)"
          >
            @if (tab.icon()) {
              <span class="tabs__tab-icon">{{ tab.icon() }}</span>
            }
            <span class="tabs__tab-label">{{ tab.label() }}</span>
            @if (tab.badge() !== undefined) {
              <span class="tabs__tab-badge">{{ tab.badge() }}</span>
            }
          </button>
        }
        <div class="tabs__indicator" [style]="indicatorStyle()"></div>
      </div>

      <!-- Tab panels -->
      <div class="tabs__panels">
        @for (tab of tabs(); track tab.tabId()) {
          <div
            class="tabs__panel"
            [class.tabs__panel--active]="tab.tabId() === activeTabId()"
            role="tabpanel"
            [id]="'panel-' + tab.tabId()"
            [attr.aria-labelledby]="'tab-' + tab.tabId()"
            [attr.hidden]="tab.tabId() !== activeTabId() ? true : null"
            [attr.tabindex]="tab.tabId() === activeTabId() ? 0 : -1"
          >
            @if (
              tab.tabId() === activeTabId() || (keepAlive() && tab.hasVisited())
            ) {
              @if (tab.contentTemplate()) {
                <ng-container
                  *ngTemplateOutlet="tab.contentTemplate()!"
                ></ng-container>
              }
            }
          </div>
        }
      </div>
    </div>
  `,
  styles: [
    `
      .tabs {
        display: flex;
        flex-direction: column;
      }

      .tabs--vertical {
        flex-direction: row;
      }

      .tabs__list {
        display: flex;
        position: relative;
        border-bottom: 1px solid #e5e7eb;
        gap: 0;
      }

      .tabs--vertical .tabs__list {
        flex-direction: column;
        border-bottom: none;
        border-right: 1px solid #e5e7eb;
        min-width: 12rem;
      }

      .tabs__tab {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        padding: 0.75rem 1rem;
        background: none;
        border: none;
        cursor: pointer;
        font-size: 0.875rem;
        font-weight: 500;
        color: #6b7280;
        transition: color 0.15s ease;
        position: relative;
        white-space: nowrap;
      }

      .tabs__tab:hover:not(:disabled) {
        color: #374151;
      }

      .tabs__tab:focus-visible {
        outline: 2px solid #f97316;
        outline-offset: -2px;
        border-radius: 0.25rem;
      }

      .tabs__tab--active {
        color: #f97316;
      }

      .tabs__tab--disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }

      .tabs__tab-icon {
        font-size: 1rem;
      }

      .tabs__tab-badge {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        min-width: 1.25rem;
        height: 1.25rem;
        padding: 0 0.375rem;
        font-size: 0.75rem;
        font-weight: 600;
        background-color: #f3f4f6;
        color: #6b7280;
        border-radius: 9999px;
      }

      .tabs__tab--active .tabs__tab-badge {
        background-color: #fff7ed;
        color: #f97316;
      }

      .tabs__indicator {
        position: absolute;
        bottom: -1px;
        height: 2px;
        background-color: #f97316;
        transition:
          left 0.2s ease,
          width 0.2s ease;
      }

      .tabs--vertical .tabs__indicator {
        bottom: auto;
        right: -1px;
        left: auto !important;
        width: 2px !important;
        height: auto;
        transition:
          top 0.2s ease,
          height 0.2s ease;
      }

      .tabs__panels {
        flex: 1;
        min-height: 0;
      }

      .tabs__panel {
        display: none;
        padding: 1rem 0;
      }

      .tabs--vertical .tabs__panel {
        padding: 0 1rem;
      }

      .tabs__panel--active {
        display: block;
      }

      .tabs__panel:focus {
        outline: none;
      }
    `,
  ],
})
export class TabsComponent implements AfterContentInit {
  /**
   * Orientation of the tabs
   */
  readonly orientation = input<"horizontal" | "vertical">("horizontal");

  /**
   * Whether to keep visited tab content in DOM
   */
  readonly keepAlive = input(false);

  /**
   * Default active tab ID (if not set, first tab is active)
   */
  readonly defaultTabId = input<string | undefined>(undefined);

  /**
   * Accessible label for the tab list
   */
  readonly ariaLabel = input("Tabs");

  /**
   * Emitted when active tab changes
   */
  readonly tabChange = output<TabChangeEvent>();

  /**
   * Tab components from content projection
   */
  readonly tabs = contentChildren(TabComponent);

  /**
   * Currently active tab ID
   */
  readonly activeTabId = signal<string>("");

  /**
   * Active tab index
   */
  readonly activeIndex = computed(() => {
    const tabs = this.tabs();
    return tabs.findIndex((t) => t.tabId() === this.activeTabId());
  });

  /**
   * Style for the active indicator
   */
  readonly indicatorStyle = signal("");

  constructor() {
    // Initialize active tab when tabs change
    effect(
      () => {
        const tabs = this.tabs();
        if (tabs.length === 0) return;

        const currentActive = this.activeTabId();
        const defaultId = this.defaultTabId();

        // If no active tab or active tab doesn't exist, set default
        if (!currentActive || !tabs.some((t) => t.tabId() === currentActive)) {
          const newActiveId =
            defaultId && tabs.some((t) => t.tabId() === defaultId)
              ? defaultId
              : tabs[0].tabId();
          this.activeTabId.set(newActiveId);
        }

        // Update tab states
        tabs.forEach((tab) => {
          const isActive = tab.tabId() === this.activeTabId();
          tab.isActive.set(isActive);
          if (isActive) {
            tab.hasVisited.set(true);
          }
        });
      },
      { allowSignalWrites: true },
    );
  }

  ngAfterContentInit(): void {
    // Update indicator position after content init
    setTimeout(() => this.updateIndicator(), 0);
  }

  /**
   * Select a tab by ID
   */
  selectTab(tabId: string): void {
    const tabs = this.tabs();
    const tab = tabs.find((t) => t.tabId() === tabId);

    if (!tab || tab.disabled()) return;

    const previousTabId = this.activeTabId();
    if (previousTabId === tabId) return;

    this.activeTabId.set(tabId);

    // Update tab states
    tabs.forEach((t) => {
      const isActive = t.tabId() === tabId;
      t.isActive.set(isActive);
      if (isActive) {
        t.hasVisited.set(true);
      }
    });

    // Update indicator
    this.updateIndicator();

    // Emit change event
    this.tabChange.emit({
      previousTabId,
      currentTabId: tabId,
      currentIndex: this.activeIndex(),
    });
  }

  /**
   * Handle keyboard navigation
   */
  onTabKeydown(event: KeyboardEvent, currentIndex: number): void {
    const tabs = this.tabs();
    const enabledTabs = tabs.filter((t) => !t.disabled());

    let newIndex: number | null = null;
    const isVertical = this.orientation() === "vertical";
    const prevKey = isVertical ? "ArrowUp" : "ArrowLeft";
    const nextKey = isVertical ? "ArrowDown" : "ArrowRight";

    if (event.key === prevKey) {
      event.preventDefault();
      newIndex = this.findPreviousEnabledIndex(currentIndex);
    } else if (event.key === nextKey) {
      event.preventDefault();
      newIndex = this.findNextEnabledIndex(currentIndex);
    } else if (event.key === "Home") {
      event.preventDefault();
      const firstEnabled = enabledTabs[0];
      if (firstEnabled) {
        newIndex = tabs.findIndex((t) => t === firstEnabled);
      }
    } else if (event.key === "End") {
      event.preventDefault();
      const lastEnabled = enabledTabs[enabledTabs.length - 1];
      if (lastEnabled) {
        newIndex = tabs.findIndex((t) => t === lastEnabled);
      }
    }

    if (newIndex !== null && newIndex >= 0) {
      this.selectTab(tabs[newIndex].tabId());
      // Focus the new tab button
      const tabButton = document.getElementById(
        "tab-" + tabs[newIndex].tabId(),
      );
      tabButton?.focus();
    }
  }

  private findPreviousEnabledIndex(currentIndex: number): number {
    const tabs = this.tabs();
    let index = currentIndex - 1;

    while (index >= 0) {
      if (!tabs[index].disabled()) {
        return index;
      }
      index--;
    }

    // Wrap to end
    index = tabs.length - 1;
    while (index > currentIndex) {
      if (!tabs[index].disabled()) {
        return index;
      }
      index--;
    }

    return currentIndex;
  }

  private findNextEnabledIndex(currentIndex: number): number {
    const tabs = this.tabs();
    let index = currentIndex + 1;

    while (index < tabs.length) {
      if (!tabs[index].disabled()) {
        return index;
      }
      index++;
    }

    // Wrap to start
    index = 0;
    while (index < currentIndex) {
      if (!tabs[index].disabled()) {
        return index;
      }
      index++;
    }

    return currentIndex;
  }

  private updateIndicator(): void {
    const tabs = this.tabs();
    const activeIndex = this.activeIndex();

    if (activeIndex < 0) {
      this.indicatorStyle.set("display: none");
      return;
    }

    const tabButton = document.getElementById(
      "tab-" + tabs[activeIndex].tabId(),
    );
    if (!tabButton) return;

    if (this.orientation() === "horizontal") {
      this.indicatorStyle.set(
        `left: ${tabButton.offsetLeft}px; width: ${tabButton.offsetWidth}px`,
      );
    } else {
      this.indicatorStyle.set(
        `top: ${tabButton.offsetTop}px; height: ${tabButton.offsetHeight}px`,
      );
    }
  }

  @HostListener("window:resize")
  onResize(): void {
    this.updateIndicator();
  }
}
