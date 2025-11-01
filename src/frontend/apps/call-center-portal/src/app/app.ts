import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LayoutComponent } from './components/layout/layout.component';
import { NavigationComponent } from './components/navigation/navigation.component';

/**
 * Root application component for call center portal
 * Provides the main layout structure with left navigation
 */
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LayoutComponent, NavigationComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {}
