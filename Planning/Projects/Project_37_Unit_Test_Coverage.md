# Project 37 — Unit Test Coverage Improvement

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Large |
| **Dependencies** | None |

---

## Business Rationale

The current codebase has 77 manager classes but only 15 test files with 103 tests, most focused on email notifications. This leaves significant business logic untested, increasing regression risk during refactoring and feature development. Improving unit test coverage enables confident code changes and faster development velocity.

---

## Objectives

### Primary Goals
- **Increase manager test coverage** to ≥ 80% of critical business logic
- **Establish testing patterns** and conventions for the codebase
- **Add CI gate** requiring tests for new manager code
- **Document testing best practices** for contributors

### Secondary Goals
- Code coverage reporting in CI
- Test data builders and fixtures
- Integration test patterns for database operations
- Controller unit tests with mocked dependencies

---

## Scope

### Phase 1: Testing Infrastructure
- ✅ Establish test project structure conventions
- ✅ Create base test classes and fixtures
- ✅ Set up mocking patterns (Moq)
- ✅ Add test data builders
- ✅ Configure code coverage reporting

### Phase 2: Core Manager Tests
Priority managers to test (high business value):

| Manager | Complexity | Tests Needed |
|---------|------------|--------------|
| EventManager | High | Create, Update, Cancel, Status changes |
| EventAttendeeManager | High | Register, Unregister, Waiver validation |
| UserManager | High | Profile updates, Privacy settings |
| TeamManager | Medium | Create, Join, Leave, Permissions |
| TeamMemberManager | Medium | Add/Remove members, Roles |
| LeaderboardManager | Medium | Ranking calculations, Caching |
| AchievementManager | Medium | Award logic, Notification state |
| LitterReportManager | Medium | Create, Status transitions |
| PartnerManager | Medium | Request workflow, Approval |

### Phase 3: Supporting Manager Tests

| Manager | Tests Needed |
|---------|--------------|
| EventSummaryManager | Aggregation calculations |
| EventAttendeeMetricsManager | Metric submissions, Approval workflow |
| WaiverManager | Signing, Expiration, Compliance |
| CommunityManager | Discovery, Membership |
| PhotoModerationManager | Flag/Approve/Reject workflow |
| EmailInviteManager | Batch creation, Status tracking |

### Phase 4: Controller Tests
- Add unit tests for controllers with mocked managers
- Test authorization attribute coverage
- Test error handling and response codes

---

## Out-of-Scope

- ❌ E2E/UI tests (covered by Project 25)
- ❌ Performance testing
- ❌ Mobile app tests
- ❌ Frontend (React) unit tests

---

## Success Metrics

### Quantitative
- **Code coverage:** ≥ 80% for TrashMob.Shared/Managers
- **Test count:** ≥ 300 unit tests (up from 103)
- **CI execution time:** < 60 seconds for unit tests
- **New code coverage gate:** 70% minimum for new PRs

### Qualitative
- Developers confident making changes to tested code
- Reduced production bugs in covered areas
- Faster code review (tests document expected behavior)

---

## Dependencies

### Blockers
- None

### Enables
- Safer refactoring
- Project 6 (Backend Standards) modernization
- Faster feature development

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Tests slow down CI** | Medium | Low | Parallelize; mock DB calls |
| **Test maintenance burden** | Medium | Medium | Good abstractions; shared fixtures |
| **Mocking complexity** | Low | Low | Document patterns; code review |
| **False confidence from high coverage** | Low | Medium | Focus on meaningful assertions |

---

## Implementation Plan

### Test Project Structure

```
TrashMob.Shared.Tests/
├── Builders/
│   ├── EventBuilder.cs
│   ├── UserBuilder.cs
│   └── TeamBuilder.cs
├── Fixtures/
│   ├── DbContextFixture.cs
│   └── MockRepositoryFixture.cs
├── Managers/
│   ├── Events/
│   │   ├── EventManagerTests.cs
│   │   ├── EventAttendeeManagerTests.cs
│   │   └── EventSummaryManagerTests.cs
│   ├── Teams/
│   │   ├── TeamManagerTests.cs
│   │   └── TeamMemberManagerTests.cs
│   ├── Gamification/
│   │   ├── LeaderboardManagerTests.cs
│   │   └── AchievementManagerTests.cs
│   └── ...
├── Controllers/
│   ├── EventsControllerTests.cs
│   └── ...
└── TestBase.cs
```

### Test Data Builder Pattern

```csharp
// Builders/EventBuilder.cs
public class EventBuilder
{
    private Event _event = new Event
    {
        Id = Guid.NewGuid(),
        Name = "Test Event",
        EventStatusId = 1,
        EventTypeId = 1,
        CreatedByUserId = Guid.NewGuid(),
        CreatedDate = DateTimeOffset.UtcNow
    };

    public EventBuilder WithName(string name)
    {
        _event.Name = name;
        return this;
    }

    public EventBuilder WithStatus(int statusId)
    {
        _event.EventStatusId = statusId;
        return this;
    }

    public EventBuilder CreatedBy(Guid userId)
    {
        _event.CreatedByUserId = userId;
        return this;
    }

    public Event Build() => _event;
}
```

### Example Manager Test

```csharp
// Managers/Events/EventManagerTests.cs
public class EventManagerTests
{
    private readonly Mock<IKeyedRepository<Event>> _eventRepository;
    private readonly Mock<IEventAttendeeManager> _attendeeManager;
    private readonly EventManager _sut;

    public EventManagerTests()
    {
        _eventRepository = new Mock<IKeyedRepository<Event>>();
        _attendeeManager = new Mock<IEventAttendeeManager>();
        _sut = new EventManager(
            _eventRepository.Object,
            _attendeeManager.Object,
            // ... other dependencies
        );
    }

    [Fact]
    public async Task CreateEvent_WithValidData_ReturnsEvent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newEvent = new EventBuilder()
            .WithName("Beach Cleanup")
            .CreatedBy(userId)
            .Build();

        _eventRepository
            .Setup(r => r.AddAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newEvent);

        // Act
        var result = await _sut.AddAsync(newEvent, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Beach Cleanup", result.Name);
        _eventRepository.Verify(r => r.AddAsync(
            It.Is<Event>(e => e.Name == "Beach Cleanup"),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CancelEvent_NotCreator_ThrowsUnauthorized()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var existingEvent = new EventBuilder()
            .CreatedBy(creatorId)
            .Build();
        existingEvent.Id = eventId;

        _eventRepository
            .Setup(r => r.GetAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEvent);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.DeleteAsync(eventId, otherUserId));
    }
}
```

### Code Coverage Configuration

```xml
<!-- TrashMob.Shared.Tests.csproj -->
<PropertyGroup>
  <CollectCoverage>true</CollectCoverage>
  <CoverletOutputFormat>cobertura</CoverletOutputFormat>
  <CoverletOutput>./TestResults/coverage.cobertura.xml</CoverletOutput>
  <Include>[TrashMob.Shared]*</Include>
  <Exclude>[*]*.Migrations.*</Exclude>
</PropertyGroup>
```

### GitHub Actions Coverage Gate

```yaml
# .github/workflows/unit-tests.yml
- name: Run tests with coverage
  run: dotnet test TrashMob.Shared.Tests --collect:"XPlat Code Coverage"

- name: Check coverage threshold
  uses: irongut/CodeCoverageSummary@v1.3.0
  with:
    filename: '**/coverage.cobertura.xml'
    badge: true
    fail_below_min: true
    format: markdown
    hide_branch_rate: false
    hide_complexity: false
    indicators: true
    output: both
    thresholds: '60 80'
```

---

## Implementation Phases

### Phase 1: Infrastructure (1-2 weeks)
- Create test builders for core entities
- Set up mock repository fixtures
- Add coverlet for coverage reporting
- Document testing patterns in CLAUDE.md

### Phase 2: Core Managers (2-3 weeks)
- EventManager tests (8-10 tests)
- EventAttendeeManager tests (6-8 tests)
- UserManager tests (5-7 tests)
- TeamManager tests (6-8 tests)

### Phase 3: Supporting Managers (2-3 weeks)
- LeaderboardManager tests
- AchievementManager tests
- LitterReportManager tests
- WaiverManager tests
- PartnerManager tests

### Phase 4: Controllers & Gate (1-2 weeks)
- Controller unit tests
- Coverage gate in CI
- Documentation updates

---

## Testing Conventions

### Naming Convention
```
MethodName_StateUnderTest_ExpectedBehavior
```

Examples:
- `CreateEvent_WithValidData_ReturnsEvent`
- `CancelEvent_NotCreator_ThrowsUnauthorized`
- `GetLeaderboard_NoCache_ComputesFromDatabase`

### Test Structure (AAA Pattern)
```csharp
[Fact]
public async Task MethodName_State_Expected()
{
    // Arrange
    // ... setup test data and mocks

    // Act
    var result = await _sut.MethodUnderTest();

    // Assert
    Assert.Equal(expected, result);
}
```

### What to Test
- **DO test:** Business logic, validation rules, state transitions, edge cases
- **DON'T test:** Framework code, simple getters/setters, third-party libraries

---

## Related Documents

- **[Project 25 - Automated Testing](./Project_25_Automated_Testing.md)** - E2E/UI testing (complementary)
- **[Project 6 - Backend Standards](./Project_06_Backend_Standards.md)** - Code quality standards
- **[CLAUDE.md](../../CLAUDE.md)** - Development guidelines

---

**Last Updated:** February 3, 2026
**Owner:** Engineering Team
**Status:** Not Started
**Next Review:** When starting implementation

---

## Changelog

- **2026-02-03:** Initial specification created
