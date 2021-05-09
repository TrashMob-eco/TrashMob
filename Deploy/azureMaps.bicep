param accounts_map_trashmobdev_name string = 'map_trashmobdev'

resource accounts_map_trashmobdev_name_resource 'Microsoft.Maps/accounts@2021-02-01' = {
  name: accounts_map_trashmobdev_name
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