# App Startup and Dependency Injection

## Entry points

- `MauiProgram.CreateMauiApp()` configures the app, registers services/pages/viewmodels, and wires platform handlers.
- `App` sets `MainPage = new AppShell()` and attaches global exception logging.
- `AppShell` registers route names for navigation and sets the flyout UI.

## Maui configuration highlights

- `UseMauiMaps()` and a custom `CustomMapHandler` for platform-specific pin behavior.
- `UseSharpnadoTabs()` for tabbed views in detail screens.
- `CommunityToolkit.Maui` for UI behaviors and `CommunityToolkit.Mvvm` for MVVM.
- Fonts: OpenSans + Lexend + Google Material Icons.
- Sentry initialized with DSN and tracing enabled.

## Dependency injection

- Services registered in `AddTrashMobServices()` include:
  - Managers (`IMobEventManager`, `ILitterReportManager`, etc.)
  - REST services (`IMobEventRestService`, `IUserRestService`, etc.)
  - User/auth helpers (`IAuthService`, `IUserService`)
  - UI support (`INotificationService`)
- Pages and ViewModels are registered as transient services in `MauiProgram`.

## HTTP client setup

- `AddRestClientServices()` registers two `HttpClient` instances:
  - `ServerAPI` for authenticated requests with retry policy and Sentry handler.
  - `ServerAPI.Anonymous` for public endpoints.
- A `BaseAddressAuthorizationMessageHandler` injects a bearer token from `UserState`.
- Retry policy uses Polly with exponential backoff on transient failures and 404s.

## Environment and build flags

- `USETEST` compile constant toggles API base URLs and Azure B2C client settings.
- Target frameworks are net10.0 for Android, iOS, and MacCatalyst.
