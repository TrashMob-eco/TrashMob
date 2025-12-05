# TrashMob

## Overview

The `TrashMob` project is the main web application for the TrashMob platform, designed to help communities organize and participate in trash cleanup events. It provides APIs, web pages, and integrations for managing events, users, locations, and reporting.

## Key Contents

- **ASP.NET Core Web Application**: Built with .NET 9, serving both API endpoints and web pages.
- **Single Page Application (SPA)**: Integrated React client app located in `client-app/`.
- **Entity Framework Core**: Data access using SQL Server and SQLite providers.
- **Authentication & Authorization**: Uses Microsoft Identity for secure user management.
- **Azure Integrations**: Key Vault, Blob Storage, and Application Insights for secrets, file storage, and monitoring.
- **Geo Data Support**: NetTopologySuite for spatial and geographic operations.
- **Email Notifications**: SendGrid integration for event and user communications.
- **API Documentation**: Swagger (Swashbuckle) for interactive API docs.

## Project Structure

- `Controllers/` - API and MVC controllers for handling requests.
- `client-app/` - React SPA source code.
- `appsettings.json` - Application configuration.
- `Models/` - Data models and DTOs.
- `wwwroot/` - Static web assets.
- `Properties/` - Project properties and launch settings.

## Helpful Links

- [Mobile App User Stories](https://github.com/TrashMob-eco/TrashMob/blob/main/MobileAppUserStories.md)
- [Website User Stories](https://github.com/TrashMob-eco/TrashMob/blob/main/WebsiteUserStories.md)

## Getting Started

1. Ensure you have [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) and [Node.js](https://nodejs.org/) installed.
2. Restore dependencies:


