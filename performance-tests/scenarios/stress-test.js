// Stress Test - Test system beyond normal operational capacity
// Usage: k6 run scenarios/stress-test.js

import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { config, getApiUrl } from '../config.js';
import {
  checkResponse,
  getRandomFutureDate,
  getRandomLocation,
  randomSleep
} from '../utils/helpers.js';

export const options = {
  stages: config.stages.stress,
  thresholds: {
    'http_req_failed': ['rate<0.05'], // Allow 5% failure rate in stress test
    'http_req_duration': ['p(95)<2000'], // 2s P95 acceptable under stress
    'http_req_duration': ['p(99)<5000'], // 5s P99 acceptable under stress
  },
  tags: {
    test_type: 'stress'
  }
};

export default function () {
  const apiUrl = getApiUrl();

  group('Stress Test - Concurrent Operations', function () {
    // Parallel requests to different endpoints
    const requests = [
      ['GET', `${apiUrl}/vehicles`, null, { tags: { endpoint: 'vehicles' } }],
      ['GET', `${apiUrl}/locations`, null, { tags: { endpoint: 'locations' } }],
      ['GET', `${apiUrl}/vehicles/categories`, null, { tags: { endpoint: 'categories' } }],
    ];

    const responses = http.batch(requests);

    responses.forEach((response, index) => {
      check(response, {
        'status is 200 or 503': (r) => r.status === 200 || r.status === 503,
      });
    });

    sleep(0.5);

    // Heavy search query
    const pickupDate = getRandomFutureDate(1, 60);
    const returnDate = getRandomFutureDate(8, 65);
    const location = getRandomLocation();

    const searchResponse = http.get(
      `${apiUrl}/vehicles/availability?` +
      `pickupDate=${pickupDate}&` +
      `returnDate=${returnDate}&` +
      `pickupLocation=${location}&` +
      `returnLocation=${location}`,
      {
        tags: { endpoint: 'availability', load: 'heavy' }
      }
    );

    check(searchResponse, {
      'search completed': (r) => r.status === 200 || r.status === 503,
      'search duration acceptable': (r) => r.timings.duration < 3000,
    });

    randomSleep(0.1, 0.5); // Minimal sleep to maintain pressure
  });
}

export function handleSummary(data) {
  console.log('\n=== STRESS TEST SUMMARY ===');
  console.log(`Test Duration: ${(data.state.testRunDurationMs / 1000 / 60).toFixed(1)} minutes`);
  console.log(`Total Requests: ${data.metrics.http_reqs.values.count}`);
  console.log(`Requests/sec: ${data.metrics.http_reqs.values.rate.toFixed(2)}`);
  console.log(`Failed Requests: ${(data.metrics.http_req_failed.values.rate * 100).toFixed(2)}%`);
  console.log(`\nResponse Times:`);
  console.log(`  Avg: ${data.metrics.http_req_duration.values.avg.toFixed(2)}ms`);
  console.log(`  P95: ${data.metrics.http_req_duration.values['p(95)'].toFixed(2)}ms`);
  console.log(`  P99: ${data.metrics.http_req_duration.values['p(99)'].toFixed(2)}ms`);
  console.log(`  Max: ${data.metrics.http_req_duration.values.max.toFixed(2)}ms`);

  // Check if thresholds passed
  const thresholdsPassed = data.metrics.http_req_failed.values.rate < 0.05;
  console.log(`\nStress Test: ${thresholdsPassed ? '✅ PASSED' : '❌ FAILED'}`);
  console.log('===========================\n');

  return {
    'performance-tests/reports/stress-test-summary.json': JSON.stringify(data, null, 2),
  };
}
