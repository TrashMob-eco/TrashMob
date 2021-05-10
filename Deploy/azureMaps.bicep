param environment string = ''
param region string = ''

var account_map_name = 'map-tm-${environment}-${region}'

resource accounts_map_trashmobdev_name_resource 'Microsoft.Maps/accounts@2021-02-01' = {
  name: account_map_name
  location: 'global'
  sku: {
    name: 'S0'
    tier: 'Standard'
  }
  kind: 'Gen1'
  properties: {
    disableLocalAuth: false
  }
}
