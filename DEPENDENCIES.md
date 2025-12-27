# Dependency Management - 12-Factor Principles

## Overview

This document describes the dependency management approach for Orange Car Rental, following **12-Factor App - Factor 2: Explicitly declare and isolate dependencies**.

## 12-Factor Dependency Principles

✅ **Explicitly declare all dependencies** - Use a dependency declaration manifest
✅ **Isolate dependencies** - Don't rely on implicit system-wide packages
✅ **Reproducible builds** - Same dependencies across all environments
✅ **Version pinning** - Exact versions for consistency

---

## Current Implementation Status

### ✅ **Backend (.NET) - Excellent Compliance**

#### **Central Package Management** ✅
```xml
<!-- Directory.Packages.props -->
<PropertyGroup>
  <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
</PropertyGroup>
```

**Benefits:**
- ✅ Single source of truth for package versions
- ✅ Prevents version conflicts across projects
- ✅ Easy to update dependencies project-wide
- ✅ Explicit version declarations

**Example:**
```xml
<!-- Directory.Packages.props -->
<PackageVersion Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
<PackageVersion Include="Serilog.AspNetCore" Version="8.0.3" />

<!-- Fleet.Api.csproj - No version needed -->
<PackageReference Include="Microsoft.EntityFrameworkCore" />
<PackageReference Include="Serilog.AspNetCore" />
```

#### **Declared Dependencies**
All NuGet packages are explicitly declared:
- **ASP.NET Core** - Web framework (v9.0.0)
- **Entity Framework Core** - ORM (v9.0.0)
- **.NET Aspire** - Orchestration (v9.5.2)
- **YARP** - Reverse proxy (v2.2.0)
- **Keycloak** - Authentication (v9.0.0)
- **Serilog** - Logging (v4.2.0 / v8.0.3)
- **xUnit** - Testing (v2.9.2)
- **Moq** - Mocking (v4.20.72)
- **FluentValidation** - Validation (v11.11.0)
- **Polly** - Resilience (v8.5.0)

Total: 29 packages explicitly versioned in `Directory.Packages.props`

---

### ⚠️ **Backend - Recommendations**

#### **1. Enable Deterministic Builds with Lock Files**

**Current:** No `packages.lock.json` files
**Risk:** Different developers/CI might get different transitive dependency versions

**Solution: Enable NuGet Lock Files**

**Add to `Directory.Build.props`:**
```xml
<Project>
  <PropertyGroup>
    <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
    <RestoreLockedMode Condition="'$(CI)' == 'true'">true</RestoreLockedMode>
  </PropertyGroup>
</Project>
```

**What this does:**
- Creates `packages.lock.json` in each project
- Locks transitive dependencies to exact versions
- CI builds use locked versions (fail if lockfile out of sync)
- Local dev can update dependencies

**Generate lock files:**
```bash
cd src/backend
dotnet restore --force-evaluate
```

---

#### **2. Dependency Audit Strategy**

**Check for vulnerable packages:**
```bash
# Check for security vulnerabilities
dotnet list package --vulnerable

# Check for outdated packages
dotnet list package --outdated

# Check for deprecated packages
dotnet list package --deprecated
```

**Schedule regular audits:**
- Weekly: Automated security scan in CI/CD
- Monthly: Review outdated packages
- Quarterly: Major version upgrades

---

### ✅ **Frontend (Node.js/Angular) - Good Compliance**

#### **Package Declaration** ✅
```json
{
  "dependencies": {
    "@angular/core": "^20.3.0",
    "keycloak-angular": "^20.0.0",
    "rxjs": "~7.8.0"
  },
  "devDependencies": {
    "@angular/cli": "^20.3.7",
    "@playwright/test": "^1.56.1",
    "typescript": "~5.9.2"
  }
}
```

**Strengths:**
- ✅ Separate production and dev dependencies
- ✅ Explicit version ranges
- ✅ `package-lock.json` for deterministic builds

#### **Semantic Versioning Ranges**

| Symbol | Meaning | Example | Updates To |
|--------|---------|---------|-----------|
| `^` | Compatible | `^20.3.0` | `20.x.x` (minor + patch) |
| `~` | Patch | `~7.8.0` | `7.8.x` (patch only) |
| none | Exact | `20.0.0` | Locked |

---

### ⚠️ **Frontend - Recommendations**

#### **1. Stricter Version Pinning for Production**

**Current Issue:** Caret ranges (`^`) allow minor version updates

**Example Risk:**
```json
"@angular/core": "^20.3.0"  // Could install 20.4.0, 20.5.0, etc.
```

**Problem:** Minor versions can introduce breaking changes despite semantic versioning

**Solution Options:**

**Option A: Use Exact Versions (Recommended for Production)**
```json
{
  "dependencies": {
    "@angular/core": "20.3.0",           // Exact - no automatic updates
    "keycloak-angular": "20.0.0",
    "rxjs": "7.8.0"
  }
}
```

**Option B: Use Tilde for Patch-Only Updates**
```json
{
  "dependencies": {
    "@angular/core": "~20.3.0",          // Only 20.3.x
    "keycloak-angular": "~20.0.0"
  }
}
```

**Option C: Keep Carets with Lock File (Current Approach)**
- Uses `package-lock.json` to lock actual installed versions
- Allows controlled updates via `npm update`
- **This is acceptable if:**
  - Lock file is committed to git ✅
  - CI uses `npm ci` instead of `npm install` ✅
  - Regular testing of updates

---

#### **2. Dependency Security Scanning**

**Check for vulnerabilities:**
```bash
# Audit npm packages
npm audit

# Fix automatically (with caution)
npm audit fix

# Fix breaking changes
npm audit fix --force

# Generate detailed report
npm audit --json > audit-report.json
```

**Recommendation:** Add to CI/CD pipeline
```yaml
# .github/workflows/security-audit.yml
name: Security Audit

on:
  schedule:
    - cron: '0 0 * * 1'  # Weekly on Monday
  push:
    paths:
      - '**/package.json'
      - '**/package-lock.json'

jobs:
  audit:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Setup Node
        uses: actions/setup-node@v4
        with:
          node-version: '22'
      - name: Audit Dependencies
        run: |
          cd src/frontend/apps/public-portal
          npm audit --audit-level=moderate
```

---

#### **3. Renovate Bot for Automated Updates**

**Problem:** Manual dependency updates are tedious and error-prone

**Solution:** Automate with Renovate Bot

**Create `renovate.json`:**
```json
{
  "$schema": "https://docs.renovatebot.com/renovate-schema.json",
  "extends": ["config:base"],
  "packageRules": [
    {
      "matchPackagePatterns": ["*"],
      "matchUpdateTypes": ["major"],
      "automerge": false,
      "reviewers": ["team:platform"]
    },
    {
      "matchPackagePatterns": ["*"],
      "matchUpdateTypes": ["minor", "patch"],
      "automerge": true,
      "minimumReleaseAge": "3 days"
    },
    {
      "matchPackagePatterns": ["@angular/*", "@playwright/*"],
      "groupName": "Angular ecosystem",
      "schedule": ["before 4am on Monday"]
    }
  ],
  "vulnerabilityAlerts": {
    "labels": ["security"],
    "assignees": ["@security-team"]
  }
}
```

**Benefits:**
- Automated PR creation for updates
- Security vulnerability alerts
- Grouped updates (e.g., all Angular packages together)
- Configurable auto-merge rules

---

## Dependency Isolation

### ✅ **Backend - No System Dependencies**

.NET SDK is self-contained:
- ✅ No implicit dependencies on system libraries
- ✅ Framework-dependent deployment uses shared runtime
- ✅ Self-contained deployment bundles everything

**Verify isolation:**
```bash
# Check for any system dependencies
dotnet list package --include-transitive | grep -i "System\."
```

All `System.*` packages are part of .NET runtime, not system libraries.

---

### ✅ **Frontend - npm Isolation**

Node.js dependencies are isolated in `node_modules`:
- ✅ No reliance on global npm packages
- ✅ All dependencies in `package.json`
- ✅ Deterministic with `package-lock.json`

**Anti-Pattern to Avoid:**
```bash
# ❌ DON'T rely on global packages
npm install -g @angular/cli
ng build  # Expects global CLI

# ✅ DO use local packages
npm install
npx ng build  # Uses local node_modules/.bin/ng
```

---

## Dependency Documentation

### **Backend Dependency Categories**

#### **Core Framework (9.0.0)**
```
Microsoft.AspNetCore.OpenApi
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
```

#### **Aspire Orchestration (9.5.2)**
```
Aspire.Hosting.AppHost
Aspire.Hosting.SqlServer
Aspire.Microsoft.EntityFrameworkCore.SqlServer
Microsoft.Extensions.ServiceDiscovery
```

#### **Authentication (9.0.0)**
```
Microsoft.AspNetCore.Authentication.JwtBearer
```

#### **API Gateway (2.2.0)**
```
Yarp.ReverseProxy
```

#### **Logging (4.2.0 / 8.0.3)**
```
Serilog
Serilog.AspNetCore
Serilog.Sinks.Console
Serilog.Enrichers.Environment
```

#### **Testing (2.9.2 / 17.11.1)**
```
xunit
xunit.runner.visualstudio
Microsoft.NET.Test.Sdk
Moq (4.20.72)
Shouldly (4.2.1)
```

#### **Validation & Resilience**
```
FluentValidation (11.11.0)
Polly (8.5.0)
```

#### **API Documentation**
```
Scalar.AspNetCore (1.2.41)
```

---

### **Frontend Dependency Categories**

#### **Angular Framework (20.3.0)**
```
@angular/core
@angular/common
@angular/compiler
@angular/forms
@angular/platform-browser
@angular/router
```

#### **Authentication**
```
keycloak-angular (20.0.0)
keycloak-js (26.2.1)
```

#### **Reactive Programming**
```
rxjs (~7.8.0)
zone.js (~0.15.0)
```

#### **Testing**
```
jasmine-core (~5.9.0)
karma (~6.4.0)
@playwright/test (^1.56.1)
puppeteer (^24.30.0)
```

#### **Build Tools**
```
@angular/cli (^20.3.7)
typescript (~5.9.2)
```

---

## Best Practices Checklist

### **Backend (.NET)**
- [x] Central Package Management enabled
- [x] Explicit version pinning
- [x] No version wildcards
- [ ] **TODO:** Enable NuGet lock files (`packages.lock.json`)
- [ ] **TODO:** Add automated security scanning to CI/CD
- [x] Separate test dependencies
- [x] No implicit system dependencies

### **Frontend (Node.js)**
- [x] `package.json` with all dependencies
- [x] `package-lock.json` committed to git
- [x] Separate dev and prod dependencies
- [ ] **TODO:** Consider stricter version pinning (replace `^` with `~` or exact)
- [ ] **TODO:** Add npm audit to CI/CD
- [ ] **TODO:** Set up Renovate Bot for automated updates
- [x] Use `npm ci` in CI/CD (not `npm install`)
- [x] No global npm dependencies in scripts

---

## Migration Plan

### **Phase 1: Enable Lock Files (Backend)** ⏳

1. Create `Directory.Build.props`:
```xml
<Project>
  <PropertyGroup>
    <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
    <RestoreLockedMode Condition="'$(CI)' == 'true'">true</RestoreLockedMode>
  </PropertyGroup>
</Project>
```

2. Generate lock files:
```bash
cd src/backend
dotnet restore --force-evaluate
```

3. Commit `packages.lock.json` files:
```bash
git add **/packages.lock.json
git commit -m "chore: enable NuGet package lock files for deterministic builds"
```

4. Update CI/CD to use `--locked-mode`:
```bash
dotnet restore --locked-mode
dotnet build --no-restore
```

---

### **Phase 2: Security Scanning** ⏳

**Backend:**
```yaml
# Add to GitHub Actions
- name: Check for vulnerable packages
  run: dotnet list package --vulnerable --include-transitive
```

**Frontend:**
```yaml
# Add to GitHub Actions
- name: Audit npm dependencies
  run: |
    cd src/frontend/apps/public-portal
    npm audit --audit-level=moderate
```

---

### **Phase 3: Automated Updates** ⏳

1. Enable Dependabot or Renovate Bot
2. Configure auto-merge rules for patch updates
3. Require manual review for major updates
4. Group related dependencies (e.g., all Angular packages)

---

## Verification Commands

### **Backend**
```bash
# Verify Central Package Management
cat src/backend/Directory.Packages.props | grep ManagePackageVersionsCentrally

# List all packages with versions
dotnet list src/backend package

# Check for security vulnerabilities
dotnet list src/backend package --vulnerable

# Check for outdated packages
dotnet list src/backend package --outdated
```

### **Frontend**
```bash
# Verify lock file exists
test -f src/frontend/apps/public-portal/package-lock.json && echo "✅ Lock file exists"

# Audit dependencies
cd src/frontend/apps/public-portal
npm audit

# Check for outdated packages
npm outdated

# List all installed packages
npm list --depth=0
```

---

## Additional Resources

- [12-Factor App - Dependencies](https://12factor.net/dependencies)
- [NuGet Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
- [NuGet Lock Files](https://learn.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files#locking-dependencies)
- [npm package-lock.json](https://docs.npmjs.com/cli/v10/configuring-npm/package-lock-json)
- [Renovate Bot](https://docs.renovatebot.com/)
- [GitHub Dependabot](https://docs.github.com/en/code-security/dependabot)

---

## Summary

| Aspect | Status | Priority |
|--------|--------|----------|
| Backend: Central Package Management | ✅ Implemented | - |
| Backend: Explicit Versions | ✅ Implemented | - |
| Backend: Lock Files | ❌ Missing | 🔴 High |
| Backend: Security Scanning | ❌ Missing | 🟡 Medium |
| Frontend: package.json | ✅ Implemented | - |
| Frontend: package-lock.json | ✅ Implemented | - |
| Frontend: Version Pinning | ⚠️ Using Carets | 🟢 Low |
| Frontend: Security Scanning | ❌ Missing | 🟡 Medium |
| Automated Updates | ❌ Missing | 🟢 Low |

**Next Steps:** Implement Phase 1 (Enable NuGet Lock Files) for full 12-Factor compliance.
