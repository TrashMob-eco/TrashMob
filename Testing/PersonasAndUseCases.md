# TrashMob.eco — Personas & Use Cases for User Acceptance Testing

This document defines the user personas expected to interact with the TrashMob.eco website and mobile app, along with the use cases that UAT testers should validate for each persona. Use this as a reference when building test plans, writing test cases, and performing exploratory testing.

---

## Table of Contents

1. [Persona Overview](#persona-overview)
2. [Persona 1: Anonymous Visitor](#persona-1-anonymous-visitor)
3. [Persona 2: Registered Volunteer (Adult)](#persona-2-registered-volunteer-adult)
4. [Persona 3: Minor Volunteer (Age 13–17)](#persona-3-minor-volunteer-age-1317)
5. [Persona 4: Event Lead / Co-Lead](#persona-4-event-lead--co-lead)
6. [Persona 5: Team Lead](#persona-5-team-lead)
7. [Persona 6: Partner Admin](#persona-6-partner-admin)
8. [Persona 7: Community Lead](#persona-7-community-lead)
9. [Persona 8: Professional Company Representative](#persona-8-professional-company-representative)
10. [Persona 9: Sponsor Representative](#persona-9-sponsor-representative)
11. [Persona 10: Site Administrator](#persona-10-site-administrator)
12. [Platform Coverage Matrix](#platform-coverage-matrix)
13. [Cross-Cutting Concerns](#cross-cutting-concerns)

---

## Persona Overview

| # | Persona | Platform Access | Auth Required | Age Restriction |
|---|---------|----------------|---------------|-----------------|
| 1 | Anonymous Visitor | Web | No | None |
| 2 | Registered Volunteer (Adult) | Web + Mobile | Yes | 18+ |
| 3 | Minor Volunteer | Web + Mobile | Yes | 13–17 |
| 4 | Event Lead / Co-Lead | Web + Mobile | Yes | 18+ only |
| 5 | Team Lead | Web + Mobile | Yes | 18+ only |
| 6 | Partner Admin | Web | Yes | 18+ only |
| 7 | Community Lead | Web | Yes | 18+ only |
| 8 | Professional Company Rep | Web | Yes | 18+ only |
| 9 | Sponsor Representative | Web | Yes | 18+ only |
| 10 | Site Administrator | Web | Yes | 18+ only |

> **Note:** Personas are cumulative. An Event Lead has all the capabilities of a Registered Volunteer. A Community Lead has all the capabilities of a Partner Admin, and so on. Test each persona at its own level and verify that inherited capabilities still work.

---

## Persona 1: Anonymous Visitor

**Who they are:** A person who lands on the TrashMob.eco website without signing in. They may be exploring what TrashMob is, looking for local events, or evaluating whether to join.

**Goals:** Learn about TrashMob, discover events and communities, decide whether to register.

**Platform:** Website only (mobile app requires authentication).

### Use Cases

#### UC-1.1: Browse Public Pages
- Visit Home page and view hero section, stats, event showcase, and "Getting Started" call-to-action
- Navigate to About Us, Board, FAQ, Help, What's New, and Shop pages
- View Privacy Policy and Terms of Service
- Access Contact Us page and submit a contact form (name, email, message)
- **Validate:** All pages load without errors; navigation is consistent; no authenticated content leaks through

#### UC-1.2: Discover Events
- Browse the events list and map on the home page or event discovery page
- View event details (name, date, location, description, attendee count)
- **Validate:** Event details are visible without sign-in; "Register" or "Attend" buttons prompt sign-in; private events are not visible

#### UC-1.3: Browse Communities
- Navigate to the Communities page (`/communities`)
- Switch between list and map views
- Click into a specific community to view its detail page (description, events, teams, gallery)
- **Validate:** Community pages load with branding (logo, colors, tagline); events shown are within community bounds

#### UC-1.4: Browse Teams
- Navigate to the Teams page (`/teams`)
- Switch between list and map views
- Click into a public team to view details (members, events)
- **Validate:** Only public teams are visible; private teams do not appear

#### UC-1.5: View Leaderboards
- Navigate to Leaderboards page
- Switch between user and team leaderboards
- Filter by category (events, bags, hours) and time period (week, month, year, all time)
- **Validate:** Only users with ShowOnLeaderboards=true appear; rankings are accurate

#### UC-1.6: View Litter Reports
- Browse the litter reports list
- Click into a specific report to view details and photos
- **Validate:** Reports are read-only; no edit/delete options shown for anonymous users

#### UC-1.7: Partner Information
- View the Partnerships page
- Access "Become a Partner" form and fill it out
- Access "Invite a Partner" form (if applicable)
- View the "For Communities" landing page
- **Validate:** Forms submit successfully; confirmation is shown; email sent to admin

---

## Persona 2: Registered Volunteer (Adult)

**Who they are:** An adult (18+) who has created an account and wants to participate in cleanup events, report litter, and track their impact.

**Goals:** Find and attend events, report litter, join teams, track personal impact, manage profile.

**Platform:** Website and Mobile App.

### Use Cases — Account Management

#### UC-2.1: Registration & Sign-In
- Register for a new account (username, email — both must be unique)
- Agree to Privacy Policy and Terms of Service (required)
- Sign in to existing account
- **Validate:** Welcome email sent; admin notification email sent; user added to database; duplicate username/email rejected with clear error

#### UC-2.2: Profile & Location Preferences
- Set home location on a map (city, region, country, latitude/longitude)
- Set travel distance radius for local event notifications
- Choose metric or imperial units
- **Validate:** Location persists across sessions; nearby events/reports reflect the chosen radius

#### UC-2.3: Newsletter Preferences
- View available newsletter categories
- Subscribe/unsubscribe from specific categories
- Unsubscribe via email link (no sign-in required)
- **Validate:** Preference changes take effect immediately; unsubscribe link works without auth

#### UC-2.4: Delete My Data
- Request account and data deletion via the "Delete My Data" page
- **Validate:** Request is processed; user data is removed per privacy policy

### Use Cases — Waivers

#### UC-2.5: Sign TrashMob Waiver
- When attempting to create or register for an event, get redirected to waiver signing if no current waiver is on file
- Read the full waiver text (must scroll to bottom)
- Check "I have read and agree" checkbox
- Type full legal name
- Click "Sign Waiver"
- **Validate:** Waiver is recorded with timestamp, IP address, user agent, and waiver version; user can now create/register for events; waiver expires December 31 of the signing year

#### UC-2.6: Re-sign Waiver
- At the start of a new calendar year, attempt to register for an event
- Get redirected to sign the current year's waiver
- **Validate:** Previous year's waiver is marked expired; new waiver signing is required before participation

#### UC-2.7: Download Waiver PDF
- After signing, download a PDF copy of the signed waiver
- **Validate:** PDF contains original waiver text, typed legal name, sign date/time (local + UTC), audit trail, and TrashMob branding

### Use Cases — Events

#### UC-2.8: Browse & Search Events
- View upcoming events near home location (list and map views)
- Filter events by date range, location, type
- View event details: name, date, time, duration, location, description, attendee count, spots remaining
- **Validate:** Only public events appear; past events show in "completed" sections; events outside travel radius are excluded from "nearby" views

#### UC-2.9: Register for an Event
- Click "Register" on an event detail page
- If waiver is not current, get redirected to sign waiver first
- Confirm registration
- **Validate:** User appears in attendee list; notification email sent to event lead; attendee count increments; max participants limit enforced (if set)

#### UC-2.10: Unregister from an Event
- Navigate to an event the user is registered for
- Click "Unregister"
- **Validate:** User removed from attendee list; attendee count decrements

#### UC-2.11: View Completed Events
- From the dashboard, view events the user attended in the past
- Filter by date range
- **Validate:** Only events the user actually attended appear; event summary data (bags, weight) is visible

### Use Cases — Litter Reports

#### UC-2.12: Create a Litter Report
- Create a new litter report with name, description, and location
- Attach 1–5 geo-tagged photos (camera on mobile, file upload on web)
- **Validate:** Report saved with status "New"; photos have GPS coordinates captured; address auto-fills from GPS; minimum 1 photo required

#### UC-2.13: Edit/Delete a Litter Report
- Edit a report the user created (update description, add/remove photos)
- Delete a report the user created
- **Validate:** Only the creator can edit/delete; status changes are tracked; deleting a report assigned to an event unlinks it

#### UC-2.14: View Nearby Litter Reports
- From the dashboard, view litter reports near the user's location
- Filter by status (New, Assigned, Cleaned, Cancelled)
- **Validate:** Reports shown are within the user's travel radius; status filters work correctly

### Use Cases — Teams

#### UC-2.15: Browse & Join Teams
- Browse public teams (list and map views)
- View team details (name, member count, events, adoptions)
- Request to join a team
- **Validate:** Only public teams within 50 miles appear (mobile); join request sent to team lead; user cannot join the same team twice

#### UC-2.16: Leave a Team
- From the dashboard "My Teams" section, leave a team
- **Validate:** User removed from team; member count decrements

### Use Cases — Dashboard & Impact

#### UC-2.17: View Personal Dashboard
- View stats overview: events attended, hours volunteered, bags collected, weight tracked, litter reports submitted
- View upcoming events the user is registered for
- View completed events
- View teams the user belongs to
- **Validate:** Stats are accurate and update after events are completed; all sections load correctly

#### UC-2.18: View Achievements
- Browse earned achievements (badges/milestones)
- View achievement descriptions, point values, and earned dates
- **Validate:** Achievements unlock based on actual participation; points accumulate correctly

#### UC-2.19: View Impact Card
- View personal impact metrics and analytics
- **Validate:** Metrics match dashboard stats

#### UC-2.20: Invite Friends
- Share referral/invite links via social sharing
- **Validate:** Links are functional and point to the correct pages

### Use Cases — Mobile-Specific

#### UC-2.21: GPS Route Tracking
- During an active event, start route tracking
- Walk a cleanup route; GPS records the path
- Stop route tracking
- **Validate:** Route is recorded accurately; route appears on event details; tracking stops when explicitly stopped

#### UC-2.22: Camera-Based Litter Reporting (Mobile)
- Open camera from litter report creation
- Take up to 5 photos
- GPS coordinates captured automatically for each photo
- **Validate:** Photos are clear; GPS coordinates are accurate; address auto-fills correctly

#### UC-2.23: Offline/Error Handling (Mobile)
- Lose network connectivity while using the app
- **Validate:** App shows "No internet connection" message; app does not crash; data is not lost

---

## Persona 3: Minor Volunteer (Age 13–17)

**Who they are:** A teenager who wants to participate in cleanup events. Subject to COPPA compliance and additional safety restrictions. Requires parental/guardian consent.

**Goals:** Attend events with guardian approval, report litter, track impact.

**Platform:** Website and Mobile App.

### Use Cases

#### UC-3.1: Registration with Age Verification
- Register with a date of birth indicating age 13–17
- **Validate:** System identifies user as a minor; additional guardian consent requirements are triggered

#### UC-3.2: Guardian Waiver Signing
- A guardian signs the waiver on behalf of the minor
- Guardian information captured: name, relationship, guardian user ID (if registered)
- **Validate:** Waiver includes IsMinor flag; guardian details are recorded; minor cannot sign their own waiver

#### UC-3.3: Event Registration with Guardian Consent
- Attempt to register for an event
- Guardian consent/waiver must be on file
- **Validate:** Registration blocked if guardian waiver is missing; registration succeeds once guardian waiver is current

#### UC-3.4: Restricted Capabilities
- Attempt to create an event → should be blocked
- Attempt to create a team → should be blocked
- Attempt to become a partner admin → should be blocked
- **Validate:** UI does not show "Create Event" or "Create Team" options; API rejects these actions if attempted directly

#### UC-3.5: Privacy Restrictions
- Minor's profile has limited public visibility
- Photo/visibility restrictions per COPPA
- **Validate:** Minor's PII is not exposed in leaderboards, public event pages, or team lists beyond what's permitted

---

## Persona 4: Event Lead / Co-Lead

**Who they are:** An adult volunteer who organizes cleanup events. They manage event logistics, coordinate with partners, and report outcomes. A Co-Lead is promoted by the Event Lead and shares management capabilities.

**Goals:** Create and manage events, coordinate attendees and partners, report event outcomes.

**Platform:** Website and Mobile App.

### Use Cases — Event Creation & Management

#### UC-4.1: Create an Event
- Fill out event creation form: name (required), location (required, map picker), date (required, must be future for public events), time, duration, type, description
- Set event as public or private
- Set max participants (optional)
- Select partner services available at the event location (matched by city/postal code)
- Assign existing litter reports to the event
- **Validate:** Event saved; email sent to TrashMob admins; email sent to partner location contacts for selected services; waiver must be current before creating

#### UC-4.2: Edit an Event
- Modify event details (name, location, time, description)
- Add or remove partner services
- Add or remove litter report assignments
- **Validate:** If location or time changes, notification emails sent to all registered attendees

#### UC-4.3: Cancel an Event
- Cancel a scheduled event with a cancellation reason
- **Validate:** All registered attendees notified; event marked as cancelled; event no longer appears in active listings

#### UC-4.4: Manage Attendees
- View list of registered attendees
- Remove an attendee from the event
- Promote an attendee to Co-Lead (maximum 5 co-leads per event)
- **Validate:** Co-leads gain event management capabilities; attendee count updates; removed attendees are notified

#### UC-4.5: Manage Pickup Locations
- Add pickup locations for the event
- Edit or remove pickup locations
- **Validate:** Pickup locations appear on the event map; partner contacts can see pickup requests

#### UC-4.6: Record Event Summary (Post-Event)
- After the event, fill out the event summary: number of bags collected, weight of litter, duration, number of attendees
- Upload event photos
- **Validate:** Summary data feeds into user stats, team stats, and leaderboards; photos appear in event gallery

#### UC-4.7: Request Partner Services
- During event creation or editing, browse available partners at the event location
- Request services: Hauling, Disposal, Startup Kits, Supplies
- **Validate:** Service request sent to partner; request appears in partner dashboard; partner contact receives email notification

#### UC-4.8: View Attendee Waiver Status
- Before or during an event, check which attendees have current waivers
- **Validate:** Waiver status is accurate; expired waivers are flagged

### Use Cases — Mobile-Specific

#### UC-4.9: Create Event via Mobile Wizard
- Complete the 5-step event creation wizard on mobile:
  1. Event Details (name, date, times, type, description, public/private)
  2. Location (map-based selection, address auto-fill)
  3. Review/Summary
  4. Event Partners (browse and select)
  5. Litter Reports (assign reports to event)
- **Validate:** All steps work correctly on mobile; location picker uses device GPS; event saves successfully

#### UC-4.10: Cancel Event from Mobile
- Cancel an event with a reason from the mobile app
- **Validate:** Same behavior as web; attendees notified; event delisted

---

## Persona 5: Team Lead

**Who they are:** An adult volunteer who creates and manages a team of volunteers. Teams can adopt cleanup areas, organize group events, and track collective impact.

**Goals:** Build and manage a team, adopt cleanup areas, organize team events, track team impact.

**Platform:** Website and Mobile App.

### Use Cases

#### UC-5.1: Create a Team
- Create a new team with name, description, and settings
- Set team as public or private
- **Validate:** Team created; creator automatically becomes Team Lead; team appears in team listings (if public)

#### UC-5.2: Manage Team Membership
- Invite members to the team (bulk invitations supported)
- Approve or reject join requests from users
- Remove members from the team
- Promote a member to co-lead
- **Validate:** Invitation emails sent; join requests appear in team lead's dashboard; member count updates accurately

#### UC-5.3: Adopt a Cleanup Area
- Submit an application to adopt an adoptable area (defined by a community)
- **Validate:** Application sent to the community/partner admin for review; adoption status tracked (pending, approved, rejected)

#### UC-5.4: Create Team Events
- Create events associated with the team
- **Validate:** Events linked to team; team members can see team events; event stats roll up to team leaderboards

#### UC-5.5: Manage Team Photos
- Upload and manage team photo gallery
- **Validate:** Photos appear on team detail page; inappropriate photos can be flagged

#### UC-5.6: View Team Leaderboards & Stats
- View team ranking on leaderboards
- View aggregate team stats (events, bags, hours)
- **Validate:** Stats include all team members' contributions; rankings are accurate

---

## Persona 6: Partner Admin

**Who they are:** An administrator for a partner organization (Government agency, Business, or Community). They manage the partner's profile, locations, services, and respond to event service requests.

**Goals:** Manage partner presence on TrashMob, respond to service requests, support cleanup events.

**Platform:** Website only.

### Use Cases — Partner Profile Management

#### UC-6.1: Edit Partner Profile
- Update partner name, description, and contact information
- Upload/update partner logo and photos
- **Validate:** Changes reflected on public partner pages; logo appears correctly

#### UC-6.2: Manage Partner Locations
- Add partner locations (disposal sites, pickup points, service centers)
- Edit location details (address, hours, contact info)
- Mark locations as active or inactive
- **Validate:** Only active locations appear in event creation partner matching; location contacts receive service request emails

#### UC-6.3: Manage Partner Services
- Enable/disable services offered: Hauling, Disposal Location, Startup Kits, Supplies
- Configure service details per location
- **Validate:** Only enabled services appear when event leads browse partner services; disabled services are hidden

#### UC-6.4: Manage Partner Contacts
- Add contact persons (name, email, phone, role)
- Assign contacts to specific locations
- **Validate:** Contacts receive email notifications for service requests at their assigned locations

#### UC-6.5: Manage Partner Documents
- Upload agreements, contracts, insurance documents
- View and manage document library
- **Validate:** Documents are stored securely; only partner admins can access them

#### UC-6.6: Manage Social Media Accounts
- Add social media profiles (Facebook, Twitter, Instagram, TikTok)
- **Validate:** Social links appear on partner public page

### Use Cases — Service Request Management

#### UC-6.7: Respond to Event Service Requests
- View incoming service requests from event leads
- Accept or decline requests
- **Validate:** Event lead is notified of response; accepted services appear on event details

#### UC-6.8: Invite Additional Partner Admins
- Send admin invitations to colleagues
- **Validate:** Invitation email sent; invitation appears in recipient's dashboard; accepting grants partner admin access

---

## Persona 7: Community Lead

**Who they are:** A Partner Admin managing a Community-type partner (city or municipality). They have all Partner Admin capabilities plus community-specific features: branded pages, area management, waivers, and enrollment.

**Goals:** Build a community presence, manage adoptable areas, configure community waivers, recruit volunteers and sponsors.

**Platform:** Website only.

### Use Cases — Community Configuration

#### UC-7.1: Configure Community Branding
- Set primary and secondary brand colors
- Upload banner image and logo
- Set tagline and contact info
- **Validate:** Branding appears correctly on the community's public page; colors/logos render properly

#### UC-7.2: Set Geographic Boundaries
- Define community bounds (north, south, east, west coordinates)
- Set region type (City, County, State, Province, Region, Country)
- **Validate:** Events within bounds appear on the community page; events outside bounds are excluded

#### UC-7.3: Configure Community Showcase Page
- Enable/disable community home page
- Mark community as "Featured"
- Customize community content and photos
- **Validate:** Enabled communities appear in the Communities directory; featured communities are highlighted

### Use Cases — Area & Adoption Management

#### UC-7.4: Create Adoptable Areas
- Define cleanup areas/routes on a map
- Set default cleanup frequency and safety requirements
- **Validate:** Areas appear on the community map; teams can submit adoption applications

#### UC-7.5: Review Adoption Applications
- View adoption applications from teams
- Approve or reject applications
- **Validate:** Approved teams are linked to the area; rejected teams are notified

#### UC-7.6: Configure Area Defaults
- Set default settings for new area adoptions (frequency, requirements)
- **Validate:** Defaults apply to newly created areas; existing areas are not affected

### Use Cases — Community Waivers

#### UC-7.7: Create Community-Specific Waivers
- Create waivers that supplement the TrashMob waiver for events in this community
- **Validate:** When a user registers for an event in this community, they must sign both the TrashMob waiver AND the community waiver

#### UC-7.8: Manage Waiver Versions
- Create new waiver versions (new version invalidates previous signatures)
- **Validate:** Users who signed a previous version must re-sign the current version

### Use Cases — Sponsor & Company Management

#### UC-7.9: Manage Sponsors
- Add sponsors who fund adoptable area maintenance
- Create/edit sponsor profiles
- **Validate:** Sponsors are linked to community; sponsor dashboard reflects community data

#### UC-7.10: Manage Professional Companies
- Add professional cleanup companies that service adopted areas
- **Validate:** Company users can log cleanup work against community areas

#### UC-7.11: Manage Sponsored Adoptions
- Create adoption agreements funded by sponsors and serviced by professional companies
- **Validate:** Cleanup logs from companies are tracked against sponsored adoptions

### Use Cases — Outreach

#### UC-7.12: Community Invitations
- Send bulk invitations to recruit community members
- View invitation batch details and response rates
- **Validate:** Invitation emails sent; response tracking works

#### UC-7.13: Prospect Management
- Create and manage community prospects (recruitment pipeline)
- Track prospect activities and outreach history
- **Validate:** Prospects can be converted to partners; activity history is maintained

---

## Persona 8: Professional Company Representative

**Who they are:** An employee of a professional cleanup company that is contracted to service adopted areas. They log cleanup work and track compliance.

**Goals:** Log cleanup activities, maintain compliance with adoption agreements, report on work performed.

**Platform:** Website only.

### Use Cases

#### UC-8.1: View Company Dashboard
- Access the company dashboard with overview and stats
- View assigned adoptable areas and adoption agreements
- **Validate:** Only assigned areas are visible; dashboard loads correctly

#### UC-8.2: Log Cleanup Work
- Record cleanup activities: date, area, bags collected, weight, duration
- Submit cleanup logs
- **Validate:** Logs are recorded against the correct adoption agreement; compliance tracking updates

#### UC-8.3: View Cleanup History
- Browse past cleanup logs
- Filter by date range, area, or adoption agreement
- **Validate:** History is complete and accurate; only the user's company logs are shown

#### UC-8.4: Monitor Compliance Status
- View compliance status for each adoption agreement (frequency requirements met or not)
- **Validate:** Compliance status accurately reflects logged cleanup frequency vs. required frequency

---

## Persona 9: Sponsor Representative

**Who they are:** A representative of an organization that funds adoptable area cleanup operations. They monitor the impact of their sponsorship.

**Goals:** Track sponsored cleanup impact, view compliance reports, monitor sponsorship ROI.

**Platform:** Website only.

### Use Cases

#### UC-9.1: View Sponsor Dashboard
- Access the sponsor dashboard with overview of sponsored initiatives
- **Validate:** Dashboard shows all sponsored adoptions and their status

#### UC-9.2: View Cleanup Impact
- View aggregated cleanup data for sponsored areas
- Browse cleanup history logs from professional companies
- **Validate:** Data matches cleanup logs submitted by professional companies

#### UC-9.3: Generate Reports
- Generate impact reports for sponsored adoptions
- **Validate:** Reports include accurate metrics; data can be exported or printed

---

## Persona 10: Site Administrator

**Who they are:** A TrashMob staff member with full system access. They manage users, moderate content, configure the platform, and handle operational tasks.

**Goals:** Ensure platform integrity, moderate content, manage users and partners, communicate with the community.

**Platform:** Website only (Site Admin menu item visible only to admins).

### Use Cases — Data Management

#### UC-10.1: Manage Users
- Search, filter, and view all user accounts
- View user details and activity history
- Take administrative actions on user accounts
- **Validate:** Search returns accurate results; admin can view any user's data; non-admins cannot access this page

#### UC-10.2: Manage Events
- Search, filter, and view all events
- Delete or archive events
- **Validate:** Admin can manage any event; deletion/archival works correctly; stats update accordingly

#### UC-10.3: Manage Partners
- View and manage all partner organizations
- Approve or reject partner requests
- **Validate:** Partner request approval grants partner admin access; rejection notifies applicant

#### UC-10.4: Manage Teams
- Monitor all teams across the platform
- **Validate:** Admin can view any team's details and membership

#### UC-10.5: Manage Litter Reports
- Moderate litter reports across the platform
- **Validate:** Admin can edit, reassign, or delete any litter report

#### UC-10.6: Manage Job Opportunities
- Create, edit, and manage job listings
- **Validate:** Job listings appear on the appropriate public page

### Use Cases — Communications

#### UC-10.7: Send Notifications
- Broadcast system-wide notifications to users
- **Validate:** Notifications delivered to target audience; notification appears in user dashboards

#### UC-10.8: Manage Newsletters
- Create newsletters with HTML content
- Schedule delivery
- Send test emails before broadcasting
- Target newsletters: all users, specific community, or specific team
- **Validate:** Newsletters sent to correct audience; test send works; scheduling triggers at the right time

#### UC-10.9: Send Bulk Invitations
- Send batch invitations to potential users
- View invite batch details and response rates
- **Validate:** Emails sent; tracking works; duplicate invitations are handled gracefully

### Use Cases — Moderation

#### UC-10.10: Review User Feedback
- View and manage user-submitted feedback
- **Validate:** Feedback items are listed with submission details; admin can mark as reviewed

#### UC-10.11: Moderate Photos
- Review flagged photos (event photos, team photos, litter report images, partner photos)
- Approve or reject flagged content
- **Validate:** Rejected photos are removed from public view; moderation actions are logged with admin ID and timestamp

#### UC-10.12: Manage Content
- Edit platform content (static pages, help text)
- **Validate:** Content changes appear immediately on public pages

### Use Cases — Waivers & Compliance

#### UC-10.13: Manage Waiver Templates
- Create new waiver versions
- Set effective and expiry dates
- Schedule waiver activation
- **Validate:** New waiver version invalidates previous signatures; users prompted to re-sign; expiry email notifications sent

#### UC-10.14: Waiver Compliance Dashboard
- View waiver signing statistics
- Export signed waiver data
- View audit logs for waiver actions
- **Validate:** Compliance data is accurate; exports include all required fields; audit trail is complete

#### UC-10.15: Bulk Waiver Export
- Export waiver records for legal/compliance review
- **Validate:** Export includes all required legal data (signer name, date, waiver text, audit trail)

### Use Cases — Prospect Management

#### UC-10.16: Manage Prospects
- Create, edit, and track partnership prospects
- Import prospects via CSV
- Use discovery tools to find potential partners
- Track prospect activities and outreach emails
- View pipeline analytics
- **Validate:** Prospect pipeline is accurate; CSV import handles duplicates; analytics reflect current data

### Use Cases — Access Control

#### UC-10.17: Verify Admin-Only Access
- Site Admin menu option visible only when user has IsSiteAdmin=true
- Non-admin users cannot access `/siteadmin` routes
- **Validate:** Direct URL navigation to admin pages redirects or shows 403 for non-admins; menu item is hidden for regular users

---

## Platform Coverage Matrix

This matrix shows which use cases should be tested on each platform.

| Use Case Area | Website | Mobile App (iOS) | Mobile App (Android) |
|---------------|---------|-----------------|---------------------|
| Public pages / discovery | Yes | N/A | N/A |
| Registration / sign-in | Yes | Yes | Yes |
| Profile / preferences | Yes | Yes | Yes |
| Waiver signing | Yes | Yes | Yes |
| Event browsing & search | Yes | Yes | Yes |
| Event registration | Yes | Yes | Yes |
| Event creation & editing | Yes | Yes | Yes |
| Event summary (post-event) | Yes | Yes | Yes |
| Litter report creation | Yes | Yes (camera) | Yes (camera) |
| Litter report browsing | Yes | Yes | Yes |
| GPS route tracking | N/A | Yes | Yes |
| Team management | Yes | View only | View only |
| Partner dashboard | Yes | N/A | N/A |
| Community management | Yes | N/A | N/A |
| Professional company portal | Yes | N/A | N/A |
| Sponsor portal | Yes | N/A | N/A |
| Site administration | Yes | N/A | N/A |
| Leaderboards | Yes | Yes | Yes |
| Achievements | Yes | Yes | Yes |
| Contact Us | Yes | Yes | Yes |
| Newsletter preferences | Yes | Yes | Yes |
| Map interactions | Yes | Yes | Yes |

### Mobile Device Test Matrix

| Platform | OS Versions | Recommended Devices |
|----------|-------------|---------------------|
| iOS | 15, 16, 17 | iPhone SE (2020), iPhone 13, iPhone 15 Pro |
| Android | 8.0, 10, 12, 14 | Pixel 5, Samsung Galaxy S21, OnePlus 9 |

---

## Cross-Cutting Concerns

These apply across all personas and should be validated during UAT for every use case.

### Accessibility (WCAG 2.2 AA)
- All interactive elements are keyboard-navigable (web)
- Screen reader support (VoiceOver on iOS, TalkBack on Android)
- Sufficient color contrast ratios
- Semantic HTML on web
- Required fields are visually tagged as required
- All fields have tooltips/labels on hover

### Form Validation
- Required fields show clear error messages when empty
- Fields with restrictions (min/max length, date ranges, numeric bounds) show errors when values are out of range
- Duplicate values (username, email) are rejected with specific error messages

### Email Notifications
- Verify that the correct emails are sent for each action (see individual use cases)
- Emails contain accurate information and working links
- Unsubscribe links work without requiring sign-in

### Error Handling
- Network errors show user-friendly messages (never stack traces)
- Timeout errors are handled gracefully
- 401/403 errors redirect to sign-in or show appropriate "access denied" messages
- 404 errors show a helpful "not found" page

### Data Security
- Users can only view/edit their own data (unless they have admin or partner admin roles)
- API endpoints enforce authentication and authorization
- No PII is exposed in URLs, error messages, or public pages
- Minor user data has additional visibility restrictions

### Performance
- Pages load within reasonable time (target: P95 API latency ≤ 300ms)
- Map views render without significant delay
- Photo uploads complete without timeout
- Lists with many items use pagination

### Responsive Design
- Website is usable on mobile browsers (responsive layout)
- Mobile app adapts to different screen sizes
- Map views are functional on all screen sizes
