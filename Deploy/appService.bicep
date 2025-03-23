param region string
param subscriptionId string
param rgName string
param appServicePlanName string
param appServiceName string
param alwaysOn bool = true

var serverfarms_tmplan_externalid = '/subscriptions/${subscriptionId}/resourceGroups/${rgName}/providers/Microsoft.Web/serverfarms/${appServicePlanName}'

resource sites_tm_name_resource 'Microsoft.Web/sites@2018-11-01' = {
  name: appServiceName
  location: region
  tags: {
    'hidden-related:/subscriptions/${subscriptionId}/resourceGroups/${rgName}/providers/Microsoft.Web/serverfarms/${appServicePlanName}': 'empty'
  }
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: '${appServiceName}.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: '${appServiceName}.scm.azurewebsites.net'
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

resource sites_tm_name_web 'Microsoft.Web/sites/config@2024-04-01' = {
  parent: sites_tm_name_resource
  name: 'web'
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
    alwaysOn: alwaysOn
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
  parent: sites_tm_name_resource
  name: '${appServiceName}.azurewebsites.net'
  properties: {
    siteName: appServiceName
    hostNameType: 'Verified'
  }
}

resource sites_tm_name_Microsoft_AspNetCore_AzureAppServices_SiteExtension 'Microsoft.Web/sites/siteextensions@2018-11-01' = {
  parent: sites_tm_name_resource
  name: 'Microsoft.AspNetCore.AzureAppServices.SiteExtension'
}
