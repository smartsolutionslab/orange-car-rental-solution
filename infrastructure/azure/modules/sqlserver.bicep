// Azure SQL Database module

@description('Location for SQL Server')
param location string

@description('Server name')
param serverName string

@description('Admin username')
param adminUsername string

@description('Database name')
param databaseName string

@description('SKU name')
param skuName string = 'Basic'

@description('SKU tier')
param skuTier string = 'Basic'

@description('Database size in GB')
param maxSizeGB int = 2

@description('Backup retention days')
param backupRetentionDays int = 7

@description('Resource tags')
param tags object

@secure()
@description('Admin password')
param adminPassword string = newGuid()

resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: serverName
  location: location
  tags: tags
  properties: {
    administratorLogin: adminUsername
    administratorLoginPassword: adminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

resource database 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  tags: tags
  sku: {
    name: skuName
    tier: skuTier
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: maxSizeGB * 1024 * 1024 * 1024
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: skuTier == 'Premium' || skuTier == 'BusinessCritical'
    requestedBackupStorageRedundancy: skuTier == 'Premium' ? 'Geo' : 'Local'
  }
}

// Firewall rule to allow Azure services
resource firewallRule 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Short-term backup retention
resource backupPolicy 'Microsoft.Sql/servers/databases/backupShortTermRetentionPolicies@2023-05-01-preview' = {
  parent: database
  name: 'default'
  properties: {
    retentionDays: backupRetentionDays
  }
}

output serverName string = sqlServer.name
output serverId string = sqlServer.id
output serverFqdn string = sqlServer.properties.fullyQualifiedDomainName
output databaseName string = database.name
