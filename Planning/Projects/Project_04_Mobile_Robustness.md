# Project 4 — Mobile App Robustness & UX Improvements

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phases 1-2 Complete, Phases 3-5 Partial) |
| **Priority** | Critical |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Project 5 (CI/CD Pipelines) |

---

## Business Rationale

Stabilize and significantly improve the .NET MAUI mobile application to ensure reliable user experience, increase app store ratings, and enable future feature development. Current issues include inconsistent error handling, app crashes, outdated toolchain, lack of telemetry for debugging production issues, and a UX that needs substantial improvement in look and flow.

Mobile app quality directly impacts user retention and app store visibility. A crash-free rate below 99% results in poor store rankings and negative reviews. Additionally, a polished, intuitive UX is essential for volunteer engagement and adoption.

---

## Objectives

### Primary Goals
- **Achieve** ≥99.5% crash-free sessions across iOS and Android
- **Upgrade** to latest .NET 10 and MAUI versions
- **Implement** comprehensive error handling with user-friendly messages
- **Integrate** Sentry.io for crash reporting and APM
- **Define** supported OS ranges (iOS 15+, Android 8.0+)
- **Increase** unit test coverage to ≥ 60%
- **Redesign** app navigation and flow for improved usability
- **Refresh** visual design for modern, polished appearance
- **Improve** key user journeys (event discovery, registration, litter reporting)

### Secondary Goals
- Establish automated UI test framework
- Create debugging runbooks for common issues
- Improve app startup performance
- Reduce app size where possible
- Consistent design language across all screens
- Improve app store presence (screenshots, descriptions, metadata)

---

## Scope

### Phase 1 - Stabilization ✅

- [x] Upgrade to .NET 10 and latest MAUI stable version
- [x] Fix known crash bugs (PR #2571 — Google Mobile Usability issues)
- [x] Implement global error handling with retry logic (PR #2588 — `BaseViewModel.ExecuteAsync` pattern with structured error handling)
- [x] Add network failure handling (PR #2588 — `HttpRequestException` and `TaskCanceledException` handling in `ExecuteAsync`)
- [x] Validate all async/await patterns for proper error propagation (PR #2590 — migrated all ViewModels to `ExecuteAsync`)

### Phase 2 - Observability ✅

- [x] Integrate Sentry.io SDK for iOS and Android (Sentry.Maui 6.0.0 — production DSN configured)
- [x] Configure breadcrumbs for user actions (`SentryHttpMessageHandler` on all HTTP clients)
- [x] Set up performance monitoring (app startup, API calls) (`TracesSampleRate=1.0`, `CaptureFailedRequests=true`)
- [ ] Create alerts for crash rate spikes (Sentry project ops task)
- [ ] Build Grafana dashboards for mobile metrics (ops task)

### Phase 3 - Testing (Partial)

- [x] Add unit tests for ViewModels and critical business logic (PR #2593 — 33 ViewModel tests)
- [ ] Create manual test matrix for devices and OS versions
- [ ] Perform regression testing on physical devices
- [ ] Load test API integrations
- [ ] Accessibility audit (TalkBack, VoiceOver)

### Phase 4 - UX Improvements (Partial)

- [ ] UX audit of current app (identify pain points and friction)
- [x] Redesign navigation structure (PR #2606 — 5-tab layout; PR #2496 — bottom tab navigation)
- [x] Refresh visual design (PR #2586 — align mobile styling with web design system)
- [ ] Improve event discovery and browsing experience
- [ ] Streamline event registration flow
- [ ] Polish litter reporting flow (camera, location, submission)
- [ ] Enhance dashboard/home screen with clear calls to action
- [ ] Add lightweight API endpoint for litter report list views with first-image thumbnail URL (avoids N+1 image URL fetches per report; relates to #2340)
- [x] Consistent loading states and feedback throughout app (PR #2588 — `IsBusy` state + user-friendly error messages)
- [x] Add required field indicators to all forms (PR #2591)

### Phase 5 - Documentation (Partial)

- [x] Document Android emulator setup (PR #2584 — MAUI Android dev setup and Google Maps key injection)
- [ ] Document supported device matrix
- [ ] Create troubleshooting guide for common errors
- [ ] Write runbook for investigating Sentry errors
- [ ] Update developer onboarding documentation

### Phase 6 - App Store Presence

- [ ] Create new screenshots showcasing improved UX
- [ ] Update app store descriptions and feature highlights
- [ ] Refresh app icon and promotional graphics (if needed)
- [ ] Improve App Store (iOS) and Play Store (Android) metadata
- [ ] Add/update app preview video (optional)
- [ ] Respond to and address existing user reviews

---

## Out-of-Scope

- ❌ Major new feature development (focus on stability and UX polish)
- ❌ Complete offline mode (partial offline handling only)
- ❌ Apple Watch or Android Wear apps
- ❌ Tablet-optimized layouts
- ❌ Performance optimization beyond critical issues

---

## Success Metrics

### Quantitative
- **Crash-free rate:** ≥ 99.5% (currently ~97%)
- **App startup time:** ≤ 2 seconds (p95)
- **API call failures handled gracefully:** 100%
- **Unit test coverage:** ≥ 60% (currently ~20%)
- **App store rating:** ≥ 4.0 stars (iOS and Android)
- **Sentry error rate:** < 1% of sessions
- **Task completion rate:** Improved for key flows (event registration, litter reporting)

### Qualitative
- Zero critical or high-severity bugs in production after release
- Positive developer feedback on debugging capabilities
- Successful release to both app stores
- User reviews mention improved stability
- User reviews mention improved look and feel
- Navigation feels intuitive and consistent
- Visual design feels modern and polished

---

## Dependencies

### Blockers
- **Project 5 (Pipelines):** Need reliable CI/CD before rolling out updates
- **Sentry.io setup:** Account configuration and instrumentation

### Related Projects
- **Project 1 (Auth):** May require mobile app updates for new auth flows
- **Project 24 (API v2):** Will benefit from auto-generated clients to reduce manual DTO code

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **MAUI upgrade breaks existing features** | Medium | High | Thorough regression testing; maintain v9 fallback branch |
| **Sentry overhead impacts performance** | Low | Medium | Performance profiling before/after; configure sampling rates |
| **Insufficient device coverage for testing** | High | Medium | Leverage BrowserStack/Sauce Labs for device testing; community beta program |
| **App store rejection** | Low | High | Submit updates early; have rollback plan; follow store guidelines |
| **Volunteer developers leave mid-project** | Medium | High | Comprehensive documentation; pair programming; knowledge transfer sessions |
| **Crash fixes introduce new crashes** | Medium | High | Staged rollout (5% ? 25% ? 100%); monitoring dashboards; hot rollback capability |

---

## Implementation Plan

### Toolchain Upgrades
- **Upgrade to .NET 10:** Update all projects to `<TargetFramework>net10.0</TargetFramework>`
- **Upgrade MAUI:** Update to latest stable MAUI workload
- **Dependency updates:** Review and update all NuGet packages
- **Deprecation warnings:** Address all compiler warnings

### Error Handling Patterns

```csharp
// Global error boundary in App.xaml.cs
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        
        // Global exception handler
        AppDomain.CurrentDomain.UnhandledExceptionAsync += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        
        MainPage = new AppShell();
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        LogError(exception, "Unhandled exception");
        ShowUserFriendlyError(exception);
    }

    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        LogError(e.Exception, "Unobserved task exception");
        e.SetObserved(); // Prevent app crash
    }

    private void LogError(Exception ex, string message)
    {
        // Log to Sentry
        SentrySdk.CaptureException(ex);
        
        // Log locally for offline scenarios
        Debug.WriteLine($"{message}: {ex}");
    }

    private void ShowUserFriendlyError(Exception ex)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            string message = ex switch
            {
                HttpRequestException => "Unable to connect. Please check your internet connection.",
                UnauthorizedAccessException => "Your session has expired. Please sign in again.",
                _ => "An unexpected error occurred. Our team has been notified."
            };
            
            await DisplayAlert("Error", message, "OK");
        });
    }
}
```

### Network Handling

```csharp
public abstract class BaseViewModel : INotifyPropertyChanged
{
    protected async Task<T> ExecuteWithErrorHandlingAsync<T>(
        Func<Task<T>> operation,
        string userFacingErrorMessage = null)
    {
        try
        {
            IsBusy = true;
            return await operation();
        }
        catch (HttpRequestException ex)
        {
            // Network failure
            SentrySdk.CaptureException(ex);
            await ShowErrorAsync(userFacingErrorMessage ?? "Unable to connect. Please check your internet connection.");
            return default;
        }
        catch (UnauthorizedAccessException ex)
        {
            // Auth failure
            SentrySdk.CaptureException(ex);
            await NavigateToLoginAsync();
            return default;
        }
        catch (Exception ex)
        {
            // Unknown error
            SentrySdk.CaptureException(ex);
            await ShowErrorAsync("An unexpected error occurred.");
            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

### Sentry Integration

```csharp
// MauiProgram.cs
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        });

    // Configure Sentry
    builder.Services.AddSentry(options =>
    {
        options.Dsn = "YOUR_SENTRY_DSN";
        options.Environment = Preferences.Get("Environment", "production");
        options.Release = AppInfo.VersionString;
        options.TracesSampleRate = 0.2; // 20% of transactions
        options.AutoSessionTracking = true;
        options.IsGlobalModeEnabled = true;
        
        // Attach breadcrumbs
        options.MaxBreadcrumbs = 50;
        options.BeforeBreadcrumb = breadcrumb =>
        {
            // Filter sensitive data from breadcrumbs
            if (breadcrumb.Data?.ContainsKey("password") == true)
            {
                breadcrumb.Data["password"] = "[Redacted]";
            }
            return breadcrumb;
        };
    });

    return builder.Build();
}
```

### Testing Strategy
- **Unit tests:** xUnit for ViewModels and services
- **Integration tests:** Test API clients against dev environment
- **Manual testing:** Device matrix covering iOS 15-17, Android 8-14
- **Beta testing:** TestFlight (iOS) and Google Play Beta (Android)

---

## Implementation Phases

### Phase 1: Fix Critical Bugs
- Address all crash reports from app stores
- Fix memory leaks
- Resolve navigation bugs

### Phase 2: Toolchain Upgrades
- Upgrade .NET and MAUI
- Update dependencies
- Test on physical devices

### Phase 3: Sentry Integration
- Add Sentry SDK
- Configure error reporting
- Test error scenarios
- Set up dashboards and alerts

### Phase 4: Testing
- Write unit tests
- Regression testing
- Performance testing
- Accessibility testing

### Phase 5: Beta Release
- Submit to TestFlight and Google Play Beta
- Recruit beta testers
- Collect feedback and fix bugs

### Phase 6: Production Release
- Staged rollout: 10% ? 50% ? 100%
- Monitor crash rates and errors
- Hot-fix any critical issues

**Note:** Phases run sequentially but developers can work on next phase prep in parallel.

---

## Testing Matrix

### Devices (Minimum Coverage)
| Platform | OS Versions | Devices |
|----------|-------------|---------|
| **iOS** | 15, 16, 17 | iPhone SE (2020), iPhone 13, iPhone 15 Pro |
| **Android** | 8.0, 10, 12, 14 | Pixel 5, Samsung Galaxy S21, OnePlus 9 |

### Test Scenarios
- ? Sign in / Sign out
- ? Browse events (list and map)
- ? Register for event
- ? Create litter report with camera
- ? View dashboard
- ? Offline mode (API failures)
- ? Low memory scenarios
- ? Background/foreground transitions
- ? Push notifications
- ? Deep linking
- ? Accessibility (VoiceOver/TalkBack)

---

## Open Questions

1. **What's the minimum iOS version to support?**
   **Recommendation:** iOS 15+ (covers 95%+ of active devices)
   **Owner:** Product Lead
   **Due:** Early in project

2. **Should we drop support for Android 7 and below?**
   **Recommendation:** Yes, Android 8.0+ (97%+ of active devices)
   **Owner:** Product Lead
   **Due:** Early in project

3. **How many beta testers do we need?**
   **Recommendation:** 50-100 covering diverse devices and OS versions
   **Owner:** Product Lead
   **Due:** Before beta phase

4. ~~**Should we implement automatic crash reporting opt-out?**~~
   **Decision:** Opt-in by default with privacy policy disclosure
   **Status:** ✅ Resolved

5. ~~**Should we implement biometric authentication for returning users?**~~
   **Decision:** Low priority, future feature
   **Status:** ✅ Resolved

6. ~~**What is our push notification strategy (types, frequency, opt-in)?**~~
   **Decision:** See Project 12 for push notification strategy
   **Status:** ✅ Resolved

7. ~~**How do we force critical app updates?**~~
   **Decision:** Minimum version check on app launch via API endpoint; block app usage until updated for critical security patches; soft reminder for feature updates
   **Status:** ✅ Resolved

8. ~~**What data should be cached for offline viewing?**~~
   **Decision:** Future update, low priority
   **Status:** ✅ Resolved

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

**Completed (Closed):**
- ~~**[#2243](https://github.com/trashmob/TrashMob/issues/2243)** - Convert Mobile app to .NET 10~~ ✅
- ~~**[#1204](https://github.com/trashmob/TrashMob/issues/1204)** - Google Mobile Usability Issues detected~~ ✅ (PR #2571)
- ~~**[#1438](https://github.com/trashmob/TrashMob/issues/1438)** - Add Required field indicators to all Mobile app pages~~ ✅ (PR #2591)
- ~~**[#1470](https://github.com/trashmob/TrashMob/issues/1470)** - All pages - Save change should close page~~ ✅ (PR #2590)
- ~~**[#2249](https://github.com/trashmob/TrashMob/issues/2249)** - Ensure exceptions in mobile app do not cause app crash~~ ✅ (PR #2588 + #2590)
- ~~**[#2248](https://github.com/trashmob/TrashMob/issues/2248)** - Review design patterns used in the mobile app~~ ✅ (PR #2590)
- ~~**[#2246](https://github.com/trashmob/TrashMob/issues/2246)** - Add Unit Tests for the Mobile App where needed~~ ✅ (PR #2593)
- ~~**[#2244](https://github.com/trashmob/TrashMob/issues/2244)** - Document and test how to set up Android Mobile Emulator~~ ✅ (PR #2584)
- ~~**[#1471](https://github.com/trashmob/TrashMob/issues/1471)** - Properly handle exceptions for GetLocation calls~~ ✅ (PR #2588)
- ~~**[#2247](https://github.com/trashmob/TrashMob/issues/2247)** - Ensure user metrics are gathered appropriately from the mobile app~~ ✅ (Sentry.Maui fully configured)

**Open — Infrastructure & Quality:**
- **[#2226](https://github.com/trashmob/TrashMob/issues/2226)** - Project 4: Mobile App Robustness (tracking issue)
- **[#2245](https://github.com/trashmob/TrashMob/issues/2245)** - Document and Test how to set up the iOS Emulator
- **[#2250](https://github.com/trashmob/TrashMob/issues/2250)** - Document minimum iOS and Android versions which can run the app
- **[#2251](https://github.com/trashmob/TrashMob/issues/2251)** - Review Apple Store settings
- **[#2252](https://github.com/trashmob/TrashMob/issues/2252)** - Review Android Store settings
- **[#2219](https://github.com/trashmob/TrashMob/issues/2219)** - Mobile App - Add way to see app version number in app
- **[#1291](https://github.com/trashmob/TrashMob/issues/1291)** - [Mobile] A better way of managing secrets and config

**Open — Mobile Bug Issues:**
- **[#2534](https://github.com/trashmob/TrashMob/issues/2534)** - Oversized TrashMob logo on app launch does not fit screen (iOS)
- **[#2340](https://github.com/trashmob/TrashMob/issues/2340)** - Home screen loads very slowly
- **[#2339](https://github.com/trashmob/TrashMob/issues/2339)** - Home screen not visible on iOS
- **[#2337](https://github.com/trashmob/TrashMob/issues/2337)** - Add a Search / Apply Filters button to the Events search screen
- **[#2335](https://github.com/trashmob/TrashMob/issues/2335)** - iOS: Cannot create Litter Report — photos not saved
- **[#2332](https://github.com/trashmob/TrashMob/issues/2332)** - Problem with sign out on iOS
- **[#1466](https://github.com/trashmob/TrashMob/issues/1466)** - View Event - Format What to Expect
- **[#1465](https://github.com/trashmob/TrashMob/issues/1465)** - View Event - Registered Pop up
- **[#1464](https://github.com/trashmob/TrashMob/issues/1464)** - View Event - Fix Duration versus end time
- **[#1459](https://github.com/trashmob/TrashMob/issues/1459)** - Edit Litter Report - Prevent deletion of last image

---

## Related Documents

- **[Project 5 - Deployment Pipelines](./Project_05_Deployment_Pipelines.md)** - CI/CD improvements
- **[Project 24 - API v2 Modernization](./Project_24_API_v2_Modernization.md)** - Auto-generated clients will reduce mobile code
- **[Project 25 - Automated Testing](./Project_25_Automated_Testing.md)** - UI automation for mobile
- **[Mobile Development Guide](../../TrashMobMobile/claude.md)** - MAUI patterns (when created)

---

**Last Updated:** February 9, 2026
**Owner:** Mobile Product Lead + MAUI Developers
**Status:** In Progress (Phases 1-2 Complete, Phases 3-5 Partial, Phase 6 Not Started)
**Next Review:** After remaining iOS bugs are triaged
