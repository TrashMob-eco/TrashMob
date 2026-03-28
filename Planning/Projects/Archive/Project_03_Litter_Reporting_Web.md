# Project 3 � Complete Litter Reporting (Web Parity)

| Attribute | Value |
|-----------|-------|
| **Status** | Complete |
| **Priority** | Moderate |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Backend API complete |

---

## Business Rationale

Achieve feature parity between mobile and web for litter reporting. Currently, users can create and view litter reports on mobile apps, but the web experience is incomplete. Completing web support will enable desktop users to report litter, increase report submissions, and integrate litter reports throughout the platform (maps, events, emails, dashboards).

---

## Objectives

### Primary Goals
- **Map integration** with litter report pins, legends, and toggles
- **List and detail views** for browsing and viewing reports
- **Create and edit flows** for web users to submit reports
- **Event association** linking reports to cleanup events
- **Email notifications** for new reports in user's area

### Secondary Goals
- Photo upload with drag-and-drop
- Bulk status updates for admins
- Export reports to CSV/Excel

---

## Scope

### Phase 1 - Viewing
- ✅ Litter reports list page with filters
- ✅ Litter report detail page
- ✅ Map integration (pins for report locations) - PR #2437
- ✅ Toggle to show/hide reports on main map - PR #2437
- ✅ Legend explaining pin colors/statuses - PR #2437
- ✅ "Litter Reports" link in main navigation - PR #2437

### Phase 2 - Creation
- ✅ Create litter report form - PR #2442
- ✅ Photo upload (single or multiple) - PR #2442
- ✅ Location picker (map + search) - PR #2442
- ✅ Description and severity fields - PR #2442
- ✅ Submit and success confirmation - PR #2442

### Phase 3 - Management
- ✅ Edit existing reports (creator or admin) - PR #2439
- ✅ Delete existing reports (creator or admin) - PR #2439
- ✅ Change status (New → Assigned → Cleaned → Cancelled) - PR #2439
- ✅ Associate with events - PR #2445
- ✅ Admin moderation tools - PR #2455

### Phase 4 - Integration
- ✅ Weekly digest emails for new reports - PR #2451
- ✅ Dashboard widget showing reports in user's area - PR #2451
- ✅ Event creation from litter report (pre-populate) - PR #2451
- ✅ Notifications when report is resolved - PR #2451

### Phase 5 - MyDashboard Integration
- ✅ "My Litter Reports" section on MyDashboard page
- ✅ List of user's submitted reports with status
- ✅ Edit/delete actions for user's own reports - PR #2439
- ✅ Litter report count in user's impact metrics - PR #2438

### Phase 6 - Event Summary Integration
- ✅ Show associated litter reports on Event Summary page - PR #2445
- ✅ Mark associated litter reports as "Cleaned" when submitting summary - PR #2445
- ✅ Add existing litter report to event (cleaned during event but not originally associated) - PR #2445
- ✅ Remove litter report association (not cleaned during this event) - PR #2445
- ✅ Update report status automatically when event summary submitted - PR #2452

---

## Out-of-Scope

- ? Real-time collaboration on reports
- ? Public commenting on reports (privacy concerns)
- ? Gamification (badges for reporters)
- ? ML/AI for severity prediction
- ? Integration with municipal 311 systems (future)

---

## Success Metrics

### Quantitative
- **Web-submitted reports:** ? 30% of total reports within 3 months
- **Events associated with reports:** ? 40% of events link to a report
- **Weekly digest open rate:** ? 25%
- **Report edit error rate:** ? 5%
- **Time to create report (web):** ? 3 minutes

### Qualitative
- Positive user feedback on ease of use
- Event leads report reports are helpful for planning
- Low support tickets related to litter reporting

---

## Dependencies

### Completed (No Blockers)
- ? Backend API complete (`LitterReportController`, `LitterImageManager`)
- ? Database schema (`LitterReports`, `LitterImages` tables)
- ? Mobile app litter reporting (reference implementation)

### Enables
- **Project 2 (Home Page):** Litter report quick action
- **Event creation workflow:** Pre-populate from report

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Photo upload UX issues** | Medium | Medium | User testing, progressive enhancement, drag-and-drop with fallback |
| **Location accuracy on web** | Medium | Low | Use browser geolocation API with manual override |
| **Spam or inappropriate reports** | Low | Medium | Admin moderation queue, automated flagging, rate limiting |
| **Privacy concerns** | Low | High | Don't expose reporter identity publicly, allow anonymous reports |

---

## Implementation Plan

### Data Model Changes

**Data Model Pattern: Report as Container, Images as Map Points**

The `LitterReport` is a container (description, status, creator) with NO primary location. Each `LitterImage` has its own geotag and appears as an individual pin on the map. This allows users to document multiple spots across an area without re-entering description info.

```sql
-- Existing tables (clarified responsibilities)
LitterReports (Id, Name, Description, Status, CreatedBy, CreatedDate, ...)
    -- NO Latitude/Longitude on report itself
    -- Centroid can be computed from child images for list views

LitterImages (Id, LitterReportId, ImageUrl, Latitude, Longitude, InReview, ReviewRequestedByUserId, ...)
    -- Each image IS a map point
    -- Latitude/Longitude required for map display
    -- InReview: boolean, true when flagged by user (hidden from display)
    -- ReviewRequestedByUserId: Guid?, user who flagged the image

EventLitterReports (EventId, LitterReportId) -- Association table
```

**Map Display Logic:**
- Show each `LitterImage` as a pin (colored by parent report status)
- Cluster pins by parent `LitterReportId` when zoomed out
- Clicking any pin opens the parent report detail view

### API Changes

**No new API endpoints required.** Existing endpoints support web:

```csharp
// Already implemented in LitterReportController
[HttpGet] GetLitterReports(filters)
[HttpGet("{id}")] GetLitterReport(id)
[HttpPost] CreateLitterReport(request)
[HttpPut("{id}")] UpdateLitterReport(id, request)
[HttpDelete("{id}")] DeleteLitterReport(id)

// Image management (LitterImageController - if exists, or part of above)
[HttpPost("{reportId}/images")] UploadImage(reportId, file)
[HttpDelete("images/{imageId}")] DeleteImage(imageId)
```

**Minor enhancement (optional):**
- Add query parameter for including images: `?includeImages=true`

### Web UX Changes

**New Pages:**

1. **`/litter-reports`** (List View)
   ```tsx
   // Components
   - LitterReportsList (table or card grid)
   - LitterReportFilters (status, date range, location)
   - LitterReportsMap (toggle between list and map)
   ```

2. **`/litter-reports/:id`** (Detail View)
   ```tsx
   // Components
   - LitterReportHeader (name, status, date)
   - LitterReportImages (gallery with lightbox)
   - LitterReportMap (shows all image locations)
   - LitterReportDetails (description, severity, creator)
   - LitterReportActions (edit, associate with event, resolve)
   ```

3. **`/litter-reports/create`** (Create Form)
   ```tsx
   // Components
   - LitterReportForm
     - NameInput
     - DescriptionTextArea
     - LocationPicker (map + search)
     - PhotoUpload (drag-drop + click)
     - SeveritySelect (optional)
     - SubmitButton
   ```

4. **`/litter-reports/:id/edit`** (Edit Form)
   - Similar to create, pre-populated

**Map Integration:**

```tsx
// Add to existing EventsMap component
<MapToggle
  label="Show Litter Reports"
  checked={showLitterReports}
  onChange={setShowLitterReports}
/>

{showLitterReports && (
  <LitterReportMarkers
    reports={litterReports}
    onMarkerClick={handleReportClick}
  />
)}
```

**Litter Report Pins:**
- ?? Red pin: New (unassigned)
- ?? Yellow pin: Assigned to event
- ?? Green pin: Cleaned/Resolved
- ? Gray pin: Cancelled

**Photo Upload Component:**

```tsx
<PhotoUpload
  maxFiles={10}
  maxSizeMB={5}
  accept="image/*"
  onUpload={handlePhotosUpload}
  enableGeotag={true}
/>
```

**MyDashboard Integration:**

Add to existing MyDashboard page:
```tsx
// My Litter Reports section
<DashboardSection title="My Litter Reports">
  <LitterReportsList
    reports={userReports}
    showEditActions={true}
    onEdit={handleEditReport}
    onDelete={handleDeleteReport}
  />
</DashboardSection>

// Add to impact metrics
<ImpactMetric label="Litter Reports Filed" value={user.litterReportCount} />
```

**Event Summary Integration:**

Add to Event Summary page:
```tsx
// Associated litter reports section
<EventSummarySection title="Litter Reports Cleaned">
  <AssociatedLitterReports
    eventId={eventId}
    reports={associatedReports}
    onMarkCleaned={handleMarkCleaned}
    onRemoveAssociation={handleRemoveAssociation}
  />
  <AddLitterReportButton
    onAdd={handleAddExistingReport}
    label="Add report cleaned during this event"
  />
</EventSummarySection>
```

### Mobile App Changes

No changes required (already implemented).

---

## Implementation Phases

### Phase 1: List and Detail Views
- Implement list page with filters
- Implement detail page with image gallery
- Connect to existing API

### Phase 2: Map Integration
- Add litter report pins to main map
- Implement toggle controls
- Add legend for pin colors

### Phase 3: Create/Edit Forms
- Build form UI
- Implement photo upload with drag-drop
- Location picker with map
- Form validation

### Phase 4: Event Association & Management
- Link reports to events (bidirectional)
- Status update workflows
- Admin moderation tools

### Phase 5: Email Integration
- Weekly digest template
- Notification triggers (new report, status change)
- Unsubscribe preferences

### Phase 6: MyDashboard Integration
- Add "My Litter Reports" section to MyDashboard
- Display user's reports with status and edit actions
- Add litter report count to user's impact metrics

### Phase 7: Event Summary Integration
- Display associated litter reports on Event Summary page
- Add controls to mark reports as cleaned
- Add ability to associate additional reports cleaned during event
- Add ability to remove report associations
- Auto-update report status on summary submission

### Phase 8: Testing & Launch
- User acceptance testing
- Accessibility audit (WCAG 2.2 AA)
- Performance testing
- Staged rollout

---

## User Flows

### Flow 1: Create Litter Report (Desktop User)

1. User clicks "Report Litter" on home page
2. Navigates to `/litter-reports/create`
3. Fills in report name and description
4. Clicks map to set location (or uses search)
5. Uploads 1-5 photos (drag-drop or click)
6. Browser prompts for location permission (optional)
7. Reviews and submits
8. Sees success message with link to report
9. Receives confirmation email

### Flow 2: Event Lead Uses Report

1. Event lead browses litter reports on map
2. Finds report near desired event location
3. Clicks "Create Event from This Report"
4. Event form pre-populates with report details
5. Adjusts date/time and saves
6. Report automatically associates with event
7. Report status changes to "Assigned"

### Flow 3: Weekly Digest Recipient

1. User receives weekly email digest
2. Email shows 3-5 new reports within 10 miles
3. User clicks on interesting report
4. Views report details on web
5. Decides to create event to clean it
6. Follows "Create Event" button

---

## Open Questions

1. **Data model clarification: Report as container, images as map points**
   **Decision:** LitterReport is a container (description, status, creator) with NO primary location. LitterImage entities each have their own geotag and appear as individual pins on the map. Map clusters images by parent report. This matches original intent: users submit one report with multiple geotagged photos across an area without re-entering description.
   **Status:** Decided
   **Owner:** Engineering Lead

2. **Should litter reports be public or private by default?**
   **Decision:** Litter reports are always public (creator name hidden for privacy)
   **Status:** Decided

2. **How long should we retain "Cleaned" reports before archiving?**
   **Decision:** Keep indefinitely for historical data, but exclude from default map view after 90 days
   **Status:** Decided

3. **Should users be able to vote or comment on report severity?**
   **Decision:** No voting or commenting on report severity
   **Status:** Decided

4. **What's the moderation workflow for inappropriate images?**
   **Decision:** Any user can report an inappropriate image. Flagged images are immediately tagged (InReview=true) and hidden from display. Email sent to TrashMob staff who can untag (false positive), delete, or report to authorities as needed. See Project 28 for full implementation details.
   **Data Model:** Add `InReview` (boolean) and `ReviewRequestedByUserId` (Guid?) to LitterImage entity. Full moderation admin page may come later, but these fields enable immediate flagging support.
   **Status:** Decided

5. ~~**Can users control notification radius for the weekly digest?**~~
   **Decision:** Already supported - user preference radius exists in the system
   **Status:** ✅ Resolved

6. ~~**Should we support truly anonymous (no login) litter reports?**~~
   **Decision:** No - require login; reporting litter is a user acquisition opportunity
   **Status:** ✅ Resolved

7. ~~**What map clustering algorithm and zoom thresholds should we use?**~~
   **Decision:** Use supercluster.js or similar; cluster at zoom levels 0-13, de-cluster at 14+; color cluster by majority status
   **Status:** ✅ Resolved

8. ~~**Can report status transition backward (e.g., Cleaned to New if litter returns)?**~~
   **Decision:** No backward transitions; if litter returns to a "Cleaned" location, user should create a new report
   **Status:** ✅ Resolved

---

## Related Documents

- **[Project 2 - Home Page](./Project_02_Home_Page.md)** - Litter report quick action
- **[Project 28 - Photo Moderation](./Project_28_Photo_Moderation.md)** - Image moderation workflow
- **[PRD Section: Litter Reporting](../../TrashMob/TrashMob.prd#3-litter-reporting)** - User stories
- **Backend API:** `LitterReportController.cs`, `LitterImageManager.cs`
- **Mobile Reference:** TrashMobMobile litter reporting screens

---

## Post-Completion Bug Fixes

- **PR #2722** (Feb 14, 2026): Fixed litter report creation 400 error
- **PR #2725** (Feb 14, 2026): Fixed litter report DB error and aligned image upload limit
- **PR #2743** (Feb 15, 2026): Fixed litter report images not displaying after upload
- **PR #2755** (Feb 15, 2026): Fixed litter report thumbnails, home feed loading, and button icon visibility

---

**Last Updated:** February 15, 2026
**Owner:** Web Product Lead + UX Designer
**Status:** Complete
**Completed:** January 31, 2026

---

## Implementation Progress

### Completed PRs
| PR | Description | Status |
|----|-------------|--------|
| [#2437](https://github.com/TrashMob-eco/TrashMob/pull/2437) | Add litter report pins to home page map with toggle and legend | Merged |
| [#2438](https://github.com/TrashMob-eco/TrashMob/pull/2438) | Add Litter Reports stat card to user dashboard | Merged |
| [#2439](https://github.com/TrashMob-eco/TrashMob/pull/2439) | Add edit and delete functionality for litter reports | Merged |
| [#2442](https://github.com/TrashMob-eco/TrashMob/pull/2442) | Add create litter report form with photo upload (Phase 2) | Merged |
| [#2445](https://github.com/TrashMob-eco/TrashMob/pull/2445) | Add litter report association to Event Summary page (Phase 6) | Merged |
| [#2451](https://github.com/TrashMob-eco/TrashMob/pull/2451) | Add Phase 4 integration features (Phase 4) | Merged |
| [#2452](https://github.com/TrashMob-eco/TrashMob/pull/2452) | Auto-update litter report status on event summary submission | Merged |
| [#2455](https://github.com/TrashMob-eco/TrashMob/pull/2455) | Add admin litter reports page (Phase 3 admin tools) | Pending |

### Summary
- **Phase 1 (Viewing):** ✅ Complete - List page, detail page, map integration with toggle/legend, navigation link
- **Phase 2 (Creation):** ✅ Complete - Create form, photo upload, location picker (PR #2442)
- **Phase 3 (Management):** ✅ Complete - Edit/delete/status/event association, admin moderation page (PR #2455)
- **Phase 4 (Integration):** ✅ Complete - Email notifications, dashboard widget, event creation from report (PR #2451)
- **Phase 5 (MyDashboard):** ✅ Complete - My reports section, edit/delete actions, stats metric
- **Phase 6 (Event Summary):** ✅ Complete - Show/add/remove litter reports, auto-update status (PRs #2445, #2452)
