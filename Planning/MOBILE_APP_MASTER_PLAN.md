# Mobile App Master Plan

Unified execution plan combining **Project 4** (Stabilization), **Project 38** (Feature Parity), and all open GitHub issues. Ordered by dependency and impact.

## Current State (as of Feb 2026)

| Item | Status |
|------|--------|
| .NET 10 / MAUI 10.0.20 | Done |
| Sentry.io SDK installed | Done, activated in Release builds (PR #2587) |
| Global error handling | Done - ExecuteAsync pattern in BaseViewModel (PR #2588) |
| Styling alignment with web | Done (PR #2586) |
| Android emulator + Google Maps key | Done (PR #2584) |
| Event search filters + login fix | Done (PR #2587) |
| App version display | Done (PR #2587) |
| Required field indicators | Done (PR #2591) |
| Save-and-close navigation | Done (PR #2590) |
| Mobile unit tests | Done - 33 tests (SearchEventsVM, MainVM, BaseVM) |
| iOS emulator docs | Not done |

## Work Streams (6 Phases)

### Phase 1: Critical Bug Fixes
**Goal:** Make the app usable again. These bugs are blocking core flows.
**Effort:** ~1-2 weeks | **Source:** GitHub Issues

| # | Issue | Platform | Severity |
|---|-------|----------|----------|
| #2336 | Event search returns no results | Both | **Critical** — core flow broken |
| #2339 | Home screen not visible | iOS | **Critical** — app unusable |
| #2332 | Sign out loops forward/backward | iOS | **Critical** — can't switch accounts |
| #2533 | Black screen after Google login post sign-out | Android | **High** — workaround exists (force close) |
| #2335 | Litter report photos not saved | iOS | **High** — core flow broken |
| #2534 | Oversized logo on launch screen | iOS | **Medium** — cosmetic |

**Root cause already identified for #2336:** `SearchEventsViewModel` applies filters cumulatively without resetting to the base event list. Each dropdown filter (country, region, city) re-filters the already-filtered `RawEvents` instead of the original API response. Fix: store original results separately, always filter from the original set.

**Approach:** Fix #2336 first (affects both platforms, root cause known). Then tackle iOS issues — likely need iOS simulator or device for #2339, #2332, #2335. Android #2533 auth issue may share root cause with iOS #2332.

### Phase 2: Activate Observability & Error Handling
**Goal:** See what's happening in production. Catch errors before users report them.
**Effort:** ~1 week | **Source:** Project 4 Phases 1-2, Issues #2249, #2247

| Task | Details |
|------|---------|
| Activate Sentry logging | Remove/condition `USETEST` flag so `LoggingService` (Sentry) is used in Release builds instead of `DebugLoggingService` |
| Add `TaskScheduler.UnobservedTaskException` handler | Current global handling misses async exceptions |
| Add `ExecuteWithErrorHandlingAsync<T>()` to `BaseViewModel` | Wrap all ViewModel commands with try/catch, user-friendly error messages, Sentry breadcrumbs |
| Wrap all REST service calls | Network failure → user-friendly "check your connection" message with retry option |
| Add app version display (#2219) | Show version on MorePage (read from `AppInfo.Current.VersionString`) |
| Verify Sentry dashboards | Confirm crash reports, performance traces, and breadcrumbs flow correctly |

### Phase 3: Testing & Quality Foundation
**Goal:** Catch regressions before they ship. Build confidence for larger changes.
**Effort:** ~2 weeks | **Source:** Project 4 Phase 3, Issues #2246, #2248, #2250

| Task | Details |
|------|---------|
| Create `TrashMobMobile.Tests` project | xUnit + Moq, targeting ViewModels and Services |
| Unit tests for ViewModels | Priority: `SearchEventsViewModel`, `MainViewModel`, `AuthService`, `CreateEventViewModel` |
| Review design patterns (#2248) | Audit MVVM compliance, identify anti-patterns, document conventions |
| Document minimum OS versions (#2250) | iOS 15.0+, Android API 21+ (already configured in csproj) |
| Document iOS emulator setup (#2245) | Complement the Android setup from PR #2584 |
| Document Android emulator setup (#2244) | Update with learnings from PR #2584 (Fast Deployment, SDK paths) |
| Add required field indicators (#1438) | Visual consistency improvement across all form pages |
| Save-and-close behavior (#1470) | After saving changes, navigate back to previous page |

### Phase 4: Navigation Redesign & UX Polish
**Goal:** Modern, intuitive navigation. This **combines** Project 4 Phase 4 with Project 38 Phase 1.
**Effort:** ~3-4 weeks | **Source:** Both Projects

**Why combine:** Both projects call for navigation redesign and UX refresh. Doing it twice would mean rewriting navigation structure, then rewriting it again. Do it once with the Project 38 target structure.

Current structure (4 tabs):
```
Home (map/list) | Dashboard | Location | More
```

Target structure (5 tabs, Strava-inspired):
```
Home (feed) | Explore (map) | + (quick action) | Impact | Profile
```

| Task | Details |
|------|---------|
| Restructure `MainTabsPage.xaml` | 5-tab Shell layout replacing current 4-tab |
| Build Home feed page | Upcoming events, nearby recommendations, activity cards |
| Enhance Explore page | Map with layer toggles (events, litter reports), search bar, list/map toggle |
| Add FAB/Quick Action | Central "+" button: Create Event, Report Litter, Quick Check-in |
| Build Impact dashboard | Stats cards, achievement previews, leaderboard rank preview |
| Build Profile page | User info, my events, my reports, settings, sign out |
| Move Location Preferences | Into Profile > Settings (not its own tab) |
| Add search filters button (#2337) | Explicit "Apply Filters" button on search screen |
| Review app store settings (#2251, #2252) | Update screenshots, descriptions, metadata after UX refresh |

### Phase 5: Feature Parity (New Features)
**Goal:** Bring mobile to parity with key web features.
**Effort:** ~6-8 weeks | **Source:** Project 38 Phases 2-5

Execute in this order (by backend readiness and user value):

**5A. Teams Integration** (P0 — backend complete)
- TeamsListPage, TeamDetailPage, MyTeamsPage
- Browse, join, view team events and stats
- ~2 weeks

**5B. Event Photos** (P1 — backend Phase 1 complete)
- Photo gallery on ViewEventPage
- Camera capture with Before/During/After types
- Offline upload queue
- ~2 weeks

**5C. Leaderboards & Achievements** (P1 — backend complete)
- LeaderboardsPage (Users/Teams/Communities tabs)
- AchievementsPage (badge grid with progress)
- ~1.5 weeks

**5D. Attendee Metrics & Impact** (P1 — backend partial)
- Weight entry on event summaries
- Personal impact page with charts
- Individual contribution logging
- ~1.5 weeks
- **Depends on:** Project 22 completion for full individual metrics API

### Phase 6: Polish & Release
**Goal:** Ship quality. Get back in the app stores with confidence.
**Effort:** ~2 weeks | **Source:** Project 4 Phases 5-6, Project 38 Phase 6-7

| Task | Details |
|------|---------|
| Newsletter preferences page | Manage email subscriptions, notification toggles |
| Team events (limited admin) | Team leads can create team-linked events |
| Beta release | TestFlight + Google Play Beta, recruit 50-100 testers |
| Regression testing | Physical device testing matrix (iOS 15-17, Android 8-14) |
| App store refresh | New screenshots, updated descriptions, promotional graphics |
| Staged production rollout | 10% → 50% → 100% with crash rate monitoring |
| Respond to existing reviews | Address concerns, highlight improvements |

## Issue-to-Phase Mapping

| Issue | Phase | Notes |
|-------|-------|-------|
| #2336 Event search no results | 1 | Root cause: cumulative filter bug |
| #2339 Home screen not visible iOS | 1 | |
| #2332 iOS sign out loops | 1 | |
| #2533 Black screen after login Android | 1 | |
| #2335 iOS litter report photos | 1 | |
| #2534 Oversized logo iOS | 1 | |
| #2249 Exceptions don't crash app | 2 | Global error handling |
| #2247 User metrics gathered | 2 | Sentry activation |
| #2219 App version display | 2 | |
| #2246 Unit tests | 3 | |
| #2248 Review design patterns | 3 | |
| #2250 Document min OS versions | 3 | |
| #2244 Android emulator docs | 3 | Partially done (PR #2584) |
| #2245 iOS emulator docs | 3 | |
| #1438 Required field indicators | 3 | |
| #1470 Save closes page | 3 | |
| #2337 Search filters button | 4 | |
| #2251 Review Apple Store settings | 6 | After UX refresh |
| #2252 Review Android Store settings | 6 | After UX refresh |
| #2265 Individual metrics | 5D | Depends on Project 22 |
| #1291 Secrets management | Done | PR #2584 (Key Vault integration) |
| #2226 Project 4 tracking | All | Parent issue |
| #206 Instagram posting | Deferred | Blocked, low priority |
| #1202 Route tracing | Separate | Project 15, cross-platform |

## Key Decisions

### 1. Combine Project 4 Phase 4 + Project 38 Phase 1
Both call for navigation redesign and UX refresh. Do it once targeting the Strava-inspired 5-tab layout. This saves ~2-3 weeks of rework.

### 2. Fix bugs before features
The 6 active bugs are destroying user trust. No point adding teams/photos if users can't sign in or search for events.

### 3. Activate Sentry before major changes
We need observability in place before the navigation redesign so we can monitor for regressions.

### 4. iOS work needs prioritization
4 of 6 critical bugs are iOS-specific. Need access to iOS simulator/device or ensure vtserej (who's assigned to most) has capacity.

### 5. Backend dependencies are mostly met
- Teams API: Complete
- Event Photos API: Phase 1 complete
- Leaderboards API: Complete
- Attendee Metrics API: Partial (may need backend work in parallel)

## Timeline Estimate

| Phase | Duration | Cumulative |
|-------|----------|------------|
| 1. Bug Fixes | 1-2 weeks | 1-2 weeks |
| 2. Observability | 1 week | 2-3 weeks |
| 3. Testing & Quality | 2 weeks | 4-5 weeks |
| 4. Navigation Redesign | 3-4 weeks | 7-9 weeks |
| 5. Feature Parity | 6-8 weeks | 13-17 weeks |
| 6. Polish & Release | 2 weeks | 15-19 weeks |

**Total: ~4-5 months** for the full plan. Phases 1-3 can be done incrementally with PRs shipping every few days. Phase 4 is the biggest single change.

## What We Can Start Right Now

1. **Fix #2336** (event search filter bug) — root cause identified, can fix immediately
2. **Activate Sentry** — flip the USETEST flag for Release builds
3. **Add version display** — simple change to MorePage
4. **Fix #2533** (Android black screen) — can debug on our emulator
