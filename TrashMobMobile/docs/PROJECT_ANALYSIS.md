# TrashMobMobile Project Analysis (Stage 1)

This document is the entry point for a deep analysis of the TrashMobMobile .NET MAUI app. It links to focused notes about architecture, startup flow, navigation, services, and platform-specific behavior.

## Scope

- Repository: TrashMobMobile (mobile app only)
- Target frameworks: Android, iOS, MacCatalyst (.NET 10.0)
- Architecture: MVVM with CommunityToolkit.Mvvm

If you want the analysis to include the broader TrashMob ecosystem (API/web/backend), we can add cross-repo context later.

## Analysis Index

- docs/analysis/architecture.md
- docs/analysis/app-startup-and-di.md
- docs/analysis/navigation.md
- docs/analysis/authentication.md
- docs/analysis/api-and-services.md
- docs/analysis/ui-and-controls.md
- docs/analysis/platform-specific.md
- docs/analysis/models-and-data.md
- docs/analysis/build-and-dependencies.md
- docs/analysis/open-questions.md

## How to use this documentation

- Start with `docs/analysis/architecture.md` for the high-level system map.
- Then read `docs/analysis/app-startup-and-di.md` and `docs/analysis/navigation.md` to understand runtime flow.
- Use `docs/analysis/api-and-services.md` and `docs/analysis/authentication.md` when working on backend integration.
- Consult `docs/analysis/ui-and-controls.md` and `docs/analysis/platform-specific.md` for UI and native details.
