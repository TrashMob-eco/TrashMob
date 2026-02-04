# Project 38 — Mobile App Feature Parity

| Attribute | Value |
|-----------|-------|
| **Status** | Planning |
| **Priority** | High |
| **Risk** | High |
| **Size** | Very Large |
| **Dependencies** | Project 9 (Teams), Project 18 (Event Photos), Project 20 (Gamification), Project 22 (Attendee Metrics) |

---

## Business Rationale

The mobile app currently supports core event management and litter reporting but lacks many volunteer-facing features available on the website. Mobile users (our primary target audience for on-the-go volunteers) cannot access teams, leaderboards, event photos, or personal impact metrics. This creates a fragmented experience where volunteers must use the web for full functionality.

**Reference UX:** Strava app - exemplary mobile navigation, activity feed, clubs, leaderboards, achievements, and social features.

---

## Objectives

### Primary Goals
- **Feature parity** with volunteer-facing web features
- **Strava-inspired UX redesign** for intuitive navigation
- **Teams integration** - browse, join, and participate in teams
- **Leaderboards & achievements** - view rankings and earned badges
- **Event photos** - capture before/during/after photos during events
- **Personal impact tracking** - view individual contribution metrics
- **Newsletter preferences** - manage subscription settings

### Secondary Goals
- Improved offline support
- Push notification integration
- Social sharing of achievements
- Quick event check-in

### Explicitly Out of Scope
- Site admin features
- Community admin features
- Full team management (web-only)
- Partner management
- Content moderation
- User management
- Analytics dashboards

---

## Current Mobile App Capabilities

### Existing Features (Keep & Enhance)
| Feature | Status | Notes |
|---------|--------|-------|
| View events on map | ✅ Working | Enhance with team events filter |
| Create events | ✅ Working | Add team association |
| Edit events | ✅ Working | Keep as-is |
| Cancel events | ✅ Working | Keep as-is |
| Event summaries | ✅ Working | Add weight tracking, attendee metrics |
| Register for events | ✅ Working | Add "attending as team" option |
| Litter reports | ✅ Working | Keep as-is |
| User dashboard | ✅ Working | Major enhancement needed |
| Location preferences | ✅ Working | Keep as-is |
| Contact support | ✅ Working | Keep as-is |
| User statistics | ✅ Working | Enhance with detailed metrics |

### Architecture Foundation
- .NET MAUI 10.0 (Android, iOS, MacCatalyst)
- MVVM pattern with CommunityToolkit.Mvvm
- Shell-based navigation with TabBar
- REST services with Polly retry policies
- Azure AD B2C authentication via MSAL
- Sentry.io error tracking

---

## Scope

### Phase 1 - Navigation & UX Redesign (Strava-Inspired)

Redesign the app navigation to follow Strava's proven patterns.

**Current Structure:**
```
MainTabsPage (4 tabs)
├── Home (MainPage) - Events map/list
├── Dashboard (MyDashboardPage) - User events/reports
├── Location (SetUserLocationPreferencePage) - Preferences
└── More (MorePage) - Settings/logout
```

**Proposed Structure (Strava-inspired):**
```
AppShell (5 tabs)
├── Home (Feed) - Activity feed + nearby events
├── Explore (Map) - Events, teams, litter on map
├── Record (Quick Action) - FAB for create event/report
├── Impact (Dashboard) - Stats, achievements, leaderboards
└── Profile (You) - Settings, teams, preferences
```

**Detailed Tab Breakdown:**

**Tab 1: Home (Feed)**
- Activity feed showing:
  - Upcoming events you're registered for
  - Recent team activity
  - Nearby events recommendation
  - Achievement notifications
  - Newsletter highlights (if subscribed)
- Pull-to-refresh
- Quick "Join Event" cards

**Tab 2: Explore**
- Interactive map with layers:
  - Events (upcoming, filtered by date)
  - Teams (public teams near you)
  - Litter reports (open reports)
- Toggle between layers
- Search bar for events/teams/locations
- List/map view toggle

**Tab 3: Record (Floating Action Button style)**
- Central "+" button opens action sheet:
  - Create Event
  - Report Litter
  - Quick Check-in (if at event)
  - Take Event Photo (if attending)

**Tab 4: Impact (Dashboard)**
- Personal statistics cards:
  - Events attended
  - Bags collected
  - Weight collected
  - Hours volunteered
- Achievement badges (earned/in-progress)
- Leaderboard preview (your rank)
- "View Full Leaderboards" link
- Streak indicator

**Tab 5: Profile (You)**
- User info and avatar
- My Teams section
- My Events (upcoming/past)
- My Litter Reports
- Settings:
  - Location preferences
  - Newsletter subscriptions
  - Notification preferences
  - Leaderboard visibility
- Sign out

### Phase 2 - Teams Integration

**New Pages:**
1. **TeamsListPage** - Browse public teams
   - Map view with team locations
   - List view with team cards
   - Search/filter by location, name
   - "Nearby Teams" section

2. **TeamDetailPage** - View team information
   - Team name, description, logo
   - Member count and list (public members)
   - Team statistics (events, bags, weight)
   - Upcoming team events
   - Photo gallery
   - "Request to Join" button (public teams)

3. **MyTeamsPage** - Teams you belong to
   - List of your teams
   - Quick access to team details
   - Pending join requests (if team lead)
   - "Create Team" link (optional - could be web-only)

4. **TeamJoinRequestsPage** (Limited Team Admin)
   - List of pending requests (for team leads)
   - Approve/reject actions
   - Requester profile preview

**New ViewModels:**
- `TeamsListViewModel`
- `TeamDetailViewModel`
- `MyTeamsViewModel`
- `TeamJoinRequestsViewModel`

**New Services:**
- `ITeamRestService` / `TeamRestService`
- `ITeamManager` / `TeamManager`

**API Endpoints to Consume:**
```
GET  /api/teams - List public teams
GET  /api/teams/{id} - Team details
GET  /api/teams/{id}/members - Team members
GET  /api/teams/{id}/events - Team events
POST /api/teams/{teamId}/join - Request to join
GET  /api/users/{userId}/teams - User's teams
GET  /api/teams/{teamId}/joinrequests - Pending requests (leads only)
PUT  /api/teams/{teamId}/joinrequests/{requestId} - Approve/reject
```

### Phase 3 - Event Photos

**Enhancements to ViewEventPage:**
- Photo gallery tab showing event photos
- "Add Photo" button (for attendees)
- Photo type selector (Before/During/After)
- Caption input

**New Pages:**
1. **EventPhotoGalleryPage** - Browse event photos
   - Grid view of photos
   - Filter by type (Before/During/After)
   - Tap to view full-size with caption

2. **TakeEventPhotoPage** - Camera capture
   - Camera integration
   - Photo type selection
   - Caption input
   - Upload progress indicator

3. **ViewEventPhotoPage** - Full-screen photo view
   - Photo with caption
   - Photo metadata (type, uploader, date)
   - "Report Photo" option

**New ViewModels:**
- `EventPhotoGalleryViewModel`
- `TakeEventPhotoViewModel`
- `ViewEventPhotoViewModel`

**New Services:**
- `IEventPhotoRestService` / `EventPhotoRestService`
- `IEventPhotoManager` / `EventPhotoManager`

**API Endpoints to Consume:**
```
GET  /api/events/{eventId}/photos - Event photos
POST /api/events/{eventId}/photos - Upload photo (multipart)
GET  /api/events/{eventId}/photos/{photoId} - Single photo
POST /api/events/{eventId}/photos/{photoId}/flag - Report photo
```

**Platform Integration:**
- Camera access (MAUI MediaPicker)
- Photo library access
- Image compression before upload
- Offline queue for uploads

### Phase 4 - Leaderboards & Achievements

**New Pages:**
1. **LeaderboardsPage** - Rankings view
   - Tab bar: Users | Teams | Communities
   - Metric picker: Events | Bags | Weight
   - Time range picker: Week | Month | Year | All
   - Top 50 list with avatars/logos
   - "Find My Rank" highlight
   - Pull-to-refresh

2. **AchievementsPage** - Badge collection
   - Grid of earned achievements
   - Greyed-out locked achievements
   - Progress bars for in-progress
   - Tap for achievement details

3. **AchievementDetailPage** - Single achievement
   - Badge icon (large)
   - Name and description
   - Earned date (or progress)
   - Share button

**New ViewModels:**
- `LeaderboardsViewModel`
- `AchievementsViewModel`
- `AchievementDetailViewModel`

**New Services:**
- `ILeaderboardRestService` / `LeaderboardRestService`
- `IAchievementRestService` / `AchievementRestService`

**API Endpoints to Consume:**
```
GET /api/leaderboards/users - User rankings
GET /api/leaderboards/teams - Team rankings
GET /api/leaderboards/communities - Community rankings
GET /api/users/me/rank - My rank
GET /api/achievements - All achievement types
GET /api/users/me/achievements - My achievements
```

### Phase 5 - Attendee Metrics & Impact Tracking

**Enhancements to Event Summary Flow:**
- Weight entry (imperial/metric)
- Drop location marking
- Individual contribution entry

**New Pages:**
1. **MyImpactPage** - Detailed personal stats
   - Total events attended
   - Total bags collected
   - Total weight collected
   - Total hours volunteered
   - Litter reports submitted
   - Charts/graphs showing trends
   - Event-by-event breakdown

2. **EventMetricsPage** - Event contribution
   - Personal bags/weight for specific event
   - Comparison to event average
   - Time contributed
   - Photos uploaded

**Enhancements to ViewEventPage:**
- "My Contribution" card (for past events)
- "Log My Impact" button (if attended, post-event)

**New ViewModels:**
- `MyImpactViewModel`
- `EventMetricsViewModel`
- `LogImpactViewModel`

**API Endpoints to Consume:**
```
GET  /api/users/{userId}/impact - User impact metrics
GET  /api/events/{eventId}/attendees/{userId}/metrics - Event contribution
POST /api/events/{eventId}/attendees/{userId}/metrics - Log contribution
PUT  /api/events/{eventId}/summary - Update with weight
```

### Phase 6 - Newsletter & Preferences

**New Pages:**
1. **NotificationPreferencesPage** - Manage notifications
   - Push notification toggles
   - Email newsletter categories
     - Monthly Digest (on/off)
     - Event Updates (on/off)
     - Community News (on/off)
     - Team Updates (on/off)
   - Achievement notifications (on/off)
   - Leaderboard visibility (on/off)

**Enhancements to Profile:**
- Quick access to notification settings
- Newsletter subscription status

**New ViewModels:**
- `NotificationPreferencesViewModel`

**API Endpoints to Consume:**
```
GET  /api/users/me/preferences - User preferences
PUT  /api/users/me/preferences - Update preferences
GET  /api/newsletters/categories - Newsletter categories
PUT  /api/users/me/newsletters - Update subscriptions
```

### Phase 7 - Team Events (Limited Team Admin)

**For Team Leads Only:**

**Enhancements to MyTeamsPage:**
- "Create Team Event" button (per team)
- Pending join requests badge

**New Pages:**
1. **CreateTeamEventPage** - Team-linked event creation
   - Pre-filled team association
   - Standard event creation flow
   - Team notification option

**Enhancements to Create Event Flow:**
- Team selector (if member of teams)
- "Create as team event" option

**API Endpoints to Consume:**
```
POST /api/events - With teamId in payload
POST /api/teams/{teamId}/events/{eventId} - Link existing event
```

---

## Technical Implementation

### New Models (Mobile-specific)

```csharp
// Models/TeamSummary.cs
public class TeamSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string LogoUrl { get; set; }
    public bool IsPublic { get; set; }
    public int MemberCount { get; set; }
    public int EventCount { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
}

// Models/LeaderboardEntry.cs
public class LeaderboardEntry
{
    public int Rank { get; set; }
    public Guid EntityId { get; set; }
    public string EntityName { get; set; }
    public string AvatarUrl { get; set; }
    public decimal Score { get; set; }
    public bool IsCurrentUser { get; set; }
}

// Models/Achievement.cs
public class Achievement
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public string Category { get; set; }
    public int Points { get; set; }
    public bool IsEarned { get; set; }
    public DateTimeOffset? EarnedDate { get; set; }
    public double? Progress { get; set; } // 0-1 for in-progress
}

// Models/UserImpact.cs
public class UserImpact
{
    public int EventsAttended { get; set; }
    public int BagsCollected { get; set; }
    public double WeightCollected { get; set; }
    public string WeightUnit { get; set; }
    public double HoursVolunteered { get; set; }
    public int LitterReportsSubmitted { get; set; }
    public int EventsLed { get; set; }
    public int PhotosUploaded { get; set; }
}
```

### Navigation Updates

**AppShell.xaml Changes:**
```xml
<Shell>
    <TabBar>
        <ShellContent Title="Home" Icon="home.png" Route="HomePage" />
        <ShellContent Title="Explore" Icon="explore.png" Route="ExplorePage" />
        <ShellContent Title="Record" Icon="add_circle.png" Route="RecordPage" />
        <ShellContent Title="Impact" Icon="trending_up.png" Route="ImpactPage" />
        <ShellContent Title="Profile" Icon="person.png" Route="ProfilePage" />
    </TabBar>
</Shell>
```

**New Route Registrations:**
```csharp
// Teams
Routing.RegisterRoute("TeamsListPage", typeof(TeamsListPage));
Routing.RegisterRoute("TeamDetailPage", typeof(TeamDetailPage));
Routing.RegisterRoute("MyTeamsPage", typeof(MyTeamsPage));
Routing.RegisterRoute("TeamJoinRequestsPage", typeof(TeamJoinRequestsPage));

// Photos
Routing.RegisterRoute("EventPhotoGalleryPage", typeof(EventPhotoGalleryPage));
Routing.RegisterRoute("TakeEventPhotoPage", typeof(TakeEventPhotoPage));
Routing.RegisterRoute("ViewEventPhotoPage", typeof(ViewEventPhotoPage));

// Leaderboards
Routing.RegisterRoute("LeaderboardsPage", typeof(LeaderboardsPage));
Routing.RegisterRoute("AchievementsPage", typeof(AchievementsPage));
Routing.RegisterRoute("AchievementDetailPage", typeof(AchievementDetailPage));

// Impact
Routing.RegisterRoute("MyImpactPage", typeof(MyImpactPage));
Routing.RegisterRoute("EventMetricsPage", typeof(EventMetricsPage));

// Settings
Routing.RegisterRoute("NotificationPreferencesPage", typeof(NotificationPreferencesPage));
```

### Service Layer Additions

```csharp
// ServiceCollectionExtensions.cs additions
services.AddSingleton<ITeamRestService, TeamRestService>();
services.AddSingleton<ITeamManager, TeamManager>();
services.AddSingleton<IEventPhotoRestService, EventPhotoRestService>();
services.AddSingleton<IEventPhotoManager, EventPhotoManager>();
services.AddSingleton<ILeaderboardRestService, LeaderboardRestService>();
services.AddSingleton<IAchievementRestService, AchievementRestService>();
services.AddSingleton<IUserPreferencesRestService, UserPreferencesRestService>();
```

---

## UX/UI Design Guidelines (Strava-Inspired)

### Visual Design Principles
1. **Clean, minimal interface** - Focus on content, not chrome
2. **Bold typography** - Large headers, readable body text
3. **Card-based layouts** - Distinct content sections
4. **Consistent iconography** - Material Design icons
5. **Vibrant accent color** - TrashMob green (#4CAF50)
6. **White/light gray backgrounds** - Clean, professional look

### Navigation Patterns
1. **Bottom tab bar** - 5 primary destinations (always visible)
2. **Pull-to-refresh** - On all list/feed pages
3. **Floating action button** - Quick create actions
4. **Swipe gestures** - Horizontal swiping for tabs/filters
5. **Modal sheets** - For quick actions and forms
6. **Back navigation** - Consistent back arrow behavior

### Key Screens Design Reference

**Home Feed:**
```
┌─────────────────────────────────────┐
│  TrashMob                      🔔   │
├─────────────────────────────────────┤
│  ┌─────────────────────────────────┐│
│  │ 🗓 UPCOMING EVENT              ││
│  │ Beach Cleanup - Tomorrow 9AM   ││
│  │ Sunset Beach • 12 attendees    ││
│  │ [View Event]                   ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ 🏆 ACHIEVEMENT UNLOCKED        ││
│  │ Week Warrior - 2 weeks streak! ││
│  │ [View Achievements]            ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ 📍 NEARBY EVENT                ││
│  │ Park Cleanup - Saturday 10AM   ││
│  │ 2.3 miles away • 8 attendees   ││
│  │ [Join]  [View]                 ││
│  └─────────────────────────────────┘│
├─────────────────────────────────────┤
│  🏠    🗺    ➕    📊    👤        │
└─────────────────────────────────────┘
```

**Impact Dashboard:**
```
┌─────────────────────────────────────┐
│  ← Impact                           │
├─────────────────────────────────────┤
│  YOUR STATS                         │
│  ┌────────┐ ┌────────┐ ┌────────┐  │
│  │   42   │ │  128   │ │ 256lbs │  │
│  │ Events │ │  Bags  │ │ Weight │  │
│  └────────┘ └────────┘ └────────┘  │
│                                     │
│  ACHIEVEMENTS          [View All >] │
│  🏅 🏅 🏅 🏅 ⭕ ⭕ ⭕ ⭕          │
│                                     │
│  YOUR RANK             [View All >] │
│  ┌─────────────────────────────────┐│
│  │ 🥇 #42 in Events (This Month)  ││
│  │ Top 5% of volunteers            ││
│  └─────────────────────────────────┘│
│                                     │
│  CURRENT STREAK                     │
│  🔥 3 weeks in a row!              │
│                                     │
├─────────────────────────────────────┤
│  🏠    🗺    ➕    📊    👤        │
└─────────────────────────────────────┘
```

**Leaderboards:**
```
┌─────────────────────────────────────┐
│  ← Leaderboards                     │
├─────────────────────────────────────┤
│  [Users] [Teams] [Communities]      │
│  ───────────────────────────────────│
│  Events ▼   |   This Month ▼        │
├─────────────────────────────────────┤
│  1  👤 Jane Smith         48 events │
│  2  👤 Mike Johnson       45 events │
│  3  👤 Sarah Wilson       42 events │
│  ...                                │
│  41 👤 Alex Chen          15 events │
│ ►42 👤 You               14 events │◄
│  43 👤 Chris Lee          14 events │
│  ...                                │
├─────────────────────────────────────┤
│  🏠    🗺    ➕    📊    👤        │
└─────────────────────────────────────┘
```

---

## Success Metrics

### Quantitative
- **Mobile session duration:** ≥ 30% increase
- **Features used per session:** ≥ 50% increase
- **Team joins via mobile:** ≥ 40% of total
- **Photos uploaded via mobile:** ≥ 70% of total
- **Leaderboard views:** Track engagement
- **App store rating:** Maintain ≥ 4.5 stars

### Qualitative
- Users report improved experience
- Feature requests shift from "add X" to "enhance X"
- Reduced "use website for this" feedback

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Scope creep** | High | High | Strict phase boundaries; defer enhancements |
| **Navigation confusion** | Medium | High | User testing; incremental rollout |
| **Performance issues** | Medium | Medium | Lazy loading; image optimization; caching |
| **Platform differences** | Medium | Low | MAUI abstractions; platform-specific handlers |
| **API compatibility** | Low | High | Version API endpoints; feature flags |

---

## Implementation Phases

| Phase | Focus | Priority |
|-------|-------|----------|
| 1 | Navigation & UX Redesign | P0 |
| 2 | Teams Integration | P0 |
| 3 | Event Photos | P1 |
| 4 | Leaderboards & Achievements | P1 |
| 5 | Attendee Metrics | P1 |
| 6 | Newsletter & Preferences | P2 |
| 7 | Team Events (Limited Admin) | P2 |

---

## Dependencies

### Blockers
- **Project 9 (Teams):** Backend APIs must exist ✅ Complete
- **Project 18 (Event Photos):** Backend APIs must exist ✅ Phase 1 Complete
- **Project 20 (Gamification):** Leaderboard APIs must exist ✅ Backend Complete
- **Project 22 (Attendee Metrics):** Individual metrics APIs ⚠️ Partial

### Enables
- Improved volunteer retention
- Mobile-first volunteer experience
- Competitive parity with other volunteer apps

---

## Testing Strategy

### Unit Testing
- ViewModel logic tests
- Service layer mocks
- Navigation flow tests

### Integration Testing
- API communication tests
- Authentication flow tests
- Offline behavior tests

### Device Testing
- iOS (iPhone 12+, iPad)
- Android (various screen sizes, API 21+)
- Real device camera testing for photos

### User Testing
- Beta testers via TestFlight/Firebase Distribution
- A/B testing of navigation layouts
- Usability studies for key flows

---

## Open Questions

1. **Should team creation be mobile or web-only?**
   - **Recommendation:** Web-only initially; add mobile later if requested
   - **Status:** Pending decision

2. **Offline photo upload queue?**
   - **Recommendation:** Yes, queue photos when offline, upload when connected
   - **Status:** Pending decision

3. **Push notifications scope?**
   - **Options:** Event reminders, team activity, achievements, leaderboard changes
   - **Status:** Pending decision

4. **Platform-specific features?**
   - iOS: Widgets for upcoming events?
   - Android: Quick actions from home screen?
   - **Status:** Defer to Phase 8

5. **Minimum supported OS versions?**
   - Current: iOS 15.0, Android API 21
   - **Recommendation:** Keep current; covers 95%+ of users
   - **Status:** Confirmed

---

## Related Documents

- **[Project 4 - Mobile Robustness](./Project_04_Mobile_Robustness.md)** - Error handling foundation
- **[Project 9 - Teams](./Project_09_Teams.md)** - Teams backend
- **[Project 18 - Event Photos](./Project_18_Before_After_Photos.md)** - Photo APIs
- **[Project 20 - Gamification](./Project_20_Gamification.md)** - Leaderboards/achievements
- **[Project 22 - Attendee Metrics](./Project_22_Attendee_Metrics.md)** - Individual impact tracking

---

**Last Updated:** February 3, 2026
**Owner:** Mobile Team + Product Lead
**Status:** Planning
**Next Review:** After UX mockups are complete
