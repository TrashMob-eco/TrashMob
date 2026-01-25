# TrashMob.eco

**Meet up. Clean up. Feel good.**

[![Build Status](https://github.com/TrashMob-eco/TrashMob/actions/workflows/container_ca-tm-dev-westus2.yml/badge.svg)](https://github.com/TrashMob-eco/TrashMob/actions)

## What is TrashMob?

TrashMob.eco is a platform dedicated to organizing groups of people to clean up the world we live in. Users create cleanup events, publicize them, and recruit people to join up, as well as ask for assistance from communities and partners to help haul away the garbage once it is gathered.

The idea is to turn what can be an intimidating process for event organizers into a few clicks and simple forms. And once the process is simple, events will spring up all over the world, and the cleanup of the world can begin.

### Live Sites

| Environment | URL |
|-------------|-----|
| **Production** | [www.trashmob.eco](https://www.trashmob.eco) |
| **Development** | [dev.trashmob.eco](https://ca-tm-dev-westus2.ashypebble-059d2628.westus2.azurecontainerapps.io) |
| **Dev Swagger** | [dev.trashmob.eco/swagger](https://ca-tm-dev-westus2.ashypebble-059d2628.westus2.azurecontainerapps.io/swagger/index.html) |

### Mobile Apps

| Platform | Link |
|----------|------|
| **Android** | [Google Play Store](https://play.google.com/store/apps/details?id=eco.trashmob.trashmobmobileapp) |
| **iOS** | [Apple App Store](https://apps.apple.com/us/app/trashmob/id1599996743) |

---

## Quick Start

For detailed development setup instructions, see [CLAUDE.md](./CLAUDE.md).

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/) (for frontend development)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- [Visual Studio Code](https://code.visualstudio.com/download) or Visual Studio 2022+

### Build & Run

```bash
# Backend API
cd TrashMob
dotnet run --environment Development

# Frontend (separate terminal)
cd TrashMob/client-app
npm install
npm start
```

Local API: https://localhost:44332
Swagger: https://localhost:44332/swagger/index.html

---

## Documentation

### For Developers

| Document | Description |
|----------|-------------|
| [CLAUDE.md](./CLAUDE.md) | Development guide, architecture, coding standards |
| [CONTRIBUTING.md](./CONTRIBUTING.md) | How to contribute |
| [CODE_OF_CONDUCT.md](./CODE_OF_CONDUCT.md) | Community guidelines |

### Product & Planning

| Document | Description |
|----------|-------------|
| [2026 Planning Hub](./Planning/README.md) | Roadmap, projects, and milestones |
| [TrashMob.prd](./TrashMob/TrashMob.prd) | Web API product requirements |
| [TrashMobMobile.prd](./TrashMobMobile/TrashMobMobile.prd) | Mobile app product requirements |

### Technical Reference

| Document | Description |
|----------|-------------|
| [Domain Model](./TrashMob.Models/TrashMob.Models.prd) | Core data concepts, business rules, entity relationships |
| [Test Scenarios](./TestScenarios.md) | Manual testing checklist |
| [Deploy Guide](./Deploy/README.md) | Infrastructure deployment |
| [Container Deployment](./Deploy/CONTAINER_DEPLOYMENT_GUIDE.md) | Docker/ACA deployment |

### Project-Specific

| Project | README |
|---------|--------|
| TrashMob (Web API) | [TrashMob/README.md](./TrashMob/README.md) |
| TrashMob.Shared | [TrashMob.Shared/README.md](./TrashMob.Shared/README.md) |
| TrashMob.Models | [TrashMob.Models/README.md](./TrashMob.Models/README.md) |
| TrashMobMobile | [TrashMobMobile/readme.md](./TrashMobMobile/readme.md) |
| Mobile Analysis | [TrashMobMobile/docs/PROJECT_ANALYSIS.md](./TrashMobMobile/docs/PROJECT_ANALYSIS.md) |

---

## Project Structure

```
TrashMob/
├── TrashMob/              # ASP.NET Core Web API + React SPA
│   └── client-app/        # React frontend (Vite + TypeScript)
├── TrashMob.Shared/       # Business logic, managers, EF Core
├── TrashMob.Models/       # Domain entities
├── TrashMob.Shared.Tests/ # Unit tests
├── TrashMobMobile/        # .NET MAUI mobile app
├── TrashMobDailyJobs/     # Background jobs (daily)
├── TrashMobHourlyJobs/    # Background jobs (hourly)
└── Deploy/                # Bicep templates, deployment scripts
```

---

## Contributing

We're actively looking for contributors! All skill levels welcome:

- **Frontend:** React, TypeScript, Tailwind CSS
- **Backend:** ASP.NET Core, Entity Framework Core
- **Mobile:** .NET MAUI
- **DevOps:** GitHub Actions, Azure, Bicep

### Getting Started

1. Check [Good First Issues](https://github.com/TrashMob-eco/TrashMob/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22)
2. Read [CONTRIBUTING.md](./CONTRIBUTING.md)
3. Set up your dev environment using [CLAUDE.md](./CLAUDE.md)
4. Contact [info@trashmob.eco](mailto:info@trashmob.eco) to be added as a contributor

### Feature Ideas

Before starting work on a new feature, please check:
- [Projects](https://github.com/orgs/TrashMob-eco/projects)
- [Open Issues](https://github.com/TrashMob-eco/TrashMob/issues)
- [2026 Product Plan](./Planning/README.md)

---

## Development Environment Setup

### Using Shared Dev Environment

If you're not making database changes, you can use the shared dev environment:

1. Email [info@trashmob.eco](mailto:info@trashmob.eco) to request contributor access
2. TrashMob will add you to the Sandbox subscription and Dev KeyVault
3. Add your IP to the [Dev Azure SQL firewall](https://portal.azure.com/#@jobeedevids.onmicrosoft.com/resource/subscriptions/39a254b7-c01a-45ab-bebd-4038ea4adea9/resourceGroups/rg-trashmob-dev-westus2/providers/Microsoft.Sql/servers/sql-tm-dev-westus2/overview)
4. Run setup script:

```powershell
az login
.\setupdev.ps1 -environment dev -region westus2 -subscription 39a254b7-c01a-45ab-bebd-4038ea4adea9
```

### Creating Your Own Environment

For major database changes, create your own environment:

1. Follow [Deploy/README.md](./Deploy/README.md)
2. Run setup with your parameters:

```powershell
.\setupdev.ps1 -environment <yourenv> -region <yourregion> -subscription <yourguid>
```

### Troubleshooting

**Data doesn't load:** Your IP may have changed. Check the VS Code debug output for your actual IP and update the Azure SQL firewall rule.

**Email not sending:** Set a dummy key to disable email:
```bash
dotnet user-secrets set "sendGridApiKey" "x"
```

---

## Mobile App Development

See [TrashMobMobile/readme.md](./TrashMobMobile/readme.md) for detailed mobile setup.

### Quick Setup (Windows)

1. Install Visual Studio with .NET MAUI workload
2. Install [Android Studio](https://developer.android.com/studio) and create an emulator
3. Open `TrashMobMobileApp.sln`
4. Get Google Maps API key from Dev KeyVault or create your own
5. Update `Platforms/Android/AndroidManifest.xml` with your key

⚠️ **Never commit API keys to the repository!**

### Test Builds

- **Android:** Request internal tester access at [info@trashmob.eco](mailto:info@trashmob.eco)
- **iOS:** Request TestFlight access at [info@trashmob.eco](mailto:info@trashmob.eco)

---

## Deployment

### Automatic Deployments

| Branch | Environment | Trigger |
|--------|-------------|---------|
| `main` | Development | Push |
| `release` | Production | Manual workflow |

### Manual Deployment

See [Deploy/CONTAINER_DEPLOYMENT_GUIDE.md](./Deploy/CONTAINER_DEPLOYMENT_GUIDE.md) for container deployment instructions.

---

## Background & History

Years ago, Scott Hanselman (and others at Microsoft) built NerdDinner.com as a demo of ASP.NET MVC. Those nerd dinners were fantastic and had a huge role in many careers, including leading the founder to join Microsoft.

This site is based on both that code and the idea that getting people together to do small good things results in larger good things in the long term.

The inspiration came from [Edgar McGregor](https://twitter.com/edgarrmcgregor), who spent over 1100 days cleaning up a park in his community, two pails of litter at a time. His actions inspired others to do the same, and TrashMob.eco was born—a platform to help people organize "mobs" to tackle cleanup together.

---

## Status

TrashMob.eco is a **501(c)(3) non-profit** in the United States, launched in production on May 15, 2022. We're actively developing new features to help communities clean up the world!

---

## Contact

- **Email:** [info@trashmob.eco](mailto:info@trashmob.eco)
- **Website:** [trashmob.eco](https://www.trashmob.eco)
- **GitHub Issues:** [Report a bug or request a feature](https://github.com/TrashMob-eco/TrashMob/issues)
