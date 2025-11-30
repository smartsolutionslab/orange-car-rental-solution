const { spawn } = require('child_process');
const fs = require('fs');
const path = require('path');

// Read environment variables
const port = process.env.PORT || '4200';
const apiUrl = process.env.API_URL || 'http://localhost:5002';
const publicPortalUrl = process.env.PUBLIC_PORTAL_URL || 'http://localhost:4201';
const callCenterPortalUrl = process.env.CALLCENTER_PORTAL_URL || 'http://localhost:4202';

console.log(`Generating runtime configuration with API URL: ${apiUrl}`);
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

console.log(`Starting Angular dev server on port: ${port}`);

// Spawn ng serve with the port
const ngServe = spawn('ng', ['serve', '--port', port], {
  stdio: 'inherit',
  shell: true
});

ngServe.on('close', (code) => {
  process.exit(code);
});
