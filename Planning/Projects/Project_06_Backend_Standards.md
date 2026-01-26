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

### Phase 2 - Dependency Updates
- ? Update all NuGet packages
- ? Address breaking changes
- ? Test thoroughly
- ? Update package documentation

### Phase 3 - Security Audit
- ? Review all API endpoints for authorization
- ? Add `[Authorize]` attributes where missing
- ? Validate input on all endpoints
- ? Add rate limiting where appropriate

### Phase 4 - Documentation
- ✓ Add XML comments to all public APIs
- ✓ Configure Swagger to include XML docs
- ? Document common patterns in wiki
- ? Create coding standards guide

---

## Out-of-Scope

- ? Major architectural refactoring
- ? Moving to microservices
- ? Database schema changes
- ? Performance optimization (separate project)

---

## Success Metrics

### Quantitative
- **Code coverage:** Maintain or increase current levels
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

**Note:** Each phase can be picked up by different volunteers independently.

---

## Open Questions

1. **Should we enforce code analysis rules as errors or warnings?**  
   **Recommendation:** Warnings for now, errors after cleanup  
   **Owner:** Engineering Lead  
   **Due:** Before Phase 1

2. **What Roslyn analyzers should we enable?**  
   **Recommendation:** Microsoft.CodeAnalysis.NetAnalyzers, StyleCop.Analyzers  
   **Owner:** Engineering Lead  
   **Due:** Before Phase 1

3. **Should we use EditorConfig for formatting?**  
   **Recommendation:** Yes, standardize across team  
   **Owner:** Engineering Lead  
   **Due:** Before Phase 1

---

## Related Documents

- **[Project 24 - API v2](./Project_24_API_v2_Modernization.md)** - Benefits from clean code foundation
- **[Project 5 - Deployment](./Project_05_Deployment_Pipelines.md)** - .NET 10 in CI/CD

---

**Last Updated:** January 25, 2026
**Owner:** Engineering Lead
**Status:** In Progress (Phase 1 complete, Phase 4 partially complete)
**Next Review:** When Phase 2 or 3 starts
