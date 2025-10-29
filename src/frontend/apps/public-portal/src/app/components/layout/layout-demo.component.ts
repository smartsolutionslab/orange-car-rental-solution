/**
 * Interactive demo component showing all layout variations.
 * Use this to test and preview different layout configurations.
 *
 * To use: Import this component and add to your routes for testing:
 * { path: 'layout-demo', component: LayoutDemoComponent }
 */

import { Component, signal } from '@angular/core';
import { LayoutComponent } from './layout.component';
import { NavigationComponent } from '../navigation/navigation.component';

@Component({
  selector: 'app-layout-demo',
  standalone: true,
  imports: [LayoutComponent, NavigationComponent],
  template: `
    <app-layout
      [showSidebar]="showSidebar()"
      [navPosition]="navPosition()"
      [fullWidth]="fullWidth()"
      [maxWidth]="maxWidth()"
    >
      <app-navigation navigation />

      <div content>
        <div class="demo-controls">
          <h1>Layout Component Demo</h1>
          <p>Interactive demo of all layout variations and configurations.</p>

          <div class="controls-grid">
            <div class="control-group">
              <h3>Layout Options</h3>

              <label class="checkbox-label">
                <input
                  type="checkbox"
                  [checked]="showSidebar()"
                  (change)="toggleSidebar()"
                />
                <span>Show Right Sidebar</span>
              </label>

              <label class="checkbox-label">
                <input
                  type="checkbox"
                  [checked]="fullWidth()"
                  (change)="toggleFullWidth()"
                />
                <span>Full Width Mode</span>
              </label>
            </div>

            <div class="control-group">
              <h3>Navigation Position</h3>

              <label class="radio-label">
                <input
                  type="radio"
                  name="navPosition"
                  value="top"
                  [checked]="navPosition() === 'top'"
                  (change)="setNavPosition('top')"
                />
                <span>Top (Horizontal)</span>
              </label>

              <label class="radio-label">
                <input
                  type="radio"
                  name="navPosition"
                  value="left"
                  [checked]="navPosition() === 'left'"
                  (change)="setNavPosition('left')"
                />
                <span>Left (Vertical Sidebar)</span>
              </label>
            </div>

            <div class="control-group">
              <h3>Max Width</h3>

              <label class="radio-label">
                <input
                  type="radio"
                  name="maxWidth"
                  value="1200px"
                  [checked]="maxWidth() === '1200px'"
                  (change)="setMaxWidth('1200px')"
                />
                <span>1200px (Narrow)</span>
              </label>

              <label class="radio-label">
                <input
                  type="radio"
                  name="maxWidth"
                  value="1400px"
                  [checked]="maxWidth() === '1400px'"
                  (change)="setMaxWidth('1400px')"
                />
                <span>1400px (Default)</span>
              </label>

              <label class="radio-label">
                <input
                  type="radio"
                  name="maxWidth"
                  value="1600px"
                  [checked]="maxWidth() === '1600px'"
                  (change)="setMaxWidth('1600px')"
                />
                <span>1600px (Wide)</span>
              </label>
            </div>
          </div>
        </div>

        <div class="demo-content">
          <h2>Main Content Area</h2>
          <p>
            This is the main content area. It adjusts automatically based on the layout configuration.
          </p>

          <div class="content-cards">
            @for (i of [1, 2, 3, 4, 5, 6]; track i) {
              <div class="content-card">
                <h3>Card {{ i }}</h3>
                <p>Sample content card with some text to demonstrate layout flow.</p>
                <button class="btn">Action</button>
              </div>
            }
          </div>

          <h2>Configuration Summary</h2>
          <div class="config-summary">
            <div class="config-item">
              <strong>Show Sidebar:</strong>
              <code>{{ showSidebar() }}</code>
            </div>
            <div class="config-item">
              <strong>Navigation Position:</strong>
              <code>{{ navPosition() }}</code>
            </div>
            <div class="config-item">
              <strong>Full Width:</strong>
              <code>{{ fullWidth() }}</code>
            </div>
            <div class="config-item">
              <strong>Max Width:</strong>
              <code>{{ maxWidth() }}</code>
            </div>
          </div>
        </div>
      </div>

      @if (showSidebar()) {
        <aside sidebar>
          <h3>Right Sidebar</h3>
          <p>This is the optional right sidebar area.</p>

          <div class="sidebar-section">
            <h4>Quick Actions</h4>
            <button class="sidebar-btn">New Booking</button>
            <button class="sidebar-btn">View Reports</button>
            <button class="sidebar-btn">Settings</button>
          </div>

          <div class="sidebar-section">
            <h4>Recent Activity</h4>
            <ul class="activity-list">
              <li>Vehicle booked - 2 min ago</li>
              <li>Payment received - 15 min ago</li>
              <li>New customer - 1 hour ago</li>
            </ul>
          </div>

          <div class="sidebar-section">
            <h4>Statistics</h4>
            <div class="stat">
              <div class="stat-value">42</div>
              <div class="stat-label">Active Bookings</div>
            </div>
            <div class="stat">
              <div class="stat-value">128</div>
              <div class="stat-label">Available Vehicles</div>
            </div>
          </div>
        </aside>
      }
    </app-layout>
  `,
  styles: [`
    .demo-controls {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 2rem;
      border-radius: 8px;
      margin-bottom: 2rem;
    }

    .demo-controls h1 {
      margin: 0 0 0.5rem 0;
      font-size: 2rem;
    }

    .demo-controls p {
      margin: 0 0 2rem 0;
      opacity: 0.9;
    }

    .controls-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 2rem;
    }

    .control-group {
      background: rgba(255, 255, 255, 0.1);
      padding: 1.5rem;
      border-radius: 6px;
    }

    .control-group h3 {
      margin: 0 0 1rem 0;
      font-size: 1.125rem;
    }

    .checkbox-label,
    .radio-label {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin-bottom: 0.75rem;
      cursor: pointer;
    }

    .checkbox-label input,
    .radio-label input {
      cursor: pointer;
    }

    .demo-content h2 {
      margin: 2rem 0 1rem 0;
      color: #333;
    }

    .content-cards {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
      gap: 1.5rem;
      margin: 2rem 0;
    }

    .content-card {
      background: white;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
    }

    .content-card h3 {
      margin: 0 0 0.5rem 0;
      color: #333;
    }

    .content-card p {
      margin: 0 0 1rem 0;
      color: #666;
      font-size: 0.875rem;
    }

    .btn {
      background-color: #ff6b35;
      color: white;
      border: none;
      padding: 0.5rem 1rem;
      border-radius: 6px;
      cursor: pointer;
      font-size: 0.875rem;
      font-weight: 500;
    }

    .btn:hover {
      background-color: #e55a28;
    }

    .config-summary {
      background: #f5f5f5;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 1.5rem;
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
    }

    .config-item {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .config-item code {
      background: white;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-family: 'Courier New', monospace;
      font-size: 0.875rem;
      color: #667eea;
    }

    /* Sidebar Styles */
    aside h3 {
      margin: 0 0 1rem 0;
      color: #333;
    }

    aside h4 {
      margin: 0 0 0.75rem 0;
      font-size: 0.875rem;
      color: #666;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .sidebar-section {
      margin-bottom: 2rem;
      padding-bottom: 2rem;
      border-bottom: 1px solid #e0e0e0;
    }

    .sidebar-section:last-child {
      border-bottom: none;
    }

    .sidebar-btn {
      display: block;
      width: 100%;
      background: white;
      border: 1px solid #e0e0e0;
      padding: 0.75rem;
      border-radius: 6px;
      cursor: pointer;
      margin-bottom: 0.5rem;
      text-align: left;
      font-size: 0.875rem;
      transition: all 0.2s;
    }

    .sidebar-btn:hover {
      background: #f5f5f5;
      border-color: #ff6b35;
      color: #ff6b35;
    }

    .activity-list {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .activity-list li {
      padding: 0.5rem 0;
      font-size: 0.875rem;
      color: #666;
      border-bottom: 1px solid #f0f0f0;
    }

    .activity-list li:last-child {
      border-bottom: none;
    }

    .stat {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      padding: 1rem;
      border-radius: 6px;
      text-align: center;
      margin-bottom: 0.5rem;
    }

    .stat-value {
      font-size: 2rem;
      font-weight: bold;
    }

    .stat-label {
      font-size: 0.75rem;
      opacity: 0.9;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }
  `]
})
export class LayoutDemoComponent {
  protected showSidebar = signal(true);
  protected navPosition = signal<'top' | 'left'>('top');
  protected fullWidth = signal(false);
  protected maxWidth = signal('1400px');

  protected toggleSidebar() {
    this.showSidebar.update(v => !v);
  }

  protected toggleFullWidth() {
    this.fullWidth.update(v => !v);
  }

  protected setNavPosition(position: 'top' | 'left') {
    this.navPosition.set(position);
  }

  protected setMaxWidth(width: string) {
    this.maxWidth.set(width);
  }
}