param environment string
param region string
param subscriptionId string

var servers_db_name = 'sql-tm-${environment}-${region}'
var db_Name = 'db-tm-${environment}-${region}'

resource servers_db_name_tm 'Microsoft.Sql/servers/databases@2020-11-01-preview' = {
  name: '${servers_db_name}/${db_Name}'
  location: region
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Geo'
    maintenanceConfigurationId: '/subscriptions/${subscriptionId}/providers/Microsoft.Maintenance/publicMaintenanceConfigurations/SQL_Default'
  }
}
