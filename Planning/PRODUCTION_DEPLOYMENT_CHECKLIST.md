# Production Deployment Checklist

**Last Updated:** February 2, 2026
**Commits Since Last Release:** ~100+ commits from main

---

## Pre-Deployment Tasks

### 0. Key Vault Secrets (If Not Already Created)

Ensure required secrets exist before deployments:

**Dev environment:**
```bash
# Strapi database password (required for Strapi Azure SQL)
az keyvault secret show --vault-name kv-tm-dev-westus2 --name strapi-db-password || \
  az keyvault secret set --vault-name kv-tm-dev-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"
```

**Production environment:**
```bash
# Strapi database password (required for Strapi Azure SQL)
az keyvault secret show --vault-name kv-tm-pr-westus2 --name strapi-db-password || \
  az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"
```

### 1. Database Migrations (REQUIRED)

Run all 4 pending migrations in order:

```bash
# From TrashMob folder, connected to production database
dotnet ef database update
```

**Migrations to apply:**
1. `20260131222334_AddEventCoLeads` - Adds IsEventLead column to EventAttendees, backfills event creators as leads
2. `20260201013924_ChangePickedWeightToDecimal` - Changes EventSummaries.PickedWeight from int to decimal(10,1)
3. `20260201030122_AddTeamsFeature` - Creates Teams, TeamMembers, TeamEvents, TeamJoinRequests, TeamPhotos tables
4. `20260201164845_AddUserFeedback` - Creates UserFeedback table for in-app feedback
5. `20260202000411_AddCommunityPhase2Fields` - Adds LogoUrl, ContactEmail, ContactPhone, PhysicalAddress to Partner (Community)

**Note:** Migration #1 includes data backfill SQL that marks all event creators as event leads.

### 2. Infrastructure Deployments

#### 2.1 Application Insights Workbook (Recommended)
```bash
az account set --subscription "TrashMobProd"

az deployment group create \
  --resource-group rg-trashmob-pr-westus2 \
  --template-file Deploy/appInsightsWorkbook.bicep \
  --parameters environment=pr region=westus2 \
    rgName=rg-trashmob-pr-westus2 \
    appInsightsName=ai-tm-pr-westus2
```

#### 2.2 Backup Alerts (If not already deployed)
```bash
az deployment group create \
  --resource-group rg-trashmob-pr-westus2 \
  --template-file Deploy/backupAlerts.bicep \
  --parameters environment=pr region=westus2
```

#### 2.3 KeyVault RBAC (Already Completed)
PR #2482 migrated KeyVault from access policies to RBAC. Verify this is working correctly.

### 3. Strapi CMS (Optional)

If deploying Strapi to production for the first time:

**a. Create KeyVault secrets:**
```bash
az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-db-password --value "$(openssl rand -base64 32)"
az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-admin-jwt-secret --value "$(openssl rand -base64 32)"
az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-api-token-salt --value "$(openssl rand -base64 32)"
az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-app-keys --value "$(openssl rand -base64 32),$(openssl rand -base64 32)"
az keyvault secret set --vault-name kv-tm-pr-westus2 --name strapi-transfer-token-salt --value "$(openssl rand -base64 32)"
```

**b. Deploy Strapi database:**
```bash
az deployment group create \
  --resource-group rg-trashmob-pr-westus2 \
  --template-file Deploy/sqlDatabaseStrapi.bicep \
  --parameters environment=pr region=westus2
```

**c. Create production workflow** (copy from dev and update environment variables)

**Note:** The workflow automatically creates the SQL user and deploys the container.

---

## Deployment Steps

### 1. Merge main to release
```bash
git checkout release
git pull origin release
git merge origin/main
git push origin release
```

### 2. Wait for CI/CD Pipeline
- Container builds will trigger automatically
- Monitor GitHub Actions for success

### 3. Verify Deployment
- Check https://www.trashmob.eco is accessible
- Test login/logout functionality
- Verify Teams feature is working
- Verify Feedback widget is visible
- Check Application Insights for errors

---

## Post-Deployment Verification

### Feature Testing Checklist

- [ ] **Teams Feature (Project 9)**
  - [ ] Create a new team
  - [ ] Join an existing team
  - [ ] View Teams map
  - [ ] Upload team photo
  - [ ] Upload team logo
  - [ ] Associate event with team

- [ ] **User Feedback (Project 34)**
  - [ ] Feedback widget visible in bottom-right corner
  - [ ] Submit test feedback
  - [ ] Admin can view feedback at /siteadmin/feedback

- [ ] **Event Co-Leads (Project 21)**
  - [ ] Event creator marked as lead
  - [ ] Can add co-leads to events

- [ ] **Weight Tracking (Project 7)**
  - [ ] Event summary accepts decimal weights
  - [ ] Weight units dropdown works

- [ ] **Litter Reports (Project 3)**
  - [ ] Create litter report with photos
  - [ ] Litter reports appear on home map
  - [ ] Admin litter reports page works

- [ ] **Feature Metrics (Project 29)**
  - [ ] Login/logout events tracked
  - [ ] Event creation tracked
  - [ ] Attendance registration tracked

- [ ] **Community Pages (Project 10)**
  - [ ] Communities discovery page at /communities
  - [ ] Community detail page at /communities/{slug}
  - [ ] Community map shows events, teams, litter reports
  - [ ] Stats widget shows community impact metrics
  - [ ] Contact card displays email/phone/address
  - [ ] Events and Teams sections display nearby data

---

## Rollback Plan

If issues occur:

### Quick Rollback (< 5 min)
```bash
# List available revisions
az containerapp revision list \
  --name ca-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --output table

# Activate previous revision
az containerapp revision activate \
  --name ca-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --revision <previous-revision-name>

# Route traffic to previous revision
az containerapp ingress traffic set \
  --name ca-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --revision-weight <previous-revision-name>=100
```

### Database Rollback (If needed)
Database migrations do NOT have automatic rollback. If critical issues:
1. Restore from backup (Azure SQL automatic backups)
2. Or manually run Down() migration scripts

---

## New Features Summary

| Feature | Project | Description |
|---------|---------|-------------|
| Teams | Project 9 | User-created teams with membership, photos, events |
| Community Pages | Project 10 | Discovery, detail pages with map, stats, events, teams |
| User Feedback | Project 34 | In-app feedback widget + admin dashboard |
| Event Co-Leads | Project 21 | Multiple leads per event |
| Weight Tracking | Project 7 | Decimal weights, weight units |
| Litter Reports Web | Project 3 | Full web parity for litter reporting |
| Feature Metrics | Project 29 | Application Insights event tracking |
| OpenTelemetry | Project 27 | Migrated from App Insights SDK |
| KeyVault RBAC | Project 26 | Migrated from access policies |
| Database Backups | Project 32 | Configured retention policies |

---

## Contacts

- **Engineering:** @JoeBergeron
- **Emergency:** Check #engineering in Slack

---

**Remember:** Test in dev first! https://dev.trashmob.eco
