# Development Workflow Guide

**Project:** Orange Car Rental Solution
**Repository:** https://github.com/smartsolutionslab/orange-car-rental-solution
**Last Updated:** 2025-11-05

---

## 📋 Table of Contents
- [Git Branching Strategy](#git-branching-strategy)
- [Feature Development Workflow](#feature-development-workflow)
- [Pull Request Process](#pull-request-process)
- [Code Review Guidelines](#code-review-guidelines)
- [CI/CD Pipeline](#cicd-pipeline)
- [Commit Message Standards](#commit-message-standards)
- [Branch Protection Rules](#branch-protection-rules)
- [Release Process](#release-process)

---

## 🌳 Git Branching Strategy

We follow **GitFlow** with these branches:

### Permanent Branches

#### `master` (Production)
- **Purpose:** Production-ready code
- **Protection:** ✅ Protected, requires PR + reviews
- **Deploy:** Auto-deploys to production
- **Merge from:** `develop` only (via release PRs)
- **Never commit directly**

#### `develop` (Integration)
- **Purpose:** Integration branch for features
- **Protection:** ✅ Protected, requires PR + CI pass
- **Deploy:** Auto-deploys to staging environment
- **Merge from:** Feature branches, hotfix branches
- **Never commit directly**

### Temporary Branches

#### Feature Branches
```
feature/US-{number}-{short-description}
```
**Examples:**
- `feature/US-7-reservation-apis`
- `feature/US-4-booking-history`
- `feature/US-3-user-authentication`

**Created from:** `develop`
**Merged into:** `develop`
**Lifespan:** Until feature is complete and merged
**Delete after:** Merge

#### Bugfix Branches
```
bugfix/{issue-number}-{short-description}
```
**Examples:**
- `bugfix/42-fix-price-calculation`
- `bugfix/88-date-validation-error`

**Created from:** `develop`
**Merged into:** `develop`
**Delete after:** Merge

#### Hotfix Branches
```
hotfix/{issue-number}-{short-description}
```
**Examples:**
- `hotfix/99-critical-security-fix`
- `hotfix/101-payment-gateway-down`

**Created from:** `master` ⚠️
**Merged into:** `master` AND `develop`
**For:** Production-critical fixes only

#### Release Branches
```
release/v{major}.{minor}.{patch}
```
**Examples:**
- `release/v1.0.0`
- `release/v1.1.0`

**Created from:** `develop`
**Merged into:** `master` AND `develop`
**Purpose:** Final testing and version bump

---

## 🚀 Feature Development Workflow

### Step 1: Select a User Story

1. Go to **GitHub Project Board**: https://github.com/orgs/smartsolutionslab/projects
2. Select an issue from "Backlog" column
3. Move it to "In Progress"
4. Assign yourself to the issue

### Step 2: Create Feature Branch

```bash
# Make sure you're on develop and it's up to date
git checkout develop
git pull origin develop

# Create and switch to feature branch
git checkout -b feature/US-7-reservation-apis

# Push branch to remote
git push -u origin feature/US-7-reservation-apis
```

### Step 3: Development

#### Write Code Following Standards
- **Backend (.NET):**
  - No primitive types in domain models
  - Use value objects for all domain concepts
  - Sealed records for value objects
  - Past tense for domain events (`ReservationConfirmed`)
  - One class per file
  - Follow `.editorconfig` rules

- **Frontend (Angular):**
  - Standalone components
  - Smart/dumb component pattern
  - Signal-based state management
  - Tailwind utility classes
  - Reactive forms

#### Test as You Go
```bash
# Backend - Run tests
cd src/backend
dotnet test --filter "Category=Unit"

# Frontend - Run tests
cd src/frontend/apps/public-portal
npm run test

# Code formatting
cd src/backend
dotnet format OrangeCarRental.sln
```

#### Commit Frequently
```bash
# Stage changes
git add .

# Commit with conventional commit message
git commit -m "feat(reservations): add search endpoint

- Implement GET /api/reservations/search
- Add filtering by status and customer ID
- Include pagination support

Implements US-7"

# Push to remote
git push
```

### Step 4: Keep Your Branch Updated

```bash
# Regularly sync with develop
git checkout develop
git pull origin develop
git checkout feature/US-7-reservation-apis
git merge develop

# Resolve conflicts if any
# Then push
git push
```

### Step 5: Final Pre-PR Checklist

Before creating a PR, ensure:

```bash
# ✅ All tests pass locally
cd src/backend
dotnet test

cd src/frontend/apps/public-portal
npm run test

# ✅ Code is formatted
cd src/backend
dotnet format OrangeCarRental.sln

# ✅ No build warnings
dotnet build --no-incremental

# ✅ Security check
dotnet list package --vulnerable

# ✅ Your branch is up to date with develop
git fetch origin
git merge origin/develop
```

---

## 🔄 Pull Request Process

### Creating a Pull Request

#### Option 1: GitHub CLI (Recommended)
```bash
gh pr create \
  --title "feat(reservations): Implement reservation search and management APIs" \
  --body "Implements #7 (US-7: List All Bookings)

## Changes
- Add GET /api/reservations/search endpoint
- Add PUT /api/reservations/{id}/confirm endpoint
- Add PUT /api/reservations/{id}/cancel endpoint
- Extend IReservationRepository with search method

## Testing
- Added unit tests for all new commands/queries
- Integration tests for API endpoints
- Manual testing with call center portal

Closes #7" \
  --base develop \
  --head feature/US-7-reservation-apis \
  --assignee @me \
  --reviewer @teammate1,@teammate2
```

#### Option 2: GitHub Web Interface
1. Go to: https://github.com/smartsolutionslab/orange-car-rental-solution/pulls
2. Click "New pull request"
3. **Base:** `develop` ← **Compare:** `feature/US-7-reservation-apis`
4. Fill out the PR template (it will auto-load)
5. Add reviewers
6. Add labels (user-story, backend, high priority, etc.)
7. Link to issues using `Closes #7`
8. Create pull request

### PR Template Checklist

The template includes:
- 📝 Description
- 🎯 Related issues
- 🔄 Type of change
- ✅ Acceptance criteria met
- 🧪 Testing details
- 📸 Screenshots (if UI changes)
- ✅ Comprehensive checklist
- 🔐 Security considerations
- 📊 Performance impact
- 🚀 Deployment notes

**Fill out EVERY section** - reviewers will check this!

### Automated CI/CD Checks

When you create a PR, these workflows run automatically:

#### 1. Backend CI (`backend-ci.yml`)
- ✅ Build solution
- ✅ Run unit tests
- ✅ Run integration tests
- ✅ Code coverage report
- ✅ Docker image build

#### 2. Frontend CI (`frontend-ci.yml`)
- ✅ Build both portals
- ✅ Run linter
- ✅ Run tests with coverage
- ✅ Docker image build

#### 3. Code Quality (`code-quality.yml`)
- ✅ Code formatting check
- ✅ Static analysis (Roslynator)
- ✅ Security vulnerability scan
- ✅ TypeScript type checking
- ✅ Prettier formatting
- ✅ CodeQL security analysis
- ✅ Dependency review

**All checks must pass** before merge! ✅

### Review Process

#### Requesting Reviews
- Assign **at least 1 reviewer** (2 for major features)
- Use `@mention` to notify reviewers
- Respond to feedback promptly
- Mark conversations as "Resolved" when addressed

#### Making Changes Based on Feedback
```bash
# Make requested changes
git add .
git commit -m "refactor: address PR feedback

- Rename method for clarity
- Add missing null check
- Improve error messages"

git push  # CI will re-run automatically
```

#### Getting Approval
- ✅ At least 1 approval required
- ✅ All CI checks must pass
- ✅ All conversations resolved
- ✅ No merge conflicts

### Merging the PR

**Merge Strategy:** **Squash and Merge** (recommended)

```bash
# Option 1: GitHub CLI
gh pr merge 42 --squash --delete-branch

# Option 2: GitHub Web UI
# 1. Click "Squash and merge"
# 2. Edit commit message if needed
# 3. Confirm merge
# 4. Delete branch
```

**After merge:**
- Branch auto-deletes
- Issue auto-closes (if used `Closes #X`)
- Project board auto-updates
- Slack/notification sent (if configured)

---

## 👀 Code Review Guidelines

### For Authors

#### Before Requesting Review
- [ ] Self-review your own code first
- [ ] Add inline comments for complex logic
- [ ] Update documentation
- [ ] Test thoroughly
- [ ] Keep PR focused (< 500 lines preferred)

#### Responding to Feedback
- **Be receptive:** Reviews make code better
- **Ask questions:** If you don't understand feedback
- **Explain decisions:** If you disagree with suggestion
- **Mark resolved:** When you've addressed feedback
- **Say thanks:** Appreciate the reviewer's time

### For Reviewers

#### What to Check

##### Functionality
- [ ] Does it meet acceptance criteria?
- [ ] Are edge cases handled?
- [ ] Error handling is comprehensive?
- [ ] Input validation is thorough?

##### Code Quality
- [ ] Code is readable and self-documenting
- [ ] No code duplication (DRY principle)
- [ ] Functions/methods have single responsibility
- [ ] Consistent with project patterns
- [ ] No hardcoded values (use configuration)

##### Testing
- [ ] Adequate test coverage
- [ ] Tests are meaningful (not just for coverage)
- [ ] Integration tests for API endpoints
- [ ] Edge cases are tested

##### Security
- [ ] No SQL injection vulnerabilities
- [ ] No XSS vulnerabilities
- [ ] Sensitive data not logged
- [ ] Authentication/authorization checks
- [ ] No secrets in code

##### Performance
- [ ] No N+1 query problems
- [ ] Efficient algorithms
- [ ] Appropriate caching
- [ ] Database indexes considered

##### Documentation
- [ ] README updated if needed
- [ ] API documentation updated
- [ ] Complex logic is commented
- [ ] USER_STORIES.md updated

#### Review Etiquette
- ✅ **Be constructive:** Suggest improvements, don't just criticize
- ✅ **Be specific:** Point to exact lines, provide examples
- ✅ **Be timely:** Review within 24 hours if possible
- ✅ **Be thorough:** Don't just "rubber stamp"
- ✅ **Praise good work:** Acknowledge clever solutions

#### Comment Prefixes
- **MUST:** Critical issue, must be fixed
- **SHOULD:** Strong suggestion, should be addressed
- **NITS:** Minor style/naming preference
- **Q:** Question for clarification
- **IMO:** Opinion/suggestion, author's choice
- **👍:** Compliment/approval

**Example:**
```
MUST: This endpoint is missing authentication. Please add [Authorize] attribute.

SHOULD: Consider extracting this logic into a separate method for reusability.

NITS: Rename variable to `customerId` (camelCase) for consistency.

Q: Why did you choose this approach over using the repository pattern?

IMO: You might want to consider caching this result.

👍 Nice use of value objects here!
```

---

## 🚦 CI/CD Pipeline

### Pipeline Overview

```
┌─────────────────┐
│ Developer Push  │
└────────┬────────┘
         │
         ▼
┌─────────────────────────────────────────────┐
│         Pull Request Created                │
└────────┬────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────┐
│  Automated Checks (Parallel Execution)      │
├─────────────────────────────────────────────┤
│  • Backend CI (build, test, coverage)       │
│  • Frontend CI (build, test, lint)          │
│  • Code Quality (format, security)          │
│  • CodeQL Security Analysis                 │
│  • Dependency Review                        │
└────────┬────────────────────────────────────┘
         │
         ├─ ❌ Checks Failed ─> Fix Issues ─┐
         │                                   │
         └─ ✅ Checks Passed                 │
         │                                   │
         ▼                                   │
┌─────────────────┐                         │
│  Code Review    │                         │
│  (1-2 approvals)│                         │
└────────┬────────┘                         │
         │                                   │
         ▼                                   │
┌─────────────────┐                         │
│  Merge to       │ ◄───────────────────────┘
│  develop        │
└────────┬────────┘
         │
         ▼
┌─────────────────────────────────────────────┐
│  Deploy to Staging (automatic)              │
└────────┬────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────┐
│  Smoke Tests on Staging                     │
└────────┬────────────────────────────────────┘
         │
    (Weekly Release)
         │
         ▼
┌─────────────────┐
│  Create Release │
│  PR to master   │
└────────┬────────┘
         │
         ▼
┌─────────────────────────────────────────────┐
│  Deploy to Production (manual approval)     │
└─────────────────────────────────────────────┘
```

### Workflow Files

#### 1. Backend CI (`.github/workflows/backend-ci.yml`)
**Triggers:** Push to `master`/`develop`, PRs to `master`/`develop`
**Jobs:**
- Restore .NET dependencies
- Build solution (Release config)
- Run unit tests (Category=Unit)
- Run integration tests (Category=Integration)
- Code coverage with Codecov
- Build Docker images
- Push images to GHCR (on master only)

**Required env/secrets:** None (uses GITHUB_TOKEN)

#### 2. Frontend CI (`.github/workflows/frontend-ci.yml`)
**Triggers:** Push to `master`/`develop`, PRs to `master`/`develop`
**Matrix:** `[public-portal, call-center-portal]`
**Jobs:**
- Install npm dependencies
- Run linter (ESLint)
- Build production bundle
- Run tests with coverage
- Build Docker images
- Push images to GHCR (on master only)

#### 3. Code Quality (`.github/workflows/code-quality.yml`)
**Triggers:** PRs to `master`/`develop`
**Jobs:**
- **Backend Quality:**
  - Code formatting check (`dotnet format`)
  - Static analysis (Roslynator)
  - Security vulnerability scan
- **Frontend Quality:**
  - ESLint
  - Prettier check
  - TypeScript type checking
  - npm audit
  - Bundle size report
- **Security:**
  - CodeQL analysis (C# and JavaScript)
  - Dependency review

#### 4. Deploy (`.github/workflows/deploy.yml`)
**Triggers:** Manual workflow_dispatch, Push to `master`
**Jobs:** (To be configured for your environment)
- Deploy to Azure
- Run database migrations
- Health checks
- Rollback on failure

### CI Status Badges

Add to your PR or README:

```markdown
[![Backend CI](https://github.com/smartsolutionslab/orange-car-rental-solution/workflows/Backend%20CI/badge.svg)](https://github.com/smartsolutionslab/orange-car-rental-solution/actions?query=workflow%3A%22Backend+CI%22)
[![Frontend CI](https://github.com/smartsolutionslab/orange-car-rental-solution/workflows/Frontend%20CI/badge.svg)](https://github.com/smartsolutionslab/orange-car-rental-solution/actions?query=workflow%3A%22Frontend+CI%22)
[![Code Quality](https://github.com/smartsolutionslab/orange-car-rental-solution/workflows/Code%20Quality/badge.svg)](https://github.com/smartsolutionslab/orange-car-rental-solution/actions?query=workflow%3A%22Code+Quality%22)
```

---

## 📝 Commit Message Standards

We follow **Conventional Commits** specification.

### Format
```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types
- **feat:** New feature
- **fix:** Bug fix
- **docs:** Documentation changes
- **style:** Code style (formatting, semicolons, etc.)
- **refactor:** Code refactoring (no feature or fix)
- **perf:** Performance improvement
- **test:** Adding or updating tests
- **build:** Build system changes
- **ci:** CI/CD changes
- **chore:** Other changes (dependencies, etc.)
- **revert:** Revert a previous commit

### Scopes
- **fleet:** Fleet Management service
- **reservations:** Reservations service
- **customers:** Customers service
- **pricing:** Pricing service
- **payments:** Payments service
- **notifications:** Notifications service
- **frontend:** Frontend changes
- **backend:** Backend changes
- **public-portal:** Public portal app
- **call-center-portal:** Call center portal app

### Examples

#### Feature Commit
```bash
git commit -m "feat(reservations): add reservation search endpoint

Implement GET /api/reservations/search with filtering by:
- Customer ID
- Status (Pending, Confirmed, etc.)
- Date range (pickup date from/to)
- Pagination support

Includes:
- Query handler and validation
- Repository search method
- Unit and integration tests
- API documentation

Implements US-7
Closes #7"
```

#### Bug Fix Commit
```bash
git commit -m "fix(pricing): correct VAT calculation for cross-location rentals

The VAT rate was incorrectly using pickup location instead of
return location for one-way rentals. This fixes the calculation
to use the appropriate location-based VAT rate.

Fixes #42"
```

#### Multiple Changes
```bash
git commit -m "refactor(customers): improve customer search performance

- Add database indexes on email and phone number
- Optimize query to avoid N+1 problem
- Cache location lookups
- Reduce payload size by projecting only needed fields

Performance improvement: Search time reduced from 2.5s to 0.3s

Related to #88"
```

### Commit Message Rules
1. ✅ Use imperative mood ("add" not "added" or "adds")
2. ✅ Don't capitalize first letter of subject
3. ✅ No period at end of subject
4. ✅ Subject line max 72 characters
5. ✅ Body wraps at 72 characters
6. ✅ Separate subject from body with blank line
7. ✅ Use body to explain **what** and **why**, not **how**
8. ✅ Reference issues/PRs in footer

---

## 🛡️ Branch Protection Rules

### Setting Up Branch Protection

#### For `master` Branch

1. Go to: https://github.com/smartsolutionslab/orange-car-rental-solution/settings/branches
2. Click "Add rule" or edit "master"
3. Configure:

```
Branch name pattern: master

✅ Require a pull request before merging
   ✅ Require approvals: 2
   ✅ Dismiss stale pull request approvals when new commits are pushed
   ✅ Require review from Code Owners

✅ Require status checks to pass before merging
   ✅ Require branches to be up to date before merging
   Required checks:
     • Backend CI / build-and-test
     • Frontend CI / build-and-test (public-portal)
     • Frontend CI / build-and-test (call-center-portal)
     • Code Quality / backend-quality
     • Code Quality / frontend-quality
     • Code Quality / codeql-analysis (csharp)
     • Code Quality / codeql-analysis (javascript)

✅ Require conversation resolution before merging

✅ Require linear history (prevents merge commits)

✅ Include administrators (even admins must follow rules)

✅ Restrict who can push to matching branches
   - Only service accounts for deployments

❌ Allow force pushes: Disabled
❌ Allow deletions: Disabled
```

#### For `develop` Branch

Same as master but:
```
✅ Require approvals: 1 (instead of 2)
```

### CODEOWNERS File

Create `.github/CODEOWNERS`:
```
# Global owners
* @heiko @team-lead

# Backend services
/src/backend/ @backend-team @heiko
/src/backend/Services/Fleet/ @fleet-team
/src/backend/Services/Reservations/ @reservations-team

# Frontend
/src/frontend/ @frontend-team @heiko
/src/frontend/apps/public-portal/ @frontend-team @ux-team
/src/frontend/apps/call-center-portal/ @frontend-team

# Infrastructure
/.github/workflows/ @devops-team @heiko
/deployment/ @devops-team

# Documentation
*.md @heiko
```

---

## 🚀 Release Process

### Creating a Release

#### 1. Prepare Release Branch
```bash
# From develop
git checkout develop
git pull origin develop

# Create release branch
git checkout -b release/v1.1.0

# Bump version numbers
# - Update package.json versions
# - Update AssemblyInfo versions
# - Update CHANGELOG.md

git add .
git commit -m "chore: bump version to 1.1.0"
git push -u origin release/v1.1.0
```

#### 2. Create Release PR to Master
```bash
gh pr create \
  --title "Release v1.1.0" \
  --body "# Release v1.1.0

## Features
- #7 - Reservation search and management APIs
- #4 - Booking history page
- #8 - Advanced filtering

## Bug Fixes
- #42 - VAT calculation fix
- #88 - Customer search performance

## Breaking Changes
None

## Migration Notes
1. Run database migrations
2. Update environment variables (see .env.example)

## Deployment Checklist
- [ ] Staging deployment successful
- [ ] Smoke tests passed
- [ ] Performance tests passed
- [ ] Security scan passed
- [ ] Documentation updated
- [ ] Release notes reviewed" \
  --base master \
  --head release/v1.1.0
```

#### 3. Merge to Master
- Requires 2 approvals
- All CI checks must pass
- QA team sign-off
- Product owner approval

#### 4. Tag Release
```bash
git checkout master
git pull origin master

# Create annotated tag
git tag -a v1.1.0 -m "Release v1.1.0

Features:
- Reservation management APIs
- Booking history
- Advanced filtering

See CHANGELOG.md for full details"

# Push tag
git push origin v1.1.0
```

#### 5. Create GitHub Release
```bash
gh release create v1.1.0 \
  --title "Version 1.1.0" \
  --notes-file RELEASE_NOTES.md \
  --target master
```

#### 6. Merge Back to Develop
```bash
# Ensure any release fixes are in develop
git checkout develop
git merge master
git push origin develop
```

#### 7. Deploy to Production
- Deployment auto-triggers from master push
- Manual approval required in GitHub Actions
- Monitor logs and metrics
- Rollback plan ready

### Hotfix Process

For critical production bugs:

```bash
# Create hotfix from master
git checkout master
git pull origin master
git checkout -b hotfix/99-critical-security-fix

# Make fix
git add .
git commit -m "fix(security): patch XSS vulnerability

Closes #99"
git push -u origin hotfix/99-critical-security-fix

# Create PR to master (expedited review)
gh pr create --base master --head hotfix/99-critical-security-fix

# After merge, also merge to develop
git checkout develop
git merge master
git push origin develop
```

---

## 📊 Workflow Metrics

Track these metrics to improve process:

### Velocity Metrics
- **Story points completed per sprint**
- **Average PR cycle time** (create → merge)
- **Deployment frequency**

### Quality Metrics
- **Test coverage percentage**
- **Code review round trips**
- **Production incidents**
- **Rollback frequency**

### Process Metrics
- **PR review time** (target: < 24 hours)
- **CI/CD success rate** (target: > 95%)
- **Branch lifetime** (target: < 3 days)

---

## 🆘 Common Issues & Solutions

### Issue: PR has merge conflicts
```bash
git checkout feature/my-branch
git fetch origin
git merge origin/develop
# Resolve conflicts
git add .
git commit -m "fix: resolve merge conflicts"
git push
```

### Issue: CI failing on formatting
```bash
cd src/backend
dotnet format OrangeCarRental.sln
git add .
git commit -m "style: fix code formatting"
git push
```

### Issue: Forgot to create branch from develop
```bash
git checkout develop
git pull origin develop
git checkout -b feature/correct-branch
git cherry-pick <commit-hash>  # Pick your commits
git push -u origin feature/correct-branch
```

### Issue: Need to update PR with latest develop
```bash
git checkout feature/my-branch
git fetch origin
git rebase origin/develop
# Resolve conflicts if any
git push --force-with-lease
```

---

## 📚 Additional Resources

- **GitHub Docs:** https://docs.github.com/
- **Conventional Commits:** https://www.conventionalcommits.org/
- **GitFlow:** https://nvie.com/posts/a-successful-git-branching-model/
- **Code Review Best Practices:** https://google.github.io/eng-practices/review/
- **Project Documentation:**
  - `USER_STORIES.md` - User story specifications
  - `ARCHITECTURE.md` - System architecture
  - `CONTRIBUTING.md` - GitHub setup guide
  - `GERMAN_MARKET_REQUIREMENTS.md` - Compliance requirements

---

## ✅ Quick Reference

### Daily Workflow
```bash
# Morning: Update local develop
git checkout develop && git pull

# Start feature
git checkout -b feature/US-X-description
# ... code ...
git add . && git commit -m "feat(scope): description"
git push

# Create PR
gh pr create --base develop --head feature/US-X-description

# After approval and merge
git checkout develop && git pull
git branch -d feature/US-X-description
```

### Common Commands
```bash
# Check status
git status
gh pr status

# View CI results
gh pr checks

# View diff
git diff develop

# Run local CI
dotnet test && npm run test
```

---

**Questions?** Open an issue or ask in the team Slack channel.

**Last Updated:** 2025-11-05
**Maintained by:** @heiko
