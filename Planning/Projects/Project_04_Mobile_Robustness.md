# Project 4 — Mobile App Robustness & UX Improvements

| Attribute | Value |
|-----------|-------|
| **Status** | Developers Engaged |
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

### Phase 1 - Stabilization
- ✅ Upgrade to .NET 10 and latest MAUI stable version
- ? Fix all known crash bugs (priority: critical ? high ? medium)
- ? Implement global error handling with retry logic
- ? Add network failure handling (offline mode where appropriate)
- ? Validate all async/await patterns for proper error propagation

### Phase 2 - Observability
- ? Integrate Sentry.io SDK for iOS and Android
- ? Configure breadcrumbs for user actions
- ? Set up performance monitoring (app startup, API calls)
- ? Create alerts for crash rate spikes
- ? Build Grafana dashboards for mobile metrics

### Phase 3 - Testing
- ☐ Add unit tests for ViewModels and critical business logic
- ☐ Create manual test matrix for devices and OS versions
- ☐ Perform regression testing on physical devices
- ☐ Load test API integrations
- ☐ Accessibility audit (TalkBack, VoiceOver)

### Phase 4 - UX Improvements
- ☐ UX audit of current app (identify pain points and friction)
- ☐ Redesign navigation structure (bottom tabs, drawer, flow)
- ☐ Refresh visual design (colors, typography, spacing, icons)
- ☐ Improve event discovery and browsing experience
- ☐ Streamline event registration flow
- ☐ Polish litter reporting flow (camera, location, submission)
- ☐ Enhance dashboard/home screen with clear calls to action
- ☐ Consistent loading states and feedback throughout app

### Phase 5 - Documentation
- ☐ Document supported device matrix
- ☐ Create troubleshooting guide for common errors
- ☐ Write runbook for investigating Sentry errors
- ☐ Update developer onboarding documentation

### Phase 6 - App Store Presence
- ☐ Create new screenshots showcasing improved UX
- ☐ Update app store descriptions and feature highlights
- ☐ Refresh app icon and promotional graphics (if needed)
- ☐ Improve App Store (iOS) and Play Store (Android) metadata
- ☐ Add/update app preview video (optional)
- ☐ Respond to and address existing user reviews

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

4. **Should we implement automatic crash reporting opt-out?**
   **Recommendation:** Opt-in by default with privacy policy disclosure
   **Owner:** Legal + Product
   **Due:** Before Sentry phase

5. **Should we implement biometric authentication for returning users?**
   **Recommendation:** Yes, as optional convenience feature (Face ID / Touch ID on iOS, fingerprint on Android)
   **Owner:** Mobile Team
   **Due:** Before Phase 2

6. **What is our push notification strategy (types, frequency, opt-in)?**
   **Recommendation:** Opt-in prompt at first launch; categories: event reminders, team activity, achievements; respect quiet hours (9 PM - 8 AM local); allow per-category opt-out
   **Owner:** Product Lead
   **Due:** Before Phase 5

7. **How do we force critical app updates?**
   **Recommendation:** Minimum version check on app launch via API endpoint; block app usage until updated for critical security patches; soft reminder for feature updates
   **Owner:** Mobile Team + DevOps
   **Due:** Before production release

8. **What data should be cached for offline viewing?**
   **Recommendation:** User profile, registered events (next 30 days), team memberships; read-only mode when offline; queue actions for sync when online
   **Owner:** Mobile Team
   **Due:** Before Phase 1

---

## Related Documents

- **[Project 5 - Deployment Pipelines](./Project_05_Deployment_Pipelines.md)** - CI/CD improvements
- **[Project 24 - API v2 Modernization](./Project_24_API_v2_Modernization.md)** - Auto-generated clients will reduce mobile code
- **[Project 25 - Automated Testing](./Project_25_Automated_Testing.md)** - UI automation for mobile
- **[Mobile Development Guide](../../TrashMobMobile/claude.md)** - MAUI patterns (when created)

---

**Last Updated:** January 24, 2026  
**Owner:** Mobile Product Lead + MAUI Developers  
**Status:** Active Development  
**Next Review:** Regular standups during development
