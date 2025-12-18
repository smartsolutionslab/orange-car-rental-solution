import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="home-container">
      <h1>Welcome to Orange Car Rental</h1>
      <p class="subtitle">Microfrontend Architecture - Shell Application</p>

      <div class="info-cards">
        <div class="card">
          <h3>Public Portal</h3>
          <p>Browse vehicles, make reservations, and manage your bookings.</p>
          <a routerLink="/vehicles" class="btn">Browse Vehicles</a>
        </div>

        <div class="card">
          <h3>Call Center Portal</h3>
          <p>Administrative tools for managing reservations and customers.</p>
          <a routerLink="/admin" class="btn">Admin Panel</a>
        </div>
      </div>

      <div class="architecture-info">
        <h2>Architecture</h2>
        <p>
          This application uses <strong>Module Federation</strong> to compose multiple
          microfrontends at runtime:
        </p>
        <ul>
          <li><strong>Shell</strong> - Host application (this)</li>
          <li><strong>Public Portal</strong> - Customer-facing features</li>
          <li><strong>Call Center Portal</strong> - Administrative features</li>
        </ul>
      </div>
    </div>
  `,
  styles: [
    `
      .home-container {
        max-width: 1200px;
        margin: 0 auto;
        padding: 2rem;
      }

      h1 {
        color: #ff6600;
        font-size: 2.5rem;
        margin-bottom: 0.5rem;
      }

      .subtitle {
        color: #666;
        font-size: 1.2rem;
        margin-bottom: 3rem;
      }

      .info-cards {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
        gap: 2rem;
        margin-bottom: 3rem;
      }

      .card {
        background: white;
        border-radius: 8px;
        padding: 2rem;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
        transition: transform 0.2s;
      }

      .card:hover {
        transform: translateY(-4px);
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
      }

      .card h3 {
        color: #ff6600;
        margin-top: 0;
      }

      .btn {
        display: inline-block;
        background: #ff6600;
        color: white;
        padding: 0.75rem 1.5rem;
        border-radius: 4px;
        text-decoration: none;
        font-weight: 500;
        transition: background 0.2s;
      }

      .btn:hover {
        background: #e55a00;
      }

      .architecture-info {
        background: #f9f9f9;
        border-left: 4px solid #ff6600;
        padding: 1.5rem;
        border-radius: 4px;
      }

      .architecture-info h2 {
        color: #333;
        margin-top: 0;
      }

      .architecture-info ul {
        line-height: 1.8;
      }

      .architecture-info strong {
        color: #ff6600;
      }
    `,
  ],
})
export class HomeComponent {}
