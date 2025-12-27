// Spike Test - Test system response to sudden traffic spike
// Usage: k6 run scenarios/spike-test.js

import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter } from 'k6/metrics';
import { config, getApiUrl } from '../config.js';
import { checkResponse, getRandomFutureDate, getRandomLocation } from '../utils/helpers.js';

const spikeFailures = new Counter('spike_failures');

export const options = {
  stages: config.stages.spike,
  thresholds: {
    'http_req_failed': ['rate<0.1'], // Allow 10% failure during spike
    'http_req_duration': ['p(99)<3000'], // 3s P99 during spike
  },
  tags: {
    test_type: 'spike'
  }
};

export default function () {
  const apiUrl = getApiUrl();

  // Simulate typical user behavior during spike
  const pickupDate = getRandomFutureDate(7, 30);
  const returnDate = getRandomFutureDate(10, 35);
  const location = getRandomLocation();

  const response = http.get(
    `${apiUrl}/vehicles/availability?` +
    `pickupDate=${pickupDate}&` +
    `returnDate=${returnDate}&` +
    `pickupLocation=${location}&` +
    `returnLocation=${location}`,
    {
      tags: { scenario: 'spike', endpoint: 'availability' }
    }
  );

  const success = check(response, {
    'spike: status ok': (r) => r.status === 200,
    'spike: response time acceptable': (r) => r.timings.duration < 5000,
  });

  if (!success) {
    spikeFailures.add(1);
  }

  sleep(0.1); // Minimal sleep during spike
}

export function handleSummary(data) {
  console.log('\n=== SPIKE TEST SUMMARY ===');
  console.log(`Test Duration: ${(data.state.testRunDurationMs / 1000 / 60).toFixed(1)} minutes`);
  console.log(`Peak VUs: 500`);
  console.log(`Total Requests: ${data.metrics.http_reqs.values.count}`);
  console.log(`Peak Requests/sec: ${data.metrics.http_reqs.values.rate.toFixed(2)}`);
  console.log(`Failed Requests: ${(data.metrics.http_req_failed.values.rate * 100).toFixed(2)}%`);
  console.log(`Spike Failures: ${data.metrics.spike_failures?.values.count || 0}`);
  console.log(`\nResponse Times During Spike:`);
  console.log(`  Avg: ${data.metrics.http_req_duration.values.avg.toFixed(2)}ms`);
  console.log(`  P95: ${data.metrics.http_req_duration.values['p(95)'].toFixed(2)}ms`);
  console.log(`  P99: ${data.metrics.http_req_duration.values['p(99)'].toFixed(2)}ms`);
  console.log(`  Max: ${data.metrics.http_req_duration.values.max.toFixed(2)}ms`);

  const passed = data.metrics.http_req_failed.values.rate < 0.1;
  console.log(`\nSpike Test: ${passed ? '✅ PASSED - System handled spike' : '❌ FAILED - System degraded'}`);
  console.log('==========================\n');

  return {
    'performance-tests/reports/spike-test-summary.json': JSON.stringify(data, null, 2),
  };
}
