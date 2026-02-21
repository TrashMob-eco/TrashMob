// Azure billing alerts — action group for resource-level metric alerts
// See Project 30 (Azure Billing Caps & Alerts) for details
//
// TENANT ARCHITECTURE:
//   - Resource tenant (this deployment): Contains Container Apps, SQL, etc.
//   - Billing tenant (c0459473-d4ce-4472-b23c-dd1218a33a7b): Manages nonprofit grant and billing
//
// This template deploys an action group in the RESOURCE tenant for resource-level
// metric alerts (e.g., DTU usage, Container App scaling). It does NOT create budget
// alerts — those must be created manually in the BILLING tenant.
//
// IMPORTANT: Azure budget APIs (Microsoft.Consumption and Microsoft.CostManagement)
// do not support Microsoft Sponsorship subscriptions (MS-AZR-0036P).
// Budget alerts must be created manually in the billing tenant's Azure Portal.
// Use direct email contacts on budget alerts (no action group needed in billing tenant).
//
// See Deploy/COST_ALERT_RUNBOOK.md for full setup instructions.

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
