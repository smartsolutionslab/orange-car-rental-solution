// Azure Database for PostgreSQL Flexible Server module

@description('Location for PostgreSQL')
param location string

@description('Server name')
param serverName string

@description('Admin username')
param adminUsername string

@description('Database name')
param databaseName string

@description('Pricing tier')
param tier string = 'Burstable'

@description('SKU name')
param skuName string = 'Standard_B2s'

@description('Storage size in GB')
param storageSizeGB int = 64

@description('Backup retention days')
param backupRetentionDays int = 7

@description('Resource tags')
param tags object

@secure()
@description('Admin password (auto-generated if not provided)')
param adminPassword string = newGuid()

resource postgres 'Microsoft.DBforPostgreSQL/flexibleServers@2023-03-01-preview' = {
  name: serverName
  location: location
  tags: tags
  sku: {
    name: skuName
    tier: tier
  }
  properties: {
    version: '15'
    administratorLogin: adminUsername
    administratorLoginPassword: adminPassword
    storage: {
      storageSizeGB: storageSizeGB
      autoGrow: 'Enabled'
    }
    backup: {
      backupRetentionDays: backupRetentionDays
      geoRedundantBackup: tier == 'GeneralPurpose' ? 'Enabled' : 'Disabled'
    }
    highAvailability: {
      mode: tier == 'GeneralPurpose' ? 'ZoneRedundant' : 'Disabled'
    }
    network: {
      publicNetworkAccess: 'Enabled'
    }
    maintenanceWindow: {
      customWindow: 'Enabled'
      dayOfWeek: 0 // Sunday
      startHour: 2
      startMinute: 0
    }
  }
}

resource database 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2023-03-01-preview' = {
  parent: postgres
  name: databaseName
  properties: {
    charset: 'UTF8'
    collation: 'en_US.utf8'
  }
}

// Firewall rule to allow Azure services
resource firewallRule 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-03-01-preview' = {
  parent: postgres
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

output serverName string = postgres.name
output serverId string = postgres.id
output serverFqdn string = postgres.properties.fullyQualifiedDomainName
output databaseName string = database.name
