const { spawn } = require('child_process');
const fs = require('fs');
const path = require('path');

// Read environment variables
// Port is fixed at 4301 for public-portal (isProxied: false in Aspire config)
const port = '4301';
const apiUrl = process.env.API_URL || 'http://localhost:5002';

console.log(`Starting public-portal on port: ${port}`);
console.log(`API URL: ${apiUrl}`);

// Create runtime config.json
const runtimeConfig = {
  apiUrl: apiUrl
};

// Write to public/config.json
const configPath = path.join(__dirname, 'public', 'config.json');
fs.writeFileSync(configPath, JSON.stringify(runtimeConfig, null, 2));
console.log(`Runtime configuration written to: ${configPath}`);

console.log(`Starting Angular dev server on port ${port}`);

// Path to Angular CLI in monorepo's node_modules
const ngCliPath = path.resolve(__dirname, '../../node_modules/@angular/cli/bin/ng.js');

// Use ng serve which uses @angular-architects/native-federation:build
// This wraps serve-original and generates the remoteEntry.json for federation
// Port is configured in angular.json
const ngServe = spawn('node', [ngCliPath, 'serve'], {
  stdio: 'inherit',
  shell: true
});

ngServe.on('close', (code) => {
  process.exit(code);
});
