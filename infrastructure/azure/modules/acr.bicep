// Azure Container Registry module

@description('Location for ACR')
param location string

@description('ACR name (must be globally unique)')
param acrName string

@description('Resource tags')
param tags object

resource acr 'Microsoft.ContainerRegistry/registries@2023-01-01-preview' = {
  name: acrName
  location: location
  tags: tags
  sku: {
    name: 'Standard'
  }
  properties: {
    adminUserEnabled: true
    publicNetworkAccess: 'Enabled'
    networkRuleBypassOptions: 'AzureServices'
    policies: {
      quarantinePolicy: {
        status: 'disabled'
      }
      retentionPolicy: {
        days: 30
        status: 'enabled'
      }
      trustPolicy: {
        status: 'disabled'
      }
    }
    encryption: {
      status: 'disabled'
    }
    dataEndpointEnabled: false
  }
}

output loginServer string = acr.properties.loginServer
output acrId string = acr.id
output acrName string = acr.name
