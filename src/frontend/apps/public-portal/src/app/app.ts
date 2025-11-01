import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LayoutComponent } from './components/layout/layout.component';
import { NavigationComponent } from './components/navigation/navigation.component';

/**
 * Root application component
 * Provides the main layout structure with navigation and content area
 */
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, LayoutComponent, NavigationComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {}
