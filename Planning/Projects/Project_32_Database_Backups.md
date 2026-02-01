# Project 32 — Database Backups

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | High |
| **Risk** | Low |
| **Size** | Small |
| **Dependencies** | None |

---

## Business Rationale

Data is TrashMob's most valuable asset. User accounts, event history, volunteer contributions, and community partnerships must be protected against accidental deletion, corruption, or disaster. Robust backup strategies ensure business continuity and meet data protection requirements.

This project directly mitigates **OR-1 (Data Loss During Migration)** and supports compliance with data retention policies.

---

## Objectives

### Primary Goals
- Configure automated backups for Azure SQL databases (main TrashMob database)
- Configure automated backups for Strapi CMS database
- Establish and document restore procedures
- Implement backup monitoring and alerting

### Secondary Goals (Nice-to-Have)
- Cross-region backup replication for disaster recovery
- Automated restore testing
- Backup cost optimization

---

## Scope

### Phase 1 - Azure SQL Backup Configuration
- ✅ Review and document current Azure SQL backup settings
- ✅ Configure short-term retention policy (point-in-time restore window)
- ✅ Configure long-term retention (LTR) policy for compliance
- ✅ Set up backup alerts for failures
- ✅ Document restore procedures

### Phase 2 - Strapi Database Backup
- ✅ Identify Strapi database location and type (SQLite or Azure SQL)
- ✅ Configure appropriate backup strategy based on database type
- ✅ Set up automated backup schedule
- ✅ Document Strapi-specific restore procedures

### Phase 3 - Validation & Monitoring
- ✅ Perform test restore to verify backup integrity
- ✅ Set up backup monitoring dashboard
- ✅ Create runbook for restore scenarios
- ✅ Schedule quarterly restore tests

---

## Out-of-Scope

- ❌ Application-level backups (code, configuration)
- ❌ Blob storage backups (handled separately)
- ❌ Full disaster recovery site setup

---

## Success Metrics

### Quantitative
- **Recovery Point Objective (RPO):** < 1 hour for production databases
- **Recovery Time Objective (RTO):** < 4 hours for full restore
- **Backup success rate:** 100% of scheduled backups complete
- **Test restores:** Quarterly validation completed

### Qualitative
- Team confident in ability to recover from data loss
- Clear documentation for restore procedures

---

## Dependencies

### Blockers (Must be complete before this project starts)
- **Azure subscription access:** Contributor role on database resources

### Enablers for Other Projects (What this unlocks)
- **Project 1 (Auth Revamp):** Safe migration with backup safety net
- **Project 5 (Deployment Pipelines):** Confidence for database migrations
- **OR-1 mitigation:** Directly addresses data loss risk

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Backup costs exceed budget** | Medium | Low | Use appropriate retention tiers; monitor costs monthly |
| **Restore process untested** | Medium | High | Quarterly restore tests to non-production environment |
| **Strapi uses SQLite with ephemeral storage** | Medium | High | Migrate to Azure SQL or implement blob-based backup |

---

## Implementation Plan

### Azure SQL Database (Main Database)

**Short-Term Retention (Point-in-Time Restore)**
```bash
# Check current backup settings
az sql db show --name trashmob-db --server sql-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --query "{retention: shortTermRetentionPolicy}"

# Configure 14-day point-in-time restore window (if not already set)
az sql db str-policy set --name trashmob-db --server sql-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --retention-days 14
```

**Long-Term Retention (LTR)**
```bash
# Configure weekly backups retained for 4 weeks, monthly for 12 months
az sql db ltr-policy set --name trashmob-db --server sql-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --weekly-retention P4W \
  --monthly-retention P12M
```

**Backup Alerts**
- Configure Azure Monitor alert for backup failures
- Send notifications to engineering team

### Strapi Database

**Current State (Dev Deployment):**
- **Database:** SQLite with ephemeral container storage
- **Configuration:** `DATABASE_CLIENT: 'sqlite'` in `Deploy/containerAppStrapi.bicep`
- **Limitation:** Azure Files SMB has locking issues with SQLite, so database is ephemeral
- **Risk:** Data loss on container restart/redeployment

**Backup Options:**
1. **Migrate to Azure SQL (recommended):** `Deploy/sqlDatabaseStrapi.bicep` exists but is not currently wired up. Migration would enable Azure's built-in backup with same policies as main database.
2. **Scheduled SQLite export:** Use Strapi CLI or custom job to export to Azure Blob Storage before each deployment.
3. **Accept ephemeral for dev:** If CMS content is recreatable, accept dev data loss; only implement backups for production.

### Restore Procedures

**Point-in-Time Restore (Azure SQL)**
```bash
# Restore to specific point in time
az sql db restore --dest-name trashmob-db-restored \
  --name trashmob-db --server sql-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --time "2026-01-25T12:00:00Z"
```

**LTR Restore**
```bash
# List available LTR backups
az sql db ltr-backup list --location westus2 \
  --server sql-tm-pr-westus2 --database trashmob-db

# Restore from LTR backup
az sql db ltr-backup restore --dest-database trashmob-db-restored \
  --dest-server sql-tm-pr-westus2 \
  --dest-resource-group rg-trashmob-pr-westus2 \
  --backup-id "<backup-id-from-list>"
```

---

## Implementation Phases

### Phase 1: Azure SQL Backup Configuration
- Audit current backup settings for dev and prod databases
- Configure short-term retention (14 days recommended)
- Configure long-term retention (weekly/monthly)
- Set up Azure Monitor alerts for backup failures
- Document current state and any changes made

### Phase 2: Strapi Database Backup
- Determine Strapi database type in each environment
- For Azure SQL: apply backup policies
- For SQLite: implement backup-to-blob or migrate to Azure SQL
- Document Strapi-specific considerations

### Phase 3: Validation & Monitoring
- Perform test restore to verify procedures
- Create backup monitoring dashboard in Azure Portal
- Write restore runbook with step-by-step instructions
- Add quarterly restore test to operations calendar

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Decisions

1. **What is the required data retention period for compliance?**
   **Decision:** 12 months minimum for audit purposes

2. **Should Strapi be migrated from SQLite to Azure SQL for production?**
   **Context:** Dev uses SQLite with ephemeral storage (`Deploy/containerAppStrapi.bicep` line 122). Bicep template for Azure SQL exists (`Deploy/sqlDatabaseStrapi.bicep`) but is not wired up.
   **Decision:** Yes, migrate to Azure SQL for reliability and consistent backup strategy

3. **Is cross-region replication needed for disaster recovery?**
   **Decision:** Not initially; evaluate after core backups in place

---

## Related Documents

- **[Risks & Mitigations](../Risks_and_Mitigations.md)** - OR-1 Data Loss risk
- **[CLAUDE.md](../../CLAUDE.md)** - Infrastructure documentation, Strapi setup
- **[Project 5 - Deployment Pipelines](./Project_05_Deployment_Pipelines.md)** - Database migration safety

---

**Last Updated:** January 31, 2026
**Owner:** Engineering Lead
**Status:** Not Started
**Next Review:** When volunteer available

---

## Changelog

- **2026-01-31:** Converted open questions to decisions (12-month retention, migrate Strapi to Azure SQL, defer cross-region replication)
- **2026-01-31:** Clarified Strapi uses SQLite with ephemeral storage in dev deployment
- **2026-01-31:** Confirmed all scope items
