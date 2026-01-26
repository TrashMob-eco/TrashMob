# TrashMob.Shared

## Overview

The `TrashMob.Shared` project provides shared services, managers, and utilities for the TrashMob platform. It acts as a core library for business logic, integrations, and reusable components that support both the web application and other solution projects.

## Key Contents

- **Managers**: Business logic layer for events, users, partners, litter reports, etc.
- **Repositories**: Data access layer using Entity Framework Core.
- **Email Templates**: Embedded HTML templates for system-generated emails (located in `Engine/EmailCopy/`).
- **Integrations**: Azure Key Vault, Blob Storage, Azure Maps, and Notification Hubs.
- **Utilities**: Helpers for configuration, image processing (ImageSharp), and email delivery (SendGrid).

## Project Structure

```
TrashMob.Shared/
├── Managers/          # Business logic layer
│   └── Interfaces/    # Manager interfaces
├── Persistence/       # EF Core DbContext and repositories
├── Engine/            # Background job logic and email templates
├── Poco/              # Plain old C# objects (DTOs, ServiceResult)
└── Migrations/        # EF Core database migrations
```

## Technologies

- .NET 10
- Entity Framework Core 10
- Azure SDKs (Key Vault, Storage, Maps, Notification Hubs)
- SendGrid (email delivery)
- SixLabors.ImageSharp (image processing)

## Key Patterns

- **Manager Pattern**: `BaseManager<T>` → `KeyedManager<T>` hierarchy
- **Repository Pattern**: `BaseRepository<T>` → `KeyedRepository<T>` hierarchy
- **ServiceResult Pattern**: For operations returning detailed success/error info

## Related Documentation

- [Root CLAUDE.md](../CLAUDE.md) - Architecture overview and coding standards
- [Domain Model](../TrashMob.Models/TrashMob.Models.prd) - Core domain concepts and business rules
- [TrashMob.Models README](../TrashMob.Models/README.md) - Entity definitions

---

*For questions or contributions, please refer to the main repository [TrashMob-eco/TrashMob](https://github.com/TrashMob-eco/TrashMob).*
