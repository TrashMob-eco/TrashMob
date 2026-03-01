# Custom Waivers: How TrashMob Lets Every Community Set Its Own Liability Rules

**Slug:** custom-waivers-per-community
**Author:** Joe Beernink
**Category:** Features
**Tags:** ["waivers", "features", "communities", "liability", "compliance", "legal", "partners"]
**Featured:** true
**Estimated Read Time:** 5

---

## Excerpt

A walkthrough of TrashMob's custom waiver system — how communities create their own liability waivers alongside the platform-wide waiver, how volunteers sign them during event registration, and how the compliance dashboard gives program managers a complete audit trail.

---

## Body

Every cleanup organization has liability paperwork. It's the price of putting volunteers on a roadside or in a park with grabbers and trash bags. But not every organization has the same paperwork.

A city parks department needs language about municipal property. A state highway authority needs language about traffic safety. A corporate sponsor might need an image release clause. The TrashMob platform waiver covers general liability, but partner communities need the ability to layer their own requirements on top.

That's what custom waivers per community does. Each partner community can have one or more custom waivers that volunteers must sign before attending events in that community — in addition to the standard TrashMob waiver. No paper forms. No clipboard at the trailhead. Everything is signed electronically before the volunteer shows up.

### How waivers work on TrashMob

Every volunteer on TrashMob signs the platform's **global waiver** before attending their first event. It covers general liability, assumption of risk, and the basics that apply to every cleanup regardless of location.

Global waivers have versions. When TrashMob updates the waiver text, a new version takes effect and volunteers sign the updated version at their next event. Old versions expire, and the system tracks exactly which version each person signed and when.

But a global waiver can only go so far. When a city partners with TrashMob to run a community cleanup program, that city often has its own legal requirements — specific indemnification language, park rules acknowledgments, or equipment liability clauses that their legal team requires.

### Community-scoped waivers

Custom waivers use a **scope** system. Every waiver version is tagged as either **Global** (applies to all events platform-wide) or **Community** (applies only to events within a specific partner community).

A site admin creates a community-scoped waiver, writes the text in markdown, sets an effective date, and assigns it to one or more partner communities. The assignment creates a link between the waiver and the community, with a flag marking whether it's required or optional.

Once assigned, the waiver appears automatically whenever a volunteer tries to RSVP for an event in that community. The volunteer doesn't need to know which waivers come from TrashMob and which come from the community — they see all required waivers in sequence and sign each one.

### The signing experience

When you RSVP for a cleanup event, TrashMob checks which waivers you need. The system looks at:

1. The current global waiver — have you signed it? Has it expired?
2. The event's associated communities — which partners are involved?
3. Each community's required waivers — have you signed the current versions?

If any waivers are missing, a **signing dialog** appears before your RSVP goes through. The dialog shows the full waiver text in a scrollable container. You must scroll to the bottom before the sign button activates — this isn't a checkbox you can click past.

Then you type your legal name as an electronic signature. If you're signing for a minor (age 13+), you provide your name, relationship, and guardian details. Hit sign, and TrashMob captures your typed signature, the exact waiver text you agreed to, your IP address, browser information, and a timestamp. A PDF is generated and stored for your records.

If you have multiple waivers to sign, the dialog walks you through them one at a time with a progress indicator ("1 of 3"). Once all waivers are signed, your RSVP goes through normally.

### What happens at the event

For **event leads**, there's a practical reality: not everyone signs waivers online. Some volunteers show up day-of without registering. Some bring friends. Some don't have smartphones.

TrashMob handles this with **paper waiver uploads**. An event lead can upload a signed paper waiver (photo or PDF) on behalf of an attendee. The system creates a waiver record with `SigningMethod: PaperUpload`, tracks who uploaded it and when, and stores the document in the same blob storage as electronic signatures.

Event leads can also check the **waiver status of their attendees** — a quick view showing who has signed, who hasn't, and which waivers are missing. No more wondering if that new volunteer filled out the clipboard.

### Waiver versioning and expiry

Waivers aren't sign-once-and-forget. TrashMob's waiver system handles versioning and expiry:

**Versions.** When a community updates its waiver text — maybe their legal team added a new clause or changed indemnification language — the admin creates a new version with a new effective date. The old version stays in the system (it's a legal record), but volunteers signing going forward get the new text. The system tracks exactly which version each person signed.

**Annual expiry.** Waiver acceptances expire at the end of the calendar year. When a new year starts, volunteers re-sign the current waiver at their next event. This ensures ongoing consent and catches any waiver text updates.

**Expiry notifications.** The system identifies volunteers with waivers expiring soon and can notify them proactively. Admins can see who's expiring in the compliance dashboard and plan accordingly.

### The compliance dashboard

The site admin compliance dashboard turns waiver data into actionable metrics:

- **Total active users** vs. **users with valid waivers** — your compliance percentage at a glance
- **Users with expiring waivers** — who needs to re-sign soon
- **Signing method breakdown** — how many signed electronically (web or mobile) vs. paper uploads
- **Minor waivers** tracked separately for COPPA compliance
- **Filterable waiver list** — search by user, waiver version, signing method, date range, or validity status
- **CSV export** — download the full waiver record for legal review or audit

For a program manager running cleanups across a city, this dashboard answers the question that used to require a filing cabinet and a spreadsheet: "Are my volunteers covered?"

### PDF records and audit trail

Every signed waiver produces a **PDF document** stored in Azure Blob Storage. The PDF includes:

- The complete waiver text as it appeared when signed
- The signer's typed legal name
- Date of acceptance and expiry date
- Signing method (web, mobile, or paper upload)
- IP address and browser information (for electronic signatures)
- Guardian consent section (for minors)
- A unique document ID

Volunteers can download their own waiver PDFs from their dashboard at any time. This isn't a black box — you can see exactly what you signed and when.

The audit trail matters for legal defensibility. If a question ever arises about whether a volunteer signed a waiver, the system has the timestamped record, the exact text they agreed to, and the digital evidence of their consent.

### Supporting minors safely

TrashMob allows volunteers ages 13 and up. For minors, the waiver system requires additional information:

- The minor is flagged during signing
- A guardian must provide their name and relationship
- If the guardian is also a registered TrashMob user, their account is linked
- The PDF includes a dedicated guardian consent section
- The compliance dashboard tracks minor waivers separately

This keeps the platform compliant with child safety requirements while still making it straightforward for families to volunteer together.

### Why this matters for communities

Before custom waivers, partner communities had two choices: use only the TrashMob platform waiver (which might not satisfy their legal team), or manage a separate paper waiver process alongside the digital one. Neither was great.

Now a city can say: "Here's our parks department liability waiver. Everyone who cleans up in our parks needs to sign it." TrashMob handles the rest — presenting it at the right time, capturing the signature, storing the record, tracking expiry, and reporting compliance.

The volunteer signs one extra screen. The program manager gets a compliance dashboard instead of a filing cabinet. And the city's legal team gets the exact language they require, versioned and auditable.

**Learn more or set up your community at [trashmob.eco](https://www.trashmob.eco).**

---

*TrashMob.eco is a 501(c)(3) nonprofit dedicated to empowering communities to keep their neighborhoods clean.*

---

## Social Posts

### LinkedIn

We just published a walkthrough of how custom waivers work on TrashMob — the feature that lets every partner community set its own liability requirements alongside the platform-wide waiver.

The highlights:

- Community-scoped waivers assigned to specific partner organizations
- Automatic presentation during event RSVP — volunteers sign all required waivers in sequence
- Electronic signatures with typed legal name, IP capture, and PDF generation
- Paper waiver uploads for day-of volunteers who show up without registering
- Annual expiry with automatic re-signing at next event
- Compliance dashboard with real-time metrics, filters, and CSV export
- Full minor/guardian support for volunteers ages 13+

One dashboard replaces the filing cabinet, the clipboard at the trailhead, and the compliance spreadsheet.

trashmob.eco | info@trashmob.eco

#LiabilityManagement #CommunityCleanup #CivicTech #Compliance #NonProfit #Waivers #VolunteerManagement

### Reddit (r/DeTrashed)

We wrote up how custom waivers work on TrashMob.eco — the free platform for organizing litter cleanups.

The problem: when a city partners with you for cleanups, their legal team usually wants their own waiver language. Managing a separate paper process alongside a digital platform is a pain.

TrashMob now lets each partner community have its own waivers. When a volunteer RSVPs for an event in that community, they see all required waivers (platform + community-specific) in sequence and sign electronically. No clipboard at the trailhead.

Practical details:

- Waivers are versioned — new text means a new version, old records preserved
- Acceptances expire end of calendar year (automatic re-sign next event)
- Event leads can upload paper waivers for walk-up volunteers
- PDF generated for every signature with full audit trail
- Compliance dashboard shows who's signed, who's expiring, export to CSV
- Minor support: guardian consent, separate tracking, COPPA-compliant

We're a 501(c)(3) nonprofit. Free for individual volunteers and community organizers.

Full post: trashmob.eco/news

### Bluesky

New post: how custom waivers work on TrashMob.

Every partner community can set their own liability waiver. Volunteers sign it electronically during RSVP. PDF generated, audit trail captured, compliance dashboard for program managers.

No clipboard. No filing cabinet. No compliance spreadsheet.

trashmob.eco/news

### Newsletter

This week we published a deep dive into custom waivers per community — the feature that lets every partner organization layer their own liability requirements on top of the TrashMob platform waiver. Here's how it works: when a city or organization partners with TrashMob, they can create community-scoped waivers with their specific legal language. Those waivers appear automatically when a volunteer RSVPs for an event in that community. The volunteer signs electronically — scrolling through the text, typing their legal name, and submitting. TrashMob captures the signature, generates a PDF, records the IP address and timestamp, and stores everything for audit purposes. Waivers are versioned, so when legal language changes, the new version takes effect without breaking old records. Acceptances expire at year-end, prompting automatic re-signing. Event leads can upload paper waivers for walk-up volunteers. And the compliance dashboard gives program managers real-time metrics on signing rates, expiring waivers, and minor consent — all exportable to CSV. Read the full walkthrough on our news page.
