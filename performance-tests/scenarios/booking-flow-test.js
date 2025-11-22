// Booking Flow Performance Test
// Tests the complete end-to-end booking flow performance
// Usage: k6 run scenarios/booking-flow-test.js

import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { Counter, Trend } from 'k6/metrics';
import { config, getApiUrl } from '../config.js';
import {
  checkResponse,
  generateCustomerData,
  getRandomFutureDate,
  getRandomDuration,
  getRandomLocation,
  randomSleep
} from '../utils/helpers.js';

// Custom metrics
const bookingFlowCompleted = new Counter('booking_flow_completed');
const bookingFlowFailed = new Counter('booking_flow_failed');
const bookingFlowDuration = new Trend('booking_flow_duration');

export const options = {
  stages: [
    { duration: '2m', target: 20 },  // Ramp up to 20 concurrent bookings
    { duration: '5m', target: 20 },  // Maintain 20 concurrent bookings
    { duration: '2m', target: 50 },  // Increase to 50
    { duration: '5m', target: 50 },  // Maintain 50
    { duration: '2m', target: 0 },   // Ramp down
  ],
  thresholds: {
    'http_req_duration': ['p(95)<1000'],
    'http_req_failed': ['rate<0.01'],
    'booking_flow_duration': ['p(95)<5000'], // Complete booking flow in < 5s
  },
  tags: {
    test_type: 'booking_flow'
  }
};

export default function () {
  const apiUrl = getApiUrl();
  const flowStart = Date.now();

  group('Complete Booking Flow', function () {
    let vehicleId = null;
    let bookingId = null;

    // Step 1: Search for available vehicles
    group('1. Search Vehicles', function () {
      const pickupDate = getRandomFutureDate(7, 30);
      const daysCount = getRandomDuration(1, 7);
      const returnDate = new Date(pickupDate);
      returnDate.setDate(returnDate.getDate() + daysCount);
      const location = getRandomLocation();

      const response = http.get(
        `${apiUrl}/vehicles/availability?` +
        `pickupDate=${pickupDate}&` +
        `returnDate=${returnDate.toISOString().split('T')[0]}&` +
        `pickupLocation=${location}&` +
        `returnLocation=${location}`,
        {
          tags: { step: 'search', endpoint: 'vehicles' }
        }
      );

      if (checkResponse(response, 'Search Vehicles')) {
        const vehicles = JSON.parse(response.body);
        if (vehicles.length > 0) {
          vehicleId = vehicles[0].id;
        }
      }

      sleep(2); // User reviews search results
    });

    if (!vehicleId) {
      bookingFlowFailed.add(1);
      return;
    }

    // Step 2: Get vehicle details
    group('2. View Vehicle Details', function () {
      const response = http.get(`${apiUrl}/vehicles/${vehicleId}`, {
        tags: { step: 'details', endpoint: 'vehicles' }
      });

      checkResponse(response, 'Get Vehicle Details');
      sleep(3); // User reviews vehicle details
    });

    // Step 3: Create booking
    group('3. Create Booking', function () {
      const customerData = generateCustomerData();
      const pickupDate = getRandomFutureDate(7, 30);
      const returnDate = new Date(pickupDate);
      returnDate.setDate(returnDate.getDate() + getRandomDuration(1, 7));
      const location = getRandomLocation();

      const bookingPayload = {
        vehicleId: vehicleId,
        pickupDate: pickupDate,
        returnDate: returnDate.toISOString().split('T')[0],
        pickupLocationCode: location,
        returnLocationCode: location,
        customer: customerData,
        extras: []
      };

      const response = http.post(
        `${apiUrl}/reservations`,
        JSON.stringify(bookingPayload),
        {
          headers: { 'Content-Type': 'application/json' },
          tags: { step: 'create', endpoint: 'reservations' }
        }
      );

      if (checkResponse(response, 'Create Booking', 201)) {
        const booking = JSON.parse(response.body);
        bookingId = booking.id;
      }

      sleep(1);
    });

    if (!bookingId) {
      bookingFlowFailed.add(1);
      return;
    }

    // Step 4: Confirm booking
    group('4. Confirm Booking', function () {
      const response = http.post(
        `${apiUrl}/reservations/${bookingId}/confirm`,
        null,
        {
          tags: { step: 'confirm', endpoint: 'reservations' }
        }
      );

      if (checkResponse(response, 'Confirm Booking')) {
        bookingFlowCompleted.add(1);

        const flowDuration = Date.now() - flowStart;
        bookingFlowDuration.add(flowDuration);
      } else {
        bookingFlowFailed.add(1);
      }
    });

    randomSleep(3, 6);
  });
}

export function handleSummary(data) {
  const completedCount = data.metrics.booking_flow_completed?.values.count || 0;
  const failedCount = data.metrics.booking_flow_failed?.values.count || 0;
  const totalAttempts = completedCount + failedCount;
  const successRate = totalAttempts > 0 ? (completedCount / totalAttempts * 100) : 0;

  console.log('\n=== BOOKING FLOW TEST SUMMARY ===');
  console.log(`Test Duration: ${(data.state.testRunDurationMs / 1000 / 60).toFixed(1)} minutes`);
  console.log(`\nBooking Flow Statistics:`);
  console.log(`  Completed Bookings: ${completedCount}`);
  console.log(`  Failed Bookings: ${failedCount}`);
  console.log(`  Success Rate: ${successRate.toFixed(2)}%`);
  console.log(`\nBooking Flow Duration:`);
  console.log(`  Avg: ${data.metrics.booking_flow_duration?.values.avg?.toFixed(2) || 'N/A'}ms`);
  console.log(`  P95: ${data.metrics.booking_flow_duration?.values['p(95)']?.toFixed(2) || 'N/A'}ms`);
  console.log(`  P99: ${data.metrics.booking_flow_duration?.values['p(99)']?.toFixed(2) || 'N/A'}ms`);
  console.log(`\nHTTP Metrics:`);
  console.log(`  Total Requests: ${data.metrics.http_reqs.values.count}`);
  console.log(`  Failed Requests: ${(data.metrics.http_req_failed.values.rate * 100).toFixed(2)}%`);
  console.log(`  Avg Response Time: ${data.metrics.http_req_duration.values.avg.toFixed(2)}ms`);
  console.log(`  P95 Response Time: ${data.metrics.http_req_duration.values['p(95)'].toFixed(2)}ms`);

  const passed = successRate > 95 && data.metrics.http_req_failed.values.rate < 0.01;
  console.log(`\nBooking Flow Test: ${passed ? '✅ PASSED' : '❌ FAILED'}`);
  console.log('=================================\n');

  return {
    'performance-tests/reports/booking-flow-summary.json': JSON.stringify(data, null, 2),
  };
}
