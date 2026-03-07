# Project 55 — Event Check-In

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Medium |
| **Dependencies** | Project 8 (Waivers V3), Project 12 (In-App Messaging) |

---

## Business Rationale

Event leads currently have no way to confirm who actually showed up to a cleanup event. For legal compliance, it's critical to verify that every attendee has valid, current waivers before they participate — especially for minors, where a parent/guardian could have revoked access since the waiver was signed. A formal check-in process creates an auditable attendance record, validates waiver compliance in real-time, and gives event leads a clear roster of confirmed participants.

## Objectives

### Primary Goals

- **Pre-event waiver validation**: Before the event, verify that each registered attendee (and their dependents) has all required waivers signed, current, and unmodified since signing
- **Minor access verification**: Confirm that the parent/guardian hasn't revoked dependent access or modified consent since original waiver signing
- **Configurable notification timing**: Allow event leads (or system default) to configure when check-in notifications are sent — 15 minutes, 1 hour, 1 day, etc. before the event
- **Event lead attendance roster**: Provide event leads with a real-time list of checked-in attendees for on-site confirmation

### Secondary Goals

- **Compliance reporting**: Generate per-event compliance reports showing waiver status at time of check-in
- **No-show tracking**: Track registered attendees who don't check in for engagement analytics
- **Dependent check-in**: Allow parents to confirm which of their registered dependents are actually attending
- **On-site paper waiver fallback**: Support late arrivals who haven't completed digital waivers (integration with existing paper waiver upload from Project 8)

## Scope

### Pre-Event Check-In Flow

1. At configured time before event, system sends check-in notification to all registered attendees via in-app messaging (Project 12)
2. Attendee opens check-in prompt (in-app or push notification)
3. System validates:
   - All required waivers are signed and current (not expired, not superseded by newer version)
   - Waiver content hasn't changed since signing (if waiver was updated, re-signing required)
   - For dependents: parent/guardian hasn't revoked access, dependent is still active
   - For community events: community-specific waivers are also valid
4. If all valid: attendee confirms check-in, status updates to "Checked In"
5. If waiver issues: attendee is prompted to resolve (re-sign, update) before checking in
6. If minor access revoked: parent is notified that dependent cannot attend

### Event Lead Dashboard

- Real-time attendee roster showing: Name, Check-in Status, Waiver Status, Dependent Count
- Filter/sort by check-in status (Checked In, Pending, Waiver Issue, No Response)
- Ability to manually check in attendees (for on-site paper waiver scenarios)
- Headcount summary: Checked In / Registered / Capacity

### Check-In Timing Configuration

- System-wide default (configurable by admin, e.g., 1 hour before event)
- Event-level override (event lead can set custom timing for their event)
- Options: 15 minutes, 30 minutes, 1 hour, 2 hours, 4 hours, 1 day before event
- Reminder notification if not checked in by a secondary threshold (e.g., 15 min before if initial was 1 hour)

## Out-of-Scope

- GPS/geofencing-based automatic check-in (future enhancement)
- QR code scanning for check-in (future enhancement)
- Payment or fee collection at check-in
- Equipment/supply distribution tracking
- Post-event check-out

## Success Metrics

### Quantitative

- 80%+ of registered attendees complete check-in before event start
- Waiver compliance rate at check-in > 95%
- Average check-in completion time < 30 seconds (when waivers are current)
- Reduction in waiver-related compliance gaps to near zero

### Qualitative

- Event leads report confidence in attendee waiver compliance
- Legal team satisfied with audit trail for liability protection
- Parents/guardians feel informed about their dependent's participation status

## Dependencies

### Blockers

- **Project 8 (Waivers V3)**: Core waiver infrastructure (complete), minor waiver signing (pending legal approval for full minor flow, but adult + dependent waiver infrastructure is in place)
- **Project 12 (In-App Messaging)**: Required for sending check-in notifications to attendees. Without this, check-in would need to be email-only or purely manual.

### Enables

- Enhanced event summary accuracy (actual vs. registered attendance)
- Insurance/liability compliance reporting
- Future: automated capacity management and waitlist promotion when no-shows are detected

## Risks & Mitigations

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| Project 12 not ready | Can't send in-app notifications | Medium | Phase 1 can use email notifications as fallback; in-app messaging added when Project 12 ships |
| Attendees ignore check-in notifications | Low check-in rates | Medium | Send reminders; allow event leads to manually check in on-site; don't block event participation |
| Waiver version changes close to event | Mass re-signing required | Low | Admin warning when publishing new waiver version with upcoming events; grace period option |
| Network issues at event location | Can't check in digitally | Medium | Offline check-in support in mobile app (leverages Project 53 offline persistence); paper waiver fallback |
| Performance with large events | Slow waiver validation | Low | Batch waiver validation ahead of notification time; cache results |

## Implementation Plan

### Data Model Changes

**New Models:**

`EventCheckIn` — Records individual check-in status per attendee per event
- `Id` (Guid, PK)
- `EventId` (Guid, FK → Event)
- `UserId` (Guid, FK → User)
- `CheckInStatus` (enum: Pending, CheckedIn, WaiverIssue, NoResponse, ManualCheckIn)
- `CheckInTime` (DateTimeOffset?, when actually checked in)
- `WaiverValidatedAt` (DateTimeOffset?, when waiver validation last ran)
- `WaiverValidationResult` (enum: Valid, Expired, VersionMismatch, Revoked, Missing)
- `NotificationSentAt` (DateTimeOffset?, when check-in notification was sent)
- `CheckedInByUserId` (Guid?, for manual check-in by event lead)
- `Notes` (string?, for event lead comments)
- Audit fields (CreatedDate, LastUpdatedDate, etc.)

`EventCheckInDependent` — Records dependent check-in status
- `Id` (Guid, PK)
- `EventCheckInId` (Guid, FK → EventCheckIn)
- `DependentId` (Guid, FK → Dependent)
- `CheckInStatus` (enum, same as above)
- `WaiverValidationResult` (enum, same as above)
- Audit fields

`EventCheckInConfig` — Per-event check-in timing configuration
- `Id` (Guid, PK)
- `EventId` (Guid, FK → Event, unique)
- `NotificationLeadTimeMinutes` (int, default from system config)
- `ReminderLeadTimeMinutes` (int?, optional second reminder)
- `IsCheckInRequired` (bool, default true)
- Audit fields

**New Enums:**

`CheckInStatus`: Pending, CheckedIn, WaiverIssue, NoResponse, ManualCheckIn

`WaiverValidationResult`: Valid, Expired, VersionMismatch, Revoked, Missing, NotRequired

**System Configuration:**
- Default check-in notification lead time (site setting, e.g., 60 minutes)
- Default reminder lead time (site setting, e.g., 15 minutes)

### API Changes

**New Endpoints:**

`EventCheckInsController`:
- `GET /api/events/{eventId}/checkins` — Get all check-in records for an event (event lead only)
- `GET /api/events/{eventId}/checkins/summary` — Get check-in summary/headcount
- `POST /api/events/{eventId}/checkins` — Attendee self-check-in (validates waivers, creates/updates record)
- `PUT /api/events/{eventId}/checkins/{userId}/manual` — Event lead manual check-in
- `GET /api/events/{eventId}/checkins/my` — Get current user's check-in status for an event

`EventCheckInConfigController`:
- `GET /api/events/{eventId}/checkin-config` — Get check-in config for event
- `PUT /api/events/{eventId}/checkin-config` — Set/update check-in config (event lead only)

**Modified Endpoints:**
- `GET /api/events/{eventId}` — Include check-in config and user's check-in status in response

### Web UX Changes

**Event Detail Page (event lead view):**
- New "Check-In" tab or section showing attendee roster with check-in status
- Status badges: Checked In (green), Pending (yellow), Waiver Issue (red), No Response (gray)
- Manual check-in button per attendee
- Headcount bar: "12 / 18 checked in"
- Check-in config editor (notification timing dropdown)

**Event Detail Page (attendee view):**
- Check-in prompt/banner when check-in window opens
- Waiver status indicator with action to resolve issues
- Confirmation UI after successful check-in

### Mobile App Changes

**ViewEventPage:**
- Check-in banner/button when check-in window is active
- Waiver validation status display
- Dependent check-in selection (which dependents are actually coming)
- Offline check-in support (queue check-in action for sync when online)

**Event Lead View:**
- Attendee check-in roster on TabDetails or new tab
- Manual check-in capability
- Real-time headcount display

### Background Job Changes

**New Scheduled Job: `EventCheckInNotifier`**
- Runs every 5 minutes (or configurable interval)
- Queries upcoming events where notification time threshold has been reached
- Creates `EventCheckIn` records (Pending status) for all registered attendees
- Sends check-in notifications via in-app messaging (Project 12) or email fallback
- Runs waiver validation for each attendee and stores result
- Sends reminder notifications at secondary threshold

## Implementation Phases

### Phase 1 — Core Check-In Infrastructure
- Data models, migrations, repository, manager, controller
- Waiver validation service (checks all required waivers are current)
- Event lead check-in roster (web)
- Manual check-in by event lead
- Email-based check-in notifications (fallback until Project 12)

### Phase 2 — Attendee Self-Check-In
- Self-check-in flow (web + mobile)
- Waiver issue resolution prompts
- Dependent check-in selection
- Check-in config per event

### Phase 3 — Notifications & Automation
- Integration with Project 12 in-app messaging
- Background job for automated check-in notifications
- Configurable timing with reminders
- Push notifications for check-in prompts

### Phase 4 — Minor & Dependent Safety
- Parent/guardian access revocation check at check-in time
- Dependent waiver re-validation
- Parent notification when dependent is checked in
- Compliance reporting for minor attendance

### Phase 5 — Analytics & Reporting
- No-show tracking and engagement metrics
- Per-event compliance reports
- Historical check-in data for event summaries
- Integration with existing event summary flow

## Open Questions

1. Should check-in be mandatory or optional per event? (Recommend: configurable per event, default on)
2. What happens if an attendee arrives but hasn't checked in digitally? (Recommend: event lead can manually check in + handle paper waiver on-site)
3. Should we block event participation for attendees with waiver issues, or just flag it? (Recommend: flag and warn, don't block — event lead makes final call)
4. How long after event start should check-in remain open? (Recommend: until event end time, for late arrivals)
5. Should check-in data feed into the event summary (actual attendance vs. registered)? (Recommend: yes, Phase 5)
6. For the notification timing — should there be a system-wide default that event leads can override, or fully per-event? (Recommend: system default with per-event override)

## GitHub Issues

_None yet — will be created when implementation begins._

## Related Documents

- [Project 8 — Waivers V3](./Project_08_Waivers_V3.md)
- [Project 12 — In-App Messaging](./Project_12_In_App_Messaging.md)
- [Project 49 — Privacy & Compliance Review](./Project_49_Privacy_Compliance_Review.md)
- [Project 53 — Mobile Offline Persistence](./Project_53_Mobile_Offline_Persistence.md)

---

**Last Updated:** 2026-03-06
**Owner:** Product / Engineering
**Status:** Not Started
**Next Review:** When Project 12 (In-App Messaging) reaches planning stage
