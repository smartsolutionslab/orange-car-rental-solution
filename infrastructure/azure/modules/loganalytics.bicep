// Log Analytics Workspace module

@description('Location for Log Analytics')
param location string

@description('Workspace name')
param workspaceName string

@description('Retention in days')
param retentionInDays int = 30

@description('Resource tags')
param tags object

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: workspaceName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: retentionInDays
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
    workspaceCapping: {
      dailyQuotaGb: -1
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

output workspaceId string = logAnalytics.id
output workspaceName string = logAnalytics.name
output customerId string = logAnalytics.properties.customerId
