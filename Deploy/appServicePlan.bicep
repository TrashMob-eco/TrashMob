param serverfarms_TrashMobDevPlan_name string = 'TrashMobDevPlan'

resource serverfarms_TrashMobDevPlan_name_resource 'Microsoft.Web/serverfarms@2018-02-01' = {
  name: serverfarms_TrashMobDevPlan_name
  location: 'West US 2'
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