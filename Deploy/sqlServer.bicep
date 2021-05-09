@secure()
param vulnerabilityAssessments_Default_storageContainerPath string
param servers_db_trashmob_name string = 'db-trashmob'

resource servers_db_trashmob_name_resource 'Microsoft.Sql/servers@2020-11-01-preview' = {
  name: servers_db_trashmob_name
  location: 'westus2'
  kind: 'v12.0'
  properties: {
    administratorLogin: 'dbadmin'
    version: '12.0'
    publicNetworkAccess: 'Enabled'
  }
}

resource servers_db_trashmob_name_CreateIndex 'Microsoft.Sql/servers/advisors@2014-04-01' = {
  name: '${servers_db_trashmob_name_resource.name}/CreateIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_trashmob_name_DbParameterization 'Microsoft.Sql/servers/advisors@2014-04-01' = {
  name: '${servers_db_trashmob_name_resource.name}/DbParameterization'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_trashmob_name_DefragmentIndex 'Microsoft.Sql/servers/advisors@2014-04-01' = {
  name: '${servers_db_trashmob_name_resource.name}/DefragmentIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_trashmob_name_DropIndex 'Microsoft.Sql/servers/advisors@2014-04-01' = {
  name: '${servers_db_trashmob_name_resource.name}/DropIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
}

resource servers_db_trashmob_name_ForceLastGoodPlan 'Microsoft.Sql/servers/advisors@2014-04-01' = {
  name: '${servers_db_trashmob_name_resource.name}/ForceLastGoodPlan'
  properties: {
    autoExecuteValue: 'Enabled'
  }
}

resource servers_db_trashmob_name_Default 'Microsoft.Sql/servers/auditingPolicies@2014-04-01' = {
  name: '${servers_db_trashmob_name_resource.name}/Default'
  location: 'West US 2'
  properties: {
    auditingState: 'Disabled'
  }
}

resource Microsoft_Sql_servers_auditingSettings_servers_db_trashmob_name_Default 'Microsoft.Sql/servers/auditingSettings@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/Default'
  properties: {
    retentionDays: 0
    auditActionsAndGroups: []
    isStorageSecondaryKeyInUse: false
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
}

resource servers_db_trashmob_name_ftm_dev 'Microsoft.Sql/servers/databases@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/ftm-dev'
  location: 'westus2'
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  kind: 'v12.0,user'
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

resource servers_db_trashmob_name_master_Default 'Microsoft.Sql/servers/databases/auditingPolicies@2014-04-01' = {
  name: '${servers_db_trashmob_name}/master/Default'
  location: 'West US 2'
  properties: {
    auditingState: 'Disabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_auditingSettings_servers_db_trashmob_name_master_Default 'Microsoft.Sql/servers/databases/auditingSettings@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name}/master/Default'
  properties: {
    retentionDays: 0
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_extendedAuditingSettings_servers_db_trashmob_name_master_Default 'Microsoft.Sql/servers/databases/extendedAuditingSettings@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name}/master/Default'
  properties: {
    retentionDays: 0
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_geoBackupPolicies_servers_db_trashmob_name_master_Default 'Microsoft.Sql/servers/databases/geoBackupPolicies@2014-04-01' = {
  name: '${servers_db_trashmob_name}/master/Default'
  location: 'West US 2'
  properties: {
    state: 'Disabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_securityAlertPolicies_servers_db_trashmob_name_master_Default 'Microsoft.Sql/servers/databases/securityAlertPolicies@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name}/master/Default'
  properties: {
    state: 'Disabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource servers_db_trashmob_name_master_current 'Microsoft.Sql/servers/databases/transparentDataEncryption@2014-04-01' = {
  name: '${servers_db_trashmob_name}/master/current'
  location: 'West US 2'
  properties: {
    status: 'Disabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_vulnerabilityAssessments_servers_db_trashmob_name_master_Default 'Microsoft.Sql/servers/databases/vulnerabilityAssessments@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name}/master/Default'
  properties: {
    recurringScans: {
      isEnabled: false
      emailSubscriptionAdmins: true
    }
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_devOpsAuditingSettings_servers_db_trashmob_name_Default 'Microsoft.Sql/servers/devOpsAuditingSettings@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/Default'
  properties: {
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
}

resource servers_db_trashmob_name_current 'Microsoft.Sql/servers/encryptionProtector@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/current'
  kind: 'servicemanaged'
  properties: {
    serverKeyName: 'ServiceManaged'
    serverKeyType: 'ServiceManaged'
    autoRotationEnabled: false
  }
}

resource Microsoft_Sql_servers_extendedAuditingSettings_servers_db_trashmob_name_Default 'Microsoft.Sql/servers/extendedAuditingSettings@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/Default'
  properties: {
    retentionDays: 0
    auditActionsAndGroups: []
    isStorageSecondaryKeyInUse: false
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
}

resource servers_db_trashmob_name_AllowAllWindowsAzureIps 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource servers_db_trashmob_name_ClientIPAddress_2021_4_8_17_28_19 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/ClientIPAddress_2021-4-8_17-28-19'
  properties: {
    startIpAddress: '216.9.30.100'
    endIpAddress: '216.9.30.100'
  }
}

resource servers_db_trashmob_name_Hugo_s_PC 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/Hugo\'s PC'
  properties: {
    startIpAddress: '170.250.53.64'
    endIpAddress: '170.250.53.64'
  }
}

resource servers_db_trashmob_name_Joe_PC 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/Joe PC'
  properties: {
    startIpAddress: '131.107.8.111'
    endIpAddress: '131.107.8.111'
  }
}

resource servers_db_trashmob_name_Saads_PC 'Microsoft.Sql/servers/firewallRules@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/Saads PC'
  properties: {
    startIpAddress: '67.160.105.11'
    endIpAddress: '67.160.105.11'
  }
}

resource servers_db_trashmob_name_ServiceManaged 'Microsoft.Sql/servers/keys@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/ServiceManaged'
  kind: 'servicemanaged'
  properties: {
    serverKeyType: 'ServiceManaged'
  }
}

resource Microsoft_Sql_servers_securityAlertPolicies_servers_db_trashmob_name_Default 'Microsoft.Sql/servers/securityAlertPolicies@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/Default'
  properties: {
    state: 'Disabled'
  }
}

resource Microsoft_Sql_servers_vulnerabilityAssessments_servers_db_trashmob_name_Default 'Microsoft.Sql/servers/vulnerabilityAssessments@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_resource.name}/Default'
  properties: {
    recurringScans: {
      isEnabled: false
      emailSubscriptionAdmins: true
    }
    storageContainerPath: vulnerabilityAssessments_Default_storageContainerPath
  }
}

resource servers_db_trashmob_name_ftm_dev_CreateIndex 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/CreateIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource servers_db_trashmob_name_ftm_dev_DbParameterization 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/DbParameterization'
  properties: {
    autoExecuteValue: 'Disabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource servers_db_trashmob_name_ftm_dev_DefragmentIndex 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/DefragmentIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource servers_db_trashmob_name_ftm_dev_DropIndex 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/DropIndex'
  properties: {
    autoExecuteValue: 'Disabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource servers_db_trashmob_name_ftm_dev_ForceLastGoodPlan 'Microsoft.Sql/servers/databases/advisors@2014-04-01' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/ForceLastGoodPlan'
  properties: {
    autoExecuteValue: 'Enabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource servers_db_trashmob_name_ftm_dev_Default 'Microsoft.Sql/servers/databases/auditingPolicies@2014-04-01' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/Default'
  location: 'West US 2'
  properties: {
    auditingState: 'Disabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_auditingSettings_servers_db_trashmob_name_ftm_dev_Default 'Microsoft.Sql/servers/databases/auditingSettings@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/Default'
  properties: {
    retentionDays: 0
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_backupLongTermRetentionPolicies_servers_db_trashmob_name_ftm_dev_default 'Microsoft.Sql/servers/databases/backupLongTermRetentionPolicies@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/default'
  properties: {
    weeklyRetention: 'PT0S'
    monthlyRetention: 'PT0S'
    yearlyRetention: 'PT0S'
    weekOfYear: 0
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_backupShortTermRetentionPolicies_servers_db_trashmob_name_ftm_dev_default 'Microsoft.Sql/servers/databases/backupShortTermRetentionPolicies@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/default'
  properties: {
    retentionDays: 7
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_extendedAuditingSettings_servers_db_trashmob_name_ftm_dev_Default 'Microsoft.Sql/servers/databases/extendedAuditingSettings@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/Default'
  properties: {
    retentionDays: 0
    isAzureMonitorTargetEnabled: false
    state: 'Disabled'
    storageAccountSubscriptionId: '00000000-0000-0000-0000-000000000000'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_geoBackupPolicies_servers_db_trashmob_name_ftm_dev_Default 'Microsoft.Sql/servers/databases/geoBackupPolicies@2014-04-01' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/Default'
  location: 'West US 2'
  properties: {
    state: 'Enabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_securityAlertPolicies_servers_db_trashmob_name_ftm_dev_Default 'Microsoft.Sql/servers/databases/securityAlertPolicies@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/Default'
  properties: {
    state: 'Disabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource servers_db_trashmob_name_ftm_dev_current 'Microsoft.Sql/servers/databases/transparentDataEncryption@2014-04-01' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/current'
  location: 'West US 2'
  properties: {
    status: 'Enabled'
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}

resource Microsoft_Sql_servers_databases_vulnerabilityAssessments_servers_db_trashmob_name_ftm_dev_Default 'Microsoft.Sql/servers/databases/vulnerabilityAssessments@2020-11-01-preview' = {
  name: '${servers_db_trashmob_name_ftm_dev.name}/Default'
  properties: {
    recurringScans: {
      isEnabled: false
      emailSubscriptionAdmins: true
    }
  }
  dependsOn: [
    servers_db_trashmob_name_resource
  ]
}