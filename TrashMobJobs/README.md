# TrashMobJobs

## Overview

The `TrashMobJobs` project contains background jobs and Azure Functions for the TrashMob platform. These jobs automate key tasks such as user management, notifications, and integration with external services, supporting the main web application and improving overall system reliability.

## Key Contents

- **Azure Functions**: Implements serverless jobs, including user deletion (`DeleteUser`) and user notifications (`UserNotifier`).
- **Active Directory Integration**: Manages user lifecycle operations via the `IActiveDirectoryManager` interface.
- **Logging & Error Handling**: Uses structured logging and robust error handling to ensure job reliability.
- **.NET 9 & C# 13**: Leverages modern .NET and C# features for performance and maintainability.

## Project Structure

- `DeleteUser.cs` – Azure Function for deleting users from Active Directory.
- `UserNotifier.cs` – Azure Function for sending notifications to users.
- `Program.cs` – Entry point for configuring and running the job host.

## Usage

1. Ensure you have [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installed.
2. Restore dependencies:
    ```sh
    dotnet restore
    ```
3. Build and run the project locally:
    ```sh
    dotnet build
    dotnet run
    ```
4. Deploy to Azure Functions for production use.

## Helpful Links

- [Mobile App User Stories](https://github.com/TrashMob-eco/TrashMob/blob/main/MobileAppUserStories.md)
- [Website User Stories](https://github.com/TrashMob-eco/TrashMob/blob/main/WebsiteUserStories.md)

---

*For questions or contributions, please refer to the main repository [TrashMob-eco/TrashMob](https://github.com/TrashMob-eco/TrashMob).*