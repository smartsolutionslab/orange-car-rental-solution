#!/bin/bash
# Deploy Orange Car Rental to Kubernetes
# Usage: ./deploy.sh <environment> [image-tag]

set -e

ENVIRONMENT=$1
IMAGE_TAG=${2:-latest}

if [ -z "$ENVIRONMENT" ]; then
  echo "Usage: ./deploy.sh <environment> [image-tag]"
  echo "Example: ./deploy.sh production v1.0.0"
  exit 1
fi

echo "Deploying Orange Car Rental to environment: $ENVIRONMENT"
echo "Image tag: $IMAGE_TAG"

# Configuration
CLUSTER_NAME="orange-${ENVIRONMENT}-aks"
RESOURCE_GROUP="orange-${ENVIRONMENT}-rg"
NAMESPACE="orange-${ENVIRONMENT}"

echo "Getting AKS credentials..."
az aks get-credentials \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CLUSTER_NAME" \
  --overwrite-existing

# Create namespace if it doesn't exist
if ! kubectl get namespace "$NAMESPACE" &> /dev/null; then
  echo "Creating namespace: $NAMESPACE"
  kubectl create namespace "$NAMESPACE"
fi

# Build manifests with kustomize and update image tags
echo "Building Kubernetes manifests..."
cd k8s/overlays/${ENVIRONMENT}

# Update image tags in kustomization.yaml
if [ "$IMAGE_TAG" != "latest" ]; then
  echo "Updating image tags to: $IMAGE_TAG"
  sed -i "s/newTag: .*/newTag: $IMAGE_TAG/g" kustomization.yaml
fi

# Build and apply manifests
kubectl apply -k . --namespace="$NAMESPACE"

cd ../../..

echo "Waiting for deployments to be ready..."

# Wait for all deployments
kubectl wait --for=condition=available --timeout=600s \
  deployment/public-portal \
  deployment/vehicles-api \
  deployment/reservations-api \
  deployment/customers-api \
  deployment/locations-api \
  deployment/keycloak \
  -n "$NAMESPACE" || true

# Check rollout status
echo ""
echo "Checking deployment status..."
kubectl rollout status deployment/public-portal -n "$NAMESPACE"
kubectl rollout status deployment/vehicles-api -n "$NAMESPACE"
kubectl rollout status deployment/reservations-api -n "$NAMESPACE"
kubectl rollout status deployment/customers-api -n "$NAMESPACE"
kubectl rollout status deployment/locations-api -n "$NAMESPACE"
kubectl rollout status deployment/keycloak -n "$NAMESPACE"

# Display pod status
echo ""
echo "Pod status:"
kubectl get pods -n "$NAMESPACE"

# Display service status
echo ""
echo "Service status:"
kubectl get svc -n "$NAMESPACE"

# Display ingress status
echo ""
echo "Ingress status:"
kubectl get ingress -n "$NAMESPACE"

echo ""
echo "✅ Deployment completed successfully!"
echo ""
echo "Access your application at:"
if [ "$ENVIRONMENT" == "production" ]; then
  echo "  - Public Portal: https://orange-rental.de"
  echo "  - API Gateway: https://api.orange-rental.de"
  echo "  - Keycloak: https://auth.orange-rental.de"
else
  echo "  - Public Portal: https://${ENVIRONMENT}.orange-rental.de"
  echo "  - API Gateway: https://api-${ENVIRONMENT}.orange-rental.de"
  echo "  - Keycloak: https://auth-${ENVIRONMENT}.orange-rental.de"
fi

echo ""
echo "To view logs:"
echo "  kubectl logs -f deployment/vehicles-api -n $NAMESPACE"
echo ""
echo "To check application health:"
echo "  kubectl exec -it deployment/vehicles-api -n $NAMESPACE -- curl http://localhost:8080/health"
