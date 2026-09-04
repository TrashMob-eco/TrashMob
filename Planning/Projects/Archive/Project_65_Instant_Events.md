# Project 65 — Instant Events

| Attribute | Value |
|-----------|-------|
| **Status** | Complete — all phases shipped 2026-07-12 through 2026-07-14 |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Small (grew to ~10 PRs; each individually small) |
| **Dependencies** | [Project 15 (Route Tracing)](./Project_15_Route_Tracing.md) — the optional route-tracking toggle depends on the mobile route-recording pipeline already shipped in Phases 1–5 |

## Shipped

| Phase / Slice | PR | Shipped |
|---------------|-----|---------|
| Planning doc + decisions | [#3496](https://github.com/TrashMob-eco/TrashMob/pull/3496) | 2026-07-12 |
| Phase 1 backend endpoints (`POST /events/instant`, `PUT /events/{id}/complete`) | [#3497](https://github.com/TrashMob-eco/TrashMob/pull/3497) | 2026-07-12 |
| Phase 1 mobile UI + local resume (Preferences-backed) | [#3498](https://github.com/TrashMob-eco/TrashMob/pull/3498) | 2026-07-12 |
| Phase 1 cross-device resume (server-side lookup fallback) | [#3499](https://github.com/TrashMob-eco/TrashMob/pull/3499) | 2026-07-12 |
| Phase 2 reverse geocoding (server-side at creation) | [#3500](https://github.com/TrashMob-eco/TrashMob/pull/3500) | 2026-07-13 |
| Phase 2 route tracking toggle + shared `IRouteRecordingCoordinator` | [#3501](https://github.com/TrashMob-eco/TrashMob/pull/3501) | 2026-07-13 |
| Wizard refactor to use the coordinator (net −70 lines) | [#3502](https://github.com/TrashMob-eco/TrashMob/pull/3502) | 2026-07-13 |
| Phase 2 route resume after app force-close | [#3503](https://github.com/TrashMob-eco/TrashMob/pull/3503) | 2026-07-13 |
| Phase 3 community auto-detection banner | [#3504](https://github.com/TrashMob-eco/TrashMob/pull/3504) | 2026-07-14 |
| Phase 4 abandonment guard (hourly job) | [#3505](https://github.com/TrashMob-eco/TrashMob/pull/3505) | 2026-07-14 |

## Lessons learned

- **The doc's "opt-in" mechanism for Phase 3 turned out to be unnecessary.** The doc assumed community linkage would need a `CommunityId` FK on `Event`. Discovered during implementation that `CommunityManager.GetCommunityStatsAsync` already aggregates events via bounding-box match with no per-event flag involved — so linkage happens automatically. Shipped as an informational banner instead of an opt-in.
- **Server-side reverse geocoding beat client-side.** Doc suggested mobile-side; server-side was cleaner (one API call, no follow-up round trip, no stale-address gap) and `IMapManager.GetAddressAsync` already existed in the manager's constructor scope.
- **Route resume after force-close needed hydration in three places**, not just the one the initial doc implied. `RouteTrackingSessionManager.TryRestoreSession` had to actually be called (nobody was), the coordinator needed a `TryResumeAsync` method distinct from `StartAsync`, and the writer's `pointOrder` had to be re-seeded from `SyncQueue.GetMaxPointOrderAsync` to avoid PointOrder collisions with existing SQLite points.
- **Sharing the coordinator with wizard events was worth the refactor.** Phase 2 slice 2 shipped the coordinator + Instant Events integration; a follow-up PR ripped ~120 lines of near-duplicate code out of `ViewEventViewModel` in exchange for a single Start/Stop call sandwiched by `Task.Delay(Timeout.Infinite, cancellationToken)`. Net −70 lines in the wizard VM, behavior fully preserved.
- **Abandonment guard didn't need the mobile "prompt on next open" the doc suggested.** Once auto-Completed, the event drops out of `GetInProgressInstantEventsAsync` (which requires Active status), so the resume banner just doesn't appear. Local Preferences → server validation → clean fresh start. No prompt needed — the existing resume flow degrades gracefully.
- **Reverse-engineering community aggregation before writing Phase 3 saved a schema migration.** Would have been easy to charge in with a `CommunityId` FK + backfill migration + join queries. Reading `GetCommunityStatsAsync` first showed that the bounding-box match was already doing the work.

## Business Rationale

TrashMob today assumes cleanup activity happens as an **organized event**: a user opens the app *in advance*, walks through a five-step wizard ([TrashMobMobile/Pages/CreateEvent/](../../TrashMobMobile/Pages/CreateEvent/)) to set title, date, time, location, description, type, visibility, and duration, and then invites others or arrives at a scheduled time. This is the right model for community-organized public cleanups — but it is completely wrong for the far more common case of someone **already outside picking up litter right now** who wants to record what they're doing.

The Private Event concept exists (visibility = 3, backdating allowed — see [Event.cs:9–16](../../TrashMob.Models/Event.cs#L9-L16)), so the data model can already represent "one person, one cleanup, private." What is missing is the **zero-friction creation path**. Today, a user who wants to log the pick they're about to do has to fill out the same wizard as a community event organizer scheduling a group cleanup three weeks out.

The analog is Strava. Strava does not ask you to name your run, enter its duration, choose a route, and confirm privacy before it starts recording. It has a single big **Record** button. The workout gets a default name (`Morning Run`), a start timestamp of "now," and everything else is filled in when you press Stop. That is the exact interaction pattern we need for solo cleanups.

**Why this matters strategically:**

- Removes a friction point that quietly loses individual-volunteer engagement — someone who almost logs a pick, gets bored of the wizard, and forgets never becomes a repeat data contributor.
- Individual data points fill in the gaps between organized events. A community's cleanup impact map is much richer if solo picks show up alongside group events.
- The stats-entry pattern (log now, add details later) matches how people actually behave when they're outside doing the activity — they don't want to type; they want to move.
- Bootstraps route-tracking usage from [Project 15](./Project_15_Route_Tracing.md) and [Project 48 (Enhanced Route Tracking)](./Archive/Project_48_Enhanced_Route_Tracking.md), which are underused today because they only apply to full scheduled events.

---

## What Exists Today

Independent-engineer scan before drafting scope:

- **`Event` entity** ([TrashMob.Models/Event.cs](../../TrashMob.Models/Event.cs)) — already supports Private visibility (`EventVisibilityId = 3`, see line 111) and backdated `EventDate`. The XML doc on the class even calls this out: *"Private events can be backdated to record past individual cleanup efforts."* This project needs no schema changes.
- **Mobile creation flow** — the five-step wizard in [`TrashMobMobile/Pages/CreateEvent/Step1.xaml.cs`](../../TrashMobMobile/Pages/CreateEvent/Step1.xaml.cs) through `Step5` is the current entry point. It is designed for organized public events and cannot be short-circuited for private-solo use.
- **Event summary flow** — `EditEventSummaryPage` ([TrashMobMobile/Pages/EditEventSummaryPage.xaml.cs](../../TrashMobMobile/Pages/EditEventSummaryPage.xaml.cs)) already captures bags, weight, notes, photos post-event. Reused as the "enter stats afterward" surface — no new stats-entry screen needed.
- **Route tracking** — [Project 15 (Route Tracing)](./Project_15_Route_Tracing.md) delivered the mobile route-recording pipeline in Phases 1–5. `EventAttendeeRoute` on-device buffer + upload flow already exists. The optional-tracking toggle in Instant Events just wires this pipeline to the new event lifecycle.
- **Waiver enforcement** — users cannot register for their first event without signing the TrashMob.eco waiver ([Event.cs:14–15](../../TrashMob.Models/Event.cs#L14-L15)). Same rule applies to Instant Events; blocked at Start if unsigned.
- **Community auto-assignment by GPS** — [Project 44 (Area Map Editor)](./Archive/Project_44_Area_Map_Editor.md) shipped community-boundary polygons. A GPS point inside a community's boundary can be resolved to that community. Optional stretch for Instant Events (see Scope below).

---

## Objectives

### Primary Goals

- **One-tap event creation.** A "Start a New Pick" button on the mobile Dashboard produces a valid Private `Event` with `EventDate = now`, `Name = "Instant Event – <formatted local timestamp>"` (persisted server-side so history lists show a distinguishable title without any client formatting), `Description = "Instant private event"`, `EventVisibilityId = Private`, `EventTypeId = General Cleanup`, `Latitude`/`Longitude` from device GPS, and status = `In Progress`. Zero user input required beyond the tap.
- **Optional route tracking at Start.** A single toggle before Start decides whether to begin the route-recording pipeline from [Project 15](./Project_15_Route_Tracing.md). The toggle **remembers the user's last choice** and defaults to `off` on first-ever use (battery-safe default that regular route-recorders don't have to re-enable every time).
- **Stop → stats.** A prominent Stop button ends the event, sets `EventDate` end / computes duration, and takes the user to `EditEventSummaryPage` for stats entry (or lets them skip and enter later).
- **Reuse everything downstream.** Photos, metrics, route review, community stats, participation-report inclusion, personal history — all work through existing event pipelines because an Instant Event *is* an `Event`.

### Secondary Goals

- **GPS-based community auto-assignment.** If the user's GPS at Start falls inside a community boundary, offer to associate the Instant Event with that community. Opt-in, one tap.
- **Reverse-geocoded address.** Populate `StreetAddress` / `City` / `Region` / `Country` / `PostalCode` from GPS so the event has a human-readable location without asking the user to type it.

---

## Scope

### In scope

- **Backend**: new endpoint `POST /api/v2/events/instant` that takes a minimal DTO (`{ latitude, longitude, trackRoute }` plus optional community id) and returns the created `Event` with all defaults filled server-side. Reuses existing `EventManager` / `EventRepository`.
- **Backend**: new endpoint `PUT /api/v2/events/{id}/stop` (or repurpose an existing status-transition endpoint) that sets the event to `Completed` status, computes duration from the elapsed time, and returns the event.
- **Mobile**: "Start a New Pick" primary action on the Dashboard (`AppShell` home) — the visual weight of Strava's Record button, not buried in a menu.
- **Mobile**: Start-pick modal / bottom sheet with a single **Track my route** toggle and a large **START** button.
- **Mobile**: In-progress view with elapsed time, GPS status, optional live route map (if tracking on), and a **STOP** button.
- **Mobile**: Post-stop navigation to `EditEventSummaryPage`, pre-populated with any auto-detected stats (route distance if tracking was on, duration).
- **Mobile**: Reverse geocoding at Start (existing Azure Maps integration).
- **Mobile**: **Pickup-locations UI works normally.** Users can drop pickup-location pins during or after an Instant Event using the existing pickup-locations flow — this is especially useful with route tracking on ("here's my route, and here are the four spots I actually picked up trash"). No changes to the pickup-locations UI; it stays a standard event feature that applies uniformly to organized and Instant Events.
- **Feature usage metrics**: track Start / Stop / Skip-Stats / community-auto-assigned events per [Project 29 (Feature Usage Metrics)](./Archive/Project_29_Feature_Usage_Metrics.md).

### Out of scope — features intentionally not offered for Instant Events

The whole premise is *no friction*. Everything below is a deliberate omission, not a to-do.

| Feature | Why skipped |
|---------|-------------|
| Title / description entry | Auto-filled server-side |
| Event date / time entry | Always "now" |
| Duration entry | Computed from Start→Stop timestamps |
| Event type picker | Default to General Cleanup |
| Visibility picker | Always Private |
| Team picker / scoping | Private events cannot be team-scoped |
| Max-participants field | Solo — implicit 1 |
| Street-address entry UI | Reverse-geocoded from GPS |
| Partner-services UI | Applies to sponsored/organized public events only |
| Attendee registration / RSVP | Creator is implicit sole attendee |
| Co-lead assignment | Creator is implicit sole lead |
| Waiver signing *in flow* | User must have already signed (block Start if not); no in-flow signing |
| Invitations (email / SMS / share) | Solo event, no one to invite |
| Nearby-volunteer notifications | Private event — not published |
| Reminder emails | Event is happening *now* |
| Confirmation / thank-you emails | Solo — no attendees to email |
| Public event listing / search inclusion | Private events already excluded |
| Cancellation workflow with reason | User can just delete; no one to notify |
| Photo before/after distinction | Post-hoc photo upload only (existing flow) |
| Litter-report linkage at creation | User can link a report to the event afterward |
| Web parity at launch | Web has no GPS Start context — see Open Questions |

### Deferred to the existing post-event stats screen (`EditEventSummaryPage`)

All optional; user can Skip and come back later:

- Number of bags collected
- Weight collected
- Photos
- Notes
- Optional route review / trim (if tracking was on) — reuses [Project 48](./Archive/Project_48_Enhanced_Route_Tracking.md) route-trim UI
- Optional dependents ("did this with my 2 kids") — reuses existing `EventDependent` flow

---

## Success Metrics

- **Adoption:** ≥ 20% of active mobile monthly users create at least one Instant Event within 60 days of launch.
- **Completion rate:** ≥ 80% of started Instant Events reach the Stop → stats screen (versus being abandoned mid-cleanup).
- **Friction reduction:** median time from tapping the primary action to `EventDate` being persisted server-side is under 10 seconds (vs. the current wizard, which takes minutes).
- **Data quality:** ≥ 50% of Instant Events have at least one metric (bags or weight) entered within 24 hours of Stop.
- **Underused-feature bootstrap:** measurable increase in overall route-recording use ([Project 15](./Project_15_Route_Tracing.md)) driven by opt-in from the Start-pick toggle.

---

## Dependencies

- **[Project 15 — Route Tracing](./Project_15_Route_Tracing.md)** (in progress; Phases 1–5 shipped, device testing remaining) — the optional-tracking toggle relies on the existing route-recording pipeline. If Project 15 device-testing surfaces blocking issues, Instant Events can still ship with tracking disabled and the toggle hidden.
- **[Project 4 — Mobile Robustness](./Project_04_Mobile_Robustness.md)** (in progress) — the Start-pick flow needs to survive backgrounding, GPS drift, and network loss. Should be verified against Project 4's device matrix before wide rollout.
- **[Project 44 — Area Map Editor](./Archive/Project_44_Area_Map_Editor.md)** (complete) — provides community boundary polygons for the GPS-based auto-assignment stretch goal.

Not blocked by any project. Can start immediately.

---

## Risks & Mitigations

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| Users start a pick, then background the app and forget — event stays In Progress forever | Medium (data hygiene, cluttered "my events" list) | Medium | Auto-transition to Completed with duration = elapsed time when app backgrounds beyond a threshold (e.g., 4 hours). Prompt on next app open to confirm or delete. |
| Waiver gating blocks the entire Start flow — user's first-ever cleanup is with Instant, they haven't signed | High (kills the value prop for new users) | High | Detect unsigned waiver *before* the Start button rather than after. Present the waiver signing flow up front once, then let the user tap Start. Measure: how many first-time users abandon at the waiver step. |
| GPS unavailable at Start (indoors, permission denied) — event has no location | Medium (broken map display, unusable community stats) | Medium | Fall back to last known location. If unavailable, block Start with a "Location required — check permissions" message. Do not create locationless Events. |
| Users flood the DB with abandoned "Instant Event – 5 seconds" records from accidental taps | Low (data noise; still a legit private event to the user) | Low | 5-second cool-down on Start button after each Start. No further protection — legitimate quick picks exist (someone grabs one bag on their walk home). |
| Reverse-geocode latency delays Start | Medium (kills the "no wait" promise) | Medium | Fire geocoding asynchronously after `Event` is created; patch address fields when geocoding returns. `Event` exists in DB within 500ms of tap regardless. |
| Route recording chews battery in the background | Medium (user complaints, uninstalls) | Medium | Default tracking to OFF. Show a warning in the toggle description ("This will use GPS in the background"). Existing Project 15 battery mitigations apply. |
| Users use Instant Events to log fabricated cleanups for participation-report / gamification credit | Medium (data integrity) | Low | Instant Events are private-by-default and don't feed public leaderboards. If integrity concerns emerge, add server-side rate limits (max N Instant Events per user per day) — do not add up front. |

---

## Implementation Plan

### Phase 1 — Backend endpoint + minimal mobile Start/Stop

- `POST /api/v2/events/instant` — creates a Private `Event` with all defaults, returns the DTO. Wraps existing `EventManager.AddAsync`. Enforces waiver gate.
- `PUT /api/v2/events/{id}/stop` — sets status = Completed, computes duration.
- Mobile: **Start a New Pick** button on Dashboard, single-toggle bottom sheet, in-progress view with timer, Stop button.
- No route tracking, no reverse geocoding, no community auto-assignment yet.
- On Stop, navigate to existing `EditEventSummaryPage`.

**Exit criteria:** an internal-tester user can create an Instant Event, see it in their event history, enter stats, and see it counted in their personal totals.

### Phase 2 — Route tracking toggle + reverse geocoding

- **Reverse geocoding (shipped 2026-07-13):** happens **server-side** inside `EventManager.AddInstantEventAsync`, not client-side as the earlier plan suggested. `IMapManager.GetAddressAsync` already exists on the server and populating address fields before the DB write is cleaner than a mobile follow-up `UpdateEvent` call. Runs synchronously — Azure Maps geocoding is fast (~500ms) and the mobile client is on the "Starting your pick…" status message during the call. Geocode failures are swallowed with a nice-to-have miss (event has GPS but no address) rather than blocking creation.
- Wire the Project 15 route-recording pipeline behind the toggle.
- Post-stop screen shows route distance and offers the trim UI from Project 48 if a route was recorded.

**Exit criteria:** an internal-tester user can complete a full loop with route tracking on and see the route in event details.

### Phase 3 — Community auto-assignment (stretch)

- On Start, query community boundaries for a polygon containing the GPS point.
- If found, show a one-tap "Log this to *[Community Name]*" opt-in in the post-stop stats screen (not before Stop — don't slow down Start).
- Persist as the community linkage via existing community-event association.

**Exit criteria:** a user standing in a community's boundary is offered the community linkage; the community's stats page reflects the Instant Event's metrics.

### Phase 4 — Abandonment guard (shipped 2026-07-14)

- **Server-side hourly sweep** in `TrashMobHourlyJobs`. `EventManager.CompleteAbandonedInstantEventsAsync(TimeSpan threshold)` queries for Instant Events in Active status with zero duration and `EventDate < now - threshold`, sets each to Complete with duration = threshold, and returns the count for logging.
- Threshold set to 4h per the original plan. Configurable via the call site if we ever want to change it.
- **Design note — no mobile prompt on next open.** The doc originally suggested "prompt on next open." In practice the interaction is cleaner: once auto-Completed the event drops out of `GetInProgressInstantEventsAsync` (which requires Active status), so the resume banner just doesn't appear. If local Preferences still point at the completed event, the VM resume path validates with the server, sees the Complete status, and clears Preferences without needing an explicit prompt.
- **Design note — audit fields use the event creator's user id**, not a system-user sentinel. Direct application of the [Project 64 lesson](./Project_64_Roles_and_RBAC_Refactor.md) about never inventing sentinel user ids. Semantic reading: "the user's own event was auto-completed on their behalf."
- **Deferred — analytics on stopped-vs-abandoned ratio.** Would need a boolean flag on `Event` (or similar) to distinguish. Not worth a schema migration for a metric that isn't driving decisions today. Duration = exactly 4h 0min is a reasonable proxy signal in ad-hoc queries.

### Phase 5 — Web parity (deferred; see Open Questions)

Only if user research shows demand.

---

## Rollout Plan

- **Internal beta** (Phase 1 exit): behind [Project 31 (Feature Flags)](./Project_31_Feature_Flags.md) flag `feature.instant_events` — enable for admin + a small set of opted-in beta testers.
- **Wider dev/beta rollout** (Phase 2 exit): enable for all `test` environment users; solicit feedback via in-app widget from [Project 34 (User Feedback)](./Archive/Project_34_User_Feedback.md).
- **Production rollout**: enable for 10% of production users for one week; monitor Sentry errors, feature-usage adoption rate, waiver-gate abandonment rate, and any anomalous event volume; then flip to 100%.
- **Kill switch**: leave the feature flag live for at least 90 days post-rollout so the button can be disabled without a deploy if a problem surfaces.

---

## Decisions

Reviewed 2026-07-12 with the product owner; scope-shaping decisions captured here so implementers don't relitigate them.

| Question | Decision | Reasoning |
|----------|----------|-----------|
| Web parity | **Mobile-only.** Web is not in scope for this project. | Web has no GPS-at-tap context and no cleanup-in-progress use case. Web users continue to use the existing wizard with visibility = Private for backdated logging. |
| Route-tracking toggle default | **Remember the user's last choice; default to `off` on first-ever use.** | Balances battery-safety (first-time users don't get surprise background GPS) with reducing repeat friction for regular route-recorders. |
| Auto-generated title | **Persisted server-side as `"Instant Event – <formatted local timestamp>"`.** Not the split display/DB approach. | Simpler client code (list views just render `.Name`). Users can rename their events afterward through the existing edit flow if the timestamp bothers them. |
| Community stats contribution | **Count equally.** Instant Events GPS-linked to a community roll up into that community's aggregate totals alongside organized-event contributions. | A bag is a bag. Optional filter for "organized vs. solo" can be added to the admin dashboard later if useful. |
| Backdating via the Start button | **No.** Start is strictly "now." | Backdating is what the existing Private Event wizard already does. Keeps the Start button single-purpose. |
| Event type | **Reuse General Cleanup for Phase 1.** No new `EventType` row. | Zero migration cost, no impact on existing type-based dashboards or filters. Segment analytics by "created via Instant flow" using the feature-usage metrics from [Project 29 (Feature Usage Metrics)](./Archive/Project_29_Feature_Usage_Metrics.md). Reconsider adding a dedicated type only if adoption data shows the segmentation would matter. |
