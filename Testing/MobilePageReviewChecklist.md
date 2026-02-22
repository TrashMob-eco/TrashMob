# TrashMob.eco — Mobile App Page Review Checklist

Track your screen-by-screen review of the TrashMob mobile app (iOS and Android). For each screen, check off:
- **Looks Good** — Layout, styling, branding, responsive behavior, no visual glitches
- **Works** — Interactions function correctly, data loads, forms submit, navigation works

Mark with `[x]` when verified. Add notes in the **Notes** column for anything that needs follow-up.

**Legend:**  `LG` = Looks Good | `W` = Works

> **Website checklist** is in a separate file: [PageReviewChecklist.md](PageReviewChecklist.md)

### Device Test Matrix

| Platform | OS Versions | Recommended Devices |
|----------|-------------|---------------------|
| iOS | 15, 16, 17 | iPhone SE (2020), iPhone 13, iPhone 15 Pro |
| Android | 8.0, 10, 12, 14 | Pixel 5, Samsung Galaxy S21, OnePlus 9 |

---

## Table of Contents

1. [Navigation & Auth](#navigation--auth)
2. [Home & Explore](#home--explore)
3. [Events](#events)
4. [Litter Reports](#litter-reports)
5. [Pickup Locations](#pickup-locations)
6. [Teams & Communities](#teams--communities)
7. [Impact & Gamification](#impact--gamification)
8. [Profile & Settings](#profile--settings)

---

## Navigation & Auth

| # | Screen | Route | LG | W | Notes |
|---|--------|-------|----|---|-------|
| M1 | Welcome / Sign In | `WelcomePage` | [ ] | [ ] | Stats display, sign-in flow |
| M2 | Main Tabs (shell) | `MainTabsPage` | [ ] | [ ] | 5-tab bottom nav |
| M3 | Logout | `LogoutPage` | [ ] | [ ] | |

---

## Home & Explore

| # | Screen | Route | LG | W | Notes |
|---|--------|-------|----|---|-------|
| M4 | Home Feed | `HomeFeedPage` | [ ] | [ ] | Upcoming events, nearby reports |
| M5 | Explore (map + list) | `ExplorePage` | [ ] | [ ] | Filters, map pins, toggle |
| M6 | Search Events | `SearchEventsPage` | [ ] | [ ] | Status filter, map/list |
| M7 | Search Litter Reports | `SearchLitterReportsPage` | [ ] | [ ] | Status filter |
| M8 | My Dashboard | `MyDashboardPage` | [ ] | [ ] | Stats, tabs for events/reports |
| M9 | Quick Action | `QuickActionPlaceholderPage` | [ ] | [ ] | Placeholder — check behavior |

---

## Events

| # | Screen | Route | LG | W | Notes |
|---|--------|-------|----|---|-------|
| M10 | Create Event (5-step wizard) | `CreateEventPage` | [ ] | [ ] | All 5 steps |
| M11 | — Step 1: Details | (within wizard) | [ ] | [ ] | Name, date, time, type |
| M12 | — Step 2: Location | (within wizard) | [ ] | [ ] | Map picker, address |
| M13 | — Step 3: Review | (within wizard) | [ ] | [ ] | Summary before save |
| M14 | — Step 4: Partners | (within wizard) | [ ] | [ ] | Browse available partners |
| M15 | — Step 5: Litter Reports | (within wizard) | [ ] | [ ] | Assign reports |
| M16 | View Event | `ViewEventPage` | [ ] | [ ] | 6 tabs |
| M17 | — Details tab | (within page) | [ ] | [ ] | |
| M18 | — Partners tab | (within page) | [ ] | [ ] | |
| M19 | — Attendees tab | (within page) | [ ] | [ ] | |
| M20 | — Litter Reports tab | (within page) | [ ] | [ ] | |
| M21 | — Photos tab | (within page) | [ ] | [ ] | |
| M22 | — Routes tab | (within page) | [ ] | [ ] | |
| M23 | Edit Event | `EditEventPage` | [ ] | [ ] | 3 tabs: Details, Location, Partners |
| M24 | Cancel Event | `CancelEventPage` | [ ] | [ ] | Reason required |
| M25 | Edit Event Summary | `EditEventSummaryPage` | [ ] | [ ] | Post-event metrics |
| M26 | View Event Summary | `ViewEventSummaryPage` | [ ] | [ ] | |
| M27 | Manage Event Partners | `ManageEventPartnersPage` | [ ] | [ ] | |
| M28 | Edit Partner Location Services | `EditEventPartnerLocationServicesPage` | [ ] | [ ] | |

---

## Litter Reports

| # | Screen | Route | LG | W | Notes |
|---|--------|-------|----|---|-------|
| M29 | Create Litter Report | `CreateLitterReportPage` | [ ] | [ ] | Camera, GPS, photos |
| M30 | View Litter Report | `ViewLitterReportPage` | [ ] | [ ] | Map, edit/clean buttons |
| M31 | Edit Litter Report | `EditLitterReportPage` | [ ] | [ ] | Name, description, status |

---

## Pickup Locations

| # | Screen | Route | LG | W | Notes |
|---|--------|-------|----|---|-------|
| M32 | Create Pickup Location | `CreatePickupLocationPage` | [ ] | [ ] | Auto-location from photo |
| M33 | Edit Pickup Location | `EditPickupLocationPage` | [ ] | [ ] | |
| M34 | View Pickup Location | `ViewPickupLocationPage` | [ ] | [ ] | |

---

## Teams & Communities

| # | Screen | Route | LG | W | Notes |
|---|--------|-------|----|---|-------|
| M35 | Browse Teams | `BrowseTeamsPage` | [ ] | [ ] | Nearby teams, join |
| M36 | View Team | `ViewTeamPage` | [ ] | [ ] | Members, join button |
| M37 | Browse Communities | `BrowseCommunitiesPage` | [ ] | [ ] | Nearby communities |
| M38 | View Community | `ViewCommunityPage` | [ ] | [ ] | Details, impact stats |

---

## Impact & Gamification

| # | Screen | Route | LG | W | Notes |
|---|--------|-------|----|---|-------|
| M39 | Impact | `ImpactPage` | [ ] | [ ] | Personal + community stats |
| M40 | Leaderboards | `LeaderboardsPage` | [ ] | [ ] | Type + time filters |
| M41 | Achievements | `AchievementsPage` | [ ] | [ ] | Badges, points, progress |

---

## Profile & Settings

| # | Screen | Route | LG | W | Notes |
|---|--------|-------|----|---|-------|
| M42 | Profile | `ProfilePage` | [ ] | [ ] | Info, events, reports, teams |
| M43 | Set Location Preference | `SetUserLocationPreferencePage` | [ ] | [ ] | Map picker, radius |
| M44 | Newsletter Preferences | `NewsletterPreferencesPage` | [ ] | [ ] | Toggle categories |
| M45 | Waiver | `WaiverPage` | [ ] | [ ] | Sign waiver |
| M46 | Contact Us | `ContactUsPage` | [ ] | [ ] | Submit form |

---

## Progress Summary

| Section | Total | Reviewed | Remaining |
|---------|-------|----------|-----------|
| Navigation & Auth | 3 | | |
| Home & Explore | 6 | | |
| Events | 19 | | |
| Litter Reports | 3 | | |
| Pickup Locations | 3 | | |
| Teams & Communities | 4 | | |
| Impact & Gamification | 3 | | |
| Profile & Settings | 5 | | |
| **TOTAL** | **46** | | |
