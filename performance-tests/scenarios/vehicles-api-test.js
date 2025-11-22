// Vehicles API Specific Performance Test
// Usage: k6 run scenarios/vehicles-api-test.js

import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { Counter, Trend } from 'k6/metrics';
import { config, getApiUrl } from '../config.js';
import {
  checkResponse,
  getRandomFutureDate,
  getRandomLocation,
  getRandomCategory,
  randomSleep
} from '../utils/helpers.js';

const vehicleSearches = new Counter('vehicle_searches');
const availabilityChecks = new Counter('availability_checks');
const availabilityCheckDuration = new Trend('availability_check_duration');

export const options = {
  stages: [
    { duration: '1m', target: 30 },
    { duration: '3m', target: 30 },
    { duration: '1m', target: 60 },
    { duration: '3m', target: 60 },
    { duration: '1m', target: 0 },
  ],
  thresholds: {
    'http_req_duration{endpoint:vehicles}': ['p(95)<300'],
    'http_req_duration{endpoint:availability}': ['p(95)<500'],
    'http_req_failed': ['rate<0.01'],
  },
  tags: {
    test_type: 'api_specific',
    api: 'vehicles'
  }
};

export default function () {
  const apiUrl = getApiUrl();

  // Test 1: Get all vehicles
  group('Get All Vehicles', function () {
    const response = http.get(`${apiUrl}/vehicles`, {
      tags: { endpoint: 'vehicles', operation: 'list' }
    });

    checkResponse(response, 'List Vehicles');
    vehicleSearches.add(1);
    sleep(1);
  });

  // Test 2: Get vehicle by ID
  group('Get Vehicle by ID', function () {
    // First get all vehicles
    const listResponse = http.get(`${apiUrl}/vehicles`, {
      tags: { endpoint: 'vehicles' }
    });

    if (listResponse.status === 200) {
      const vehicles = JSON.parse(listResponse.body);
      if (vehicles.length > 0) {
        const randomVehicle = vehicles[Math.floor(Math.random() * vehicles.length)];

        sleep(0.5);

        const response = http.get(`${apiUrl}/vehicles/${randomVehicle.id}`, {
          tags: { endpoint: 'vehicles', operation: 'get' }
        });

        checkResponse(response, 'Get Vehicle by ID');
      }
    }

    sleep(1);
  });

  // Test 3: Get vehicle categories
  group('Get Categories', function () {
    const response = http.get(`${apiUrl}/vehicles/categories`, {
      tags: { endpoint: 'vehicles', operation: 'categories' }
    });

    checkResponse(response, 'Get Categories');
    sleep(1);
  });

  // Test 4: Filter vehicles by category
  group('Filter by Category', function () {
    const category = getRandomCategory();
    const response = http.get(`${apiUrl}/vehicles?category=${category}`, {
      tags: { endpoint: 'vehicles', operation: 'filter' }
    });

    checkResponse(response, 'Filter by Category');
    sleep(1);
  });

  // Test 5: Check availability (most critical endpoint)
  group('Check Availability', function () {
    const pickupDate = getRandomFutureDate(7, 30);
    const returnDate = getRandomFutureDate(10, 35);
    const location = getRandomLocation();

    const start = Date.now();
    const response = http.get(
      `${apiUrl}/vehicles/availability?` +
      `pickupDate=${pickupDate}&` +
      `returnDate=${returnDate}&` +
      `pickupLocation=${location}&` +
      `returnLocation=${location}`,
      {
        tags: { endpoint: 'availability', operation: 'check' }
      }
    );

    const duration = Date.now() - start;
    availabilityCheckDuration.add(duration);
    availabilityChecks.add(1);

    checkResponse(response, 'Check Availability');
    sleep(2);
  });

  // Test 6: Complex availability check with filters
  group('Complex Availability Query', function () {
    const pickupDate = getRandomFutureDate(7, 30);
    const returnDate = getRandomFutureDate(10, 35);
    const location = getRandomLocation();
    const category = getRandomCategory();

    const response = http.get(
      `${apiUrl}/vehicles/availability?` +
      `pickupDate=${pickupDate}&` +
      `returnDate=${returnDate}&` +
      `pickupLocation=${location}&` +
      `returnLocation=${location}&` +
      `category=${category}&` +
      `transmission=AUTOMATIC&` +
      `minSeats=4`,
      {
        tags: { endpoint: 'availability', operation: 'complex' }
      }
    );

    checkResponse(response, 'Complex Availability Query');
    sleep(2);
  });

  randomSleep(2, 4);
}

export function handleSummary(data) {
  console.log('\n=== VEHICLES API TEST SUMMARY ===');
  console.log(`Test Duration: ${(data.state.testRunDurationMs / 1000 / 60).toFixed(1)} minutes`);
  console.log(`\nAPI Operations:`);
  console.log(`  Vehicle Searches: ${data.metrics.vehicle_searches?.values.count || 0}`);
  console.log(`  Availability Checks: ${data.metrics.availability_checks?.values.count || 0}`);
  console.log(`\nAvailability Check Performance:`);
  console.log(`  Avg: ${data.metrics.availability_check_duration?.values.avg?.toFixed(2) || 'N/A'}ms`);
  console.log(`  P95: ${data.metrics.availability_check_duration?.values['p(95)']?.toFixed(2) || 'N/A'}ms`);
  console.log(`  P99: ${data.metrics.availability_check_duration?.values['p(99)']?.toFixed(2) || 'N/A'}ms`);
  console.log(`\nHTTP Metrics:`);
  console.log(`  Total Requests: ${data.metrics.http_reqs.values.count}`);
  console.log(`  Requests/sec: ${data.metrics.http_reqs.values.rate.toFixed(2)}`);
  console.log(`  Failed Requests: ${(data.metrics.http_req_failed.values.rate * 100).toFixed(2)}%`);
  console.log(`  Avg Response Time: ${data.metrics.http_req_duration.values.avg.toFixed(2)}ms`);
  console.log(`  P95 Response Time: ${data.metrics.http_req_duration.values['p(95)'].toFixed(2)}ms`);

  const passed = data.metrics.http_req_failed.values.rate < 0.01 &&
                 data.metrics.http_req_duration.values['p(95)'] < 500;
  console.log(`\nVehicles API Test: ${passed ? '✅ PASSED' : '❌ FAILED'}`);
  console.log('=================================\n');

  return {
    'performance-tests/reports/vehicles-api-summary.json': JSON.stringify(data, null, 2),
  };
}
