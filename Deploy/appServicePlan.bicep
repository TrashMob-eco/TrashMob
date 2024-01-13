param region string
param appServicePlanName string

resource serverfarms_tmplan_name_resource 'Microsoft.Web/serverfarms@2018-02-01' = {
  name: appServicePlanName
  location: region
  sku: {
    name: 'S1'
  }
  kind: 'app'
  properties: {
    perSiteScaling: false
    maximumElasticWorkerCount: 1
    isSpot: false
    reserved: false
    isXenon: false
    hyperV: false
    targetWorkerCount: 0
    targetWorkerSizeId: 0
  }
}
