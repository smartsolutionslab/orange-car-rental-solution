/**
 * Lighthouse CI Configuration - Public Portal
 *
 * Performance budgets and CI settings for automated Lighthouse testing.
 */

module.exports = {
  ci: {
    collect: {
      // URLs to test
      url: [
        'http://localhost:4200',
        'http://localhost:4200/booking-history',
        'http://localhost:4200/search',
      ],

      // Number of runs per URL
      numberOfRuns: 3,

      // Lighthouse settings
      settings: {
        preset: 'desktop',
        // Custom settings
        throttling: {
          rttMs: 40,
          throughputKbps: 10240,
          cpuSlowdownMultiplier: 1,
        },
      },
    },

    assert: {
      // Performance budgets
      assertions: {
        // Core Web Vitals
        'first-contentful-paint': ['error', { maxNumericValue: 2000 }],
        'largest-contentful-paint': ['error', { maxNumericValue: 2500 }],
        'cumulative-layout-shift': ['error', { maxNumericValue: 0.1 }],
        'total-blocking-time': ['error', { maxNumericValue: 300 }],

        // Other metrics
        'speed-index': ['warn', { maxNumericValue: 3000 }],
        'interactive': ['error', { maxNumericValue: 5000 }],

        // Category scores (0-100)
        'categories:performance': ['error', { minScore: 0.90 }],
        'categories:accessibility': ['error', { minScore: 0.95 }],
        'categories:best-practices': ['error', { minScore: 0.90 }],
        'categories:seo': ['error', { minScore: 0.90 }],

        // Resource budgets
        'resource-summary:document:size': ['warn', { maxNumericValue: 50000 }],
        'resource-summary:script:size': ['warn', { maxNumericValue: 500000 }],
        'resource-summary:stylesheet:size': ['warn', { maxNumericValue: 100000 }],
        'resource-summary:image:size': ['warn', { maxNumericValue: 200000 }],
        'resource-summary:font:size': ['warn', { maxNumericValue: 100000 }],

        // Best practices
        'uses-http2': 'warn',
        'uses-responsive-images': 'warn',
        'offscreen-images': 'warn',
        'modern-image-formats': 'warn',
        'uses-text-compression': 'error',
        'uses-optimized-images': 'warn',
      },
    },

    upload: {
      // Upload results to temporary public storage
      target: 'temporary-public-storage',
    },

    // Server configuration (if using LHCI server)
    // server: {
    //   baseURL: 'https://your-lhci-server.com',
    //   token: process.env.LHCI_SERVER_TOKEN,
    // },
  },
};
