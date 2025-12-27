// k6 Helper Functions

import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

// Custom metrics
export const errorRate = new Rate('errors');

// Random sleep between min and max seconds
export function randomSleep(min = 1, max = 3) {
  sleep(Math.random() * (max - min) + min);
}

// Generate random date in the future (for bookings)
export function getRandomFutureDate(minDays = 7, maxDays = 60) {
  const today = new Date();
  const daysAhead = Math.floor(Math.random() * (maxDays - minDays)) + minDays;
  const futureDate = new Date(today);
  futureDate.setDate(today.getDate() + daysAhead);
  return futureDate.toISOString().split('T')[0];
}

// Generate random booking duration (1-14 days)
export function getRandomDuration(min = 1, max = 14) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

// Random location code
export function getRandomLocation() {
  const locations = ['MUC', 'BER', 'FRA', 'HAM', 'DUS', 'CGN'];
  return locations[Math.floor(Math.random() * locations.length)];
}

// Random vehicle category
export function getRandomCategory() {
  const categories = ['COMPACT', 'MIDSIZE', 'SUV', 'LUXURY', 'VAN'];
  return categories[Math.floor(Math.random() * categories.length)];
}

// Check response and log errors
export function checkResponse(response, name, expectedStatus = 200) {
  const checkResult = check(response, {
    [`${name}: status is ${expectedStatus}`]: (r) => r.status === expectedStatus,
    [`${name}: response time < 1s`]: (r) => r.timings.duration < 1000,
    [`${name}: has valid JSON`]: (r) => {
      try {
        JSON.parse(r.body);
        return true;
      } catch (e) {
        return false;
      }
    },
  });

  if (!checkResult) {
    errorRate.add(1);
    console.error(`Check failed for ${name}:`, {
      status: response.status,
      duration: response.timings.duration,
      body: response.body.substring(0, 200)
    });
  } else {
    errorRate.add(0);
  }

  return checkResult;
}

// Parse JSON response safely
export function parseJson(response) {
  try {
    return JSON.parse(response.body);
  } catch (e) {
    console.error('Failed to parse JSON:', response.body);
    return null;
  }
}

// Generate random customer data
export function generateCustomerData() {
  const firstNames = ['Max', 'Anna', 'Thomas', 'Sarah', 'Michael', 'Lisa'];
  const lastNames = ['Müller', 'Schmidt', 'Weber', 'Wagner', 'Becker', 'Hoffmann'];

  const firstName = firstNames[Math.floor(Math.random() * firstNames.length)];
  const lastName = lastNames[Math.floor(Math.random() * lastNames.length)];
  const timestamp = Date.now();

  return {
    firstName: firstName,
    lastName: lastName,
    email: `perf.${firstName.toLowerCase()}.${timestamp}@orange-rental.de`,
    dateOfBirth: '1990-01-15',
    phone: '+49 151 12345678',
    address: {
      street: 'Teststraße',
      houseNumber: '123',
      postalCode: '80333',
      city: 'München',
      country: 'Deutschland'
    },
    driversLicense: {
      number: `B${timestamp}`,
      issueDate: '2010-01-01',
      expiryDate: '2030-01-01',
      issuingCountry: 'Deutschland'
    }
  };
}

// Format duration for display
export function formatDuration(ms) {
  if (ms < 1000) return `${ms.toFixed(0)}ms`;
  if (ms < 60000) return `${(ms / 1000).toFixed(2)}s`;
  return `${(ms / 60000).toFixed(2)}m`;
}

// Log test summary
export function logSummary(data) {
  console.log('=== Test Summary ===');
  console.log(`Total Requests: ${data.http_reqs.count}`);
  console.log(`Failed Requests: ${data.http_req_failed.rate * 100}%`);
  console.log(`Avg Response Time: ${formatDuration(data.http_req_duration.avg)}`);
  console.log(`P95 Response Time: ${formatDuration(data.http_req_duration['p(95)'])}`);
  console.log(`P99 Response Time: ${formatDuration(data.http_req_duration['p(99)'])}`);
  console.log(`Requests/sec: ${data.http_reqs.rate.toFixed(2)}`);
  console.log('===================');
}

export default {
  randomSleep,
  getRandomFutureDate,
  getRandomDuration,
  getRandomLocation,
  getRandomCategory,
  checkResponse,
  parseJson,
  generateCustomerData,
  formatDuration,
  logSummary,
  errorRate
};
