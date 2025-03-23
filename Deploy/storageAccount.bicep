param storageAccountName string = ''
param region string = ''

resource storageAccount 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: storageAccountName
  location: region
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}
