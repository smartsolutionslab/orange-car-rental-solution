const fs = require('fs');
const path = require('path');

// Read API_URL from environment, default to localhost:5002
const apiUrl = process.env.API_URL || 'http://localhost:5002';

console.log(`Configuring proxy to target: ${apiUrl}`);

// Create proxy configuration
const proxyConfig = {
  '/api': {
    target: apiUrl,
    secure: false,
    changeOrigin: true,
    logLevel: 'debug'
  }
};

// Write to proxy.conf.json
const proxyPath = path.join(__dirname, 'proxy.conf.json');
fs.writeFileSync(proxyPath, JSON.stringify(proxyConfig, null, 2));

console.log(`Proxy configuration written to: ${proxyPath}`);
