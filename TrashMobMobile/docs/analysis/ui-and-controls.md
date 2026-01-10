# UI, Styles, and Controls

## Styling and themes

- Global colors and styles are defined in `Resources/Styles/Colors.xaml` and `Resources/Styles/Styles.xaml`.
- Fonts include Lexend and OpenSans; icons come from Google Material Icons.
- Light/dark theme bindings use `AppThemeBinding` for most core components.

## Custom controls

- `CustomMap` extends `Map` to expose `InitialMapSpanAndroid` for reliable map centering.
- `CustomPin` supports custom pin images per address type.
- `TMEntry`, `TMEditor`, `TMDatePicker`, `TMTimePicker`, `TMPicker` provide consistent input styling.

## UI composition patterns

- Primary pages live in `Pages/` and bind directly to ViewModels via `x:DataType`.
- Tabbed views use `Sharpnado.Tabs` and are composed from `Views/` (e.g., `Views/ViewEvent/*`).
- `MainPage` supports map vs list view toggles and uses `CustomMap` to render event pins.

## Notifications

- `NotificationService` uses CommunityToolkit Toast and Snackbar for user feedback.
- Errors show a red snackbar with a fixed duration.
