#!/bin/bash
# Restore database from backup for Orange Car Rental
# Usage: ./restore-database.sh <environment> <backup-file>

set -e

ENVIRONMENT=$1
BACKUP_FILE=$2

if [ -z "$ENVIRONMENT" ] || [ -z "$BACKUP_FILE" ]; then
  echo "Usage: ./restore-database.sh <environment> <backup-file>"
  echo "Example: ./restore-database.sh production backups/backup-20240101-120000.sql.gz"
  exit 1
fi

echo "⚠️  WARNING: This will restore the database from a backup!"
echo "Environment: $ENVIRONMENT"
echo "Backup file: $BACKUP_FILE"
echo ""
read -p "Are you sure you want to continue? (yes/NO) " -r
echo
if [[ ! $REPLY =~ ^yes$ ]]; then
  echo "Restore cancelled"
  exit 0
fi

# Configuration
CLUSTER_NAME="orange-${ENVIRONMENT}-aks"
RESOURCE_GROUP="orange-${ENVIRONMENT}-rg"
NAMESPACE="orange-${ENVIRONMENT}"

# Extract backup if gzipped
if [[ $BACKUP_FILE == *.gz ]]; then
  echo "Extracting backup file..."
  gunzip -c "$BACKUP_FILE" > /tmp/restore.sql
  RESTORE_FILE="/tmp/restore.sql"
else
  RESTORE_FILE="$BACKUP_FILE"
fi

echo "Getting AKS credentials..."
az aks get-credentials \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CLUSTER_NAME" \
  --overwrite-existing

# Get database credentials
echo "Retrieving database credentials..."
DB_USER=$(kubectl get secret database-secrets -n "$NAMESPACE" -o jsonpath='{.data.username}' | base64 -d)
DB_PASSWORD=$(kubectl get secret database-secrets -n "$NAMESPACE" -o jsonpath='{.data.password}' | base64 -d)
DB_NAME="orangecarrental"

# Scale down applications to prevent database access during restore
echo "Scaling down applications..."
kubectl scale deployment --all --replicas=0 -n "$NAMESPACE"

# Wait for pods to terminate
sleep 10

# Create backup using port-forward
echo "Setting up port forward..."
kubectl port-forward svc/postgres 5432:5432 -n "$NAMESPACE" &
PORT_FORWARD_PID=$!

# Wait for port-forward to be ready
sleep 3

# Drop and recreate database
echo "Recreating database..."
PGPASSWORD="$DB_PASSWORD" psql \
  -h localhost \
  -p 5432 \
  -U "$DB_USER" \
  -d postgres \
  -c "DROP DATABASE IF EXISTS ${DB_NAME};"

PGPASSWORD="$DB_PASSWORD" psql \
  -h localhost \
  -p 5432 \
  -U "$DB_USER" \
  -d postgres \
  -c "CREATE DATABASE ${DB_NAME};"

# Restore backup
echo "Restoring database from backup..."
PGPASSWORD="$DB_PASSWORD" psql \
  -h localhost \
  -p 5432 \
  -U "$DB_USER" \
  -d "$DB_NAME" \
  -f "$RESTORE_FILE"

# Kill port-forward
kill $PORT_FORWARD_PID

# Clean up temp file
if [ -f "/tmp/restore.sql" ]; then
  rm /tmp/restore.sql
fi

# Scale applications back up
echo "Scaling applications back up..."
kubectl scale deployment --all --replicas=1 -n "$NAMESPACE"

echo "✅ Database restore completed successfully!"
echo ""
echo "⚠️  Remember to scale your deployments to the correct replica count!"
echo "Check the kustomization.yaml for the correct replica counts per environment."
