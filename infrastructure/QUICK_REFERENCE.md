# Deployment Quick Reference

## Initial Setup (One-time)

```bash
# 1. Deploy Azure infrastructure
cd infrastructure/azure
az deployment sub create \
  --name orange-prod-deployment \
  --location westeurope \
  --template-file main.bicep \
  --parameters @parameters.production.json

# 2. Get AKS credentials
az aks get-credentials \
  --resource-group orange-production-rg \
  --name orange-production-aks

# 3. Setup secrets
cd ../scripts
./setup-secrets.sh production orange-production-kv

# 4. Setup ingress and SSL
./setup-ingress.sh production

# 5. Configure DNS (use IP from step 4)
# Point your domains to the external IP
```

## Regular Deployment

```bash
# 1. Build and push images
az acr login --name orangeproductionacr

docker build -t orangeproductionacr.azurecr.io/vehicles-api:v1.0.0 .
docker push orangeproductionacr.azurecr.io/vehicles-api:v1.0.0

# 2. Run migrations (if needed)
cd infrastructure/scripts
./run-migrations.sh production

# 3. Deploy application
./deploy.sh production v1.0.0
```

## Common Operations

### View Status

```bash
# Pods
kubectl get pods -n orange-production

# Services
kubectl get svc -n orange-production

# Ingress
kubectl get ingress -n orange-production

# Health check
kubectl exec -it deployment/vehicles-api -n orange-production -- curl http://localhost:8080/health
```

### View Logs

```bash
# Live logs
kubectl logs -f deployment/vehicles-api -n orange-production

# Last 100 lines
kubectl logs deployment/vehicles-api -n orange-production --tail=100

# Previous pod logs
kubectl logs <pod-name> -n orange-production --previous
```

### Scale Services

```bash
# Scale specific service
kubectl scale deployment vehicles-api --replicas=10 -n orange-production

# Check auto-scaling status
kubectl get hpa -n orange-production
```

### Database Operations

```bash
# Backup
./backup-database.sh production

# Restore
./restore-database.sh production backups/backup-file.sql.gz

# Migrations
./run-migrations.sh production
```

### Troubleshooting

```bash
# Describe pod (shows events and issues)
kubectl describe pod <pod-name> -n orange-production

# Get pod YAML
kubectl get pod <pod-name> -n orange-production -o yaml

# Execute shell in pod
kubectl exec -it <pod-name> -n orange-production -- /bin/bash

# Port forward to service
kubectl port-forward svc/vehicles-api 8080:8080 -n orange-production

# View events
kubectl get events -n orange-production --sort-by='.lastTimestamp'
```

### Rollback

```bash
# View rollout history
kubectl rollout history deployment/vehicles-api -n orange-production

# Rollback to previous version
kubectl rollout undo deployment/vehicles-api -n orange-production

# Rollback to specific revision
kubectl rollout undo deployment/vehicles-api --to-revision=2 -n orange-production
```

### Restart Services

```bash
# Restart specific deployment
kubectl rollout restart deployment/vehicles-api -n orange-production

# Restart all deployments
kubectl rollout restart deployment --all -n orange-production
```

## Emergency Procedures

### Service Down

```bash
# 1. Check pod status
kubectl get pods -n orange-production

# 2. Check logs
kubectl logs -l app=vehicles-api -n orange-production --tail=100

# 3. Restart if needed
kubectl rollout restart deployment/vehicles-api -n orange-production

# 4. If persists, rollback
kubectl rollout undo deployment/vehicles-api -n orange-production
```

### Database Issues

```bash
# 1. Check database pod
kubectl get pods -l app=postgres -n orange-production

# 2. Check database connectivity
kubectl exec -it deployment/vehicles-api -n orange-production -- /bin/bash
psql -h postgres -U pgadmin -d orangecarrental

# 3. Restart database (last resort)
kubectl rollout restart statefulset/postgres -n orange-production
```

### SSL Certificate Issues

```bash
# 1. Check certificate status
kubectl get certificate -n orange-production
kubectl describe certificate orange-rental-tls -n orange-production

# 2. Check cert-manager logs
kubectl logs -n cert-manager -l app=cert-manager --tail=100

# 3. Force renewal
kubectl delete certificate orange-rental-tls -n orange-production
```

## Monitoring URLs

### Production
- Public Portal: https://orange-rental.de
- API Gateway: https://api.orange-rental.de
- Keycloak: https://auth.orange-rental.de
- Azure Portal: https://portal.azure.com

### Staging
- Public Portal: https://staging.orange-rental.de
- API Gateway: https://api-staging.orange-rental.de
- Keycloak: https://auth-staging.orange-rental.de

## Resource Names

### Production
- Resource Group: `orange-production-rg`
- AKS Cluster: `orange-production-aks`
- ACR: `orangeproductionacr`
- PostgreSQL: `orange-production-postgres`
- Key Vault: `orange-production-kv`
- Namespace: `orange-production`

### Staging
- Resource Group: `orange-staging-rg`
- AKS Cluster: `orange-staging-aks`
- ACR: `orangestagingacr`
- PostgreSQL: `orange-staging-postgres`
- Key Vault: `orange-staging-kv`
- Namespace: `orange-staging`

### Development
- Resource Group: `orange-dev-rg`
- AKS Cluster: `orange-dev-aks`
- ACR: `orangedevacr`
- PostgreSQL: `orange-dev-postgres`
- Key Vault: `orange-dev-kv`
- Namespace: `orange-dev`

## Contacts

- **DevOps:** devops@orange-rental.de
- **On-Call:** oncall@orange-rental.de
- **Slack:** #orange-car-rental-alerts

## Useful Links

- [Full Deployment Guide](../DEPLOYMENT.md)
- [Monitoring Guide](../MONITORING.md)
- [CI/CD Documentation](../CI_CD.md)
- [Azure Portal](https://portal.azure.com)
- [Application Insights](https://portal.azure.com/#blade/HubsExtension/BrowseResource/resourceType/microsoft.insights%2Fcomponents)
