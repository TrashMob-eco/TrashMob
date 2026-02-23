param region string
param containerAppName string
param containerAppsEnvironmentId string
param containerRegistryName string
param containerImage string
param keyVaultName string
param storageAccountName string
param environment string
param minReplicas int = 1
param maxReplicas int = 3
param strapiBaseUrl string = ''

// Custom domain configuration (optional)
// The managed certificate must be created separately before deployment
param customDomainName string = ''
param managedCertificateName string = ''

// Build the managed certificate resource ID if provided
var managedCertificateId = managedCertificateName != '' ? '${containerAppsEnvironmentId}/managedCertificates/${managedCertificateName}' : ''

// Derive the Application Insights name from environment and region
var appInsightsName = 'ai-tm-${environment}-${region}'

// Azure AD Entra External ID configuration - these are public values, not secrets
// Note: Microsoft accounts work natively in Entra External ID (no external IDP setup needed)
var entraInstance = environment == 'dev' ? 'https://auth-dev.trashmob.eco/' : 'https://auth.trashmob.eco/'
var entraDomain = environment == 'dev' ? 'TrashMobEcoDev.onmicrosoft.com' : 'trashmobecopr.onmicrosoft.com'
var entraBackendClientId = environment == 'dev' ? '84df543d-6535-45f5-afab-4d38528b721a' : 'dc09e17b-bce4-4af9-82ab-f7b12af586b4'
var entraTenantId = environment == 'dev' ? '8577fa31-4b86-4e4b-8b02-93fba708cb19' : 'b5fc8717-29eb-496e-8e09-cf90d344ce9f'
var entraFrontendClientId = environment == 'dev' ? '1e6ae74d-0160-4a01-9d75-04048e03b17e' : '0604ef02-6b84-450f-b5d5-2196e96f3b48'

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: containerRegistryName
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' existing = {
  name: storageAccountName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
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
        external: true
        targetPort: 8080
        transport: 'auto'
        allowInsecure: false
        customDomains: customDomainName != '' && managedCertificateId != '' ? [
          {
            name: customDomainName
            certificateId: managedCertificateId
            bindingType: 'SniEnabled'
          }
        ] : []
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
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Production'
            }
            {
              name: 'VaultUri'
              value: keyVault.properties.vaultUri
            }
            {
              name: 'StorageAccountUri'
              value: 'https://${storageAccount.name}.blob.${az.environment().suffixes.storage}/'
            }
            {
              name: 'ASPNETCORE_HTTP_PORTS'
              value: '8080'
            }
            {
              name: 'EnableSwagger'
              value: environment != 'prod' ? 'true' : 'false'
            }
            {
              name: 'ApplicationInsights__ConnectionString'
              value: appInsights.properties.ConnectionString
            }
            // Azure AD Entra External ID backend settings (for JWT validation)
            {
              name: 'AzureAdEntra__Instance'
              value: entraInstance
            }
            {
              name: 'AzureAdEntra__ClientId'
              value: entraBackendClientId
            }
            {
              name: 'AzureAdEntra__Domain'
              value: entraDomain
            }
            {
              name: 'AzureAdEntra__TenantId'
              value: entraTenantId
            }
            // Azure AD Entra External ID frontend settings (for MSAL in browser)
            {
              name: 'AzureAdEntra__FrontendClientId'
              value: entraFrontendClientId
            }
            // Strapi CMS integration
            {
              name: 'StrapiBaseUrl'
              value: strapiBaseUrl
            }
          ]
          probes: [
            {
              type: 'liveness'
              httpGet: {
                path: '/health/live'
                port: 8080
              }
              initialDelaySeconds: 60
              periodSeconds: 30
              timeoutSeconds: 10
              failureThreshold: 3
            }
            {
              type: 'readiness'
              httpGet: {
                path: '/health'
                port: 8080
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
        rules: [
          {
            name: 'http-scaling'
            http: {
              metadata: {
                concurrentRequests: '100'
              }
            }
          }
        ]
      }
    }
  }
  tags: {
    environment: environment
  }
}

// Note: Key Vault access is granted via RBAC (Key Vault Secrets User role) in the GitHub workflow
// See Project 26 (KeyVault RBAC Migration) for details on the RBAC authorization model

output containerAppFqdn string = containerApp.properties.configuration.ingress.fqdn
output containerAppName string = containerApp.name
output principalId string = containerApp.identity.principalId
