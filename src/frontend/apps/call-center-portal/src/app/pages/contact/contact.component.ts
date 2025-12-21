import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

/**
 * Contact page for call center
 * Displays contact information and support tools
 */
@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.css',
})
export class ContactComponent {}
