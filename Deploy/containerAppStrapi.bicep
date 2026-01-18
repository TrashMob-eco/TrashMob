param region string
param containerAppName string
param containerAppsEnvironmentId string
param containerRegistryName string
param containerImage string
param keyVaultName string
param environment string
param minReplicas int = 1
param maxReplicas int = 2

// Strapi database configuration
param strapiDatabaseHost string
param strapiDatabaseName string
param strapiDatabaseUsername string
@secure()
param strapiDatabasePassword string

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

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
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
        external: false  // Internal only - accessed via TrashMob API proxy
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
          name: 'database-password'
          value: strapiDatabasePassword
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
              value: 'mssql'
            }
            {
              name: 'DATABASE_HOST'
              value: strapiDatabaseHost
            }
            {
              name: 'DATABASE_PORT'
              value: '1433'
            }
            {
              name: 'DATABASE_NAME'
              value: strapiDatabaseName
            }
            {
              name: 'DATABASE_USERNAME'
              value: strapiDatabaseUsername
            }
            {
              name: 'DATABASE_PASSWORD'
              secretRef: 'database-password'
            }
            {
              name: 'DATABASE_SSL'
              value: 'true'
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
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
  tags: {
    environment: environment
  }
}

output containerAppFqdn string = containerApp.properties.configuration.ingress.fqdn
output containerAppName string = containerApp.name
output principalId string = containerApp.identity.principalId
