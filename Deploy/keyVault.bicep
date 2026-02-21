param environment string
param region string

// Note: accessPolicies parameter removed - KeyVault now uses RBAC authorization
// See Project 26 (KeyVault RBAC Migration) for details

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
    // RBAC authorization replaces access policies for better security governance
    // Role assignments are made separately for each managed identity
    enableRbacAuthorization: true
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
  }
}

output keyVaultName string = keyvault_name_resource.name
output keyVaultId string = keyvault_name_resource.id
