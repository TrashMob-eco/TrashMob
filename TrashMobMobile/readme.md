# TrashMobMobile

TrashMobMobile is a cross-platform mobile application built with .NET MAUI, targeting .NET 10. The app is designed to support community-driven trash cleanup events, allowing users to discover, register, and manage events, track locations, and interact with other participants.

## Features

- **Event Discovery & Details**: Browse upcoming trash cleanup events, view event details including location, date, time, description, and attendee count.
- **Interactive Maps**: Visualize event locations and addresses using custom map pins.
- **Event Registration**: Register or unregister for events directly from the app.
- **Event Management**: Create, edit, and cancel events (for authorized users).
- **Litter Reports**: Create geotagged litter reports with photos.
- **Dashboard**: Track completed events and view your participation history.
- **Contact & Support**: Reach out to organizers or support via the Contact Us page.
- **Partner Locations**: View and interact with partner location services.

## Project Structure

```
TrashMobMobile/
├── Views/           # XAML views (event details, dashboard, forms)
├── ViewModels/      # MVVM pattern - data binding and business logic
├── Models/          # Data models (events, addresses, requests)
├── Pages/           # Navigation pages and modal dialogs
├── Controls/        # Custom UI controls (maps, pins)
├── Services/        # API communication and local services
├── Platforms/       # Platform-specific code (Android, iOS, Mac)
└── App.xaml.cs      # Application entry point
```

## Getting Started

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2022 (latest version) with .NET MAUI workload
- Android Studio (for Android emulator)
- Xcode (for iOS development on Mac)

### Build & Run

1. Open `TrashMobMobileApp.sln` in Visual Studio
2. Restore NuGet packages
3. Configure Google Maps API key (see below)
4. Select your target platform and run

### Google Maps API Key

For Android, update `Platforms/Android/AndroidManifest.xml`:
```xml
<meta-data android:name="com.google.android.geo.API_KEY" android:value="YOUR_KEY_HERE" />
```

Get the dev key from the [Dev KeyVault](https://portal.azure.com/#@jobeedevids.onmicrosoft.com/asset/Microsoft_Azure_KeyVault/Secret/https://kv-tm-dev-westus2.vault.azure.net/secrets/Android-Google-ApiKey-Dev) or create your own at [Google Cloud Console](https://developers.google.com/maps/gmp-get-started).

**Never commit API keys to the repository!**

## Related Documentation

- [CLAUDE.md](./CLAUDE.md) - Mobile-specific development patterns
- [TrashMobMobile.prd](./TrashMobMobile.prd) - Product requirements document
- [Project Analysis](./docs/PROJECT_ANALYSIS.md) - Detailed architecture analysis
- [Root README](../README.md) - Main project documentation
- [2026 Product Plan](../Planning/README.md) - Roadmap

## Test Builds

- **Android Internal Testing**: Request access at [info@trashmob.eco](mailto:info@trashmob.eco)
- **iOS TestFlight**: Request access at [info@trashmob.eco](mailto:info@trashmob.eco)

## Production Apps

- [Google Play Store](https://play.google.com/store/apps/details?id=eco.trashmob.trashmobmobileapp)
- [Apple App Store](https://apps.apple.com/us/app/trashmob/id1599996743)

## Contributing

Contributions are welcome! Please:
1. Check [open issues](https://github.com/TrashMob-eco/TrashMob/issues?q=is%3Aissue+is%3Aopen+label%3A%22mobile%22)
2. Read the [CONTRIBUTING guide](../CONTRIBUTING.md)
3. For major changes, open an issue first to discuss

## License

This project is licensed under the MIT License.

---

For more information, visit the [TrashMob-eco GitHub repository](https://github.com/TrashMob-eco/TrashMob).
