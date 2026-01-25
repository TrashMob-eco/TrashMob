# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Run Commands

### Backend (.NET 10)
```bash
dotnet build                              # Build solution
dotnet run --environment Development      # Run web API (from TrashMob/ folder)
dotnet test                               # Run tests (from TrashMob.Shared.Tests/)
```

### Frontend (React/Vite)
```bash
cd TrashMob/client-app
npm install        # Install dependencies
npm start          # Dev server
npm run build      # Production build
npm test           # Run Vitest tests
npm run lint       # ESLint with fixes
```

### Database Migrations
```bash
# From TrashMob folder
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### Docker
```bash
docker build -f TrashMob/Dockerfile -t trashmob:latest .
docker build -f TrashMobDailyJobs/Dockerfile -t trashmob-daily-jobs:latest .
docker build -f TrashMobHourlyJobs/Dockerfile -t trashmob-hourly-jobs:latest .
```

## Architecture Overview

**Solution Structure:**
- `TrashMob/` - ASP.NET Core Web API + React SPA frontend (in `client-app/`)
- `TrashMob.Shared/` - Business logic, managers, repositories, EF Core context
- `TrashMob.Models/` - Domain entities and POCOs
- `TrashMob.Shared.Tests/` - xUnit tests
- `TrashMobDailyJobs/` & `TrashMobHourlyJobs/` - Azure Container App background jobs
- `TrashMobMobile/` - .NET MAUI cross-platform mobile app
- `Deploy/` - Bicep templates and deployment scripts

**Key Architectural Patterns:**

1. **Manager Pattern** (`TrashMob.Shared/Managers/`) - Business logic layer
   - `BaseManager<T>` → `KeyedManager<T>` hierarchy
   - Interfaces in `Managers/Interfaces/`

2. **Repository Pattern** (`TrashMob.Shared/Persistence/`) - Data access layer
   - `BaseRepository<T>` → `KeyedRepository<T>` hierarchy
   - `MobDbContext` for entity configuration

3. **Controller Pattern** (`TrashMob/Controllers/`)
   - `BaseController` → `SecureController` hierarchy
   - `SecureController` provides `UserId` from auth claims
   - Authorization via `AuthorizationPolicyConstants`

**Data Flow:** Controller → Manager → Repository → DbContext

## Key Files

| Purpose | Location |
|---------|----------|
| DI Registration | `TrashMob/Program.cs`, `TrashMob.Shared/ServiceBuilder.cs` |
| EF DbContext | `TrashMob.Shared/Persistence/MobDbContext.cs` |
| Migrations | `TrashMob.Shared/Migrations/` |
| Email Templates | `TrashMob.Shared/Engine/EmailCopy/` |
| React Entry | `TrashMob/client-app/src/App.tsx` |
| API Services | `TrashMob/client-app/src/services/` |

## Development Setup

1. Install .NET 10 SDK and Azure CLI
2. Run `az login` to authenticate
3. Add your IP to Dev Azure SQL firewall rules
4. Run: `.\setupdev.ps1 -environment dev -region westus2 -subscription <guid>`
5. Local API: https://localhost:44332
6. Swagger: https://localhost:44332/swagger/index.html

## Key Domain Concepts

| Concept | Description |
|---------|-------------|
| **Event** | A scheduled cleanup activity with location, date, and lead organizer |
| **Volunteer/User** | Registered participant; can be adult (18+) or minor (13+) |
| **Community** | A city/region partnership with branded presence and custom programs |
| **Team** | User-created group with membership and collective impact tracking |
| **Partner** | Organization sponsoring or supporting cleanup efforts |
| **Litter Report** | User-submitted report of litter locations needing attention |
| **Event Summary** | Post-event metrics (bags collected, weight, duration, attendees) |
| **Waiver** | Liability waiver signed by participants before events |

## Technology Stack

- **Backend:** .NET 10, EF Core 10, Azure SQL, Azure B2C (→ Entra External ID), SendGrid, Azure Maps
- **Frontend:** React 18, TypeScript 5.8, Vite 7, Tailwind CSS 4, React Query
- **Mobile:** .NET MAUI (MVVM pattern)
- **Infrastructure:** Azure Container Apps, GitHub Actions, Bicep IaC

## Branching Strategy

- `main` - Development/integration (auto-deploys to dev)
- `release` - Production releases
- `dev/{developer}/{feature}` - Feature branches

## Coding Standards & Patterns

### General Principles
- Follow **.NET coding conventions** and C# style guidelines
- Use **async/await** for all I/O operations
- Implement proper **error handling** with meaningful messages
- Add **XML documentation** for public APIs (required for Swagger)
- Write **unit tests** for business logic (xUnit preferred)

### API Design
- RESTful endpoints with proper HTTP verbs (GET, POST, PUT, DELETE)
- Return appropriate HTTP status codes (200, 201, 400, 401, 403, 404, 500)
- Use **DTOs** for request/response to decouple from database models
- Implement **pagination** for list endpoints
- Add **authentication/authorization** attributes where needed

### Database & EF Core
- Use **migrations** for schema changes (never manual SQL)
- Implement **soft deletes** where appropriate (IsDeleted flag)
- Add **audit fields**: CreatedDate, CreatedByUserId, LastUpdatedDate, LastUpdatedByUserId
- Use **GUIDs** for primary keys where cross-system references needed
- Proper **indexes** on foreign keys and frequently queried columns

### Error Handling
- Use **try-catch** blocks around external service calls
- Log errors with **structured logging** (Application Insights for web, Sentry.io for mobile)
- Return **user-friendly error messages** (never expose stack traces to users)
- Implement **retry logic** for transient failures

### Mobile (MAUI)
- Follow **MVVM pattern** (Model-View-ViewModel)
- Use **platform-specific** code only when necessary
- Implement **offline support** where feasible
- Handle **network connectivity** gracefully
- Target **crash-free sessions ≥ 99.5%**

## Security & Privacy

### Authentication & Authorization
- Transitioning from **Azure B2C** to **Entra External ID**
- Support **SSO** for partner communities
- Implement **role-based access control** (Admin, Event Lead, User)
- Special handling for **minors (13+)**: parental consent, visibility restrictions

### Data Protection
- **COPPA compliance** for minors
- No PII exposed unnecessarily
- **Waiver storage** with legal retention requirements
- **Photo moderation** pipeline for user-generated content

### API Security
- All endpoints require **authentication** except public read-only data
- Validate **user permissions** before allowing data access/modification
- Implement **rate limiting** to prevent abuse
- Use **HTTPS** only

## Accessibility

- Commit to **WCAG 2.2 AA** compliance
- Semantic HTML on web
- Proper screen reader support on mobile
- Keyboard navigation on web
- Sufficient color contrast ratios

## Key 2026 Initiatives

Refer to `Planning/README.md` for detailed roadmap. Priority areas:

1. **Project 1:** Auth migration (Azure B2C → Entra External ID)
2. **Project 4:** Mobile stabilization and error handling
3. **Project 7:** Event weight tracking (Phase 1 & 2)
4. **Project 9:** Teams feature (MVP)
5. **Project 10:** Community Pages (MVP)

## Common Patterns

### Adding a new API endpoint
1. Create/update model in `TrashMob.Models/`
2. Add interface method to `IXxxManager` in `TrashMob.Shared/Managers/Interfaces/`
3. Implement in manager class in `TrashMob.Shared/Managers/`
4. Add controller method in `TrashMob/Controllers/`
5. Add React Query service in `TrashMob/client-app/src/services/`

### ServiceResult Pattern
For operations needing specific error messages:
```csharp
public async Task<ServiceResult<T>> DoSomethingAsync(...) {
    if (invalid) return ServiceResult<T>.Failure("Specific error");
    return ServiceResult<T>.Success(result);
}
```

## Troubleshooting

**Data doesn't load locally:** Check Azure SQL firewall rules - your IP may have changed. Look for actual IP in VS Code debug output.

**Email not sending:** `dotnet user-secrets set "sendGridApiKey" "x"` to disable, or use real key from dev KeyVault.

## Testing

- **Unit Tests:** Business logic in services
- **Integration Tests:** API endpoints with test database
- **Manual Testing:** Mobile apps on physical devices
- Target **change failure rate ≤ 10%**

## Performance Goals

- **P95 API latency:** ≤ 300ms
- **Crash-free sessions (mobile):** ≥ 99.5%
- **Database queries:** Use proper indexing, avoid N+1 queries
- **Caching:** Implement where appropriate (Redis consideration)

## Observability

- **Mobile App:** Sentry.io for error tracking and crash reporting
- **Web App:** Azure Application Insights SDK for telemetry, logging, and error tracking
- **Structured logging** with context
- **Business event tracking** (signups, event creation, attendance)
- **Dashboards** for key metrics
- **Alerting** for critical issues

**Future:** Migrate web app from Application Insights SDK to OpenTelemetry for vendor-neutral observability

## Infrastructure & Custom Domain

### Azure Resources

| Environment | Container App | Environment | Resource Group | Custom Domain |
|-------------|---------------|-------------|----------------|---------------|
| **Production** | `ca-tm-pr-westus2` | `cae-tm-pr-westus2` | `rg-trashmob-pr-westus2` | `www.trashmob.eco` |
| **Development** | `ca-tm-dev-westus2` | `cae-tm-dev-westus2` | `rg-trashmob-dev-westus2` | `dev.trashmob.eco` |

### Custom Domain & SSL Certificate

Both sites use Azure-managed SSL certificates bound to Container Apps. Managed certificates **auto-renew automatically** - no manual intervention required.

**Verify current certificate binding:**
```bash
# Production
az containerapp hostname list --name ca-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2

# Development
az containerapp hostname list --name ca-tm-dev-westus2 --resource-group rg-trashmob-dev-westus2
```

**Check certificate status:**
```bash
# Production
az containerapp env certificate list --name cae-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2 --query "[?name=='trashmob-eco-cert']"

# Development
az containerapp env certificate list --name cae-tm-dev-westus2 --resource-group rg-trashmob-dev-westus2 --query "[?name=='dev-trashmob-eco-cert']"
```

**If certificate needs to be recreated** (rare - only if deleted or corrupted):
```bash
# 1. Add TXT record for domain verification (get token from Azure Portal or CLI error message)
#    Name: asuid.www.trashmob.eco
#    Value: <verification-token>

# 2. Add hostname to container app
az containerapp hostname add \
  --name ca-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --hostname www.trashmob.eco

# 3. Create managed certificate (takes 2-3 minutes to provision)
az containerapp env certificate create \
  --name cae-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --hostname www.trashmob.eco \
  --certificate-name trashmob-eco-cert \
  --validation-method CNAME

# 4. Bind certificate to hostname
az containerapp hostname bind \
  --name ca-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --hostname www.trashmob.eco \
  --certificate trashmob-eco-cert \
  --environment cae-tm-pr-westus2
```

**DNS Requirements:**

| Environment | Record Type | Name | Value |
|-------------|-------------|------|-------|
| Production | CNAME | `www` | `ca-tm-pr-westus2.greenground-fd8fc385.westus2.azurecontainerapps.io` |
| Production | TXT | `asuid.www` | domain verification token (initial setup only) |
| Development | CNAME | `dev` | `ca-tm-dev-westus2.ashypebble-059d2628.westus2.azurecontainerapps.io` |
| Development | TXT | `asuid.dev` | domain verification token (initial setup only) |

**DNS Management:** DNS records for trashmob.eco are managed in [Microsoft 365 Admin Center](https://admin.cloud.microsoft) under Domains.

See `Deploy/CUSTOM_DOMAIN_MIGRATION.md` for full migration documentation.

## Additional Resources

- **2026 Planning:** `Planning/README.md` - Navigation hub for all planning docs
- **Individual Projects:** `Planning/Projects/` - 25 detailed project specifications
- **Product Plan:** `Planning/README.md` - Master roadmap document
- **Domain Model:** `TrashMob.Models/TrashMob.Models.prd` - Entity relationships and business rules
- **Test Scenarios:** `TestScenarios.md` - Manual test cases (automation planned)
- **Container Deployment:** `Deploy/CONTAINER_DEPLOYMENT_GUIDE.md`
