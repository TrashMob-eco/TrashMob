param environment string
param region string
param accessPolicies array = []

var keyvault_name = 'kv-tm-${environment}-${region}'

resource keyvault_name_resource 'Microsoft.KeyVault/vaults@2024-11-01' = {
  name: keyvault_name
  location: region
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: accessPolicies
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enableRbacAuthorization: false
  }
}
