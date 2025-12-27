import { bootstrapApplication } from '@angular/platform-browser';
// Import locale registration (also imported by app.routes.ts for federation)
import './app/locale-de';
import { appConfig } from './app/app.config';
import { App } from './app/app';

bootstrapApplication(App, appConfig).catch((err) => console.error(err));
