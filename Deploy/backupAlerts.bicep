// Backup monitoring alerts for Azure SQL Database
// See Project 32 (Database Backups) for details

param environment string
param region string
param alertEmail string = 'info@trashmob.eco'

var actionGroupName = 'ag-tm-${environment}-${region}-backup'
var alertRuleName = 'alert-tm-${environment}-${region}-backup-health'
var servers_db_name = 'sql-tm-${environment}-${region}'
var db_Name = 'db-tm-${environment}-${region}'

// Reference existing SQL database
resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' existing = {
  name: servers_db_name
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-05-01-preview' existing = {
  parent: sqlServer
  name: db_Name
}

// Action group for backup alert notifications
resource actionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: actionGroupName
  location: 'global'
  properties: {
    groupShortName: 'TMBackup'
    enabled: true
    emailReceivers: [
      {
        name: 'BackupAlertEmail'
        emailAddress: alertEmail
        useCommonAlertSchema: true
      }
    ]
  }
}

// Alert rule for database availability/health
// This monitors the database connection success rate which correlates with backup health
resource databaseHealthAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: alertRuleName
  location: 'global'
  properties: {
    description: 'Alert when database connection failures indicate potential backup issues'
    severity: 1
    enabled: true
    scopes: [
      sqlDatabase.id
    ]
    evaluationFrequency: 'PT15M'
    windowSize: 'PT1H'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'ConnectionFailure'
          metricName: 'connection_failed'
          metricNamespace: 'Microsoft.Sql/servers/databases'
          operator: 'GreaterThan'
          threshold: 5
          timeAggregation: 'Total'
          criterionType: 'StaticThresholdCriterion'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
  }
}

// Activity log alert for backup-related service health events
// Note: Service health alerts require subscription scope
resource serviceHealthAlert 'Microsoft.Insights/activityLogAlerts@2020-10-01' = {
  name: 'alert-tm-${environment}-${region}-sql-health'
  location: 'global'
  properties: {
    description: 'Alert for Azure SQL service health issues that may affect backups'
    enabled: true
    scopes: [
      subscription().id
    ]
    condition: {
      allOf: [
        {
          field: 'category'
          equals: 'ServiceHealth'
        }
        {
          field: 'properties.impactedServices[*].ServiceName'
          containsAny: [
            'SQL Database'
            'Azure SQL Database'
          ]
        }
        {
          field: 'properties.impactedServices[*].ImpactedRegions[*].RegionName'
          containsAny: [
            region
            'Global'
          ]
        }
      ]
    }
    actions: {
      actionGroups: [
        {
          actionGroupId: actionGroup.id
        }
      ]
    }
  }
}

output actionGroupId string = actionGroup.id
output alertRuleId string = databaseHealthAlert.id
