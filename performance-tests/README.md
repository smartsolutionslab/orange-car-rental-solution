# Performance Testing - Orange Car Rental

## Overview

This directory contains performance and load testing infrastructure for the Orange Car Rental system using [k6](https://k6.io/), a modern load testing tool.

## Test Types

### 1. Smoke Test (`smoke-test.js`)
**Purpose:** Verify that the system can handle minimal load

- **Duration:** 2 minutes
- **Virtual Users:** 1-5
- **When to run:** Before deploying to production, after every deployment
- **Success criteria:** All endpoints respond successfully

```bash
npm run test:smoke
```

### 2. Load Test (`load-test.js`)
**Purpose:** Test system under expected normal load

- **Duration:** 16 minutes
- **Virtual Users:** Ramps from 0 → 50 → 100 → 0
- **When to run:** Weekly on staging, before major releases
- **Success criteria:**
  - < 1% error rate
  - P95 response time < 500ms
  - System stable throughout test

```bash
npm run test:load
```

### 3. Stress Test (`stress-test.js`)
**Purpose:** Test system beyond normal operational capacity

- **Duration:** 26 minutes
- **Virtual Users:** Ramps from 0 → 100 → 200 → 300 → 0
- **When to run:** Before capacity planning, quarterly
- **Success criteria:**
  - < 5% error rate
  - P95 response time < 2s
  - System recovers after stress

```bash
npm run test:stress
```

### 4. Spike Test (`spike-test.js`)
**Purpose:** Test system response to sudden traffic spike

- **Duration:** 6.5 minutes
- **Virtual Users:** 50 → 500 (spike) → 50 → 0
- **When to run:** Before major marketing campaigns, quarterly
- **Success criteria:**
  - < 10% error rate during spike
  - System doesn't crash
  - System recovers after spike

```bash
npm run test:spike
```

### 5. Booking Flow Test (`booking-flow-test.js`)
**Purpose:** Test complete end-to-end booking performance

- **Duration:** 16 minutes
- **Virtual Users:** Ramps from 0 → 20 → 50 → 0
- **When to run:** Weekly, after changes to booking logic
- **Success criteria:**
  - > 95% booking success rate
  - Complete booking flow < 5s (P95)
  - < 1% error rate

```bash
npm run test:booking-flow
```

### 6. Vehicles API Test (`vehicles-api-test.js`)
**Purpose:** Focused testing of Vehicles API performance

- **Duration:** 9 minutes
- **Virtual Users:** Ramps from 0 → 30 → 60 → 0
- **When to run:** After changes to Vehicles API
- **Success criteria:**
  - Availability checks < 500ms (P95)
  - Vehicle listing < 300ms (P95)
  - < 1% error rate

```bash
npm run test:vehicles
```

## Installation

### Prerequisites

**Install k6:**

```bash
# macOS
brew install k6

# Windows (using Chocolatey)
choco install k6

# Linux (Debian/Ubuntu)
sudo gpg -k
sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update
sudo apt-get install k6
```

Verify installation:
```bash
k6 version
```

### Project Setup

```bash
cd performance-tests
npm install  # Optional: for package.json scripts
```

## Running Tests

### Local Development

Test against local environment:
```bash
npm run test:smoke
npm run test:load
npm run test:vehicles
```

### Staging Environment

Test against staging:
```bash
ENVIRONMENT=staging npm run test:smoke
ENVIRONMENT=staging npm run test:load
```

### Production Environment

**⚠️ WARNING:** Only run smoke tests on production!

```bash
ENVIRONMENT=production npm run test:smoke
```

### Run All Tests

```bash
npm run test:all
```

## CI/CD Integration

### GitHub Actions

Performance tests run automatically:

**Scheduled (Weekly):**
- Every Monday at 2 AM UTC
- Runs load test on staging
- Results saved as artifacts

**Manual Trigger:**
```
GitHub → Actions → Performance Tests → Run workflow
```

**On Pull Request:**
Add `perf-test` label to PR to trigger smoke test

### View Results

1. Go to GitHub Actions
2. Select "Performance Tests" workflow
3. Click on the run
4. View summary in "Run performance test" step
5. Download artifacts for detailed JSON results

## Configuration

### Environment URLs

Edit `config.js` to update environment URLs:

```javascript
environments: {
  local: 'http://localhost:4300',
  dev: 'https://dev.orange-rental.de',
  staging: 'https://staging.orange-rental.de',
  production: 'https://orange-rental.de'
}
```

### Performance Thresholds (SLOs)

Default thresholds in `config.js`:

```javascript
thresholds: {
  http_req_duration: ['p(95)<500', 'p(99)<1000'],
  http_req_failed: ['rate<0.01'],
  http_reqs: ['rate>100'],
  checks: ['rate>0.99']
}
```

## Test Scenarios

### Vehicle Search Flow (60% of users)
1. Get locations
2. Search for available vehicles
3. Filter by category (50% of users)
4. Review results

### Vehicle Details Flow (30% of users)
1. Browse vehicle list
2. View vehicle details
3. Review specifications

### Browse Categories Flow (10% of users)
1. Get all categories
2. Filter vehicles by category
3. Browse filtered results

## Performance Metrics

### Key Metrics Tracked

**HTTP Metrics:**
- `http_reqs` - Total number of HTTP requests
- `http_req_duration` - Request duration (avg, min, max, P95, P99)
- `http_req_failed` - Failed request rate
- `http_req_blocked` - Time spent blocked before request
- `http_req_connecting` - Time spent establishing TCP connection
- `http_req_receiving` - Time spent receiving response
- `http_req_sending` - Time spent sending request

**Custom Metrics:**
- `vehicle_searches` - Total vehicle searches
- `availability_checks` - Total availability checks
- `booking_attempts` - Total booking attempts
- `booking_flow_completed` - Successful bookings
- `booking_flow_duration` - Complete booking flow duration

### Reading Results

```bash
# View summary
cat performance-tests/reports/load-test-summary.json | jq '.metrics.http_req_duration'

# Get P95 response time
cat performance-tests/reports/load-test-summary.json | jq '.metrics.http_req_duration.values["p(95)"]'

# Get error rate
cat performance-tests/reports/load-test-summary.json | jq '.metrics.http_req_failed.values.rate'
```

## Performance Benchmarks

### Target Performance (SLOs)

| Endpoint | P95 Response Time | P99 Response Time | Error Rate |
|----------|-------------------|-------------------|------------|
| Frontend Homepage | < 200ms | < 500ms | < 0.1% |
| Vehicle List | < 300ms | < 600ms | < 0.5% |
| Vehicle Details | < 250ms | < 500ms | < 0.5% |
| Availability Check | < 500ms | < 1000ms | < 1% |
| Create Booking | < 800ms | < 1500ms | < 1% |
| Confirm Booking | < 600ms | < 1200ms | < 1% |

### Load Capacity

| Environment | Concurrent Users | Requests/sec | Notes |
|-------------|------------------|--------------|-------|
| Development | 10 | 20 | Single replica, minimal resources |
| Staging | 50 | 100 | 2 replicas per service |
| Production | 500 | 1000+ | Auto-scaling, 3-20 replicas |

## Troubleshooting

### High Error Rate

```bash
# Check which endpoints are failing
k6 run scenarios/load-test.js --out json=results.json
jq '.metrics | to_entries[] | select(.key | contains("failed"))' results.json
```

**Common causes:**
- Database connection pool exhausted
- Memory limits reached
- Network timeout
- Rate limiting triggered

### Slow Response Times

```bash
# Identify slow endpoints
jq '.metrics | to_entries[] | select(.key | contains("duration"))' results.json
```

**Common causes:**
- Inefficient database queries
- Missing indexes
- N+1 query problems
- Large response payloads
- Cold start issues

### Test Fails to Start

**Check prerequisites:**
```bash
k6 version
curl https://staging.orange-rental.de/health
```

**Check environment variables:**
```bash
echo $ENVIRONMENT
```

## Best Practices

### DO:
- ✅ Run smoke tests before every deployment
- ✅ Run load tests weekly on staging
- ✅ Gradually ramp up virtual users
- ✅ Monitor system metrics during tests
- ✅ Test realistic user scenarios
- ✅ Include think time (sleep) between requests
- ✅ Randomize test data

### DON'T:
- ❌ Run stress tests on production
- ❌ Run tests during business hours (production)
- ❌ Use production data in tests
- ❌ Ignore test failures
- ❌ Test without monitoring
- ❌ Spike to maximum load instantly
- ❌ Hardcode test data

## Advanced Usage

### Custom Test Duration

```bash
k6 run --duration 30m scenarios/load-test.js
```

### Custom Virtual Users

```bash
k6 run --vus 100 scenarios/load-test.js
```

### k6 Cloud (Optional)

Upload results to k6 Cloud for advanced analytics:

```bash
k6 login cloud
k6 cloud run scenarios/load-test.js
```

### Custom Thresholds

Override thresholds via command line:

```bash
k6 run scenarios/load-test.js \
  --threshold "http_req_duration=p(95)<300" \
  --threshold "http_req_failed=rate<0.001"
```

## Reports

Test results are saved to `performance-tests/reports/`:

- `*-summary.json` - Test summary with all metrics
- `*-results.json` - Detailed per-request results (if enabled)

**Retention:**
- Local: Keep for analysis
- CI/CD: 30 days in GitHub Artifacts

## Support

### Resources

- [k6 Documentation](https://k6.io/docs/)
- [k6 Examples](https://k6.io/docs/examples/)
- [k6 Community](https://community.k6.io/)

### Internal Support

- **DevOps Team:** devops@orange-rental.de
- **Performance Issues:** Create GitHub issue with `performance` label
- **Slack:** #orange-car-rental-perf

## Contributing

When adding new performance tests:

1. Create test in `scenarios/` directory
2. Follow existing naming convention
3. Add npm script to `package.json`
4. Include custom metrics for tracking
5. Set appropriate thresholds
6. Document test purpose and criteria
7. Update this README

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2024-01-21 | Initial performance testing infrastructure |

---

**Last Updated:** 2025-12-27
**Maintained by:** DevOps Team
**Contact:** devops@orange-rental.de
