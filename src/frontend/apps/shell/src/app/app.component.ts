import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <div class="shell-container">
      <header class="shell-header">
        <nav>
          <h1>Orange Car Rental</h1>
          <ul class="nav-links">
            <li><a routerLink="/">Home</a></li>
            <li><a routerLink="/vehicles">Vehicles</a></li>
            <li><a routerLink="/booking">Booking</a></li>
            <li><a routerLink="/admin">Admin</a></li>
          </ul>
        </nav>
      </header>

      <main class="shell-main">
        <router-outlet></router-outlet>
      </main>

      <footer class="shell-footer">
        <p>&copy; 2025 Orange Car Rental - Microfrontend Architecture</p>
      </footer>
    </div>
  `,
  styles: [`
    .shell-container {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }

    .shell-header {
      background-color: #ff6600;
      color: white;
      padding: 1rem 2rem;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .shell-header nav {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .shell-header h1 {
      margin: 0;
      font-size: 1.5rem;
    }

    .nav-links {
      display: flex;
      list-style: none;
      gap: 2rem;
      margin: 0;
      padding: 0;
    }

    .nav-links a {
      color: white;
      text-decoration: none;
      font-weight: 500;
      transition: opacity 0.2s;
    }

    .nav-links a:hover {
      opacity: 0.8;
    }

    .shell-main {
      flex: 1;
      padding: 2rem;
      background-color: #f5f5f5;
    }

    .shell-footer {
      background-color: #333;
      color: white;
      text-align: center;
      padding: 1rem;
    }

    .shell-footer p {
      margin: 0;
    }
  `]
})
export class AppComponent {
  title = 'Shell Application';
}
