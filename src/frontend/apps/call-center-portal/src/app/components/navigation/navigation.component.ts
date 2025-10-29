import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.css'
})
export class NavigationComponent {
  protected readonly navItems = [
    { path: '/', label: 'Fahrzeuge', icon: 'car' },
    { path: '/reservations', label: 'Buchungen', icon: 'calendar' },
    { path: '/locations', label: 'Standorte', icon: 'location' },
    { path: '/contact', label: 'Kontakt', icon: 'mail' }
  ];
}