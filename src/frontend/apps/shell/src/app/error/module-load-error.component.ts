import { Component } from '@angular/core';

@Component({
  selector: 'app-module-load-error',
  standalone: true,
  template: `
    <div class="error-container">
      <div class="error-icon">⚠️</div>
      <h1>Module Load Error</h1>
      <p>Unable to load the requested microfrontend module.</p>

      <div class="error-details">
        <h3>Possible Causes:</h3>
        <ul>
          <li>The remote microfrontend is not running</li>
          <li>Network connectivity issues</li>
          <li>Module Federation configuration mismatch</li>
          <li>CORS policy blocking the request</li>
        </ul>
      </div>

      <div class="error-actions">
        <h3>Troubleshooting:</h3>
        <ol>
          <li>Ensure all microfrontend applications are running</li>
          <li>Check the browser console for detailed error messages</li>
          <li>Verify the Module Federation configuration</li>
          <li>Check network requests in Developer Tools</li>
        </ol>
      </div>

      <a href="/" class="btn-home">Return to Home</a>
    </div>
  `,
  styles: [
    `
      .error-container {
        max-width: 800px;
        margin: 2rem auto;
        padding: 2rem;
        text-align: center;
      }

      .error-icon {
        font-size: 4rem;
        margin-bottom: 1rem;
      }

      h1 {
        color: #d32f2f;
        margin-bottom: 1rem;
      }

      .error-details,
      .error-actions {
        background: #fff3cd;
        border: 1px solid #ffc107;
        border-radius: 4px;
        padding: 1.5rem;
        margin: 1.5rem 0;
        text-align: left;
      }

      .error-details h3,
      .error-actions h3 {
        color: #856404;
        margin-top: 0;
      }

      ul,
      ol {
        margin: 0.5rem 0;
        padding-left: 1.5rem;
      }

      li {
        margin: 0.5rem 0;
        color: #856404;
      }

      .btn-home {
        display: inline-block;
        background: #ff6600;
        color: white;
        padding: 0.75rem 2rem;
        border-radius: 4px;
        text-decoration: none;
        font-weight: 500;
        margin-top: 1.5rem;
        transition: background 0.2s;
      }

      .btn-home:hover {
        background: #e55a00;
      }
    `,
  ],
})
export class ModuleLoadErrorComponent {}
