# TrashMobMobile

TrashMobMobile is a cross-platform mobile application built with .NET MAUI, targeting .NET 9. The app is designed to support community-driven trash cleanup events, allowing users to discover, register, and manage events, track locations, and interact with other participants.

## Features

- **Event Discovery & Details**: Browse upcoming trash cleanup events, view event details including location, date, time, description, and attendee count.
- **Interactive Maps**: Visualize event locations and addresses using custom map pins.
- **Event Registration**: Register or unregister for events directly from the app.
- **Event Management**: Create, edit, and cancel events (for authorized users).
- **Dashboard**: Track completed events and view your participation history.
- **Contact & Support**: Reach out to organizers or support via the Contact Us page.
- **Partner Locations**: View and interact with partner location services.

## Project Structure

- `Views/`: Contains XAML views for pages such as event details, dashboard, and contact forms.
- `ViewModels/`: Implements MVVM pattern, providing data binding and business logic for views.
- `Models/`: Defines data models used throughout the app (e.g., events, addresses, requests).
- `Pages/`: Contains navigation pages and modal dialogs.
- `Controls/`: Custom UI controls, such as map and pin components.
- `App.xaml.cs`: Application entry point and global configuration.

## Getting Started

1. **Requirements**:
   - [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
   - Visual Studio 2022 (latest version recommended)
   - Android/iOS/Mac/Windows device or emulator

2. **Build & Run**:
   - Open the solution in Visual Studio.
   - Restore NuGet packages.
   - Select your target platform and run the app.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request. For major changes, open an issue first to discuss your ideas.

## License

This project is licensed under the MIT License.

---

For more information, visit the [TrashMob-eco GitHub repository](https://github.com/TrashMob-eco/TrashMob).