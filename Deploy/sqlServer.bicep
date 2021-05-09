@secure()

param environment string
param region string
param subscriptionId string
param sqlAdminPassword string

var servers_db_name = 'sql-tm-${environment}-${region}'
var db_Name = 'db-tm-${environment}-${region}'

resource servers_db_name_resource 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: servers_db_name
  location: '${region}'
  properties: {
    administratorLogin: 'dbadmin'
    version: '12.0'
    publicNetworkAccess: 'Enabled'
    administratorLoginPassword: '${sqlAdminPassword}'
  }
}

resource servers_db_name_CreateIndex 'Microsoft.Sql/servers/advisors@2014-04-01' = {
  name: '${servers_db_name_resource.name}/CreateIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_name_DbParameterization 'Microsoft.Sql/servers/advisors@2014-04-01' = {
  name: '${servers_db_name_resource.name}/DbParameterization'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_name_DefragmentIndex 'Microsoft.Sql/servers/advisors@2014-04-01' = {
  name: '${servers_db_name_resource.name}/DefragmentIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_name_DropIndex 'Microsoft.Sql/servers/advisors@2014-04-01' = {
  name: '${servers_db_name_resource.name}/DropIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_name_ForceLastGoodPlan 'Microsoft.Sql/servers/advisors@2014-04-01' = {
  name: '${servers_db_name_resource.name}/ForceLastGoodPlan'
  properties: {
    autoExecuteValue: 'Enabled'
  }
}

resource servers_db_name_Default 'Microsoft.Sql/servers/auditingPolicies@2014-04-01' = {
  name: '${servers_db_name_resource.name}/Default'
  properties: {
    auditingState: 'Disabled'
  }
}

resource Microsoft_Sql_servers_auditingSettings_servers_db_name_Default 'Microsoft.Sql/servers/auditingSettings@2020-11-01-preview' = {
  name: '${servers_db_name_resource.name}/Default'
  properties: {
    retentionDays: 0
    auditActionsAndGroups: []
    isStorageSecondaryKeyInUse: false
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
}

resource servers_db_name_tm_ 'Microsoft.Sql/servers/databases@2020-11-01-preview' = {
  name: '${servers_db_name_resource.name}/${db_Name}'
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
    maintenanceConfigurationId: '/subscriptions/${subscriptionId}/providers/Microsoft.Maintenance/publicMaintenanceConfigurations/SQL_Default'
  }
}

resource servers_db_name_master_Default 'Microsoft.Sql/servers/databases/auditingPolicies@2014-04-01' = {
  name: '${servers_db_name}/master/Default'
  properties: {
    auditingState: 'Disabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_auditingSettings_servers_db_name_master_Default 'Microsoft.Sql/servers/databases/auditingSettings@2020-11-01-preview' = {
  name: '${servers_db_name}/master/Default'
  properties: {
    retentionDays: 0
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_extendedAuditingSettings_servers_db_name_master_Default 'Microsoft.Sql/servers/databases/extendedAuditingSettings@2020-11-01-preview' = {
  name: '${servers_db_name}/master/Default'
  properties: {
    retentionDays: 0
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_geoBackupPolicies_servers_db_name_master_Default 'Microsoft.Sql/servers/databases/geoBackupPolicies@2014-04-01' = {
  name: '${servers_db_name}/master/Default'
  properties: {
    state: 'Disabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_securityAlertPolicies_servers_db_name_master_Default 'Microsoft.Sql/servers/databases/securityAlertPolicies@2020-11-01-preview' = {
  name: '${servers_db_name}/master/Default'
  properties: {
    state: 'Disabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource servers_db_name_master_current 'Microsoft.Sql/servers/databases/transparentDataEncryption@2014-04-01' = {
  name: '${servers_db_name}/master/current'
  properties: {
    status: 'Disabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_vulnerabilityAssessments_servers_db_name_master_Default 'Microsoft.Sql/servers/databases/vulnerabilityAssessments@2020-11-01-preview' = {
  name: '${servers_db_name}/master/Default'
  properties: {
    recurringScans: {
      isEnabled: false
      emailSubscriptionAdmins: true
    }
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_devOpsAuditingSettings_servers_db_name_Default 'Microsoft.Sql/servers/devOpsAuditingSettings@2020-11-01-preview' = {
  name: '${servers_db_name_resource.name}/Default'
  properties: {
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
}

resource servers_db_name_current 'Microsoft.Sql/servers/encryptionProtector@2020-11-01-preview' = {
  name: '${servers_db_name_resource.name}/current'
  properties: {
    serverKeyName: 'ServiceManaged'
    serverKeyType: 'ServiceManaged'
    autoRotationEnabled: false
  }
}

resource Microsoft_Sql_servers_extendedAuditingSettings_servers_db_name_Default 'Microsoft.Sql/servers/extendedAuditingSettings@2020-11-01-preview' = {
  name: '${servers_db_name_resource.name}/Default'
  properties: {
    retentionDays: 0
    auditActionsAndGroups: []
    isStorageSecondaryKeyInUse: false
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
}

resource servers_db_name_AllowAllWindowsAzureIps 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  name: '${servers_db_name_resource.name}/AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource servers_db_name_ServiceManaged 'Microsoft.Sql/servers/keys@2020-11-01-preview' = {
  name: '${servers_db_name_resource.name}/ServiceManaged'
  properties: {
    serverKeyType: 'ServiceManaged'
  }
}

resource Microsoft_Sql_servers_securityAlertPolicies_servers_db_name_Default 'Microsoft.Sql/servers/securityAlertPolicies@2020-11-01-preview' = {
  name: '${servers_db_name_resource.name}/Default'
  properties: {
    state: 'Disabled'
  }
}

resource servers_db_name_tm_CreateIndex 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_name_tm_.name}/CreateIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource servers_db_name_tm_DbParameterization 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_name_tm_.name}/DbParameterization'
  properties: {
    autoExecuteValue: 'Disabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource servers_db_name_tm_DefragmentIndex 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_name_tm_.name}/DefragmentIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource servers_db_name_tm_DropIndex 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_name_tm_.name}/DropIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource servers_db_name_tm_ForceLastGoodPlan 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_name_tm_.name}/ForceLastGoodPlan'
  properties: {
    autoExecuteValue: 'Enabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/auditingPolicies@2014-04-01' = {
  name: '${servers_db_name_tm_.name}/Default'
  properties: {
    auditingState: 'Disabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_auditingSettings_servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/auditingSettings@2020-11-01-preview' = {
  name: '${servers_db_name_tm_.name}/Default'
  properties: {
    retentionDays: 0
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_backupLongTermRetentionPolicies_servers_db_name_tm_default 'Microsoft.Sql/servers/databases/backupLongTermRetentionPolicies@2020-11-01-preview' = {
  name: '${servers_db_name_tm_.name}/default'
  properties: {
    weeklyRetention: 'PT0S'
    monthlyRetention: 'PT0S'
    yearlyRetention: 'PT0S'
    weekOfYear: 0
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_backupShortTermRetentionPolicies_servers_db_name_tm_default 'Microsoft.Sql/servers/databases/backupShortTermRetentionPolicies@2020-11-01-preview' = {
  name: '${servers_db_name_tm_.name}/default'
  properties: {
    retentionDays: 7
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_extendedAuditingSettings_servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/extendedAuditingSettings@2020-11-01-preview' = {
  name: '${servers_db_name_tm_.name}/Default'
  properties: {
    retentionDays: 0
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_geoBackupPolicies_servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/geoBackupPolicies@2014-04-01' = {
  name: '${servers_db_name_tm_.name}/Default'
  properties: {
    state: 'Enabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_securityAlertPolicies_servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/securityAlertPolicies@2020-11-01-preview' = {
  name: '${servers_db_name_tm_.name}/Default'
  properties: {
    state: 'Disabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource servers_db_name_tm_current 'Microsoft.Sql/servers/databases/transparentDataEncryption@2014-04-01' = {
  name: '${servers_db_name_tm_.name}/current'
  properties: {
    status: 'Enabled'
  }
  dependsOn: [
    servers_db_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_vulnerabilityAssessments_servers_db_name_tm_Default 'Microsoft.Sql/servers/databases/vulnerabilityAssessments@2020-11-01-preview' = {
  name: '${servers_db_name_tm_.name}/Default'
  properties: {
    recurringScans: {
      isEnabled: false
      emailSubscriptionAdmins: true
    }
  }
  dependsOn: [
    servers_db_name_resource
  ]
}
