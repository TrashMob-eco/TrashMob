param env string
param region string

var keyvault_name = 'kv-tm-${env}-${region}'

resource keyvault_name_resource 'Microsoft.KeyVault/vaults@2021-04-01-preview' = {
  name: keyvault_name
  location: region
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
    vaultUri: 'https://${keyvault_name}.${environment().suffixes.keyvaultDns}/'
  }
}
