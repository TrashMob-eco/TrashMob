// Azure billing alerts and budget caps for cost management
// See Project 30 (Azure Billing Caps & Alerts) for details
//
// IMPORTANT: Azure budget APIs (Microsoft.Consumption and Microsoft.CostManagement)
// do not support Microsoft Sponsorship subscriptions (MS-AZR-0036P).
// This template deploys the action group for email notifications only.
// Budgets must be created manually in Azure Portal:
//   Azure Portal > Cost Management > Budgets > + Add
//
// Manual budget configuration:
//   1. Monthly budget: "budget-tm-{env}-{region}-monthly"
//      - Amount: $500, Reset: Monthly
//      - Alert thresholds: 50%, 75%, 90%, 100% (Actual)
//      - Action group: ag-tm-{env}-{region}-billing
//      - Contact: joe@trashmob.eco
//
//   2. Grant monitor: "budget-tm-{env}-{region}-grant"
//      - Amount: $1, Reset: Annually
//      - Alert threshold: 100% (Actual)
//      - Action group: ag-tm-{env}-{region}-billing
//      - Contact: joe@trashmob.eco

@description('Environment identifier (dev, pr)')
param environment string

@description('Azure region for deployment')
param region string

@description('Email address for billing alert notifications')
param alertEmail string = 'joe@trashmob.eco'

var actionGroupName = 'ag-tm-${environment}-${region}-billing'

// Action group for billing alert notifications
// Used by manually-created budgets in Azure Portal
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

output actionGroupId string = billingActionGroup.id
