import * as React from 'react';
import { Link } from 'react-router';
import events from '@/components/assets/faq/Event.svg';
import gloves from '@/components/assets/gloves.svg';
import teams from '@/components/assets/card/twofigure.svg';
import communities from '@/components/assets/home/Person.svg';
import ai from '@/components/assets/faq/volunteer.svg';
import { HeroSection } from '@/components/Customization/HeroSection';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';

const tabContents = [
    {
        category: 'Events',
        desc: 'How to create and manage cleanup events',
        icon: events,
        questions: [
            {
                question: 'Before You Start',
                answer: `
                    <p>Before creating an event, complete these one-time setup steps:</p>
                    <ol class="list-decimal pl-8">
                        <li><strong>Create an account</strong> - Click "Sign In" in the header to register or log in</li>
                        <li><strong>Set your location</strong> - Click your profile icon and select "Location Preference" to set your home area and travel radius</li>
                        <li><strong>Sign the waiver</strong> - You'll be prompted to sign a liability waiver when you create your first event</li>
                    </ol>
                    <p>Your location preference determines which events you'll be notified about, so set it to a central point in your area.</p>
                `,
            },
            {
                question: 'Creating an Event',
                answer: `
                    <p>Ready to organize a cleanup? Here's how:</p>
                    <ol class="list-decimal pl-8">
                        <li>Click <strong>"Create Event"</strong> from the home page or navigation menu</li>
                        <li>Fill in the event details:
                            <ul class="list-disc pl-8 mt-2">
                                <li><strong>Name</strong> - Give your event a clear, descriptive title</li>
                                <li><strong>Date & Time</strong> - When will you meet?</li>
                                <li><strong>Duration</strong> - How long do you expect the cleanup to take?</li>
                                <li><strong>Description</strong> - What will you be cleaning? Include parking info, what to wear, and what to bring</li>
                            </ul>
                        </li>
                        <li>Set the <strong>meeting location</strong> using the map - zoom in and place the pin precisely so volunteers can find you</li>
                        <li>Click <strong>Save</strong> to publish your event</li>
                    </ol>
                `,
            },
            {
                question: 'Requesting Partner Support',
                answer: `
                    <p>Partners can provide hauling, disposal, supplies, or starter kits for your event.</p>
                    <p>After creating your event:</p>
                    <ol class="list-decimal pl-8">
                        <li>Check if partners are available in your area</li>
                        <li>Select the services you need (hauling, disposal, supplies, starter kits)</li>
                        <li>Submit your request - the partner will be notified</li>
                    </ol>
                    <p>No partners in your area? Click <strong>"Invite a Partner"</strong> to help grow the network in your community.</p>
                `,
            },
            {
                question: 'After the Event — Event Summary',
                answer: `
                    <p>After your cleanup, record what you accomplished on the <strong>Event Summary</strong> page:</p>
                    <ol class="list-decimal pl-8">
                        <li>Go to <strong>My Dashboard</strong> and find your completed event</li>
                        <li>Click <strong>"Add Summary"</strong> (or "Edit Summary" if one exists)</li>
                    </ol>
                    <h6>Event Summary Information</h6>
                    <p>Fill in the details of your cleanup:</p>
                    <ul class="list-disc pl-8">
                        <li><strong>Actual Number of Attendees</strong> - How many people showed up (pre-filled from event max)</li>
                        <li><strong>Duration in Minutes</strong> - How long the cleanup actually took</li>
                        <li><strong>Number of Bags</strong> - Total bags of litter collected</li>
                        <li><strong>Number of Buckets</strong> - Total buckets used</li>
                        <li><strong>Picked Weight</strong> - Total weight of litter collected</li>
                        <li><strong>Weight Unit</strong> - Pounds or Kilograms (defaults to your profile preference)</li>
                        <li><strong>Notes</strong> - Anything noteworthy about the cleanup</li>
                    </ul>
                    <p>Only the event lead can edit these fields. Click <strong>Save</strong> to record the summary.</p>
                    <h6>Pickup Locations</h6>
                    <p>If you requested hauling support from a partner, tell them exactly where to find the collected litter:</p>
                    <ol class="list-decimal pl-8">
                        <li>Click <strong>"Add Pickup Location"</strong> to open the location form</li>
                        <li>Set a <strong>name</strong> for the pile (e.g. "Corner of Main & 5th")</li>
                        <li>Place the <strong>pin on the map</strong> as precisely as possible so the hauling crew can find it</li>
                        <li>Add <strong>notes</strong> with any extra detail (e.g. "Behind the dumpster by the parking lot")</li>
                        <li>You can add as many locations as needed — use <strong>Edit</strong> or <strong>Remove</strong> from the menu on each row</li>
                        <li>When all locations are entered, click <strong>"Submit"</strong> to notify the hauling partner</li>
                    </ol>
                    <p>Each location shows a status: <strong>Not submitted</strong>, <strong>Submitted</strong>, or <strong>Picked Up</strong>.</p>
                    <h6>Litter Reports Cleaned</h6>
                    <p>If your cleanup addressed any existing <strong>Litter Reports</strong>, associate them with this event so they're marked as cleaned. This links your cleanup effort to community-reported litter.</p>
                    <h6>Hauling Partner Contacts</h6>
                    <p>If a hauling partner is assigned to your event, their contact details (name, email, phone) are displayed here for easy reference.</p>
                    <p>Recording your impact helps TrashMob track community progress and recognize top volunteers!</p>
                `,
            },
        ],
    },
    {
        category: 'Partnerships',
        desc: 'How to become a TrashMob partner',
        icon: gloves,
        questions: [
            {
                question: 'What is a Partner?',
                answer: `
                    <p>Partners help volunteers by providing services that individuals can't easily do themselves:</p>
                    <ul class="list-disc pl-8">
                        <li><strong>Hauling</strong> - Picking up collected litter from cleanup sites</li>
                        <li><strong>Disposal</strong> - Providing dumpsters or drop-off locations</li>
                        <li><strong>Supplies</strong> - Trash bags, gloves, and other cleanup materials</li>
                        <li><strong>Starter Kits</strong> - Complete kits with bucket, grabber, vest, bags, and gloves</li>
                    </ul>
                    <p>Partners can be local governments, businesses, nonprofits, or any organization that wants to support community cleanups.</p>
                `,
            },
            {
                question: 'Becoming a Partner',
                answer: `
                    <p>Here's how to apply:</p>
                    <ol class="list-decimal pl-8">
                        <li><strong>Sign in</strong> to your TrashMob account (or create one)</li>
                        <li>Go to the <a href='https://www.trashmob.eco/partnerships'>Partnerships page</a></li>
                        <li>Click <strong>"Become a Partner"</strong></li>
                        <li>Complete the application:
                            <ul class="list-disc pl-8 mt-2">
                                <li>Organization name</li>
                                <li>Type (government or business)</li>
                                <li>Contact email, website, and phone</li>
                                <li>Main office location (set via map)</li>
                            </ul>
                        </li>
                        <li>Submit and wait for approval</li>
                    </ol>
                    <p>TrashMob staff will review your application and contact you if we have questions. Once approved, you'll receive an email with next steps.</p>
                `,
            },
            {
                question: 'Setting Up Your Partnership',
                answer: `
                    <p>After approval, activate your partnership from <a href='https://www.trashmob.eco/mydashboard'>My Dashboard</a>:</p>
                    <ol class="list-decimal pl-8">
                        <li>Find your partnership under "My Partnerships"</li>
                        <li>Click <strong>"Activate Partnership"</strong> to open the Partner Dashboard</li>
                    </ol>
                    <p>The Partner Dashboard has a sidebar with the following sections:</p>
                    <h6>Service Requests</h6>
                    <p>View and manage cleanup event requests that volunteers have submitted for your services. This is the default landing page for the dashboard.</p>
                    <h6>Edit Partner</h6>
                    <p>Update your organization's name, website, and other basic details.</p>
                    <h6>Locations</h6>
                    <p>Manage the physical locations where you provide services (dumpster sites, supply pickup points, offices). You can view locations in a table or on a map. See the <strong>"Adding Partner Locations"</strong> section below for details.</p>
                    <h6>Services</h6>
                    <p>Configure which services (hauling, disposal, supplies, starter kits) you offer at each location. Each service can be enabled or disabled per location, and you can add notes about availability or conditions. See <strong>"Configuring Services"</strong> below.</p>
                    <h6>Contacts</h6>
                    <p>Manage two types of contacts:</p>
                    <ul class="list-disc pl-8">
                        <li><strong>Organization contacts</strong> - General contacts displayed publicly for your partnership</li>
                        <li><strong>Location contacts</strong> - People who receive email notifications when volunteers request services at specific locations (tip: use a distribution list rather than an individual)</li>
                    </ul>
                    <h6>Admins</h6>
                    <p>Invite and manage people who can administer your partnership. You can send email invitations, track their status (pending or accepted), and resend or cancel invitations. We recommend having at least 2 admins for continuity.</p>
                    <h6>Documents</h6>
                    <p>Upload and manage partnership documents such as agreements, contracts, reports, insurance certificates, and more. You can either <strong>upload files directly</strong> (PDF, Word, Excel, PNG, JPEG — max 25 MB each) or <strong>link to external URLs</strong>. Documents can be categorized by type, given expiration dates, and downloaded or opened at any time. A storage meter shows how much of your allocated space is in use. See <strong>"Managing Documents"</strong> below for details.</p>
                    <h6>Social Media</h6>
                    <p>List your social media accounts so TrashMob can tag your organization when posting about events in your area.</p>
                `,
            },
            {
                question: 'Adding Partner Locations',
                answer: `
                    <p>Locations are where you provide services - a dumpster site, supply pickup point, or office to arrange hauling.</p>
                    <ol class="list-decimal pl-8">
                        <li>Go to <strong>Partner Locations</strong> in your dashboard</li>
                        <li>Click <strong>"Add Location"</strong></li>
                        <li>Set the location details:
                            <ul class="list-disc pl-8 mt-2">
                                <li><strong>Name</strong> - Identify this location</li>
                                <li><strong>Map location</strong> - Be precise, especially for drop-off points</li>
                                <li><strong>Public Notes</strong> - Hours, instructions (visible to event leads and attendees)</li>
                                <li><strong>Private Notes</strong> - Internal notes for TrashMob staff</li>
                            </ul>
                        </li>
                        <li>Add <strong>Location Contacts</strong> who will receive event notifications (tip: use a distribution list rather than individuals)</li>
                        <li>Set the location status to <strong>Active</strong></li>
                    </ol>
                `,
            },
            {
                question: 'Configuring Services',
                answer: `
                    <p>For each location, specify which services you offer:</p>
                    <ul class="list-disc pl-8">
                        <li><strong>Hauling</strong> - You'll be notified after events with pickup locations for collected litter</li>
                        <li><strong>Disposal</strong> - Provide a dumpster or drop-off point</li>
                        <li><strong>Supplies</strong> - Provide bags, gloves, and other materials</li>
                        <li><strong>Starter Kits</strong> - Complete volunteer kits (bucket, grabber, vest, bags, gloves)</li>
                    </ul>
                    <h6>Service Options</h6>
                    <ul class="list-disc pl-8">
                        <li><strong>Auto-approve requests</strong> - Automatically approve all requests without manual review</li>
                        <li><strong>Require advance notice</strong> - Get notified before events (important for hauling to ensure crews are available)</li>
                    </ul>
                    <p>Once configured, set your <strong>Partner Status to Active</strong>. Your locations will then appear to event leads in your area.</p>
                `,
            },
            {
                question: 'Managing Documents',
                answer: `
                    <p>The <strong>Documents</strong> tab lets you store important partnership files and links in one place.</p>
                    <h6>Adding a Document</h6>
                    <ol class="list-decimal pl-8">
                        <li>Click <strong>"Add Document"</strong></li>
                        <li>Give the document a <strong>name</strong></li>
                        <li>Select a <strong>type</strong>: Agreement, Contract, Report, Insurance, Certificate, or Other</li>
                        <li>Choose how to store it:
                            <ul class="list-disc pl-8 mt-2">
                                <li><strong>Upload a file</strong> - Drag and drop or browse to upload (PDF, Word, Excel, PNG, JPEG — max 25 MB)</li>
                                <li><strong>Link to a URL</strong> - Paste a link to an externally hosted document</li>
                            </ul>
                        </li>
                        <li>Optionally set an <strong>expiration date</strong> to track when the document needs renewal</li>
                        <li>Click <strong>Save</strong></li>
                    </ol>
                    <h6>Managing Existing Documents</h6>
                    <ul class="list-disc pl-8">
                        <li><strong>Download</strong> uploaded files or <strong>Open</strong> linked URLs directly from the table</li>
                        <li><strong>Edit</strong> a document's name, type, or expiration date</li>
                        <li><strong>Remove</strong> documents you no longer need</li>
                        <li><strong>Filter by type</strong> using the dropdown to find specific documents quickly</li>
                        <li><strong>Search</strong> by document name using the search bar</li>
                    </ul>
                    <h6>Storage</h6>
                    <p>A progress bar at the top of the Documents page shows how much of your allocated storage is in use. If you're running low on space, remove outdated files or use external URLs instead of uploads.</p>
                `,
            },
        ],
    },
    {
        category: 'Teams',
        desc: 'How to create and manage volunteer teams',
        icon: teams,
        questions: [
            {
                question: 'Creating a Team',
                answer: `
                    <p>Teams let you organize regular cleanups with a consistent group of volunteers.</p>
                    <ol class="list-decimal pl-8">
                        <li><strong>Sign in</strong> to your TrashMob account</li>
                        <li>Go to the <a href='https://www.trashmob.eco/teams'>Teams page</a></li>
                        <li>Click <strong>"Create Team"</strong></li>
                        <li>Fill in your team details:
                            <ul class="list-disc pl-8 mt-2">
                                <li><strong>Name</strong> - Choose a unique, descriptive name</li>
                                <li><strong>Description</strong> - Share your focus, meeting frequency, or cleanup areas</li>
                                <li><strong>Location</strong> - Set your primary location on the map</li>
                                <li><strong>Visibility</strong> - Public (open to join requests) or Private (invite-only)</li>
                                <li><strong>Require Approval</strong> - Auto-accept members or review each request</li>
                            </ul>
                        </li>
                        <li>Click <strong>"Create Team"</strong> - you'll be the team lead</li>
                    </ol>
                `,
            },
            {
                question: 'Managing Your Team',
                answer: `
                    <p>As a team lead, you can manage your team from the team's detail page.</p>
                    <h6>Editing Team Details</h6>
                    <p>Click <strong>"Edit Team"</strong> to update name, description, location, or visibility settings.</p>
                    <h6>Managing Members</h6>
                    <ul class="list-disc pl-8">
                        <li><strong>Review join requests</strong> - Approve or decline pending requests</li>
                        <li><strong>Promote to Co-Lead</strong> - Give members lead privileges to help manage</li>
                        <li><strong>Remove members</strong> - Remove inactive or inappropriate members</li>
                    </ul>
                    <h6>Tips for Success</h6>
                    <ul class="list-disc pl-8">
                        <li>Have at least 2 team leads for continuity</li>
                        <li>Keep your description current with meeting schedules</li>
                        <li>Respond to join requests promptly</li>
                        <li>Make your team public to attract more members</li>
                    </ul>
                `,
            },
            {
                question: 'Team Photos & Logo',
                answer: `
                    <p>Build your team's identity with photos and a logo.</p>
                    <h6>Uploading Photos</h6>
                    <ol class="list-decimal pl-8">
                        <li>Go to your team's page and click <strong>"Manage Team"</strong></li>
                        <li>Scroll to the <strong>"Team Photos"</strong> section</li>
                        <li>Click <strong>"Upload Photo"</strong> and select an image</li>
                    </ol>
                    <h6>Adding a Logo</h6>
                    <ol class="list-decimal pl-8">
                        <li>Go to your team's page and click <strong>"Manage Team"</strong></li>
                        <li>Find the <strong>"Team Logo"</strong> section</li>
                        <li>Click <strong>"Upload Logo"</strong> and select an image</li>
                    </ol>
                    <h6>Image Guidelines</h6>
                    <ul class="list-disc pl-8">
                        <li>Photos: Max 10MB, JPEG or PNG</li>
                        <li>Logo: Square recommended (200x200px+), max 5MB</li>
                        <li>Images are automatically resized for display</li>
                        <li>Only team leads can upload or delete images</li>
                    </ul>
                `,
            },
            {
                question: 'Joining a Team',
                answer: `
                    <p>Find and join teams in your area:</p>
                    <ol class="list-decimal pl-8">
                        <li>Go to the <a href='https://www.trashmob.eco/teams'>Teams page</a></li>
                        <li>Browse the list or use map view to find nearby teams</li>
                        <li>Click a team to view details (description, location, member count)</li>
                        <li>Click <strong>"Request to Join"</strong></li>
                        <li>Wait for approval - you'll get an email when accepted</li>
                    </ol>
                    <p>Once approved, the team appears in your dashboard under "My Teams".</p>
                    <p><strong>Note:</strong> Private teams don't appear in search results - you'll need a direct invitation from a team lead.</p>
                `,
            },
            {
                question: 'Adopting an Area',
                answer: `
                    <p>Some communities offer <strong>Adopt-a-Location</strong> programs where teams can take responsibility for keeping a specific area clean on a regular schedule.</p>
                    <h6>How to Adopt an Area</h6>
                    <ol class="list-decimal pl-8">
                        <li>Find a community that has adoptable areas available</li>
                        <li>Browse the available areas — each area shows its type (Park, Trail, Highway, etc.), cleanup frequency, and minimum events per year</li>
                        <li>Submit an <strong>adoption application</strong> for the area your team wants to maintain</li>
                        <li>Include any <strong>notes</strong> about why your team is a good fit</li>
                        <li>Wait for the community admin to <strong>review and approve</strong> your application</li>
                    </ol>
                    <h6>What to Expect After Approval</h6>
                    <ul class="list-disc pl-8">
                        <li>Your team commits to cleaning the adopted area at the required frequency (e.g. every 90 days)</li>
                        <li>You should meet the <strong>minimum number of events per year</strong> set by the community</li>
                        <li>Review any <strong>safety requirements</strong> specific to the area before organizing cleanups</li>
                        <li>The community tracks your team's <strong>compliance</strong> — keeping up with the schedule helps maintain your adoption</li>
                        <li>Some areas allow <strong>co-adoption</strong>, meaning multiple teams can share responsibility for the same location</li>
                    </ul>
                    <p>Adopting an area is a great way to make a lasting, visible impact in your community and build team identity around a specific place.</p>
                `,
            },
        ],
    },
    {
        category: 'Communities',
        desc: 'How to explore and engage with local communities',
        icon: communities,
        questions: [
            {
                question: 'Exploring Communities',
                answer: `
                    <p>Communities are local areas or organizations that have partnered with TrashMob to organize cleanups in their region. Communities can represent cities, counties, states, or regional organizations — any geographic area that wants to coordinate cleanup efforts.</p>
                    <h6>Finding Communities</h6>
                    <ol class="list-decimal pl-8">
                        <li>Go to the <a href='https://www.trashmob.eco/communities'>Communities page</a></li>
                        <li>Browse the list or search by name/location</li>
                        <li>Click a community card to view details</li>
                    </ol>
                    <h6>What's on a Community Page</h6>
                    <ul class="list-disc pl-8">
                        <li><strong>Branding</strong> - Banner, logo, and tagline</li>
                        <li><strong>About</strong> - Description and cleanup initiatives</li>
                        <li><strong>Contact</strong> - Email, phone, and address</li>
                        <li><strong>Events</strong> - Upcoming and past cleanups in the area</li>
                        <li><strong>Teams</strong> - Volunteer teams active in the community</li>
                        <li><strong>Impact</strong> - Bags collected, volunteers, hours, and more</li>
                    </ul>
                `,
            },
            {
                question: 'Getting Involved',
                answer: `
                    <p>Communities are geographic partnerships, not membership groups. Here's how to participate:</p>
                    <h6>Attend Events</h6>
                    <p>Find events on a community's page and register. Your participation automatically contributes to the community's impact statistics.</p>
                    <h6>Join Local Teams</h6>
                    <p>Check the Teams section on a community page to find groups for regular cleanup activities.</p>
                    <h6>Report Litter</h6>
                    <p>Spot litter? Submit a <a href='https://www.trashmob.eco/litterreports/create'>Litter Report</a> to flag areas that need attention.</p>
                    <h6>Contact the Community</h6>
                    <p>Questions or ideas? Use the contact info on the community page to reach administrators.</p>
                `,
            },
            {
                question: 'Community Administration',
                answer: `
                    <p>Community Admins manage their organization's presence on TrashMob.</p>
                    <h6>Accessing Your Dashboard</h6>
                    <ol class="list-decimal pl-8">
                        <li>Sign in to your TrashMob account</li>
                        <li>Go to <a href='https://www.trashmob.eco/mydashboard'>My Dashboard</a></li>
                        <li>Find your community under "My Communities"</li>
                        <li>Click <strong>"Manage"</strong> to open the admin dashboard</li>
                    </ol>
                    <h6>Admin Capabilities</h6>
                    <ul class="list-disc pl-8">
                        <li>Update branding (banner, logo, tagline)</li>
                        <li>Edit the About section and description</li>
                        <li>Manage contact information</li>
                        <li>View statistics and recent activity</li>
                        <li>See associated events and teams</li>
                    </ul>
                    <p><strong>Note:</strong> Admin access is granted to partner organization representatives. <a href='https://www.trashmob.eco/contactus'>Contact us</a> to discuss options.</p>
                `,
            },
            {
                question: 'Becoming a Community Partner',
                answer: `
                    <p>Represent a city, county, state, or regional organization? Here's how to create a community page:</p>
                    <h6>Who Can Partner?</h6>
                    <ul class="list-disc pl-8">
                        <li>Cities and municipalities</li>
                        <li>Counties and county departments</li>
                        <li>State agencies and programs</li>
                        <li>Regional environmental organizations</li>
                        <li>Neighborhood associations</li>
                        <li>Business improvement districts</li>
                        <li>Organizations focused on community cleanliness</li>
                    </ul>
                    <h6>Getting Started</h6>
                    <ol class="list-decimal pl-8">
                        <li><a href='https://www.trashmob.eco/contactus'>Contact us</a> to express interest</li>
                        <li>We'll discuss your goals and how we can help</li>
                        <li>Complete the partnership application</li>
                        <li>We'll set up your branded community page</li>
                        <li>You'll receive admin access to manage your page</li>
                    </ol>
                    <h6>Partnership Benefits</h6>
                    <ul class="list-disc pl-8">
                        <li>Branded page showcasing your area</li>
                        <li>Visibility into cleanup events and volunteer activity</li>
                        <li>Impact statistics to share with stakeholders</li>
                        <li>Connection with active volunteer teams</li>
                        <li>Tools to promote cleanup initiatives</li>
                    </ul>
                `,
            },
            {
                question: 'Adopt-a-Location Programs',
                answer: `
                    <p>Communities can create <strong>Adopt-a-Location</strong> programs that let teams take ownership of specific areas and commit to keeping them clean on a regular schedule.</p>
                    <h6>How It Works</h6>
                    <ol class="list-decimal pl-8">
                        <li>A <strong>community admin</strong> defines adoptable areas (parks, trails, highways, streets, waterways, or spots) on the community dashboard</li>
                        <li><strong>Teams</strong> browse available areas and submit an application to adopt one</li>
                        <li>The community admin <strong>reviews and approves or rejects</strong> the application</li>
                        <li>Once approved, the team commits to cleaning the area at the required frequency (e.g. every 90 days)</li>
                        <li>The community admin monitors <strong>compliance</strong> — tracking whether teams are meeting their cleanup commitments</li>
                    </ol>
                    <h6>Setting Up Adoptable Areas (Community Admins)</h6>
                    <p>From the Partner Dashboard, go to the <strong>Community</strong> section in the sidebar:</p>
                    <ol class="list-decimal pl-8">
                        <li><strong>Area Defaults</strong> - Set default requirements that apply to all new areas:
                            <ul class="list-disc pl-8 mt-2">
                                <li>Default cleanup frequency (e.g. every 90 days)</li>
                                <li>Minimum events per year (e.g. 4)</li>
                                <li>Default safety requirements</li>
                                <li>Whether to allow co-adoption (multiple teams sharing one area)</li>
                            </ul>
                        </li>
                        <li><strong>Adoptable Areas</strong> - Create and manage individual areas:
                            <ul class="list-disc pl-8 mt-2">
                                <li>Give the area a <strong>name</strong> and <strong>description</strong></li>
                                <li>Select the <strong>area type</strong>: Highway, Park, Trail, Waterway, Street, or Spot</li>
                                <li>Set cleanup frequency and minimum events per year (or click "Use Community Defaults")</li>
                                <li>Add <strong>safety requirements</strong> specific to the area</li>
                                <li>Draw the area <strong>boundaries on the map</strong> or paste GeoJSON</li>
                            </ul>
                        </li>
                        <li><strong>Bulk Import</strong> - Import many areas at once from GeoJSON or KML files using the import wizard. The wizard auto-detects field mappings and shows a map preview before importing.</li>
                    </ol>
                    <h6>Managing Adoptions</h6>
                    <p>The <strong>Adoptions</strong> tab in the community dashboard provides three views:</p>
                    <ul class="list-disc pl-8">
                        <li><strong>Pending Applications</strong> - Review team requests and approve or reject them (with a reason)</li>
                        <li><strong>Approved Adoptions</strong> - See all active adoptions with compliance status, event counts, and last event dates</li>
                        <li><strong>Delinquent Adoptions</strong> - Teams that have fallen behind on their cleanup schedule, with days overdue</li>
                    </ul>
                    <p>A compliance dashboard at the top shows key metrics: total adoptions, adoption rate, compliance rate, and counts of at-risk or delinquent adoptions. You can also <strong>export all adoption data as CSV</strong> for reporting.</p>
                    <h6>Sponsored Adoptions</h6>
                    <p>Communities can also set up <strong>sponsored adoptions</strong> where a sponsor funds a professional company to maintain an area. These are managed separately under the <strong>Sponsored Adoptions</strong> tab, with their own compliance tracking and cleanup logs.</p>
                `,
            },
        ],
    },
    {
        category: 'AI Integration',
        desc: 'Use AI assistants with TrashMob data',
        icon: ai,
        questions: [
            {
                question: 'What is MCP?',
                answer: `
                    <p>TrashMob provides an <strong>MCP (Model Context Protocol) server</strong> that lets AI assistants access TrashMob data through natural language.</p>
                    <p>MCP is an open protocol that allows AI tools like Claude, ChatGPT, and others to connect to external data sources. With the TrashMob MCP server, you can ask questions like:</p>
                    <ul class="list-disc pl-8">
                        <li>"Find cleanup events near Seattle this weekend"</li>
                        <li>"Who are the top volunteers this month?"</li>
                        <li>"What achievement badges can I earn?"</li>
                        <li>"Show me communities in Washington"</li>
                        <li>"What are the route stats for this event?"</li>
                    </ul>
                    <p>The server provides <strong>read-only access to public data</strong> only — no private information is ever exposed.</p>
                `,
            },
            {
                question: 'Available Tools',
                answer: `
                    <p>The MCP server exposes 9 tools that AI assistants can use:</p>
                    <ul class="list-disc pl-8">
                        <li><strong>Search Events</strong> - Find cleanup events by location, date, and status</li>
                        <li><strong>Get Stats</strong> - View platform-wide impact statistics</li>
                        <li><strong>Search Teams</strong> - Find volunteer teams by location or name</li>
                        <li><strong>Search Communities</strong> - Discover community pages near you</li>
                        <li><strong>Search Litter Reports</strong> - Find litter reports by location and status</li>
                        <li><strong>Search Partner Locations</strong> - Find partners offering hauling, disposal, or supplies</li>
                        <li><strong>Get Leaderboard</strong> - View volunteer and team rankings</li>
                        <li><strong>Get Achievement Types</strong> - See available badges and milestones</li>
                        <li><strong>Get Event Route Stats</strong> - View route tracking data for events</li>
                    </ul>
                `,
            },
            {
                question: 'MCP Server URLs',
                answer: `
                    <p>Use the following URLs to connect to the TrashMob MCP server:</p>
                    <ul class="list-disc pl-8">
                        <li><strong>Production:</strong> <code>https://mcp.trashmob.eco</code></li>
                        <li><strong>Development:</strong> <code>https://dev-mcp.trashmob.eco</code></li>
                    </ul>
                    <p>The server supports two transport methods:</p>
                    <ul class="list-disc pl-8">
                        <li><strong>Streamable HTTP</strong> — <code>/messages</code> endpoint (recommended)</li>
                        <li><strong>Server-Sent Events</strong> — <code>/sse</code> endpoint</li>
                    </ul>
                    <p>Most AI clients will auto-detect the correct transport when given the base URL.</p>
                `,
            },
            {
                question: 'Connecting from Claude Desktop',
                answer: `
                    <p>To use TrashMob with Claude Desktop:</p>
                    <ol class="list-decimal pl-8">
                        <li>Open Claude Desktop and go to <strong>Settings</strong></li>
                        <li>Navigate to the <strong>MCP Servers</strong> section</li>
                        <li>Click <strong>"Add Server"</strong></li>
                        <li>Enter the TrashMob MCP server URL: <code>https://mcp.trashmob.eco</code></li>
                        <li>Save and restart Claude Desktop</li>
                    </ol>
                    <p>Once connected, you can ask Claude natural language questions about TrashMob events, teams, communities, and more.</p>
                `,
            },
            {
                question: 'Connecting from Claude Code',
                answer: `
                    <p>To use TrashMob with Claude Code (CLI):</p>
                    <ol class="list-decimal pl-8">
                        <li>Open your Claude Code settings file</li>
                        <li>Add the TrashMob MCP server to your configuration:
                            <pre class="bg-gray-100 p-3 rounded mt-2 text-sm overflow-x-auto">{
  "mcpServers": {
    "trashmob": {
      "url": "https://mcp.trashmob.eco"
    }
  }
}</pre>
                        </li>
                        <li>Restart Claude Code</li>
                    </ol>
                    <p>Claude Code will automatically discover the available tools and you can use them in your conversations.</p>
                `,
            },
            {
                question: 'Privacy & Security',
                answer: `
                    <p>The TrashMob MCP server is designed with privacy as a priority:</p>
                    <ul class="list-disc pl-8">
                        <li><strong>Read-only</strong> - The server cannot create, modify, or delete any data</li>
                        <li><strong>Public data only</strong> - Only information already visible on the TrashMob website is accessible</li>
                        <li><strong>No personal information</strong> - User emails, phone numbers, and other private data are never exposed</li>
                        <li><strong>Anonymized statistics</strong> - Route data and metrics are aggregated, not individual</li>
                    </ul>
                    <p>The MCP server follows the same privacy standards as the public TrashMob website.</p>
                `,
            },
        ],
    },
];

export const Help: React.FC = () => {
    const [selectedTab, setSelectedTab] = React.useState<string>('Events');
    const tabContent = tabContents.find((tab) => tab.category === selectedTab);

    return (
        <div>
            <HeroSection Title='Site Help' Description='Answering your questions!' />
            <div className='container mx-auto my-5 pt-5 space-y-4'>
                <div className='flex flex-col sm:flex-row gap-4'>
                    {tabContents.map((tabContent, ind) => (
                        <Card
                            key={ind}
                            className={`grow basis-1/3 ${selectedTab === tabContent.category && 'border-primary'}`}
                            onClick={() => setSelectedTab(tabContent.category)}
                        >
                            <CardHeader>
                                <CardTitle>{tabContent.category}</CardTitle>
                                <CardDescription>{tabContent.desc}</CardDescription>
                            </CardHeader>
                            <div className='flex justify-end'>
                                <img src={tabContent.icon} alt='icon' className='mt-0 max-w-24' />
                            </div>
                        </Card>
                    ))}
                </div>
                {tabContent ? (
                    <Card>
                        <CardHeader>
                            <CardTitle>{tabContent.category}</CardTitle>
                        </CardHeader>
                        <CardContent className='flex flex-col gap-4'>
                            {tabContent.questions.map((item, i) => (
                                <div className='prose' key={`q-${i}`}>
                                    <h5>{item.question}</h5>
                                    <div
                                        className='text-base text-foreground/75'
                                        dangerouslySetInnerHTML={{ __html: item.answer }}
                                    />
                                </div>
                            ))}
                        </CardContent>
                    </Card>
                ) : null}
            </div>
            <div className='bg-card py-5 text-center'>
                <h1 className='font-medium text-center'>Are we missing something?</h1>
                <h4 className='text-center'>Let us know by reaching out.</h4>
                <Button asChild>
                    <Link to='/contactus'>Contact Us</Link>
                </Button>
            </div>
        </div>
    );
};
