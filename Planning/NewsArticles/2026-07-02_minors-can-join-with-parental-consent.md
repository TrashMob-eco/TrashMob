# Teens and Families Can Now Sign Up Together on TrashMob

**Slug:** minors-can-join-with-parental-consent
**Author:** Joe Beernink
**Category:** Announcements
**Tags:** ["Safety", "Minors", "PRIVO", "COPPA", "Parental Consent", "Families"]
**Featured:** true
**Estimated Read Time:** 3

---

## Excerpt

Back in April we announced our partnership with PRIVO to build COPPA-compliant parental consent into TrashMob. That work is now live in production — parents can invite their kids ages 13 and up to join TrashMob with their own account, sign a dependent waiver on their child's behalf, and get notified whenever their minor RSVPs for an event.

---

## Body

### What's live today

Three flows are now available to families:

**1. A parent brings their child to an event.** If you're an adult TrashMob volunteer and you'd like to bring your kids to a cleanup you're attending, you can register them as *dependents* on your profile and sign a single waiver on their behalf. No separate account needed. Best for younger kids (under 13) and for families whose teens aren't ready to manage their own account yet.

**2. A parent invites their teen to get their own account.** For teens 13 to 17 who want to sign up for events on their own, a parent can send them an invitation from the "Manage Dependents" section of their profile. Their teen gets an email, creates their own TrashMob account, and is automatically linked to the parent's profile. Age verification and consent happens through PRIVO before the account can do anything sensitive.

**3. A teen registers themselves and asks a parent to consent.** If your 13-to-17-year-old finds TrashMob first, they can start the signup flow themselves — TrashMob then reaches out to the parent for consent via PRIVO before the account is fully activated.

### What protections are in place

Once a minor account is live, we apply a stack of safeguards behind the scenes:

- **Names are masked** on public views. A minor named Alex Martinez shows up as "Alex M." on event attendee lists, team member lists, and leaderboards.
- **Minors can't create or lead events.** Every event has at least one adult organiser, and the last remaining adult lead can't leave the event without transferring the role to another adult first.
- **Parents get notified** on every RSVP their minor makes and every time a new waiver is required. No RSVPs happen silently.
- **Minor accounts use dependent waivers** signed by the parent, not TrashMob's standard adult waiver.
- **We follow COPPA and applicable state laws** for the collection, use, and deletion of any information tied to a minor account.

You can read the full details in our [Privacy Policy](/privacypolicy) and the [Delete My Data](/deletemydata) page.

### Why this matters

We started this work because families kept asking us the same question: *"My daughter wants to come to your cleanup — can she sign up too?"* Our old answer was "you can bring her as your guest, but she can't have her own account until she's 18." That answer didn't reflect how families actually organise. Teenagers plan their own weekends, coordinate with friends, and want to participate on their own terms. And the environmental problem we're all trying to solve isn't going anywhere — the next generation of stewards should be able to show up.

Building this the right way took time. COPPA-compliant identity verification and parental consent isn't something you spin up over a weekend — that's why we partnered with [PRIVO](https://privo.com) rather than building it from scratch. TrashMob is the first organisation to integrate Microsoft Entra External ID with PRIVO's consent workflow, and we're proud to say the whole flow is now shipping in production.

### How to try it

If you're a parent:

1. Sign in to your TrashMob account.
2. Go to your profile and open **Manage Dependents**.
3. Click **Add a Dependent** or **Invite a Teen (13+)** depending on what fits.
4. Follow the on-screen prompts. If you're going through the identity verification and consent step, PRIVO will guide you through it in a separate window.

If you're a young person who wants to volunteer:

1. Ask a parent or legal guardian first — TrashMob needs their consent before your account can be active.
2. Have them start the process from their own profile, or go through the invited-teen flow if they sent you an invitation.
3. Bring a parent, a guardian, or another authorised adult to your first event. Every TrashMob cleanup has an adult organiser on-site.

### What's next

A few pieces are still in flight and worth knowing about:

- **Mobile app support** for the full parent-invite flow is landing in a coming release; today it works best on the web.
- **Legal review of the minor waiver text** for individual states is ongoing. The waiver you'll sign today is our best current version; we'll keep it up to date as legal reviews conclude.
- **We're publishing a detailed integration case study** with PRIVO to help other nonprofit and civic-tech organisations follow the same path. If you'd like a copy when it's out, [contact us](/contactus).

Thanks to the volunteer developers, community managers, and — especially — to the families who told us what they needed. We're glad to finally have this out the door.

— The TrashMob team

---

## Social Posts

**Twitter / X:**
> Big update: teens 13+ can now join TrashMob with their own accounts (with parental consent). Full COPPA-compliant flow, live in production. Thanks to @privo_com for making it possible. Details 👉 [link]

**LinkedIn:**
> Since April we've been working with PRIVO to make TrashMob one of the safest places for young people to volunteer online. That work is live: teens 13-17 can now have their own TrashMob account with parental consent, name masking, dependent waivers signed by parents, and a full COPPA-compliant identity verification flow. We're the first organisation to integrate PRIVO with Microsoft Entra External ID, and we'll be publishing the technical case study for other civic-tech nonprofits soon.

**Facebook:**
> Families have been asking: "Can my kids sign up for a TrashMob account too?" As of this week, the answer is yes. Parents can invite their teens 13+ to have their own account, or bring younger kids to any cleanup as dependents. All the safeguards you'd expect — parental consent through PRIVO, name masking, adult organisers required at every event, and parent notifications whenever a minor RSVPs. Full details on our blog.
