param region string
param containerAppJobName string
param containerAppsEnvironmentId string
param containerRegistryName string
param containerImage string
param keyVaultName string
param azureMapsName string
param storageAccountName string
param environment string
param cronExpression string = '0 */6 * * *' // Every 6 hours by default

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: containerRegistryName
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
  name: storageAccountName
}

resource containerAppJob 'Microsoft.App/jobs@2024-03-01' = {
  name: containerAppJobName
  location: region
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    environmentId: containerAppsEnvironmentId
    configuration: {
      triggerType: 'Schedule'
      scheduleTriggerConfig: {
        cronExpression: cronExpression
        parallelism: 1
        replicaCompletionCount: 1
      }
      replicaTimeout: 1800 // 30 minutes
      replicaRetryLimit: 1
      registries: [
        {
          server: containerRegistry.properties.loginServer
          username: containerRegistry.listCredentials().username
          passwordSecretRef: 'registry-password'
        }
      ]
      secrets: [
        {
          name: 'registry-password'
          value: containerRegistry.listCredentials().passwords[0].value
        }
      ]
    }
    template: {
      containers: [
        {
          name: containerAppJobName
          image: containerImage
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Production'
            }
            {
              name: 'VaultUri'
              value: keyVault.properties.vaultUri
            }
            {
              name: 'InstanceName'
              value: 'as-tm-${environment}-${region}'
            }
            {
              name: 'StorageAccountUri'
              value: 'https://${storageAccount.name}.blob.${az.environment().suffixes.storage}/'
            }
          ]
        }
      ]
    }
  }
  tags: {
    environment: environment
  }
}

// Grant the container app job managed identity access to the key vault
resource keyVaultAccessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: containerAppJob.identity.principalId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
  }
}

// Grant the container app job managed identity access to the storage account
// Storage Blob Data Contributor role (ba92f5b4-2d11-453d-a403-e96b0029c9fe)
resource storageRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, containerAppJob.id, 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
  scope: storageAccount
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
    principalId: containerAppJob.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

output containerAppJobName string = containerAppJob.name
output principalId string = containerAppJob.identity.principalId
