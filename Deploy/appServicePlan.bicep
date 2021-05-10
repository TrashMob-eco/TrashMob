param environment string
param region string

var serverfarms_tmplan_name = 'asp-tm-${environment}-${region}'

resource serverfarms_tmplan_name_resource 'Microsoft.Web/serverfarms@2018-02-01' = {
  name: serverfarms_tmplan_name
  location: '${region}'
  sku: {
    name: 'D1'
    tier: 'Shared'
    size: 'D1'
    family: 'D'
    capacity: 0
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
