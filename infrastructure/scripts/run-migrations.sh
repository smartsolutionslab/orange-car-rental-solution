#!/bin/bash
# Run database migrations for Orange Car Rental
# Usage: ./run-migrations.sh <environment>

set -e

ENVIRONMENT=$1

if [ -z "$ENVIRONMENT" ]; then
  echo "Usage: ./run-migrations.sh <environment>"
  echo "Example: ./run-migrations.sh production"
  exit 1
fi

echo "Running database migrations for environment: $ENVIRONMENT"

# Get AKS credentials
CLUSTER_NAME="orange-${ENVIRONMENT}-aks"
RESOURCE_GROUP="orange-${ENVIRONMENT}-rg"

echo "Getting AKS credentials..."
az aks get-credentials \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CLUSTER_NAME" \
  --overwrite-existing

# Set namespace
NAMESPACE="orange-${ENVIRONMENT}"

# Check if namespace exists
if ! kubectl get namespace "$NAMESPACE" &> /dev/null; then
  echo "Creating namespace: $NAMESPACE"
  kubectl create namespace "$NAMESPACE"
fi

# Apply kustomization to get the right configuration
echo "Building Kubernetes manifests..."
kubectl kustomize "k8s/overlays/${ENVIRONMENT}" > /tmp/migration-job.yaml

# Extract just the migration job
echo "Deploying migration job..."
kubectl apply -f k8s/base/db-migration-job.yaml -n "$NAMESPACE"

# Wait for job to complete
echo "Waiting for migration to complete..."
kubectl wait --for=condition=complete --timeout=300s job/db-migration -n "$NAMESPACE"

# Show logs
echo "Migration logs:"
kubectl logs -l app=db-migration -n "$NAMESPACE" --tail=50

# Check job status
JOB_STATUS=$(kubectl get job db-migration -n "$NAMESPACE" -o jsonpath='{.status.succeeded}')

if [ "$JOB_STATUS" == "1" ]; then
  echo "✅ Migration completed successfully!"
else
  echo "❌ Migration failed!"
  kubectl describe job db-migration -n "$NAMESPACE"
  exit 1
fi

# Clean up job (optional)
read -p "Delete migration job? (y/N) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
  kubectl delete job db-migration -n "$NAMESPACE"
  echo "Migration job deleted"
fi
