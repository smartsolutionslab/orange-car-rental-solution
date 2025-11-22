#!/bin/bash
# Setup Azure Key Vault secrets for Orange Car Rental
# Usage: ./setup-secrets.sh <environment> <key-vault-name>

set -e

ENVIRONMENT=$1
KEY_VAULT_NAME=$2

if [ -z "$ENVIRONMENT" ] || [ -z "$KEY_VAULT_NAME" ]; then
  echo "Usage: ./setup-secrets.sh <environment> <key-vault-name>"
  echo "Example: ./setup-secrets.sh production orange-production-kv"
  exit 1
fi

echo "Setting up secrets in Key Vault: $KEY_VAULT_NAME for environment: $ENVIRONMENT"

# Generate secure passwords
DB_PASSWORD=$(openssl rand -base64 32)
KEYCLOAK_DB_PASSWORD=$(openssl rand -base64 32)
KEYCLOAK_ADMIN_PASSWORD=$(openssl rand -base64 32)

# Database secrets
echo "Creating database secrets..."
az keyvault secret set \
  --vault-name "$KEY_VAULT_NAME" \
  --name "database-username" \
  --value "pgadmin" \
  --description "PostgreSQL admin username"

az keyvault secret set \
  --vault-name "$KEY_VAULT_NAME" \
  --name "database-password" \
  --value "$DB_PASSWORD" \
  --description "PostgreSQL admin password"

# Get PostgreSQL server FQDN (assumes naming convention)
POSTGRES_SERVER="orange-${ENVIRONMENT}-postgres.postgres.database.azure.com"

az keyvault secret set \
  --vault-name "$KEY_VAULT_NAME" \
  --name "database-connection-string" \
  --value "Host=${POSTGRES_SERVER};Database=orangecarrental;Username=pgadmin;Password=${DB_PASSWORD};SSL Mode=Require" \
  --description "PostgreSQL connection string"

# Keycloak database secrets
echo "Creating Keycloak database secrets..."
az keyvault secret set \
  --vault-name "$KEY_VAULT_NAME" \
  --name "keycloak-db-username" \
  --value "keycloak" \
  --description "Keycloak database username"

az keyvault secret set \
  --vault-name "$KEY_VAULT_NAME" \
  --name "keycloak-db-password" \
  --value "$KEYCLOAK_DB_PASSWORD" \
  --description "Keycloak database password"

# Keycloak admin secrets
echo "Creating Keycloak admin secrets..."
az keyvault secret set \
  --vault-name "$KEY_VAULT_NAME" \
  --name "keycloak-admin-username" \
  --value "admin" \
  --description "Keycloak admin username"

az keyvault secret set \
  --vault-name "$KEY_VAULT_NAME" \
  --name "keycloak-admin-password" \
  --value "$KEYCLOAK_ADMIN_PASSWORD" \
  --description "Keycloak admin password"

# Application Insights connection string
echo "Creating Application Insights secret..."
APP_INSIGHTS_NAME="orange-${ENVIRONMENT}-ai"
RESOURCE_GROUP="orange-${ENVIRONMENT}-rg"

APP_INSIGHTS_CONNECTION_STRING=$(az monitor app-insights component show \
  --app "$APP_INSIGHTS_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --query "connectionString" \
  --output tsv)

az keyvault secret set \
  --vault-name "$KEY_VAULT_NAME" \
  --name "appinsights-connection-string" \
  --value "$APP_INSIGHTS_CONNECTION_STRING" \
  --description "Application Insights connection string"

echo ""
echo "✅ Secrets successfully created in $KEY_VAULT_NAME"
echo ""
echo "⚠️  IMPORTANT: Save these credentials securely!"
echo "---------------------------------------------------"
echo "Database Password: $DB_PASSWORD"
echo "Keycloak DB Password: $KEYCLOAK_DB_PASSWORD"
echo "Keycloak Admin Password: $KEYCLOAK_ADMIN_PASSWORD"
echo "---------------------------------------------------"
echo ""
echo "Next steps:"
echo "1. Configure AKS to use managed identity for Key Vault access"
echo "2. Deploy SecretProviderClass to Kubernetes"
echo "3. Deploy applications"
