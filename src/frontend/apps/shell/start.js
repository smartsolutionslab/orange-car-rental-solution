const { spawn } = require('child_process');
const fs = require('fs');
const path = require('path');

// Read environment variables
// Port is fixed at 4300 for shell (isProxied: false in Aspire config)
const port = '4300';
const apiUrl = process.env.API_URL || 'http://localhost:5002';
const publicPortalUrl = process.env.PUBLIC_PORTAL_URL || 'http://localhost:4301';
const callCenterPortalUrl = process.env.CALLCENTER_PORTAL_URL || 'http://localhost:4302';

console.log(`Starting shell on port: ${port}`);
console.log(`API URL: ${apiUrl}`);
console.log(`Public Portal URL: ${publicPortalUrl}`);
console.log(`Call Center Portal URL: ${callCenterPortalUrl}`);

// Create runtime config.json
const runtimeConfig = {
  apiUrl: apiUrl
};

// Write to public/config.json
const configPath = path.join(__dirname, 'public', 'config.json');
fs.writeFileSync(configPath, JSON.stringify(runtimeConfig, null, 2));
console.log(`Runtime configuration written to: ${configPath}`);

// Generate federation manifest with dynamic URLs
const federationManifest = {
  publicPortal: `${publicPortalUrl}/remoteEntry.json`,
  callCenterPortal: `${callCenterPortalUrl}/remoteEntry.json`
};

// Write to src/assets/federation.manifest.json
const manifestPath = path.join(__dirname, 'src', 'assets', 'federation.manifest.json');
fs.writeFileSync(manifestPath, JSON.stringify(federationManifest, null, 2));
console.log(`Federation manifest written to: ${manifestPath}`);

// Also write to public/federation.manifest.json for runtime access
const publicManifestPath = path.join(__dirname, 'public', 'federation.manifest.json');
fs.writeFileSync(publicManifestPath, JSON.stringify(federationManifest, null, 2));

console.log(`Starting Angular dev server on port ${port}`);

// Path to Angular CLI in monorepo's node_modules
const ngCliPath = path.resolve(__dirname, '../../node_modules/@angular/cli/bin/ng.js');

// Use ng serve which uses @angular-architects/native-federation:build
// This wraps serve-original and handles federation setup
// Port is configured in angular.json
const ngServe = spawn('node', [ngCliPath, 'serve'], {
  stdio: 'inherit',
  shell: true
});

ngServe.on('close', (code) => {
  process.exit(code);
});
