param sites_TrashMobDev_name string = 'TrashMobDev'
param serverfarms_TrashMobDevPlan_externalid string = '/subscriptions/39a254b7-c01a-45ab-bebd-4038ea4adea9/resourceGroups/TrashMob/providers/Microsoft.Web/serverfarms/TrashMobDevPlan'

resource sites_TrashMobDev_name_resource 'Microsoft.Web/sites@2018-11-01' = {
  name: sites_TrashMobDev_name
  location: 'West US 2'
  tags: {
    'hidden-related:/subscriptions/39a254b7-c01a-45ab-bebd-4038ea4adea9/resourceGroups/TrashMob/providers/Microsoft.Web/serverfarms/TrashMobDevPlan': 'empty'
  }
  kind: 'app'
  identity: {
    principalId: '7315d8f3-ecd4-4409-b800-ef779d71f58b'
    tenantId: 'f0062da2-b273-427c-b99e-6e85f75b23eb'
    type: 'SystemAssigned'
  }
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: 'trashmobdev.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: 'trashmobdev.scm.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    serverFarmId: serverfarms_TrashMobDevPlan_externalid
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

resource sites_TrashMobDev_name_web 'Microsoft.Web/sites/config@2018-11-01' = {
  name: '${sites_TrashMobDev_name_resource.name}/web'
  location: 'West US 2'
  tags: {
    'hidden-related:/subscriptions/39a254b7-c01a-45ab-bebd-4038ea4adea9/resourceGroups/TrashMob/providers/Microsoft.Web/serverfarms/TrashMobDevPlan': 'empty'
  }
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
    publishingUsername: '$TrashMobDev'
    azureStorageAccounts: {}
    scmType: 'GitHubAction'
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

resource sites_TrashMobDev_name_7e8af4dc3a844019b044d003947ee57d 'Microsoft.Web/sites/deployments@2018-11-01' = {
  name: '${sites_TrashMobDev_name_resource.name}/7e8af4dc3a844019b044d003947ee57d'
  location: 'West US 2'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY'
    message: '{"type":"deployment","sha":"fde34cad56d0d5e9b1c3a4bc940bdb8c93c296db","repoName":"joebeernink/FlashTrashMob","slotName":"production"}'
    start_time: '5/8/2021 9:02:16 PM'
    end_time: '5/8/2021 9:02:22 PM'
    active: true
  }
}

resource sites_TrashMobDev_name_f952d3ddde4b4ba5a79781326c0030db 'Microsoft.Web/sites/deployments@2018-11-01' = {
  name: '${sites_TrashMobDev_name_resource.name}/f952d3ddde4b4ba5a79781326c0030db'
  location: 'West US 2'
  properties: {
    status: 4
    author_email: 'N/A'
    author: 'N/A'
    deployer: 'GITHUB_ZIP_DEPLOY'
    message: '{"type":"deployment","sha":"dbda1504c72c15a9246df01924e641fc15d49010","repoName":"joebeernink/FlashTrashMob","slotName":"production"}'
    start_time: '5/8/2021 6:12:35 PM'
    end_time: '5/8/2021 6:12:43 PM'
    active: false
  }
}

resource sites_TrashMobDev_name_sites_TrashMobDev_name_azurewebsites_net 'Microsoft.Web/sites/hostNameBindings@2018-11-01' = {
  name: '${sites_TrashMobDev_name_resource.name}/${sites_TrashMobDev_name}.azurewebsites.net'
  location: 'West US 2'
  properties: {
    siteName: 'TrashMobDev'
    hostNameType: 'Verified'
  }
}

resource sites_TrashMobDev_name_Microsoft_AspNetCore_AzureAppServices_SiteExtension 'Microsoft.Web/sites/siteextensions@2018-11-01' = {
  name: '${sites_TrashMobDev_name_resource.name}/Microsoft.AspNetCore.AzureAppServices.SiteExtension'
  location: 'West US 2'
}