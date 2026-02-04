import * as React from 'react';
import { Link } from 'react-router';
import events from '@/components/assets/faq/Event.svg';
import gloves from '@/components/assets/gloves.svg';
import teams from '@/components/assets/card/twofigure.svg';
import communities from '@/components/assets/home/Person.svg';
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
                question: 'After the Event',
                answer: `
                    <p>After your cleanup, record what you accomplished:</p>
                    <ol class="list-decimal pl-8">
                        <li>Go to <strong>My Dashboard</strong> and find your completed event</li>
                        <li>Click <strong>"Add Summary"</strong> to record:
                            <ul class="list-disc pl-8 mt-2">
                                <li>Number of attendees</li>
                                <li>Bags of litter collected</li>
                                <li>Actual duration</li>
                                <li>Notes about the cleanup</li>
                            </ul>
                        </li>
                        <li>If a partner handled hauling, mark pickup locations so they know where to collect the bags</li>
                    </ol>
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
                    <h6>Partner Dashboard Sections</h6>
                    <ul class="list-disc pl-8">
                        <li><strong>Manage Partner</strong> - Update organization name and website</li>
                        <li><strong>Contacts</strong> - Set who TrashMob staff should contact for partnership issues</li>
                        <li><strong>Admins</strong> - Add multiple people who can manage the partnership (recommended for continuity)</li>
                        <li><strong>Documents</strong> - Store links to agreements, marketing materials, etc.</li>
                        <li><strong>Social Media</strong> - List accounts to tag when we post about your events</li>
                    </ul>
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
                    <p>Communities are local areas or organizations that have partnered with TrashMob to organize cleanups in their region.</p>
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
                    <p>Represent a city, organization, or regional group? Here's how to create a community page:</p>
                    <h6>Who Can Partner?</h6>
                    <ul class="list-disc pl-8">
                        <li>Cities and municipalities</li>
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
