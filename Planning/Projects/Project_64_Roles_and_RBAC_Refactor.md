# Project 64 — Roles and RBAC Refactor

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress — Phases 1–4a shipped to production 2026-07-05; Phase 4b (column drop) time-gated on 30-day observation window |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Large |
| **Dependencies** | _None_ — this project unblocks [Project 63 (Municipal Sales Pipeline Reporting)](./Project_63_Municipal_Sales_Pipeline_Reporting.md) and any future non-admin role |

## Shipped

| Phase | PR | Shipped |
|-------|-----|---------|
| Phase 1 — Role / UserRole model + `IUserRoleService` + compat bridge | [#3468](https://github.com/TrashMob-eco/TrashMob/pull/3468) | 2026-07-03 |
| Phase 2 — Auth handler migration + `SalesRep` policy + `UsersV2Controller` `[Authorize]` | [#3473](https://github.com/TrashMob-eco/TrashMob/pull/3473) | 2026-07-04 |
| Phase 3 — Role admin UI + `RolesV2Controller` + grant/revoke emails | [#3474](https://github.com/TrashMob-eco/TrashMob/pull/3474) | 2026-07-04 |
| Phase 4a — Sweep production `IsSiteAdmin` readers | [#3475](https://github.com/TrashMob-eco/TrashMob/pull/3475) | 2026-07-04 |
| Prod release + backfill migration hotfix | [#3481](https://github.com/TrashMob-eco/TrashMob/pull/3481) + [#3483](https://github.com/TrashMob-eco/TrashMob/pull/3483) | 2026-07-05 |
| **Phase 4b — Drop `IsSiteAdmin` column** | (queued) | Earliest ~2026-08-05 (30 days after Phase 4a in prod) |

## Lessons learned

- **The "well-known system user" pattern was a fiction.** The plan referenced "a well-known 'System' user id used elsewhere for automated writes" and told the backfill migration to use `00000000-0000-0000-0000-000000000001` for the `GrantedBy` / `CreatedBy` / `LastUpdatedBy` audit fields. That row doesn't actually exist in the `Users` table. Dev happened to pass because dev has no `IsSiteAdmin = 1` rows to trigger the failing `INSERT SELECT`; prod caught it with `FK_UserRoles_User_CreatedBy`.
  - Fix: [#3483](https://github.com/TrashMob-eco/TrashMob/pull/3483) — re-attributed audit fields to `u.Id` (the user whose grant is being backfilled). Semantically weird ("each SiteAdmin granted themselves this role during migration") but every FK resolves to a real row.
  - Guardrail for future migrations: **never invent a sentinel user id.** Either use a real existing user (self-reference, or query for any admin), or add a nullable FK.

- **Dev-passing migrations can still fail in prod.** The dev DB has a much smaller / different user base than prod, and any migration that touches `Users`-referencing tables should be tested against a schema populated with production-representative rows before rolling to release.

---

## Business Rationale

Authorization on TrashMob is currently gated by a single `IsSiteAdmin` boolean on the `User` entity. Every non-user-owned operation that isn't public is either "any authenticated user" or "site admin only" — there is no middle tier. This has worked for the last five years because the site had exactly two audiences (volunteers and one small operations team who had full admin rights).

That model breaks the moment a second role shows up:

- The **1099 municipal sales contractor** (see [Project 63](./Project_63_Municipal_Sales_Pipeline_Reporting.md)) needs to see and edit the prospect CRM, view sales reports, and nothing else. They must not see user administration, waivers, event moderation, photo moderation, community management, or site-wide financial data.
- A **future community manager** at a partner org needs write access to their community's pages but not to other communities.
- A **content editor** (if we decide to onboard volunteer writers to draft news articles into Strapi) needs Strapi-and-newsletter access but not to touch the API or user data.
- **A partner admin** who runs adopted-location sponsorships needs to see only their own sponsor org's data.

The `IsSiteAdmin` boolean is a sledgehammer. Handing it to any of the above puts data at risk. Handing them nothing means they can't do their job. We need the roles infrastructure that every other B2B SaaS has, and it's a strict prerequisite before the sales contractor gets a login.

**Scope discipline:** this project delivers **coarse roles**, not fine-grained permissions. A role like `SalesRep` grants a fixed bundle of capabilities. We are not building a permission-per-endpoint policy editor; that lives in a follow-up project (or never — most SaaS at our scale never build one). The line is "which capability tier are you in?", not "can this user perform action X on resource Y?".

---

## What Exists Today

Independent-engineer scan of the auth surface before drafting scope:

- **`User.IsSiteAdmin`** boolean, set/preserved in `UserManager.AddAsync` — new users default to `false`, updates preserve the current value (users cannot self-promote). Direct DB writes are the only way to grant admin today.
- **234 usages** of `IsSiteAdmin` across [`TrashMob`](../../TrashMob/), [`TrashMob.Shared`](../../TrashMob.Shared/), and [`TrashMob.Models`](../../TrashMob.Models/). Not all of those need to change — many are read-side checks that will keep working once we introduce a role-aware helper.
- **6 authorization handlers** in [`TrashMob/Security/`](../../TrashMob/Security/) that check `IsSiteAdmin` directly:
  - `UserIsAdminAuthHandler`
  - `UserIsEventLeadOrIsAdminAuthHandler`
  - `UserIsPartnerUserOrIsAdminAuthHandler`
  - `UserIsProfessionalCompanyUserOrIsAdminAuthHandler`
  - `UserOwnsEntityOrIsAdminAuthHandler`
  - `UserIsValidUserAuthHandler` (indirect — via user provisioning)
- **9 authorization policies** in `AuthorizationPolicyConstants.cs`: `UserIsAdmin`, `UserIsPartnerUserOrIsAdmin`, `UserOwnsEntity`, `UserOwnsEntityOrIsAdmin`, `UserIsEventLead`, `UserIsEventLeadOrIsAdmin`, `ValidUser`, `UserIsProfessionalCompanyUserOrIsAdmin`, `IftttServiceKey`. All work today; new role-scoped policies will layer alongside them.
- **`UsersV2Controller`** — 4 endpoints touch `IsSiteAdmin` on writes (create + update + admin-only writes to another user). These are the endpoints that need to route through a role-grant/revoke API in Phase 3.
- **Photo moderation** and other jobs query `IsSiteAdmin` to find moderators for notifications; those queries need to swap to "users with the `PhotoModerator` role or the `SiteAdmin` role" (a small change once the data model lands).

The refactor is entirely additive up to Phase 4 — no existing endpoint changes behavior until we're ready to flip the boolean.

---

## Objectives

### Primary Goals

- **Introduce a `Role` + `UserRole` data model** that supports many-to-many assignment with audit fields (who granted, when, optional expiry).
- **Preserve existing behavior** during migration: every user with `IsSiteAdmin = true` today gets granted the `SiteAdmin` role, and every authorization handler continues to accept `IsSiteAdmin OR SiteAdmin-role-member` until the boolean is decommissioned.
- **Ship a `SalesRep` role** as the first non-admin role, with a policy narrow enough that Project 63 can rely on it in production.
- **Provide an admin UI** to grant / revoke roles with an audit trail.
- **Deprecate and remove `User.IsSiteAdmin`** once all readers are migrated (last phase).

### Secondary Goals

- Establish the pattern so adding a new role (`PartnerAdmin`, `CommunityManager`, `ContentEditor`) is a one-migration + one-policy exercise, not a codebase-wide refactor.
- Give role assignments an optional `ExpiryDate` so we can grant temporary access without back-and-forth (contractor onboarding trials, security incident response).
- Support role-membership queries efficiently — the `PhotoModerationManager` and any future "notify all Xs" job needs a fast lookup.

---

## Scope

### Phase 1 — Data model + read-side layering (~1 week; ships first, no behavior change)

- ☐ Add `Role` entity: `Id (Guid)`, `Name (nvarchar(60), unique)`, `Description (nvarchar(300))`, standard audit fields
- ☐ Add `UserRole` entity: `Id`, `UserId (FK)`, `RoleId (FK)`, `GrantedByUserId`, `GrantedDate`, `ExpiryDate (nullable)`, `RevokedDate (nullable)`, `RevokedByUserId (nullable)`, audit fields. Unique constraint on (UserId, RoleId) where `RevokedDate IS NULL`
- ☐ Seed default roles: `SiteAdmin`, `SalesRep`. (More roles land as needed; do not pre-seed speculative ones.)
- ☐ Backfill migration: for every `User` with `IsSiteAdmin = true`, insert a `UserRole` row granting `SiteAdmin`. GrantedByUserId = a well-known system user id documented in `Constants.cs`.
- ☐ New `IUserRoleService` in `TrashMob.Shared.Managers.Interfaces`:
  - `Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken ct)`
  - `Task<IReadOnlyCollection<string>> GetRoleNamesAsync(Guid userId, CancellationToken ct)`
  - `Task<IReadOnlyCollection<User>> GetUsersInRoleAsync(string roleName, CancellationToken ct)`
- ☐ Implementation in `UserRoleManager` with in-memory caching per-request via `IHttpContextAccessor` (so authorization handlers don't do a DB round trip per policy check)
- ☐ **Compatibility bridge:** `HasRoleAsync("SiteAdmin", ...)` returns `true` if the user has an active `UserRole` for SiteAdmin **OR** `IsSiteAdmin = true`. This keeps the pre-migration state working.
- ☐ Unit tests for the manager covering: active grant, revoked grant, expired grant, boolean-only fallback, role-mixed users
- ☐ Wire into DI in [`ServiceBuilder.cs`](../../TrashMob.Shared/ServiceBuilder.cs)

### Phase 2 — Authorization handler migration (~1 week; enables SalesRep role in production)

- ☐ Update the 6 auth handlers to check `HasRoleAsync("SiteAdmin", ...)` instead of `user.IsSiteAdmin`. The compatibility bridge above means no behavior change for existing SiteAdmins.
- ☐ New `AuthorizationPolicyConstants` entries: `UserIsSalesRepOrIsAdmin` and any others required by [Project 63](./Project_63_Municipal_Sales_Pipeline_Reporting.md)
- ☐ New handler `UserIsSalesRepOrIsAdminAuthHandler` following the existing pattern
- ☐ Update [`Program.cs`](../../TrashMob/Program.cs) DI: register the new handler
- ☐ Apply `[Authorize(Policy = UserIsSalesRepOrIsAdmin)]` to the Prospect-related v2 controllers (`CommunityProspectsV2Controller`, `ProspectContactsV2Controller`, and the new sales-report controllers from Project 63)
- ☐ Update integration tests in [`TrashMob.Shared.Tests/Controllers/V2/`](../../TrashMob.Shared.Tests/Controllers/V2/) — one covering a SalesRep hitting a prospect endpoint (allowed), another covering a SalesRep hitting a user-admin endpoint (forbidden)
- ☐ [Project 62](./Project_62_TrashMob_Site_and_Codebase_Reevaluation.md) also called out the `UsersV2Controller` missing class-level `[Authorize]` — take that fix in this project's PR to eliminate a real anonymous-read leak while we're editing the file

### Phase 3 — Admin UI for role management (~1 week)

- ☐ New v2 controller `RolesV2Controller`:
  - `GET /api/v2/roles` — list all roles
  - `GET /api/v2/roles/{roleName}/members` — users with the role
  - `POST /api/v2/users/{userId}/roles` — grant a role (body: `{ roleName, expiryDate? }`)
  - `DELETE /api/v2/users/{userId}/roles/{roleName}` — revoke
  - All gated on `UserIsAdmin` policy — only SiteAdmins can grant/revoke roles
- ☐ New admin page `/siteadmin/roles`: table of roles with member counts + click to see members
- ☐ New admin page `/siteadmin/users/{userId}/roles`: shows current roles for the user, add/remove buttons, expiry date picker
- ☐ Extend `/siteadmin/users` list view with a "Roles" column (comma-separated pill list)
- ☐ Email templates: `RoleGranted.html`, `RoleRevoked.html` — notify the affected user with a plain "Your account has been granted the SalesRep role by {actor}" message

### Phase 4 — Deprecate `IsSiteAdmin` boolean (~1 week; ships when Phase 2+3 are stable)

- ☐ Rewrite the 234 direct `IsSiteAdmin` references to go through the role-aware helper (`await userRoleService.HasRoleAsync(user.Id, "SiteAdmin", ct)` where an async call is possible; a synchronous fallback via cached user context where it isn't)
- ☐ Update `UsersV2Controller` write endpoints to route SiteAdmin promotion through the role-grant API rather than setting the boolean directly
- ☐ Add a computed property `User.IsSiteAdmin => Roles.Any(r => r.Name == "SiteAdmin")` (backed by the navigation property) so any callers we missed continue to work as read-only
- ☐ Remove the persisted `IsSiteAdmin` column via a migration after a 30-day observation window with no regressions
- ☐ Update the seed data + fixture builders to grant `SiteAdmin` role rather than set the boolean

### Phase 5 — Deferred (open a follow-up project if pursued)

- ☐ Fine-grained permissions (per-endpoint capability model)
- ☐ Delegated admin — an org admin who can grant roles within their org only
- ☐ Just-in-time role elevation with 2FA re-prompt
- ☐ Role change audit report / dashboard tie-in with [Project 56 (Board Metrics Dashboard)](./Project_56_Board_Metrics_Dashboard.md)

---

## Out of Scope

- **Fine-grained per-endpoint or per-resource permissions.** Roles are coarse bundles by design.
- **Multi-tenant role isolation.** If we ever ship a spinout that needs tenant-scoped roles, that's a bigger effort.
- **External identity provider role sync** (SSO groups → TrashMob roles). Later, if needed.
- **Retroactive role assignment audit** for the 5-year `IsSiteAdmin` history. The backfill grants happen once; no attempt to reconstruct who granted admin to whom in the past.
- **UI to create new roles.** Roles are seeded from code and require a migration. This is intentional — accidental role creation shouldn't be a UI action.

---

## Success Metrics

### Quantitative

- **Zero regressions in existing SiteAdmin capability** verified by full test-suite pass at each phase boundary, including the 1,115 existing tests
- **SalesRep role gates production access** — after Phase 2 ships, a user with only `SalesRep` role gets HTTP 200 on prospect endpoints and HTTP 403 on user-admin endpoints, verified by an xUnit integration test
- **All 234 direct `IsSiteAdmin` references migrated** to the role-aware helper by end of Phase 4
- **Median policy-check latency stays under 5 ms** (target — the per-request cache should make this trivial, but it's worth tracking to prevent a regression sneaking in)

### Qualitative

- Adding the next new role (`PartnerAdmin` or similar) requires only a migration + a policy + a handler — no touching auth infrastructure
- The salesperson from Project 63 onboards without needing to be granted `IsSiteAdmin` "temporarily to unblock them"
- Admin UI makes it obvious who can do what across the small team without needing to open the database

---

## Dependencies

### Blockers (Must be complete before this project starts)

- _None._ This project can start immediately. It depends only on the current auth infrastructure, all of which exists.

### Enablers for Other Projects (What this unlocks)

- **[Project 63 — Municipal Sales Pipeline Reporting](./Project_63_Municipal_Sales_Pipeline_Reporting.md):** requires the `SalesRep` role from Phase 2 before the 1099 sales contractor is given a login
- **Future partner-admin, community-manager, content-editor roles** — the pattern established here makes each of these a small, well-scoped addition
- **[Project 62 — TrashMob Site & Codebase Re-evaluation](./Project_62_TrashMob_Site_and_Codebase_Reevaluation.md)** — one of the follow-up hardening items surfaced there was the `UsersV2Controller` missing class-level `[Authorize]`. Phase 2 of this project closes that gap while touching the same file.

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| **A missed `IsSiteAdmin` read in Phase 4 silently downgrades a SiteAdmin's capability** | High | High | Grep-based inventory of all 234 usages *before* starting Phase 4. Land the boolean → role-helper migration in slices (one handler at a time) with tests. Keep the computed property fallback so anything we miss reads correctly. |
| **The per-request role cache stales incorrectly and a just-revoked user still has access for the rest of the request** | Low | Medium | The cache is per-request only, scoped to the single `HttpContext`. Revocation takes effect on the next request. Explicitly documented. Acceptable for a coarse-role system. |
| **Backfill migration incorrectly grants SiteAdmin to a user who had `IsSiteAdmin=true` due to an old testing artifact** | Medium | Medium | Manually review the seed backfill output before running in prod. Currently ~5 users have the flag; individually verify. |
| **A malicious admin grants SiteAdmin to themselves and covers tracks by revoking the auditor** | Low | High | Every role change writes an immutable audit row. Grant/revoke of SiteAdmin sends an email to *all* existing SiteAdmins (not just the affected user). Acceptable defense-in-depth for a small team. |
| **The 6 auth handlers get updated inconsistently — Handler A checks role, Handler B still checks boolean, and a user with the role but not the boolean sees inconsistent behavior** | Medium | High | Handle the migration in Phase 2 as a **single PR** touching all 6 handlers together, not one at a time. Compatibility bridge (Phase 1) means the transition is safe in either direction. |
| **The new admin UI is a persistent target — a SiteAdmin grants access accidentally** | Medium | Medium | Grant/revoke are two-step (select role + confirm modal with explicit consequences shown). Every action is audited and emailed. |
| **Existing tests reference `.IsSiteAdmin` extensively in fixtures — updating them is tedious and error-prone** | High | Low | Keep the boolean as a computed property (Phase 4). Fixtures continue to set `IsSiteAdmin = true` and the property reflects role membership on read. No test rewrites needed for the read path. |
| **Deprecation of the `IsSiteAdmin` column happens before all readers are migrated → runtime NRE** | Low | High | The column stays until an explicit greenlight after Phase 4 stabilises. Don't run the drop migration for at least 30 days after all readers move to the helper. |
| **Project 63 timing pressure — the salesperson is starting soon** | High | Medium | Phase 1 + Phase 2 can ship in a 2-week window. If schedule tight, Project 63 Phase 1 (data model + admin forms) can ship *without* the SalesRep role and the salesperson can be granted temporary `SiteAdmin` for the transition — with the caveat that this is explicitly time-boxed and revoked once Project 64 Phase 2 lands. |

---

## Implementation Plan

### Data Model Changes

```sql
-- Phase 1
CREATE TABLE Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(60) NOT NULL,
    Description NVARCHAR(300) NULL,
    CreatedDate DATETIMEOFFSET NOT NULL,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    LastUpdatedDate DATETIMEOFFSET NOT NULL,
    LastUpdatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT UQ_Roles_Name UNIQUE (Name)
);

CREATE TABLE UserRoles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    GrantedByUserId UNIQUEIDENTIFIER NOT NULL,
    GrantedDate DATETIMEOFFSET NOT NULL,
    ExpiryDate DATETIMEOFFSET NULL,
    RevokedDate DATETIMEOFFSET NULL,
    RevokedByUserId UNIQUEIDENTIFIER NULL,
    RevokedReason NVARCHAR(500) NULL,
    -- standard audit
    CreatedDate DATETIMEOFFSET NOT NULL,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    LastUpdatedDate DATETIMEOFFSET NOT NULL,
    LastUpdatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT FK_UserRoles_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_UserRoles_Role FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
-- Unique constraint on active grants (partial index; SQL Server supports filtered indexes)
CREATE UNIQUE INDEX UX_UserRoles_Active
    ON UserRoles(UserId, RoleId)
    WHERE RevokedDate IS NULL;
CREATE INDEX IX_UserRoles_RoleId ON UserRoles(RoleId) WHERE RevokedDate IS NULL;
CREATE INDEX IX_UserRoles_UserId ON UserRoles(UserId) WHERE RevokedDate IS NULL;

-- Backfill: existing SiteAdmins
INSERT INTO UserRoles (Id, UserId, RoleId, GrantedByUserId, GrantedDate, CreatedDate, CreatedByUserId, LastUpdatedDate, LastUpdatedByUserId)
SELECT NEWID(), u.Id, r.Id, '<system-user-guid>', SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), '<system-user-guid>', SYSDATETIMEOFFSET(), '<system-user-guid>'
FROM Users u
CROSS JOIN Roles r
WHERE u.IsSiteAdmin = 1 AND r.Name = 'SiteAdmin';

-- Phase 4 (30 days after Phase 2 stable)
ALTER TABLE Users DROP COLUMN IsSiteAdmin;
```

### API Changes

```csharp
// Phase 1 — service surface
public interface IUserRoleService
{
    Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken ct);
    Task<IReadOnlyCollection<string>> GetRoleNamesAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyCollection<User>> GetUsersInRoleAsync(string roleName, CancellationToken ct);
}

// Phase 2 — authorization handler shape
public class UserIsAdminAuthHandler(IUserRoleService roles) : AuthorizationHandler<UserIsAdminRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext ctx, UserIsAdminRequirement req)
    {
        var user = await ResolveUser(ctx);
        if (user is null) return;
        if (await roles.HasRoleAsync(user.Id, RoleNames.SiteAdmin, ctx.HttpContext.RequestAborted))
        {
            ctx.Succeed(req);
        }
    }
}

// Phase 3 — role admin endpoints
[Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
public class RolesV2Controller(...) : ControllerBase
{
    [HttpGet] public Task<ActionResult<IEnumerable<RoleDto>>> ListRoles(...);
    [HttpGet("{roleName}/members")] public Task<...> ListMembers(...);
    [HttpPost("~/api/v2/users/{userId}/roles")] public Task<...> GrantRole(Guid userId, [FromBody] GrantRoleRequest req, ...);
    [HttpDelete("~/api/v2/users/{userId}/roles/{roleName}")] public Task<...> RevokeRole(Guid userId, string roleName, ...);
}
```

### Web UX Changes

- **`/siteadmin/roles`** — DataTable of roles, columns: name, description, member count, actions
- **`/siteadmin/roles/{roleName}`** — role detail with member list
- **`/siteadmin/users/{userId}/roles`** — per-user roles with grant/revoke controls
- **`/siteadmin/users`** list — add a "Roles" column so the current tier of every user is visible at a glance

### Mobile App Changes

None. Roles gate site-admin surface only; the mobile app doesn't expose any of the affected endpoints.

---

## Implementation Phases

### Phase 1 — Data model + read-side layering (~1 week)

Ships first with **no behavior change** for anyone. Once merged and running for a few days, we know the read path is stable.

### Phase 2 — Handler migration + SalesRep role (~1 week)

Ships when Phase 1 has soaked. Unblocks Project 63 immediately.

### Phase 3 — Admin UI (~1 week)

Ships alongside or shortly after Phase 2. Not strictly required for Project 63 (roles can be granted via SQL script if the UI slips), but strongly preferred.

### Phase 4 — Deprecate the boolean (~1 week, ships 30+ days after Phase 2 stable)

Sweep the codebase, drop the column, close the loop.

**Note:** Phases are sequential but not time-bound.

---

## Open Questions

1. **What roles do we seed on day 1?**
   **Recommendation:** Just `SiteAdmin` and `SalesRep`. Don't pre-seed speculative roles — they'll rot before they're used. Every new role is a migration; that's a good friction.
   **Owner:** Joe + Cynthia
   **Due:** Phase 1 kickoff

2. **Should role grants be self-service revocable, or admin-only?**
   **Recommendation:** Admin-only for grant *and* revoke. A user can't revoke their own SiteAdmin role (fat-finger prevention). If someone truly needs to step down, they ask another admin.
   **Owner:** Joe
   **Due:** Phase 3 kickoff

3. **Do we need role hierarchy (SiteAdmin implicitly has SalesRep capabilities)?**
   **Recommendation:** No — the auth policies already handle this via the "SalesRep OR SiteAdmin" pattern (`UserIsSalesRepOrIsAdmin`). Explicit hierarchy adds accidental-privilege risk. Stick with policy composition.
   **Owner:** Joe
   **Due:** Phase 2 kickoff

4. **How do we handle the system user id for the backfill grant?**
   **Recommendation:** There's a well-known "System" user id used elsewhere for automated writes; use the same one. Document it in `Constants.cs` if not already.
   **Owner:** Joe
   **Due:** Phase 1 migration

5. **Which policies does the new `SalesRep` role satisfy?**
   **Recommendation:** Introduce two policies specifically for prospect/report access: `UserIsSalesRepOrIsAdmin` (read + write on prospect and sales-report endpoints only) and reuse `ValidUser` for anything a normal authenticated user can do. The salesperson should not implicitly gain any other capability.
   **Owner:** Joe + salesperson (once hired) via a scope-check exercise
   **Due:** Phase 2 kickoff

6. **Do role changes trigger a re-issue of the caller's token, or does the change take effect on the next request only?**
   **Recommendation:** Next request only. Re-issuing tokens is a bigger surface (and would need Entra External ID coordination). Documented so the admin UI shows a "changes take effect on next login or next API call" note.
   **Owner:** Joe
   **Due:** Phase 3

---

## Related Documents

- **[Project 63 - Municipal Sales Pipeline Reporting](./Project_63_Municipal_Sales_Pipeline_Reporting.md)** — primary consumer of the `SalesRep` role
- **[Project 62 - TrashMob Site & Codebase Re-evaluation](./Project_62_TrashMob_Site_and_Codebase_Reevaluation.md)** — the `UsersV2Controller` anonymous-read leak flagged in the audit gets fixed here as a side-effect
- **[Project 1 - Auth Revamp](./Project_01_Auth_Revamp.md)** — modernized *authentication* (Entra External ID); this project modernizes *authorization*. Adjacent, not blocking.

---

**Last Updated:** 2026-07-05
**Owner:** Joe (engineering)
**Status:** In Progress — Phases 1–4a shipped; Phase 4b (column drop) queued for ~2026-08-05
**Next Review:** 2026-08-05 — decide whether the 30-day observation window is enough to drop `IsSiteAdmin`; if any missed reader has caused a production incident since 4a, extend the window and grep the codebase again first
