# CI/CD Pipelines Documentation

This directory contains the GitHub Actions workflows for the Orange Car Rental system. The pipelines provide comprehensive automation for building, testing, code quality checks, and deployment.

## 📋 Overview

The CI/CD setup consists of four main workflows:

1. **Backend CI** (`backend-ci.yml`) - Build, test, and package backend services
2. **Frontend CI** (`frontend-ci.yml`) - Build, test, and package frontend applications
3. **Code Quality** (`code-quality.yml`) - Enforce code standards and security
4. **Deploy** (`deploy.yml`) - Deploy to staging and production environments

## 🔄 Workflow Triggers

### Backend CI
- **Triggers**: Push or PR to `main` or `develop` branches
- **Path filters**: `src/backend/**`, `.github/workflows/backend-ci.yml`
- **What it does**:
  - Restores dependencies and builds .NET solution
  - Runs unit tests (filtered by `Category=Unit`)
  - Runs integration tests (filtered by `Category=Integration`)
  - Uploads test results and coverage to Codecov
  - Builds Docker images for all microservices
  - Pushes images to GitHub Container Registry (on `main` branch only)

### Frontend CI
- **Triggers**: Push or PR to `main` or `develop` branches
- **Path filters**: `src/frontend/**`, `.github/workflows/frontend-ci.yml`
- **What it does**:
  - Installs dependencies for both Angular apps (public-portal, call-center-portal)
  - Runs linting (if configured)
  - Builds applications in production mode
  - Runs tests with headless Chrome and code coverage
  - Builds Docker images for both apps
  - Pushes images to GitHub Container Registry (on `main` branch only)

### Code Quality
- **Triggers**: Push or PR to `main` or `develop` branches
- **What it does**:
  - **Backend**:
    - Checks code formatting with `dotnet format`
    - Runs Roslynator static analysis
    - Scans for security vulnerabilities in NuGet packages
    - Optional SonarQube integration (commented out)
  - **Frontend**:
    - Runs ESLint (if configured)
    - Checks Prettier formatting
    - Performs TypeScript compilation checks
    - Scans for npm security vulnerabilities
    - Analyzes bundle size
  - **Security**:
    - Dependency review for pull requests
    - CodeQL security analysis for C# and JavaScript

### Deploy
- **Triggers**:
  - Automatically after successful CI workflow completion on `main` or `develop`
  - Manual dispatch via GitHub UI
- **What it does**:
  - Determines target environment (staging for `develop`, production for `main`)
  - Pulls Docker images from GitHub Container Registry
  - Deploys backend services and frontend applications
  - Sends deployment notifications

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Code Push/PR                              │
└────────────┬────────────────────────┬───────────────────────┘
             │                        │
             ▼                        ▼
    ┌────────────────┐       ┌────────────────┐
    │  Backend CI    │       │  Frontend CI   │
    │                │       │                │
    │ - Build        │       │ - Build        │
    │ - Test         │       │ - Test         │
    │ - Docker Build │       │ - Docker Build │
    └────────┬───────┘       └────────┬───────┘
             │                        │
             └────────────┬───────────┘
                          ▼
                ┌──────────────────┐
                │   Code Quality   │
                │                  │
                │ - Format Check   │
                │ - Linting        │
                │ - Security Scan  │
                │ - CodeQL         │
                └────────┬─────────┘
                         │
                         ▼
                ┌─────────────────┐
                │     Deploy      │
                │                 │
                │ - Staging       │
                │ - Production    │
                └─────────────────┘
```

## 🚀 Deployment Strategy

### Environments

- **Staging**: Automatically deployed from `develop` branch
- **Production**: Automatically deployed from `main` branch (with environment protection)

### Image Tagging Strategy

- **Develop branch**: Images are tagged as `latest`
- **Main branch**: Images are tagged with the commit SHA for immutability

### Deployment Options

The deployment workflow includes commented-out examples for various platforms:

#### Backend Deployment Options:
1. **Azure Container Apps** - For serverless container hosting
2. **Azure App Service** - For managed container hosting
3. **Kubernetes** - For self-managed orchestration
4. **Docker Compose** - For traditional server deployment via SSH

#### Frontend Deployment Options:
1. **Azure Static Web Apps** - For serverless static hosting
2. **Azure Blob Storage** - For CDN-backed static hosting
3. **Netlify** - For third-party static hosting
4. **Vercel** - For third-party static hosting with edge functions

## 🔧 Configuration

### Required Secrets

To enable deployments, configure the following secrets in GitHub repository settings:

#### For Azure deployments:
```
AZURE_RESOURCE_GROUP
AZURE_STORAGE_ACCOUNT
AZURE_STATIC_WEB_APPS_API_TOKEN
```

#### For SSH-based deployments:
```
DEPLOY_HOST
DEPLOY_USER
DEPLOY_SSH_KEY
```

#### For SonarQube analysis:
```
SONAR_TOKEN
SONAR_HOST_URL
```

#### For notifications:
```
SLACK_WEBHOOK_URL
```

### Environment Protection Rules

For production deployments, configure environment protection rules:

1. Go to **Settings** → **Environments** → **production**
2. Enable **Required reviewers** (recommended)
3. Set **Wait timer** if needed
4. Configure **Deployment branches** to allow only `main`

## 📊 Test Categories

The backend tests use categories to separate unit and integration tests:

```csharp
[Fact]
[Trait("Category", "Unit")]
public void UnitTest_Example()
{
    // Unit test implementation
}

[Fact]
[Trait("Category", "Integration")]
public void IntegrationTest_Example()
{
    // Integration test implementation
}
```

## 🐳 Docker Images

### Backend Services

All backend services are built and pushed to GitHub Container Registry:

- `ghcr.io/<repository>/fleet-api:tag`
- `ghcr.io/<repository>/reservations-api:tag`
- `ghcr.io/<repository>/customers-api:tag`
- `ghcr.io/<repository>/pricing-api:tag`
- `ghcr.io/<repository>/payments-api:tag`
- `ghcr.io/<repository>/notifications-api:tag`

### Frontend Applications

- `ghcr.io/<repository>/public-portal:tag`
- `ghcr.io/<repository>/call-center-portal:tag`

## 📝 Local Testing

### Backend

```bash
# Run all tests
dotnet test src/backend/OrangeCarRental.sln

# Run only unit tests
dotnet test src/backend/OrangeCarRental.sln --filter "Category=Unit"

# Run only integration tests
dotnet test src/backend/OrangeCarRental.sln --filter "Category=Integration"

# Check code formatting
dotnet format src/backend/OrangeCarRental.sln --verify-no-changes

# Check for vulnerabilities
dotnet list package --vulnerable --include-transitive
```

### Frontend

```bash
# Install dependencies
cd src/frontend/apps/public-portal
npm ci

# Run tests
npm run test

# Run linting
npm run lint

# Check formatting
npx prettier --check "src/**/*.{ts,html,css,scss,json}"

# Build for production
npm run build -- --configuration production

# Check for vulnerabilities
npm audit --audit-level=high
```

## 🔐 Security Features

1. **Dependency Scanning**: Automatic scanning for vulnerable packages
2. **CodeQL Analysis**: Deep code analysis for security vulnerabilities
3. **Dependency Review**: Reviews new dependencies in pull requests
4. **License Compliance**: Blocks problematic licenses (GPL-2.0, GPL-3.0)
5. **Container Scanning**: GitHub automatically scans pushed Docker images

## 🎯 Best Practices

1. **Always run tests locally** before pushing
2. **Keep dependencies up to date** to avoid security issues
3. **Use semantic versioning** for releases
4. **Write meaningful commit messages** for better traceability
5. **Tag releases** in production for rollback capability
6. **Monitor deployment logs** in GitHub Actions
7. **Enable branch protection** on `main` and `develop` branches

## 🐛 Troubleshooting

### Build Failures

1. Check the specific step that failed in GitHub Actions logs
2. Reproduce locally using the same commands
3. Verify dependencies are up to date
4. Check for environment-specific issues

### Test Failures

1. Run tests locally to reproduce
2. Check if integration tests need database/services
3. Verify test data and fixtures are correct
4. Review recent code changes that might affect tests

### Deployment Failures

1. Verify all required secrets are configured
2. Check environment permissions
3. Validate Docker image availability in registry
4. Review deployment logs for specific errors

## 📚 Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [Angular Testing Guide](https://angular.dev/guide/testing)
- [Docker Multi-Stage Builds](https://docs.docker.com/build/building/multi-stage/)
- [CodeQL Documentation](https://codeql.github.com/docs/)

## 🤝 Contributing

When adding or modifying workflows:

1. Test changes in a feature branch first
2. Use workflow dispatch for manual testing
3. Document any new required secrets or configurations
4. Update this README with relevant changes
5. Get review from team leads before merging
