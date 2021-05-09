param environment string
param region string
param subscription string
param rgName string

var sites_tm_name = 'as_tm_${environment}-${region}'
var serverfarms_tmplan_externalid = '/subscriptions/${subscription}/resourceGroups/${rgName}/providers/Microsoft.Web/serverfarms/asp_tm_${environment}-${region}'

resource sites_tm_name_resource 'Microsoft.Web/sites@2018-11-01' = {
  name: sites_tm_name
  location: '${region}'
  tags: {
    'hidden-related:/subscriptions/39a254b7-c01a-45ab-bebd-4038ea4adea9/resourceGroups/TrashMob/providers/Microsoft.Web/serverfarms/TrashMobDevPlan': 'empty'
  }
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: '${sites_tm_name}.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: '${sites_tm_name}.scm.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    serverFarmId: serverfarms_tmplan_externalid
    reserved: false
    isXenon: false
    hyperV: false
    siteConfig: {}
    scmSiteAlsoStopped: false
    clientAffinityEnabled: true
    clientCertEnabled: false
    hostNamesDisabled: false
    containerSize: 0
    dailyMemoryTimeQuota: 0
    httpsOnly: true
    redundancyMode: 'None'
  }
}

resource sites_tm_name_web 'Microsoft.Web/sites/config@2018-11-01' = {
  name: '${sites_tm_name_resource.name}/web'
  properties: {
    numberOfWorkers: 1
    defaultDocuments: [
      'Default.htm'
      'Default.html'
      'Default.asp'
      'index.htm'
      'index.html'
      'iisstart.htm'
      'default.aspx'
      'index.php'
      'hostingstart.html'
    ]
    netFrameworkVersion: 'v5.0'
    phpVersion: '5.6'
    requestTracingEnabled: false
    remoteDebuggingEnabled: false
    httpLoggingEnabled: false
    logsDirectorySizeLimit: 35
    detailedErrorLoggingEnabled: false
    azureStorageAccounts: {}
    use32BitWorkerProcess: true
    webSocketsEnabled: false
    alwaysOn: false
    managedPipelineMode: 'Integrated'
    virtualApplications: [
      {
        virtualPath: '/'
        physicalPath: 'site\\wwwroot'
        preloadEnabled: false
      }
    ]
    loadBalancing: 'LeastRequests'
    experiments: {
      rampUpRules: []
    }
    autoHealEnabled: false
    localMySqlEnabled: false
    managedServiceIdentityId: 7519
    ipSecurityRestrictions: [
      {
        ipAddress: 'Any'
        action: 'Allow'
        priority: 1
        name: 'Allow all'
        description: 'Allow all access'
      }
    ]
    scmIpSecurityRestrictions: [
      {
        ipAddress: 'Any'
        action: 'Allow'
        priority: 1
        name: 'Allow all'
        description: 'Allow all access'
      }
    ]
    scmIpSecurityRestrictionsUseMain: false
    http20Enabled: false
    minTlsVersion: '1.2'
    ftpsState: 'AllAllowed'
    reservedInstanceCount: 0
  }
}

resource sites_tm_name_sites_tm_name_azurewebsites_net 'Microsoft.Web/sites/hostNameBindings@2018-11-01' = {
  name: '${sites_tm_name_resource.name}/${sites_tm_name}.azurewebsites.net'
  properties: {
    siteName: '${sites_tm_name}'
    hostNameType: 'Verified'
  }
}

resource sites_tm_name_Microsoft_AspNetCore_AzureAppServices_SiteExtension 'Microsoft.Web/sites/siteextensions@2018-11-01' = {
  name: '${sites_tm_name_resource.name}/Microsoft.AspNetCore.AzureAppServices.SiteExtension'
}
