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

## CI/CD & App Store Deployment

### Workflows

| Workflow | Branch | Environment | Purpose |
|----------|--------|-------------|---------|
| `main_trashmobmobileapp.yml` | `main` | `test` | Dev builds → TestFlight & Google Play internal |
| `release_trashmobmobileapp.yml` | `release` | `production` | Production releases |

### iOS TestFlight Upload

The iOS deploy uses `xcrun altool --upload-app` to upload IPA files to TestFlight. This replaced the `apple-actions/upload-testflight-build@v4` action because `iTMSTransporter` is broken on GitHub Actions macos runners (error -10814).

**Note:** `altool` is deprecated by Apple but still functional. If Apple removes it in a future Xcode release, switch to `fastlane pilot upload` (available on the runners) or wait for an `iTMSTransporter` fix.

### App Store Connect API Key

The upload requires an App Store Connect API key with **Admin** or **App Manager** role.

| GitHub Secret | Description |
|---------------|-------------|
| `APPSTORE_KEY_ID` | API key ID (10 chars, e.g. `X8J78CDY98`) |
| `APPSTORE_ISSUER_ID` | Issuer ID (UUID, from App Store Connect API keys page) |
| `APPSTORE_PRIVATE_KEY` | Full `.p8` file contents including BEGIN/END headers |

Manage keys at [App Store Connect → Users and Access → Integrations → App Store Connect API](https://appstoreconnect.apple.com/access/integrations/api).

**Important:** These are *App Store Connect API keys*, not Apple Sign In keys. The Sign In keys (for Entra External ID) are a separate key type managed under Services in the Apple Developer portal.

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
