# Project 6 � Backend Code Standards & Structure

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

Unify coding patterns across the backend codebase, upgrade to .NET 10, secure all endpoints, update dependencies, and improve API documentation via Swagger/OpenAPI. This improves maintainability, reduces bugs, and makes onboarding new developers easier.

---

## Objectives

### Primary Goals
- **Upgrade to .NET 10** across all projects
- **Security audit** on all API endpoints
- **Update NuGet packages** to latest stable versions
- **Add XML documentation** for Swagger/OpenAPI
- **Standardize patterns** (error handling, logging, validation)

### Secondary Goals
- Implement code analysis rules (Roslyn analyzers)
- Add EditorConfig for consistent formatting
- Document architectural patterns in README

---

## Scope

### Phase 1 - .NET 10 Upgrade ✓
- ✓ Upgrade all .csproj files to `<TargetFramework>net10.0</TargetFramework>`
- ✓ Update MAUI workload (if needed)
- ✓ Test all projects compile and run
- ✓ Update GitHub Actions workflows

### Phase 2 - Dependency Updates (Partial)
- ✓ Renovate bot keeps NuGet and npm packages updated (8 Renovate PRs merged Feb 2026)
- ? Address breaking changes (handled per Renovate PR)
- ? Test thoroughly
- ? Update package documentation

### Phase 2.5 - Code Modernization ✅ (22 PRs — Feb 2026)

Comprehensive code quality sweep across the entire backend:

| Category | PRs | Changes |
|----------|-----|---------|
| **Primary Constructors** | #2705, #2706, #2708, #2709 | Converted all controllers, managers, repositories, engines, and services to C# 12 primary constructors |
| **Collection Expressions** | #2711, #2712 | Replaced `new List<T>()` with `[]`, `Tuple` with value tuples, `string.Format` with interpolation |
| **Structured Logging** | #2713 | Converted logger format placeholders to structured logging patterns |
| **Null Pattern Matching** | #2715, #2717 | Replaced `== null` / `!= null` with `is null` / `is not null` across controllers, security handlers, and jobs |
| **Magic String Elimination** | #2701 | Extracted magic strings into constants, standardized Delete endpoints |
| **N+1 Query Fixes** | #2697 | Eliminated N+1 queries and unbounded table loads |
| **Dead Code Removal** | #2696 | Removed dead reCAPTCHA code, added user ownership checks |
| **Auth Boilerplate** | #2698 | Extracted `IsAuthorizedAsync` helper to reduce authorization boilerplate |
| **ConfigureAwait Cleanup** | #2699 | Removed `ConfigureAwait(false)` and standardized string validation |
| **Async Naming** | #2703 | Fixed async method naming, removed redundant `[ApiController]` |
| **Exception Logging** | #2702 | Added exception logging, removed redundant `.AsEnumerable()` calls |
| **HTTP Status Codes** | #2707 | Fixed HTTP status codes, redundant fields, and code style issues |

### Phase 3 - Security Audit (Partial)
- ✓ User ownership checks added (PR #2696)
- ✓ Auth handler tests added — 45 tests for all 8 authorization handlers (PR #2687)
- ? Review all remaining API endpoints for authorization
- ? Add `[Authorize]` attributes where missing
- ? Validate input on all endpoints
- ? Add rate limiting where appropriate
- ? **Least-privilege database access** — Split database credentials so the application runtime user has only the permissions it needs (read/write on application tables) rather than admin/owner access. Create a separate migration user for schema changes. This reduces blast radius if the app is compromised.

### Phase 4 - Documentation
- ✓ Add XML comments to all public APIs
- ✓ Configure Swagger to include XML docs
- ✓ Document common patterns in wiki
- ✓ Create coding standards guide

### Phase 5 - Unit Test Coverage ✓
- ✓ Increase test coverage for TrashMob.Shared managers
- ✓ Add tests for repository layer
- ✓ Add tests for controller validation logic
- ✓ Achieve minimum 70% code coverage for business logic
- ✓ Integrate coverage reporting in CI/CD

**Note:** Completed via [Project 37 - Unit Test Coverage](./Project_37_Unit_Test_Coverage.md) with 242 tests total.

### Phase 6 - Frontend Standards ✓
- ✓ Create FRONTEND_STANDARDS.md with React best practices
- ✓ Add ErrorBoundary component for graceful error handling
- ✓ Add NotFound page component for 404/route not found handling
- ✓ Document component patterns and composition
- ✓ Document data fetching with React Query
- ✓ Document TypeScript guidelines and strict mode
- ✓ Document accessibility requirements

**Note:** All documentation items covered in [FRONTEND_STANDARDS.md](../../TrashMob/client-app/FRONTEND_STANDARDS.md)

---

## Out-of-Scope

- ? Major architectural refactoring
- ? Moving to microservices
- ? Database schema changes
- ? Performance optimization (separate project)

---

## Success Metrics

### Quantitative
- **Code coverage:** Achieve ≥ 70% coverage for TrashMob.Shared managers
- **Compilation warnings:** Reduce to zero
- **Security vulnerabilities:** Zero high/critical
- **API documentation:** 100% of endpoints documented
- **Defects post-release:** Reduce by 30%

### Qualitative
- Positive developer feedback on code clarity
- Faster onboarding for new contributors
- Improved API consumer satisfaction

---

## Dependencies

### Blockers
None - can run in parallel with other work

### Enables
- Better foundation for all future development
- Easier API v2 implementation (Project 24)

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Breaking changes in .NET 10** | Medium | Medium | Thorough testing, gradual rollout |
| **NuGet package incompatibilities** | Low | Medium | Update one at a time, test each |
| **Volunteer availability** | High | Low | Break into small, independent tasks |

---

## Implementation Plan

### .NET 10 Upgrade Steps

```xml
<!-- Update all .csproj files -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
</Project>
```

### Security Patterns

```csharp
// Standard authorization pattern
[Authorize]
[HttpPost("api/events")]
public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventRequest request)
{
    // Validate input
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    // Check user permissions
    var userId = User.GetUserId();
    if (!await _authService.CanCreateEvent(userId))
        return Forbid();
    
    // Business logic
    var result = await _eventService.CreateEventAsync(request, userId);
    return CreatedAtAction(nameof(GetEvent), new { id = result.Id }, result);
}
```

### XML Documentation

```csharp
/// <summary>
/// Creates a new event
/// </summary>
/// <param name="request">Event creation details</param>
/// <returns>The created event</returns>
/// <response code="201">Event created successfully</response>
/// <response code="400">Invalid request data</response>
/// <response code="401">User not authenticated</response>
/// <response code="403">User not authorized to create events</response>
[HttpPost]
[ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventRequest request)
{
    // Implementation
}
```

### Coding Standards Document

Create `CODING_STANDARDS.md`:

```markdown
# TrashMob Backend Coding Standards

## Naming Conventions
- Classes: PascalCase
- Methods: PascalCase
- Private fields: _camelCase
- Constants: UPPER_CASE

## Error Handling
- Always use try-catch for external calls
- Log all errors with context
- Return Problem Details for API errors

## Async Patterns
- Suffix async methods with "Async"
- Always use ConfigureAwait(false) in library code
- Never use .Result or .Wait()

## Validation
- Use Data Annotations on DTOs
- Validate in controllers before service layer
- Return 400 Bad Request for validation errors
```

---

## Implementation Phases

### Phase 1: Upgrade Foundation
- Upgrade .NET version
- Fix compilation errors
- Update CI/CD

### Phase 2: Package Updates
- Update packages one at a time
- Test each update
- Document any breaking changes

### Phase 3: Security Hardening
- Audit endpoint authorization
- Add missing [Authorize] attributes
- Review input validation
- Add rate limiting

### Phase 4: Documentation
- Add XML comments
- Generate API documentation
- Write coding standards
- Create contribution guide

### Phase 5: Unit Test Coverage
- Add unit tests for managers in TrashMob.Shared
- Test repository methods with in-memory database
- Add controller validation tests
- Configure code coverage reporting (Coverlet)
- Set up coverage thresholds in CI pipeline

**Note:** Each phase can be picked up by different volunteers independently.

---

## Open Questions

1. ~~**Should we enforce code analysis rules as errors or warnings?**~~
   **Decision:** Enforce as errors - `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` added to all projects
   **Status:** Resolved

2. ~~**What Roslyn analyzers should we enable?**~~
   **Decision:** .NET Analyzers (built-in) + StyleCop.Analyzers added via Directory.Build.props
   **Status:** Resolved

3. ~~**Should we use EditorConfig for formatting?**~~
   **Decision:** Yes - comprehensive .editorconfig added with C#, TypeScript, formatting, and naming conventions
   **Status:** Resolved

4. ~~**What are the rate limiting thresholds per endpoint category?**~~
   **Decision:** Public endpoints: 100 req/min; Authenticated: 300 req/min; Admin: 600 req/min; adjust based on production telemetry
   **Status:** ✅ Resolved

5. ~~**What is our API deprecation policy?**~~
   **Decision:** No external consumers - deprecate endpoints only after web and mobile apps are converted off them
   **Status:** ✅ Resolved

6. ~~**What logging level and content standards should we follow?**~~
   **Decision:** Info for API request/response summaries; Debug for detailed processing flow; Error with full exception context and correlation ID; never log PII or credentials
   **Status:** ✅ Resolved

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#2228](https://github.com/trashmob/TrashMob/issues/2228)** - Project 6: Improve Back End Code Standards and Structure (tracking issue)

---

## Related Documents

- **[Project 24 - API v2](./Project_24_API_v2_Modernization.md)** - Benefits from clean code foundation
- **[Project 5 - Deployment](./Project_05_Deployment_Pipelines.md)** - .NET 10 in CI/CD

---

**Last Updated:** February 15, 2026
**Owner:** Engineering Lead
**Status:** In Progress (Phase 1, 2.5, 4, 5 & 6 complete; Phase 2 ongoing via Renovate; Phase 3 partial with auth handler tests and ownership checks)
**Next Review:** When Phase 3 security audit continues
