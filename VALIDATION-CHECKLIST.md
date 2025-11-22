# Validation Checklist - Production Readiness

This checklist helps verify that all testing and CI/CD infrastructure is correctly set up and working.

**Date**: 2025-11-20
**Version**: 1.0

---

## 🎯 Quick Validation

Run these commands to quickly verify everything works:

```bash
# 1. Check Node.js version (18+)
node --version

# 2. Check npm version
npm --version

# 3. Install root dependencies
npm install

# 4. Install Playwright browsers
npx playwright install

# 5. Verify Playwright installation
npx playwright --version
```

---

## 📁 File Structure Validation

### ✅ Core Files

- [ ] `package.json` (root) exists with Playwright dependencies
- [ ] `playwright.config.ts` exists
- [ ] `.env.example` exists
- [ ] `.gitignore` includes test artifacts
- [ ] `README.md` has testing section

### ✅ Documentation

- [ ] `QUICK-START-TESTING.md` exists
- [ ] `TEST-COVERAGE-REPORT.md` exists
- [ ] `TESTING-AND-CI-CD-SUMMARY.md` exists
- [ ] `CI-CD-SETUP.md` exists
- [ ] `IMPLEMENTATION-COMPLETE.md` exists
- [ ] `CHANGELOG.md` exists
- [ ] `COMMIT-GUIDE.md` exists
- [ ] `VALIDATION-CHECKLIST.md` exists (this file)

### ✅ E2E Test Files

- [ ] `e2e/` directory exists
- [ ] `e2e/helpers/auth.helper.ts` exists
- [ ] `e2e/pages/booking-history.page.ts` exists
- [ ] `e2e/pages/reservations.page.ts` exists
- [ ] `e2e/booking-history.e2e.spec.ts` exists
- [ ] `e2e/reservations.e2e.spec.ts` exists
- [ ] `e2e/README.md` exists

### ✅ Integration Test Files

- [ ] `src/frontend/apps/public-portal/src/app/pages/booking-history/booking-history.component.integration.spec.ts` exists
- [ ] `src/frontend/apps/call-center-portal/src/app/pages/reservations/reservations.component.integration.spec.ts` exists

### ✅ CI/CD Workflows

- [ ] `.github/workflows/unit-tests.yml` exists
- [ ] `.github/workflows/integration-tests.yml` exists
- [ ] `.github/workflows/e2e-tests.yml` exists
- [ ] `.github/workflows/build.yml` exists
- [ ] `.github/workflows/deploy.yml` exists

### ✅ GitHub Templates

- [ ] `.github/PULL_REQUEST_TEMPLATE.md` exists
- [ ] `.github/ISSUE_TEMPLATE/bug_report.md` exists
- [ ] `.github/ISSUE_TEMPLATE/feature_request.md` exists

---

## 🧪 Test Validation

### Unit Tests

```bash
# Public Portal
cd src/frontend/apps/public-portal
npm install
npm test

# Expected: All tests pass
# Expected: ~42 tests for booking history component
```

**Validation:**
- [ ] Public portal unit tests run successfully
- [ ] All 42 booking history tests pass
- [ ] No errors in console

```bash
# Call Center Portal
cd src/frontend/apps/call-center-portal
npm install
npm test

# Expected: All tests pass
# Expected: ~45 tests for reservations component
```

**Validation:**
- [ ] Call center unit tests run successfully
- [ ] All 45 reservations tests pass
- [ ] No errors in console

### Integration Tests

**Prerequisites:**
- [ ] Backend API is running on http://localhost:5000
- [ ] Database is accessible
- [ ] Keycloak is running (if needed)

```bash
# Public Portal Integration Tests
cd src/frontend/apps/public-portal
npm run test:integration

# Expected: All integration tests pass
# Expected: ~25+ tests
```

**Validation:**
- [ ] Integration tests run successfully
- [ ] HTTP calls are made correctly
- [ ] All integration tests pass

```bash
# Call Center Portal Integration Tests
cd src/frontend/apps/call-center-portal
npm run test:integration

# Expected: All integration tests pass
# Expected: ~25+ tests
```

**Validation:**
- [ ] Integration tests run successfully
- [ ] All integration tests pass

### E2E Tests

**Prerequisites:**
- [ ] Backend API running on http://localhost:5000
- [ ] Public portal running on http://localhost:4200
- [ ] Call center portal running on http://localhost:4201
- [ ] Test users configured in Keycloak
- [ ] `.env` file created from `.env.example`

```bash
# From project root
npm run test:e2e

# Expected: All E2E tests pass across 6 browsers
```

**Validation:**
- [ ] E2E tests run successfully
- [ ] Tests pass in Chromium
- [ ] Tests pass in Firefox
- [ ] Tests pass in WebKit
- [ ] Mobile tests run (optional)
- [ ] Screenshots captured on failures (if any)
- [ ] HTML report generated

---

## 📦 Package Configuration Validation

### Root package.json

```bash
# From project root
cat package.json
```

**Verify Scripts:**
- [ ] `test:e2e` script exists
- [ ] `test:e2e:headed` script exists
- [ ] `test:e2e:debug` script exists
- [ ] `test:e2e:ui` script exists
- [ ] `test:all` script exists

**Verify Dependencies:**
- [ ] `@playwright/test` is in devDependencies
- [ ] `@types/node` is in devDependencies

### Public Portal package.json

```bash
cd src/frontend/apps/public-portal
cat package.json
```

**Verify Scripts:**
- [ ] `test:ci` script exists
- [ ] `test:coverage` script exists
- [ ] `test:integration` script exists
- [ ] `test:watch` script exists
- [ ] `build:development` script exists
- [ ] `build:production` script exists
- [ ] `lint` script exists
- [ ] `format` script exists

### Call Center Portal package.json

```bash
cd src/frontend/apps/call-center-portal
cat package.json
```

**Verify Scripts:**
- [ ] `test:ci` script exists
- [ ] `test:coverage` script exists
- [ ] `test:integration` script exists
- [ ] `test:watch` script exists
- [ ] `build:development` script exists
- [ ] `build:production` script exists
- [ ] `lint` script exists
- [ ] `format` script exists

---

## 🔧 Configuration Validation

### Playwright Configuration

```bash
cat playwright.config.ts
```

**Verify:**
- [ ] `testDir: './e2e'` is set
- [ ] Multiple projects configured (chromium, firefox, webkit)
- [ ] Mobile projects configured
- [ ] `webServer` configuration exists for 3 services
- [ ] Timeout settings are reasonable
- [ ] Reporter configuration includes html, json, and list

### Environment Variables

```bash
# Check .env.example exists
ls -la .env.example

# Verify you have .env file (don't commit this)
ls -la .env
```

**Verify .env contains:**
- [ ] `BASE_URL`
- [ ] `TEST_CUSTOMER_USERNAME`
- [ ] `TEST_CUSTOMER_PASSWORD`
- [ ] `TEST_AGENT_USERNAME`
- [ ] `TEST_AGENT_PASSWORD`
- [ ] `TEST_RESERVATION_ID`
- [ ] `TEST_GUEST_EMAIL`

### GitIgnore

```bash
cat .gitignore | grep -A 20 "Testing Artifacts"
```

**Verify ignored:**
- [ ] `/test-results/`
- [ ] `/playwright-report/`
- [ ] `coverage/`
- [ ] `*.trace.zip`

---

## 🚀 CI/CD Pipeline Validation

### GitHub Actions Workflows

**Verify each workflow file:**

```bash
# Check unit-tests.yml
cat .github/workflows/unit-tests.yml
```

**Verify:**
- [ ] Triggers on push to develop, main
- [ ] Triggers on pull requests
- [ ] Has jobs for public-portal, call-center-portal, backend
- [ ] Uploads coverage to Codecov
- [ ] Uploads test results

```bash
# Check e2e-tests.yml
cat .github/workflows/e2e-tests.yml
```

**Verify:**
- [ ] Triggers on push, PR, and schedule
- [ ] Starts PostgreSQL service
- [ ] Starts Keycloak service
- [ ] Starts all 3 applications
- [ ] Runs Playwright tests
- [ ] Uploads artifacts on failure

### GitHub Secrets (Manual Check)

**Required Secrets:**
- [ ] `TEST_CUSTOMER_USERNAME` configured in GitHub
- [ ] `TEST_CUSTOMER_PASSWORD` configured in GitHub
- [ ] `TEST_AGENT_USERNAME` configured in GitHub
- [ ] `TEST_AGENT_PASSWORD` configured in GitHub
- [ ] `KUBE_CONFIG_STAGING` configured (for deployment)
- [ ] `KUBE_CONFIG_PRODUCTION` configured (for deployment)
- [ ] `SLACK_WEBHOOK` configured (for notifications)

---

## 📊 Coverage Validation

### Generate Coverage Reports

```bash
# Public Portal
cd src/frontend/apps/public-portal
npm run test:coverage

# Expected: Coverage report generated in coverage/
# Expected: >80% coverage
```

**Verify:**
- [ ] `coverage/` directory created
- [ ] `coverage/index.html` exists
- [ ] Coverage is above 80%
- [ ] lcov.info file generated

```bash
# Call Center Portal
cd src/frontend/apps/call-center-portal
npm run test:coverage

# Expected: Coverage report generated
# Expected: >80% coverage
```

**Verify:**
- [ ] Coverage report generated
- [ ] Coverage is above 80%

---

## 🌐 Browser Compatibility Validation

### Local E2E Tests

```bash
# Test specific browsers
npm run test:e2e:chromium
npm run test:e2e:firefox
npm run test:e2e:webkit
```

**Verify:**
- [ ] Chromium tests pass
- [ ] Firefox tests pass
- [ ] WebKit tests pass

### Mobile Tests

```bash
npm run test:e2e:mobile
```

**Verify:**
- [ ] Mobile Chrome tests pass
- [ ] Mobile Safari tests pass

---

## 📝 Documentation Validation

### README Links

Open `README.md` and verify all links work:

- [ ] Link to QUICK-START-TESTING.md works
- [ ] Link to TEST-COVERAGE-REPORT.md works
- [ ] Link to e2e/README.md works
- [ ] Link to CI-CD-SETUP.md works
- [ ] Link to TESTING-AND-CI-CD-SUMMARY.md works

### Documentation Consistency

**Verify consistent information across:**
- [ ] Test counts match across all docs (~87 unit, 50+ integration, 50+ E2E)
- [ ] Coverage percentages match (~89%)
- [ ] Browser counts match (6 browsers)
- [ ] Workflow counts match (5 workflows)

---

## ✅ Final Checks

### Pre-Push Checklist

Before pushing to GitHub:

- [ ] All unit tests pass locally
- [ ] All integration tests pass locally
- [ ] All E2E tests pass locally
- [ ] No console errors or warnings
- [ ] All documentation updated
- [ ] CHANGELOG.md updated
- [ ] USER_STORIES.md updated
- [ ] No sensitive data in commits
- [ ] `.env` file not committed
- [ ] Test artifacts ignored in .gitignore

### Post-Push Validation

After pushing to GitHub:

- [ ] Unit tests workflow passes
- [ ] Integration tests workflow passes
- [ ] E2E tests workflow passes
- [ ] Build workflow passes
- [ ] No workflow failures
- [ ] Coverage reports uploaded to Codecov
- [ ] PR checks all green

---

## 🔍 Common Issues and Solutions

### Issue: Playwright browsers not installed

**Solution:**
```bash
npx playwright install --force
```

### Issue: Tests fail due to missing environment variables

**Solution:**
```bash
cp .env.example .env
# Edit .env with your values
```

### Issue: Integration tests fail - API not running

**Solution:**
```bash
cd src/backend/Api
dotnet run
```

### Issue: E2E tests fail - Frontend not running

**Solution:**
```bash
# Terminal 1
cd src/frontend/apps/public-portal
npm start

# Terminal 2
cd src/frontend/apps/call-center-portal
npm start
```

### Issue: Port already in use

**Solution:**
```bash
# Windows
netstat -ano | findstr :4200
taskkill /PID <pid> /F

# macOS/Linux
lsof -ti:4200 | xargs kill -9
```

---

## 📈 Success Criteria

All items below should be ✅ before considering the setup complete:

### Core Setup
- [ ] All test files exist and are accessible
- [ ] All documentation files exist
- [ ] All CI/CD workflows configured
- [ ] GitHub templates created

### Testing
- [ ] 87 unit tests pass
- [ ] 50+ integration tests pass
- [ ] 50+ E2E tests pass
- [ ] Coverage above 80%

### Configuration
- [ ] package.json files updated
- [ ] Playwright configured
- [ ] Environment variables set
- [ ] .gitignore updated

### Documentation
- [ ] All docs created
- [ ] All links work
- [ ] Information consistent
- [ ] Examples clear

### CI/CD
- [ ] All 5 workflows configured
- [ ] Secrets configured in GitHub
- [ ] Deployment pipelines ready

---

## 🎉 Validation Complete

If all items above are checked ✅, your testing and CI/CD infrastructure is:

```
╔════════════════════════════════════════╗
║     ✅ PRODUCTION READY                ║
║     ✅ FULLY TESTED                    ║
║     ✅ CI/CD CONFIGURED                ║
║     ✅ DOCUMENTED                      ║
╚════════════════════════════════════════╝
```

---

**Validation Date**: _____________
**Validated By**: _____________
**Status**: ⬜ Pass / ⬜ Fail

**Notes:**




---

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
