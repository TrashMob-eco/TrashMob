# TrashMob.eco — AI Assistant Context

## Project Overview

TrashMob.eco is a community-driven platform that connects volunteers to organize and participate in litter cleanup events. The platform enables cities, communities, and individuals to coordinate cleanup efforts, track environmental impact, and build a network of environmentally-conscious volunteers.

**Mission:** Mobilize communities to clean up litter and create lasting environmental impact through organized events and measurable results.

## Technology Stack

### Backend
- **.NET 10** (ASP.NET Core Web API)
- **Entity Framework Core** for data access
- **Azure SQL Database** for persistence
- **Azure App Service** for hosting (transitioning to Docker/Azure Container Apps)
- **Azure B2C** (migrating to **Entra External ID**) for authentication
- **SendGrid** for email notifications
- **Google Maps API** for location services

### Frontend
- **Blazor WebAssembly** for web application
- **.NET MAUI** for cross-platform mobile apps (iOS/Android)

### Infrastructure & DevOps
- **GitHub Actions** for CI/CD
- **Docker** containerization (in progress)
- **Sentry.io** for error tracking and APM
- **Azure Monitor** for observability

## Project Structure

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
- Log errors with **structured logging** (Sentry.io integration)
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

Refer to `TrashMob_2026_Product_Engineering_Plan.md` for detailed roadmap. Priority areas:

1. **Project 1:** Auth migration (Azure B2C → Entra External ID)
2. **Project 4:** Mobile stabilization and error handling
3. **Project 7:** Event weight tracking (Phase 1 & 2)
4. **Project 9:** Teams feature (MVP)
5. **Project 10:** Community Pages (MVP)

## Development Workflow

### Branching Strategy
- `main` — production-ready code
- `dev` — integration branch
- `dev/{developer}/{feature}` — feature branches

### Commit Messages
- Use clear, descriptive messages
- Reference issue numbers where applicable

### Pull Requests
- Require review before merge
- Must pass all CI checks
- Include tests for new features

## Common Patterns & Examples

### Controller Example

````````
{
  "Event": {
    "Id": "123",
    "Title": "River Cleanup",
    "Description": "Join us for a cleanup of the local river.",
    "Location": {
      "Latitude": 34.0522,
      "Longitude": -118.2437
    },
    "Date": "2023-10-01T10:00:00Z",
    "Duration": 120,
    "VolunteersNeeded": 10,
    "Status": "Upcoming",
    "CreatedDate": "2023-09-01T12:00:00Z",
    "CreatedByUserId": "456",
    "LastUpdatedDate": "2023-09-15T12:00:00Z",
    "LastUpdatedByUserId": "456"
  },
  "Volunteer": {
    "Id": "456",
    "FirstName": "John",
    "LastName": "Doe",
    "Email": "john.doe@example.com",
    "Phone": "555-1234",
    "IsAdmin": false,
    "CommunityId": "789",
    "ConsentToContact": true,
    "CreatedDate": "2023-09-01T12:00:00Z",
    "LastLoginDate": "2023-09-20T12:00:00Z"
  }
}
````````

### Service Example


````````

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

- **Sentry.io** for error tracking
- **Structured logging** with context
- **Business event tracking** (signups, event creation, attendance)
- **Dashboards** for key metrics
- **Alerting** for critical issues

## Cost Optimization

Monitor and optimize:
- Azure App Service costs
- Database DTU/vCore usage
- Google Maps API calls
- SendGrid email volume

## Getting Help

- **Product Plan:** `TrashMob_2026_Product_Engineering_Plan.md`
- **GitHub Issues:** Track bugs and features
- **Project Wiki:** (if available)
- **Code Comments:** Check inline documentation

## AI Assistant Guidelines

When working with this codebase:

1. **Respect existing patterns** — maintain consistency with current code style
2. **Consider volunteer context** — code should be maintainable by contributors with varying experience
3. **Think security-first** — especially for auth, minors protection, and data privacy
4. **Plan for scale** — features should work for 1 event or 10,000 events
5. **Mobile-first considerations** — ensure features work well on mobile devices
6. **Accessibility by default** — build inclusive features from the start
7. **Document as you go** — clear comments and XML docs are essential
8. **Test thoroughly** — volunteer-run project needs reliable code

---

**Last Updated:** January 23, 2026  
**For Questions:** Refer to product engineering plan or project maintainers

