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
- ✅ Badge system
- ✅ Milestones (first event, 10 events, etc.)
- ✅ Streaks (consecutive weeks)
- ✅ Special achievements

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

**Modification: User (add gamification preferences)**
```csharp
// Add to existing TrashMob.Models/User.cs
#region Gamification Preferences

/// <summary>
/// Gets or sets whether the user appears on public leaderboards.
/// </summary>
public bool ShowOnLeaderboards { get; set; } = true;

/// <summary>
/// Gets or sets whether the user receives achievement notifications.
/// </summary>
public bool AchievementNotificationsEnabled { get; set; } = true;

#endregion
```

**New Entity: LeaderboardCache**
```csharp
// New file: TrashMob.Models/LeaderboardCache.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Pre-computed leaderboard entry for performance.
    /// </summary>
    public class LeaderboardCache
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the leaderboard type (User, Team, Community).
        /// </summary>
        public string LeaderboardType { get; set; }

        /// <summary>
        /// Gets or sets the metric type (Events, Bags, Weight, Hours).
        /// </summary>
        public string MetricType { get; set; }

        /// <summary>
        /// Gets or sets the time range (Today, Week, Month, Year, AllTime).
        /// </summary>
        public string TimeRange { get; set; }

        /// <summary>
        /// Gets or sets the location scope (Global, Region:XX, City:XX).
        /// </summary>
        public string LocationScope { get; set; }

        /// <summary>
        /// Gets or sets the entity ID (user, team, or community).
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Gets or sets the entity display name.
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the score for this entry.
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// Gets or sets the rank position.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets when this entry was computed.
        /// </summary>
        public DateTimeOffset ComputedDate { get; set; }
    }
}
```

**New Entity: UserAchievement**
```csharp
// New file: TrashMob.Models/UserAchievement.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents an achievement earned by a user.
    /// </summary>
    public class UserAchievement : KeyedModel
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the achievement type identifier.
        /// </summary>
        public int AchievementTypeId { get; set; }

        /// <summary>
        /// Gets or sets when the achievement was earned.
        /// </summary>
        public DateTimeOffset EarnedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual AchievementType AchievementType { get; set; }
    }
}
```

**New Entity: AchievementType (lookup table)**
```csharp
// New file: TrashMob.Models/AchievementType.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Defines an achievement type that users can earn.
    /// </summary>
    public class AchievementType : LookupModel
    {
        /// <summary>
        /// Gets or sets the achievement description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the URL of the achievement icon/badge.
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Gets or sets the category (Events, Impact, Streaks, Special).
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the JSON criteria rules for earning this achievement.
        /// </summary>
        public string Criteria { get; set; }

        /// <summary>
        /// Gets or sets the points value of this achievement.
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets whether this achievement is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<UserAchievement> UserAchievements { get; set; }
    }
}
```

**New Entity: LeaderboardAuditLog**
```csharp
// New file: TrashMob.Models/LeaderboardAuditLog.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Audit log for leaderboard activity and fraud detection.
    /// </summary>
    public class LeaderboardAuditLog
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the related event identifier (if applicable).
        /// </summary>
        public Guid? EventId { get; set; }

        /// <summary>
        /// Gets or sets the action type being logged.
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// Gets or sets additional details as JSON.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets whether this entry is flagged for review.
        /// </summary>
        public bool FlaggedForReview { get; set; }

        /// <summary>
        /// Gets or sets when this log entry was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Event Event { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<User>(entity =>
{
    // Add to existing User configuration
    entity.Property(e => e.ShowOnLeaderboards).HasDefaultValue(true);
    entity.Property(e => e.AchievementNotificationsEnabled).HasDefaultValue(true);
});

modelBuilder.Entity<LeaderboardCache>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).UseIdentityColumn();
    entity.Property(e => e.LeaderboardType).HasMaxLength(50).IsRequired();
    entity.Property(e => e.MetricType).HasMaxLength(50).IsRequired();
    entity.Property(e => e.TimeRange).HasMaxLength(20).IsRequired();
    entity.Property(e => e.LocationScope).HasMaxLength(50);
    entity.Property(e => e.EntityName).HasMaxLength(200).IsRequired();
    entity.Property(e => e.Score).HasPrecision(18, 2);

    entity.HasIndex(e => new { e.LeaderboardType, e.MetricType, e.TimeRange });
    entity.HasIndex(e => e.EntityId);
});

modelBuilder.Entity<UserAchievement>(entity =>
{
    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.AchievementType)
        .WithMany(a => a.UserAchievements)
        .HasForeignKey(e => e.AchievementTypeId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.UserId);
    entity.HasIndex(e => new { e.UserId, e.AchievementTypeId }).IsUnique();
});

modelBuilder.Entity<AchievementType>(entity =>
{
    entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
    entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
    entity.Property(e => e.IconUrl).HasMaxLength(500);
    entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
});

modelBuilder.Entity<LeaderboardAuditLog>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).UseIdentityColumn();
    entity.Property(e => e.ActionType).HasMaxLength(50).IsRequired();

    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.Event)
        .WithMany()
        .HasForeignKey(e => e.EventId)
        .OnDelete(DeleteBehavior.NoAction);
});
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
- Leaderboard computation job (daily Azure Container App job)
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

## Initial Achievement Types

**Milestone Achievements (Events Category):**
| Name | Description | Criteria |
|------|-------------|----------|
| First Steps | Attended your first cleanup event | events_attended >= 1 |
| Regular Volunteer | Attended 10 cleanup events | events_attended >= 10 |
| Dedicated Volunteer | Attended 25 cleanup events | events_attended >= 25 |
| Super Volunteer | Attended 50 cleanup events | events_attended >= 50 |
| Cleanup Champion | Attended 100 cleanup events | events_attended >= 100 |

**Milestone Achievements (Impact Category):**
| Name | Description | Criteria |
|------|-------------|----------|
| Trash Collector | Collected 10 bags of trash | bags_collected >= 10 |
| Trash Warrior | Collected 50 bags of trash | bags_collected >= 50 |
| Trash Hero | Collected 100 bags of trash | bags_collected >= 100 |

**Streak Achievements:**
| Name | Description | Criteria |
|------|-------------|----------|
| Week Warrior | Attended events 2 weeks in a row | consecutive_weeks >= 2 |
| Month of Service | Attended events 4 weeks in a row | consecutive_weeks >= 4 |
| Quarterly Champion | Attended events 12 weeks in a row | consecutive_weeks >= 12 |

**Special Achievements:**
| Name | Description | Criteria |
|------|-------------|----------|
| Event Leader | Led your first cleanup event | events_led >= 1 |
| Team Player | Joined a cleanup team | team_member = true |
| Community Builder | Participated in a community event | community_event_attended = true |

---

## Resolved Questions

1. **Leaderboard refresh frequency?**
   **Decision:** Daily refresh for all time ranges (simpler compute, sufficient for engagement)

2. **Minimum events to appear on leaderboard?**
   **Decision:** 3 events minimum (prevents gaming with single high-impact event)

3. **Achievement notification preferences?**
   **Decision:** Default on with opt-out (users receive notifications by default; can disable in preferences)

4. **Opt-out of leaderboards?**
   **Decision:** Yes, privacy option to hide from public leaderboards (users can still earn achievements privately)

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#2240](https://github.com/trashmob/TrashMob/issues/2240)** - Project 20: Gamification (tracking issue)

---

## Related Documents

- **[Project 7 - Event Weights](./Project_07_Event_Weights.md)** - Weight metrics
- **[Project 9 - Teams](./Project_09_Teams.md)** - Team rankings
- **[Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)** - Individual stats

---

**Last Updated:** January 31, 2026
**Owner:** Product Lead + Engineering
**Status:** Not Started
**Next Review:** When dependencies complete
