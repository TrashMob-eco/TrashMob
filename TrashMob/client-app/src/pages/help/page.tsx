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
        desc: 'How to create an event',
        icon: events,
        questions: [
            {
                question: 'How to create an event',
                answer: `
                  <p>First things first, if you are not signed up or signed in, click the sign in button to create a new account or sign in.</p>
                  <p>Once you are registered and logged in, the first thing you want to do is to set your preferred
                        location. Click on the User icon in the top right corner of the tool bar, and select "Set
                        Location Preference". This will open a new screen that allows you to set your "Home" location.
                        Note that this isn't where you live. It's just a point nearby.</p>
                  <p>Next, set how far you are willing to travel for an event, and indicate whether or not you prefer
                        miles or kilometers. You will only get notified for events within that travel radius from the
                        location you choose, so choose the location and the distance appropriately so you are getting
                        notified for events that don't demand a lot of travel time. Save your changes.</p>
                `,
            },
            {
                question: 'Now you are ready to create your first event.',
                answer: `
                <p>Go back to the <a href='https://www.trashmob.eco'>home</a> page and click the "Create Event" button.</p>
                <p>If you haven't already signed a Liability Waiver with TrashMob, you will be asked to sign a
                        waiver on-line. After you accept the waiver, a screen will open that will allow you to create a
                        new event.</p>
                <p>Fill in a name for the event, a date/time and how long you expect the event to be. Enter a
                        description for the event... what you think you will be cleaning, and any special instructions
                        for clothes to wear, gear to bring, or where to park.</p>
                <p>Next, use the map to drag and drop a pin to where you want to meet up. Zoom in as needed and set
                        the location as accurately as possible to that other will be able to find you easily at the
                        start of the event.</p>
                <p>Save your changes.</p>
                <p>If you want help from a partner for the event, and there are partners registered in your area,
                        you can select from the list of available partners and the services they provide (Hauling,
                        Disposal, Supplies or Starter Kits). If there are no partners available in your area, click the
                        Invite a Partner button and invite potential partners in your community to join TrashMob!</p>
              `,
            },
        ],
    },
    {
        category: 'Partnerships',
        desc: 'How to set up a partnership',
        icon: gloves,
        questions: [
            {
                question: 'How to set up a partnership',
                answer: `
            <p>
                TrashMob.eco helps community volunteers to organize litter and neighborhood cleanups. But volunteers
                need help in ways that TrashMob.eco cannot provide. When the litter has been picked, it needs to be
                hauled or disposed of properly. Most volunteers won't have the ability to haul or dispose of more
                than 1-2 bags or garbage per week. That where TrashMob.eco Partners can help!
            </p>
            <p>
                Partners can be local governments, state governments, local businesses and organizations, or even
                large corporations.
            </p>
            <p>Getting started as a partner involves registering on the website.</p>
            <p>
                First things first, if you are not signed up or signed in, click the sign in button to create a new
                account or sign in.
            </p>
            <p>
                Next, go to the <a href='https://www.trashmob.eco/partnerships'>Partnerships</a> page via the link
                on the footer
            </p>
            <p>On the Partnership page, click the "Become a Partner" Button</p>
            <p>
                This will take you to a form that will allow you to apply to Become a Partner. Fill out the form,
                specifying a Name (i.e. your company name or city name), whether you are a government or a business,
                your contact email and website, and a phone number. Then, using the map, set the location for your
                main office. You may have other locations (franchises, shops, yards) but those will be added later.
                Click the submit button.
            </p>
            <p>
                After hitting the submit button, your application will be reviewed and approved by a member of the
                TrashMob.eco staff. If we have any questions about it, we will contact you via the information you
                have provided.
            </p>
            <p>
                Once we have approved your application, you will get an email to let you know your application has
                been approved, along with instructions one what to do next. Here's a quick overview.
            </p>
            <p>
                Go to the <a href='https://www.trashmob.eco/mydashboard>'>My Dashboard</a> page on the TrashMob.eco
                website.
            </p>
            <p>
                Under My Partnerships, locate the approved partner, and under the Actions column, select Activate
                Partnership. This will take you to the Partnership Dashboard. There's a ton going on here, but we'll
                walk through each screen.
            </p>
            <p>
                The Manage Partner page allows you to alter the name and website of your headquarters.
            </p>
            <p>
                The Manage Partner Contacts page allows you to set who TrashMob.eco staff should contact in case
                there is an issue with the partnership.
            </p>
            <p>
                The Manage Partner Admins page allows you to set who can update the locations, contacts, etc. for
                the partner. It is recommended to have multiple admins set up for each partner just in case someone
                leaves your organization or is unavailable when needed. It's a good idea to keep this list updated
                as people join/leave your organization to keep your Partnership data secure.
            </p>
            <p>
                The Manage Partner Documents page allows you and TrashMob to associate documents to the Partnership
                as needed. These documents may be Organizational Volunteer Agreements, or marketing docs, etc. This
                feature is still in development, but currently allows you to store links to documents as needed.
            </p>
            <p>
                The Manage Partner Social Media Accounts allows you to list the social media accounts that are
                managed by your organization which you would like us to copy in when we post about events you are
                involved in to social media. This allows TrashMob to gain volunteers through your social media
                reach, and allows you to gain community appreciation through your involvement with TrashMob.
            </p>
            <p>
                The most important sub-page under the Manage Partner page is the Manage Partner Location Page.
                Partner Locations are an integral part of TrashMob.eco. A partner location can be as simple as the
                location of a dumpster that TrashMob.eco volunteers can use to dispose of litter during an event, or
                an office to contact to arrange for hauling of picked litter, or a place to pick up supplies or
                starter kits. A partner can have many locations, but must always have at least 1.
            </p>
            <!-- Second Column -->
            <p>On the Partner Locations sub page, click the Add Location button</p>
            <p>
                Choose a name and set the location via the map. Make this setting as accurate as possible,
                especially if the location is a drop off point for picked litter. Public Notes will be visible to
                the general public when the event lead is looking for partners, and for any event attendees who need
                information about the location. This should include hours of operation, or any special instructions
                to follow when using this location. Private notes are only for communication with TrashMob staff or
                for internal use for the partner location to communicate back to the headquarters.
            </p>
            <p>
                Once you have set the location, save the changes. You will now be able to Add Partner Location
                Contacts. These are the people who will receive emails of get phone calls from Event Leads when
                events are set up near you. It is recommended to set up an email distribution group for each
                location internally instead of listing individuals to prevent messages from getting lost when people
                leave your organization. Save changes when complete.
            </p>
            <p>
                Next, you will want to specify which services are available at each location. There are currently 4
                choices:
                <ol class="list-decimal pl-8">
                    <li>Hauling</li>
                    <li>Disposal</li>
                    <li>Starter Kits</li>
                    <li>Supplies</li>
                </ol>
            </p>
            <p>
                You don't have to provide all services. Just set up the ones you have capabilities to handle. If you
                would like to provide starter kits, the basics are a 5 gallon plastic bucket, a grabber, a
                reflective safety vest, 30 gallon trash bags, and gloves. You can either obtain these yourself, or
                work with TrashMob.eco staff to put the kits together.
            </p>
            <p>
                If you are offering hauling services, you will be notified via email to the location contacts after
                events when there are piles of litter to be picked up, with information about where the piles are.
                Please make sure you have the appropriate hauling/safety vehicles before signing up to provide this
                this service.
            </p>
            <p>
                You can also set a flag on the service to indicate whether or not service requests are
                auto-approved. When an event lead requests the service, they will automatically be told their
                request was approved, without an admin approving the request manually.
            </p>
            <p>
                You can also set a flag that indicates whether or not you need advance notification for the request.
                If, for instance, you are offering starter kits no matter where or when within you community, and
                the event lead can just show up and get the kits at any time, then set this flag to false. If,
                however, you need to make sure crews are available to haul litter before you agree to handle the
                request for hauling, set this to true to make sure you have advance notice before an event so the
                bags don't just sit by the roadside indefinitely.
            </p>
            <p>
                Once you have configured you location, make sure you set the Partner Location status to active and
                save changes.
            </p>
            <p>
                Finally, go back to the Partner Dashboard, and set the Partner Status to active and save changes.
                Your locations will now be offered to event leads for the services you have set up.
            </p>
            <p>
                Becoming a partner takes a little bit of work, but helping your community get and stay clean can be
                extremely rewarding. It doesn't require a huge outlay of cash up front (unless you want to put
                together a large collection of starter kits). A few minutes a month can go a long way to generating
                goodwill within the community, and helping to create a cleaner planet!
            </p>
          `,
            },
        ],
    },
    {
        category: 'Teams',
        desc: 'How to create and manage a team',
        icon: teams,
        questions: [
            {
                question: 'How to create a Team',
                answer: `
                <p>Teams allow you to organize regular cleanup efforts with a consistent group of volunteers. Here's how to get started:</p>
                <p>First, make sure you are signed in. If you don't have an account, create one by clicking the sign in button.</p>
                <p>Navigate to the <a href='https://www.trashmob.eco/teams'>Teams page</a> and click the "Create Team" button.</p>
                <p>Fill in your team details:</p>
                <ul class="list-disc pl-8">
                    <li><strong>Team Name:</strong> Choose a unique, descriptive name for your team</li>
                    <li><strong>Description:</strong> Tell others about your team's focus, meeting frequency, or areas you clean</li>
                    <li><strong>Location:</strong> Set your team's primary location using the map or search box</li>
                    <li><strong>Visibility:</strong> Choose Public (visible on map, open for join requests) or Private (invite-only)</li>
                    <li><strong>Require Approval:</strong> Decide if you want to approve new member requests or allow anyone to join immediately</li>
                </ul>
                <p>Click "Create Team" to finish. You'll be automatically added as the team lead.</p>
                `,
            },
            {
                question: 'Managing your Team',
                answer: `
                <p>As a team lead, you have several management capabilities:</p>
                <h6>Editing Team Details</h6>
                <p>Go to your team's page and click "Edit Team" to update the name, description, location, or visibility settings.</p>
                <h6>Managing Members</h6>
                <p>From the team edit page, you can:</p>
                <ul class="list-disc pl-8">
                    <li><strong>View pending join requests:</strong> Approve or decline requests from users who want to join</li>
                    <li><strong>Promote to Co-Lead:</strong> Give other members lead privileges to help manage the team</li>
                    <li><strong>Remove members:</strong> Remove members who are no longer active or appropriate for the team</li>
                </ul>
                <h6>Tips for Team Leads</h6>
                <ul class="list-disc pl-8">
                    <li>Have at least 2 team leads to ensure continuity</li>
                    <li>Keep your team description up-to-date with meeting schedules</li>
                    <li>Respond to join requests promptly to keep new volunteers engaged</li>
                    <li>Consider making your team public to attract more members from your community</li>
                </ul>
                `,
            },
            {
                question: 'Team Photos',
                answer: `
                <p>Team leads can upload photos to showcase their team's cleanup activities and build community engagement.</p>
                <h6>Uploading Photos</h6>
                <ol class="list-decimal pl-8">
                    <li>Go to your team's page and click "Manage Team" (you must be a team lead)</li>
                    <li>Scroll down to the "Team Photos" section</li>
                    <li>Click "Upload Photo" and select an image file (JPEG, PNG, etc.)</li>
                    <li>The photo will be uploaded and appear in the gallery</li>
                </ol>
                <h6>Photo Guidelines</h6>
                <ul class="list-disc pl-8">
                    <li>Images must be less than 10MB in size</li>
                    <li>Supported formats: JPEG, PNG, and other common image formats</li>
                    <li>Photos are automatically resized for optimal display</li>
                    <li>Only team leads can upload or delete photos</li>
                </ul>
                <h6>Viewing Photos</h6>
                <p>Team photos are visible to anyone viewing the team's detail page. They help showcase your team's impact and can attract new members!</p>
                `,
            },
            {
                question: 'Team Logo',
                answer: `
                <p>Team leads can upload a logo or avatar image to give their team a visual identity.</p>
                <h6>Uploading a Logo</h6>
                <ol class="list-decimal pl-8">
                    <li>Go to your team's page and click "Manage Team" (you must be a team lead)</li>
                    <li>Find the "Team Logo" section</li>
                    <li>Click "Upload Logo" and select an image file</li>
                    <li>The logo will be uploaded and displayed on your team's profile</li>
                </ol>
                <h6>Logo Guidelines</h6>
                <ul class="list-disc pl-8">
                    <li>Recommended: Square image, at least 200x200 pixels</li>
                    <li>Maximum file size: 5MB</li>
                    <li>Supported formats: JPEG, PNG, and other common image formats</li>
                    <li>Logos are automatically resized for optimal display</li>
                </ul>
                <h6>Where the Logo Appears</h6>
                <ul class="list-disc pl-8">
                    <li>Team detail page (next to team name)</li>
                    <li>Teams list page (in the team name column)</li>
                    <li>Teams map (in the info popup)</li>
                </ul>
                `,
            },
            {
                question: 'Joining a Team',
                answer: `
                <p>Finding and joining a team is easy:</p>
                <ol class="list-decimal pl-8">
                    <li>Go to the <a href='https://www.trashmob.eco/teams'>Teams page</a></li>
                    <li>Browse the list view or switch to map view to find teams near you</li>
                    <li>Click on a team to view its details, including description, location, and member count</li>
                    <li>Click "Request to Join" to submit your request</li>
                    <li>Wait for a team lead to approve your request (you'll receive an email notification)</li>
                </ol>
                <p>Once approved, the team will appear in your dashboard under "My Teams". You can then participate in team events and track your collective impact!</p>
                <p><strong>Note:</strong> Private teams are not visible in search results. You'll need a direct invitation from a team lead to join a private team.</p>
                `,
            },
        ],
    },
    {
        category: 'Communities',
        desc: 'How to explore and engage with communities',
        icon: communities,
        questions: [
            {
                question: 'Exploring Communities',
                answer: `
                <p>Communities represent local areas or organizations that have partnered with TrashMob to organize cleanup efforts in their region.</p>
                <h6>Finding Communities</h6>
                <ol class="list-decimal pl-8">
                    <li>Go to the <a href='https://www.trashmob.eco/communities'>Communities page</a></li>
                    <li>Browse the list of available communities</li>
                    <li>Use the search box to find communities by name or location</li>
                    <li>Click on a community card to view its full details</li>
                </ol>
                <h6>What You'll Find</h6>
                <p>Each community page includes:</p>
                <ul class="list-disc pl-8">
                    <li><strong>Banner and branding:</strong> Visual identity of the community</li>
                    <li><strong>About section:</strong> Description of the community and its cleanup initiatives</li>
                    <li><strong>Contact information:</strong> Email, phone, and address for the community</li>
                    <li><strong>Events:</strong> Upcoming and past cleanup events in the area</li>
                    <li><strong>Teams:</strong> Volunteer teams operating in the community</li>
                    <li><strong>Impact statistics:</strong> Total bags collected, volunteers, hours, and more</li>
                </ul>
                `,
            },
            {
                question: 'Getting Involved in a Community',
                answer: `
                <p>Communities are geographic partnerships, not membership groups like Teams. Here's how you can participate:</p>
                <h6>Attend Events</h6>
                <p>Browse the events listed on a community's page and register for upcoming cleanups in that area. Your participation automatically contributes to the community's impact statistics.</p>
                <h6>Join Local Teams</h6>
                <p>Many communities have active teams. Check the Teams section on a community page to find groups you can join for regular cleanup activities.</p>
                <h6>Report Litter</h6>
                <p>If you notice litter in a community, submit a <a href='https://www.trashmob.eco/litterreport'>Litter Report</a> to help identify areas that need attention.</p>
                <h6>Contact the Community</h6>
                <p>Have questions or ideas? Use the contact information on the community page to reach out to the community administrators.</p>
                `,
            },
            {
                question: 'Community Administration',
                answer: `
                <p>Community Admins are designated by partner organizations to manage their community's presence on TrashMob.</p>
                <h6>Accessing the Admin Dashboard</h6>
                <p>If you are a designated Community Admin:</p>
                <ol class="list-decimal pl-8">
                    <li>Sign in to your TrashMob account</li>
                    <li>Go to your <a href='https://www.trashmob.eco/mydashboard'>Dashboard</a></li>
                    <li>Look for "My Communities" section to find communities you administer</li>
                    <li>Click "Manage" to access the Community Admin Dashboard</li>
                </ol>
                <h6>Admin Capabilities</h6>
                <p>As a Community Admin, you can:</p>
                <ul class="list-disc pl-8">
                    <li>Update community branding (banner image, logo, tagline)</li>
                    <li>Edit the About section and community description</li>
                    <li>Manage contact information (email, phone, address)</li>
                    <li>View community statistics and recent activity</li>
                    <li>See events and teams associated with the community</li>
                </ul>
                <h6>Becoming an Admin</h6>
                <p>Community Admin access is granted to representatives of partner organizations. If you represent an organization and want to administer your community page, <a href='https://www.trashmob.eco/contactus'>Contact us</a> to discuss partnership options.</p>
                `,
            },
            {
                question: 'Becoming a Community Partner',
                answer: `
                <p>If you represent a city, organization, or regional group interested in creating a community page on TrashMob, here's how to get started:</p>
                <h6>Who Can Become a Partner?</h6>
                <ul class="list-disc pl-8">
                    <li>Cities and municipalities</li>
                    <li>Regional environmental organizations</li>
                    <li>Neighborhood associations</li>
                    <li>Business improvement districts</li>
                    <li>Other organizations focused on community cleanliness</li>
                </ul>
                <h6>Getting Started</h6>
                <ol class="list-decimal pl-8">
                    <li><a href='https://www.trashmob.eco/contactus'>Contact us</a> to express your interest</li>
                    <li>We'll discuss your goals and how TrashMob can help</li>
                    <li>Complete the partnership application process</li>
                    <li>Once approved, we'll set up your branded community page</li>
                    <li>You'll be given Community Admin access to manage your page</li>
                </ol>
                <h6>Benefits of Partnership</h6>
                <ul class="list-disc pl-8">
                    <li>Branded community page showcasing your area</li>
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
