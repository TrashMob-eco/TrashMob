param environment string
param region string

// Backup retention settings
param shortTermRetentionDays int = 14
param weeklyRetention string = 'P4W'    // Keep weekly backups for 4 weeks
param monthlyRetention string = 'P12M'  // Keep monthly backups for 12 months

var servers_db_name = 'sql-tm-${environment}-${region}'
var db_Name = 'db-tm-${environment}-${region}'

resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' existing = {
  name: servers_db_name
}

resource servers_db_name_tm 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: sqlServer
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

// Short-term retention policy (point-in-time restore window)
// See Project 32 (Database Backups) for details
resource shortTermRetention 'Microsoft.Sql/servers/databases/backupShortTermRetentionPolicies@2023-05-01-preview' = {
  parent: servers_db_name_tm
  name: 'default'
  properties: {
    retentionDays: shortTermRetentionDays
    diffBackupIntervalInHours: 24
  }
}

// Long-term retention policy (weekly and monthly backups)
// See Project 32 (Database Backups) for details
resource longTermRetention 'Microsoft.Sql/servers/databases/backupLongTermRetentionPolicies@2023-05-01-preview' = {
  parent: servers_db_name_tm
  name: 'default'
  properties: {
    weeklyRetention: weeklyRetention
    monthlyRetention: monthlyRetention
    yearlyRetention: 'PT0S'  // Not using yearly retention
    weekOfYear: 1
  }
}
