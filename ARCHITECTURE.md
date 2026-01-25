# TrashMob Architecture

## System Overview

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                                    CLIENTS                                          │
├─────────────────────┬─────────────────────┬─────────────────────┬──────────────────┤
│   TrashMob Mobile   │   TrashMob Web      │   TrashMob Admin    │   Power BI       │
│   (.NET MAUI)       │   (React + Vite)    │   (React + Vite)    │   Reports        │
│                     │                     │                     │                  │
│   ┌───────────┐     │                     │                     │                  │
│   │ Sentry.io │     │                     │                     │                  │
│   └───────────┘     │                     │                     │                  │
└─────────┬───────────┴──────────┬──────────┴──────────┬──────────┴────────┬─────────┘
          │                      │                     │                   │
          │                      │ HTTPS               │                   │
          ▼                      ▼                     ▼                   │
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                          AZURE CONTAINER APPS ENVIRONMENT                           │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  ┌─────────────────────────────────────────────────────────────────────────────┐   │
│  │                    TrashMob Web API (ASP.NET Core 10)                       │   │
│  │                         Azure Container App                                  │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐ │   │
│  │  │ Controllers │  │  Managers   │  │    EF Core  │  │ React SPA (wwwroot) │ │   │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────────────┘ │   │
│  └─────────────────────────────────────────────────────────────────────────────┘   │
│                                                                                     │
│  ┌────────────────────────────────┐  ┌────────────────────────────────┐            │
│  │   TrashMobDailyJobs            │  │   TrashMobHourlyJobs           │            │
│  │   Azure Container App          │  │   Azure Container App          │            │
│  │   (Scheduled Background Jobs)  │  │   (Scheduled Background Jobs)  │            │
│  └────────────────────────────────┘  └────────────────────────────────┘            │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
          │                      │                     │
          │                      │                     │
          ▼                      ▼                     ▼
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                              AZURE SERVICES                                         │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐                     │
│  │  Azure SQL      │  │  Azure Key      │  │  Azure Blob     │                     │
│  │  Database       │  │  Vault          │  │  Storage        │                     │
│  │                 │  │  (Secrets)      │  │  (Photos)       │                     │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘                     │
│                                                                                     │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐                     │
│  │  Azure AD B2C   │  │  Azure Maps     │  │  Azure          │                     │
│  │  (Auth)         │  │  (Geocoding)    │  │  Notification   │                     │
│  │                 │  │                 │  │  Hub            │                     │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘                     │
│                                                                                     │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐                     │
│  │  Application    │  │  Log Analytics  │  │  Container      │                     │
│  │  Insights       │  │  Workspace      │  │  Registry       │                     │
│  │  (Telemetry)    │  │                 │  │  (ACR)          │                     │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘                     │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
          │                      │
          │                      │
          ▼                      ▼
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                           EXTERNAL SERVICES                                         │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐                     │
│  │  SendGrid       │  │  DocuSign       │  │  Firebase       │                     │
│  │  (Email)        │  │  (Waivers)      │  │  (Push - Mobile)│                     │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘                     │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

## Component Details

### Client Applications

| Component | Technology | Description |
|-----------|------------|-------------|
| **Web Client** | React 19, Vite, TypeScript | Single-page application for event browsing, registration, and management |
| **Mobile App** | .NET MAUI | Cross-platform mobile app for iOS and Android |
| **Admin Portal** | React (part of Web Client) | Site administration interface for managing users, events, and partners |

### Backend Services

| Component | Technology | Description |
|-----------|------------|-------------|
| **Web API** | ASP.NET Core 10, Azure Container App | RESTful API serving web and mobile clients, hosts React SPA |
| **Daily Jobs** | ASP.NET Core 10, Azure Container App | Background jobs running once per day (notifications, cleanup) |
| **Hourly Jobs** | ASP.NET Core 10, Azure Container App | Background jobs running every hour (reminders, stats) |

### Data & Storage

| Service | Purpose |
|---------|---------|
| **Azure SQL Database** | Primary relational database for all application data |
| **Azure Blob Storage** | Photo storage for events, litter reports, user profiles |
| **Azure Key Vault** | Secrets management (connection strings, API keys) |

### Identity & Security

| Service | Purpose |
|---------|---------|
| **Azure AD B2C** | User authentication and identity management |
| **Managed Identity** | Service-to-service authentication within Azure |

### Observability

| Service | Purpose |
|---------|---------|
| **Application Insights** | Web app telemetry, logging, and error tracking |
| **Sentry.io** | Mobile app crash reporting and error tracking |
| **Log Analytics** | Centralized logging and diagnostics |

### External Integrations

| Service | Purpose |
|---------|---------|
| **Azure Maps** | Geocoding, address search, map display |
| **SendGrid** | Transactional email delivery |
| **DocuSign** | Electronic waiver signing |
| **Firebase** | Push notifications for mobile app |
| **Azure Notification Hub** | Push notification orchestration |

## Data Flow

### User Authentication
```
Mobile/Web → Azure AD B2C → JWT Token → API (validates token)
```

### Event Creation
```
User → Web/Mobile → API → SQL Database
                      ↓
              Azure Maps (geocoding)
                      ↓
              Blob Storage (photos)
```

### Notifications
```
Hourly/Daily Jobs → SQL Database (query upcoming events)
                 ↓
         SendGrid (email) / Notification Hub (push)
```

## Deployment

- **Infrastructure**: Bicep templates in `/Deploy`
- **CI/CD**: GitHub Actions workflows
- **Environments**: Development (main branch) → Production (release branch)

## Related Documentation

- [CLAUDE.md](./CLAUDE.md) - Development guide and coding standards
- [Container Deployment Guide](./Deploy/CONTAINER_DEPLOYMENT_GUIDE.md) - Deployment procedures
- [Domain Model](./TrashMob.Models/TrashMob.Models.prd) - Entity relationships
