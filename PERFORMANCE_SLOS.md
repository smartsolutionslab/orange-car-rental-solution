# Performance SLOs and Benchmarks

## Service Level Objectives (SLOs)

### Overview

These SLOs define the expected performance characteristics of the Orange Car Rental system. They are measured continuously via performance tests and monitored in production via Application Insights.

## Response Time SLOs

### API Endpoints

| Endpoint | Operation | P50 | P95 | P99 | Target Throughput |
|----------|-----------|-----|-----|-----|-------------------|
| **Vehicles API** |
| GET /vehicles | List all vehicles | < 100ms | < 300ms | < 600ms | 500 req/s |
| GET /vehicles/{id} | Get vehicle details | < 80ms | < 250ms | < 500ms | 300 req/s |
| GET /vehicles/categories | List categories | < 50ms | < 150ms | < 300ms | 200 req/s |
| GET /vehicles/availability | Check availability | < 200ms | < 500ms | < 1000ms | 200 req/s |
| **Reservations API** |
| POST /reservations | Create booking | < 300ms | < 800ms | < 1500ms | 100 req/s |
| POST /reservations/{id}/confirm | Confirm booking | < 250ms | < 600ms | < 1200ms | 100 req/s |
| GET /reservations | List bookings | < 150ms | < 400ms | < 800ms | 150 req/s |
| GET /reservations/{id} | Get booking details | < 100ms | < 300ms | < 600ms | 200 req/s |
| POST /reservations/{id}/cancel | Cancel booking | < 200ms | < 500ms | < 1000ms | 50 req/s |
| **Customers API** |
| POST /customers | Create customer | < 200ms | < 500ms | < 1000ms | 50 req/s |
| GET /customers/{id} | Get customer | < 80ms | < 250ms | < 500ms | 150 req/s |
| PUT /customers/{id} | Update customer | < 150ms | < 400ms | < 800ms | 100 req/s |
| **Locations API** |
| GET /locations | List locations | < 50ms | < 150ms | < 300ms | 300 req/s |
| GET /locations/{code} | Get location | < 40ms | < 120ms | < 250ms | 200 req/s |

### Frontend Pages

| Page | First Contentful Paint | Largest Contentful Paint | Time to Interactive |
|------|------------------------|--------------------------|---------------------|
| Homepage | < 800ms | < 1200ms | < 2000ms |
| Vehicle Search | < 600ms | < 1000ms | < 1800ms |
| Booking Form | < 700ms | < 1100ms | < 2000ms |
| Confirmation Page | < 500ms | < 900ms | < 1500ms |

## Availability SLOs

| Service | Target Availability | Allowed Downtime |
|---------|---------------------|------------------|
| Public Portal | 99.5% | ~3.6 hours/month |
| API Gateway | 99.9% | ~43 minutes/month |
| Vehicles API | 99.9% | ~43 minutes/month |
| Reservations API | 99.9% | ~43 minutes/month |
| Customers API | 99.5% | ~3.6 hours/month |
| Locations API | 99.9% | ~43 minutes/month |
| Database | 99.95% | ~21 minutes/month |
| Keycloak | 99.5% | ~3.6 hours/month |

## Error Rate SLOs

| Operation Type | Target Error Rate | Alert Threshold |
|----------------|-------------------|-----------------|
| HTTP GET requests | < 0.1% | > 0.5% |
| HTTP POST requests | < 1% | > 2% |
| Database queries | < 0.1% | > 0.5% |
| Authentication | < 0.5% | > 1% |
| Payment processing | < 0.01% | > 0.1% |

## Capacity SLOs

### Concurrent Users

| Environment | Target Capacity | Peak Capacity | Notes |
|-------------|----------------|---------------|-------|
| Development | 10 users | 20 users | For testing only |
| Staging | 50 users | 100 users | Mirror of production |
| Production | 500 users | 2000 users | With auto-scaling |

### Throughput

| Environment | Steady State | Peak Load | Burst Capacity |
|-------------|-------------|-----------|----------------|
| Development | 20 req/s | 50 req/s | 100 req/s |
| Staging | 100 req/s | 200 req/s | 500 req/s |
| Production | 1000 req/s | 5000 req/s | 10000 req/s |

## Database Performance SLOs

| Operation | P95 Query Time | Max Connections | Notes |
|-----------|---------------|-----------------|-------|
| SELECT (simple) | < 10ms | - | Single table, indexed |
| SELECT (complex) | < 50ms | - | Joins, aggregations |
| INSERT | < 20ms | - | Single row |
| UPDATE | < 30ms | - | Single row |
| DELETE | < 20ms | - | Single row |
| Bulk operations | < 100ms | - | Batch size < 100 |
| Connection pool | - | 100 | Per service instance |

## Infrastructure SLOs

### Kubernetes Pods

| Metric | Target | Alert Threshold |
|--------|--------|-----------------|
| Pod restart rate | < 1/hour | > 5/hour |
| Pod ready time | < 30s | > 60s |
| CPU utilization | 40-70% | > 85% |
| Memory utilization | 50-75% | > 90% |
| Network latency | < 5ms | > 20ms |

### Auto-Scaling

| Trigger | Scale Up Threshold | Scale Down Threshold | Cool-down Period |
|---------|-------------------|---------------------|------------------|
| CPU | > 70% | < 40% | 5 minutes |
| Memory | > 80% | < 50% | 5 minutes |
| Request count | > 100 req/s/pod | < 30 req/s/pod | 10 minutes |

## Performance Test Success Criteria

### Smoke Test
- ✅ All endpoints respond with 200 OK
- ✅ Response time < 1s for all requests
- ✅ Zero errors

### Load Test
- ✅ Error rate < 1%
- ✅ P95 response time < 500ms
- ✅ P99 response time < 1000ms
- ✅ Requests/sec > 100
- ✅ No pod restarts
- ✅ CPU utilization < 70%
- ✅ Memory utilization < 80%

### Stress Test
- ✅ Error rate < 5%
- ✅ P95 response time < 2000ms
- ✅ System recovers after stress
- ✅ No data corruption
- ✅ Graceful degradation

### Spike Test
- ✅ Error rate < 10% during spike
- ✅ System doesn't crash
- ✅ System recovers within 2 minutes
- ✅ P95 response time < 3000ms during spike

### Booking Flow Test
- ✅ Success rate > 95%
- ✅ Complete booking flow < 5s (P95)
- ✅ Error rate < 1%
- ✅ All booking steps succeed

## Monitoring and Alerting

### Critical Alerts (P1)
**Response:** Immediate (< 15 minutes)

- API availability < 99% (5-minute window)
- Error rate > 5% (5-minute window)
- P95 response time > 2000ms (5-minute window)
- Database connection failures
- Payment processing failures

### High Priority Alerts (P2)
**Response:** Within 1 hour

- API availability < 99.5% (15-minute window)
- Error rate > 2% (15-minute window)
- P95 response time > 1000ms (15-minute window)
- CPU utilization > 85% (10-minute window)
- Memory utilization > 90% (10-minute window)
- High pod restart rate (> 5/hour)

### Medium Priority Alerts (P3)
**Response:** Within 4 hours

- Error rate > 1% (30-minute window)
- P95 response time > 500ms (30-minute window)
- CPU utilization > 70% (30-minute window)
- Memory utilization > 80% (30-minute window)
- Slow database queries (> 100ms P95)

## Performance Budgets

### Frontend Performance Budget

| Resource Type | Budget (Gzipped) | Budget (Uncompressed) |
|---------------|------------------|----------------------|
| JavaScript | 200 KB | 600 KB |
| CSS | 50 KB | 150 KB |
| Images | 500 KB | 2 MB |
| Fonts | 100 KB | 300 KB |
| Total Page Size | 1 MB | 3 MB |

### API Response Size Budget

| Endpoint Type | Max Response Size |
|---------------|-------------------|
| List endpoints | 100 KB |
| Detail endpoints | 50 KB |
| Search results | 200 KB |

## Historical Benchmarks

### Baseline Performance (Production)

Measured on: 2024-01-21
Environment: Production (3 replicas, Standard_D2s_v3 nodes)

| Metric | Value |
|--------|-------|
| Avg Response Time | 145ms |
| P95 Response Time | 387ms |
| P99 Response Time | 726ms |
| Requests/sec | 847 |
| Error Rate | 0.03% |
| Availability | 99.94% |
| Avg CPU Utilization | 42% |
| Avg Memory Utilization | 58% |

### Load Test Results

| Test Date | VUs | Duration | Requests | Avg RT | P95 RT | Error Rate | Result |
|-----------|-----|----------|----------|--------|--------|------------|--------|
| 2024-01-21 | 100 | 16m | 48,532 | 162ms | 423ms | 0.04% | ✅ Pass |
| 2024-01-14 | 100 | 16m | 47,891 | 178ms | 456ms | 0.06% | ✅ Pass |
| 2024-01-07 | 100 | 16m | 46,234 | 189ms | 512ms | 0.08% | ✅ Pass |

## Continuous Improvement

### Performance Optimization Priorities

**Q1 2024:**
1. Reduce P95 response time to < 300ms (currently 387ms)
2. Improve availability to 99.95% (currently 99.94%)
3. Optimize database query performance
4. Implement advanced caching strategies

**Q2 2024:**
1. Frontend performance optimization (< 1s LCP)
2. API response payload optimization
3. Database connection pool tuning
4. CDN implementation for static assets

## Testing Schedule

| Test Type | Frequency | Environment | Day/Time |
|-----------|-----------|-------------|----------|
| Smoke Test | Before each deployment | Staging/Production | As needed |
| Load Test | Weekly | Staging | Monday 2 AM UTC |
| Stress Test | Quarterly | Staging | First Monday 2 AM UTC |
| Spike Test | Quarterly | Staging | Second Monday 2 AM UTC |
| Booking Flow Test | Weekly | Staging | Monday 3 AM UTC |

## Escalation Process

### Performance Degradation

**Level 1: Minor (SLO breach < 5 minutes)**
- Automated alert to on-call engineer
- No immediate action required
- Monitor for escalation

**Level 2: Moderate (SLO breach 5-15 minutes)**
- Page on-call engineer
- Begin investigation
- Prepare rollback if needed

**Level 3: Severe (SLO breach > 15 minutes)**
- Page on-call + team lead
- Immediate investigation
- Consider rollback
- Incident commander assigned

**Level 4: Critical (Complete outage)**
- Page entire on-call rotation
- Incident commander assigned
- Executive notification
- Immediate rollback or fix

## SLO Review Process

**Monthly Review:**
- Review all SLO violations
- Analyze root causes
- Update alert thresholds if needed

**Quarterly Review:**
- Review SLO targets
- Update based on business requirements
- Adjust performance budgets
- Update test scenarios

---

**Owner:** DevOps Team
**Last Review:** 2025-12-27
**Next Review:** 2026-03-27
**Contact:** devops@orange-rental.de
