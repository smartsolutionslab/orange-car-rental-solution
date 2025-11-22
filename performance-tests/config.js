// k6 Performance Test Configuration

export const config = {
  // Environment URLs
  environments: {
    local: 'http://localhost:4200',
    dev: 'https://dev.orange-rental.de',
    staging: 'https://staging.orange-rental.de',
    production: 'https://orange-rental.de'
  },

  // API Endpoints
  api: {
    local: 'http://localhost:8080',
    dev: 'https://api-dev.orange-rental.de',
    staging: 'https://api-staging.orange-rental.de',
    production: 'https://api.orange-rental.de'
  },

  // Test Users
  testUsers: [
    {
      email: 'perf.test1@orange-rental.de',
      password: 'TestPassword123!'
    },
    {
      email: 'perf.test2@orange-rental.de',
      password: 'TestPassword123!'
    },
    {
      email: 'perf.test3@orange-rental.de',
      password: 'TestPassword123!'
    }
  ],

  // Performance Thresholds (SLOs)
  thresholds: {
    // HTTP request duration
    http_req_duration: ['p(95)<500', 'p(99)<1000'], // 95% < 500ms, 99% < 1s

    // HTTP request failure rate
    http_req_failed: ['rate<0.01'], // < 1% failure rate

    // Specific API thresholds
    'http_req_duration{endpoint:vehicles}': ['p(95)<300'],
    'http_req_duration{endpoint:reservations}': ['p(95)<500'],
    'http_req_duration{endpoint:customers}': ['p(95)<400'],

    // Check success rate
    checks: ['rate>0.99'], // 99% of checks should pass

    // Request rate
    http_reqs: ['rate>100'], // At least 100 requests per second
  },

  // Load test stages
  stages: {
    smoke: [
      { duration: '30s', target: 5 },  // Ramp up to 5 users
      { duration: '1m', target: 5 },   // Stay at 5 users
      { duration: '30s', target: 0 },  // Ramp down
    ],
    load: [
      { duration: '2m', target: 50 },   // Ramp up to 50 users
      { duration: '5m', target: 50 },   // Stay at 50 users
      { duration: '2m', target: 100 },  // Ramp up to 100 users
      { duration: '5m', target: 100 },  // Stay at 100 users
      { duration: '2m', target: 0 },    // Ramp down
    ],
    stress: [
      { duration: '2m', target: 100 },  // Ramp up to 100 users
      { duration: '5m', target: 100 },  // Stay at 100 users
      { duration: '2m', target: 200 },  // Ramp up to 200 users
      { duration: '5m', target: 200 },  // Stay at 200 users
      { duration: '2m', target: 300 },  // Ramp up to 300 users
      { duration: '5m', target: 300 },  // Stay at 300 users
      { duration: '5m', target: 0 },    // Ramp down
    ],
    spike: [
      { duration: '1m', target: 50 },   // Ramp up to baseline
      { duration: '30s', target: 500 }, // Spike to 500 users
      { duration: '3m', target: 500 },  // Stay at spike
      { duration: '1m', target: 50 },   // Back to baseline
      { duration: '1m', target: 0 },    // Ramp down
    ],
    soak: [
      { duration: '5m', target: 100 },  // Ramp up to 100 users
      { duration: '2h', target: 100 },  // Stay at 100 users for 2 hours
      { duration: '5m', target: 0 },    // Ramp down
    ],
  },
};

// Get environment from environment variable or default to local
export function getBaseUrl() {
  const env = __ENV.ENVIRONMENT || 'local';
  return config.environments[env];
}

export function getApiUrl() {
  const env = __ENV.ENVIRONMENT || 'local';
  return config.api[env];
}

export default config;
