# Project 20 — Gamification

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | High |
| **Size** | Medium |
| **Dependencies** | Project 7 (Event Weights), Project 9 (Teams) |

---

## Business Rationale

Drive engagement with leaderboards across roles and time ranges while preventing fraud. Gamification increases volunteer retention and creates friendly competition that motivates participation.

---

## Objectives

### Primary Goals
- **Leaderboards** by user, team, and community
- **Time windows** (Today, Week, Month, Year, All Time)
- **Anti-gaming guardrails** to prevent fraud
- **Multiple metrics** (events, bags, weight, hours)

### Secondary Goals
- Achievements/badges
- Streaks and milestones
- Social sharing of achievements
- Community challenges

---

## Scope

### Phase 1 - User Leaderboards
- ✅ Top volunteers by events attended
- ✅ Top volunteers by bags collected
- ✅ Top volunteers by weight collected
- ✅ Time range filters
- ✅ Location filters (city, region, global)

### Phase 2 - Team Leaderboards
- ✅ Top teams by events
- ✅ Top teams by total bags/weight
- ✅ Top teams by member count
- ✅ Team vs team comparisons

### Phase 3 - Community Leaderboards
- ✅ Top communities by activity
- ✅ Community rankings
- ✅ Inter-community challenges

### Phase 4 - Achievements
- ❓ Badge system
- ❓ Milestones (first event, 10 events, etc.)
- ❓ Streaks (consecutive weeks)
- ❓ Special achievements

---

## Out-of-Scope

- ❌ Monetary rewards/prizes
- ❌ NFTs or blockchain badges
- ❌ Real-time score updates
- ❌ Betting/wagering
- ❌ Purchasable advantages

---

## Success Metrics

### Quantitative
- **Leaderboard views:** Track engagement
- **Repeat participation:** ≥ 20% increase in returning volunteers
- **Events per user:** ≥ 15% increase in average
- **Fraud incidents:** < 0.1%

### Qualitative
- Volunteers motivated by rankings
- Healthy competition without toxicity
- Teams use leaderboards for recruiting

---

## Dependencies

### Blockers
- **Project 7 (Event Weights):** Weight metrics for leaderboards
- **Project 9 (Teams):** Team leaderboards

### Enables
- Volunteer retention
- Team engagement
- Community pride

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Gaming/fraud** | High | High | Anomaly detection; lead verification; rate limits |
| **Discourages casual volunteers** | Medium | Medium | Multiple metrics; "personal best" focus; participation badges |
| **Toxic competition** | Low | Medium | Clear guidelines; report function; moderation |
| **Data accuracy disputes** | Medium | Low | Clear rules; dispute process; lead verification |

---

## Implementation Plan

### Data Model Changes

```sql
-- Leaderboard cache (pre-computed for performance)
CREATE TABLE LeaderboardCache (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    LeaderboardType NVARCHAR(50) NOT NULL, -- User, Team, Community
    MetricType NVARCHAR(50) NOT NULL, -- Events, Bags, Weight, Hours
    TimeRange NVARCHAR(20) NOT NULL, -- Today, Week, Month, Year, AllTime
    LocationScope NVARCHAR(50) NULL, -- Global, Region:XX, City:XX
    EntityId UNIQUEIDENTIFIER NOT NULL, -- UserId, TeamId, or CommunityId
    EntityName NVARCHAR(200) NOT NULL,
    Score DECIMAL(18,2) NOT NULL,
    Rank INT NOT NULL,
    ComputedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
);

-- User achievements
CREATE TABLE UserAchievements (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    AchievementTypeId INT NOT NULL,
    EarnedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    UNIQUE (UserId, AchievementTypeId)
);

-- Achievement types
CREATE TABLE AchievementTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    IconUrl NVARCHAR(500) NULL,
    Category NVARCHAR(50) NOT NULL, -- Events, Impact, Streaks, Special
    Criteria NVARCHAR(MAX) NOT NULL, -- JSON rules
    Points INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Fraud detection log
CREATE TABLE LeaderboardAuditLog (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    UserId UNIQUEIDENTIFIER NOT NULL,
    EventId UNIQUEIDENTIFIER NULL,
    ActionType NVARCHAR(50) NOT NULL,
    Details NVARCHAR(MAX) NULL,
    FlaggedForReview BIT NOT NULL DEFAULT 0,
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
);

CREATE INDEX IX_LeaderboardCache_Type_Metric_Time ON LeaderboardCache(LeaderboardType, MetricType, TimeRange);
CREATE INDEX IX_LeaderboardCache_EntityId ON LeaderboardCache(EntityId);
CREATE INDEX IX_UserAchievements_UserId ON UserAchievements(UserId);
```

### API Changes

```csharp
// Leaderboards
[HttpGet("api/leaderboards/users")]
public async Task<ActionResult<LeaderboardDto>> GetUserLeaderboard(
    [FromQuery] string metric = "events",
    [FromQuery] string timeRange = "month",
    [FromQuery] string location = "global",
    [FromQuery] int limit = 50)
{
    // Return cached leaderboard
}

[HttpGet("api/leaderboards/teams")]
public async Task<ActionResult<LeaderboardDto>> GetTeamLeaderboard(
    [FromQuery] string metric = "events",
    [FromQuery] string timeRange = "month",
    [FromQuery] int limit = 50)
{
    // Return team leaderboard
}

[HttpGet("api/leaderboards/communities")]
public async Task<ActionResult<LeaderboardDto>> GetCommunityLeaderboard(
    [FromQuery] string metric = "events",
    [FromQuery] string timeRange = "month",
    [FromQuery] int limit = 50)
{
    // Return community leaderboard
}

// User's rank
[Authorize]
[HttpGet("api/users/me/rank")]
public async Task<ActionResult<UserRankDto>> GetMyRank(
    [FromQuery] string metric = "events",
    [FromQuery] string timeRange = "month")
{
    // Return user's rank and surrounding entries
}

// Achievements
[HttpGet("api/achievements")]
public async Task<ActionResult<IEnumerable<AchievementTypeDto>>> GetAchievementTypes()
{
    // Return all available achievements
}

[Authorize]
[HttpGet("api/users/me/achievements")]
public async Task<ActionResult<IEnumerable<UserAchievementDto>>> GetMyAchievements()
{
    // Return user's earned achievements
}

[HttpGet("api/users/{userId}/achievements")]
public async Task<ActionResult<IEnumerable<UserAchievementDto>>> GetUserAchievements(Guid userId)
{
    // Return user's public achievements
}
```

### Web UX Changes

**Leaderboard Page:**
- Tab navigation (Users, Teams, Communities)
- Metric selector (Events, Bags, Weight, Hours)
- Time range selector
- Location filter
- Top 50 list with avatars
- "Find My Rank" button
- Surrounding context (users near your rank)

**User Profile:**
- Achievements showcase
- Personal stats
- Rank badges
- Progress to next milestone

**Dashboard:**
- Personal leaderboard position
- Recent achievements
- Streak indicator

### Mobile App Changes

- Leaderboards view
- My achievements
- Rank notifications
- Share achievements

---

## Anti-Gaming Measures

1. **Anomaly Detection:**
   - Flag unusual spikes in activity
   - Review events with unusually high metrics
   - Monitor new user rapid progression

2. **Verification Requirements:**
   - Event leads verify attendee participation
   - Event summaries reviewed for outliers
   - Photo evidence for high-impact claims

3. **Rate Limits:**
   - Maximum events per user per day
   - Minimum event duration
   - Geographic constraints

4. **Moderation:**
   - Report suspicious activity
   - Admin review queue
   - Temporary suspensions for violations

---

## Implementation Phases

### Phase 1: Infrastructure
- Database schema
- Leaderboard computation job
- Caching strategy
- Basic API

### Phase 2: User Leaderboards
- User rankings
- Time ranges
- Location filters
- UI components

### Phase 3: Team/Community
- Team leaderboards
- Community leaderboards
- Comparisons

### Phase 4: Achievements
- Badge system
- Achievement rules engine
- Notifications
- Social sharing

**Note:** Start simple; iterate based on engagement data.

---

## Open Questions

1. **Leaderboard refresh frequency?**
   **Recommendation:** Hourly for active time ranges; daily for historical
   **Owner:** Engineering
   **Due:** Before Phase 1

2. **Minimum events to appear on leaderboard?**
   **Recommendation:** 3 events to prevent gaming with single high-impact event
   **Owner:** Product Lead
   **Due:** Before Phase 2

3. **Achievement notification preferences?**
   **Recommendation:** Opt-in for notifications; always visible in profile
   **Owner:** Product Lead
   **Due:** Before Phase 4

4. **Opt-out of leaderboards?**
   **Recommendation:** Yes, privacy option to hide from public leaderboards
   **Owner:** Product Lead
   **Due:** Before Phase 2

---

## Related Documents

- **[Project 7 - Event Weights](./Project_07_Event_Weights.md)** - Weight metrics
- **[Project 9 - Teams](./Project_09_Teams.md)** - Team rankings
- **[Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)** - Individual stats

---

**Last Updated:** January 24, 2026
**Owner:** Product Lead + Engineering
**Status:** Not Started
**Next Review:** When dependencies complete
