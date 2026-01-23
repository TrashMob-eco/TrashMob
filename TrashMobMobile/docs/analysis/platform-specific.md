# Platform-Specific Behavior

## Custom map handlers

Platform-specific `CustomMapHandler` implementations override pin rendering and map behaviors:

- `Platforms/Android/CustomMapHandler.cs`
  - Uses Google Maps SDK.
  - Manually sets the initial map span to work around MAUI map centering issues.
  - Supports custom pin icons from `CustomPin.ImageSource`.

- `Platforms/iOS/CustomMapHandler.cs` and `Platforms/MacCatalyst/CustomMapHandler.cs`
  - Uses MapKit annotations.
  - Supports custom pin images and info window interaction.
  - Attaches tap gesture recognizers to trigger pin info actions.

## Resource notes

- Android color resources may mirror `Resources/Styles/Colors.xaml`.
- App icons, splash screens, and images are defined in `Resources/`.
