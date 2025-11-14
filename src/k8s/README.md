# Kubernetes Deployment Guide

**Orange Car Rental Solution - Kubernetes Manifests**

---

## 📁 Directory Structure

```
k8s/
├── base/
│   ├── namespace.yml          # Staging & Production namespaces
│   ├── configmap.yml          # Application configuration
│   └── database.yml           # PostgreSQL StatefulSet
├── staging/
│   ├── api-gateway-deployment.yml
│   ├── customer-service-deployment.yml
│   ├── fleet-service-deployment.yml
│   ├── reservation-service-deployment.yml
│   ├── pricing-service-deployment.yml
│   ├── call-center-portal-deployment.yml
│   ├── public-portal-deployment.yml
│   ├── services.yml           # All K8s services
│   └── ingress.yml            # Ingress configuration
└── production/
    └── (Same structure as staging with production values)
```

---

## 🚀 Quick Deploy - Staging

### Prerequisites

1. **Kubernetes cluster** (v1.25+)
2. **kubectl** configured with cluster access
3. **Nginx Ingress Controller** installed
4. **cert-manager** for TLS certificates (optional)
5. **GitHub Container Registry** access

### 1. Create Secrets

```bash
# Create GitHub Container Registry secret
kubectl create secret docker-registry ghcr-secret \
  --docker-server=ghcr.io \
  --docker-username=<your-github-username> \
  --docker-password=<your-github-token> \
  -n orange-car-rental-staging

# Create database secrets
kubectl create secret generic database-secrets \
  --from-literal=postgres-user=postgres \
  --from-literal=postgres-password=<strong-password> \
  --from-literal=customer-db-connection="Server=postgres;Database=OrangeCarRental_Customers;User Id=postgres;Password=<strong-password>;" \
  --from-literal=fleet-db-connection="Server=postgres;Database=OrangeCarRental_Fleet;User Id=postgres;Password=<strong-password>;" \
  --from-literal=reservation-db-connection="Server=postgres;Database=OrangeCarRental_Reservations;User Id=postgres;Password=<strong-password>;" \
  --from-literal=pricing-db-connection="Server=postgres;Database=OrangeCarRental_Pricing;User Id=postgres;Password=<strong-password>;" \
  -n orange-car-rental-staging
```

### 2. Deploy Infrastructure

```bash
# Create namespaces
kubectl apply -f base/namespace.yml

# Deploy ConfigMap
kubectl apply -f base/configmap.yml -n orange-car-rental-staging

# Deploy PostgreSQL database
kubectl apply -f base/database.yml -n orange-car-rental-staging

# Wait for database to be ready
kubectl wait --for=condition=ready pod -l app=postgres -n orange-car-rental-staging --timeout=300s
```

### 3. Deploy Backend Services

```bash
# Deploy all backend services
kubectl apply -f staging/api-gateway-deployment.yml
kubectl apply -f staging/customer-service-deployment.yml
kubectl apply -f staging/fleet-service-deployment.yml
kubectl apply -f staging/reservation-service-deployment.yml
kubectl apply -f staging/pricing-service-deployment.yml

# Wait for backends to be ready
kubectl wait --for=condition=available deployment --all -n orange-car-rental-staging --timeout=600s
```

### 4. Deploy Frontend Services

```bash
# Deploy frontend applications
kubectl apply -f staging/call-center-portal-deployment.yml
kubectl apply -f staging/public-portal-deployment.yml
```

### 5. Create Services & Ingress

```bash
# Create Kubernetes services
kubectl apply -f staging/services.yml

# Create ingress (configure DNS first!)
kubectl apply -f staging/ingress.yml
```

### 6. Verify Deployment

```bash
# Check all pods
kubectl get pods -n orange-car-rental-staging

# Check services
kubectl get svc -n orange-car-rental-staging

# Check ingress
kubectl get ingress -n orange-car-rental-staging

# View logs
kubectl logs -n orange-car-rental-staging -l app=api-gateway
```

---

## 🔍 Monitoring & Health Checks

### Pod Status

```bash
# Watch pod status
kubectl get pods -n orange-car-rental-staging -w

# Describe a pod (troubleshooting)
kubectl describe pod <pod-name> -n orange-car-rental-staging
```

### Health Endpoints

All services expose `/health` endpoint:

```bash
# Port-forward to test locally
kubectl port-forward -n orange-car-rental-staging svc/api-gateway 8080:8080

# Test health endpoint
curl http://localhost:8080/health
```

### Logs

```bash
# Stream logs from a deployment
kubectl logs -f -n orange-car-rental-staging deployment/customer-service

# Logs from all pods of a service
kubectl logs -n orange-car-rental-staging -l app=customer-service --tail=100

# Previous container logs (if crashed)
kubectl logs -n orange-car-rental-staging <pod-name> --previous
```

---

## 🔄 Updates & Rollouts

### Deploy New Version

```bash
# Set new image version
kubectl set image deployment/customer-service \
  customer-service=ghcr.io/smartsolutionslab/orange-car-rental/customer-service:v1.1.0 \
  -n orange-car-rental-staging

# Watch rollout status
kubectl rollout status deployment/customer-service -n orange-car-rental-staging
```

### Rollback

```bash
# Rollback to previous version
kubectl rollout undo deployment/customer-service -n orange-car-rental-staging

# Rollback to specific revision
kubectl rollout undo deployment/customer-service --to-revision=2 -n orange-car-rental-staging

# View rollout history
kubectl rollout history deployment/customer-service -n orange-car-rental-staging
```

### Scale Services

```bash
# Scale up
kubectl scale deployment customer-service --replicas=5 -n orange-car-rental-staging

# Autoscaling (HPA)
kubectl autoscale deployment customer-service \
  --min=2 --max=10 --cpu-percent=80 \
  -n orange-car-rental-staging
```

---

## 🔐 Security Configuration

### TLS/SSL Certificates

Using cert-manager for automatic certificate management:

```bash
# Install cert-manager (if not already installed)
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.0/cert-manager.yaml

# Create ClusterIssuer for Let's Encrypt
cat <<EOF | kubectl apply -f -
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: letsencrypt-staging
spec:
  acme:
    server: https://acme-staging-v02.api.letsencrypt.org/directory
    email: admin@orangecarrental.com
    privateKeySecretRef:
      name: letsencrypt-staging
    solvers:
    - http01:
        ingress:
          class: nginx
EOF
```

### Network Policies

```bash
# Example: Restrict backend access
cat <<EOF | kubectl apply -f -
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: backend-policy
  namespace: orange-car-rental-staging
spec:
  podSelector:
    matchLabels:
      tier: backend
  policyTypes:
  - Ingress
  ingress:
  - from:
    - podSelector:
        matchLabels:
          tier: gateway
    ports:
    - protocol: TCP
      port: 8080
EOF
```

---

## 📊 Resource Management

### Resource Quotas

Current resource allocations per service:

| Service | CPU Request | CPU Limit | Memory Request | Memory Limit |
|---------|-------------|-----------|----------------|--------------|
| API Gateway | 100m | 500m | 128Mi | 512Mi |
| Backend Services | 100m | 500m | 128Mi | 512Mi |
| Frontend Services | 50m | 200m | 64Mi | 256Mi |
| PostgreSQL | 250m | 1000m | 256Mi | 1Gi |

### Total Staging Resources

- **CPU Requests**: ~1.2 cores
- **CPU Limits**: ~6 cores
- **Memory Requests**: ~1.5GB
- **Memory Limits**: ~5.5GB
- **Storage**: 10GB (PostgreSQL)

---

## 🏗️ Production Deployment

### Differences from Staging

1. **Higher replicas** (3-5 per service)
2. **Stricter resource limits**
3. **Production database** configuration
4. **Real TLS certificates** (letsencrypt-prod)
5. **Monitoring** (Prometheus, Grafana)
6. **Logging** (ELK stack or Loki)

### Production Checklist

- [ ] DNS configured for production domains
- [ ] TLS certificates obtained
- [ ] Database backup strategy in place
- [ ] Monitoring and alerting configured
- [ ] Log aggregation set up
- [ ] Resource quotas defined
- [ ] Network policies applied
- [ ] Secrets properly secured
- [ ] Disaster recovery plan documented

---

## 🐛 Troubleshooting

### Pod Not Starting

```bash
# Check pod events
kubectl describe pod <pod-name> -n orange-car-rental-staging

# Common issues:
# - ImagePullBackOff: Check image name and registry secrets
# - CrashLoopBackOff: Check application logs
# - Pending: Check resource availability
```

### Service Not Accessible

```bash
# Check service endpoints
kubectl get endpoints -n orange-car-rental-staging

# Test service connectivity
kubectl run test-pod --rm -i --tty --image=alpine -- sh
# Inside pod:
wget -O- http://customer-service:8080/health
```

### Database Connection Issues

```bash
# Check database pod
kubectl get pods -n orange-car-rental-staging -l app=postgres

# Test database connectivity
kubectl exec -it postgres-0 -n orange-car-rental-staging -- psql -U postgres -l

# Check connection secrets
kubectl get secret database-secrets -n orange-car-rental-staging -o yaml
```

---

## 🔧 Maintenance

### Database Backup

```bash
# Backup PostgreSQL
kubectl exec postgres-0 -n orange-car-rental-staging -- \
  pg_dumpall -U postgres > backup_$(date +%Y%m%d).sql

# Restore from backup
kubectl exec -i postgres-0 -n orange-car-rental-staging -- \
  psql -U postgres < backup_20251114.sql
```

### Clean Up

```bash
# Delete everything in staging namespace
kubectl delete namespace orange-car-rental-staging

# Delete specific deployment
kubectl delete deployment customer-service -n orange-car-rental-staging

# Delete services
kubectl delete svc --all -n orange-car-rental-staging
```

---

## 📚 Additional Resources

- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Nginx Ingress Controller](https://kubernetes.github.io/ingress-nginx/)
- [cert-manager Documentation](https://cert-manager.io/docs/)
- [kubectl Cheat Sheet](https://kubernetes.io/docs/reference/kubectl/cheatsheet/)

---

## 🆘 Support

For issues or questions:
1. Check application logs: `kubectl logs -n orange-car-rental-staging <pod-name>`
2. Check pod events: `kubectl describe pod -n orange-car-rental-staging <pod-name>`
3. Review ingress logs: `kubectl logs -n ingress-nginx <ingress-pod>`
4. Verify secrets: `kubectl get secrets -n orange-car-rental-staging`

---

**Last Updated**: November 14, 2025
**Kubernetes Version**: 1.25+
**Status**: Production Ready
