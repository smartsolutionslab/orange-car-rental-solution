# Deployment Guide - Orange Car Rental

## Overview

This guide covers the complete deployment process for Orange Car Rental, from infrastructure provisioning to application deployment and operations.

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         Azure Cloud                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  Azure Kubernetes Service (AKS)                          │  │
│  │                                                            │  │
│  │  ┌────────────┐  ┌────────────┐  ┌────────────┐        │  │
│  │  │   Public   │  │  Vehicles  │  │Reservations│        │  │
│  │  │   Portal   │  │    API     │  │    API     │        │  │
│  │  └────────────┘  └────────────┘  └────────────┘        │  │
│  │                                                            │  │
│  │  ┌────────────┐  ┌────────────┐  ┌────────────┐        │  │
│  │  │ Customers  │  │ Locations  │  │  Keycloak  │        │  │
│  │  │    API     │  │    API     │  │    Auth    │        │  │
│  │  └────────────┘  └────────────┘  └────────────┘        │  │
│  │                                                            │  │
│  │  ┌─────────────────────────────────────────────────────┐ │  │
│  │  │          NGINX Ingress Controller                    │ │  │
│  │  └─────────────────────────────────────────────────────┘ │  │
│  └──────────────────────────────────────────────────────────┘  │
│                              │                                   │
│  ┌───────────────────┐  ┌───────────────────┐  ┌─────────────┐│
│  │   PostgreSQL      │  │  Azure Key Vault  │  │ Application ││
│  │   Flexible Server │  │   (Secrets)       │  │  Insights   ││
│  └───────────────────┘  └───────────────────┘  └─────────────┘│
│                                                                   │
│  ┌───────────────────┐  ┌───────────────────┐                  │
│  │  Container        │  │  Storage Account  │                  │
│  │  Registry (ACR)   │  │   (Backups)       │                  │
│  └───────────────────┘  └───────────────────┘                  │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

## Prerequisites

### Required Tools

```bash
# Azure CLI
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
az --version

# kubectl
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
sudo install -o root -g root -m 0755 kubectl /usr/local/bin/kubectl
kubectl version --client

# Helm
curl https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3 | bash
helm version

# Kustomize
curl -s "https://raw.githubusercontent.com/kubernetes-sigs/kustomize/master/hack/install_kustomize.sh" | bash
sudo mv kustomize /usr/local/bin/
kustomize version
```

### Azure Authentication

```bash
# Login to Azure
az login

# Set subscription
az account set --subscription "Your-Subscription-ID"

# Verify
az account show
```

## Infrastructure Provisioning

### Step 1: Deploy Azure Resources

Deploy the complete Azure infrastructure using Bicep templates:

#### Development Environment

```bash
cd infrastructure/azure

az deployment sub create \
  --name orange-dev-deployment \
  --location westeurope \
  --template-file main.bicep \
  --parameters @parameters.dev.json
```

#### Staging Environment

```bash
az deployment sub create \
  --name orange-staging-deployment \
  --location westeurope \
  --template-file main.bicep \
  --parameters @parameters.staging.json
```

#### Production Environment

```bash
az deployment sub create \
  --name orange-production-deployment \
  --location westeurope \
  --template-file main.bicep \
  --parameters @parameters.production.json
```

**Deployment time:** ~15-20 minutes

**Resources created:**
- Resource Group
- Azure Kubernetes Service (AKS)
- Azure Container Registry (ACR)
- Azure Database for PostgreSQL
- Azure Key Vault
- Application Insights
- Log Analytics Workspace
- Storage Account
- Action Group (for alerts)

### Step 2: Configure Access

#### Get AKS Credentials

```bash
ENVIRONMENT=production
az aks get-credentials \
  --resource-group orange-${ENVIRONMENT}-rg \
  --name orange-${ENVIRONMENT}-aks \
  --overwrite-existing

# Verify connection
kubectl get nodes
```

#### Configure ACR Access

```bash
# Get ACR login server
ACR_NAME=$(az acr list --resource-group orange-${ENVIRONMENT}-rg --query "[0].name" -o tsv)
ACR_LOGIN_SERVER=$(az acr show --name $ACR_NAME --query loginServer -o tsv)

# Attach ACR to AKS
az aks update \
  --resource-group orange-${ENVIRONMENT}-rg \
  --name orange-${ENVIRONMENT}-aks \
  --attach-acr $ACR_NAME

echo "ACR Login Server: $ACR_LOGIN_SERVER"
```

### Step 3: Setup Secrets

Configure secrets in Azure Key Vault and Kubernetes:

```bash
cd infrastructure/scripts

# Make scripts executable
chmod +x setup-secrets.sh

# Setup secrets in Key Vault
./setup-secrets.sh production orange-production-kv
```

**IMPORTANT:** Save the generated credentials securely!

### Step 4: Setup Ingress and SSL

Install NGINX Ingress Controller and cert-manager:

```bash
chmod +x setup-ingress.sh
./setup-ingress.sh production
```

This will:
1. Install NGINX Ingress Controller
2. Install cert-manager
3. Create Let's Encrypt ClusterIssuers
4. Display the external IP address

**Configure DNS Records:**

Point your domains to the external IP:

```
A    orange-rental.de              → <EXTERNAL_IP>
A    www.orange-rental.de          → <EXTERNAL_IP>
A    api.orange-rental.de          → <EXTERNAL_IP>
A    auth.orange-rental.de         → <EXTERNAL_IP>
```

## Application Deployment

### Step 5: Build and Push Docker Images

Build Docker images and push to Azure Container Registry:

```bash
# Login to ACR
az acr login --name $ACR_NAME

# Build and push images
cd src/frontend/apps/public-portal
docker build -t ${ACR_LOGIN_SERVER}/public-portal:v1.0.0 .
docker push ${ACR_LOGIN_SERVER}/public-portal:v1.0.0

cd ../../../Services/Vehicles.API
docker build -t ${ACR_LOGIN_SERVER}/vehicles-api:v1.0.0 .
docker push ${ACR_LOGIN_SERVER}/vehicles-api:v1.0.0

cd ../Reservations.API
docker build -t ${ACR_LOGIN_SERVER}/reservations-api:v1.0.0 .
docker push ${ACR_LOGIN_SERVER}/reservations-api:v1.0.0

cd ../Customers.API
docker build -t ${ACR_LOGIN_SERVER}/customers-api:v1.0.0 .
docker push ${ACR_LOGIN_SERVER}/customers-api:v1.0.0

cd ../Locations.API
docker build -t ${ACR_LOGIN_SERVER}/locations-api:v1.0.0 .
docker push ${ACR_LOGIN_SERVER}/locations-api:v1.0.0
```

### Step 6: Run Database Migrations

```bash
cd infrastructure/scripts
chmod +x run-migrations.sh
./run-migrations.sh production
```

### Step 7: Deploy Application

```bash
chmod +x deploy.sh
./deploy.sh production v1.0.0
```

This script will:
1. Apply Kubernetes manifests using Kustomize
2. Wait for deployments to be ready
3. Check rollout status
4. Display pod and service information

**Deployment time:** ~5-10 minutes

### Step 8: Verify Deployment

```bash
# Check pod status
kubectl get pods -n orange-production

# Check service health
kubectl exec -it deployment/vehicles-api -n orange-production -- curl http://localhost:8080/health

# View logs
kubectl logs -f deployment/vehicles-api -n orange-production

# Check ingress
kubectl get ingress -n orange-production
```

## Environment Configuration

### Development

- **Purpose:** Local development and testing
- **Replicas:** 1 per service
- **Resources:** Minimal (128-256Mi RAM, 250m CPU)
- **Database:** Burstable tier (Standard_B2s)
- **Monitoring:** Basic Application Insights

### Staging

- **Purpose:** Pre-production testing and QA
- **Replicas:** 2 per service
- **Resources:** Medium (256-512Mi RAM, 500m CPU)
- **Database:** Burstable tier (Standard_B2s)
- **Monitoring:** Full Application Insights

### Production

- **Purpose:** Live production environment
- **Replicas:** 3-5 per service (with auto-scaling)
- **Resources:** High (512Mi-1Gi RAM, 500m-1000m CPU)
- **Database:** General Purpose tier (Standard_D4s_v3)
- **Monitoring:** Full monitoring stack with alerting

## Scaling Configuration

### Manual Scaling

```bash
# Scale specific deployment
kubectl scale deployment vehicles-api --replicas=10 -n orange-production

# Scale all deployments
kubectl scale deployment --all --replicas=3 -n orange-production
```

### Auto-Scaling (Production Only)

Horizontal Pod Autoscaler is configured for production:

```bash
# Check HPA status
kubectl get hpa -n orange-production

# Describe HPA
kubectl describe hpa vehicles-api -n orange-production
```

**Auto-scaling configuration:**
- Min replicas: 5
- Max replicas: 20
- Target CPU: 70%
- Target Memory: 80%

## Database Operations

### Backups

#### Automated Backups

Automated daily backups run at 2 AM (configured as CronJob):

```bash
# Check backup job status
kubectl get cronjob db-backup -n orange-production

# View backup job logs
kubectl logs -l app=db-backup -n orange-production --tail=50
```

#### Manual Backups

```bash
cd infrastructure/scripts
./backup-database.sh production
```

Backup files are stored in:
- Local: `./backups/`
- Azure Storage: `backups` container

### Restore

```bash
cd infrastructure/scripts
./restore-database.sh production backups/backup-20240101-120000.sql.gz
```

⚠️ **WARNING:** This will stop all applications and restore the database!

### Migrations

Run database migrations after deploying new schema changes:

```bash
cd infrastructure/scripts
./run-migrations.sh production
```

## Monitoring and Observability

### Application Insights

Access Azure Application Insights:

```bash
RESOURCE_GROUP=orange-production-rg
APP_INSIGHTS_NAME=orange-production-ai

az monitor app-insights component show \
  --app $APP_INSIGHTS_NAME \
  --resource-group $RESOURCE_GROUP
```

**Portal URL:** https://portal.azure.com → Application Insights

### Health Checks

```bash
# Liveness probe
kubectl exec -it deployment/vehicles-api -n orange-production -- curl http://localhost:8080/health/live

# Readiness probe
kubectl exec -it deployment/vehicles-api -n orange-production -- curl http://localhost:8080/health/ready

# Detailed health
kubectl exec -it deployment/vehicles-api -n orange-production -- curl http://localhost:8080/health
```

### Logs

```bash
# View logs for specific service
kubectl logs -f deployment/vehicles-api -n orange-production

# View logs for all pods with label
kubectl logs -l app=vehicles-api -n orange-production --tail=100

# View logs for specific pod
kubectl logs pod-name -n orange-production --previous
```

### Metrics

```bash
# Check resource usage
kubectl top pods -n orange-production
kubectl top nodes

# Get detailed pod metrics
kubectl describe pod <pod-name> -n orange-production
```

## Troubleshooting

### Pod Not Starting

```bash
# Describe pod
kubectl describe pod <pod-name> -n orange-production

# Check events
kubectl get events -n orange-production --sort-by='.lastTimestamp'

# Check logs
kubectl logs <pod-name> -n orange-production
```

**Common issues:**
- Image pull errors → Check ACR credentials
- Secret not found → Verify Key Vault secrets
- CrashLoopBackOff → Check application logs

### Database Connection Issues

```bash
# Test database connectivity from pod
kubectl exec -it deployment/vehicles-api -n orange-production -- /bin/bash
psql -h postgres -U pgadmin -d orangecarrental

# Check database secrets
kubectl get secret database-secrets -n orange-production -o yaml

# Port forward to database
kubectl port-forward svc/postgres 5432:5432 -n orange-production
psql -h localhost -U pgadmin -d orangecarrental
```

### Ingress Issues

```bash
# Check ingress status
kubectl describe ingress -n orange-production

# Check ingress controller logs
kubectl logs -n ingress-nginx -l app.kubernetes.io/component=controller --tail=100

# Check certificate status
kubectl get certificate -n orange-production
kubectl describe certificate orange-rental-tls -n orange-production
```

### SSL Certificate Issues

```bash
# Check cert-manager logs
kubectl logs -n cert-manager -l app=cert-manager --tail=100

# Check certificate request
kubectl get certificaterequest -n orange-production
kubectl describe certificaterequest <name> -n orange-production

# Force certificate renewal
kubectl delete certificate orange-rental-tls -n orange-production
```

## Rollback

### Rollback Deployment

```bash
# Check rollout history
kubectl rollout history deployment/vehicles-api -n orange-production

# Rollback to previous version
kubectl rollout undo deployment/vehicles-api -n orange-production

# Rollback to specific revision
kubectl rollout undo deployment/vehicles-api --to-revision=2 -n orange-production

# Check rollback status
kubectl rollout status deployment/vehicles-api -n orange-production
```

### Rollback Database

Use the restore script with a previous backup:

```bash
./restore-database.sh production backups/backup-before-migration.sql.gz
```

## Maintenance

### Update Application

```bash
# Build new image
docker build -t ${ACR_LOGIN_SERVER}/vehicles-api:v1.1.0 .
docker push ${ACR_LOGIN_SERVER}/vehicles-api:v1.1.0

# Deploy new version
./deploy.sh production v1.1.0
```

### Update Kubernetes Configuration

```bash
# Edit kustomization
vim k8s/overlays/production/kustomization.yaml

# Apply changes
kubectl apply -k k8s/overlays/production -n orange-production
```

### Restart Services

```bash
# Restart specific deployment
kubectl rollout restart deployment/vehicles-api -n orange-production

# Restart all deployments
kubectl rollout restart deployment --all -n orange-production
```

## Security Best Practices

### Secrets Management

✅ **DO:**
- Store secrets in Azure Key Vault
- Use SecretProviderClass to inject secrets
- Rotate secrets regularly
- Use managed identities

❌ **DON'T:**
- Commit secrets to Git
- Store secrets in ConfigMaps
- Use hardcoded passwords
- Share credentials

### Network Security

- All external traffic uses HTTPS (TLS 1.2+)
- Internal service communication uses ClusterIP
- Network policies restrict pod-to-pod communication
- Ingress rate limiting: 100 req/s per IP

### Access Control

```bash
# Create service account for deployments
kubectl create serviceaccount deploy-sa -n orange-production

# Create role binding
kubectl create rolebinding deploy-sa-binding \
  --clusterrole=edit \
  --serviceaccount=orange-production:deploy-sa \
  -n orange-production
```

## Cost Optimization

### Resource Limits

All deployments have resource requests and limits to prevent over-provisioning:

```yaml
resources:
  requests:
    memory: "512Mi"
    cpu: "500m"
  limits:
    memory: "1Gi"
    cpu: "1000m"
```

### Auto-Scaling

HPA automatically scales services based on load, reducing costs during low traffic.

### Development/Staging

- Use smaller instance types
- Reduce replica counts
- Use Burstable database tier
- Shorter log retention (7-30 days)

## Disaster Recovery

### Backup Strategy

- **Database:** Daily automated backups, 35-day retention (production)
- **Configuration:** Stored in Git
- **Secrets:** Backed up in Key Vault (soft-delete enabled, 90 days)
- **Application State:** Stateless (can be redeployed)

### Recovery Time Objective (RTO)

- **Infrastructure:** 30 minutes (re-provision from Bicep)
- **Application:** 10 minutes (re-deploy from ACR)
- **Database:** 15-60 minutes (depends on backup size)

**Total RTO:** ~2 hours

### Recovery Point Objective (RPO)

- **Database:** 24 hours (daily backups)
- **Configuration:** Real-time (Git)
- **Application:** Real-time (ACR)

## CI/CD Integration

### GitHub Actions Integration

The deployment is automated via GitHub Actions:

```yaml
# .github/workflows/deploy-production.yml
- name: Deploy to AKS
  run: |
    az aks get-credentials --resource-group ${{ secrets.RESOURCE_GROUP }} --name ${{ secrets.CLUSTER_NAME }}
    ./infrastructure/scripts/deploy.sh production ${{ github.sha }}
```

### Manual Deployment

For manual deployments, follow the steps in this guide.

## Support and Contacts

### Azure Support

- **Portal:** https://portal.azure.com
- **Support:** Create support ticket in Azure Portal

### Application Issues

- **Monitoring:** Application Insights
- **Logs:** Azure Log Analytics
- **Alerts:** Check Action Group notifications

### On-Call

- **Email:** oncall@orange-rental.de
- **Slack:** #orange-car-rental-alerts

## Appendix

### Useful Commands

```bash
# Get all resources in namespace
kubectl get all -n orange-production

# Check resource quotas
kubectl describe quota -n orange-production

# Check resource usage by pod
kubectl top pod -n orange-production --sort-by=memory

# Delete stuck pod
kubectl delete pod <pod-name> -n orange-production --force --grace-period=0

# Get pod YAML
kubectl get pod <pod-name> -n orange-production -o yaml

# Execute command in pod
kubectl exec -it <pod-name> -n orange-production -- /bin/bash

# Copy file from pod
kubectl cp orange-production/<pod-name>:/path/to/file ./local-file

# Port forward to service
kubectl port-forward svc/vehicles-api 8080:8080 -n orange-production
```

### File Structure

```
infrastructure/
├── azure/
│   ├── main.bicep                 # Main infrastructure template
│   ├── modules/                   # Bicep modules
│   │   ├── acr.bicep
│   │   ├── aks.bicep
│   │   ├── postgres.bicep
│   │   ├── keyvault.bicep
│   │   ├── appinsights.bicep
│   │   ├── loganalytics.bicep
│   │   ├── storage.bicep
│   │   └── actiongroup.bicep
│   ├── parameters.dev.json
│   ├── parameters.staging.json
│   └── parameters.production.json
└── scripts/
    ├── setup-secrets.sh           # Setup Key Vault secrets
    ├── setup-ingress.sh           # Setup ingress and SSL
    ├── deploy.sh                  # Deploy application
    ├── run-migrations.sh          # Run database migrations
    ├── backup-database.sh         # Manual database backup
    └── restore-database.sh        # Restore database

k8s/
├── base/                          # Base Kubernetes manifests
│   ├── public-portal-deployment.yaml
│   ├── vehicles-api-deployment.yaml
│   ├── reservations-api-deployment.yaml
│   ├── customers-api-deployment.yaml
│   ├── locations-api-deployment.yaml
│   ├── postgres-deployment.yaml
│   ├── keycloak-deployment.yaml
│   ├── configmap.yaml
│   ├── ingress.yaml
│   ├── secrets-template.yaml
│   ├── db-migration-job.yaml
│   └── kustomization.yaml
└── overlays/                      # Environment-specific overlays
    ├── development/
    │   └── kustomization.yaml
    ├── staging/
    │   └── kustomization.yaml
    └── production/
        └── kustomization.yaml
```

### Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2024-01-21 | Initial production release |

---

**Last Updated:** 2024-01-21
**Maintained by:** DevOps Team
**Contact:** devops@orange-rental.de
