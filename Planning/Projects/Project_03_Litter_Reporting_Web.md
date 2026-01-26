# Project 3 — Complete Litter Reporting (Web Parity)

| Attribute | Value |
|-----------|-------|
| **Status** | Ready for Team Review |
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
- ? Litter reports list page with filters
- ? Litter report detail page
- ? Map integration (pins for report locations)
- ? Toggle to show/hide reports on main map
- ? Legend explaining pin colors/statuses

### Phase 2 - Creation
- ? Create litter report form
- ? Photo upload (single or multiple)
- ? Location picker (map + search)
- ? Description and severity fields
- ? Submit and success confirmation

### Phase 3 - Management
- ? Edit existing reports (creator only)
- ? Change status (New ? Assigned ? Cleaned ? Cancelled)
- ? Associate with events
- ? Admin moderation tools

### Phase 4 - Integration
- ? Weekly digest emails for new reports
- ? Dashboard widget showing reports in user's area
- ? Event creation from litter report (pre-populate)
- ? Notifications when report is resolved

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

**No database changes required.** Existing schema supports all features:

```sql
-- Existing tables (for reference)
LitterReports (Id, Name, Description, Status, CreatedBy, CreatedDate, ...)
LitterImages (Id, LitterReportId, ImageUrl, Latitude, Longitude, ...)
EventLitterReports (EventId, LitterReportId) -- Association table
```

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

### Phase 6: Testing & Launch
- User acceptance testing
- Accessibility audit (WCAG 2.2 AA)
- Performance testing
- Staged rollout (25% ? 50% ? 100%)

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

1. **Should litter reports be public or private by default?**  
   **Recommendation:** Public by default (address shown, creator name hidden for privacy)  
   **Owner:** Product Lead  
   **Due:** Before Phase 1

2. **How long should we retain "Cleaned" reports before archiving?**  
   **Recommendation:** Keep indefinitely for historical data, but exclude from default map view after 90 days  
   **Owner:** Product Lead  
   **Due:** Before Phase 2

3. **Should users be able to vote or comment on report severity?**  
   **Recommendation:** No for MVP; future feature if requested  
   **Owner:** Product Lead  
   **Due:** N/A

4. **What's the moderation workflow for inappropriate images?**  
   **Recommendation:** Admin review queue, flagging, deletion with email notice  
   **Owner:** Product Lead + Admin  
   **Due:** Before Phase 5

---

## Related Documents

- **[Project 2 - Home Page](./Project_02_Home_Page.md)** - Litter report quick action
- **[PRD Section: Litter Reporting](../../TrashMob/TrashMob.prd#3-litter-reporting)** - User stories
- **Backend API:** `LitterReportController.cs`, `LitterImageManager.cs`
- **Mobile Reference:** TrashMobMobile litter reporting screens

---

**Last Updated:** January 24, 2026  
**Owner:** Web Product Lead + UX Designer  
**Status:** Ready for Team Review  
**Next Review:** When development begins
