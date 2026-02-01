@description('Environment identifier (dev, pr)')
param environment string

@description('Azure region for deployment')
param region string

@description('Resource group name')
param rgName string

@description('Application Insights resource name')
param appInsightsName string

// Reference existing Application Insights resource
resource appInsights 'microsoft.insights/components@2020-02-02' existing = {
  name: appInsightsName
}

// Feature Metrics Workbook
resource featureMetricsWorkbook 'microsoft.insights/workbooks@2022-04-01' = {
  name: guid('feature-metrics-workbook', rgName, environment)
  location: region
  kind: 'shared'
  properties: {
    displayName: 'TrashMob Feature Usage Metrics'
    category: 'workbook'
    sourceId: appInsights.id
    serializedData: loadTextContent('workbooks/feature-metrics-workbook.json')
  }
  tags: {
    'hidden-title': 'TrashMob Feature Usage Metrics'
    environment: environment
  }
}

output workbookId string = featureMetricsWorkbook.id
output workbookName string = featureMetricsWorkbook.name
