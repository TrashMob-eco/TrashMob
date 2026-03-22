# Project 25 — Automated UI/E2E Testing

| Attribute | Value |
|-----------|-------|
| **Status** | Phases 1–3 Complete — 197 E2E tests, 32 test files, authenticated + admin flows, CI integration |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Project 5 (CI/CD Infrastructure) |

---

## Business Rationale

Reduce regression risk, enable confident releases, and replace manual test scenarios with executable, maintainable test suites for both web and mobile platforms. Currently, testing relies on manual scenarios documented in TestScenarios.md.

---

## Objectives

### Primary Goals
- **Playwright test framework** for web UI with CI integration
- **UI testing for mobile** app (Appium or MAUI test framework)
- **Convert manual test scenarios** (TestScenarios.md) to automated tests
- **GitHub Actions integration** with test reports
- **Parallel test execution** for faster feedback

### Secondary Goals
- Visual regression testing
- Performance/load testing
- API contract testing
- Accessibility testing automation

### Future Improvements
- **Adversarial/bug-finding unit tests** - Current unit tests verify code does what it does, not what it *should* do. Add:
  - Property-based testing (generate random inputs, verify invariants)
  - Mutation testing (verify tests fail when code is deliberately broken)
  - Edge case coverage (nulls, empty collections, boundary values, concurrent access)
  - Tests written from specifications before reading implementation
  - Integration tests with real database/service interactions

---

## Scope

### Web (Playwright) ✅ IMPLEMENTED
- ✅ User authentication (Entra External ID login with MSAL session restoration)
- ✅ Event creation wizard (multi-step form)
- ✅ Event details and registration (attend button, calendar, share)
- ✅ Event editing (dashboard interactions)
- ✅ Litter report listing and filtering
- ✅ Litter report editing
- ✅ Partner request forms (become/invite partner)
- ✅ Site administration (25 admin pages)
- ✅ Contact form submission and validation
- ✅ Dashboard (events, teams, impact, waivers, newsletters, invites)
- ✅ Profile editing with save/discard/validation
- ✅ Team creation and detail views
- ✅ Leaderboard filtering and tab switching
- ✅ Community listing and detail
- ✅ Location preference page
- ✅ Newsletter preference toggles
- ✅ Waiver status in dashboard
- ✅ Share dialog (social + QR code)
- ✅ Navigation menus (Radix hover)
- ✅ Mobile responsive layout
- ✅ API health checks (6 endpoints)
- ✅ Error pages (404)
- ✅ Accessibility (keyboard navigation, ARIA)

### Mobile (Appium/.NET MAUI Testing) — NOT STARTED
- ❌ Authentication flows
- ❌ Event discovery and details
- ❌ Event registration
- ❌ Litter report creation with camera
- ❌ Dashboard and stats viewing
- ❌ Map interactions

---

## Out-of-Scope

- ❌ Visual regression testing (future phase)
- ❌ Performance/load testing (separate project)
- ❌ Third-party service integration tests
- ❌ Mobile UI testing (deferred — mobile app is stable and tested manually)

---

## Success Metrics

### Quantitative
- **E2E test count:** 197 tests (target was critical flows ≥ 80% — achieved)
- **CI pipeline runs all E2E tests on PRs:** ✅ Yes (`.github/workflows/e2e-tests.yml`)
- **Test execution time:** ~4–5 minutes in CI (target was < 10 minutes — achieved)
- **Flaky test rate:** < 3% (2-3 tests occasionally need retry — target was < 5%)
- **Manual regression testing time:** Significantly reduced — all critical pages have automated coverage

### Qualitative
- ✅ Developers confident in automated test coverage
- ✅ Faster release cycles (PRs verified automatically)
- ✅ Fewer production regressions (caught paginated response bugs, attendee DTO mismatches via E2E)

---

## Dependencies

### Blockers
- **Project 5 (CI/CD Infrastructure):** ✅ Stable pipeline — complete

### Enables
- ✅ Faster development velocity
- ✅ Confident refactoring (Project 24 v2 migration validated by E2E)
- ✅ Better code quality

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Flaky tests** | Medium | Medium | ✅ Retry logic (1-2 retries per test); stable auth state caching (30min); `waitForLoadState` + explicit timeouts |
| **Slow test execution** | Low | Medium | ✅ Parallel execution (2 CI workers); runs in ~4-5 min |
| **Test maintenance burden** | Medium | Medium | ✅ Page object pattern; shared auth fixture; error boundary detection |
| **Auth session expiry** | Medium | Medium | ✅ Global setup re-login if state > 30 min old; sessionStorage capture/restore |
| **Mobile emulator issues in CI** | N/A | N/A | Mobile testing deferred |

---

## Current Implementation (Phases 1–3 Complete)

### Infrastructure
- ✅ **Playwright** installed and configured in `TrashMob/client-app/`
- ✅ **GitHub Actions workflow** (`.github/workflows/e2e-tests.yml`) runs on PRs to main + pushes to main
- ✅ **Trigger paths:** `TrashMob/client-app/**` + `TrashMob/Controllers/**` (controller changes trigger E2E too)
- ✅ **Path exclusions:** `e2e/**` and `playwright.config.ts` excluded from app build/deploy workflows (e2e-only changes don't trigger rebuilds)
- ✅ **Tests run against dev.trashmob.eco** with Chromium browser
- ✅ **Test reports** uploaded as artifacts (HTML report always, test results on failure)
- ✅ **Authenticated testing** via Entra External ID login with MSAL sessionStorage capture
- ✅ **Admin testing** via separate admin user with site admin privileges
- ✅ **dotenv integration** for local credentials (`.env.local`, gitignored)

### Authentication Infrastructure
- **Global setup** (`e2e/global-setup.ts`): Logs in both standard user and admin user via Entra External ID, saves cookies + MSAL sessionStorage to `.auth/user.json` and `.auth/admin.json`
- **Auth fixture** (`e2e/fixtures/auth.fixture.ts`): Provides `authenticatedPage` and `adminPage` fixtures that restore MSAL sessions; auto-skips tests when credentials unavailable
- **Session caching:** Auth state cached for 30 minutes to avoid re-login on every test run
- **CI secrets:** `E2E_USER_EMAIL`, `E2E_USER_PASSWORD`, `E2E_ADMIN_EMAIL`, `E2E_ADMIN_PASSWORD`

### File Structure
```
TrashMob/client-app/
├── playwright.config.ts          # Multi-browser config with globalSetup + dotenv
├── e2e/
│   ├── global-setup.ts           # Entra login for user + admin, saves auth state
│   ├── .auth/                    # Auth state files (gitignored)
│   │   ├── user.json
│   │   └── admin.json
│   ├── fixtures/
│   │   ├── auth.fixture.ts       # authenticatedPage + adminPage fixtures
│   │   └── base.fixture.ts       # Base test fixture
│   ├── pages/
│   │   ├── home.page.ts          # Home page object (Radix nav hover)
│   │   ├── contact.page.ts       # Contact form page object
│   │   └── events.page.ts        # Events page object
│   ├── tests/                    # 32 test files, 197 tests
│   │   ├── accessibility.spec.ts
│   │   ├── api-health.spec.ts
│   │   ├── authenticated.spec.ts
│   │   ├── communities.spec.ts
│   │   ├── contact.spec.ts
│   │   ├── dashboard-interactions.spec.ts
│   │   ├── dashboard.spec.ts
│   │   ├── error-pages.spec.ts
│   │   ├── event-creation.spec.ts
│   │   ├── event-details.spec.ts
│   │   ├── event-registration.spec.ts
│   │   ├── footer.spec.ts
│   │   ├── home-interactions.spec.ts
│   │   ├── home-page.spec.ts
│   │   ├── leaderboard-interactions.spec.ts
│   │   ├── leaderboards.spec.ts
│   │   ├── litter-report-edit.spec.ts
│   │   ├── litter-reports.spec.ts
│   │   ├── location-preference.spec.ts
│   │   ├── mobile.spec.ts
│   │   ├── newsletter-preferences.spec.ts
│   │   ├── page-coverage.spec.ts
│   │   ├── partner-request.spec.ts
│   │   ├── profile-edit.spec.ts
│   │   ├── profile.spec.ts
│   │   ├── public-pages.spec.ts
│   │   ├── share-dialog.spec.ts
│   │   ├── siteadmin-interactions.spec.ts
│   │   ├── siteadmin.spec.ts
│   │   ├── team-create.spec.ts
│   │   ├── team-detail.spec.ts
│   │   └── waivers.spec.ts
│   └── utils/
│       └── helpers.ts            # Shared utilities (uniqueId, dates, toasts)
```

### Test Inventory (197 tests across 32 files)

#### Public Pages (no auth required)
| Test File | Tests | Coverage |
|-----------|-------|----------|
| `public-pages.spec.ts` | 24 | Home page, navigation (Explore/About menus), 18 static pages, accessibility |
| `home-page.spec.ts` | 5 | Hero section, stats, introduction, events, getting started |
| `home-interactions.spec.ts` | 7 | Tab switching, map/list toggle, event count, Take Action menu, stats |
| `footer.spec.ts` | 5 | Copyright, nav links, social media, non-profit disclosure |
| `mobile.spec.ts` | 6 | Hamburger menu, nav toggle, sign-in, sections, footer, static pages |
| `error-pages.spec.ts` | 4 | 404 page, back/home links, contact link |
| `contact.spec.ts` | 8 | Form display, required fields, validation, submission, accessibility |
| `communities.spec.ts` | 3 | Page display, browse heading, community detail navigation |
| `litter-reports.spec.ts` | 4 | Filter controls, list/map toggle, location search, Report Litter link |
| `page-coverage.spec.ts` (public) | 3 | News page, unsubscribe, non-existent article |
| `api-health.spec.ts` | 6 | Config, stats, Google Maps key, events, communities, leaderboards |

#### Authenticated User (standard user)
| Test File | Tests | Coverage |
|-----------|-------|----------|
| `authenticated.spec.ts` | 6 | Account menu, dashboard, profile, event creation, teams, litter reports |
| `dashboard.spec.ts` | 9 | Hero, logged in, events heading/link/upcoming/completed, teams, browse teams, impact |
| `dashboard-interactions.spec.ts` | 7 | Event filter dropdown, list/map toggle, sidebar nav (impact/waivers/newsletters), invite friends |
| `profile.spec.ts` | 6 | Form with data, email read-only, save/discard, data privacy, photo, name fields |
| `profile-edit.spec.ts` | 4 | Edit + save with toast, discard navigation, username validation, data persistence |
| `event-details.spec.ts` | 3 | Event name display, attend button, share button |
| `event-creation.spec.ts` | 4 | Step 1 display, step indicators, Next button, manage event layout |
| `event-registration.spec.ts` | 3 | Attend button state, calendar dropdown options, calendar/share visibility |
| `share-dialog.spec.ts` | 2 | Open dialog with social tab, switch to QR code tab |
| `leaderboards.spec.ts` | 4 | Page with tabs, filter dropdowns, tab switching, ranking info |
| `leaderboard-interactions.spec.ts` | 3 | Type filter dropdown, volunteers/teams tabs, ranking display |
| `location-preference.spec.ts` | 4 | Heading, preferences card, save/discard, address fields |
| `newsletter-preferences.spec.ts` | 3 | Toggle display, subscription toggle + restore, unsubscribe all |
| `waivers.spec.ts` | 2 | Dashboard waivers section, signed waiver status |
| `team-detail.spec.ts` | 4 | Team info, members, share button, join/manage action |
| `team-create.spec.ts` | 5 | Form fields, name/description, public/approval toggles, create/cancel, validation |
| `partner-request.spec.ts` | 3 | Become partner form + fields, email field, invite partner |
| `litter-report-edit.spec.ts` | 8 | Page load, form fields, buttons, access control, cancellation |
| `page-coverage.spec.ts` (auth) | 5 | Waivers, become/invite partner, delete data + buttons, achievements |
| `accessibility.spec.ts` | 8 | Heading hierarchy, alt text, form labels, ARIA, color contrast, focus, landmarks, skip links |

#### Site Admin (admin user)
| Test File | Tests | Coverage |
|-----------|-------|----------|
| `siteadmin.spec.ts` | 25 | Layout, sidebar nav, events/users/partners tables, partner requests, litter reports, teams, contacts/CRM, contact tags, waivers + compliance, feedback + status tabs, newsletters + create, email templates, content management, bulk invites, photo moderation, donations, grants, prospects, job opportunities |
| `siteadmin-interactions.spec.ts` | 5 | Events search + sort header, users search, contacts tab filter, feedback status filter, sidebar navigation |

### Bugs Found by E2E Tests
E2E tests caught real bugs during development:
1. **Home page crash** — `GetFilteredEvents` returned `PagedResponse` but frontend expected raw array (PR #3100)
2. **Event details crash** — `EventAttendeeDto` has `userId` but components referenced `.id` (PR #3117)
3. **Teams page silent failure** — `GetPublicTeams` response was `PagedResponse` but not unwrapped (PR #3109)
4. **Google Maps 404** — MapsV2Controller missing `googlemapkey`, `search`, `reversegeocode` endpoints (PR #3101)
5. **Azure Maps 404 on every page** — Frontend fetched unused Azure Maps key via deprecated endpoint (PR #3130)

### Test Scenarios Coverage

| Scenario (from TestScenarios.md) | Status | Test File(s) |
|---|---|---|
| Sign up for site | ✅ Partial (login tested, not signup) | `authenticated.spec.ts` |
| Sign in to site | ✅ Complete | `global-setup.ts`, `authenticated.spec.ts` |
| Contact Us | ✅ Complete | `contact.spec.ts` |
| Update User Location Preference | ✅ Complete | `location-preference.spec.ts` |
| Create Event | ✅ Partial (form navigation, not submission) | `event-creation.spec.ts` |
| Update event | ✅ Partial (dashboard interactions) | `dashboard-interactions.spec.ts` |
| Sign Waiver | ✅ Partial (waiver status verified) | `waivers.spec.ts` |
| Register for event | ✅ Complete | `event-registration.spec.ts`, `event-details.spec.ts` |
| Send Invite to potential partner | ✅ Complete | `partner-request.spec.ts` |
| Request to become a partner | ✅ Complete | `partner-request.spec.ts` |
| Update Partner | ✅ Partial (admin list view) | `siteadmin.spec.ts` |
| Site Administration | ✅ Complete (25 admin pages) | `siteadmin.spec.ts`, `siteadmin-interactions.spec.ts` |

---

## Implementation Phases

### Phase 1: Web Foundation ✅ COMPLETE
- ✅ Set up Playwright in client-app
- ✅ Configure test fixtures (base + auth)
- ✅ Implement public page tests (55 tests)
- ✅ Add GitHub Actions workflow

### Phase 2: Authenticated Flows ✅ COMPLETE
- ✅ Entra External ID login via global setup
- ✅ MSAL sessionStorage capture/restore
- ✅ Dashboard, profile, event, team, litter report tests
- ✅ Admin user login + site admin tests (25 admin pages)
- ✅ API health checks

### Phase 3: User Interactions ✅ COMPLETE
- ✅ Form editing (profile save/discard/validation)
- ✅ Tab switching (leaderboards, home events, admin feedback)
- ✅ List/map view toggles (events, litter reports, dashboard)
- ✅ Dropdown filters (event filter, leaderboard type/range)
- ✅ Share dialog (open/close, social/QR tabs)
- ✅ Navigation menu hover (Radix NavigationMenu)
- ✅ Newsletter preference toggles
- ✅ Event creation wizard steps
- ✅ Team creation form
- ✅ Partner request forms
- ✅ Page coverage for previously untested pages

### Phase 4: Mobile Testing — Tests Written, CI Blocked
- ✅ Evaluate Appium vs MAUI testing → Appium with UiAutomator2 selected
- ✅ Set up mobile test project (TrashMobMobile.UITests — 28 tests, 8 files)
- ✅ Add AutomationIds to XAML views (HomeFeed, Explore, Impact, Profile)
- ✅ Implement tests: app launch, tab navigation, home feed, explore map, impact stats, profile, quick action, screenshots
- ❌ Configure CI — **blocked** (see Phase 4 CI Investigation below)
- Tests run locally: `appium & dotnet test TrashMobMobile.UITests/`

---

## Decisions

1. **Test data management strategy?**
   **Decision:** Use real dev environment data; no destructive operations in tests; profile edits restored after test

2. **Test environment?**
   **Decision:** Tests run against dev.trashmob.eco (deployed). Local tests proxy API via `.env.local`

3. **Required vs optional PR checks?**
   **Decision:** Currently optional. Promoting to required after stability is confirmed.

4. **Browser/device matrix?**
   **Decision:** Chromium only in CI (fastest). Firefox and mobile projects defined but not run in CI.

5. **Auth approach?**
   **Decision:** Entra External ID login via global setup with sessionStorage capture. Two test users: standard + admin.

---

## Phase 4 CI Investigation (March 2026)

We spent a full day attempting to run the Appium tests in GitHub Actions. **Tests work locally but CI is blocked by emulator instability.** Documented here so we don't repeat the effort.

**Reddit discussion:** [Running MAUI + Appium UI Tests on GitHub Actions](https://www.reddit.com/r/dotnetMAUI/comments/1s0tiq3/running_maui_appium_ui_tests_on_github_actions/)

### What We Tried (in order)

| # | Runner | API | Arch | GPU | Result |
|---|--------|-----|------|-----|--------|
| 1 | ubuntu-latest | 35 | x86_64 | swiftshader | Boot timeout — API 35 too heavy without KVM |
| 2 | ubuntu-latest | 31 | x86_64 | swiftshader | Booted, but `dotnet build -t:Install` → ADB0010 broken pipe |
| 3 | ubuntu-latest | 31 | x86_64 | swiftshader | `adb install` also broken pipe — Ubuntu emulator too unstable |
| 4 | ubuntu-latest | 30 | x86_64 | swiftshader | Same broken pipe on install |
| 5 | ubuntu-latest | 31 | x86_64 | swiftshader | Appium session created but "Appium Settings app not running after 30s" |
| 6 | macos-13 | 30 | x86_64 | swiftshader | Runner deprecated — "configuration not supported" |
| 7 | macos-15 | 30 | x86_64 | swiftshader | Boot timeout — Apple Silicon can't run x86_64 via Rosetta |
| 8 | macos-14 | 30 | x86_64 | swiftshader | Boot timeout — same Rosetta issue |
| 9 | macos-14 | 31 | arm64-v8a | swiftshader | Boot timeout — swiftshader is x86-only |
| 10 | macos-14 | 31 | arm64-v8a | host | `VK_ERROR_OUT_OF_DEVICE_MEMORY` — CI runner has no GPU memory |
| 11 | macos-14 | 31 | arm64-v8a | auto | Boot timeout — falls back to host GPU, same memory error |

### What Actually Worked (partially)

- **Emulator booting:** Ubuntu + API 30/31 (slow but functional)
- **App building:** `dotnet publish -c Release -p:TargetFrameworks=net10.0-android`
- **App installing:** Direct `adb install` on Ubuntu (when emulator was stable — intermittent)
- **Appium session:** With 5-min timeout + `skipDeviceInitialization: true`
- **Tests executing:** All 28 ran on Ubuntu but failed due to Appium Settings timeout

### Root Cause

GitHub Actions runners don't provide a stable Android emulator for MAUI + Appium:
- **Ubuntu:** No KVM hardware acceleration → emulator slow/unstable, adb broken pipes
- **macOS ARM (14/15):** arm64 emulators exist but GPU rendering crashes (no dedicated GPU memory)
- **macOS Intel (13):** Deprecated and removed

Lightweight native Android apps work with `reactivecircus/android-emulator-runner`. MAUI apps are heavier (larger APK, longer startup) and Appium adds helper apps that compound the problem.

### Key Appium Capabilities for CI (reference for future attempts)

```csharp
options.AddAdditionalAppiumOption("uiautomator2ServerInstallTimeout", 120000);
options.AddAdditionalAppiumOption("uiautomator2ServerLaunchTimeout", 120000);
options.AddAdditionalAppiumOption("adbExecTimeout", 180000);
options.AddAdditionalAppiumOption("androidInstallTimeout", 180000);
options.AddAdditionalAppiumOption("skipDeviceInitialization", true);
options.AddAdditionalAppiumOption("newCommandTimeout", 300);
options.AddAdditionalAppiumOption("appWaitForLaunch", false);
Driver = new AndroidDriver(new Uri(serverUrl), options, TimeSpan.FromMinutes(5));
```

### Community-Recommended Tools (Reddit, March 2026)

After posting our CI challenges to r/dotnetMAUI, two tools were recommended:

**1. [Cuttlefish](https://github.com/jonathanpeppers/cuttlefish)** — by Jonathan Peppers (Microsoft MAUI team)
- Purpose-built for booting Android emulators, running MAUI apps, and taking screenshots
- Lighter weight than full Appium — may work on GitHub hosted runners
- **Status:** Needs evaluation — see Phase 5 below

**2. [maui-containers](https://github.com/maui-containers/maui-containers)** — Docker container images
- Pre-configured Android emulator + Appium server in Docker
- Usage: forward ports, bind volume with built APK, point Appium to mounted path
- Better suited for self-hosted/custom runners (same emulator resource constraints apply)

**3. Custom/self-hosted runner** — recommended by redth (Microsoft MAUI team)
- GitHub hosted runners likely don't have enough resources for Android emulator UI tests
- A dedicated self-hosted runner with KVM support would solve all emulator stability issues

### Cuttlefish Evaluation (March 2026) ✅ PROMISING

Deep evaluation of jonathanpeppers/cuttlefish shows this is a strong candidate:

**Why it works where we failed:**
- GitHub Actions ubuntu-latest **does have KVM** — Cuttlefish uses it properly (our standard emulator didn't)
- Uses `guest_swiftshader` GPU mode — no dedicated GPU memory needed
- Google's own virtual device, designed for headless CI environments
- Exposes a standard ADB device — our existing Appium tests connect to it unchanged

**Proven performance on ubuntu-latest (API 34):**
- Setup Cuttlefish (download + boot): ~2m30s
- Build + run MAUI app (`dotnet build -t:Run`): ~2m18s
- Screenshot capture: <1s
- Total: ~5 minutes
- **100% pass rate** on API 34+ across all observed runs

**Limitations:**
- Ubuntu only (not macOS — can't use for iOS testing)
- Not a test framework — still need Appium for UI interactions/assertions
- API 11-13 crashes runners (not relevant — we target API 34+)

### Recommended Path Forward

1. **Integrate Cuttlefish into mobile-ui-tests.yml** — replace emulator setup, keep existing Appium tests ← **IN PROGRESS**
2. **If stable** → promote workflow from `[Experimental]` to standard CI
3. **Fallback** → maui-containers on a self-hosted runner if reliability issues emerge

### Current Status

- **Tests:** 28 Appium tests across 8 files — run locally only
- **Workflow:** `.github/workflows/mobile-ui-tests.yml` marked `[Experimental]`
- **Revisit when:** Cuttlefish evaluation complete, or GitHub Actions adds KVM support on Ubuntu

---

## Related Documents

- **[Project 5 - CI/CD](./Project_05_Deployment_Pipelines.md)** - Pipeline infrastructure
- **[Project 24 - API v2](./Project_24_API_v2_Modernization.md)** - V2 migration validated by E2E tests
- **[TestScenarios.md](../../TestScenarios.md)** - Manual scenarios (most now automated)
- **[Playwright Documentation](https://playwright.dev)** - Framework docs

---

**Last Updated:** March 22, 2026
**Owner:** Engineering Team
**Status:** Phases 1–3 Complete, Phase 4 Tests Written (CI blocked), evaluating Cuttlefish for Phase 5
**Next Review:** After Cuttlefish evaluation

---

## Changelog

- **2026-03-22:** Deep evaluation of Cuttlefish — 100% reliable on API 34+, ~5min total on ubuntu-latest with KVM. Prototyping integration with existing Appium tests.
- **2026-03-22:** Added community-recommended CI tools (Cuttlefish, maui-containers) from Reddit r/dotnetMAUI thread. Defined recommended evaluation path for Phase 5.
- **2026-03-22:** Phase 4 mobile testing — 28 Appium tests written (8 files), AutomationIds added to XAML views. CI workflow created but blocked by GitHub Actions emulator instability. Documented all CI attempts. Reddit discussion posted. Tests work locally.
- **2026-03-15:** Major update — Phases 2 and 3 complete. 197 E2E tests across 32 files covering public pages, authenticated user flows, admin pages, and user interactions. Auth infrastructure (Entra login, MSAL session capture, admin user). CI workflow triggers on controller changes. E2E-only changes excluded from build/deploy workflows. Documented all test files, coverage, bugs found, and test scenario mapping.
- **2026-02-05:** Updated status to "In Progress (Phase 1 Complete)". Playwright framework installed, GitHub Actions workflow running on PRs, page objects and initial tests implemented.
- **2026-01-31:** Converted open questions to decisions; confirmed all scope items
