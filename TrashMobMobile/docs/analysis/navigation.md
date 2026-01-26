# Navigation and Routing

## Shell structure

- `AppShell` is the root navigation container.
- The Flyout menu hosts the `Home` item (MainPage) and menu entries for Dashboard, Location, Privacy/Terms, Waiver, Contact, and Sign out.

## Route registration

`AppShell` registers routes by page type name, for example:

- `CreateEventPage`, `EditEventPage`, `ViewEventPage`
- `CreateLitterReportPage`, `ViewLitterReportPage`
- `MyDashboardPage`, `WelcomePage`, `LogoutPage`

This enables navigation via `Shell.Current.GoToAsync("{nameof(Page)}?Param=...")`.

## Parameterized navigation

- `MainPage` navigates to event or litter report details based on map pin `AutomationId` (format: `AddressType:Id`).
- `ViewEventPage` accepts `EventId` as a query parameter.
- `ViewLitterReportPage` accepts `LitterReportId`.
- Create/edit flows pass relevant IDs through query parameters.

## View composition

- ContentPages live in `Pages/`.
- Sub-views for tab layouts live in `Views/` and are composed with `Sharpnado.Tabs`.

## Typical user flows

- Landing: `MainPage` shows stats, map/list toggle, and action buttons.
- Event details: `ViewEventPage` uses tabs for details, partners, attendees, and litter reports.
- Dashboard: `MyDashboardPage` for a user’s history and event participation.
- Location setup: `SetUserLocationPreferencePage` for travel distance and address.
