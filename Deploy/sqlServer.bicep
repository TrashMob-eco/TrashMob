@secure()

param environment string
param region string

@secure()
param sqlAdminPassword string

var servers_db_name = 'sql-tm-${environment}-${region}'
var db_Name = 'db-tm-${environment}-${region}'

resource servers_db_name_resource 'Microsoft.Sql/servers@2021-11-01' = {
  name: servers_db_name
  location: region
  properties: {
    administratorLogin: 'dbadmin'
    version: '12.0'
    publicNetworkAccess: 'Enabled'
    administratorLoginPassword: sqlAdminPassword
  }
}

resource servers_db_name_tm_ 'Microsoft.Sql/servers/databases@2020-11-01-preview' = {
  parent: servers_db_name_resource
  name: db_Name
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
    maintenanceConfigurationId: subscriptionResourceId('Microsoft.Maintenance/publicMaintenanceConfigurations', 'SQL_Default')
  }
}

resource servers_db_name_AllowAllWindowsAzureIps 'Microsoft.Sql/servers/firewallRules@2021-11-01' = {
  parent: servers_db_name_resource
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}
