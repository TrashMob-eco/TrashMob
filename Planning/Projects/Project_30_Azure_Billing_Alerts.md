# Project 30 — Azure Billing Caps & Alerts

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Small |
| **Dependencies** | None |

---

## Business Rationale

As a nonprofit, TrashMob must carefully manage cloud costs. Unexpected spikes in Azure, Maps API, or SendGrid usage could strain the budget and divert funds from mission-critical activities. Proactive billing alerts and spending caps provide early warning and automatic protection against runaway costs.

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

### Azure Portal Configuration

1. **Create Budget in Azure Cost Management**
   - Navigate to Cost Management + Billing > Budgets
   - Create budget for each subscription/resource group
   - Set monthly budget amount based on historical usage + buffer

2. **Configure Alert Rules**
   - Add action groups for email notifications
   - Set thresholds: 50% (informational), 75% (warning), 90% (critical), 100% (exceeded)

3. **Enable Anomaly Detection**
   - Navigate to Cost Management > Cost alerts
   - Enable anomaly alerts for unexpected spikes

### SendGrid Configuration

1. Log into SendGrid dashboard
2. Navigate to Settings > Billing > Alerts
3. Configure usage alerts at 75% and 90% of plan limits

### Google Maps Configuration

1. Log into Google Cloud Console
2. Navigate to Billing > Budgets & alerts
3. Create budget for Maps API usage

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
   **Decision:** Start with alerts only; enable hard caps on dev environment only to prevent runaway costs during development

---

## Related Documents

- **[Risks & Mitigations](../Risks_and_Mitigations.md)** - SR-5 Cost Spikes risk
- **[CLAUDE.md](../../CLAUDE.md)** - Infrastructure documentation

---

**Last Updated:** January 31, 2026
**Owner:** Engineering Lead
**Status:** Not Started
**Next Review:** When volunteer available

---

## Changelog

- **2026-01-31:** Converted open questions to decisions; confirmed all scope items
