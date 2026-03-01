# Project 49 — Privacy and Compliance Review

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phase 5 remaining) |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Medium |
| **Dependencies** | Project 1 (Auth Revamp), Project 23 (Parental Consent) |

---

## Business Rationale

TrashMob collects personal data from users including names, email addresses, locations, event participation history, photos, and route tracking GPS data. As the platform grows — particularly into community partnerships and minor participation — privacy compliance becomes both a legal obligation and a trust signal to users.

The existing "Delete My Data" feature performs a mix of hard deletes and anonymization, but it has never been comprehensively audited against the full data model. Several gaps exist:

1. **Incomplete deletion audit:** Some user-linked data (litter reports, team membership, achievements, feedback) may not be fully anonymized or deleted. Profile photos in blob storage may be orphaned rather than deleted.

2. **No data export:** GDPR Article 20 (Right to Data Portability) requires that users can export their personal data in a machine-readable format. TrashMob currently has no user-facing data export feature.

3. **No formal GDPR compliance review:** While the platform anonymizes audit fields on deletion, there is no documented data processing inventory, no retention policy documentation, and no cookie/tracking consent mechanism beyond the existing terms of service.

4. **Aggregate preservation not verified:** Historical impact metrics (total bags, weight, event counts) should survive user deletion so community dashboards and leaderboards remain accurate. This needs explicit verification and documentation.

---

## Objectives

### Primary Goals
- Audit and fix the "Delete My Data" flow to ensure complete deletion or anonymization of all user PII across every table and blob storage
- Verify that aggregate metrics (event summaries, community totals, leaderboard data) are preserved after user deletion without retaining PII
- Implement a "Download My Data" feature allowing users to export all personal data in a standard format (JSON)
- Document data processing practices for GDPR compliance

### Secondary Goals (Nice-to-Have)
- Add a data retention policy configuration for communities (how long to keep anonymized records)
- Add privacy settings page where users can control data visibility preferences
- Create an admin data audit log showing anonymization/deletion events

### Phase 5 Goals (Cookie Consent — Promoted from Secondary)
- Remove unused third-party tracking scripts (Facebook SDK confirmed unused)
- Implement cookie consent banner for all users (GDPR/CCPA)
- Update Privacy Policy to accurately reflect actual tracking and cookie usage

---

## Scope

### Phase 1 — Deletion Audit & Fix

Comprehensive audit of the `UserManager.DeleteAsync()` flow against every table in `MobDbContext` that references a user.

- [x] Map every table and column that stores user references (UserId, CreatedByUserId, LastUpdatedByUserId, LeadUserId, etc.)
- [x] Compare the deletion flow against the full map — identify any tables or columns missed
- [x] Verify profile photo deletion from Azure Blob Storage occurs in `UserManager.DeleteAsync()`
- [x] Verify litter reports created by the user have their `CreatedByUserId` anonymized
- [x] Verify team membership records are cleaned up (user removed from teams)
- [x] Verify team lead ownership is handled (transfer or anonymize, not orphan)
- [x] Verify `UserAchievement` records are deleted or anonymized
- [x] Verify `UserFeedback` records are deleted or anonymized
- [x] Verify `UserWaiver` records are handled per legal retention requirements (preserve waiver, anonymize user link after retention period)
- [x] Wrap the entire deletion flow in a database transaction for atomicity
- [x] Add integration tests that create a user with data in every related table, delete the user, and verify no PII remains
- [x] Document what is deleted vs anonymized vs preserved and why

> **Implementation:** `UserDeletionService.cs` with 7-phase deletion (A-H) across 40+ entities, transaction-wrapped. PR #2867.

### Phase 2 — Aggregate Preservation Verification

Verify that key metrics survive user deletion.

- [x] Write integration tests that:
  - Create a user, create events with summaries, add attendance records, add route data
  - Record the aggregate totals (total bags, weight, distance, event count)
  - Delete the user
  - Verify aggregate totals remain unchanged
- [x] Verify community dashboard metrics are unaffected by user deletion
- [x] Verify leaderboard data handles deleted users gracefully (no broken references, no PII displayed)
- [x] Verify the `EventSummary` table retains totals after the creating user is deleted
- [x] Verify `EventAttendeeMetrics` records are preserved for historical impact but have no resolvable user reference
- [x] Document the aggregate preservation strategy for stakeholders

> **Implementation:** 11 unit tests in `AggregatePreservationAfterDeletionTests.cs`. Guid.Empty seed user set to `ShowOnLeaderboards = false`. PR #2870.

### Phase 3 — Data Export (Download My Data)

Allow users to export all personal data in machine-readable JSON format per GDPR Article 20.

- [x] Create `IUserDataExportManager` service that collects all user data:
  - User profile (name, email, city, region, country, preferences)
  - Event participation history (events attended, events led)
  - Event summaries created by the user
  - Route tracking data (routes, distances, durations, metrics)
  - Litter reports created by the user
  - Team memberships
  - Achievements earned
  - Waivers signed (dates, versions)
  - Feedback submitted
  - Partner admin roles
  - Notification preferences
- [x] Create `GET /api/users/{userId}/export` endpoint returning a JSON file download
- [x] Add authorization: users can only export their own data; site admins can export any user
- [x] Add rate limiting: one export per user per 24 hours
- [x] Add "Download My Data" button to the user profile / settings page
- [x] Add "Download My Data" link on the Delete My Data page (encourage users to export before deleting)
- [ ] ~~Include a `README.txt` in the export explaining each data section~~ (Skipped — JSON `_metadata` section provides format documentation inline)

> **Implementation:** `UserDataExportManager.cs` with streaming `Utf8JsonWriter` to avoid Front Door timeout. 13 data categories. Rate limited via `LastDataExportRequestedDate` on User model. PR #2872.

### Phase 4 — GDPR Documentation & Compliance

- [x] Create a data processing inventory documenting:
  - What personal data is collected
  - Why it is collected (legal basis)
  - How long it is retained
  - Who has access to it
  - How it is protected
- [x] Document data retention policies:
  - Active user data: retained while account is active
  - Deleted user data: PII removed immediately, anonymized records retained indefinitely for aggregate metrics
  - Waiver data: retained per legal requirements (configurable retention period)
  - Route GPS data: anonymized on deletion (UserPath retained without user link for aggregate heatmaps)
- [x] Review and update the Privacy Policy page to accurately reflect current practices
- [x] Ensure the Delete My Data page clearly explains what will be deleted vs preserved
- [x] Add data processing documentation to the repository (for developer reference)

> **Implementation:** `DATA_PROCESSING_INVENTORY.md`, `DEVELOPER_DATA_HANDLING_GUIDE.md`, Privacy Policy page rewritten to v1.0, Delete My Data page enhanced with collapsible data explanation.

### Phase 5 — Cookie Consent & Third-Party Tracking Audit

Audit of `index.html` (Feb 2026) revealed significantly more third-party tracking than documented. Cookie consent banner is required for GDPR compliance.

#### Third-Party Scripts in index.html

| Script | Purpose | Sets Cookies/Tracks | Status |
|--------|---------|-------------------|--------|
| **Microsoft Clarity** | Session recording & heatmaps | Yes — tracking cookies | Gated behind consent (PR #2939) |
| **Facebook SDK** (`connect.facebook.net`, appId `149684225125954`, `autoLogAppEvents=1`) | Social plugins | Yes — extensive tracking | **Removed** (PR #2939) |
| **Application Insights** (`js.monitor.azure.com`) | Telemetry & error tracking | Yes — session tracking | Gated behind consent (PR #2939) |
| **Twitter Widgets** (`platform.twitter.com/widgets.js`) | Social embedding | Yes — tracking cookies | **Removed** — unused (PR #2939) |
| **Google APIs** (`apis.google.com/js/platform.js`) | Sign-in / general | Potentially | **Removed** — unused (PR #2939) |
| **MyShop/Spreadshop** (`myspreadshop.com`) | Merchandise store embed | Potentially | Only on store page |

#### Facebook SDK Finding

The Facebook SDK is loaded with `autoLogAppEvents=1` (aggressive auto-logging of page views and user actions) but is **completely unused**:
- No `FB.*` API calls anywhere in the codebase
- No Facebook login/auth integration
- No XFBML social plugins rendered
- The only Facebook references are: a footer link to the TrashMob Facebook page (plain hyperlink), a `getFacebookUrl()` in `ShareUrl.tsx` that is never called, and standard Open Graph meta tags (used by all platforms, not Facebook-specific)
- **Action: Remove Facebook SDK from index.html immediately** — it adds tracking with zero functionality

#### Privacy Policy Inaccuracies

The current Privacy Policy (v1.0, Feb 23 2026) states:
- *"TrashMob uses minimal cookies, limited to authentication session management"* — **inaccurate**, Clarity and App Insights set tracking cookies
- *"We do not use third-party advertising cookies, cross-site tracking pixels, or behavioral analytics"* — **inaccurate**, Clarity IS behavioral analytics, Facebook SDK has `autoLogAppEvents`
- Third-party services list omits: Clarity, Facebook SDK, Twitter Platform, Application Insights, MyShop/Spreadshop

#### Tasks

- [x] Remove Facebook SDK from `index.html` (unused, adds tracking for zero benefit) — PR #2939
- [x] Audit Twitter Widgets and Google APIs usage — removed, both unused — PR #2939
- [x] Implement cookie consent banner that blocks Clarity, App Insights, and any remaining social scripts until user accepts — PR #2939, #2940
- [ ] Update Privacy Policy to accurately list all third-party services and their cookie/tracking behavior
- [ ] Update `DATA_PROCESSING_INVENTORY.md` with Clarity, App Insights, and any remaining third-party tracking

---

## Out of Scope

- CCPA-specific compliance (California) — address if user base grows there
- Data Processing Agreements (DPAs) with third parties (Azure, SendGrid) — handled at the organizational level
- Automated data breach notification system
- Privacy Impact Assessments for new features (establish process but don't retroactively assess all features)
- HIPAA compliance (not applicable)

---

## Success Metrics

### Quantitative
- **Deletion completeness:** 100% of PII columns across all tables are handled in the deletion flow (currently estimated ~85%)
- **Aggregate preservation:** 100% of integration tests pass confirming metrics survive deletion
- **Export coverage:** Export includes data from 100% of user-linked tables
- **Export response time:** Data export completes in < 30 seconds for typical users

### Qualitative
- Engineering team has confidence that user deletion is thorough and atomic
- A clear, documented data map exists showing what data is stored, why, and for how long
- Users can self-serve both data export and data deletion without contacting support

---

## Dependencies

### Blockers (Must be complete before this project starts)
- None — this project can begin immediately as an audit of existing functionality

### Enablers for Other Projects (What this unlocks)
- **[Project 23 — Parental Consent](./Project_23_Parental_Consent.md):** COPPA compliance requires verified deletion of minor data; this project establishes the deletion audit framework
- **[Project 1 — Auth Revamp](./Project_01_Auth_Revamp.md):** Entra External ID migration changes where user identity data lives; deletion flow must account for both B2C and Entra scenarios

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| **Missed table in deletion audit** | Medium | High | Automated test that queries `INFORMATION_SCHEMA.COLUMNS` for all columns referencing user IDs and compares against handled list |
| **Transaction wrapping causes deadlocks** | Low | Medium | Use `ReadCommitted` isolation level; test with concurrent deletion requests |
| **Data export is slow for power users with thousands of events** | Low | Medium | Stream JSON response; add background job option for large exports |
| **Legal retention requirements conflict with deletion requests** | Medium | High | Document retention exceptions clearly on the Delete My Data page; retain only what is legally required (waivers) with anonymized user references |
| **Blob storage cleanup fails silently** | Medium | Medium | Add error logging and retry logic for blob deletion; run periodic orphaned blob cleanup job |

---

## Implementation Plan

### Data Model Changes

No new tables required. Potential additions:

```sql
-- Track data export requests (rate limiting)
ALTER TABLE Users ADD LastDataExportRequestedDate DATETIMEOFFSET NULL;

-- Track deletion audit completeness
-- (No schema change — handled via integration tests)
```

### API Changes

```csharp
// New endpoint: Export user data
[HttpGet("api/users/{userId}/export")]
[Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
public async Task<IActionResult> ExportUserData(Guid userId)
{
    // Authorization: self or admin only
    // Rate limit: once per 24 hours
    // Returns JSON file download
}
```

### Web UX Changes

- Add "Download My Data" button to user profile/settings page
- Add "Download My Data" link on the `/deletemydata` page with explanatory text
- Update Delete My Data page text to clearly document what is deleted vs preserved

### Mobile App Changes

- Add "Download My Data" option in Settings screen (opens web browser to export endpoint)
- No native export UI needed — redirect to web

---

## Implementation Phases

### Phase 1: Deletion Audit & Fix (1-2 weeks)
1. Map all user-referencing columns across every table in MobDbContext
2. Compare against current `UserManager.DeleteAsync()` — document gaps
3. Fix gaps: add missing anonymization/deletion for each missed entity
4. Add blob storage cleanup for profile photos
5. Wrap deletion in a database transaction
6. Write integration tests for deletion completeness
7. Document deletion behavior (table-by-table)

### Phase 2: Aggregate Preservation Verification (3-5 days)
1. Write integration tests: create user → add data → record aggregates → delete user → verify aggregates unchanged
2. Test community dashboard, leaderboard, and event summary queries post-deletion
3. Document the aggregate preservation strategy

### Phase 3: Data Export (1-2 weeks)
1. Implement `IUserDataExportManager` — collect all user-linked data
2. Create export API endpoint with authorization and rate limiting
3. Add "Download My Data" button to profile and delete-my-data pages
4. Write tests for export completeness and authorization

### Phase 4: GDPR Documentation (3-5 days)
1. Create data processing inventory document
2. Document retention policies
3. Review and update Privacy Policy page content
4. Add developer-facing data handling documentation to repo

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Current State Analysis

### What Currently Works

The `UserManager.DeleteAsync()` method (in `TrashMob.Shared/Managers/UserManager.cs`) performs:

| Entity | Current Action |
|--------|---------------|
| `User` | Hard delete |
| `EventAttendee` | Hard delete (removes from all events) |
| `UserNotification` | Hard delete |
| `NonEventUserNotification` | Hard delete |
| `PartnerAdmin` (user is admin) | Hard delete |
| `PartnerRequest` (created/updated by) | Anonymize → `Guid.Empty` |
| `EventSummary` (created/updated by) | Anonymize → `Guid.Empty` |
| `EventPartnerLocationService` (created/updated by) | Anonymize → `Guid.Empty` |
| `Event` (created/updated by) | Anonymize → `Guid.Empty` |
| `Partner` (created/updated by) | Anonymize → `Guid.Empty` |
| `PartnerLocation` (created/updated by) | Anonymize → `Guid.Empty` |
| Profile photo | Deleted from blob storage |

### Known Gaps to Investigate

| Entity | Gap |
|--------|-----|
| `LitterReport` | Creator anonymization not confirmed |
| `TeamMember` / Team membership | User removal from teams not confirmed |
| `Team` (if user is team lead) | Ownership transfer or anonymization not confirmed |
| `UserAchievement` | Deletion or anonymization not confirmed |
| `UserFeedback` | Deletion or anonymization not confirmed |
| `EventAttendeeRoute` | Route data preservation vs anonymization not confirmed |
| `EventAttendeeMetrics` | Preservation confirmed but no explicit test |
| `UserWaiver` | Retained for legal reasons — anonymization timeline unclear |
| Transaction atomicity | No transaction boundary — partial deletion possible on failure |

---

## Open Questions

1. **Waiver retention period:** How long must signed waivers be retained after account deletion? State laws vary (typically 3-7 years for liability waivers).
   **Recommendation:** Retain waiver records for 7 years after the event date, then hard delete. Anonymize user reference immediately on account deletion but keep the waiver record with a hash of the signer's identity for legal verification.
   **Owner:** Legal / @JoeBeernink
   **Due:** Before Phase 1 completion

2. **Team lead deletion:** When a team lead deletes their account, should the team be transferred to another member, archived, or deleted?
   **Recommendation:** If other members exist, transfer to the longest-tenured member. If no other members, archive the team (soft delete). Never hard-delete a team with historical event associations.
   **Owner:** @JoeBeernink
   **Due:** Before Phase 1 completion

3. **Route data after deletion:** Should anonymized route GPS paths be preserved for community heatmaps, or should they be deleted entirely?
   **Recommendation:** Preserve anonymized route data (it's already anonymized — no user link after EventAttendee deletion). GPS paths are not PII when disconnected from a user identity.
   **Owner:** @JoeBeernink
   **Due:** Before Phase 1 completion

4. **Export format:** Should the export be a single JSON file or a ZIP with multiple files (one per data type)?
   **Recommendation:** Single JSON file with top-level keys per data type (`profile`, `events`, `routes`, `litterReports`, etc.) plus a `README.txt`. ZIP only if file size exceeds 50MB.
   **Owner:** Engineering
   **Due:** Before Phase 3

---

## Related Documents

- **[Project 1 — Auth Revamp](./Project_01_Auth_Revamp.md)** — Auth migration affects where user identity data is stored
- **[Project 23 — Parental Consent](./Project_23_Parental_Consent.md)** — COPPA compliance for minors requires verified data deletion
- **[Project 8 — Waivers V3](./Project_08_Waivers_V3.md)** — Waiver retention requirements intersect with deletion policy
- **[Cross-Cutting: Minor Privacy Standards](../Cross_Cutting_Minor_Privacy_Standards.md)** — Privacy rules for minor users
- `TrashMob.Shared/Managers/UserManager.cs` — Current deletion implementation
- `TrashMob/Controllers/UsersController.cs` — Delete endpoint (lines 189-211)
- `TrashMob/client-app/src/pages/deletemydata/index.tsx` — Delete My Data UI
- `TrashMob.Shared/Persistence/MobDbContext.cs` — Entity relationships and cascade configurations

---

## GitHub Issues

_To be created on project start._

---

**Last Updated:** February 28, 2026
**Owner:** @JoeBeernink
**Status:** Phases 1-4 Complete; Phase 5 (Cookie Consent & Tracking Audit) Not Started
**Next Review:** On Phase 5 kickoff
