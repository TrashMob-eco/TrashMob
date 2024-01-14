param environment string = ''
param region string = ''

var account_map_name = 'map-tm-${environment}-${region}'

resource accounts_map_trashmobdev_name_resource 'Microsoft.Maps/accounts@2021-02-01' = {
  name: account_map_name
  location: 'global'
  sku: {
    name: 'G2'
  }
  kind: 'Gen2'
  properties: {
    disableLocalAuth: false
  }
}
