param storageAccountName string = ''
param region string = ''

resource storageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: region
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}
