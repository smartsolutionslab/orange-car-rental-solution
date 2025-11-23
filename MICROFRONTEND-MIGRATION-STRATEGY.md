# Microfrontend Migration Strategy

## Executive Summary

This document outlines the strategy for migrating the Orange Car Rental frontend from a **multi-app monorepo** architecture to a **Module Federation-based microfrontend** architecture. The migration will enable runtime composition, independent deployments, and better team autonomy while maintaining code sharing capabilities.

## Table of Contents

- [Current Architecture](#current-architecture)
- [Target Architecture](#target-architecture)
- [Technology Stack](#technology-stack)
- [Migration Approach](#migration-approach)
- [Implementation Phases](#implementation-phases)
- [Technical Implementation](#technical-implementation)
- [Deployment Strategy](#deployment-strategy)
- [Testing Strategy](#testing-strategy)
- [Rollback Plan](#rollback-plan)
- [Timeline and Resources](#timeline-and-resources)

---

## Current Architecture

### Overview

**Architecture Type**: Multi-app monorepo with shared libraries

### Current Structure

```
src/frontend/
├── apps/
│   ├── public-portal/          # Customer-facing SPA (port 4200)
│   └── call-center-portal/     # Internal staff SPA (port 4201)
└── libs/
    ├── data-access/            # Shared data services
    ├── shared-ui/              # Shared UI components
    ├── ui-components/          # Reusable UI components
    └── util/                   # Utility functions
```

### Characteristics

✅ **Strengths**:
- Code sharing via monorepo libraries
- Consistent build tooling (Angular CLI)
- Shared dependencies and versioning
- Simple development setup

❌ **Limitations**:
- No runtime composition
- Cannot deploy apps independently
- Entire app must reload for any update
- Tight coupling at build time
- Large bundle sizes (duplicate vendor code)
- Team dependencies for releases

### Current Deployment

Each app is:
- Built separately as a complete SPA
- Containerized with NGINX
- Deployed to separate Kubernetes pods
- Served on different domains/paths

---

## Target Architecture

### Overview

**Architecture Type**: Module Federation-based microfrontends with shell application

### Target Structure

```
src/frontend/
├── apps/
│   ├── shell/                  # Host/container application (NEW)
│   │   ├── src/
│   │   ├── module-federation.config.ts
│   │   └── package.json
│   ├── public-portal/          # Remote microfrontend #1
│   │   ├── src/
│   │   ├── module-federation.config.ts
│   │   └── package.json
│   └── call-center-portal/     # Remote microfrontend #2
│       ├── src/
│       ├── module-federation.config.ts
│       └── package.json
└── libs/
    ├── data-access/            # Shared libraries (unchanged)
    ├── shared-ui/
    ├── ui-components/
    └── util/
```

### Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    SHELL APPLICATION                         │
│  (Host - Port 4200)                                          │
│  - Routing & Navigation                                      │
│  - Authentication                                            │
│  - Layout & Theme                                            │
│  - Module Federation Host Config                            │
└──────────────┬────────────────────────────┬─────────────────┘
               │                            │
               │ Load at Runtime            │ Load at Runtime
               ▼                            ▼
    ┌──────────────────────┐    ┌──────────────────────┐
    │  PUBLIC PORTAL       │    │  CALL CENTER PORTAL  │
    │  (Remote - Port 4201)│    │  (Remote - Port 4202)│
    │  - Booking Features  │    │  - Reservation Mgmt  │
    │  - Vehicle Search    │    │  - Customer Support  │
    │  - User Profile      │    │  - Fleet Management  │
    └──────────────────────┘    └──────────────────────┘
               │                            │
               └────────────┬───────────────┘
                            ▼
                 ┌──────────────────────┐
                 │   SHARED LIBRARIES   │
                 │  - UI Components     │
                 │  - Data Access       │
                 │  - Utilities         │
                 └──────────────────────┘
```

### Key Benefits

✅ **Independent Deployment**:
- Deploy microfrontends without rebuilding shell
- Update features without full app redeployment
- Reduced deployment risk

✅ **Runtime Composition**:
- Load modules on-demand
- Dynamic feature toggling
- A/B testing capabilities

✅ **Team Autonomy**:
- Teams own complete features
- Independent release cycles
- Reduced coordination overhead

✅ **Performance**:
- Shared vendor chunks (React, Angular core)
- Lazy loading of microfrontends
- Smaller initial bundle size

✅ **Scalability**:
- Add new microfrontends easily
- Scale teams independently
- Better code organization

---

## Technology Stack

### Module Federation

**Webpack Module Federation** (Native Federation for Angular)

#### Why Module Federation?

1. **Native Angular Support**: `@angular-architects/native-federation`
2. **Runtime Loading**: Modules loaded dynamically at runtime
3. **Version Agnostic**: Different Angular versions possible (with care)
4. **Shared Dependencies**: Automatic vendor chunk deduplication
5. **Production Ready**: Used by major enterprises

#### Alternatives Considered

| Technology | Pros | Cons | Decision |
|------------|------|------|----------|
| **Module Federation** | Native Angular support, runtime loading, shared deps | Learning curve | ✅ **SELECTED** |
| **Single-SPA** | Framework agnostic, mature | More boilerplate, complex setup | ❌ Not Angular-optimized |
| **Iframe-based** | Simple, complete isolation | Performance overhead, communication complexity | ❌ Poor UX |
| **Web Components** | Standard-based, framework agnostic | Limited Angular integration | ❌ Immature ecosystem |

### Supporting Technologies

| Component | Technology | Purpose |
|-----------|-----------|---------|
| **Build Tool** | Angular CLI + Native Federation | Building microfrontends |
| **Routing** | Angular Router | Cross-microfrontend navigation |
| **State Management** | RxJS + Shared Services | Cross-app communication |
| **Authentication** | Keycloak (existing) | SSO across microfrontends |
| **API Gateway** | YARP (existing) | Backend API routing |
| **Deployment** | Kubernetes + NGINX | Container orchestration |

---

## Migration Approach

### Strategy: **Strangler Fig Pattern**

Gradually migrate to microfrontends while keeping existing apps functional.

### Phases

```
Phase 1: Setup & Infrastructure
    ↓
Phase 2: Create Shell Application
    ↓
Phase 3: Migrate Public Portal
    ↓
Phase 4: Migrate Call Center Portal
    ↓
Phase 5: Optimization & Cleanup
```

### Parallel Operation

During migration:
- ✅ Old SPAs remain functional
- ✅ New microfrontends run in parallel
- ✅ Gradual traffic shifting
- ✅ Easy rollback at any phase

---

## Implementation Phases

### Phase 1: Setup & Infrastructure (Week 1)

#### Objectives
- Install Module Federation tooling
- Configure build infrastructure
- Set up development environment

#### Tasks

**1.1 Install Dependencies**
```bash
# Install Native Federation
npm install @angular-architects/native-federation -D

# Install Module Federation DevTools
npm install @module-federation/utilities -D
```

**1.2 Update Build Configuration**
- Configure webpack for Module Federation
- Set up dynamic port allocation
- Configure CORS for local development

**1.3 Update Package Scripts**
```json
{
  "scripts": {
    "start:shell": "ng serve shell --port 4200",
    "start:public": "ng serve public-portal --port 4201",
    "start:callcenter": "ng serve call-center-portal --port 4202",
    "start:all": "concurrently \"npm:start:*\""
  }
}
```

**1.4 Documentation**
- Create developer guide for microfrontends
- Document module federation concepts
- Update README with new architecture

#### Deliverables
- ✅ Module Federation installed and configured
- ✅ Development environment ready
- ✅ Documentation updated

#### Success Criteria
- All apps still build and run independently
- No breaking changes to existing functionality
- Development team trained on new architecture

---

### Phase 2: Create Shell Application (Week 2)

#### Objectives
- Create shell/host application
- Implement routing framework
- Set up authentication flow

#### Tasks

**2.1 Generate Shell Application**
```bash
cd src/frontend
npx ng generate application shell --routing --style=css
```

**2.2 Configure Shell as Module Federation Host**

Create `src/frontend/apps/shell/module-federation.config.ts`:
```typescript
import { ModuleFederationConfig } from '@angular-architects/native-federation';

const config: ModuleFederationConfig = {
  name: 'shell',

  remotes: {
    'publicPortal': 'http://localhost:4201/remoteEntry.json',
    'callCenterPortal': 'http://localhost:4202/remoteEntry.json',
  },

  shared: {
    '@angular/core': { singleton: true, strictVersion: true },
    '@angular/common': { singleton: true, strictVersion: true },
    '@angular/router': { singleton: true, strictVersion: true },
    'keycloak-angular': { singleton: true, strictVersion: true },
    'rxjs': { singleton: true, strictVersion: false },
  },
};

export default config;
```

**2.3 Implement Shell Routing**

`src/frontend/apps/shell/src/app/app.routes.ts`:
```typescript
import { Routes } from '@angular/router';
import { loadRemoteModule } from '@angular-architects/native-federation';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadComponent: () => import('./home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'booking',
    loadChildren: () =>
      loadRemoteModule('publicPortal', './Routes').then(m => m.routes)
  },
  {
    path: 'admin',
    loadChildren: () =>
      loadRemoteModule('callCenterPortal', './Routes').then(m => m.routes),
    canActivate: [AuthGuard]
  },
  {
    path: '**',
    redirectTo: '/home'
  }
];
```

**2.4 Shell Layout & Navigation**
- Create shared header/footer components
- Implement main navigation menu
- Set up responsive layout

**2.5 Authentication Integration**
- Configure Keycloak in shell
- Implement SSO across microfrontends
- Share authentication state

**2.6 Shared Services**
- Create EventBus service for cross-app communication
- Implement shared state management
- Set up error handling

#### Deliverables
- ✅ Shell application running on port 4200
- ✅ Routing framework with lazy loading
- ✅ Authentication working in shell
- ✅ Navigation and layout components

#### Success Criteria
- Shell application loads successfully
- Can navigate between routes
- Authentication persists across navigation
- No console errors

---

### Phase 3: Migrate Public Portal (Week 3-4)

#### Objectives
- Convert public-portal to Module Federation remote
- Expose routes and components
- Integrate with shell application

#### Tasks

**3.1 Configure Public Portal as Remote**

Create `src/frontend/apps/public-portal/module-federation.config.ts`:
```typescript
import { ModuleFederationConfig } from '@angular-architects/native-federation';

const config: ModuleFederationConfig = {
  name: 'publicPortal',

  exposes: {
    './Routes': './src/app/app.routes.ts',
    './BookingModule': './src/app/pages/booking/booking.routes.ts',
    './VehicleSearchModule': './src/app/pages/vehicles/vehicles.routes.ts',
  },

  shared: {
    '@angular/core': { singleton: true, strictVersion: true },
    '@angular/common': { singleton: true, strictVersion: true },
    '@angular/router': { singleton: true, strictVersion: true },
    'keycloak-angular': { singleton: true, strictVersion: true },
    'rxjs': { singleton: true, strictVersion: false },
  },
};

export default config;
```

**3.2 Update Public Portal Routing**

Refactor routes to be loadable by shell:
```typescript
import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'search',
        loadComponent: () =>
          import('./pages/vehicles/vehicle-search.component')
            .then(m => m.VehicleSearchComponent)
      },
      {
        path: 'booking',
        loadComponent: () =>
          import('./pages/booking/booking.component')
            .then(m => m.BookingComponent)
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./pages/profile/profile.component')
            .then(m => m.ProfileComponent)
      }
    ]
  }
];
```

**3.3 Remove Shell Concerns**
- Remove top-level navigation (now in shell)
- Remove authentication init (handled by shell)
- Keep only feature-specific code

**3.4 Update Base HREF**
- Configure for sub-path routing
- Update asset paths
- Fix relative URLs

**3.5 Testing**
- Test standalone (port 4201)
- Test integrated with shell
- Verify all features work

#### Deliverables
- ✅ Public portal runs as Module Federation remote
- ✅ Routes exposed and loadable by shell
- ✅ All features working in both modes
- ✅ Tests passing

#### Success Criteria
- Public portal loads in shell via Module Federation
- Navigation works between shell and remote
- Authentication state shared correctly
- All existing features functional

---

### Phase 4: Migrate Call Center Portal (Week 5)

#### Objectives
- Convert call-center-portal to Module Federation remote
- Expose admin routes and components
- Integrate with shell application

#### Tasks

**4.1 Configure Call Center Portal as Remote**

Similar to public portal, create module federation config:
```typescript
const config: ModuleFederationConfig = {
  name: 'callCenterPortal',

  exposes: {
    './Routes': './src/app/app.routes.ts',
    './ReservationsModule': './src/app/pages/reservations/reservations.routes.ts',
    './CustomersModule': './src/app/pages/customers/customers.routes.ts',
  },

  shared: { /* same as public portal */ },
};
```

**4.2 Update Routing**
- Refactor routes for remote loading
- Implement role-based access control
- Configure admin-specific layouts

**4.3 Remove Shell Concerns**
- Same as public portal
- Keep admin-specific features only

**4.4 Integration Testing**
- Test with shell application
- Verify admin features work
- Check role-based access

#### Deliverables
- ✅ Call center portal as Module Federation remote
- ✅ Admin routes exposed and loadable
- ✅ Role-based access working
- ✅ Tests passing

#### Success Criteria
- Call center portal loads in shell
- Admin navigation works correctly
- Only authorized users can access
- All features functional

---

### Phase 5: Optimization & Cleanup (Week 6)

#### Objectives
- Optimize bundle sizes
- Improve performance
- Clean up deprecated code

#### Tasks

**5.1 Bundle Optimization**
- Analyze bundle sizes
- Remove duplicate dependencies
- Configure shared chunks optimally

**5.2 Performance Optimization**
- Implement preloading strategies
- Optimize lazy loading
- Add loading indicators

**5.3 Code Cleanup**
- Remove old standalone app code
- Update Docker configurations
- Clean up package dependencies

**5.4 Documentation**
- Update architecture diagrams
- Document Module Federation patterns
- Create troubleshooting guide

**5.5 Monitoring & Observability**
- Add performance metrics
- Implement error tracking
- Set up logging

#### Deliverables
- ✅ Optimized bundle sizes
- ✅ Performance improvements documented
- ✅ Clean codebase
- ✅ Complete documentation

#### Success Criteria
- Bundle size reduced by >30%
- Initial load time <2s
- No deprecated code remaining
- Documentation complete

---

## Technical Implementation

### Module Federation Configuration

#### Shared Dependencies Strategy

**Singleton Libraries** (strict versioning):
```typescript
shared: {
  '@angular/core': {
    singleton: true,
    strictVersion: true,
    requiredVersion: '^20.3.0'
  },
  '@angular/common': {
    singleton: true,
    strictVersion: true
  },
  '@angular/router': {
    singleton: true,
    strictVersion: true
  },
  'keycloak-angular': {
    singleton: true,
    strictVersion: true
  },
}
```

**Flexible Libraries** (version ranges):
```typescript
shared: {
  'rxjs': {
    singleton: true,
    strictVersion: false,
    requiredVersion: '^7.8.0'
  },
  'tslib': {
    singleton: false
  },
}
```

### Cross-App Communication

#### Event Bus Pattern

```typescript
// src/frontend/libs/shared/src/lib/services/event-bus.service.ts
import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';

export interface MicroFrontendEvent {
  type: string;
  payload: any;
}

@Injectable({ providedIn: 'root' })
export class EventBusService {
  private eventSubject = new Subject<MicroFrontendEvent>();

  emit(type: string, payload: any): void {
    this.eventSubject.next({ type, payload });
  }

  on<T = any>(type: string): Observable<T> {
    return this.eventSubject.pipe(
      filter(event => event.type === type),
      map(event => event.payload)
    );
  }
}
```

#### Usage Example

```typescript
// Public Portal - emit event
constructor(private eventBus: EventBusService) {}

bookingCompleted(booking: Booking) {
  this.eventBus.emit('BOOKING_COMPLETED', booking);
}

// Shell - listen to event
ngOnInit() {
  this.eventBus.on('BOOKING_COMPLETED').subscribe(booking => {
    console.log('Booking completed:', booking);
    this.showSuccessNotification(booking);
  });
}
```

### Authentication Sharing

#### Keycloak Initialization in Shell

```typescript
// src/frontend/apps/shell/src/app/app.config.ts
import { APP_INITIALIZER } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';

function initializeKeycloak(keycloak: KeycloakService) {
  return () =>
    keycloak.init({
      config: {
        url: environment.keycloakUrl,
        realm: 'orange-car-rental',
        clientId: 'shell-app'
      },
      initOptions: {
        onLoad: 'check-sso',
        silentCheckSsoRedirectUri:
          window.location.origin + '/assets/silent-check-sso.html'
      }
    });
}

export const appConfig: ApplicationConfig = {
  providers: [
    KeycloakService,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeKeycloak,
      multi: true,
      deps: [KeycloakService]
    }
  ]
};
```

#### Token Sharing with Remotes

```typescript
// Shared in singleton Keycloak instance
// Remotes automatically get the same instance due to Module Federation
```

### Routing Strategy

#### Shell Routes Configuration

```typescript
const routes: Routes = [
  // Shell-owned routes
  { path: '', component: HomeComponent },
  { path: 'about', component: AboutComponent },

  // Public portal routes (remote)
  {
    path: 'vehicles',
    loadChildren: () =>
      loadRemoteModule('publicPortal', './VehicleSearchModule')
        .then(m => m.routes)
  },
  {
    path: 'booking',
    loadChildren: () =>
      loadRemoteModule('publicPortal', './BookingModule')
        .then(m => m.routes),
    canActivate: [AuthGuard]
  },

  // Call center routes (remote)
  {
    path: 'admin/reservations',
    loadChildren: () =>
      loadRemoteModule('callCenterPortal', './ReservationsModule')
        .then(m => m.routes),
    canActivate: [AuthGuard, RoleGuard],
    data: { role: 'admin' }
  },
];
```

---

## Deployment Strategy

### Development Environment

#### Docker Compose Setup

```yaml
version: '3.8'

services:
  shell:
    build:
      context: ./src/frontend/apps/shell
    ports:
      - "4200:80"
    environment:
      - REMOTE_PUBLIC_PORTAL_URL=http://localhost:4201
      - REMOTE_CALLCENTER_URL=http://localhost:4202
    networks:
      - microfrontend-network

  public-portal:
    build:
      context: ./src/frontend/apps/public-portal
    ports:
      - "4201:80"
    networks:
      - microfrontend-network

  call-center-portal:
    build:
      context: ./src/frontend/apps/call-center-portal
    ports:
      - "4202:80"
    networks:
      - microfrontend-network

networks:
  microfrontend-network:
    driver: bridge
```

### Kubernetes Deployment

#### Shell Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: shell
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: shell
        image: orangecarrental.azurecr.io/shell:latest
        env:
        - name: REMOTE_PUBLIC_PORTAL_URL
          value: "https://public.orangecarrental.com/remoteEntry.json"
        - name: REMOTE_CALLCENTER_URL
          value: "https://admin.orangecarrental.com/remoteEntry.json"
---
apiVersion: v1
kind: Service
metadata:
  name: shell
spec:
  type: ClusterIP
  ports:
  - port: 80
    targetPort: 80
```

#### Remote Deployments

Similar deployments for each microfrontend:
- `public-portal-deployment.yaml`
- `call-center-portal-deployment.yaml`

#### Ingress Configuration

```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: microfrontend-ingress
spec:
  rules:
  - host: orangecarrental.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: shell
            port:
              number: 80

  - host: public.orangecarrental.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: public-portal
            port:
              number: 80

  - host: admin.orangecarrental.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: call-center-portal
            port:
              number: 80
```

### CI/CD Pipeline

#### Independent Builds

```yaml
# .github/workflows/build-microfrontends.yml
name: Build Microfrontends

on:
  push:
    branches: [develop, main]

jobs:
  build-shell:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Build Shell
        run: |
          cd src/frontend/apps/shell
          npm ci
          npm run build:production
      - name: Build Docker Image
        run: docker build -t shell:${{ github.sha }} .
      - name: Push to Registry
        run: docker push orangecarrental.azurecr.io/shell:${{ github.sha }}

  build-public-portal:
    runs-on: ubuntu-latest
    steps:
      # Similar to shell

  build-call-center:
    runs-on: ubuntu-latest
    steps:
      # Similar to shell
```

### Deployment Strategies

#### Blue-Green Deployment

1. Deploy new version to "green" environment
2. Test green environment
3. Switch traffic from blue to green
4. Keep blue as rollback option

#### Canary Deployment

1. Deploy new version to 10% of pods
2. Monitor metrics (errors, latency)
3. Gradually increase to 50%, then 100%
4. Rollback if issues detected

#### Feature Flags

Use feature flags to control microfrontend loading:

```typescript
// Load new or old version based on feature flag
const remotesConfig = featureFlags.newPublicPortal
  ? { publicPortal: 'https://public-v2.orangecarrental.com/remoteEntry.json' }
  : { publicPortal: 'https://public-v1.orangecarrental.com/remoteEntry.json' };
```

---

## Testing Strategy

### Unit Testing

Each microfrontend maintains its own unit tests:

```bash
# Test each microfrontend independently
cd src/frontend/apps/public-portal
npm run test

cd src/frontend/apps/call-center-portal
npm run test

cd src/frontend/apps/shell
npm run test
```

### Integration Testing

Test microfrontend loading and communication:

```typescript
// src/frontend/apps/shell/src/app/shell.integration.spec.ts
describe('Shell Integration', () => {
  it('should load public portal module', async () => {
    const module = await loadRemoteModule('publicPortal', './Routes');
    expect(module).toBeDefined();
    expect(module.routes).toBeDefined();
  });

  it('should share authentication across microfrontends', () => {
    // Test Keycloak singleton sharing
  });

  it('should communicate via event bus', () => {
    // Test cross-microfrontend events
  });
});
```

### End-to-End Testing

Use Playwright for full user journey tests:

```typescript
// e2e/microfrontend-navigation.spec.ts
test('should navigate between microfrontends', async ({ page }) => {
  // Start at shell
  await page.goto('http://localhost:4200');

  // Navigate to public portal route
  await page.click('a[href="/vehicles"]');
  await expect(page).toHaveURL(/.*vehicles/);

  // Verify microfrontend loaded
  await expect(page.locator('[data-mfe="public-portal"]')).toBeVisible();

  // Navigate to booking
  await page.click('a[href="/booking"]');
  await expect(page).toHaveURL(/.*booking/);
});
```

### Performance Testing

Monitor bundle sizes and load times:

```bash
# Analyze bundle sizes
npm run build -- --stats-json
npx webpack-bundle-analyzer dist/stats.json

# Lighthouse CI
npx lhci autorun
```

### Load Testing

Test module federation under load:

```yaml
# k6 load test script
import http from 'k6/http';

export default function() {
  // Load shell
  http.get('http://localhost:4200');

  // Load remotes
  http.get('http://localhost:4201/remoteEntry.json');
  http.get('http://localhost:4202/remoteEntry.json');
}
```

---

## Rollback Plan

### Rollback Triggers

Roll back if:
- ❌ Critical functionality broken
- ❌ >5% error rate increase
- ❌ >20% performance degradation
- ❌ Security vulnerabilities discovered
- ❌ Unable to load microfrontends

### Rollback Procedures

#### Phase 1-2 Rollback
If issues during shell creation:
1. Delete shell application
2. Revert package.json changes
3. Continue using standalone apps
4. **Impact**: Minimal, old apps still working

#### Phase 3-4 Rollback
If issues during portal migration:
1. Update shell to not load problematic remote
2. Redirect routes to standalone app
3. Deploy standalone app version
4. **Impact**: Moderate, graceful degradation

#### Production Rollback

**Quick Rollback** (< 5 minutes):
```bash
# Rollback to previous deployment
kubectl rollout undo deployment/shell
kubectl rollout undo deployment/public-portal
kubectl rollout undo deployment/call-center-portal

# Verify rollback
kubectl rollout status deployment/shell
```

**Full Rollback** (< 30 minutes):
1. Update Ingress to point to standalone apps
2. Scale up standalone app pods
3. Scale down microfrontend pods
4. Update DNS if necessary

**Post-Rollback**:
1. Investigate root cause
2. Fix issues in development
3. Re-test thoroughly
4. Plan re-deployment

---

## Timeline and Resources

### Estimated Timeline

| Phase | Duration | Parallel Work Possible |
|-------|----------|------------------------|
| Phase 1: Setup | 1 week | No |
| Phase 2: Shell App | 1 week | No |
| Phase 3: Public Portal | 2 weeks | Yes (with Phase 4) |
| Phase 4: Call Center | 1 week | Yes (with Phase 3) |
| Phase 5: Optimization | 1 week | Partial |
| **Total** | **6 weeks** | **4-5 weeks with parallelization** |

### Resource Requirements

#### Team Composition

| Role | Count | Responsibilities |
|------|-------|------------------|
| **Frontend Lead** | 1 | Architecture decisions, code reviews |
| **Frontend Developers** | 2-3 | Implementation, testing |
| **DevOps Engineer** | 1 | CI/CD, Kubernetes, deployment |
| **QA Engineer** | 1 | Testing strategy, E2E tests |
| **Tech Writer** | 0.5 | Documentation |

#### Infrastructure

- Development environment servers
- Kubernetes test cluster
- CI/CD pipeline resources
- Monitoring and logging tools

### Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Module Federation learning curve | High | Medium | Training, documentation, POC |
| Version conflicts | Medium | High | Strict version management, testing |
| Performance degradation | Low | High | Performance testing, monitoring |
| Breaking changes during migration | Medium | Medium | Feature flags, gradual rollout |
| Team resistance | Low | Medium | Training, clear benefits communication |

---

## Success Metrics

### Technical Metrics

| Metric | Current | Target | Measurement |
|--------|---------|--------|-------------|
| **Initial Bundle Size** | ~2.5 MB | <1.5 MB | Webpack Bundle Analyzer |
| **Time to Interactive** | ~3.5s | <2.0s | Lighthouse |
| **Deployment Frequency** | Weekly | Daily | CI/CD metrics |
| **Deployment Time** | 15 min | 5 min | CI/CD pipeline duration |
| **Hot Reload Time** | 8-10s | 3-5s | Developer experience |

### Business Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| **Independent Deployments** | >80% of deployments only affect single microfrontend | Deployment logs |
| **Team Velocity** | +20% due to reduced coordination | Sprint velocity tracking |
| **Defect Escape Rate** | -30% due to isolated changes | Bug tracking |
| **Developer Satisfaction** | >8/10 | Survey |

---

## Appendices

### A. Module Federation Resources

- [Native Federation Documentation](https://www.npmjs.com/package/@angular-architects/native-federation)
- [Module Federation Examples](https://github.com/module-federation/module-federation-examples)
- [Webpack Module Federation Plugin](https://webpack.js.org/concepts/module-federation/)

### B. Angular Microfrontend Patterns

- [Angular Architects: Module Federation](https://www.angulararchitects.io/en/aktuelles/the-microfrontend-revolution-module-federation-in-webpack-5/)
- [Micro Frontends with Angular](https://medium.com/@ManfredSteyer/micro-frontends-with-modern-angular-part-1-basic-concepts-42f1bbe8dd6f)

### C. Example Projects

- [Angular Module Federation Example](https://github.com/angular-architects/module-federation-plugin-example)
- [Microfrontend Reference Architecture](https://github.com/neuland/micro-frontends)

### D. Team Training Plan

**Week 1: Module Federation Basics**
- What is Module Federation?
- How it works under the hood
- Hands-on: Simple MFE example

**Week 2: Angular + Module Federation**
- Native Federation for Angular
- Routing across microfrontends
- Hands-on: Build a shell app

**Week 3: Advanced Patterns**
- State sharing
- Communication patterns
- Error handling
- Hands-on: Event bus implementation

### E. FAQ

**Q: Can we mix different Angular versions?**
A: Technically yes, but not recommended. Better to keep versions aligned.

**Q: What happens if a remote fails to load?**
A: Implement error boundaries and fallback UI. Shell should remain functional.

**Q: How do we share state?**
A: Use RxJS subjects in singleton services or implement an event bus pattern.

**Q: What about SEO?**
A: Use Angular Universal for SSR, or pre-render critical routes.

**Q: How do we handle different deployment schedules?**
A: Each microfrontend can deploy independently. Shell fetches latest remoteEntry.json.

---

## Conclusion

This migration strategy provides a clear path from the current multi-app monorepo to a Module Federation-based microfrontend architecture. The phased approach minimizes risk, allows for parallel development, and ensures we can roll back at any stage.

**Key Takeaways**:
- ✅ Gradual migration using Strangler Fig pattern
- ✅ Module Federation for runtime composition
- ✅ Independent deployments and team autonomy
- ✅ Shared code via singleton dependencies
- ✅ Clear rollback plan at each phase

**Next Steps**:
1. Get stakeholder approval
2. Assemble migration team
3. Set up development environment
4. Begin Phase 1: Setup & Infrastructure

---

**Document Version**: 1.0
**Last Updated**: 2025-11-23
**Author**: Engineering Team
**Status**: Awaiting Approval
