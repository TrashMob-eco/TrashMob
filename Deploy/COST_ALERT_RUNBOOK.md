# Cost Alert Runbook

This runbook covers how to respond to billing alerts and perform monthly cost reviews for TrashMob's Azure infrastructure.

## Tenant Architecture

TrashMob uses two Azure tenants:

| Tenant | ID | Purpose |
|--------|----|---------|
| **Resource tenant** | *(default)* | Contains all resource groups, Container Apps, SQL, etc. |
| **Billing tenant** | `c0459473-d4ce-4472-b23c-dd1218a33a7b` | Manages the Microsoft nonprofit grant and billing |

**Important:** All budget alerts and cost reviews must be performed in the **billing tenant**, not the resource tenant. The billing tenant is where actual charges and grant credits are tracked.

### Switching tenants

```bash
# Switch to billing tenant for cost management
az login --tenant c0459473-d4ce-4472-b23c-dd1218a33a7b

# Switch back to resource tenant for infrastructure work
az login  # defaults to resource tenant
```

In the Azure Portal, use the tenant switcher (top-right gear icon > Directories + subscriptions) to switch to the billing tenant.

---

## Initial Setup (Manual — One-Time)

Azure budget APIs do not support Microsoft Sponsorship subscriptions (MS-AZR-0036P). Budgets must be created manually in the Azure Portal **within the billing tenant**. Email contacts are used directly on budget alerts (no action group needed in the billing tenant).

The action group deployed via `Deploy/billingAlerts.bicep` remains in the resource tenant for resource-level metric alerts only.

### Create Monthly Budget

1. **Switch to billing tenant** (`c0459473-d4ce-4472-b23c-dd1218a33a7b`) in Azure Portal
2. Cost Management > Budgets > **+ Add**
3. Scope: select the sponsorship subscription
4. Name: `budget-tm-monthly`
5. Reset period: **Monthly**
6. Amount: **$500**
7. Add alert conditions:
   - 50% of budget (Actual) — email: joe@trashmob.eco
   - 75% of budget (Actual) — email: joe@trashmob.eco
   - 90% of budget (Actual) — email: joe@trashmob.eco
   - 100% of budget (Actual) — email: joe@trashmob.eco
8. Click **Create**

### Create Grant Expiration Monitor

1. **Switch to billing tenant** (`c0459473-d4ce-4472-b23c-dd1218a33a7b`) in Azure Portal
2. Cost Management > Budgets > **+ Add**
3. Scope: select the sponsorship subscription
4. Name: `budget-tm-grant-monitor`
5. Reset period: **Annually**
6. Amount: **$1**
7. Add alert condition:
   - 100% of budget (Actual) — email: joe@trashmob.eco
8. Click **Create**

---

## Alert Types

| Alert | Trigger | Urgency | Meaning |
|-------|---------|---------|---------|
| **Monthly 50%** | Actual spend exceeds 50% of monthly budget | Low | Informational mid-month checkpoint |
| **Monthly 75%** | Actual spend exceeds 75% of monthly budget | Medium | Spending is elevated; review resource usage |
| **Monthly 90%** | Actual spend exceeds 90% of monthly budget | High | Approaching budget limit; take action |
| **Monthly 100%** | Actual spend exceeds 100% of monthly budget ($500) | Critical | Budget exceeded; investigate immediately |
| **Grant Expired** | Actual annual spend exceeds $1 | Critical | Microsoft nonprofit grant may have expired or is not being applied |

## Response Procedures

### Low (50% threshold)

1. Note the alert — no immediate action required
2. Check if spending is on track for the month (e.g., 50% alert at mid-month is normal)
3. Review Azure Cost Analysis (in billing tenant) to confirm no unusual resource consumption

### Medium (75% threshold)

1. **Switch to billing tenant** in Azure Portal
2. Open Cost Management + Billing > Cost Analysis
3. Filter by resource group (`rg-trashmob-pr-westus2` or `rg-trashmob-dev-westus2`)
4. Identify which resources are driving the increase
5. Check for: unexpected scaling, orphaned resources, or misconfigured services
6. If spending is expected (e.g., seasonal traffic spike), document and monitor

### High (90% threshold)

1. Follow the Medium steps above
2. Switch to **resource tenant** and check Container App replica counts:
   ```bash
   az containerapp revision list --name ca-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2 --output table
   ```
3. Check SQL Database DTU usage:
   ```bash
   az monitor metrics list --resource /subscriptions/<sub-id>/resourceGroups/rg-trashmob-pr-westus2/providers/Microsoft.Sql/servers/sql-tm-pr-westus2/databases/db-tm-pr-westus2 --metric dtu_consumption_percent --interval PT1H
   ```
4. Consider reducing non-critical resources (e.g., scale down dev environment)

### Critical (100% threshold — budget exceeded)

1. **Immediately** investigate the cause
2. Switch to **billing tenant** and check if the Microsoft nonprofit grant is still active:
   - Cost Management + Billing > Billing scopes
   - Verify the sponsorship/grant is listed and active
3. If grant is active but costs are high:
   - Identify the top cost drivers in Cost Analysis
   - Scale down or stop non-essential resources (in resource tenant)
   - Consider pausing the dev environment if only prod is needed
4. If grant appears expired — see Grant Expiration procedure below

### Critical (Grant Expiration — $1/year threshold)

This alert indicates real charges are being incurred, which should not happen under the Microsoft nonprofit grant.

1. **Immediately** check grant status in **billing tenant**:
   - Azure Portal > Cost Management + Billing > Billing scopes
   - Look for the Microsoft sponsorship/grant subscription
2. If grant has expired:
   - Contact Microsoft nonprofit support to request renewal
   - Scale down all non-production resources to minimize charges (in resource tenant)
   - Notify the project owner (joe@trashmob.eco)
3. If grant is active but charges appear:
   - Check if any resources are in a subscription NOT covered by the grant
   - Verify all resource groups are under the correct subscription

## Monthly Cost Review Checklist

Perform this review on the 1st of each month.

- [ ] Switch to **billing tenant** (`c0459473-d4ce-4472-b23c-dd1218a33a7b`)
- [ ] Open Azure Portal > Cost Management > Cost Analysis
- [ ] Review previous month's total spend by resource group
- [ ] Compare to previous months — flag any >20% increase
- [ ] Switch to **resource tenant** for infrastructure checks:
  - [ ] Check for orphaned resources (disks, IPs, storage accounts not attached to anything)
  - [ ] Review Container App scaling history — are replicas appropriate?
- [ ] Check SendGrid usage (see below)
- [ ] Check Google Maps API usage (see below)
- [ ] Document findings and any actions taken

### Where to check costs

```bash
# Switch to billing tenant first
az login --tenant c0459473-d4ce-4472-b23c-dd1218a33a7b

# Quick cost summary for last 30 days (production)
az consumption usage list --start-date $(date -d '-30 days' +%Y-%m-%d) --end-date $(date +%Y-%m-%d) --query "[?contains(instanceName, 'tm-pr')].{Resource:instanceName, Cost:pretaxCost, Currency:currency}" --output table

# Quick cost summary (development)
az consumption usage list --start-date $(date -d '-30 days' +%Y-%m-%d) --end-date $(date +%Y-%m-%d) --query "[?contains(instanceName, 'tm-dev')].{Resource:instanceName, Cost:pretaxCost, Currency:currency}" --output table

# Switch back to resource tenant when done
az login
```

## Third-Party Service Alerts

### SendGrid (Manual Setup)

SendGrid billing alerts must be configured in the SendGrid dashboard.

1. Log into https://app.sendgrid.com
2. Settings > Account Details > Billing
3. Under "Billing Alerts", click "Add Alert"
4. Set alert at 75% of monthly email limit
5. Set alert at 90% of monthly email limit
6. Add recipient: joe@trashmob.eco
7. For daily sending limit: Settings > Mail Settings > Set daily sending limit

### Google Maps Platform (Manual Setup)

Google Maps API billing alerts must be configured in Google Cloud Console.

1. Log into https://console.cloud.google.com
2. Billing > Budgets & alerts
3. Click "Create Budget"
4. Name: "TrashMob Maps API", Amount: $100/month
5. Set thresholds: 50%, 90%, 100%
6. Enable email notifications
7. For request limits: APIs & Services > Quotas > Maps API > Edit Quota > Set daily request limit

## Escalation

| Severity | Response Time | Contact |
|----------|---------------|---------|
| Low (50%) | Next business day | Engineering lead reviews at convenience |
| Medium (75%) | Same business day | Engineering lead investigates |
| High (90%) | Within 4 hours | Engineering lead takes action |
| Critical (100% or grant) | Within 1 hour | Engineering lead + project owner notified |

## Related Resources

- **Bicep template:** `Deploy/billingAlerts.bicep` (action group in resource tenant only)
- **Project spec:** `Planning/Projects/Project_30_Azure_Billing_Alerts.md`
- **Risk register:** `Planning/Risks_and_Mitigations.md` (SR-5)
- **Billing tenant:** `c0459473-d4ce-4472-b23c-dd1218a33a7b`
