#!/bin/bash
# Setup NGINX Ingress Controller and cert-manager for Orange Car Rental
# Usage: ./setup-ingress.sh <environment>

set -e

ENVIRONMENT=$1

if [ -z "$ENVIRONMENT" ]; then
  echo "Usage: ./setup-ingress.sh <environment>"
  echo "Example: ./setup-ingress.sh production"
  exit 1
fi

echo "Setting up Ingress Controller and cert-manager for environment: $ENVIRONMENT"

# Configuration
CLUSTER_NAME="orange-${ENVIRONMENT}-aks"
RESOURCE_GROUP="orange-${ENVIRONMENT}-rg"
NAMESPACE="orange-${ENVIRONMENT}"

echo "Getting AKS credentials..."
az aks get-credentials \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CLUSTER_NAME" \
  --overwrite-existing

# Install NGINX Ingress Controller
echo "Installing NGINX Ingress Controller..."
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo update

helm upgrade --install ingress-nginx ingress-nginx/ingress-nginx \
  --namespace ingress-nginx \
  --create-namespace \
  --set controller.replicaCount=2 \
  --set controller.nodeSelector."kubernetes\.io/os"=linux \
  --set controller.service.annotations."service\.beta\.kubernetes\.io/azure-load-balancer-health-probe-request-path"=/healthz \
  --set controller.metrics.enabled=true \
  --set controller.podAnnotations."prometheus\.io/scrape"=true \
  --set controller.podAnnotations."prometheus\.io/port"=10254

# Wait for ingress controller to be ready
echo "Waiting for Ingress Controller to be ready..."
kubectl wait --namespace ingress-nginx \
  --for=condition=ready pod \
  --selector=app.kubernetes.io/component=controller \
  --timeout=300s

# Get the external IP
echo "Getting external IP..."
EXTERNAL_IP=$(kubectl get svc ingress-nginx-controller -n ingress-nginx -o jsonpath='{.status.loadBalancer.ingress[0].ip}')

if [ -z "$EXTERNAL_IP" ]; then
  echo "⚠️  External IP not yet assigned. Waiting..."
  sleep 30
  EXTERNAL_IP=$(kubectl get svc ingress-nginx-controller -n ingress-nginx -o jsonpath='{.status.loadBalancer.ingress[0].ip}')
fi

echo "External IP: $EXTERNAL_IP"

# Install cert-manager
echo "Installing cert-manager..."
helm repo add jetstack https://charts.jetstack.io
helm repo update

helm upgrade --install cert-manager jetstack/cert-manager \
  --namespace cert-manager \
  --create-namespace \
  --version v1.13.0 \
  --set installCRDs=true \
  --set global.leaderElection.namespace=cert-manager

# Wait for cert-manager to be ready
echo "Waiting for cert-manager to be ready..."
kubectl wait --namespace cert-manager \
  --for=condition=ready pod \
  --selector=app.kubernetes.io/instance=cert-manager \
  --timeout=300s

# Create ClusterIssuer for Let's Encrypt
echo "Creating Let's Encrypt ClusterIssuer..."
cat <<EOF | kubectl apply -f -
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: letsencrypt-staging
spec:
  acme:
    server: https://acme-staging-v02.api.letsencrypt.org/directory
    email: devops@orange-rental.de
    privateKeySecretRef:
      name: letsencrypt-staging
    solvers:
    - http01:
        ingress:
          class: nginx
---
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: letsencrypt-prod
spec:
  acme:
    server: https://acme-v02.api.letsencrypt.org/directory
    email: devops@orange-rental.de
    privateKeySecretRef:
      name: letsencrypt-prod
    solvers:
    - http01:
        ingress:
          class: nginx
EOF

echo "✅ Ingress Controller and cert-manager setup complete!"
echo ""
echo "External IP: $EXTERNAL_IP"
echo ""
echo "Next steps:"
echo "1. Configure DNS records to point to this IP:"
echo "   - orange-rental.de → $EXTERNAL_IP"
echo "   - www.orange-rental.de → $EXTERNAL_IP"
echo "   - api.orange-rental.de → $EXTERNAL_IP"
echo "   - auth.orange-rental.de → $EXTERNAL_IP"
echo ""
echo "2. Deploy your application with ingress configuration"
echo "3. SSL certificates will be automatically issued by Let's Encrypt"
