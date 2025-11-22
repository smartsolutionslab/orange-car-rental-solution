#!/bin/bash
# Manual database backup for Orange Car Rental
# Usage: ./backup-database.sh <environment>

set -e

ENVIRONMENT=$1

if [ -z "$ENVIRONMENT" ]; then
  echo "Usage: ./backup-database.sh <environment>"
  echo "Example: ./backup-database.sh production"
  exit 1
fi

echo "Creating manual database backup for environment: $ENVIRONMENT"

# Configuration
CLUSTER_NAME="orange-${ENVIRONMENT}-aks"
RESOURCE_GROUP="orange-${ENVIRONMENT}-rg"
NAMESPACE="orange-${ENVIRONMENT}"
BACKUP_DIR="./backups"
BACKUP_FILE="manual-backup-$(date +%Y%m%d-%H%M%S).sql"

# Create backup directory
mkdir -p "$BACKUP_DIR"

echo "Getting AKS credentials..."
az aks get-credentials \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CLUSTER_NAME" \
  --overwrite-existing

# Get database credentials from secrets
echo "Retrieving database credentials..."
DB_HOST=$(kubectl get svc postgres -n "$NAMESPACE" -o jsonpath='{.spec.clusterIP}')
DB_USER=$(kubectl get secret database-secrets -n "$NAMESPACE" -o jsonpath='{.data.username}' | base64 -d)
DB_PASSWORD=$(kubectl get secret database-secrets -n "$NAMESPACE" -o jsonpath='{.data.password}' | base64 -d)
DB_NAME="orangecarrental"

# Create backup using port-forward
echo "Creating backup..."
kubectl port-forward svc/postgres 5432:5432 -n "$NAMESPACE" &
PORT_FORWARD_PID=$!

# Wait for port-forward to be ready
sleep 3

# Run pg_dump
PGPASSWORD="$DB_PASSWORD" pg_dump \
  -h localhost \
  -p 5432 \
  -U "$DB_USER" \
  -d "$DB_NAME" \
  -F p \
  -f "${BACKUP_DIR}/${BACKUP_FILE}"

# Kill port-forward
kill $PORT_FORWARD_PID

# Compress backup
echo "Compressing backup..."
gzip "${BACKUP_DIR}/${BACKUP_FILE}"

echo "✅ Backup completed successfully!"
echo "Backup file: ${BACKUP_DIR}/${BACKUP_FILE}.gz"
echo ""

# Upload to Azure Storage (optional)
read -p "Upload backup to Azure Storage? (y/N) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
  STORAGE_ACCOUNT="orange${ENVIRONMENT}storage"

  echo "Uploading to Azure Storage..."
  az storage blob upload \
    --account-name "$STORAGE_ACCOUNT" \
    --container-name backups \
    --file "${BACKUP_DIR}/${BACKUP_FILE}.gz" \
    --name "${BACKUP_FILE}.gz" \
    --auth-mode login

  echo "✅ Backup uploaded to Azure Storage"
fi
