# Troubleshooting Guide

This guide covers common issues users encounter on TrashMob.eco and how to resolve them. It is organized by feature area and intended for support volunteers answering user questions.

> **Tip:** The in-app admin guide at `/siteadmin/guide` covers feature documentation (how things work). This guide focuses on what to do when things go wrong.

---

## Table of Contents

1. [Account & Login](#account--login)
2. [Events](#events)
3. [Waivers](#waivers)
4. [Route Tracking (Mobile)](#route-tracking-mobile)
5. [Litter Reports](#litter-reports)
6. [Photos & Image Upload](#photos--image-upload)
7. [Communities & Adoptions](#communities--adoptions)
8. [Teams](#teams)
9. [Partners](#partners)
10. [Mobile App (General)](#mobile-app-general)
11. [Web App (General)](#web-app-general)

---

## Account & Login

### "I can't sign in" / Login page shows an error

**Possible causes:**
- Browser cache or cookies interfering with authentication
- Entra External ID (CIAM) session expired

**Resolution:**
1. Clear browser cookies and cache for trashmob.eco
2. Try signing in using an incognito/private browser window
3. If using social login (Google, etc.), verify the social account email matches the TrashMob account
4. If the error persists, note the exact error message and escalate to engineering

### "I created an account but my data isn't loading"

**Possible cause:** First-time login may take a moment to provision the user account on the backend.

**Resolution:**
1. Sign out completely
2. Wait 30 seconds
3. Sign back in — the system retries user lookup automatically
4. If data still doesn't load after a second sign-in, escalate to engineering with the user's email address

### "I forgot my password"

**Resolution:** Use the "Forgot password" link on the sign-in page. The system uses Entra External ID (CIAM), which handles password reset via email verification.

---

## Events

### "I can't create an event"

**Common validation errors and fixes:**

| Error Message | Fix |
|---------------|-----|
| "Event name is required." | Enter an event name |
| "Event description is required." | Enter an event description |
| "Event duration must be at least 1 hour." | Set duration to 1 hour or more |
| "Event duration cannot exceed 10 hours." | Reduce duration to 10 hours or less |
| "Event dates for public events must be in the future." | Set the event date to a future date |
| "A team must be selected for Team Only events." | Select a team from the dropdown if event type is "Team Only" |

### "I can't register for an event" / Waiver prompt appears

**Cause:** The event (or the community hosting it) requires a signed waiver before registration.

**Resolution:**
1. The system will prompt the user to sign the required waiver(s)
2. Read and accept the waiver terms by typing their legal name
3. After signing, retry event registration
4. If the waiver prompt keeps appearing after signing, escalate to engineering

### "I promoted someone to co-lead but now I can't remove them"

**Note:** Events can have a maximum of 5 co-leads. The last remaining event lead cannot be removed — at least one lead is always required.

### "I accidentally cancelled my event"

**Resolution:** Currently, event cancellation cannot be undone by users. Contact support (engineering) to investigate restoring the event if needed.

---

## Waivers

### "I already signed this waiver but it's asking me again"

**Possible causes:**
- The community admin published a new waiver version — users must sign each new version
- The previous waiver version may have expired

**Resolution:**
1. Check if the waiver version number or effective date has changed
2. If it's a new version, the user needs to sign the updated waiver
3. If the user believes they already signed this exact version, note the waiver name and user email, then escalate

### "This waiver version is not yet effective" / "This waiver version has expired"

**Cause:** The waiver has a defined effective date range and the current date falls outside it.

**Resolution:** Contact the community admin who manages the waiver to update the effective dates. Community admins can manage waivers from their community dashboard.

### "I need to upload a paper waiver for someone"

**Resolution:** Community admins and event leads can upload scanned paper waivers:
1. Go to the event's waiver management section
2. Use the "Upload Paper Waiver" option
3. Accepted formats: PDF, JPEG, PNG, WebP (max 10 MB)
4. Enter the signer's name as it appears on the paper waiver

---

## Route Tracking (Mobile)

### "My route didn't record any data"

**Possible causes:**
- Location permissions were not granted to the TrashMob app
- GPS signal was weak (indoors, dense urban area, heavy tree cover)
- The route was started and stopped too quickly (fewer than 2 GPS points)

**Resolution:**
1. Verify location permissions: Settings → TrashMob → Location → "While Using the App" or "Always"
2. Ensure the phone has a clear view of the sky for GPS signal
3. Routes need at least 2 GPS points to be recorded — walk for at least 30 seconds before stopping
4. If permissions are correct and the route still didn't record, note the device model and OS version for the bug report

### "My route distance seems wrong" / Route includes driving distance

**Cause:** The route recorder captures GPS points continuously. If the user drove to/from the cleanup site while recording, that distance is included.

**Resolution:** Use the **Trim Route** feature:
1. Open the route in the event details
2. Tap "Trim" on the route card
3. Select a new end time that excludes the driving portion
4. The route distance and duration will recalculate based on the trimmed time range
5. If the user trimmed too aggressively, they can "Restore" to undo the trim

### "I can't delete my route"

**Possible causes:**
- The user may not be viewing their own route (routes from other attendees appear on the event's Routes tab but cannot be deleted by other users)
- The route may have already been included in event metrics

**Resolution:** Users can only delete their own routes. The "Delete" button should appear on routes where `IsOwnRoute` is true.

### "My route shows the wrong privacy level"

**Resolution:** Users can change route privacy from the route card:
1. Tap the "Privacy: [level]" button on the route card
2. Options: Private (only you), EventOnly (event attendees), Public (anyone)
3. Privacy changes take effect immediately

---

## Litter Reports

### "I can't submit a litter report"

**Common validation errors:**

| Error Message | Fix |
|---------------|-----|
| "Litter report must include at least one image." | Attach at least one photo |
| "Litter report name is required." | Enter a name/title for the report |
| "Invalid file type" | Use JPEG, PNG, or WebP images only |
| "File too large" | Each image must be under 10 MB |

### "My litter report location is wrong"

**Cause:** The report location is derived from the photo's GPS metadata or the user's current location. If the photo doesn't have location data, it falls back to the device's current GPS position.

**Resolution:**
1. When submitting, verify the map pin is in the correct location
2. Drag the map pin to adjust the location if needed
3. For best results, take the photo at the litter location with location services enabled

---

## Photos & Image Upload

### "My photo upload failed"

**Common causes and fixes:**

| Error | Fix |
|-------|-----|
| "Invalid file type" | Use JPEG, PNG, or WebP only |
| "File too large" | Resize the image to under 10 MB |
| "Maximum 5 images allowed" | Remove an existing image before adding a new one |

### "My uploaded photo doesn't appear"

**Cause:** Photos go through a moderation queue. Site admins review and approve uploaded photos before they appear publicly.

**Resolution:**
1. The photo was likely uploaded successfully and is in the moderation queue
2. Site admins can approve photos from the Photo Moderation section in the admin panel
3. If the photo has been waiting more than a few days, escalate to a site admin

---

## Communities & Adoptions

### "I submitted an adoption application but haven't heard back"

**Cause:** Adoption applications require approval from the community admin.

**Resolution:**
1. Applications are reviewed by the community admin for the relevant community
2. Processing time depends on the community's review cadence
3. The user will receive an email notification when their application is approved or rejected
4. If it's been more than 2 weeks, contact the community admin to follow up

### "I can't find adoptable areas in my community"

**Possible causes:**
- The community hasn't created adoptable areas yet
- The community may not have the Adopt-a-Location feature enabled

**Resolution:**
1. Check the community's page — adoptable areas appear on the community detail map
2. If no areas are shown, the community admin needs to create them (via AI generation, bulk import, or manual creation)
3. Direct the community admin to the admin guide section on "Adoptable Areas"

---

## Teams

### "I can't join a team"

**Possible causes:**
- The team may be inactive
- The user may already be a member

**Resolution:**
1. Verify the team is active (visible in the teams listing)
2. Check if the user is already a member — the UI should show "Leave Team" instead of "Join Team"
3. If the team appears active but joining fails, note the team name and user email for escalation

### "I left a team by accident"

**Resolution:** The user can rejoin the team by navigating to the team page and clicking "Join Team" again. Team membership is self-service.

---

## Partners

### "I submitted a partner request but haven't heard back"

**Cause:** Partner requests require approval by a site admin.

**Resolution:**
1. Partner requests are reviewed by TrashMob site administrators
2. The user will receive an email when their request is processed
3. If it's been more than 2 weeks, escalate to the site admin team

### "I can't manage my partner organization"

**Cause:** Only designated partner admins can manage partner details.

**Resolution:**
1. Verify the user is listed as a partner admin for the organization
2. Partner admin access is managed from the partner's admin panel
3. If the user should have access but doesn't, a current partner admin or site admin can grant it

---

## Mobile App (General)

### "The app crashes on startup"

**Resolution:**
1. Force-close the app and reopen it
2. Check for app updates in the App Store / Google Play
3. Restart the phone
4. If the crash persists, uninstall and reinstall the app (login data is cloud-stored and will be restored)
5. Note the device model, OS version, and when the crash started for the bug report

### "The app says I need to update"

**Cause:** A minimum app version is required to ensure compatibility with the current API.

**Resolution:** Update the app from the App Store (iOS) or Google Play (Android). The force-update prompt cannot be bypassed.

### "No internet connection" error

**Resolution:**
1. Verify the phone has an active internet connection (Wi-Fi or cellular data)
2. Try opening a web page in the browser to confirm connectivity
3. If connected but the app still shows the error, the TrashMob servers may be temporarily unavailable — try again in a few minutes

### "The map doesn't load" / Map shows a gray area

**Possible causes:**
- Google Maps API key issue (engineering concern)
- Poor internet connection preventing map tiles from loading

**Resolution:**
1. Check internet connectivity
2. Try closing and reopening the app
3. If the map consistently fails to load, escalate to engineering — this may indicate a Google Maps API configuration issue

---

## Web App (General)

### "The page won't load" / Blank white screen

**Resolution:**
1. Try a hard refresh: Ctrl+Shift+R (Windows) or Cmd+Shift+R (Mac)
2. Clear browser cache and cookies for trashmob.eco
3. Try a different browser (Chrome, Firefox, Edge)
4. Check if the issue occurs in incognito/private mode
5. If the issue persists across browsers, the server may be down — escalate to engineering

### "Data doesn't load" / Spinner never stops

**Possible causes:**
- Network connectivity issues
- API server may be slow or unavailable
- Azure SQL firewall may be blocking requests (dev environments)

**Resolution:**
1. Check internet connectivity
2. Try refreshing the page
3. If specific data doesn't load (e.g., events list, user profile), note what page and what data is missing
4. Escalate to engineering if the issue persists for more than 15 minutes

### "I got an error submitting a form"

**Resolution:**
1. Check for validation error messages highlighted on the form fields
2. Ensure all required fields are filled in
3. If no validation errors are shown but submission still fails, try again — it may be a transient network issue
4. If the error repeats, note the exact error message and the form being submitted, then escalate

---

## Escalation Quick Reference

| Symptom | Escalate To | Priority |
|---------|-------------|----------|
| Site completely down | Engineering | Critical |
| Data loss / data corruption | Engineering + Director | Critical |
| Security vulnerability reported | Engineering + Director | Critical |
| Feature broken for all users | Engineering | High |
| Login/auth failures for multiple users | Engineering | High |
| Single-user bug (reproducible) | GitHub Issue → Engineering | Normal |
| Cosmetic / typo issue | GitHub Issue | Low |

---

**Last Updated:** March 1, 2026
