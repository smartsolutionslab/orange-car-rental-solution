# CI/CD Pipeline Documentation

## Overview

The Orange Car Rental project uses GitHub Actions for Continuous Integration and Continuous Deployment (CI/CD). The pipeline ensures code quality, runs automated tests, builds Docker images, and deploys to staging and production environments.

## Workflows

### 1. Pull Request Checks (`pr-checks.yml`)

**Triggers:** Pull requests to `main` or `develop` branches

**Jobs:**
- **Lint Frontend Code** - ESLint and Prettier checks for both portals
- **Unit Tests** - Runs Jest/Karma tests with coverage reporting
- **E2E Smoke Tests** - Quick smoke tests tagged with `@smoke`
- **Build Check** - Verifies production builds succeed
- **Summary** - Posts comprehensive results as PR comment

**Status:** ✅ Runs on every PR

**Typical Duration:** 10-15 minutes

### 2. E2E Tests - Public Portal (`e2e-public-portal.yml`)

**Triggers:**
- Push to `main` or `develop`
- Pull requests
- Manual workflow dispatch

**Jobs:**
- **Run E2E Tests** - Executes all 171 E2E tests across:
  - 3 browsers (Chromium, Firefox, WebKit)
  - 3 shards for parallel execution (9 jobs total)
- **Publish Test Report** - Merges reports and uploads artifacts
- **Comment PR** - Posts test results summary on PRs
- **Notify on Failure** - Alerts team if tests fail on `main`

**Test Coverage:**
- US-1: Vehicle Search (30 tests)
- US-2: Booking Flow (48 tests)
- US-3: Authentication (20 tests)
- US-4: Booking History (38 tests)
- US-5: Profile Pre-fill (14 tests)
- US-6: Similar Vehicles (21 tests)

**Status:** ✅ Comprehensive cross-browser testing

**Typical Duration:** 20-30 minutes

### 3. Build and Deploy (`build-and-deploy.yml`)

**Triggers:**
- Push to `main` branch
- Manual workflow dispatch

**Jobs:**
1. **Build Frontend** - Builds both Angular applications
2. **Build Backend** - Builds all .NET microservices
3. **Docker Build** - Creates and pushes Docker images to GHCR
4. **Deploy Staging** - Deploys to staging environment
5. **Deploy Production** - Requires manual approval, deploys to production

**Environments:**
- **Staging**: Automatic deployment for testing
- **Production**: Manual approval required

**Status:** ✅ Full deployment pipeline

**Typical Duration:** 30-45 minutes

## Test Artifacts

### E2E Test Artifacts

All E2E test runs produce the following artifacts:

| Artifact | Description | Retention |
|----------|-------------|-----------|
| `playwright-results-*` | Raw test results per browser/shard | 7 days |
| `playwright-report-*` | HTML reports per browser/shard | 7 days |
| `merged-playwright-report` | Combined report across all runs | 30 days |
| `screenshots-*` | Screenshots from failed tests | 7 days |
| `videos-*` | Video recordings of failed tests | 7 days |

### Unit Test Artifacts

| Artifact | Description | Retention |
|----------|-------------|-----------|
| `test-results-*` | Coverage reports (frontend) | 30 days |
| `test-results-*-service` | Test results (backend) | 30 days |
| Coverage data sent to Codecov | N/A |

## Running Tests Locally

### E2E Tests

```bash
# Run all E2E tests
cd src/frontend/apps/public-portal
npm run e2e

# Run with UI mode
npm run e2e:ui

# Run specific browser
npm run e2e:chromium

# Run in headed mode
npm run e2e:headed

# Debug tests
npm run e2e:debug
```

### Using Docker Compose

```bash
# Run entire test environment
docker-compose -f docker-compose.test.yml up

# Run specific service
docker-compose -f docker-compose.test.yml up public-portal-e2e

# View logs
docker-compose -f docker-compose.test.yml logs -f

# Cleanup
docker-compose -f docker-compose.test.yml down -v
```

### Unit Tests

```bash
# Frontend
cd src/frontend/apps/public-portal
npm run test              # Watch mode
npm run test:ci           # Headless mode
npm run test:coverage     # With coverage

# Backend
cd src/Services/Vehicles
dotnet test               # Run tests
dotnet test --collect:"XPlat Code Coverage"  # With coverage
```

## Environment Variables

### E2E Tests

| Variable | Description | Default |
|----------|-------------|---------|
| `CI` | Indicates CI environment | `false` |
| `BASE_URL` | Application base URL | `http://localhost:4200` |
| `HEADLESS` | Run in headless mode | `true` in CI |

### Backend Tests

| Variable | Description |
|----------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Set to `Testing` |
| `ConnectionStrings__DefaultConnection` | Test database connection |

## Debugging Failed Tests

### E2E Test Failures

1. **Download artifacts** from the GitHub Actions run
2. **Open HTML report**: `playwright-report/index.html`
3. **View screenshots**: Located in test results by test name
4. **Watch videos**: WebM files showing test execution
5. **Check traces**: Playwright trace files (if enabled)

### Steps to Debug:

```bash
# 1. Download the playwright-report artifact from GitHub Actions

# 2. Extract and open locally
unzip merged-playwright-report.zip
cd playwright-report
npx playwright show-report

# 3. Or run locally with trace
npx playwright test --trace on

# 4. View trace
npx playwright show-trace trace.zip
```

### Common Failure Patterns

| Issue | Solution |
|-------|----------|
| Timeout errors | Increase timeout or check if backend is running |
| Element not found | Verify selectors, check if UI changed |
| Flaky tests | Add proper wait conditions, avoid `waitForTimeout` |
| Network errors | Check backend availability, API responses |

## Branch Protection Rules

### Main Branch

- ✅ Require pull request reviews (1 approver)
- ✅ Require status checks to pass:
  - Lint Frontend Code
  - Unit Tests (all apps)
  - E2E Smoke Tests
  - Build Check
- ✅ Require branches to be up to date
- ✅ Require conversation resolution
- ❌ Allow force pushes (disabled)

### Develop Branch

- ✅ Require pull request reviews (1 approver)
- ✅ Require status checks to pass (same as main)
- ✅ Allow fast-forward merges

## Deployment Environments

### Staging

- **URL**: `https://staging.orange-car-rental.example.com`
- **Deployment**: Automatic on merge to `main`
- **Database**: Separate staging database
- **Purpose**: Pre-production testing

### Production

- **URL**: `https://orange-car-rental.example.com`
- **Deployment**: Manual approval required
- **Database**: Production database
- **Purpose**: Live customer-facing environment

## Monitoring and Notifications

### Slack Notifications (Future)

Configure Slack webhook for:
- ❌ Failed deployments
- ❌ Failed E2E tests on `main`
- ✅ Successful production deployments

### Email Notifications

GitHub automatically sends emails for:
- Workflow failures (if you authored/commented)
- @mentions in workflow output

## Maintenance

### Updating Dependencies

```bash
# Frontend dependencies
cd src/frontend/apps/public-portal
npm outdated
npm update

# Playwright
npm install @playwright/test@latest
npx playwright install

# Backend dependencies
cd src/Services/Vehicles
dotnet list package --outdated
dotnet add package <PackageName>
```

### Cleaning Up Old Artifacts

Artifacts are automatically deleted based on retention policy:
- E2E results: 7 days
- Test reports: 30 days

Manual cleanup:
```bash
# GitHub CLI
gh run list --limit 100
gh run view <run-id>
gh run delete <run-id>
```

## Performance Optimization

### Parallel Execution

E2E tests are split across:
- 3 browsers (Chromium, Firefox, WebKit)
- 3 shards per browser
- **Total**: 9 parallel jobs

This reduces total execution time from ~90 minutes to ~30 minutes.

### Caching

- ✅ npm dependencies cached
- ✅ Playwright browsers cached
- ✅ Docker layer caching enabled

### Test Sharding Example

```bash
# Run shard 1 of 3
npx playwright test --shard=1/3

# Run shard 2 of 3
npx playwright test --shard=2/3

# Run shard 3 of 3
npx playwright test --shard=3/3
```

## Troubleshooting

### Workflow Not Triggering

Check:
1. Path filters in workflow file
2. Branch name matches trigger
3. GitHub Actions enabled for repository

### Tests Pass Locally But Fail in CI

Common causes:
1. **Timing issues**: CI is slower, add proper waits
2. **Environment differences**: Check env variables
3. **Port conflicts**: Ensure unique ports
4. **Missing dependencies**: Check Dockerfile/workflow

### Docker Build Failures

```bash
# Test Docker build locally
docker-compose -f docker-compose.test.yml build

# Check logs
docker-compose -f docker-compose.test.yml logs

# Clean rebuild
docker-compose -f docker-compose.test.yml build --no-cache
```

## Best Practices

### Writing Tests

1. **Use page object pattern** for complex pages
2. **Add explicit waits** instead of sleep/timeout
3. **Tag smoke tests** with `@smoke` for quick PR checks
4. **Keep tests independent** - no shared state
5. **Use test data fixtures** for consistency

### CI/CD Workflow

1. **Keep builds fast** - aim for <30 minutes
2. **Fail fast** - run quick checks first
3. **Parallelize** where possible
4. **Cache dependencies** aggressively
5. **Monitor flaky tests** - fix or quarantine

### Security

1. **Never commit secrets** - use GitHub Secrets
2. **Rotate credentials** regularly
3. **Review dependency updates** for vulnerabilities
4. **Use Dependabot** for security updates
5. **Audit Docker images** for CVEs

## Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Playwright Documentation](https://playwright.dev/)
- [Docker Documentation](https://docs.docker.com/)
- [Codecov Documentation](https://docs.codecov.com/)

## Support

For CI/CD issues:
1. Check workflow logs in GitHub Actions
2. Review this documentation
3. Contact DevOps team
4. Create issue in repository
