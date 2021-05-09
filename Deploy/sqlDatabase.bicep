param environment string
param region string

var servers_db_name = 'db-tm-${environment}-${region}'

resource servers_db_name_tm 'Microsoft.Sql/servers/databases@2020-11-01-preview' = {
  name: '${servers_db_name}/tm-${environment}'
  location: '${region}'
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
    maintenanceConfigurationId: '/subscriptions/39a254b7-c01a-45ab-bebd-4038ea4adea9/providers/Microsoft.Maintenance/publicMaintenanceConfigurations/SQL_Default'
  }
}

resource servers_db_name_tm_CreateIndex 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_name_tm.name}/CreateIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_name_tm_DbParameterization 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_name_tm.name}/DbParameterization'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_name_tm_DefragmentIndex 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_name_tm.name}/DefragmentIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_name_tm_DropIndex 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_name_tm.name}/DropIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_name_tm_ForceLastGoodPlan 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_name_tm.name}/ForceLastGoodPlan'
  properties: {
    autoExecuteValue: 'Enabled'
  }
}

resource servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/auditingPolicies@2014-04-01' = {
  name: '${servers_db_name_tm.name}/Default'
  properties: {
    auditingState: 'Disabled'
  }
}

resource Microsoft_Sql_servers_databases_auditingSettings_servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/auditingSettings@2020-11-01-preview' = {
  name: '${servers_db_name_tm.name}/Default'
  properties: {
    retentionDays: 0
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
}

resource Microsoft_Sql_servers_databases_backupLongTermRetentionPolicies_servers_db_name_tm_default 'Microsoft.Sql/servers/databases/backupLongTermRetentionPolicies@2020-11-01-preview' = {
  name: '${servers_db_name_tm.name}/default'
  properties: {
    weeklyRetention: 'PT0S'
    monthlyRetention: 'PT0S'
    yearlyRetention: 'PT0S'
    weekOfYear: 0
  }
}

resource Microsoft_Sql_servers_databases_backupShortTermRetentionPolicies_servers_db_name_tm_default 'Microsoft.Sql/servers/databases/backupShortTermRetentionPolicies@2020-11-01-preview' = {
  name: '${servers_db_name_tm.name}/default'
  properties: {
    retentionDays: 7
  }
}

resource Microsoft_Sql_servers_databases_extendedAuditingSettings_servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/extendedAuditingSettings@2020-11-01-preview' = {
  name: '${servers_db_name_tm.name}/Default'
  properties: {
    retentionDays: 0
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
}

resource Microsoft_Sql_servers_databases_geoBackupPolicies_servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/geoBackupPolicies@2014-04-01' = {
  name: '${servers_db_name_tm.name}/Default'
  properties: {
    state: 'Enabled'
  }
}

resource Microsoft_Sql_servers_databases_securityAlertPolicies_servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/securityAlertPolicies@2020-11-01-preview' = {
  name: '${servers_db_name_tm.name}/Default'
  properties: {
    state: 'Disabled'
  }
}

resource servers_db_name_tm_current 'Microsoft.Sql/servers/databases/transparentDataEncryption@2014-04-01' = {
  name: '${servers_db_name_tm.name}/current'
  properties: {
    status: 'Enabled'
  }
}

resource Microsoft_Sql_servers_databases_vulnerabilityAssessments_servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/vulnerabilityAssessments@2020-11-01-preview' = {
  name: '${servers_db_name_tm.name}/Default'
  properties: {
    recurringScans: {
      isEnabled: false
      emailSubscriptionAdmins: true
    }
  }
}
