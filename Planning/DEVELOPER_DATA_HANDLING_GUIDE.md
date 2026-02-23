# Developer Data Handling Guide

| Attribute | Value |
|-----------|-------|
| **Last Updated** | February 23, 2026 |
| **Owner** | Engineering |
| **Audience** | Developers working on features that touch personal data |

---

This guide is the internal reference for anyone building or modifying features that store, display, or process personal data in TrashMob. Follow it whenever you add a new entity, field, integration, or photo type that involves user information.

---

## 1. PII Checklist (Before You Code)

Before adding any new data field or feature, answer every question below. If any answer is unclear, discuss with the team before writing code.

| # | Question | Why It Matters |
|---|----------|----------------|
| 1 | **Does this field contain PII?** Names, emails, dates of birth, locations tied to a person, photos, IP addresses, and device identifiers are all PII. | Determines whether deletion, export, and privacy rules apply. |
| 2 | **Is it necessary? Can the feature work without it?** | Data minimization is a GDPR core principle. Collect only what you need. |
| 3 | **What is the legal basis?** (consent, legitimate interest, legal obligation) | Must be documented in `Planning/DATA_PROCESSING_INVENTORY.md`. |
| 4 | **Who can access it?** (public, authenticated users, event leads, admins, the user themselves) | Drives authorization checks in controllers and display filtering in the frontend. |
| 5 | **Is it handled by `UserDeletionService`?** | Every PII field must be deleted or anonymized when a user exercises their right to erasure. |
| 6 | **Is it included in `UserDataExportManager`?** | Every PII field must be returned when a user exercises their right to data portability. |
| 7 | **Does it need minor-specific handling?** | Minors (13-17) have stricter display rules. See `Planning/Cross_Cutting_Minor_Privacy_Standards.md`. |

If you answered "yes" to question 1, you must complete **all** of the steps in sections 2-3 below and update the files listed in section 8.

---

## 2. Adding a New Entity That References Users

When you create or modify an entity that has a `UserId`, `CreatedByUserId`, or any other user-referencing foreign key, you must update the deletion pipeline, the data export, and the processing inventory.

### Step 1: Update `UserDeletionService.cs`

**File:** `TrashMob.Shared/Managers/UserDeletionService.cs`

The service runs a 7-phase transactional pipeline (A through H). Determine which phase applies to your entity:

| Phase | Method | When to Use | What Happens |
|-------|--------|-------------|--------------|
| **A** | `DeleteUserSpecificRecordsAsync` | The record has **no value** after the user is deleted (e.g., notifications, attendance sign-ups, membership records). | `ExecuteDeleteAsync` removes rows. |
| **B** | `AnonymizeRequiredUserIdFieldsAsync` | The record has **aggregate or historical value** that should survive deletion (e.g., route data for heatmaps, attendee metrics for event totals, moderation audit trails). The `UserId` column is required (non-nullable). | `ExecuteUpdateAsync` sets `UserId` to `Guid.Empty` (the anonymous user). |
| **C** | `AnonymizePhotoReferencesAsync` | The record is a **photo entity** with `UploadedByUserId`, `ReviewRequestedByUserId`, or `ModeratedByUserId` columns. | Anonymizes upload/review/moderation user references while preserving the photo itself. |
| **D** | `CleanUpNullableUserReferencesAsync` | The record has a **nullable** `UserId` or `ReviewedByUserId` field. | Sets the nullable FK to `null`. |
| **E** | `AnonymizeWaiverRecordsAsync` | The record is a **waiver** that must be retained for legal compliance. | Anonymizes user link; never deletes the waiver record. |
| **F** | `AnonymizeAllAuditFieldsAsync` | **Always add your entity here** if it inherits from `BaseModel`. This handles `CreatedByUserId` and `LastUpdatedByUserId`. | Generic `AnonymizeAuditFieldsAsync<T>` sets audit user IDs to `Guid.Empty`. |
| **G** | `HandleTeamLeadTransferAsync` | Only for team lead membership changes. You should not need to modify this unless changing team structure. | Transfers lead role to the longest-tenured remaining member. |
| **H** | User record deletion | Automatic (final step). Do not modify. | `context.Users.Remove(user)` cascades per EF configuration. |

**Typical steps for a new entity with a required `UserId`:**

1. Add deletion or anonymization logic in the appropriate phase method (A or B).
2. Add `await AnonymizeAuditFieldsAsync<YourEntity>(userId, ct);` in `AnonymizeAllAuditFieldsAsync` (Phase F).

**Typical steps for a new entity with only audit fields (no direct `UserId`):**

1. Add `await AnonymizeAuditFieldsAsync<YourEntity>(userId, ct);` in `AnonymizeAllAuditFieldsAsync` (Phase F). That is the only change needed.

### Step 2: Update `UserDataExportManager.cs`

**File:** `TrashMob.Shared/Managers/UserDataExportManager.cs`

The export manager streams JSON to the response. It currently exports 13 data categories:

1. Profile
2. Event Participation
3. Events Led
4. Event Summaries
5. Route Data
6. Attendee Metrics
7. Litter Reports
8. Team Memberships
9. Achievements
10. Waivers
11. Feedback
12. Partner Admin Roles
13. Notification Preferences

To add a new category:

1. Create a new `record` type for the exported shape (keep it flat; exclude internal IDs the user does not need).
2. Add a `WriteYourCategoryAsync` method following the existing pattern:
   ```csharp
   private async Task WriteYourCategoryAsync(Utf8JsonWriter writer, Guid userId, CancellationToken ct)
   {
       var items = await context.YourEntities
           .AsNoTracking()
           .Where(x => x.UserId == userId)
           .Select(x => new ExportedYourCategory(x.Field1, x.Field2))
           .ToListAsync(ct);

       writer.WritePropertyName("yourCategory");
       JsonSerializer.Serialize(writer, items);
       await writer.FlushAsync(ct);
   }
   ```
3. Call the new method from `WriteExportToStreamAsync`, before the final `writer.WriteEndObject()`.

### Step 3: Update Data Processing Inventory

**File:** `Planning/DATA_PROCESSING_INVENTORY.md`

Add a row for the new data category with:
- Data type and description
- Legal basis (consent, legitimate interest, legal obligation)
- Retention period
- Who has access
- Whether it is included in deletion and export

---

## 3. Audit Field Pattern

All entities inheriting from `BaseModel` (`TrashMob.Models/BaseModel.cs`) automatically get four audit fields:

| Field | Type | Description |
|-------|------|-------------|
| `CreatedByUserId` | `Guid` | User who created the record |
| `CreatedDate` | `DateTimeOffset?` | When the record was created |
| `LastUpdatedByUserId` | `Guid` | User who last modified the record |
| `LastUpdatedDate` | `DateTimeOffset?` | When the record was last modified |

These are **automatically anonymized** in Phase F of `UserDeletionService` via the generic helper:

```csharp
private async Task AnonymizeAuditFieldsAsync<T>(Guid userId, CancellationToken ct)
    where T : BaseModel
{
    await context.Set<T>()
        .Where(e => e.CreatedByUserId == userId || e.LastUpdatedByUserId == userId)
        .ExecuteUpdateAsync(s => s
            .SetProperty(e => e.CreatedByUserId, e => e.CreatedByUserId == userId ? AnonymousUserId : e.CreatedByUserId)
            .SetProperty(e => e.LastUpdatedByUserId, e => e.LastUpdatedByUserId == userId ? AnonymousUserId : e.LastUpdatedByUserId),
            ct);
}
```

**When adding a new `BaseModel` entity:** Add a line to `AnonymizeAllAuditFieldsAsync` in `UserDeletionService.cs`:

```csharp
await AnonymizeAuditFieldsAsync<YourNewEntity>(userId, ct);
```

Forgetting this step means that `CreatedByUserId` / `LastUpdatedByUserId` will still reference the deleted user, which is a privacy violation.

---

## 4. Photo Handling

Photos have special privacy considerations because they may contain faces and are stored in Azure Blob Storage.

| Rule | Details |
|------|---------|
| **Moderation pipeline** | All user-uploaded photos go through moderation (auto-review + optional manual review). |
| **Phase C anonymization** | `UploadedByUserId`, `ReviewRequestedByUserId`, and `ModeratedByUserId` are anonymized during deletion. This applies to `EventPhoto`, `PartnerPhoto`, and `TeamPhoto`. |
| **Profile photos** | Deleted from blob storage when the user record is removed (Phase H cascade). |
| **Event / Partner / Team photos** | Photo blobs are **preserved** after the uploader is deleted because they have community value. Only the user reference is anonymized. |
| **Litter images** | `ReviewRequestedByUserId` and `ModeratedByUserId` are nulled in Phase D. The image itself is preserved. |

**When adding a new photo entity type:**

1. Add anonymization logic in `AnonymizePhotoReferencesAsync` (Phase C).
2. Configure cascade behavior in `MobDbContext.cs` (`TrashMob.Shared/Persistence/MobDbContext.cs`).
3. Decide: should the blob be deleted with the user (like profile photos) or preserved (like event photos)?
4. Add to `AnonymizeAllAuditFieldsAsync` (Phase F) for the `BaseModel` audit fields.

---

## 5. Location Data

Location data varies in sensitivity depending on context:

| Data Type | On User Deletion | Rationale |
|-----------|-----------------|-----------|
| **Profile location** (city, region, lat/lng on `User`) | Deleted with the user record (Phase H). | Tied directly to a person's home area. |
| **Event locations** | Preserved. | Public data; the event continues to be visible regardless of who created it. |
| **Route GPS data** (`EventAttendeeRoute.UserPath`) | User link anonymized (Phase B); geometry preserved. | Anonymized routes still power community heatmaps. The GPS path alone, without a user ID, is not PII. |
| **Litter report locations** | Preserved. | Public data; the report location is community information. |

**Key point:** Never store precise home addresses. The profile location fields (`City`, `Region`, `Latitude`, `Longitude`) are approximate and used for event matching, not street-level identification.

---

## 6. Third-Party Integration Checklist

When integrating a new third-party service that sends or receives user data:

| Step | File to Update | Details |
|------|---------------|---------|
| 1 | `Planning/DATA_PROCESSING_INVENTORY.md` | Document what data is sent, the legal basis, and the vendor's role (processor vs. controller). |
| 2 | `Planning/PRIVACY_MANIFEST.md` | Add the SDK to the third-party SDK table. If it is a mobile SDK, add any new API categories to the iOS privacy manifest section and any new permissions to the Android section. |
| 3 | Vendor DPA | Verify the vendor has a GDPR-compliant Data Processing Agreement. Keep a record of the DPA. |
| 4 | `TrashMob/client-app/src/pages/privacypolicy/index.tsx` | Update the privacy policy page if the integration creates a new user-facing data flow (e.g., sharing email with a new email provider, sending location to a new mapping service). |
| 5 | Store declarations | If the integration collects new data types on mobile, update the Apple App Store privacy nutrition label and Google Play Data Safety form (see `Planning/PRIVACY_MANIFEST.md` section 6 for URLs). |

**Current third-party services** (as of February 2026):

| Service | Data Sent |
|---------|-----------|
| Sentry.io | Crash logs, device info |
| Azure Maps | Location coordinates |
| SendGrid | Email addresses |
| MSAL (Microsoft Entra) | Auth tokens, user identifiers |
| Google Maps (Android) | Location coordinates |

---

## 7. Testing Data Privacy

### Run Existing Tests

The existing test suite in `TrashMob.Shared.Tests/Managers/UserManagerTests.cs` verifies the deletion pipeline through mocked `IUserDeletionService` calls. Run the tests before and after changes:

```bash
dotnet test TrashMob.Shared.Tests/
```

### For New Entities: Write a Deletion Test

Every new entity that stores PII needs a test verifying the full cycle:

1. **Create** test data (user + entity referencing the user).
2. **Delete** the user via `UserDeletionService.DeleteUserDataAsync`.
3. **Verify** no PII remains:
   - If Phase A: the row should no longer exist.
   - If Phase B/C/D/E: the row should exist but user references should be `Guid.Empty` or `null`.
   - Phase F: `CreatedByUserId` and `LastUpdatedByUserId` should be `Guid.Empty`.

### Test Data Export

After adding a new export category:

1. Create test data for the new entity.
2. Call `UserDataExportManager.WriteExportToStreamAsync`.
3. Deserialize the JSON output and verify the new category is present and contains the expected data.

### Minor Privacy Testing

If the new entity displays user-visible data, follow the testing checklist in `Planning/Cross_Cutting_Minor_Privacy_Standards.md`:

- Verify minors are excluded from public-facing lists and displays.
- Verify guardian consent is checked before showing minor photos.
- Verify minor names are masked appropriately by context (public, team, admin).
- Test with a minor user account.

---

## 8. Quick Reference Table

| I need to... | Update these files |
|---|---|
| Add new entity with `UserId` | `TrashMob.Shared/Managers/UserDeletionService.cs` (appropriate phase + Phase F), `TrashMob.Shared/Managers/UserDataExportManager.cs`, `Planning/DATA_PROCESSING_INVENTORY.md` |
| Add new entity with only audit fields | `TrashMob.Shared/Managers/UserDeletionService.cs` (Phase F only) |
| Add new photo type | `TrashMob.Shared/Managers/UserDeletionService.cs` (Phase C + Phase F), `TrashMob.Shared/Persistence/MobDbContext.cs` (cascade config) |
| Add new third-party SDK | `Planning/PRIVACY_MANIFEST.md`, `Planning/DATA_PROCESSING_INVENTORY.md`, `TrashMob/client-app/src/pages/privacypolicy/index.tsx` |
| Change data retention | `Planning/DATA_PROCESSING_INVENTORY.md`, `TrashMob/client-app/src/pages/privacypolicy/index.tsx` |
| Add minor-visible data | `Planning/Cross_Cutting_Minor_Privacy_Standards.md`, apply display rules in frontend components |
| Add new waiver type | `TrashMob.Shared/Managers/UserDeletionService.cs` (Phase E), `TrashMob.Shared/Managers/UserDataExportManager.cs` |

---

## Key File Locations

| File | Path | Purpose |
|------|------|---------|
| UserDeletionService | `TrashMob.Shared/Managers/UserDeletionService.cs` | 7-phase deletion and anonymization pipeline |
| UserDataExportManager | `TrashMob.Shared/Managers/UserDataExportManager.cs` | Streaming JSON export of all user data (13 categories) |
| BaseModel | `TrashMob.Models/BaseModel.cs` | Audit fields inherited by all entities |
| MobDbContext | `TrashMob.Shared/Persistence/MobDbContext.cs` | EF Core entity configuration and cascade rules |
| Privacy Manifest | `Planning/PRIVACY_MANIFEST.md` | App store privacy declarations and SDK inventory |
| Minor Privacy Standards | `Planning/Cross_Cutting_Minor_Privacy_Standards.md` | Display rules for users aged 13-17 |
| Privacy Policy (frontend) | `TrashMob/client-app/src/pages/privacypolicy/index.tsx` | User-facing privacy policy page |
| Deletion Tests | `TrashMob.Shared.Tests/Managers/UserManagerTests.cs` | Unit tests for user deletion flow |
