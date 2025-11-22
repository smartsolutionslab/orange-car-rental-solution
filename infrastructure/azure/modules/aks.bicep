// Azure Kubernetes Service module

@description('Location for AKS cluster')
param location string

@description('AKS cluster name')
param clusterName string

@description('Number of nodes')
param nodeCount int = 3

@description('VM size for nodes')
param nodeSize string = 'Standard_D2s_v3'

@description('Resource tags')
param tags object

resource aks 'Microsoft.ContainerService/managedClusters@2023-10-01' = {
  name: clusterName
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    dnsPrefix: clusterName
    kubernetesVersion: '1.28.3'
    enableRBAC: true

    agentPoolProfiles: [
      {
        name: 'systempool'
        count: nodeCount
        vmSize: nodeSize
        osType: 'Linux'
        mode: 'System'
        enableAutoScaling: true
        minCount: nodeCount
        maxCount: nodeCount + 5
        type: 'VirtualMachineScaleSets'
        availabilityZones: [
          '1'
          '2'
          '3'
        ]
      }
    ]

    networkProfile: {
      networkPlugin: 'azure'
      networkPolicy: 'azure'
      serviceCidr: '10.0.0.0/16'
      dnsServiceIP: '10.0.0.10'
      loadBalancerSku: 'standard'
    }

    addonProfiles: {
      azureKeyvaultSecretsProvider: {
        enabled: true
      }
      omsagent: {
        enabled: true
      }
      azurepolicy: {
        enabled: true
      }
    }

    autoUpgradeProfile: {
      upgradeChannel: 'stable'
    }

    securityProfile: {
      workloadIdentity: {
        enabled: true
      }
    }
  }
}

output clusterName string = aks.name
output clusterId string = aks.id
output kubeletIdentityObjectId string = aks.properties.identityProfile.kubeletidentity.objectId
output nodeResourceGroup string = aks.properties.nodeResourceGroup
