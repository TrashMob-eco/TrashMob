param region string
param containerAppName string
param containerAppsEnvironmentId string
param containerRegistryName string
param containerImage string
param storageAccountName string
param environment string
param minReplicas int = 1
param maxReplicas int = 1  // SQLite requires single replica

// Strapi secrets
@secure()
param strapiAdminJwtSecret string
@secure()
param strapiApiTokenSalt string
@secure()
param strapiAppKeys string
@secure()
param strapiTransferTokenSalt string

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: containerRegistryName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

// Create file share for Strapi uploads (SQLite uses local ephemeral storage due to SMB locking issues)
resource fileServices 'Microsoft.Storage/storageAccounts/fileServices@2023-01-01' existing = {
  parent: storageAccount
  name: 'default'
}

resource strapiUploadsShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2023-01-01' = {
  parent: fileServices
  name: 'strapi-uploads'
  properties: {
    shareQuota: 5  // 5 GB quota for media uploads
  }
}

// Reference existing Container Apps Environment
resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' existing = {
  name: split(containerAppsEnvironmentId, '/')[8]  // Extract name from resource ID
}

// Add Azure Files storage to the managed environment for uploads
resource environmentUploadsStorage 'Microsoft.App/managedEnvironments/storages@2024-03-01' = {
  parent: containerAppsEnvironment
  name: 'strapi-uploads'
  properties: {
    azureFile: {
      accountName: storageAccountName
      accountKey: storageAccount.listKeys().keys[0].value
      shareName: 'strapi-uploads'
      accessMode: 'ReadWrite'
    }
  }
  dependsOn: [
    strapiUploadsShare
  ]
}

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: region
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId
    configuration: {
      ingress: {
        external: true  // External access needed for admin panel (Strapi has built-in authentication)
        targetPort: 1337
        transport: 'auto'
        allowInsecure: false
      }
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
        {
          name: 'admin-jwt-secret'
          value: strapiAdminJwtSecret
        }
        {
          name: 'api-token-salt'
          value: strapiApiTokenSalt
        }
        {
          name: 'app-keys'
          value: strapiAppKeys
        }
        {
          name: 'transfer-token-salt'
          value: strapiTransferTokenSalt
        }
      ]
    }
    template: {
      containers: [
        {
          name: containerAppName
          image: containerImage
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
          env: [
            {
              name: 'DATABASE_CLIENT'
              value: 'sqlite'  // SQLite uses local ephemeral storage (Azure Files SMB has locking issues)
            }
            {
              name: 'ADMIN_JWT_SECRET'
              secretRef: 'admin-jwt-secret'
            }
            {
              name: 'API_TOKEN_SALT'
              secretRef: 'api-token-salt'
            }
            {
              name: 'APP_KEYS'
              secretRef: 'app-keys'
            }
            {
              name: 'TRANSFER_TOKEN_SALT'
              secretRef: 'transfer-token-salt'
            }
            {
              name: 'NODE_ENV'
              value: 'production'
            }
            {
              name: 'HOST'
              value: '0.0.0.0'
            }
            {
              name: 'PORT'
              value: '1337'
            }
          ]
          volumeMounts: [
            {
              volumeName: 'strapi-uploads'
              mountPath: '/app/public/uploads'
            }
          ]
          probes: [
            {
              type: 'liveness'
              httpGet: {
                path: '/_health'
                port: 1337
              }
              initialDelaySeconds: 60
              periodSeconds: 30
              timeoutSeconds: 10
              failureThreshold: 3
            }
            {
              type: 'readiness'
              httpGet: {
                path: '/_health'
                port: 1337
              }
              initialDelaySeconds: 30
              periodSeconds: 10
              timeoutSeconds: 5
              failureThreshold: 3
            }
          ]
        }
      ]
      volumes: [
        {
          name: 'strapi-uploads'
          storageType: 'AzureFile'
          storageName: 'strapi-uploads'
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
  tags: {
    environment: environment
  }
  dependsOn: [
    environmentUploadsStorage
  ]
}

output containerAppFqdn string = containerApp.properties.configuration.ingress.fqdn
output containerAppName string = containerApp.name
output principalId string = containerApp.identity.principalId
