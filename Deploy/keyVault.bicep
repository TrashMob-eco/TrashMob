param vaults_kv_ftm_dev_name string = 'kv-ftm-dev'

resource vaults_kv_ftm_dev_name_resource 'Microsoft.KeyVault/vaults@2021-04-01-preview' = {
  name: vaults_kv_ftm_dev_name
  location: 'westus2'
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: 'f0062da2-b273-427c-b99e-6e85f75b23eb'
    accessPolicies: [
      {
        tenantId: 'f0062da2-b273-427c-b99e-6e85f75b23eb'
        objectId: '4a6d045b-bcc6-4cdb-9d29-d519db9535f8'
        permissions: {
          keys: [
            'get'
            'list'
            'update'
            'create'
            'import'
            'delete'
            'recover'
            'backup'
            'restore'
          ]
          secrets: [
            'get'
            'list'
            'set'
            'delete'
            'recover'
            'backup'
            'restore'
          ]
          certificates: [
            'get'
            'list'
            'update'
            'create'
            'import'
            'delete'
            'recover'
            'backup'
            'restore'
            'managecontacts'
            'manageissuers'
            'getissuers'
            'listissuers'
            'setissuers'
            'deleteissuers'
          ]
        }
      }
      {
        tenantId: 'f0062da2-b273-427c-b99e-6e85f75b23eb'
        objectId: '88e9c4c3-4391-4cab-a6e7-19dcd79844db'
        permissions: {
          keys: [
            'get'
            'list'
            'update'
            'create'
            'import'
            'delete'
            'recover'
            'backup'
            'restore'
          ]
          secrets: [
            'get'
            'list'
            'set'
            'delete'
            'recover'
            'backup'
            'restore'
          ]
          certificates: [
            'get'
            'list'
            'update'
            'create'
            'import'
            'delete'
            'recover'
            'backup'
            'restore'
            'managecontacts'
            'manageissuers'
            'getissuers'
            'listissuers'
            'setissuers'
            'deleteissuers'
          ]
        }
      }
      {
        tenantId: 'f0062da2-b273-427c-b99e-6e85f75b23eb'
        objectId: '2b1d93cb-8cee-4a9c-848a-dbe68395c07a'
        permissions: {
          keys: [
            'get'
            'list'
            'update'
            'create'
            'import'
            'delete'
            'recover'
            'backup'
            'restore'
          ]
          secrets: [
            'get'
            'list'
            'set'
            'delete'
            'recover'
            'backup'
            'restore'
          ]
          certificates: [
            'get'
            'list'
            'update'
            'create'
            'import'
            'delete'
            'recover'
            'backup'
            'restore'
            'managecontacts'
            'manageissuers'
            'getissuers'
            'listissuers'
            'setissuers'
            'deleteissuers'
          ]
        }
      }
      {
        tenantId: 'f0062da2-b273-427c-b99e-6e85f75b23eb'
        objectId: '7315d8f3-ecd4-4409-b800-ef779d71f58b'
        permissions: {
          keys: []
          secrets: [
            'get'
            'list'
          ]
          certificates: []
        }
      }
    ]
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enableRbacAuthorization: false
    vaultUri: 'https://${vaults_kv_ftm_dev_name}.vault.azure.net/'
    provisioningState: 'Succeeded'
  }
}

resource vaults_kv_ftm_dev_name_AzureMapsDev 'Microsoft.KeyVault/vaults/secrets@2021-04-01-preview' = {
  name: '${vaults_kv_ftm_dev_name_resource.name}/AzureMapsDev'
  location: 'westus2'
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource vaults_kv_ftm_dev_name_AzureMapsKey 'Microsoft.KeyVault/vaults/secrets@2021-04-01-preview' = {
  name: '${vaults_kv_ftm_dev_name_resource.name}/AzureMapsKey'
  location: 'westus2'
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource vaults_kv_ftm_dev_name_ConnectionStrings_TMDBServerConnectionString 'Microsoft.KeyVault/vaults/secrets@2021-04-01-preview' = {
  name: '${vaults_kv_ftm_dev_name_resource.name}/ConnectionStrings--TMDBServerConnectionString'
  location: 'westus2'
  properties: {
    attributes: {
      enabled: true
    }
  }
}

resource vaults_kv_ftm_dev_name_TMDBServerConnectionString 'Microsoft.KeyVault/vaults/secrets@2021-04-01-preview' = {
  name: '${vaults_kv_ftm_dev_name_resource.name}/TMDBServerConnectionString'
  location: 'westus2'
  properties: {
    attributes: {
      enabled: true
    }
  }
}