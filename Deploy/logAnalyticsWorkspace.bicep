param environment string
param region string

var workspaces_law_tm_name = 'law-tm-${environment}-${region}'

resource workspaces_law_tm_name_resource 'microsoft.operationalinsights/workspaces@2020-10-01' = {
  name: workspaces_law_tm_name
  location: '${region}'
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    features: {
      legacy: 0
      searchVersion: 1
      enableLogAccessUsingOnlyResourcePermissions: true
    }
    workspaceCapping: {
      dailyQuotaGb: -1
    }
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}
