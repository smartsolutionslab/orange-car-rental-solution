// Action Group module for alerting

@description('Action group name')
param actionGroupName string

@description('Short name (max 12 characters)')
param shortName string

@description('Email address for notifications')
param emailAddress string

@description('Resource tags')
param tags object

resource actionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: actionGroupName
  location: 'global'
  tags: tags
  properties: {
    groupShortName: shortName
    enabled: true
    emailReceivers: [
      {
        name: 'DevOpsTeam'
        emailAddress: emailAddress
        useCommonAlertSchema: true
      }
    ]
    smsReceivers: []
    webhookReceivers: []
    armRoleReceivers: []
    azureFunctionReceivers: []
    logicAppReceivers: []
  }
}

output actionGroupId string = actionGroup.id
output actionGroupName string = actionGroup.name
