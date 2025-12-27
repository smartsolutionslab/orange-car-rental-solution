// Load Test - Test system under expected normal load
// Usage: k6 run scenarios/load-test.js
// With environment: ENVIRONMENT=staging k6 run scenarios/load-test.js

import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { Counter, Trend } from 'k6/metrics';
import { config, getApiUrl, getBaseUrl } from '../config.js';
import {
  checkResponse,
  randomSleep,
  getRandomFutureDate,
  getRandomDuration,
  getRandomLocation,
  getRandomCategory
} from '../utils/helpers.js';

// Custom metrics
const vehicleSearches = new Counter('vehicle_searches');
const bookingAttempts = new Counter('booking_attempts');
const searchDuration = new Trend('search_duration');

export const options = {
  stages: config.stages.load,
  thresholds: {
    ...config.thresholds,
    'http_req_duration{scenario:search}': ['p(95)<400'],
    'http_req_duration{scenario:booking}': ['p(95)<800'],
  },
  tags: {
    test_type: 'load'
  }
};

export default function () {
  const baseUrl = getBaseUrl();
  const apiUrl = getApiUrl();

  // Scenario 1: Vehicle Search (60% of users)
  if (Math.random() < 0.6) {
    group('Vehicle Search Flow', function () {
      // Step 1: Get locations
      let response = http.get(`${apiUrl}/locations`, {
        tags: { scenario: 'search', endpoint: 'locations' }
      });
      checkResponse(response, 'Get Locations');

      sleep(1);

      // Step 2: Search for vehicles
      const pickupDate = getRandomFutureDate(7, 30);
      const returnDate = getRandomFutureDate(
        parseInt(pickupDate.split('-')[2]) + getRandomDuration(1, 7),
        parseInt(pickupDate.split('-')[2]) + getRandomDuration(8, 14)
      );
      const location = getRandomLocation();

      const searchStart = Date.now();
      response = http.get(
        `${apiUrl}/vehicles/availability?` +
        `pickupDate=${pickupDate}&` +
        `returnDate=${returnDate}&` +
        `pickupLocation=${location}&` +
        `returnLocation=${location}`,
        {
          tags: { scenario: 'search', endpoint: 'vehicles' }
        }
      );

      const duration = Date.now() - searchStart;
      searchDuration.add(duration);
      vehicleSearches.add(1);

      checkResponse(response, 'Search Vehicles');

      sleep(2);

      // Step 3: Filter by category (optional)
      if (Math.random() < 0.5) {
        const category = getRandomCategory();
        response = http.get(
          `${apiUrl}/vehicles/availability?` +
          `pickupDate=${pickupDate}&` +
          `returnDate=${returnDate}&` +
          `pickupLocation=${location}&` +
          `returnLocation=${location}&` +
          `category=${category}`,
          {
            tags: { scenario: 'search', endpoint: 'vehicles' }
          }
        );
        checkResponse(response, 'Filter by Category');
      }

      randomSleep(2, 5);
    });
  }

  // Scenario 2: View Vehicle Details (30% of users)
  if (Math.random() < 0.3) {
    group('Vehicle Details Flow', function () {
      // Get a random vehicle
      let response = http.get(`${apiUrl}/vehicles`, {
        tags: { scenario: 'details', endpoint: 'vehicles' }
      });
      checkResponse(response, 'Get Vehicles');

      if (response.status === 200) {
        const vehicles = JSON.parse(response.body);
        if (vehicles.length > 0) {
          const randomVehicle = vehicles[Math.floor(Math.random() * vehicles.length)];

          sleep(1);

          // Get vehicle details
          response = http.get(`${apiUrl}/vehicles/${randomVehicle.id}`, {
            tags: { scenario: 'details', endpoint: 'vehicles' }
          });
          checkResponse(response, 'Get Vehicle Details');
        }
      }

      randomSleep(3, 6);
    });
  }

  // Scenario 3: Browse Categories (10% of users)
  if (Math.random() < 0.1) {
    group('Browse Categories Flow', function () {
      // Get categories
      let response = http.get(`${apiUrl}/vehicles/categories`, {
        tags: { scenario: 'browse', endpoint: 'vehicles' }
      });
      checkResponse(response, 'Get Categories');

      sleep(2);

      // Get vehicles by category
      if (response.status === 200) {
        const categories = JSON.parse(response.body);
        if (categories.length > 0) {
          const randomCategory = categories[Math.floor(Math.random() * categories.length)];

          response = http.get(
            `${apiUrl}/vehicles?category=${randomCategory.code}`,
            {
              tags: { scenario: 'browse', endpoint: 'vehicles' }
            }
          );
          checkResponse(response, 'Get Vehicles by Category');
        }
      }

      randomSleep(2, 4);
    });
  }

  randomSleep(1, 3);
}

export function handleSummary(data) {
  console.log('\n=== LOAD TEST SUMMARY ===');
  console.log(`Test Duration: ${(data.state.testRunDurationMs / 1000).toFixed(0)}s`);
  console.log(`Total Requests: ${data.metrics.http_reqs.values.count}`);
  console.log(`Requests/sec: ${data.metrics.http_reqs.values.rate.toFixed(2)}`);
  console.log(`Failed Requests: ${(data.metrics.http_req_failed.values.rate * 100).toFixed(2)}%`);
  console.log(`\nResponse Times:`);
  console.log(`  Avg: ${data.metrics.http_req_duration.values.avg.toFixed(2)}ms`);
  console.log(`  Min: ${data.metrics.http_req_duration.values.min.toFixed(2)}ms`);
  console.log(`  Max: ${data.metrics.http_req_duration.values.max.toFixed(2)}ms`);
  console.log(`  P50: ${data.metrics.http_req_duration.values.med.toFixed(2)}ms`);
  console.log(`  P95: ${data.metrics.http_req_duration.values['p(95)'].toFixed(2)}ms`);
  console.log(`  P99: ${data.metrics.http_req_duration.values['p(99)'].toFixed(2)}ms`);
  console.log(`\nCustom Metrics:`);
  console.log(`  Vehicle Searches: ${data.metrics.vehicle_searches.values.count}`);
  console.log(`  Avg Search Duration: ${data.metrics.search_duration?.values.avg?.toFixed(2) || 'N/A'}ms`);
  console.log('=========================\n');

  return {
    'performance-tests/reports/load-test-summary.json': JSON.stringify(data, null, 2),
    'stdout': JSON.stringify(data, null, 2),
  };
}
