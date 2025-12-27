// Storage Account module

@description('Location for storage account')
param location string

@description('Storage account name (must be globally unique, 3-24 lowercase alphanumeric)')
param storageAccountName string

@description('Resource tags')
param tags object

resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
    encryption: {
      services: {
        blob: {
          enabled: true
        }
        file: {
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
  }
}

// Blob container for backups
resource backupsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  name: '${storage.name}/default/backups'
  properties: {
    publicAccess: 'None'
  }
}

// Blob container for logs
resource logsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  name: '${storage.name}/default/logs'
  properties: {
    publicAccess: 'None'
  }
}

output storageAccountName string = storage.name
output storageAccountId string = storage.id
output primaryEndpoints object = storage.properties.primaryEndpoints
