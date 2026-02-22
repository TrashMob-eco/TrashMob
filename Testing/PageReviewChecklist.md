# TrashMob.eco — Website Page Review Checklist

Track your page-by-page review of the TrashMob.eco website. For each page, check off:
- **Looks Good** — Layout, styling, branding, responsive behavior, no visual glitches
- **Works** — Interactions function correctly, data loads, forms submit, navigation works

Mark with `[x]` when verified. Add notes in the **Notes** column for anything that needs follow-up.

**Legend:**  `LG` = Looks Good | `W` = Works

> **Mobile app checklist** is in a separate file: [MobilePageReviewChecklist.md](MobilePageReviewChecklist.md)

---

## Table of Contents

1. [Website — Public Pages](#website--public-pages)
2. [Website — User Dashboard & Account](#website--user-dashboard--account)
3. [Website — Events](#website--events)
4. [Website — Litter Reports](#website--litter-reports)
5. [Website — Teams](#website--teams)
6. [Website — Partner Dashboard](#website--partner-dashboard)
7. [Website — Community Management](#website--community-management)
8. [Website — Company Dashboard](#website--company-dashboard)
9. [Website — Sponsor Dashboard](#website--sponsor-dashboard)
10. [Website — Site Admin](#website--site-admin)

---

## Website — Public Pages

| # | Page | Route | LG | W | Notes |
|---|------|-------|----|---|-------|
| 1 | Home | `/` | [x] | [x] | |
| 2 | About Us | `/aboutus` | [ ] | [ ] | Needs an update |
| 3 | Board | `/board` | [x] | [x] | |
| 4 | Contact Us | `/contactus` | [x] | [x] | Submit form, verify email sent |
| 5 | FAQ | `/faq` | [x] | [x] | |
| 6 | Help | `/help` | [x] | [x] | |
| 7 | Getting Started | `/gettingstarted` | [x] | [x] | |
| 8 | What's New | `/whatsnew` | [x] | [x] | Update before ship to prod |
| 9 | Shop | `/shop` | [ ] | [ ] | |
| 10 | Partnerships | `/partnerships` | [x] | [x] | |
| 11 | For Communities | `/for-communities` | [x] | [x] | |
| 12 | Volunteer Opportunities | `/volunteeropportunities` | [x] | [x] | |
| 13 | Privacy Policy | `/privacypolicy` | [x] | [x] | |
| 14 | Terms of Service | `/termsofservice` | [ ] | [ ] | |
| 15 | Unsubscribe | `/unsubscribe` | [ ] | [ ] | Test with valid email |
| 16 | Communities List | `/communities` | [ ] | [ ] | List view + map view |
| 17 | Community Detail | `/communities/:slug` | [ ] | [ ] | Test with a real community |
| 18 | Teams List | `/teams` | [x] | [x] | List view + map view |
| 19 | Team Detail | `/teams/:teamId` | [ ] | [ ] | Test with a real team - Users brought over from the old site show as unknown |
| 20 | Leaderboards | `/leaderboards` | [x] | [ ] | Filter by category + time |
| 21 | Litter Reports List | `/litterreports` | [ ] | [ ] | |
| 22 | Litter Report Detail | `/litterreports/:id` | [ ] | [ ] | Test with a real report
Need to verify photos work |
| 23 | Event Details (public) | `/eventdetails/:eventId` | [ ] | [ ] | Test with a real event |
| 24 | Attendee Metrics Review | `/eventdetails/:eventId/attendee-metrics` | [ ] | [ ] | |
| 25 | Become a Partner | `/becomeapartner` | [ ] | [ ] | Submit form |
| 26 | Invite a Partner | `/inviteapartner` | [ ] | [ ] | Submit form |
| 27 | Partner Request Details | `/partnerrequestdetails/:id` | [ ] | [ ] | |
| 28 | 404 / Not Found | `/nonexistent-page` | [ ] | [ ] | Navigate to invalid URL |

---

## Website — User Dashboard & Account

| # | Page | Route | LG | W | Notes |
|---|------|-------|----|---|-------|
| 29 | My Dashboard | `/mydashboard` | [ ] | [ ] | All sections load |
| 30 | — Stats Overview section | (scroll section) | [x] | [x] | Verify counts are accurate |
| 31 | — Waivers section | (scroll section) | [x] | [x] | Sign/view waivers |
| 32 | — Newsletters section | (scroll section) | [x] | [ ] | Toggle preferences |
| 33 | — Impact Card section | (scroll section) | [x] | [ ] | |
| 34 | — Invite Friends section | (scroll section) | [x] | [ ] | Share links work |
| 35 | — My Events section | (scroll section) | [ ] | [ ] | Upcoming + completed, list/map |
| 36 | — My Teams section | (scroll section) | [ ] | [ ] | |
| 37 | — My Routes section | (scroll section) | [ ] | [ ] | |
| 38 | — Litter Reports section | (scroll section) | [ ] | [ ] | Status filter works |
| 39 | — Partnerships section | (scroll section) | [ ] | [ ] | Sub-sections load |
| 40 | — Professional Companies | (scroll section) | [ ] | [ ] | |
| 41 | — Sponsors section | (scroll section) | [ ] | [ ] | |
| 42 | Location Preference | `/locationpreference` | [ ] | [ ] | Map picker, radius, units |
| 43 | Waivers Page | `/waivers` | [ ] | [ ] | Sign, view, download PDF |
| 44 | Achievements | `/achievements` | [ ] | [ ] | Badges, points, progress |
| 45 | Delete My Data | `/deletemydata` | [ ] | [ ] | |

---

## Website — Events

| # | Page | Route | LG | W | Notes |
|---|------|-------|----|---|-------|
| 46 | Create Event | `/events/create` | [ ] | [ ] | Full form, map picker, partners |
| 47 | Edit Event | `/events/:eventId/edit` | [ ] | [ ] | Pre-populated data |
| 48 | Cancel Event | `/cancelevent/:eventId` | [ ] | [ ] | Reason required, notifications |
| 49 | Event Summary (edit) | `/eventsummary/:eventId` | [ ] | [ ] | Bags, weight, hours, photos |
| 50 | Create Pickup Location | `/eventsummary/:id/pickup-locations/create` | [ ] | [ ] | |
| 51 | Edit Pickup Location | `/eventsummary/:id/pickup-locations/:locId/edit` | [ ] | [ ] | |

---

## Website — Litter Reports

| # | Page | Route | LG | W | Notes |
|---|------|-------|----|---|-------|
| 52 | Create Litter Report | `/litterreports/create` | [x] | [ ] | Photo upload, location |
| 53 | Edit Litter Report | `/litterreports/:id/edit` | [ ] | [ ] | |

---

## Website — Teams

| # | Page | Route | LG | W | Notes |
|---|------|-------|----|---|-------|
| 54 | Create Team | `/teams/create` | [ ] | [ ] | |
| 55 | Edit Team | `/teams/:teamId/edit` | [ ] | [ ] | |
| 56 | Team Invites | `/teams/:teamId/invites` | [ ] | [ ] | Bulk invite |
| 57 | Team Invite Details | `/teams/:teamId/invites/:batchId` | [ ] | [ ] | |

---

## Website — Partner Dashboard

| # | Page | Route | LG | W | Notes |
|---|------|-------|----|---|-------|
| 58 | Partner Index (service requests) | `/partnerdashboard/:id` | [ ] | [ ] | |
| 59 | Edit Partner | `/partnerdashboard/:id/edit` | [ ] | [ ] | |
| 60 | Partner Locations | `/partnerdashboard/:id/locations` | [ ] | [ ] | |
| 61 | Create Location | `/partnerdashboard/:id/locations/create` | [ ] | [ ] | |
| 62 | Edit Location | `/partnerdashboard/:id/locations/:locId/edit` | [ ] | [ ] | |
| 63 | Partner Services | `/partnerdashboard/:id/services` | [ ] | [ ] | |
| 64 | Enable Service | `/partnerdashboard/:id/services/enable` | [ ] | [ ] | |
| 65 | Edit Service | `/partnerdashboard/:id/services/edit` | [ ] | [ ] | |
| 66 | Partner Contacts | `/partnerdashboard/:id/contacts` | [ ] | [ ] | |
| 67 | Create Contact | `/partnerdashboard/:id/contacts/create` | [ ] | [ ] | |
| 68 | Edit Contact (org-wide) | `/partnerdashboard/:id/contacts/:cId/edit` | [ ] | [ ] | |
| 69 | Edit Contact (location) | `/partnerdashboard/:id/contacts/by-location/:cId/edit` | [ ] | [ ] | |
| 70 | Partner Admins | `/partnerdashboard/:id/admins` | [ ] | [ ] | |
| 71 | Invite Admin | `/partnerdashboard/:id/admins/invite` | [ ] | [ ] | |
| 72 | Partner Documents | `/partnerdashboard/:id/documents` | [ ] | [ ] | |
| 73 | Create Document | `/partnerdashboard/:id/documents/create` | [ ] | [ ] | |
| 74 | Edit Document | `/partnerdashboard/:id/documents/:docId/edit` | [ ] | [ ] | |
| 75 | Social Media Accounts | `/partnerdashboard/:id/socials` | [ ] | [ ] | |
| 76 | Create Social Account | `/partnerdashboard/:id/socials/create` | [ ] | [ ] | |
| 77 | Edit Social Account | `/partnerdashboard/:id/socials/:acctId/edit` | [ ] | [ ] | |

---

## Website — Community Management

These pages are under `/partnerdashboard/:id/community/...` and are only accessible to Community-type partners.

| # | Page | Route (suffix) | LG | W | Notes |
|---|------|----------------|----|---|-------|
| 78 | Community Dashboard | `/community` | [ ] | [ ] | |
| 79 | Community Content | `/community/content` | [ ] | [ ] | Branding, photos, tagline |
| 80 | Adoptable Areas | `/community/areas` | [ ] | [ ] | |
| 81 | Create Area | `/community/areas/create` | [ ] | [ ] | Map editor |
| 82 | Edit Area | `/community/areas/:areaId/edit` | [ ] | [ ] | |
| 83 | Area Defaults | `/community/area-defaults` | [ ] | [ ] | |
| 84 | Adoptions | `/community/adoptions` | [ ] | [ ] | |
| 85 | Sponsors | `/community/sponsors` | [ ] | [ ] | |
| 86 | Create Sponsor | `/community/sponsors/create` | [ ] | [ ] | |
| 87 | Edit Sponsor | `/community/sponsors/:sponsorId/edit` | [ ] | [ ] | |
| 88 | Professional Companies | `/community/companies` | [ ] | [ ] | |
| 89 | Create Company | `/community/companies/create` | [ ] | [ ] | |
| 90 | Edit Company | `/community/companies/:companyId/edit` | [ ] | [ ] | |
| 91 | Sponsored Adoptions | `/community/sponsored-adoptions` | [ ] | [ ] | |
| 92 | Create Sponsored Adoption | `/community/sponsored-adoptions/create` | [ ] | [ ] | |
| 93 | Edit Sponsored Adoption | `/community/sponsored-adoptions/:adoptionId/edit` | [ ] | [ ] | |
| 94 | Community Invites | `/community/invites` | [ ] | [ ] | |
| 95 | Invite Details | `/community/invites/:batchId` | [ ] | [ ] | |

---

## Website — Company Dashboard

| # | Page | Route | LG | W | Notes |
|---|------|-------|----|---|-------|
| 96 | Company Dashboard | `/companydashboard/:companyId` | [ ] | [ ] | |
| 97 | Log Cleanup | `/companydashboard/:companyId/log-cleanup` | [ ] | [ ] | |
| 98 | Cleanup History | `/companydashboard/:companyId/history` | [ ] | [ ] | |

---

## Website — Sponsor Dashboard

| # | Page | Route | LG | W | Notes |
|---|------|-------|----|---|-------|
| 99 | Sponsor Dashboard | `/sponsordashboard/:sponsorId` | [ ] | [ ] | |
| 100 | Cleanup History | `/sponsordashboard/:sponsorId/history` | [ ] | [ ] | |
| 101 | Reports | `/sponsordashboard/:sponsorId/reports` | [ ] | [ ] | |

---

## Website — Site Admin

All pages under `/siteadmin/...` — visible only to site administrators.

| # | Page | Route (suffix) | LG | W | Notes |
|---|------|----------------|----|---|-------|
| 102 | Users | `/users` | [ ] | [ ] | Search, filter, manage |
| 103 | Events | `/events` | [ ] | [ ] | Search, filter, delete |
| 104 | Partners | `/partners` | [ ] | [ ] | |
| 105 | Teams | `/teams` | [ ] | [ ] | |
| 106 | Documents | `/documents` | [ ] | [ ] | |
| 107 | Litter Reports | `/litter-reports` | [ ] | [ ] | |
| 108 | Partner Requests | `/partner-requests` | [ ] | [ ] | Approve/reject |
| 109 | Job Opportunities | `/job-opportunities` | [ ] | [ ] | |
| 110 | Create Job | `/job-opportunities/create` | [ ] | [ ] | |
| 111 | Edit Job | `/job-opportunities/:jobId/edit` | [ ] | [ ] | |
| 112 | Send Notifications | `/send-notifications` | [ ] | [ ] | |
| 113 | Email Templates | `/email-templates` | [ ] | [ ] | |
| 114 | Newsletters | `/newsletters` | [ ] | [ ] | Create, schedule, test send |
| 115 | Bulk Invites | `/invites` | [ ] | [ ] | |
| 116 | Invite Details | `/invites/:batchId` | [ ] | [ ] | |
| 117 | User Feedback | `/feedback` | [ ] | [ ] | |
| 118 | Photo Moderation | `/photo-moderation` | [ ] | [ ] | |
| 119 | Manage Content | `/content` | [ ] | [ ] | |
| 120 | Waivers | `/waivers` | [ ] | [ ] | |
| 121 | Create Waiver | `/waivers/create` | [ ] | [ ] | |
| 122 | Edit Waiver | `/waivers/:waiverId/edit` | [ ] | [ ] | |
| 123 | Waiver Compliance | `/waivers/compliance` | [ ] | [ ] | |
| 124 | Prospects | `/prospects` | [ ] | [ ] | |
| 125 | Create Prospect | `/prospects/create` | [ ] | [ ] | |
| 126 | Prospect Discovery | `/prospects/discovery` | [ ] | [ ] | |
| 127 | Prospect Import (CSV) | `/prospects/import` | [ ] | [ ] | |
| 128 | Prospect Analytics | `/prospects/analytics` | [ ] | [ ] | |
| 129 | Prospect Detail | `/prospects/:prospectId` | [ ] | [ ] | |
| 130 | Edit Prospect | `/prospects/:prospectId/edit` | [ ] | [ ] | |

---

## Progress Summary

Use this section to track overall progress.

| Section | Total | Reviewed | Remaining |
|---------|-------|----------|-----------|
| Website — Public Pages | 28 | | |
| Website — Dashboard & Account | 17 | | |
| Website — Events | 6 | | |
| Website — Litter Reports | 2 | | |
| Website — Teams | 4 | | |
| Website — Partner Dashboard | 20 | | |
| Website — Community Mgmt | 18 | | |
| Website — Company Dashboard | 3 | | |
| Website — Sponsor Dashboard | 3 | | |
| Website — Site Admin | 29 | | |
| **TOTAL** | **130** | | |
