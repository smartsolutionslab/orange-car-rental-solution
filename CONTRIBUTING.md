# Contributing to Orange Car Rental

Thank you for your interest in contributing to Orange Car Rental! This document provides guidelines and instructions for contributing to the project.

---

## 📋 Table of Contents

- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Code Standards](#code-standards)
- [Testing Requirements](#testing-requirements)
- [Commit Guidelines](#commit-guidelines)
- [Pull Request Process](#pull-request-process)
- [Project Structure](#project-structure)
- [Common Tasks](#common-tasks)

---

## 🚀 Getting Started

### Prerequisites

- Node.js 20+ ([Download](https://nodejs.org/))
- .NET 8.0+ ([Download](https://dotnet.microsoft.com/))
- Git
- Docker (optional, for local services)

### Initial Setup

1. **Fork and Clone**
   ```bash
   git clone https://github.com/smartsolutionslab/orange-car-rental-solution.git
   cd orange-car-rental-solution
   ```

2. **Run Automated Setup**
   ```bash
   # Linux/macOS
   npm run setup

   # Windows
   npm run setup:windows
   ```

3. **Configure Environment**
   ```bash
   # Edit .env with your credentials
   code .env
   ```

4. **Verify Setup**
   ```bash
   npm run test:unit
   ```

**Setup time:** ~10 minutes

---

## 🔄 Development Workflow

### 1. Create a Feature Branch

```bash
git checkout develop
git pull origin develop
git checkout -b feature/your-feature-name
```

**Branch Naming:**
- Features: `feature/description`
- Bug fixes: `fix/description`
- Docs: `docs/description`
- Tests: `test/description`

### 2. Make Your Changes

- Write code following our [code standards](#code-standards)
- Add tests for new functionality
- Update documentation as needed

### 3. Test Your Changes

```bash
# Run all tests
npm run test:all

# Or run specific test types
npm run test:unit
npm run test:integration
npm run test:e2e

# Check code quality
npm run lint
npm run format:check
```

### 4. Pre-commit Validation

Before committing, run our automated checks:

```bash
npm run precommit
```

This checks:
- ✅ Linting
- ✅ Unit tests
- ✅ console.log statements
- ✅ TODO/FIXME comments

### 5. Commit Your Changes

Follow our [commit guidelines](#commit-guidelines):

```bash
git add .
git commit -m "feat: add booking history feature"
```

### 6. Push and Create PR

```bash
git push origin feature/your-feature-name

# Create PR via GitHub or gh CLI
gh pr create --base develop --title "feat: Add booking history"
```

---

## 📐 Code Standards

### TypeScript/Angular

- **Style Guide:** Follow [Angular Style Guide](https://angular.io/guide/styleguide)
- **Formatting:** Prettier (runs automatically)
- **Linting:** ESLint with Angular rules
- **Naming:**
  - Components: `kebab-case.component.ts`
  - Services: `kebab-case.service.ts`
  - Models: `kebab-case.model.ts`

**Example:**
```typescript
// Good
export class BookingHistoryComponent {
  private readonly reservations = signal<Reservation[]>([]);

  ngOnInit(): void {
    this.loadReservations();
  }
}

// Avoid
export class BookingHistoryComponent {
  reservations: any; // Use proper types

  ngOnInit() { // Use explicit return type
    console.log('test'); // Remove console.log
  }
}
```

### C# / .NET

- **Style Guide:** Follow [Microsoft C# Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- **Formatting:** Built-in .NET formatter
- **Naming:**
  - Classes: `PascalCase`
  - Methods: `PascalCase`
  - Private fields: `_camelCase`

### General Principles

1. **DRY** - Don't Repeat Yourself
2. **SOLID** - Follow SOLID principles
3. **Clear naming** - Self-documenting code
4. **Small functions** - Single responsibility
5. **Error handling** - Always handle errors gracefully

---

## 🧪 Testing Requirements

All code contributions must include appropriate tests.

### Test Coverage Requirements

- **Minimum:** 80% code coverage
- **Target:** 90% code coverage
- **Critical paths:** 100% coverage

### Test Types

#### 1. Unit Tests (Required)

Test individual components/services in isolation.

```typescript
// booking-history.component.spec.ts
it('should load reservations on init', () => {
  component.ngOnInit();

  expect(mockReservationService.getReservations).toHaveBeenCalled();
  expect(component.reservations()).toHaveLength(3);
});
```

#### 2. Integration Tests (Recommended)

Test component integration with services and HTTP.

```typescript
// booking-history.component.integration.spec.ts
it('should fetch reservations from API', fakeAsync(() => {
  component.ngOnInit();
  tick();

  const req = httpMock.expectOne('/api/reservations/search');
  req.flush({ items: mockReservations });
  tick();

  expect(component.reservations()).toHaveLength(3);
}));
```

#### 3. E2E Tests (For Major Features)

Test complete user workflows.

```typescript
// booking-history.e2e.spec.ts
test('should display booking history', async ({ page }) => {
  await page.goto('/booking-history');
  await page.waitForSelector('.reservation-card');

  const cards = await page.locator('.reservation-card').count();
  expect(cards).toBeGreaterThan(0);
});
```

### Running Tests

```bash
# Unit tests (fast, run frequently)
npm run test:unit

# Integration tests (slower, run before commit)
npm run test:integration

# E2E tests (slowest, run before PR)
npm run test:e2e

# All tests (run before creating PR)
npm run test:all
```

---

## 💬 Commit Guidelines

We follow [Conventional Commits](https://www.conventionalcommits.org/).

### Commit Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting)
- `refactor`: Code refactoring
- `test`: Test additions/changes
- `chore`: Build process or tooling changes

### Examples

```bash
# Feature
git commit -m "feat(booking): add cancellation functionality"

# Bug fix
git commit -m "fix(auth): correct token refresh logic"

# Documentation
git commit -m "docs(readme): update setup instructions"

# With body
git commit -m "feat(reservations): add advanced filtering

- Add status filter
- Add date range filter
- Add location filter
- Update tests

Implements: US-8"
```

See [COMMIT-GUIDE.md](./COMMIT-GUIDE.md) for detailed examples.

---

## 🔀 Pull Request Process

### Before Creating PR

1. ✅ All tests pass locally
2. ✅ Code coverage meets requirements
3. ✅ Pre-commit checks pass
4. ✅ Branch is up-to-date with develop
5. ✅ Documentation updated

### PR Checklist

When creating a PR, ensure you've completed the checklist in our [PR template](.github/PULL_REQUEST_TEMPLATE.md):

- [ ] Tests added/updated
- [ ] Documentation updated
- [ ] No breaking changes (or documented)
- [ ] Code coverage maintained (>80%)
- [ ] All CI checks passing

### PR Review Process

1. **Automated Checks** (5-45 minutes)
   - Unit tests
   - Integration tests
   - E2E tests
   - Build verification
   - Code coverage

2. **Code Review** (1-3 days)
   - At least one approval required
   - Address all comments
   - Keep PR updated

3. **Merge** (After approval)
   - Squash and merge to develop
   - Delete feature branch

---

## 📁 Project Structure

```
orange-car-rental/
├── src/
│   ├── frontend/
│   │   └── apps/
│   │       ├── public-portal/          # Customer-facing app
│   │       └── call-center-portal/     # Agent-facing app
│   └── backend/
│       └── Api/                        # .NET Web API
├── e2e/                                # E2E tests
│   ├── helpers/
│   ├── pages/
│   └── fixtures/
├── scripts/                            # Automation scripts
├── .github/
│   └── workflows/                      # CI/CD pipelines
└── docs/                               # Documentation
```

### Key Directories

- **Components:** `src/frontend/apps/*/src/app/pages/`
- **Services:** `src/frontend/apps/*/src/app/services/`
- **Models:** `src/frontend/apps/*/src/app/services/*.model.ts`
- **Unit Tests:** `*.spec.ts`
- **Integration Tests:** `*.integration.spec.ts`
- **E2E Tests:** `e2e/*.e2e.spec.ts`

---

## 🛠️ Common Tasks

### Adding a New Feature

1. Create feature branch
2. Implement component/service
3. Add unit tests
4. Add integration tests
5. Add E2E tests (if major feature)
6. Update documentation
7. Create PR

### Fixing a Bug

1. Create fix branch
2. Write failing test that reproduces bug
3. Fix the bug
4. Ensure test passes
5. Add regression test if needed
6. Create PR

### Updating Documentation

1. Create docs branch
2. Update relevant .md files
3. Check links and formatting
4. Create PR

### Adding Tests to Existing Code

1. Create test branch
2. Add missing tests
3. Achieve target coverage
4. Create PR

---

## 🐛 Reporting Bugs

Use our [bug report template](.github/ISSUE_TEMPLATE/bug_report.md):

1. Go to Issues → New Issue
2. Select "Bug Report"
3. Fill in all sections
4. Add screenshots if applicable
5. Submit

---

## 💡 Requesting Features

Use our [feature request template](.github/ISSUE_TEMPLATE/feature_request.md):

1. Go to Issues → New Issue
2. Select "Feature Request"
3. Describe the feature
4. Explain use case
5. Submit

---

## 📚 Resources

### Documentation
- [CI/CD Setup](./CI-CD-SETUP.md)
- [Commit Guide](./COMMIT-GUIDE.md)

### External
- [Angular Documentation](https://angular.io/docs)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Playwright Documentation](https://playwright.dev/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

## ❓ Getting Help

- **Questions:** Open a GitHub Discussion
- **Bugs:** Open an Issue
- **Chat:** Join our Slack channel (if available)
- **Documentation:** Check the docs/ folder

---

## 🎯 Best Practices

1. **Test First** - Write tests before or alongside code
2. **Small PRs** - Keep PRs focused and reviewable
3. **Clear Commits** - Write meaningful commit messages
4. **Documentation** - Update docs with code changes
5. **Code Review** - Review others' code constructively
6. **Stay Updated** - Keep your branch current with develop

---

## ✅ Checklist for First-Time Contributors

- [ ] Read this contributing guide
- [ ] Set up development environment
- [ ] Run all tests successfully
- [ ] Read code standards
- [ ] Explore the codebase
- [ ] Find a "good first issue"
- [ ] Ask questions if stuck

---

Thank you for contributing to Orange Car Rental! 🚗🧡

---

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
