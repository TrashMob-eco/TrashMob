@description('Environment identifier (dev, pr)')
param environment string

@description('Azure region for deployment')
param region string

@description('Resource group name')
param rgName string

@description('Application Insights resource name')
param appInsightsName string

@description('Email address for weekly reports')
param reportEmailAddress string = ''

// Reference existing Application Insights resource
resource appInsights 'microsoft.insights/components@2020-02-02' existing = {
  name: appInsightsName
}

// Action group for feature metrics reports
resource featureMetricsActionGroup 'microsoft.insights/actionGroups@2023-01-01' = if (!empty(reportEmailAddress)) {
  name: 'ag-feature-metrics-${environment}'
  location: 'global'
  properties: {
    groupShortName: 'FeatureRpts'
    enabled: true
    emailReceivers: [
      {
        name: 'FeatureMetricsReport'
        emailAddress: reportEmailAddress
        useCommonAlertSchema: true
      }
    ]
  }
}

// Weekly feature usage summary alert (fires every Monday)
resource weeklyFeatureUsageAlert 'microsoft.insights/scheduledqueryrules@2023-03-15-preview' = if (!empty(reportEmailAddress)) {
  name: 'sqr-weekly-feature-usage-${environment}'
  location: region
  properties: {
    displayName: 'Weekly Feature Usage Summary'
    description: 'Weekly summary of TrashMob feature usage metrics'
    enabled: true
    evaluationFrequency: 'P1D'
    scopes: [appInsights.id]
    severity: 4  // Informational
    windowSize: 'P7D'
    criteria: {
      allOf: [
        {
          query: '''
            customEvents
            | where timestamp >= ago(7d)
            | where name startswith "Auth_" or name startswith "Event_" or name startswith "Attendance_" or name startswith "LitterReport_"
            | summarize
                Logins = countif(name == "Auth_Login_Success"),
                EventsCreated = countif(name == "Event_Create_Submit"),
                Registrations = countif(name == "Attendance_Register_Submit"),
                LitterReports = countif(name == "LitterReport_Create_Submit")
            | where Logins > 0 or EventsCreated > 0 or Registrations > 0 or LitterReports > 0
          '''
          timeAggregation: 'Count'
          dimensions: []
          operator: 'GreaterThan'
          threshold: 0
          failingPeriods: {
            numberOfEvaluationPeriods: 1
            minFailingPeriodsToAlert: 1
          }
        }
      ]
    }
    autoMitigate: false
    actions: {
      actionGroups: [featureMetricsActionGroup.id]
      customProperties: {
        reportType: 'Weekly Feature Usage'
        environment: environment
      }
    }
  }
}

// Low activity alert - fires if no events created in 7 days
resource lowActivityAlert 'microsoft.insights/scheduledqueryrules@2023-03-15-preview' = if (!empty(reportEmailAddress)) {
  name: 'sqr-low-activity-${environment}'
  location: region
  properties: {
    displayName: 'Low Feature Activity Alert'
    description: 'Alert when no cleanup events have been created in the past 7 days'
    enabled: true
    evaluationFrequency: 'P1D'
    scopes: [appInsights.id]
    severity: 2  // Warning
    windowSize: 'P7D'
    criteria: {
      allOf: [
        {
          query: '''
            customEvents
            | where timestamp >= ago(7d)
            | where name == "Event_Create_Submit"
            | summarize EventsCreated = count()
            | where EventsCreated == 0
          '''
          timeAggregation: 'Count'
          dimensions: []
          operator: 'GreaterThan'
          threshold: 0
          failingPeriods: {
            numberOfEvaluationPeriods: 1
            minFailingPeriodsToAlert: 1
          }
        }
      ]
    }
    autoMitigate: true
    actions: {
      actionGroups: [featureMetricsActionGroup.id]
      customProperties: {
        alertType: 'Low Activity Warning'
        environment: environment
      }
    }
  }
}

output actionGroupId string = !empty(reportEmailAddress) ? featureMetricsActionGroup.id : ''
output weeklyAlertId string = !empty(reportEmailAddress) ? weeklyFeatureUsageAlert.id : ''
