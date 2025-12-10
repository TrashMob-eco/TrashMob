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

## Special Business Logic

TrashMobMobile implements several business rules to ensure a robust and user-friendly experience:

- **Event Registration & Validation**: Users can only register for events if registration is enabled and they meet eligibility criteria. The app prevents duplicate registrations and enforces event capacity limits.
- **Event Creation & Editing**: When creating or editing events, the app validates required fields (such as date, time, and location) and ensures logical consistency (e.g., end time must be after start time).
- **Address Management**: Custom logic in address view models ensures that only valid and geocoded addresses are displayed on maps and used for event locations.
- **Contact Requests**: The Contact Us feature includes validation for required fields and email format before allowing submission.
- **Partner Location Services**: Integration with partner services includes checks for service availability and user permissions.
- **Event Cancellation**: Only authorized users can cancel events, and cancellation triggers notifications to registered attendees.

These business rules are primarily implemented in the ViewModels layer, leveraging the MVVM pattern for maintainability and testability.

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