// Smoke Test - Verify basic functionality under minimal load
// Usage: k6 run scenarios/smoke-test.js

import http from 'k6/http';
import { check, sleep } from 'k6';
import { config, getApiUrl, getBaseUrl } from '../config.js';
import { checkResponse, randomSleep } from '../utils/helpers.js';

export const options = {
  stages: config.stages.smoke,
  thresholds: config.thresholds,
  tags: {
    test_type: 'smoke'
  }
};

export default function () {
  const baseUrl = getBaseUrl();
  const apiUrl = getApiUrl();

  // Test 1: Frontend loads
  let response = http.get(baseUrl, {
    tags: { endpoint: 'frontend' }
  });
  checkResponse(response, 'Frontend Homepage');

  sleep(1);

  // Test 2: Vehicles API - Get all vehicles
  response = http.get(`${apiUrl}/vehicles`, {
    tags: { endpoint: 'vehicles' }
  });
  checkResponse(response, 'Get Vehicles');

  sleep(1);

  // Test 3: Locations API - Get all locations
  response = http.get(`${apiUrl}/locations`, {
    tags: { endpoint: 'locations' }
  });
  checkResponse(response, 'Get Locations');

  sleep(1);

  // Test 4: Vehicle Categories
  response = http.get(`${apiUrl}/vehicles/categories`, {
    tags: { endpoint: 'vehicles' }
  });
  checkResponse(response, 'Get Vehicle Categories');

  sleep(1);

  // Test 5: Check availability
  const pickupDate = new Date();
  pickupDate.setDate(pickupDate.getDate() + 7);
  const returnDate = new Date(pickupDate);
  returnDate.setDate(returnDate.getDate() + 3);

  response = http.get(
    `${apiUrl}/vehicles/availability?` +
    `pickupDate=${pickupDate.toISOString().split('T')[0]}&` +
    `returnDate=${returnDate.toISOString().split('T')[0]}&` +
    `pickupLocation=MUC&` +
    `returnLocation=MUC`,
    {
      tags: { endpoint: 'vehicles' }
    }
  );
  checkResponse(response, 'Check Availability');

  randomSleep(2, 4);
}

export function handleSummary(data) {
  console.log('\n=== SMOKE TEST SUMMARY ===');
  console.log(`Test Duration: ${data.state.testRunDurationMs / 1000}s`);
  console.log(`Total Requests: ${data.metrics.http_reqs.values.count}`);
  console.log(`Failed Requests: ${(data.metrics.http_req_failed.values.rate * 100).toFixed(2)}%`);
  console.log(`Avg Response Time: ${data.metrics.http_req_duration.values.avg.toFixed(2)}ms`);
  console.log(`P95 Response Time: ${data.metrics.http_req_duration.values['p(95)'].toFixed(2)}ms`);
  console.log(`P99 Response Time: ${data.metrics.http_req_duration.values['p(99)'].toFixed(2)}ms`);
  console.log('==========================\n');

  return {
    'performance-tests/reports/smoke-test-summary.json': JSON.stringify(data, null, 2),
  };
}
