export interface GuideEntry {
    title: string;
    content: string;
    adminPath?: string;
    keywords?: string[];
}

export interface GuideSection {
    id: string;
    title: string;
    description: string;
    entries: GuideEntry[];
}

export const adminGuideData: GuideSection[] = [
    {
        id: 'data-management',
        title: 'Data Management',
        description: 'Managing users, events, partners, and other core platform data.',
        entries: [
            {
                title: 'Users',
                adminPath: '/siteadmin/users',
                content: `View and manage all registered TrashMob users.

**Common tasks:**
- **Search users** by username, email, city, region, or country
- **View user details** — click a row to see their full profile, events attended, and account metadata
- **Delete a user** — use the row action menu (irreversible, use only for spam or abuse)

**When to use:** Investigating user reports, verifying accounts, handling abuse, or answering support emails about account issues.`,
                keywords: ['accounts', 'members', 'volunteers', 'profiles'],
            },
            {
                title: 'Events',
                adminPath: '/siteadmin/events',
                content: `View all cleanup events across the platform.

**Common tasks:**
- **Search events** by name, location, or date
- **View event details** by clicking the event row
- **Monitor activity** to spot spam or duplicate events

**When to use:** When users report issues with events, when monitoring platform activity, or when investigating flagged content.`,
                keywords: ['cleanups', 'activities'],
            },
            {
                title: 'Partners',
                adminPath: '/siteadmin/partners',
                content: `View all partner organizations (Government, Business, Community types).

**Common tasks:**
- **Browse partners** — see all registered partner organizations
- **View partner details** — click to see locations, services, admins, and documents
- **Monitor partner status** — active vs. inactive partners

**When to use:** Reviewing partner configurations, checking service availability, or supporting partner admins with setup questions.`,
                keywords: ['organizations', 'sponsors', 'communities'],
            },
            {
                title: 'Documents',
                adminPath: '/siteadmin/documents',
                content: `Manage platform-wide documents (agreements, policies, insurance certificates).

**Common tasks:**
- **View all documents** uploaded by partners
- **Check document status** — review expiration dates and types

**When to use:** Auditing partner compliance, checking for expired documents, or preparing for reporting.`,
                keywords: ['files', 'agreements', 'contracts', 'insurance'],
            },
            {
                title: 'Teams',
                adminPath: '/siteadmin/teams',
                content: `View all volunteer teams on the platform.

**Common tasks:**
- **Browse teams** — see public and private teams
- **View team details** — members, leads, events, adoptions
- **Monitor team activity** — check for inactive or problematic teams

**When to use:** Supporting team leads, investigating reported teams, or reviewing team-level activity.`,
                keywords: ['groups', 'crews', 'squads'],
            },
            {
                title: 'Litter Reports',
                adminPath: '/siteadmin/litter-reports',
                content: `View all litter reports submitted by users.

**Common tasks:**
- **Browse reports** — see all submitted litter locations
- **Review report details** — location, photos, status
- **Monitor for spam** — check for duplicate or inappropriate reports

**When to use:** Reviewing reported litter hotspots, connecting reports to cleanup events, or investigating user complaints about litter report quality.`,
                keywords: ['reports', 'litter', 'hotspots', 'submissions'],
            },
            {
                title: 'Partner Requests',
                adminPath: '/siteadmin/partner-requests',
                content: `Review and process applications from organizations wanting to become TrashMob partners.

**Common tasks:**
- **Review pending requests** — see new partner applications
- **Approve or reject** — click a request to view details and take action
- **Verify organization info** — check the organization's website, type, and stated purpose before approving

**Approval workflow:**
1. Organization submits a partner request form
2. Request appears here as "Pending"
3. Admin reviews the details and verifies legitimacy
4. Admin approves (creates the partner) or rejects (with reason)

**When to use:** Regularly — check this page at least weekly to avoid leaving organizations waiting.`,
                keywords: ['applications', 'onboarding', 'approval'],
            },
            {
                title: 'Job Opportunities',
                adminPath: '/siteadmin/job-opportunities',
                content: `Create and manage job postings displayed on the platform.

**Common tasks:**
- **Create a job posting** — click "Create" to add a new opportunity with title, description (markdown supported), location, and application link
- **Edit existing postings** — update details or mark as filled
- **Remove old postings** — delete expired or filled positions

**When to use:** When the organization has open positions or volunteer leadership roles to advertise.`,
                keywords: ['jobs', 'hiring', 'positions', 'careers'],
            },
            {
                title: 'Prospects',
                adminPath: '/siteadmin/prospects',
                content: `Manage the sales pipeline for potential partner organizations.

**Common tasks:**
- **View all prospects** — see organizations in various pipeline stages
- **Update prospect status** — move through stages (Lead, Contacted, Qualified, Proposal, Negotiation, Won, Lost)
- **Add notes** — track interactions and follow-ups
- **Convert to partner** — when a prospect commits, convert them to a full partner

**Related pages:**
- **Discovery** — find new prospects from external data sources
- **Import CSV** — bulk import prospect lists from spreadsheets
- **Pipeline Analytics** — view conversion rates, stage distribution, and pipeline velocity

**When to use:** For business development — tracking outreach to cities, counties, and organizations that could become TrashMob community partners.`,
                keywords: ['pipeline', 'sales', 'leads', 'CRM', 'business development'],
            },
        ],
    },
    {
        id: 'fundraising',
        title: 'Fundraising',
        description: 'Managing contacts, donations, pledges, and fundraising appeals.',
        entries: [
            {
                title: 'Contacts',
                adminPath: '/siteadmin/contacts',
                content: `Manage the fundraising contact database (separate from platform user accounts).

**Common tasks:**
- **Add contacts** — create records for donors, prospects, volunteers, and board members
- **View contact details** — click a contact to see their full profile, donation history, and communication log
- **Filter by type** — use tabs to filter by contact type (Individual, Organization, Foundation, etc.)
- **Tag contacts** — apply tags for segmentation and targeting
- **Send communications** — send thank-you emails, receipts, or appeals from the contact detail page

**When to use:** Managing donor relationships, preparing for fundraising campaigns, or tracking engagement with supporters.`,
                keywords: ['donors', 'supporters', 'CRM', 'database'],
            },
            {
                title: 'Contact Tags',
                adminPath: '/siteadmin/contact-tags',
                content: `Create and manage tags for categorizing fundraising contacts.

**Common tasks:**
- **Create tags** — add new categories (e.g., "Major Donor", "Board Member", "Corporate Sponsor")
- **View tag usage** — see how many contacts have each tag
- **Delete unused tags** — clean up tags no longer in use

**When to use:** Setting up segmentation for fundraising campaigns, cleaning up the contact database, or preparing targeted communications.`,
                keywords: ['categories', 'labels', 'segmentation'],
            },
            {
                title: 'Donations',
                adminPath: '/siteadmin/donations',
                content: `Record and track monetary donations.

**Common tasks:**
- **Record a donation** — enter amount, date, donor (linked to contact), payment method, and notes
- **Edit donation records** — update details or correct errors
- **Track recurring donations** — mark donations as recurring with frequency
- **Record matching gifts** — link corporate matching contributions
- **Send receipts** — generate and email tax-deductible donation receipts from the contact detail page

**When to use:** After receiving donations, during end-of-year reporting, or when preparing financial summaries for the board.`,
                keywords: ['gifts', 'contributions', 'payments', 'revenue'],
            },
            {
                title: 'Pledges',
                adminPath: '/siteadmin/pledges',
                content: `Track donation pledges (commitments to donate in the future).

**Common tasks:**
- **Record a pledge** — enter the committed amount, donor, expected date, and pledge type
- **Track fulfillment** — monitor whether pledges are converted to actual donations
- **Edit pledges** — update amounts or dates as plans change

**When to use:** During fundraising campaigns when donors commit to future giving, or when tracking outstanding commitments.`,
                keywords: ['commitments', 'promises', 'future donations'],
            },
            {
                title: 'Appeals',
                adminPath: '/siteadmin/appeals',
                content: `Create and manage fundraising appeal campaigns.

**Common tasks:**
- **Create an appeal** — set up a fundraising campaign with name, goal, date range, and target audience
- **Send appeal emails** — compose and send fundraising asks to selected contacts
- **Track results** — monitor donations received against the appeal goal

**When to use:** Planning and executing fundraising campaigns, year-end giving drives, or grant-related outreach.`,
                keywords: ['campaigns', 'asks', 'fundraising drives'],
            },
            {
                title: 'Grants',
                adminPath: '/siteadmin/grants',
                content: `Track grant applications and awards.

**Common tasks:**
- **Record a grant** — enter the grantor, amount, application date, status, and notes
- **Track grant status** — monitor applications through stages (Applied, Under Review, Awarded, Declined)
- **Manage awarded grants** — track reporting requirements and deadlines
- **View grant history** — see all past and current grant activity

**When to use:** When applying for grants, tracking grant deadlines, or preparing financial reports for the board.`,
                keywords: ['grants', 'funding', 'foundations', 'applications', 'awards'],
            },
        ],
    },
    {
        id: 'communications',
        title: 'Communications',
        description: 'Sending notifications, managing email templates, newsletters, and bulk invites.',
        entries: [
            {
                title: 'Send Notifications',
                adminPath: '/siteadmin/send-notifications',
                content: `Send push notifications to platform users.

**Common tasks:**
- **Compose a notification** — write a title and message body
- **Select recipients** — target all users, specific regions, or event attendees
- **Send immediately** — notifications are delivered in real-time

**Important:** Notifications go to all targeted users. Double-check your message and recipient selection before sending. There is no "unsend" option.

**When to use:** Announcing platform-wide updates, emergency weather cancellations, or major milestones.`,
                keywords: ['push', 'alerts', 'announcements', 'broadcast'],
            },
            {
                title: 'Email Templates',
                adminPath: '/siteadmin/email-templates',
                content: `View and manage system email templates used for automated communications.

**Common tasks:**
- **Browse templates** — see all email templates (event reminders, welcome emails, partner notifications, etc.)
- **View template content** — click to see the HTML content and variables used
- **Understand the template system** — templates use placeholder variables that get replaced with actual data at send time

**Note:** Template modifications affect all future emails of that type. Test changes carefully.

**When to use:** When investigating email delivery issues, reviewing what automated emails look like, or understanding the communication flow.`,
                keywords: ['email', 'automated', 'messages', 'HTML'],
            },
            {
                title: 'Newsletters',
                adminPath: '/siteadmin/newsletters',
                content: `Create, schedule, and send newsletters to subscribers.

**Common tasks:**
- **Create a newsletter** — compose with the rich text editor, add images and links
- **Schedule delivery** — set a future send date/time
- **Send a test** — preview the newsletter in your inbox before sending to subscribers
- **View history** — see sent newsletters, drafts, and scheduled sends
- **Filter by status** — use tabs to view Draft, Scheduled, or Sent newsletters

**Workflow:**
1. Create a new newsletter (starts as Draft)
2. Compose the content using the editor
3. Send a test email to yourself to verify formatting
4. Schedule or send immediately

**When to use:** Regular community updates, event announcements, impact reports, or seasonal campaigns.`,
                keywords: ['email blasts', 'subscriber updates', 'marketing'],
            },
            {
                title: 'Bulk Invites',
                adminPath: '/siteadmin/invites',
                content: `View and manage batch invitation campaigns.

**Common tasks:**
- **Browse invitation batches** — see all bulk invite campaigns sent
- **View batch details** — click to see individual invitations and their delivery status
- **Monitor delivery** — check which invitations were delivered, opened, or bounced

**Note:** Bulk invites are typically sent from community or team pages. This admin view provides oversight of all invitation activity across the platform.

**When to use:** Monitoring invitation campaigns, troubleshooting delivery issues, or auditing invitation volume.`,
                keywords: ['invitations', 'onboarding', 'recruitment'],
            },
        ],
    },
    {
        id: 'moderation',
        title: 'Moderation',
        description: 'Reviewing user feedback, moderating photos, managing content, and administering waivers.',
        entries: [
            {
                title: 'User Feedback',
                adminPath: '/siteadmin/feedback',
                content: `Review feedback and support requests submitted by users.

**Common tasks:**
- **Browse feedback** — see all user-submitted feedback items
- **Review and respond** — read user comments and take appropriate action
- **Track resolution** — monitor which items have been addressed

**When to use:** Regularly — check at least weekly to stay on top of user concerns and bug reports.`,
                keywords: ['support', 'bugs', 'issues', 'complaints', 'suggestions'],
            },
            {
                title: 'Photo Moderation',
                adminPath: '/siteadmin/photo-moderation',
                content: `Review and moderate user-uploaded photos (event photos, litter report images, team photos, partner photos).

**Common tasks:**
- **Review pending photos** — the Pending tab shows photos awaiting moderation
- **Review flagged photos** — the Flagged tab shows photos reported by users
- **Approve or reject** — approve appropriate photos, reject those that violate guidelines
- **Dismiss flags** — if a flagged photo is acceptable, dismiss the flag
- **View moderation history** — the Moderated tab shows previously reviewed photos

**Moderation criteria:**
- Reject photos containing inappropriate content, personal information, or content unrelated to cleanup activities
- Approve photos showing cleanup activities, litter, before/after shots, and group photos
- When in doubt, err on the side of caution — reject and contact the uploader

**When to use:** Regularly — check the Pending and Flagged tabs daily to keep the moderation queue clear.`,
                keywords: ['images', 'review', 'approval', 'content moderation', 'flagged'],
            },
            {
                title: 'Manage Content',
                adminPath: '/siteadmin/content',
                content: `Manage platform-wide content settings and CMS integration.

**Common tasks:**
- **Review content settings** — check platform content configuration
- **Manage CMS content** — oversee content published through the Strapi CMS

**When to use:** When updating platform-wide content or managing the CMS integration.`,
                keywords: ['CMS', 'Strapi', 'pages', 'website content'],
            },
            {
                title: 'Waivers',
                adminPath: '/siteadmin/waivers',
                content: `Create and manage liability waivers that participants sign before attending events.

**Common tasks:**
- **Create a waiver** — write waiver text (markdown supported), set effective dates, and specify the waiver type (Individual Adult, Minor with Parent/Guardian)
- **Edit waivers** — update waiver text or dates for future versions
- **View compliance** — check the compliance dashboard to see which active waivers need attention
- **Manage versions** — waivers are versioned. When you create a new version with a new effective date, users will be prompted to sign the updated waiver

**Important considerations:**
- Waiver changes should be reviewed by legal counsel before publishing
- Existing signatures remain valid — new versions only apply to future event attendance
- Community-specific waivers can be created for partner communities with custom requirements

**When to use:** When legal requirements change, when onboarding a new community partner with custom waiver needs, or during annual waiver review.`,
                keywords: ['liability', 'legal', 'signatures', 'consent', 'compliance'],
            },
        ],
    },
    {
        id: 'community-admin',
        title: 'Community Admin',
        description: 'Managing community partner dashboards, branding, adoptable areas, and adoption programs.',
        entries: [
            {
                title: 'Community Dashboard',
                content: `The community dashboard is the home page for community partner admins. It shows overview statistics and quick links to management tools.

**Key metrics displayed:**
- Total events in the community
- Total participants
- Total bags collected
- Total weight collected

**Navigation:** From here, community admins can access all management pages via the sidebar navigation.

**When to use:** First stop for community admins to check overall program health.`,
                keywords: ['overview', 'stats', 'metrics', 'home'],
            },
            {
                title: 'Community Content & Branding',
                content: `Edit the community's public-facing page content and visual branding.

**Editable fields:**
- **Tagline** — short description shown on the community card
- **About text** — detailed description with markdown support
- **Logo and banner image** — uploaded with crop tool
- **Brand colors** — primary and secondary colors for the community page
- **Contact info** — email, phone, address, website

**When to use:** During initial community setup, seasonal campaigns, or when the community rebrands.`,
                keywords: ['branding', 'logo', 'colors', 'public page', 'customization'],
            },
            {
                title: 'Adoptable Areas',
                content: `Create and manage the areas available for volunteer or sponsored adoption.

**Three ways to create areas:**

1. **AI Generation** — select a category (parks, schools, streets, trails, highways) and TrashMob discovers areas from OpenStreetMap data within the community boundary. Areas are staged for review before going live.

2. **Bulk Import** — upload GeoJSON, KML, KMZ, or Shapefile to import areas from existing GIS systems. The import wizard auto-maps fields and validates data.

3. **Manual Creation** — draw polygons or lines on the interactive map editor, or use the AI suggest tool with a natural language description.

**Area management:**
- Edit area names, types, and boundaries
- Set cleanup frequency requirements
- Mark areas as available, unavailable, or adopted
- Review staged areas from AI generation before approving

**When to use:** During initial program setup, when expanding the adoption program, or when adjusting area boundaries.`,
                keywords: ['areas', 'zones', 'segments', 'map', 'GIS', 'OpenStreetMap', 'import'],
            },
            {
                title: 'Adoptions & Compliance',
                content: `View and manage volunteer team adoptions of areas.

**Common tasks:**
- **Review adoption applications** — approve or reject teams requesting to adopt areas
- **Monitor compliance** — check which teams are meeting their cleanup frequency
- **Revoke adoptions** — remove an adoption if a team is consistently delinquent
- **View compliance dashboard** — see at-a-glance stats for on-schedule vs. overdue adoptions

**Compliance tracking:**
- The system automatically tracks last event date and cleanup frequency per adoption
- Teams approaching or past their deadline are flagged as "at risk" or "delinquent"
- Overdue adoptions are highlighted in red

**When to use:** Regularly — check the compliance dashboard weekly to catch delinquent adoptions early.`,
                keywords: ['adopt-a-street', 'adopt-a-highway', 'adopt-a-park', 'compliance', 'delinquent'],
            },
            {
                title: 'Sponsors & Sponsored Adoptions',
                content: `Manage businesses that sponsor area adoptions and the professional cleanup companies they hire.

**Sponsors** are businesses paying for adoption (visibility on public map, access to compliance reports).

**Professional Companies** are cleanup service providers contracted by sponsors to maintain adopted areas.

**Sponsored Adoptions** link an area to a sponsor and a professional company, with defined cleanup frequency and start/end dates.

**Key distinction:** Professional cleanup data is tracked completely separately from volunteer metrics. Paid work never appears on volunteer leaderboards.

**When to use:** When onboarding a new business sponsor, setting up professional cleanup contracts, or reviewing sponsored adoption compliance.`,
                keywords: ['business', 'corporate', 'professional', 'paid cleanup', 'contracts'],
            },
            {
                title: 'Community Invites',
                content: `Send batch email invitations to recruit volunteers into the community.

**Common tasks:**
- **Create an invitation batch** — enter email addresses (one per line or comma-separated) and compose the invitation message
- **View invitation history** — see all batches sent, including delivery status
- **Monitor onboarding** — track which invitees have created accounts and joined the community

**When to use:** Onboarding a neighborhood association, corporate partner employee group, or existing volunteer mailing list.`,
                keywords: ['recruitment', 'onboarding', 'email invites', 'bulk email'],
            },
        ],
    },
    {
        id: 'partner-admin',
        title: 'Partner Admin',
        description: 'Managing partner organizations, service locations, and administrative access.',
        entries: [
            {
                title: 'Service Requests',
                content: `The partner dashboard home page shows pending event service requests from partner locations.

**Workflow:**
1. An event organizer requests a service (e.g., trash hauling, portable toilets) from a partner location
2. The request appears on the partner dashboard as "Pending"
3. The partner admin reviews and approves or declines
4. For auto-approved services, requests are accepted immediately

**When to use:** Partners should check this page regularly to respond to service requests promptly.`,
                keywords: ['hauling', 'services', 'requests', 'approval'],
            },
            {
                title: 'Edit Partner Details',
                content: `Edit the partner organization's core information.

**Editable fields:**
- **Name** — organization display name
- **Website** — organization URL
- **Partner type** — Government, Business, or Community
- **Status** — Active or Inactive
- **Public notes** — visible to all users
- **Private notes** — visible only to partner admins and site admins

**When to use:** During initial partner setup or when organization details change.`,
                keywords: ['profile', 'organization info', 'settings'],
            },
            {
                title: 'Partner Locations',
                content: `Manage the physical locations where a partner provides services.

**Common tasks:**
- **Add a location** — name, address, and map coordinates
- **View on map** — toggle between table and map views
- **Edit location details** — update address or contact information
- **Delete locations** — remove locations no longer in use

**When to use:** When a partner adds a new service location, closes a location, or needs to update address information.`,
                keywords: ['addresses', 'facilities', 'sites', 'map'],
            },
            {
                title: 'Partner Services',
                content: `Configure which services each partner location offers to event organizers.

**Common tasks:**
- **Enable/disable services** — toggle specific service types (hauling, disposal, portable toilets, etc.) per location
- **Set auto-approval** — mark services as auto-approved (no manual review needed) or requiring advance notice
- **Configure service details** — set capacity, availability, and any restrictions

**When to use:** During initial partner setup or when a location's service offerings change.`,
                keywords: ['hauling', 'disposal', 'toilets', 'offerings'],
            },
            {
                title: 'Partner Contacts',
                content: `Manage contact persons for each partner location.

**Common tasks:**
- **Add a contact** — name, email, phone, and notes
- **Assign to locations** — link contacts to specific partner locations
- **Update contact info** — keep phone numbers and emails current

**When to use:** When a location has a new point of contact, or when contact information changes.`,
                keywords: ['people', 'phone', 'email', 'point of contact'],
            },
            {
                title: 'Partner Admins',
                content: `Manage who has administrative access to the partner dashboard.

**Common tasks:**
- **Invite admins** — send an invitation email to grant partner admin access
- **View pending invitations** — see invitations that haven't been accepted
- **Resend invitations** — resend to users who didn't receive or lost the original
- **Remove admins** — revoke partner admin access

**Important:** Partner admins can manage locations, services, contacts, and respond to service requests. Be selective about who gets this access.

**When to use:** When onboarding a new partner admin, when staff changes occur, or when cleaning up unused admin accounts.`,
                keywords: ['access', 'permissions', 'invitations', 'team'],
            },
            {
                title: 'Partner Documents',
                content: `Upload and manage documents for the partner organization.

**Document types:**
- Agreements and contracts
- Insurance certificates
- Reports and compliance documents
- Other supporting files

**Common tasks:**
- **Upload a document** — select type, add description, set expiration date
- **View documents** — browse all uploaded files
- **Track expiration** — monitor documents approaching their expiration date
- **Delete old documents** — remove expired or superseded files

**When to use:** During partner onboarding, annual compliance reviews, or contract renewals.`,
                keywords: ['files', 'uploads', 'agreements', 'insurance', 'certificates'],
            },
            {
                title: 'Partner Social Media',
                content: `Manage social media account links for the partner organization.

**Common tasks:**
- **Add social accounts** — link Facebook, Twitter/X, Instagram, LinkedIn, and other platforms
- **Update URLs** — keep social media links current
- **Enable broadcasting** — configure which platforms receive event announcements

**When to use:** During partner setup or when the partner updates their social media presence.`,
                keywords: ['social', 'Facebook', 'Twitter', 'Instagram', 'LinkedIn', 'broadcasting'],
            },
        ],
    },
];
