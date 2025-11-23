const { spawn } = require('child_process');
const fs = require('fs');
const path = require('path');

// Read environment variables
const port = process.env.PORT || '4200';
const apiUrl = process.env.API_URL || 'http://localhost:5002';

console.log(`Generating runtime configuration with API URL: ${apiUrl}`);

// Create runtime config.json
const runtimeConfig = {
  apiUrl: apiUrl
};

// Write to public/config.json
const configPath = path.join(__dirname, 'public', 'config.json');
fs.writeFileSync(configPath, JSON.stringify(runtimeConfig, null, 2));
console.log(`Runtime configuration written to: ${configPath}`);

console.log(`Starting Angular dev server on port: ${port}`);

// Spawn ng serve with the port
const ngServe = spawn('ng', ['serve', '--port', port], {
  stdio: 'inherit',
  shell: true
});

ngServe.on('close', (code) => {
  process.exit(code);
});
