# API and Services

## REST service pattern

- Each REST client inherits from `RestServiceBase`.
- `RestServiceBase` sets the API base URL, configures JSON options, and exposes:
  - `AuthorizedHttpClient` (adds bearer token)
  - `AnonymousHttpClient`
- Controllers are defined per-service via `protected override string Controller`.

## Manager layer

Managers wrap REST services and provide small orchestration helpers, for example:

- `MobEventManager` wraps event summary, attendee, and event APIs.
- `LitterReportManager`, `PickupLocationManager`, `WaiverManager` follow the same pattern.

The manager layer is typically injected into ViewModels rather than REST clients directly.

## HTTP behavior

- JSON is handled by `Newtonsoft.Json` in most REST services.
- Retry policy uses Polly exponential backoff and also retries 404s.
- Some services use `JsonContent.Create` for request bodies and `EnsureSuccessStatusCode()` for errors.

## Example endpoints

- `events/pagedfilteredevents` for filtered event search.
- `events/userevents/{userId}/{showFutureEventsOnly}` for user events.
- `eventsummaries/v2/{eventId}` for event summary, with a 404 fallback to defaults.
- `users/getuserbyemail/{email}` for user lookup.

## Shared domain models

- The app consumes models from `TrashMob.Models` (project reference).
- Additional mobile-specific models live in `Models/` (e.g., `EventCancellationRequest`).
