# TrashMob.Shared

## Overview

The `TrashMob.Shared` project provides shared services, managers, and utilities for the TrashMob platform. It acts as a core library for business logic, integrations, and reusable components that support both the web application and other solution projects.

## Key Contents

- **Managers**: Classes for handling images, notifications, and other business operations.
- **Email Templates**: Embedded HTML templates for system-generated emails (located in `Engine/EmailCopy/`).
- **Integrations**: Azure Key Vault, Blob Storage, Azure Maps, and Notification Hubs.
- **Data Access**: Entity Framework Core support for SQL Server and SQLite, including spatial data via NetTopologySuite.
- **Utilities**: Helpers for configuration, image processing (ImageSharp), and email delivery (SendGrid).
- **References**: Depends on `TrashMob.Models` for shared data models.

## Technologies

- .NET 9.0
- Entity Framework Core
- Azure SDKs (Key Vault, Storage, Maps, Notification Hubs)
- SendGrid (email delivery)
- SixLabors.ImageSharp (image processing)

## Usage

Reference this project from other solution components to access shared business logic, integrations, and utilities. Most cross-cutting concerns and reusable features are implemented here.

## Helpful Links

- [Mobile App User Stories](https://github.com/TrashMob-eco/TrashMob/blob/main/MobileAppUserStories.md)
- [Website User Stories](https://github.com/TrashMob-eco/TrashMob/blob/main/WebsiteUserStories.md)

---

*For questions or contributions, please refer to the main repository [TrashMob-eco/TrashMob](https://github.com/TrashMob-eco/TrashMob).*