# CI/CD Pipeline

## Quick Start

### Local Development with Docker
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

## GitHub Actions Workflows

### 1. Backend CI
- **Triggers**: Push/PR to main/develop
- **Actions**: Build, test, coverage, lint, security scan

### 2. Frontend CI
- **Triggers**: Push/PR to main/develop
- **Actions**: Build, test, lint, Lighthouse performance

### 3. Docker Build
- **Triggers**: Push to main/develop, tags `v*`
- **Actions**: Build images for all 7 services, push to ghcr.io, security scan

### 4. Deploy Staging
- **Triggers**: Push to develop
- **Actions**: Deploy to staging Kubernetes cluster

### 5. Deploy Production
- **Triggers**: Tags `v*` or manual
- **Actions**: Deploy to production Kubernetes cluster, create GitHub release

## Docker Images

**Backend Services** (5):
- api-gateway
- fleet-service
- reservation-service
- customer-service
- location-service

**Frontend Services** (2):
- call-center-portal
- public-portal

All images available at: `ghcr.io/smartsolutionslab/orange-car-rental-solution/<service>:<tag>`

## Deployment

### Staging
```bash
# Automatic on push to develop
git push origin develop
```

### Production
```bash
# Create version tag
git tag v1.0.0
git push origin v1.0.0
```

## Required Secrets

In GitHub repository settings → Secrets:
- `KUBE_CONFIG_STAGING`: Staging cluster kubeconfig (base64)
- `KUBE_CONFIG_PROD`: Production cluster kubeconfig (base64)
- `SLACK_WEBHOOK`: Notification webhook (optional)
