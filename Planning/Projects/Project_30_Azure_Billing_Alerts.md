# Project 30 — Azure Billing Caps & Alerts

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Small |
| **Dependencies** | None |

---

## Business Rationale

As a nonprofit, TrashMob must carefully manage cloud costs. Unexpected spikes in Azure, Maps API, or SendGrid usage could strain the budget and divert funds from mission-critical activities. Proactive billing alerts and spending caps provide early warning and automatic protection against runaway costs.

**Critical Context:** TrashMob receives an annual Microsoft nonprofit grant that covers Azure costs. This grant often hides actual costs in billing. **An alert at $1/year is essential** - any charge above this threshold may indicate the grant has expired or is not being applied correctly.

**Tenant Note:** Billing is managed in a separate Azure tenant (`c0459473-d4ce-4472-b23c-dd1218a33a7b`), not the resource tenant where Container Apps and databases run. All budget alerts and cost reviews must be created/performed in the billing tenant. See `Deploy/COST_ALERT_RUNBOOK.md` for detailed instructions.

This project directly mitigates **SR-5 (Cost Spikes)** identified in the Risks & Mitigations document.

---

## Objectives

### Primary Goals
- Set up billing alerts at multiple thresholds to provide early warning of cost increases
- Configure spending caps where supported to prevent budget overruns
- Establish a monthly cost review process with clear ownership

### Secondary Goals (Nice-to-Have)
- Create a cost dashboard for at-a-glance monitoring
- Document cost optimization opportunities identified during setup

---

## Scope

### Phase 1 - Azure Cost Management Setup
- ✅ Configure Azure Cost Management budgets for each environment (dev, prod)
- ✅ Set up alert thresholds at 50%, 75%, 90%, and 100% of monthly budget
- ✅ Configure alert recipients (engineering lead, finance)
- ✅ Enable cost anomaly detection alerts
- ✅ **Critical:** Set $1/year threshold alert to detect Microsoft nonprofit grant expiration
- ✅ **Critical:** Configure $500 hard spending cap to automatically stop resources if exceeded

### Phase 2 - Third-Party Service Alerts
- ✅ Configure SendGrid usage alerts and spending limits
- ✅ Set up Google Maps API billing alerts in Google Cloud Console
- ✅ Document current baseline usage for each service

### Phase 3 - Process & Documentation
- ✅ Document monthly cost review process
- ✅ Create runbook for responding to cost alerts
- ✅ Add cost review to monthly operations checklist

---

## Out-of-Scope

- ❌ Architectural changes to reduce costs (separate optimization projects)
- ❌ Migration to alternative services
- ❌ Automated cost remediation (shutting down services)

---

## Success Metrics

### Quantitative
- **Alert coverage:** 100% of billable services have alerts configured
- **Response time:** Cost anomalies detected within 24 hours
- **Budget accuracy:** Monthly spend within 10% of forecast

### Qualitative
- Engineering team has confidence in cost visibility
- No surprise bills at end of month

---

## Dependencies

### Blockers (Must be complete before this project starts)
- **Azure subscription access:** Contributor or Cost Management Contributor role required

### Enablers for Other Projects (What this unlocks)
- **All projects:** Provides cost guardrails for new feature development
- **SR-5 mitigation:** Directly addresses cost spike risk

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Alert fatigue from too many notifications** | Medium | Low | Start with conservative thresholds, tune based on experience |
| **Spending caps cause service disruption** | Low | High | Use alerts first; only enable hard caps on non-critical resources |
| **Incomplete service coverage** | Low | Medium | Audit all Azure resource groups and third-party integrations |

---

## Implementation Plan

**Principle:** Use vendor APIs for configuration where available. API-based setup is preferred over manual portal configuration to ensure consistency and repeatability. Where APIs are not available, explicit step-by-step manual instructions are documented.

### Azure Cost Management (Manual — Billing Tenant)

Budget APIs (`Microsoft.Consumption`) do not support Microsoft Sponsorship subscriptions (MS-AZR-0036P). Budgets must be created manually in the Azure Portal **within the billing tenant** (`c0459473-d4ce-4472-b23c-dd1218a33a7b`). Email contacts are used directly on budget alerts — no action group is needed in the billing tenant.

**Manual Portal Steps (in billing tenant)**
1. Switch to billing tenant in Azure Portal (gear icon > Directories + subscriptions)
2. Cost Management > Budgets > **+ Add**
3. Scope: select the sponsorship subscription
4. Create monthly budget: `budget-tm-monthly`, Amount: $500, Reset: Monthly
5. Add alert conditions: 50%, 75%, 90%, 100% (Actual), email: joe@trashmob.eco
6. Create grant monitor: `budget-tm-grant-monitor`, Amount: $1, Reset: Annually
7. Add alert condition: 100% (Actual), email: joe@trashmob.eco

See `Deploy/COST_ALERT_RUNBOOK.md` for full step-by-step instructions.

### SendGrid (API Available ✅)

**Preferred: SendGrid API**
```bash
# Note: SendGrid billing alerts require contacting support or using dashboard
# API endpoint for usage limits not publicly available
# Must use manual configuration
```

**Required: Manual Portal Steps**
1. Log into https://app.sendgrid.com
2. Settings > Account Details > Billing
3. Under "Billing Alerts", click "Add Alert"
4. Set alert at 75% of monthly email limit
5. Set alert at 90% of monthly email limit
6. Add recipient: joe@trashmob.eco
7. **For hard cap:** Settings > Mail Settings > Email Activity > Set daily sending limit

### Google Maps Platform (API Available ✅)

**Preferred: gcloud CLI**
```bash
# Create budget alert for Maps API
gcloud billing budgets create \
  --billing-account=BILLING_ACCOUNT_ID \
  --display-name="TrashMob Maps API" \
  --budget-amount=100USD \
  --threshold-rule=percent=0.5,basis=CURRENT_SPEND \
  --threshold-rule=percent=0.9,basis=CURRENT_SPEND \
  --threshold-rule=percent=1.0,basis=CURRENT_SPEND \
  --notifications-rule-pubsub-topic=projects/trashmob/topics/billing-alerts

# Set hard cap (requires quota adjustment)
gcloud services api-keys update KEY_ID \
  --api-target=service=maps-backend.googleapis.com
```

**Fallback: Manual Portal Steps**
1. Log into https://console.cloud.google.com
2. Billing > Budgets & alerts
3. Click "Create Budget"
4. Name: "TrashMob Maps API", Amount: $100/month
5. Set thresholds: 50%, 90%, 100%
6. Enable email notifications
7. **For hard cap:** APIs & Services > Quotas > Maps API > Edit Quota > Set daily request limit

---

## Implementation Phases

### Phase 1: Azure Cost Management Setup
- Audit current Azure spending patterns
- Define budget amounts per environment
- Configure budgets and alerts in Azure Portal
- Test alert delivery

### Phase 2: Third-Party Service Alerts
- Configure SendGrid alerts
- Configure Google Maps API alerts
- Document baseline usage metrics

### Phase 3: Process & Documentation
- Write cost review runbook
- Add to monthly operations checklist
- Train team on responding to alerts

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Decisions

1. **What are the current monthly budget targets per environment?**
   **Decision:** Review last 3 months of Azure invoices to establish baseline; add 20% buffer for growth

2. **Should hard spending caps be enabled, or alerts only?**
   **Decision:** Yes, enable hard spending caps. Any expense exceeding $500 must be immediately stopped. This protects against runaway costs given the nonprofit grant context.

---

## Related Documents

- **[Risks & Mitigations](../Risks_and_Mitigations.md)** - SR-5 Cost Spikes risk
- **[CLAUDE.md](../../CLAUDE.md)** - Infrastructure documentation

---

**Last Updated:** January 31, 2026
**Owner:** Engineering Lead
**Status:** In Progress
**Next Review:** When volunteer available

---

## Changelog

- **2026-01-31:** Added $500 hard spending cap requirement
- **2026-01-31:** Added $1/year threshold alert for Microsoft nonprofit grant expiration detection
- **2026-01-31:** Converted open questions to decisions; confirmed all scope items
- **2026-02-06:** Phase 1 implemented: billingAlerts.bicep, deployInfra.ps1 updated, COST_ALERT_RUNBOOK.md created
- **2026-02-08:** Updated for billing tenant separation — budgets must be created in tenant `c0459473-d4ce-4472-b23c-dd1218a33a7b`, not the resource tenant
