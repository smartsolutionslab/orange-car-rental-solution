// Main Bicep template for Orange Car Rental Azure Infrastructure
// Deploy with: az deployment sub create --location westeurope --template-file main.bicep

targetScope = 'subscription'

@description('Environment name (dev, staging, production)')
@allowed(['dev', 'staging', 'production'])
param environment string = 'production'

@description('Azure region for resources')
param location string = 'westeurope'

@description('Resource name prefix')
param prefix string = 'orange'

@description('Administrator email for notifications')
param adminEmail string

@description('Tags for all resources')
param tags object = {
  project: 'orange-car-rental'
  managedBy: 'bicep'
}

// Resource Group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: '${prefix}-${environment}-rg'
  location: location
  tags: tags
}

// Azure Container Registry
module acr './modules/acr.bicep' = {
  scope: rg
  name: 'acr-deployment'
  params: {
    location: location
    acrName: '${prefix}${environment}acr'
    tags: tags
  }
}

// Azure Kubernetes Service
module aks './modules/aks.bicep' = {
  scope: rg
  name: 'aks-deployment'
  params: {
    location: location
    clusterName: '${prefix}-${environment}-aks'
    nodeCount: environment == 'production' ? 5 : 3
    nodeSize: environment == 'production' ? 'Standard_D4s_v3' : 'Standard_D2s_v3'
    tags: tags
  }
}

// Azure SQL Database
module sqlServer './modules/sqlserver.bicep' = {
  scope: rg
  name: 'sqlserver-deployment'
  params: {
    location: location
    serverName: '${prefix}-${environment}-sql'
    adminUsername: 'sqladmin'
    databaseName: 'orangecarrental'
    skuName: environment == 'production' ? 'S3' : 'Basic'
    skuTier: environment == 'production' ? 'Standard' : 'Basic'
    maxSizeGB: environment == 'production' ? 250 : 2
    backupRetentionDays: environment == 'production' ? 35 : 7
    tags: tags
  }
}

// Azure Key Vault
module keyVault './modules/keyvault.bicep' = {
  scope: rg
  name: 'keyvault-deployment'
  params: {
    location: location
    keyVaultName: '${prefix}-${environment}-kv'
    tenantId: subscription().tenantId
    tags: tags
  }
}

// Application Insights
module appInsights './modules/appinsights.bicep' = {
  scope: rg
  name: 'appinsights-deployment'
  params: {
    location: location
    appInsightsName: '${prefix}-${environment}-ai'
    tags: tags
  }
}

// Log Analytics Workspace
module logAnalytics './modules/loganalytics.bicep' = {
  scope: rg
  name: 'loganalytics-deployment'
  params: {
    location: location
    workspaceName: '${prefix}-${environment}-logs'
    retentionInDays: environment == 'production' ? 90 : 30
    tags: tags
  }
}

// Storage Account for backups and logs
module storage './modules/storage.bicep' = {
  scope: rg
  name: 'storage-deployment'
  params: {
    location: location
    storageAccountName: '${prefix}${environment}storage'
    tags: tags
  }
}

// Action Group for alerts
module actionGroup './modules/actiongroup.bicep' = {
  scope: rg
  name: 'actiongroup-deployment'
  params: {
    actionGroupName: '${prefix}-${environment}-alerts'
    shortName: 'OrangeAlrt'
    emailAddress: adminEmail
    tags: tags
  }
}

// Outputs
output resourceGroupName string = rg.name
output acrLoginServer string = acr.outputs.loginServer
output aksClusterName string = aks.outputs.clusterName
output sqlServerName string = sqlServer.outputs.serverName
output keyVaultName string = keyVault.outputs.keyVaultName
output appInsightsConnectionString string = appInsights.outputs.connectionString
output logAnalyticsWorkspaceId string = logAnalytics.outputs.workspaceId
output storageAccountName string = storage.outputs.storageAccountName
