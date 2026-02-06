// Azure billing alerts and budget caps for cost management
// See Project 30 (Azure Billing Caps & Alerts) for details

@description('Environment identifier (dev, pr)')
param environment string

@description('Azure region for deployment')
param region string

@description('Email address for billing alert notifications')
param alertEmail string = 'joe@trashmob.eco'

@description('Monthly budget amount in USD')
param monthlyBudgetAmount int = 500

@description('Annual grant monitor threshold in USD - alerts if actual spend exceeds this amount')
param grantBudgetAmount int = 1

var actionGroupName = 'ag-tm-${environment}-${region}-billing'
var monthlyBudgetName = 'budget-tm-${environment}-${region}-monthly'
var grantMonitorBudgetName = 'budget-tm-${environment}-${region}-grant'

// Action group for billing alert notifications
resource billingActionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: actionGroupName
  location: 'global'
  properties: {
    groupShortName: 'TMBilling'
    enabled: true
    emailReceivers: [
      {
        name: 'BillingAlertEmail'
        emailAddress: alertEmail
        useCommonAlertSchema: true
      }
    ]
  }
}

// Monthly budget with alerts at 50%, 75%, 90%, and 100% thresholds
resource monthlyBudget 'Microsoft.Consumption/budgets@2023-11-01' = {
  name: monthlyBudgetName
  properties: {
    category: 'Cost'
    amount: monthlyBudgetAmount
    timeGrain: 'Monthly'
    timePeriod: {
      startDate: '2026-02-01'
      endDate: '2027-01-31'
    }
    notifications: {
      Actual_GreaterThan_50_Percent: {
        enabled: true
        operator: 'GreaterThan'
        threshold: 50
        thresholdType: 'Actual'
        contactEmails: [
          alertEmail
        ]
        contactGroups: [
          billingActionGroup.id
        ]
      }
      Actual_GreaterThan_75_Percent: {
        enabled: true
        operator: 'GreaterThan'
        threshold: 75
        thresholdType: 'Actual'
        contactEmails: [
          alertEmail
        ]
        contactGroups: [
          billingActionGroup.id
        ]
      }
      Actual_GreaterThan_90_Percent: {
        enabled: true
        operator: 'GreaterThan'
        threshold: 90
        thresholdType: 'Actual'
        contactEmails: [
          alertEmail
        ]
        contactGroups: [
          billingActionGroup.id
        ]
      }
      Actual_GreaterThan_100_Percent: {
        enabled: true
        operator: 'GreaterThan'
        threshold: 100
        thresholdType: 'Actual'
        contactEmails: [
          alertEmail
        ]
        contactGroups: [
          billingActionGroup.id
        ]
      }
    }
  }
}

// Annual $1 budget to detect Microsoft nonprofit grant expiration
// Any charge above $1/year likely means the grant has expired or is not being applied
resource grantMonitorBudget 'Microsoft.Consumption/budgets@2023-11-01' = {
  name: grantMonitorBudgetName
  properties: {
    category: 'Cost'
    amount: grantBudgetAmount
    timeGrain: 'Annually'
    timePeriod: {
      startDate: '2026-02-01'
      endDate: '2027-01-31'
    }
    notifications: {
      Grant_May_Have_Expired: {
        enabled: true
        operator: 'GreaterThan'
        threshold: 100
        thresholdType: 'Actual'
        contactEmails: [
          alertEmail
        ]
        contactGroups: [
          billingActionGroup.id
        ]
      }
    }
  }
}

output actionGroupId string = billingActionGroup.id
output monthlyBudgetId string = monthlyBudget.id
output grantMonitorBudgetId string = grantMonitorBudget.id
