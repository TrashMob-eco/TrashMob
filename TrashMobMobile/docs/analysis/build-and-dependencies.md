# Build, Targets, and Dependencies

## Target frameworks

- Android, iOS, MacCatalyst targets using .NET 10 (`net10.0-*`).
- Windows and Tizen targets are present but commented out in the project file.

## Key dependencies

- `CommunityToolkit.Maui` and `CommunityToolkit.Mvvm` for UI behaviors and MVVM.
- `Microsoft.Identity.Client` (MSAL) for Entra External ID authentication.
- `Microsoft.Maui.Controls.Maps` for map support.
- `Sharpnado.Tabs.Maui` for tabbed navigation UI.
- `Sentry.Maui` for crash/error reporting.
- `Polly` for HTTP retry policies.
- `Newtonsoft.Json` for JSON serialization in REST services.

## Build configuration notes

- `USETEST` compile constant is enabled in the project file and controls API endpoints and B2C configuration.
- App ID and display name are set in the csproj (`eco.trashmob.trashmobmobileapp`).
