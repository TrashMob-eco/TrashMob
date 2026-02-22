# TrashMob

## Overview

The `TrashMob` project is the main web application for the TrashMob platform, designed to help communities organize and participate in trash cleanup events. It provides APIs, web pages, and integrations for managing events, users, locations, and reporting.

## Key Contents

- **ASP.NET Core Web Application**: Built with .NET 10, serving both API endpoints and web pages.
- **Single Page Application (SPA)**: Integrated React client app located in `client-app/`.
- **Entity Framework Core**: Data access using SQL Server.
- **Authentication & Authorization**: Entra External ID (CIAM) for secure user management.
- **Azure Integrations**: Key Vault, Blob Storage, and Application Insights for secrets, file storage, and monitoring.
- **Geo Data Support**: NetTopologySuite for spatial and geographic operations.
- **Email Notifications**: SendGrid integration for event and user communications.
- **API Documentation**: Swagger (Swashbuckle) for interactive API docs.

## Project Structure

```
TrashMob/
├── Controllers/       # API and MVC controllers
├── client-app/        # React SPA source code (Vite + TypeScript)
├── Properties/        # Launch settings
├── appsettings.json   # Application configuration
└── Program.cs         # Application entry point
```

## Getting Started

1. Ensure you have [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and [Node.js 20+](https://nodejs.org/) installed.
2. Restore dependencies:
   ```bash
   dotnet restore
   cd client-app && npm install
   ```
3. Run the application:
   ```bash
   dotnet run --environment Development
   ```
4. Access the app at https://localhost:44332
5. View API docs at https://localhost:44332/swagger/index.html

## Related Documentation

- [CLAUDE.md](./CLAUDE.md) - Development patterns and coding standards
- [TrashMob.prd](./TrashMob.prd) - Product requirements document
- [Root README](../README.md) - Main project documentation
- [2026 Product Plan](../Planning/README.md) - Roadmap and initiatives

---

*For questions or contributions, please refer to the main repository [TrashMob-eco/TrashMob](https://github.com/TrashMob-eco/TrashMob).*
