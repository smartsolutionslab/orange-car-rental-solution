# CI/CD Pipeline Setup

This document describes the CI/CD pipelines for the Orange Car Rental project using GitHub Actions.

## Overview

The project uses GitHub Actions for automated testing, building, and deployment across multiple environments.

## Workflow Files

All workflows are located in `.github/workflows/`:

### 1. **unit-tests.yml** - Unit Testing

**Triggers:**
- Push to `develop` or `main`
- Pull requests to these branches

**Jobs:**
- `test-public-portal`: Runs unit tests for public portal
- `test-call-center-portal`: Runs unit tests for call center portal
- `test-backend`: Runs .NET backend unit tests

**Features:**
- Parallel test execution
- Code coverage reports uploaded to Codecov
- Test result artifacts
- Summary generation

**Commands:**
```yaml
npm run test:ci          # Frontend tests
npm run test:coverage    # Coverage generation
dotnet test              # Backend tests
```

### 2. **integration-tests.yml** - Integration Testing

**Triggers:**
- Push to `develop` or `main`
- Pull requests to these branches

**Services:**
- SQL Server database
- Keycloak authentication server

**Jobs:**
- Starts backend API
- Runs database migrations
- Executes integration tests for both portals

**Features:**
- Real HTTP interactions with HttpTestingController
- Database integration testing
- Service-to-service communication tests

### 3. **e2e-tests.yml** - End-to-End Testing

**Triggers:**
- Push to `develop` or `main`
- Pull requests to these branches
- Daily schedule at 2 AM UTC
- Manual workflow dispatch

**Jobs:**
- `e2e-tests`: Full browser testing with Playwright
- `e2e-mobile`: Mobile device testing

**Browsers Tested:**
- Chrome/Chromium
- Firefox
- WebKit (Safari)
- Mobile Chrome
- Mobile Safari

**Features:**
- Full application testing
- Screenshot and video capture on failure
- HTML reports
- PR comments with results

**Environment Variables Required:**
```env
TEST_CUSTOMER_USERNAME
TEST_CUSTOMER_PASSWORD
TEST_AGENT_USERNAME
TEST_AGENT_PASSWORD
TEST_RESERVATION_ID
TEST_GUEST_EMAIL
```

### 4. **build.yml** - Build and Lint

**Triggers:**
- Push to `develop` or `main`
- Pull requests to these branches

**Jobs:**
- `lint-frontend`: ESLint and Prettier checks
- `build-frontend`: Angular production builds
- `lint-backend`: .NET code formatting and analysis
- `build-backend`: .NET compilation and publish
- `docker-build`: Docker image builds

**Matrix Strategies:**
- Frontend: 2 apps × 2 environments = 4 builds
- Backend: Debug + Release configurations
- Docker: 3 images (API, Public Portal, Call Center Portal)

**Features:**
- Bundle size reporting
- Code formatting verification
- Multi-environment builds
- Docker layer caching

### 5. **deploy.yml** - Deployment

**Triggers:**
- Push to `main` (production)
- Push to `develop` (staging)
- Tags matching `v*` (releases)
- Manual workflow dispatch with environment selection

**Jobs:**
- `build-and-push-images`: Build and push Docker images to GHCR
- `deploy-staging`: Deploy to staging environment
- `deploy-production`: Deploy to production environment
- `rollback-production`: Automatic rollback on failure

**Environments:**
- **Staging**: `https://staging.orange-car-rental.com`
- **Production**: `https://orange-car-rental.com`

**Features:**
- Docker image versioning with SHA tags
- Kubernetes deployments
- Database backups before production deploy
- Smoke tests after deployment
- Slack notifications
- Automatic rollback on failure
- GitHub release creation for tags

## Required GitHub Secrets

Configure these in your GitHub repository settings (`Settings > Secrets and variables > Actions`):

### Authentication
- `TEST_CUSTOMER_USERNAME`: Test customer account username
- `TEST_CUSTOMER_PASSWORD`: Test customer account password
- `TEST_AGENT_USERNAME`: Test agent account username
- `TEST_AGENT_PASSWORD`: Test agent account password

### Test Data
- `TEST_RESERVATION_ID`: Valid test reservation ID
- `TEST_GUEST_EMAIL`: Test guest email

### Deployment
- `KUBE_CONFIG_STAGING`: Base64-encoded kubeconfig for staging cluster
- `KUBE_CONFIG_PRODUCTION`: Base64-encoded kubeconfig for production cluster
- `SLACK_WEBHOOK`: Slack webhook URL for notifications

### Registry (Automatic)
- `GITHUB_TOKEN`: Automatically provided by GitHub Actions

## Workflow Triggers Summary

| Workflow | Push | PR | Schedule | Manual |
|----------|------|-----|----------|--------|
| Unit Tests | ✅ | ✅ | ❌ | ❌ |
| Integration Tests | ✅ | ✅ | ❌ | ❌ |
| E2E Tests | ✅ | ✅ | ✅ (daily) | ✅ |
| Build & Lint | ✅ | ✅ | ❌ | ❌ |
| Deploy | ✅ | ❌ | ❌ | ✅ |

## Deployment Strategy

### Staging Deployment
- **Trigger**: Push to `develop` branch
- **Automatic**: Yes
- **Approval**: Not required
- **Environment**: `staging`

### Production Deployment
- **Trigger**: Push to `main` or version tags
- **Automatic**: Yes
- **Approval**: Required (configured in GitHub Environment)
- **Environment**: `production`
- **Rollback**: Automatic on failure

## Pipeline Execution Flow

```
┌─────────────────┐
│   Push/PR       │
└────────┬────────┘
         │
         ├──────> Unit Tests (3-5 min)
         ├──────> Build & Lint (5-7 min)
         └──────> Integration Tests (8-10 min)
                  │
                  └──────> E2E Tests (15-20 min)
                           │
                           └──> Build Docker Images
                                │
                                ├──> Deploy to Staging (auto)
                                │
                                └──> Deploy to Production (approval required)
```

## Test Coverage Requirements

- **Unit Tests**: > 80% coverage
- **Integration Tests**: All critical user flows
- **E2E Tests**: Complete end-to-end scenarios

## Artifacts

Each workflow generates artifacts that are retained for specified periods:

- **Test Results**: 7-30 days
- **Build Artifacts**: 7 days
- **Playwright Reports**: 30 days
- **Screenshots/Videos**: 7 days (failures only)
- **Application Logs**: 7 days (failures only)

## Monitoring and Notifications

### Job Status
- View status badges in README
- Check GitHub Actions tab for detailed logs
- PR comments with E2E test results

### Deployment Notifications
- Slack notifications for production deployments
- GitHub release notes for version tags
- Deployment summaries in GitHub Actions

## Local Testing

### Run Tests Locally

```bash
# Unit tests
cd src/frontend/apps/public-portal
npm run test

# Integration tests
npm run test:integration

# E2E tests
npx playwright test

# Backend tests
cd src/backend
dotnet test
```

### Validate Workflows

```bash
# Install act (GitHub Actions local runner)
brew install act  # macOS
choco install act # Windows

# Run workflows locally
act -l                        # List workflows
act push                      # Simulate push event
act pull_request              # Simulate PR event
```

## Troubleshooting

### Tests Failing in CI but Pass Locally

1. Check for hardcoded paths or environment-specific code
2. Verify all dependencies are in `package.json` / `.csproj`
3. Check timezone or locale differences
4. Review CI logs for specific error messages

### Docker Build Failures

1. Verify Dockerfiles are present
2. Check for missing build context files
3. Review layer caching issues
4. Ensure build arguments are properly set

### Deployment Failures

1. Check kubeconfig secrets are properly formatted
2. Verify Kubernetes cluster connectivity
3. Review deployment manifests
4. Check namespace and resource names

### E2E Test Timeouts

1. Increase timeout in `playwright.config.ts`
2. Check if services are starting properly
3. Review application startup logs
4. Verify network connectivity

## Best Practices

1. **Always run tests before pushing**: Use pre-commit hooks
2. **Keep workflows fast**: Parallelize when possible
3. **Use caching**: npm, Docker layers, build artifacts
4. **Fail fast**: Stop on first critical failure
5. **Monitor costs**: GitHub Actions has usage limits
6. **Security**: Never commit secrets, use GitHub Secrets
7. **Documentation**: Update this file when workflows change

## Maintenance

### Regular Tasks

- Review and update workflow dependencies quarterly
- Clean up old artifacts and logs
- Monitor GitHub Actions usage and costs
- Update test data and credentials
- Review and optimize slow workflows

### Updating Workflows

1. Create a feature branch
2. Update workflow files
3. Test using `act` or in a PR
4. Document changes in this file
5. Merge after review

## Performance Metrics

### Target Execution Times

- Unit Tests: < 5 minutes
- Integration Tests: < 10 minutes
- E2E Tests: < 20 minutes
- Build & Lint: < 7 minutes
- Total PR Pipeline: < 30 minutes

### Current Performance

Monitor actual times in GitHub Actions insights:
- `Actions > Workflows > [Workflow Name] > View runs`

## Support

For issues with CI/CD:
1. Check GitHub Actions logs
2. Review this documentation
3. Consult team leads
4. Open an issue in the repository

---

**Last Updated**: 2025-11-20
**Version**: 1.0
**Maintained by**: DevOps Team

---

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
