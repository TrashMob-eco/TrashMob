# Architecture Overview

## High-level shape

TrashMobMobile is a .NET MAUI app that follows an MVVM pattern with a service layer wrapping REST calls. The main flow is:

1. App startup configures DI, handlers, and shared services in `MauiProgram`.
2. `App` sets up global error logging and launches `AppShell`.
3. `AppShell` registers routes and presents the Shell Flyout navigation.
4. Pages bind to ViewModels that call Managers/Services to fetch and mutate data.

## Key layers

- UI: XAML pages in `Pages/` and `Views/`, with shared styles in `Resources/Styles/`.
- ViewModels: `ViewModels/` uses `CommunityToolkit.Mvvm` for observable state and commands.
- Services: `Services/` provides REST service implementations and manager facades.
- Authentication: `Authentication/` integrates MSAL for Entra External ID and stores user context.
- Models: shared domain models are from `../TrashMob.Models`; mobile-specific models live in `Models/`.

## Responsibilities by folder

- `App.xaml` and `App.xaml.cs`: app resources and global exception logging.
- `AppShell.xaml` and `AppShell.xaml.cs`: Shell UI, flyout items, and route registration.
- `Pages/`: top-level screens (ContentPage) including create/edit flows.
- `Views/`: nested XAML views for tabbed and sub-screen layouts.
- `ViewModels/`: UI state and business logic; typically orchestrates service calls.
- `Services/`: REST clients, managers, and cross-cutting utilities (notifications, logging).
- `Controls/`: custom UI components (custom map, picker variants, custom pins).
- `Platforms/`: platform-specific handlers (custom map rendering).
- `Config/Settings.cs`: API base URLs and default values.

## Data flow summary

- ViewModels request data via managers (e.g., `IMobEventManager`).
- Managers delegate to REST services (e.g., `MobEventRestService`) that use `HttpClientFactory`.
- REST services build requests using `Settings.ApiBaseUrl` and return deserialized models.
- ViewModels convert shared models into UI-friendly ViewModels using extension methods in `Extensions/`.
