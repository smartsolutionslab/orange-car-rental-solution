// Karma configuration file
// Using Puppeteer's bundled Chromium for headless testing (unless CHROME_BIN is already set)

// Only use puppeteer's Chrome if CHROME_BIN isn't already set (e.g., in CI)
if (!process.env.CHROME_BIN) {
  try {
    const puppeteer = require('puppeteer');
    process.env.CHROME_BIN = puppeteer.executablePath();
  } catch {
    // puppeteer not available, rely on system Chrome
  }
}

module.exports = function (config) {
  config.set({
    basePath: '',
    frameworks: ['jasmine'],
    plugins: [
      require('karma-jasmine'),
      require('karma-chrome-launcher'),
      require('karma-jasmine-html-reporter'),
      require('karma-coverage')
    ],
    client: {
      jasmine: {
        // Jasmine configuration options
      },
      clearContext: false // leave Jasmine Spec Runner output visible in browser
    },
    jasmineHtmlReporter: {
      suppressAll: true // removes the duplicated traces
    },
    coverageReporter: {
      dir: require('path').join(__dirname, './coverage/public-portal'),
      subdir: '.',
      reporters: [
        { type: 'html' },
        { type: 'text-summary' },
        { type: 'lcovonly' }
      ]
    },
    reporters: ['progress', 'kjhtml'],
    browsers: ['ChromeHeadlessNoSandbox'],
    customLaunchers: {
      ChromeHeadlessNoSandbox: {
        base: 'ChromeHeadless',
        flags: [
          '--no-sandbox',
          '--disable-gpu',
          '--disable-dev-shm-usage',
          '--disable-software-rasterizer',
          '--disable-extensions'
        ]
      }
    },
    restartOnFileChange: true,
    singleRun: false,
    browserNoActivityTimeout: 60000
  });
};
