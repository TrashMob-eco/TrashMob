# Models and Data

## Shared models

- The app references `../TrashMob.Models`, which appears to contain domain models such as `Event`, `EventSummary`, `User`, and `Location`.
- ViewModels typically map shared models into UI-specific representations using extension methods in `Extensions/`.

## Mobile-specific models

`Models/` contains request/response helpers and value objects used by the mobile app:

- `EventCancellationRequest` for event cancellation payloads.
- `EnvelopeRequest` and `EnvelopeResponse` for wrapping API interactions.
- `ImageUpload` and `PickupLocationImage` for image handling.
- `WaiverVersion` for client-side waiver tracking.

## ViewModel adapters

- Many ViewModels implement `ToXViewModel` conversion helpers via extension methods in `Extensions/`.
- `AddressViewModel` is used to normalize map display and pin metadata across events and litter reports.
