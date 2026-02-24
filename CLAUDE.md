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

## Architecture Overview

**Solution Structure:**
- `TrashMob/` - ASP.NET Core Web API + React SPA frontend (in `client-app/`)
- `TrashMob.Shared/` - Business logic, managers, repositories, EF Core context
- `TrashMob.Models/` - Domain entities and POCOs
- `TrashMob.Shared.Tests/` - xUnit tests
- `TrashMobDailyJobs/` & `TrashMobHourlyJobs/` - Azure Container App background jobs
- `TrashMobMobile/` - .NET MAUI cross-platform mobile app
- `Deploy/` - Bicep templates and deployment scripts
- `Strapi/` - Strapi CMS for content management

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
| Auth Handler | `TrashMob/Security/UserIsValidUserAuthHandler.cs` |
| CIAM Graph Service | `TrashMob/Services/CiamGraphService.cs` |
| Frontend Auth | `TrashMob/client-app/src/store/AuthStore.tsx` |
| Frontend Login Hook | `TrashMob/client-app/src/hooks/useLogin.ts` |

## Development Setup

### Prerequisites
1. Install .NET 10 SDK and Azure CLI
2. Run `az login` to authenticate
3. Add your IP to Dev Azure SQL firewall rules
4. Run: `.\setupdev.ps1 -environment dev -region westus2 -subscription <guid>`

### Running Locally

**Full-stack developers** (running both frontend and backend):
```bash
# Terminal 1: Start backend
cd TrashMob
dotnet run --environment Development
# API available at https://localhost:44332

# Terminal 2: Start frontend
cd TrashMob/client-app
npm start
# Frontend at http://localhost:3000, API calls proxy to localhost:44332
```

**UX/Frontend developers** (frontend only, using dev server for API):
```bash
cd TrashMob/client-app

# Create .env.local to point to dev server
echo "VITE_API_URL=https://dev.trashmob.eco/api" > .env.local

npm start
# Frontend at http://localhost:3000, API calls go to dev.trashmob.eco
```

### Local URLs
- **Local API:** https://localhost:44332
- **Swagger:** https://localhost:44332/swagger/index.html
- **Frontend (Vite):** http://localhost:3000

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

- **Backend:** .NET 10, EF Core 10, Azure SQL, Entra External ID (CIAM), SendGrid, Azure Maps
- **Frontend:** React 18, TypeScript 5.8, Vite 7, Tailwind CSS 4, TanStack React Query, Radix UI, Zod, React Hook Form
- **Mobile:** .NET MAUI with CommunityToolkit.Mvvm (MVVM + source generators), Sentry.io for crash reporting
- **Infrastructure:** Azure Container Apps, GitHub Actions, Bicep IaC

## Branching Strategy

- `main` - Development/integration (auto-deploys to dev)
- `release` - Production releases
- `dev/{developer}/{feature}` - Feature branches

**IMPORTANT: Never push directly to main.** All changes must go through a pull request, even hotfixes. Create a feature branch, push it, and create a PR for review.

## AI Assistant Boundaries

- **Do not make autonomous structural changes** to project plans (e.g., moving features between projects, removing rollout strategies, reorganizing phases) unless explicitly asked. Suggest improvements first and wait for approval.
- **Scope changes to what's requested.** A bug fix doesn't need surrounding code cleaned up. A planning doc update doesn't need other docs refactored.
- **When debugging, explain reasoning before trying fixes.** List what you think the problem is and your proposed approach before making changes — especially for unfamiliar frameworks (Strapi, MAUI, Bicep).

## Coding Standards & Patterns

### C# Conventions (Backend + Mobile)
- Use **primary constructors** (C# 12) for all new classes — controllers, managers, ViewModels, repositories
- Use **async/await** with **`CancellationToken`** on all async method signatures
- Use **collection expressions** (`[item1, item2]`) instead of `new List<T> { }` or `.ToList()` where appropriate
- Use **structured logging** with `LoggerMessage` source generators or message templates (not string interpolation)
- Use `== null` / `!= null` in EF Core LINQ expressions (not `is null` / `is not null` — causes CS8122)
- Add **XML documentation** for public APIs (required for Swagger)
- Write **unit tests** for business logic (xUnit, 450+ tests)

### API Design
- RESTful endpoints with proper HTTP verbs (GET, POST, PUT, DELETE)
- Return appropriate HTTP status codes (200, 201, 400, 401, 403, 404, 500)
- Use **DTOs** (in `TrashMob.Models/Poco/`) for request/response to decouple from database models
- Add **`[ProducesResponseType]`** attributes for all responses (required for Swagger)
- Add **authentication/authorization** attributes: `[Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]`
- Use **`[RequiredScope(Constants.TrashMobWriteScope)]`** on write endpoints
- Call **`TrackEvent()`** for telemetry on mutation operations

### Database & EF Core
- Use **migrations** for schema changes (never manual SQL)
- Implement **soft deletes** where appropriate (IsDeleted flag)
- Add **audit fields**: CreatedDate, CreatedByUserId, LastUpdatedDate, LastUpdatedByUserId
- Use **GUIDs** for primary keys where cross-system references needed
- Proper **indexes** on foreign keys and frequently queried columns

### Error Handling
- Log errors with **structured logging** (Application Insights for web, Sentry.io for mobile)
- Return **user-friendly error messages** (never expose stack traces to users)

### Mobile (MAUI)
- Follow **MVVM pattern** using **CommunityToolkit.Mvvm** source generators
- Use **`[ObservableProperty]`** for bindable properties, **`[RelayCommand]`** for commands
- Use **primary constructors** for ViewModels: `public partial class XxxViewModel(...) : BaseViewModel(notificationService)`
- Initialize ViewModel data in **`OnNavigatedTo`**, not in the constructor
- Error handling via `BaseViewModel.ExecuteAsync()` which wraps Sentry capture

See component-level CLAUDE.md files for detailed patterns: `TrashMob/CLAUDE.md`, `TrashMobMobile/CLAUDE.md`

## Security & Privacy

### Authentication & Authorization
- **Entra External ID (CIAM)** — production since February 2026
- **CIAM token behavior:** id_tokens lack `email` claim; access_tokens include it. Backend resolves users via 4-step process: email → ObjectId → Graph API → auto-create (`UserIsValidUserAuthHandler.cs`)
- **Graph API service:** `CiamGraphService` uses `User.Read.All` (Application) to resolve emails from CIAM directory. Requires `AzureAdEntra:ClientSecret` in Key Vault.
- **Frontend auth:** `validateToken()` accepts tokens with `email` or `oid`; `useLogin.ts` falls back to OID-based lookup when email missing from id_token
- Support **SSO** for partner communities
- Implement **role-based access control** (Admin, Event Lead, User)
- Special handling for **minors (13+)**: parental consent, visibility restrictions

### Data Protection
- **COPPA compliance** for minors
- No PII exposed unnecessarily
- **Waiver storage** with legal retention requirements
- **Photo moderation** pipeline for user-generated content

## Accessibility

- Commit to **WCAG 2.2 AA** compliance
- Semantic HTML on web
- Proper screen reader support on mobile
- Keyboard navigation on web
- Sufficient color contrast ratios

## Common Patterns

### Adding a new API endpoint
1. Create/update model in `TrashMob.Models/` (inherit `KeyedModel` for entities with GUID Id)
2. Add interface in `TrashMob.Shared/Managers/Interfaces/` (extend `IKeyedManager<T>`)
3. Implement manager in `TrashMob.Shared/Managers/` (extend `KeyedManager<T>`, use primary constructor)
4. Register in `TrashMob.Shared/ServiceBuilder.cs` (repositories are auto-resolved)
5. Add controller in `TrashMob/Controllers/` (extend `KeyedController<T>` or `SecureController`, use primary constructor)
6. Add React Query service in `TrashMob/client-app/src/services/` (factory returning `{ key, service }`)

### Controller Template (Current Pattern)
```csharp
public class ThingsController(
    IThingManager thingManager,
    IOtherManager otherManager)
    : KeyedController<Thing>(thingManager)
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await Manager.GetAsync(id, cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public async Task<IActionResult> Add(Thing instance, CancellationToken cancellationToken)
    {
        var result = await Manager.AddAsync(instance, UserId, cancellationToken);
        TrackEvent("AddThing");
        return Ok(result);
    }
}
```

### Authorization Checks
```csharp
// Check entity ownership
if (!await IsAuthorizedAsync(entity, AuthorizationPolicyConstants.UserOwnsEntity))
    return Forbid();

// Check event lead role
if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
    return Forbid();
```

### ServiceResult Pattern
For operations needing specific error messages:
```csharp
public async Task<ServiceResult<T>> DoSomethingAsync(...) {
    if (invalid) return ServiceResult<T>.Failure("Specific error");
    return ServiceResult<T>.Success(result);
}
```

### Adding a new frontend page
1. Create page component in `TrashMob/client-app/src/pages/`
2. Add lazy import and route in `App.tsx`
3. Use React Query hooks with service factories from `services/`
4. Use Zod + React Hook Form for forms, DataTable for lists
5. See `TrashMob/CLAUDE.md` for detailed frontend patterns

### Adding a new mobile screen
1. Create ViewModel in `TrashMobMobile/ViewModels/` (extend `BaseViewModel`, use `partial class` + primary constructor)
2. Create Page in `TrashMobMobile/Pages/` (XAML + code-behind with `[QueryProperty]`)
3. Register both in `TrashMobMobile/MauiProgram.cs` DI container
4. Add Shell route in `AppShell.xaml.cs`
5. See `TrashMobMobile/CLAUDE.md` for detailed mobile patterns

## Troubleshooting

**Data doesn't load locally:** Check Azure SQL firewall rules - your IP may have changed. Look for actual IP in VS Code debug output.

**Email not sending:** `dotnet user-secrets set "sendGridApiKey" "x"` to disable, or use real key from dev KeyVault.

### Windows Environment

- **ImageMagick `convert.exe`** conflicts with the Windows disk conversion utility (`C:\Windows\System32\convert.exe`). Use PowerShell .NET methods (`System.Drawing`) or specify the full ImageMagick path explicitly.
- **Android SDK paths:** When setting up MAUI, ensure `cmdline-tools` is in the correct subdirectory (`latest/`) and JDK 17+ is installed. Windows SDK paths differ from macOS/Linux documentation.

### Azure Infrastructure

- **Subscription tier matters:** Azure Sponsorship subscriptions have restricted API support (e.g., budget/billing APIs may not be available). Always confirm the target subscription tier before using budget or consumption APIs in Bicep templates.
- **Bicep environment naming:** Use the exact environment suffix from our naming convention (`dev`, `pr`) — not alternatives like `test` or `staging`.

## Infrastructure & Operations

See `Deploy/OPERATIONS_RUNBOOK.md` for infrastructure procedures including rollback, backups, DNS, Key Vault, SSL certificates, and Strapi CMS infrastructure.

See `Deploy/CONTAINER_DEPLOYMENT_GUIDE.md` for container deployment procedures.
