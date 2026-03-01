# Event Day Playbook

This playbook covers what to do when things go wrong during a live cleanup event. Time is critical on event day — follow these steps to resolve issues quickly and keep the event running smoothly.

---

## Before the Event

### Pre-Event Checklist (Event Lead)
- [ ] Verify the event is visible on the platform and details are correct
- [ ] Confirm attendee registrations and send any reminder communications
- [ ] Test the mobile app on your device (open it, verify you can see the event)
- [ ] Charge your phone — route tracking uses GPS and drains battery
- [ ] Download offline maps for the area (Google Maps → download region) in case of poor connectivity
- [ ] Know who to contact for support: info@trashmob.eco
- [ ] If waivers are required, bring paper waiver forms as backup

### Pre-Event Checklist (Support)
- [ ] Be aware of scheduled major events for the day
- [ ] Monitor the support email and in-app feedback for event-related issues
- [ ] Ensure you can reach engineering if a critical issue arises

---

## Common Event Day Issues

### 1. Attendees Can't Register for the Event

**Symptoms:** User gets an error when trying to register, or the register button doesn't work.

**Quick Fixes:**
1. **Waiver required:** If the event or community requires a waiver, the user must sign it first. Walk them through the waiver signing process.
2. **Event is full:** Check if there's an attendee cap. The event lead can increase it if needed.
3. **Event has already started:** Some events may restrict late registration. The event lead can adjust the event times.
4. **App needs update:** If on mobile, ensure the user has the latest version.

**Workaround:** The event lead can manually add attendees from the event management page.

---

### 2. Route Tracking Won't Start

**Symptoms:** User taps "Start Route" but nothing happens, or the route button is grayed out.

**Quick Fixes:**
1. **Check location permissions:** The app needs location access. Go to phone Settings → TrashMob → Location → set to "While Using the App" or "Always."
2. **Restart the app:** Force-close and reopen TrashMob.
3. **Check GPS signal:** Move to an area with a clear view of the sky. GPS is unreliable indoors or in dense urban canyons.
4. **Check internet:** Route tracking needs connectivity to start (though it can buffer points briefly during dropouts).

**Workaround:** If route tracking won't work on one device, another attendee can record a route for the group. Routes can be shared via privacy settings.

---

### 3. Route Tracking Stopped Unexpectedly

**Symptoms:** The route was recording but the app shows it stopped, or the "Recording" indicator disappeared.

**Possible Causes:**
- Phone killed the app in the background to save battery
- App crashed
- Phone ran out of battery

**Quick Fixes:**
1. **Check if the route was saved:** Open the Routes tab on the event — a partial route may have been recorded.
2. **Start a new route:** If the event is still ongoing, start a new recording. Multiple routes per event are supported.
3. **Battery saver:** Disable battery saver / battery optimization for the TrashMob app to prevent the OS from killing it.

**For Android:** Settings → Apps → TrashMob → Battery → Unrestricted
**For iOS:** Settings → TrashMob → Background App Refresh → On

**Workaround:** If route tracking is unreliable, focus on logging the event summary (bags collected, area covered) after the event — this data is still valuable without a GPS route.

---

### 4. Can't Upload Photos During the Event

**Symptoms:** Photo upload button doesn't work, upload fails with an error, or the photo doesn't appear.

**Quick Fixes:**
1. **Check file size:** Photos must be under 10 MB. High-resolution camera photos may exceed this. Try reducing photo quality in camera settings.
2. **Check file type:** Only JPEG, PNG, and WebP are supported. Screenshots in HEIC format (iPhone default) may need to be converted.
3. **Check internet:** Photo uploads require a stable connection. If connectivity is poor, save photos and upload after the event.
4. **Storage permissions:** Ensure the app has permission to access photos/camera.

**Workaround:** Take photos with the phone camera normally. Upload them to the event after returning to a stable internet connection.

---

### 5. Event Details Won't Load / "Data Not Loading"

**Symptoms:** Event page shows a spinner, blank screen, or "error loading" message.

**Quick Fixes:**
1. **Check internet connectivity** — try loading a web page in the browser.
2. **Force-close and reopen the app.**
3. **Check if the issue is platform-wide:** Try accessing trashmob.eco in a web browser.

**If the platform is down:**
- This is a **Critical** escalation — contact engineering immediately.
- **Workaround:** Proceed with the cleanup event as planned. The event data (attendance, route, photos, summary) can all be entered after the event once the platform is back up.

---

### 6. Waiver Signing Fails

**Symptoms:** User can't complete the waiver signing process — form doesn't submit, or an error appears.

**Quick Fixes:**
1. **"Typed legal name is required"** — The user must type their full legal name in the signature field.
2. **"This waiver version has expired"** — Contact the community admin to update the waiver's effective dates.
3. **"This waiver version is not yet effective"** — The waiver's start date is in the future. Contact the community admin.
4. **General form submission failure** — Check internet connectivity and try again.

**Workaround:** Use a **paper waiver** as backup. Bring printed copies of the waiver. Paper waivers can be scanned and uploaded to the platform after the event by the event lead or community admin.

---

### 7. Event Summary Can't Be Submitted

**Symptoms:** After the event, the lead tries to submit the event summary (bags collected, weight, attendees) but gets an error.

**Quick Fixes:**
1. **"Event not found"** — Verify the event hasn't been cancelled. Refresh the page.
2. **"You must be registered as an attendee"** — The person submitting must be registered for the event. Register them first.
3. **Form validation errors** — Check that all required fields are filled in with valid numbers.

**Workaround:** If the summary can't be submitted immediately, note the data on paper and submit later when the issue is resolved. Key data to capture:
- Number of bags collected
- Total weight (estimate if no scale available)
- Number of attendees
- Duration of the event
- Any notes about the area or conditions

---

### 8. Pickup Location Issues

**Symptoms:** Can't add a pickup location, or the pickup location doesn't appear on the map.

**Quick Fixes:**
1. Ensure location services are enabled for accurate pin placement
2. Check internet connectivity
3. Try refreshing the event page

**Workaround:** Communicate pickup locations verbally or via text message to attendees. Add them to the platform after the event.

---

## Emergency Escalation

### When to Escalate Immediately

| Situation | Action |
|-----------|--------|
| **Platform completely down** (trashmob.eco unreachable) | Contact engineering immediately. Proceed with event offline. |
| **Data loss** (routes, event data disappeared) | Contact engineering + Director. Document what was lost. |
| **Security concern** (suspicious activity, unauthorized access) | Contact Director immediately. |

### How to Escalate

1. **Email:** info@trashmob.eco (include "EVENT DAY - URGENT" in subject line)
2. **GitHub Issue:** Create with `critical` + `event-day` labels
3. **Direct contact:** Reach out to the Director of Product & Engineering

### What to Include in an Escalation

- Event name and date
- Number of attendees affected
- Description of the issue
- Steps that were tried
- Screenshots if possible
- Device/browser information

---

## After the Event

### Post-Event Checklist

- [ ] Submit event summary (bags, weight, attendees, duration) if not done during the event
- [ ] Upload any photos that couldn't be uploaded during the event
- [ ] Review and trim any route recordings if they include driving time
- [ ] Report any platform issues encountered during the event (even if workarounds were found)
- [ ] Thank attendees and share impact metrics

### Reporting Issues After the Fact

Even if workarounds were used during the event, please report any platform issues encountered:
1. Use the in-app feedback widget, or
2. Email info@trashmob.eco with details, or
3. Create a GitHub Issue

This helps the team fix issues before the next event.

---

**Last Updated:** March 1, 2026
