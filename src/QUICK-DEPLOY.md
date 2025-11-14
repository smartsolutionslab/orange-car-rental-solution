# Quick Deployment Reference

**Orange Car Rental - CI/CD Pipeline**
**Status**: ✅ Production Ready | **Build Success**: 100% (7/7)

---

## 📋 Pre-Flight Checklist

Before deploying, ensure you have:

- [ ] Kubernetes cluster (v1.25+) access configured
- [ ] `kubectl` installed and configured
- [ ] Nginx Ingress Controller installed
- [ ] DNS records configured
- [ ] GitHub Container Registry credentials

---

## 🚀 Deploy to Staging (5 Minutes)

### Step 1: Create Secrets (1 min)

```bash
# GitHub Container Registry secret
kubectl create secret docker-registry ghcr-secret \
  --docker-server=ghcr.io \
  --docker-username=YOUR_GITHUB_USERNAME \
  --docker-password=YOUR_GITHUB_TOKEN \
  -n orange-car-rental-staging

# Database secrets
kubectl create secret generic database-secrets \
  --from-literal=postgres-user=postgres \
  --from-literal=postgres-password=YOUR_STRONG_PASSWORD \
  --from-literal=customer-db-connection="Server=postgres;Database=OrangeCarRental_Customers;User Id=postgres;Password=YOUR_STRONG_PASSWORD;" \
  --from-literal=fleet-db-connection="Server=postgres;Database=OrangeCarRental_Fleet;User Id=postgres;Password=YOUR_STRONG_PASSWORD;" \
  --from-literal=reservation-db-connection="Server=postgres;Database=OrangeCarRental_Reservations;User Id=postgres;Password=YOUR_STRONG_PASSWORD;" \
  --from-literal=pricing-db-connection="Server=postgres;Database=OrangeCarRental_Pricing;User Id=postgres;Password=YOUR_STRONG_PASSWORD;" \
  -n orange-car-rental-staging
```

### Step 2: Deploy Infrastructure (2 min)

```bash
cd k8s

# Create namespaces
kubectl apply -f base/namespace.yml

# Deploy ConfigMap and Database
kubectl apply -f base/configmap.yml -n orange-car-rental-staging
kubectl apply -f base/database.yml -n orange-car-rental-staging

# Wait for database
kubectl wait --for=condition=ready pod -l app=postgres \
  -n orange-car-rental-staging --timeout=300s
```

### Step 3: Deploy Services (2 min)

```bash
# Deploy all backend services
kubectl apply -f staging/api-gateway-deployment.yml
kubectl apply -f staging/customer-service-deployment.yml
kubectl apply -f staging/fleet-service-deployment.yml
kubectl apply -f staging/reservation-service-deployment.yml
kubectl apply -f staging/pricing-service-deployment.yml

# Deploy frontend services
kubectl apply -f staging/call-center-portal-deployment.yml
kubectl apply -f staging/public-portal-deployment.yml

# Create services and ingress
kubectl apply -f staging/services.yml
kubectl apply -f staging/ingress.yml
```

### Step 4: Verify Deployment (30 sec)

```bash
# Check all pods are running
kubectl get pods -n orange-car-rental-staging

# Check services
kubectl get svc -n orange-car-rental-staging

# Check ingress
kubectl get ingress -n orange-car-rental-staging

# View logs (if needed)
kubectl logs -n orange-car-rental-staging -l app=api-gateway --tail=50
```

---

## 🏗️ Build Docker Images Locally

```bash
cd src

# Backend services
cd backend
docker build -f ApiGateway/Dockerfile -t orange-car-rental/api-gateway:latest .
docker build -f Services/Customer/Dockerfile -t orange-car-rental/customer-service:latest .
docker build -f Services/Fleet/Dockerfile -t orange-car-rental/fleet-service:latest .
docker build -f Services/Reservation/Dockerfile -t orange-car-rental/reservation-service:latest .
docker build -f Services/Pricing/Dockerfile -t orange-car-rental/pricing-service:latest .

# Frontend services
cd ../frontend
docker build -f apps/call-center-portal/Dockerfile -t orange-car-rental/call-center-portal:latest apps/call-center-portal
docker build -f apps/public-portal/Dockerfile -t orange-car-rental/public-portal:latest apps/public-portal

# Or use docker-compose
cd ..
docker-compose build
```

---

## 🐳 Docker Compose Quick Start

```bash
cd src

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Check status
docker-compose ps

# Stop all services
docker-compose down
```

---

## 📊 Service Endpoints

### Staging URLs
- **Public Portal**: https://staging.orangecarrental.com
- **Call Center**: https://staging-callcenter.orangecarrental.com
- **API Gateway**: https://staging-api.orangecarrental.com/api
- **API Health**: https://staging-api.orangecarrental.com/health

### Local Development (docker-compose)
- **Public Portal**: http://localhost:4200
- **Call Center**: http://localhost:4201
- **API Gateway**: http://localhost:5000
- **Customer Service**: http://localhost:5010
- **Fleet Service**: http://localhost:5020
- **Reservation Service**: http://localhost:5030
- **Pricing Service**: http://localhost:5040

---

## 🔍 Quick Troubleshooting

### Pod Not Starting

```bash
# Describe pod to see events
kubectl describe pod POD_NAME -n orange-car-rental-staging

# Common issues:
# - ImagePullBackOff: Check ghcr-secret exists and is valid
# - CrashLoopBackOff: Check application logs
# - Pending: Check node resources
```

### Database Connection Issues

```bash
# Check database pod
kubectl get pods -n orange-car-rental-staging -l app=postgres

# Test database connectivity
kubectl exec -it postgres-0 -n orange-car-rental-staging -- \
  psql -U postgres -l

# Check connection secrets
kubectl get secret database-secrets -n orange-car-rental-staging -o yaml
```

### Service Not Accessible

```bash
# Check service endpoints
kubectl get endpoints -n orange-car-rental-staging

# Test internal connectivity
kubectl run test-pod --rm -i --tty --image=alpine -n orange-car-rental-staging -- sh
wget -O- http://customer-service:8080/health
```

### View Application Logs

```bash
# Stream logs from a service
kubectl logs -f -n orange-car-rental-staging deployment/customer-service

# Logs from all pods of a service
kubectl logs -n orange-car-rental-staging -l app=customer-service --tail=100

# Previous container logs (if crashed)
kubectl logs -n orange-car-rental-staging POD_NAME --previous
```

---

## 🔄 Update & Rollback

### Deploy New Version

```bash
# Set new image version
kubectl set image deployment/customer-service \
  customer-service=ghcr.io/smartsolutionslab/orange-car-rental/customer-service:v1.1.0 \
  -n orange-car-rental-staging

# Watch rollout
kubectl rollout status deployment/customer-service -n orange-car-rental-staging
```

### Rollback to Previous

```bash
# Rollback to previous version
kubectl rollout undo deployment/customer-service -n orange-car-rental-staging

# Rollback to specific revision
kubectl rollout undo deployment/customer-service --to-revision=2 \
  -n orange-car-rental-staging

# View history
kubectl rollout history deployment/customer-service -n orange-car-rental-staging
```

---

## 📈 Scale Services

```bash
# Scale up
kubectl scale deployment customer-service --replicas=5 \
  -n orange-car-rental-staging

# Scale down
kubectl scale deployment customer-service --replicas=2 \
  -n orange-car-rental-staging

# Autoscaling (HPA)
kubectl autoscale deployment customer-service \
  --min=2 --max=10 --cpu-percent=80 \
  -n orange-car-rental-staging
```

---

## 🧹 Cleanup

### Remove Staging Environment

```bash
# Delete everything in namespace
kubectl delete namespace orange-car-rental-staging

# Or delete individual resources
kubectl delete deployment --all -n orange-car-rental-staging
kubectl delete svc --all -n orange-car-rental-staging
kubectl delete ingress --all -n orange-car-rental-staging
```

### Clean Local Docker

```bash
# Remove all orange-car-rental images
docker images | grep orange-car-rental | awk '{print $3}' | xargs docker rmi

# Docker compose cleanup
docker-compose down -v --remove-orphans
```

---

## 📚 Documentation Links

- **Full Deployment Guide**: [k8s/README.md](k8s/README.md)
- **CI/CD Quick Start**: [CICD-QUICK-START.md](CICD-QUICK-START.md)
- **Docker Optimization**: [DOCKER-IMAGE-OPTIMIZATION.md](DOCKER-IMAGE-OPTIMIZATION.md)
- **Test Report**: [CICD-TEST-REPORT.md](CICD-TEST-REPORT.md)
- **Deployment Readiness**: [DEPLOYMENT-READINESS.md](DEPLOYMENT-READINESS.md)

---

## 🆘 Emergency Contacts

```bash
# Check cluster status
kubectl cluster-info
kubectl get nodes

# Check all resources
kubectl get all -n orange-car-rental-staging

# Export logs for support
kubectl logs -n orange-car-rental-staging --all-containers=true \
  --prefix=true --timestamps > logs-$(date +%Y%m%d-%H%M%S).txt
```

---

## ✅ Health Check Commands

```bash
# Quick health check all services
for svc in api-gateway customer-service fleet-service reservation-service pricing-service; do
  echo "=== $svc ==="
  kubectl exec -n orange-car-rental-staging \
    $(kubectl get pod -n orange-car-rental-staging -l app=$svc -o jsonpath='{.items[0].metadata.name}') \
    -- wget -qO- http://localhost:8080/health 2>/dev/null || echo "FAILED"
done
```

---

**Last Updated**: November 14, 2025
**Pipeline Version**: v1.0.0
**Status**: ✅ Production Ready
