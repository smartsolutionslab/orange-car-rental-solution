const { spawn } = require('child_process');

// Read PORT from environment, default to 4202
const port = process.env.PORT || '4202';

console.log(`Starting Angular dev server on port: ${port}`);

// Spawn ng serve with the port and proxy config
const ngServe = spawn('ng', ['serve', '--port', port, '--proxy-config', 'proxy.conf.json'], {
  stdio: 'inherit',
  shell: true
});

ngServe.on('close', (code) => {
  process.exit(code);
});
