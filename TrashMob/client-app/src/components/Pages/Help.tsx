import * as React from 'react';
import { Col, Container, Row, Image } from 'react-bootstrap';
import { HeroSection } from '../Customization/HeroSection';

export const Help: React.FC = () => {

    React.useEffect(() => {
        window.scrollTo(0, 0);
    })

    return (
        <>
            <HeroSection Title="Site Help" Description="Answering your questions!" />

            <Container fluid className="mt-1 bg-white p-5">
                <Row className="mb-4 px-md-5" xs={1} lg={2}>
                    <Col className="mb-5 pl-lg-5">
                        <div className="px-lg-5">
                            <h2>How to create an event</h2>
                            <p>
                                First things first, if you are not signed up or signed in, click the sign in button to create a new account or sign in.
                            </p>
                            <p>
                                Once you are registered and logged in, the first thing you want to do is to set your preferred location. Click on the User icon in the top right corner of the tool bar, and select "Set Location Preference".
                                This will open a new screen that allows you to set your "Home" location. Note that this isn't where you live. It's just a point nearby. Next, set how far you are willing to travel for an event, and indicate
                                whether or not you prefer miles or kilometers. You will only get notified for events within that travel radius from the location you choose, so choose the location and the distance appropriately
                                so you are getting notified for events that don't demand a lot of travel time. Save your changes.
                            </p>
                            <p>
                                Now you are ready to create your first event.
                            </p>
                            <p>
                                Go back to the <a href="https://www.trashmob.eco">home</a> page and click the "Create Event" button.
                            </p>
                            <p>
                                If you haven't already signed a Liability Waiver with TrashMob, you will be asked to sign a waiver on-line. After you accept the waiver, a screen will open that will allow you to create a new event.
                            </p>
                            <p>
                                Fill in a name for the event, a date/time and how long you expect the event to be. Enter a description for the event... what you think you will be cleaning, and any special instructions for clothes to wear, gear to bring, or where to park.
                            </p>
                            <p>
                                Next, use the map to drag and drop a pin to where you want to meet up. Zoom in as needed and set the location as accurately as possible to that other will be able to find you easily at the start of the event.
                            </p>
                            <p>
                                Save your changes.
                            </p>
                            <p>
                                If you want help from a partner for the event, and there are partners registered in your area, you can select from the list of available partners and the services they provide (Hauling, Disposal, Supplies or Starter Kits). If there are no partners available in your area, click the Invite a Partner button and invite potential partners in your community to join TrashMob!
                            </p>
                        </div>
                        <div className="px-lg-5">
                            <h2>How to set up a partnership</h2>
                            <p>
                                TrashMob.eco helps community volunteers to organize litter and neighborhood cleanups. But volunteers need help in ways that TrashMob.eco cannot provide. When the litter has been picked, it needs to be
                                hauled or disposed of properly. Most volunteers won't have the ability to haul or dispose of more than 1-2 bags or garbage per week. That where TrashMob.eco Partners can help!
                            </p>
                            <p>
                                Partners can be local governments, state governments, local businesses and organizations, or even large corporations.
                            </p>
                            <p>
                                Getting started as a partner involves registering on the website.
                            </p>
                            <p>
                                First things first, if you are not signed up or signed in, click the sign in button to create a new account or sign in.
                            </p>
                            <p>
                                Next, go to the <a href="https://www.trashmob.eco/partnerships">Partnerships</a> page via the link on the footer
                            </p>
                            <p>
                                On the Partnership page, click the "Become a Partner" Button
                            </p>
                            <p>
                                This will take you to a form that will allow you to apply to Become a Partner. Fill out the form, specifying a Name (i.e. your company name or city name), whether you are a government or a business, your contact email and website, and a phone number. Then, using the map, set the location for your main office. You may have other locations (franchises, shops, yards) but those will be added later. Click the submit button.
                            </p>
                            <p>
                                After hitting the submit button, your application will be reviewed and approved by a member of the TrashMob.eco staff. If we have any questions about it, we will contact you via the information you have provided.
                            </p>
                            <p>
                                Once we have approved your application, you will get an email to let you know your application has been approved, along with instructions one what to do next. Here's a quick overview.
                            </p>
                            <p>
                                Go to the <a href="https://www.trashmob.eco/mydashboard>">My Dashboard</a> page on the TrashMob.eco website.
                            </p>
                            <p>
                                Under My Partnerships, locate the approved partner, and under the Actions column, select Activate Partnership. This will take you to the Partnership Dashboard. There's a ton going on here, but we'll walk through each screen.
                            </p>
                            <p>
                                The Manage Partner page allows you to alter the name and website of your headquarters.
                            </p>
                            <p>
                                The Manage Partner Contacts page allows you to set who TrashMob.eco staff should contact in case there is an issue with the partnership.
                            </p>
                            <p>
                                The Manage Partner Admins page allows you to set who can update the locations, contacts, etc. for the partner. It is recommended to have multiple admins set up for each partner just in case someone leaves your organization or is unavailable when needed. It's a good idea to keep this list updated as people join/leave your organization to keep your Partnership data secure.
                            </p>
                            <p>
                                The Manage Partner Documents page allows you and TrashMob to associate documents to the Partnership as needed. These documents may be Organizational Volunteer Agreements, or marketing docs, etc. This feature is still in development, but currently allows you to store links to documents as needed.
                            </p>
                            <p>
                                The Manage Partner Social Media Accounts allows you to list the social media accounts that are managed by your organization which you would like us to copy in when we post about events you are involved in to social media. This allows TrashMob to gain volunteers through your social media reach, and allows you to gain community appreciation through your involvement with TrashMob.
                            </p>
                            <p>
                                The most important sub-page under the Manage Partner page is the Manage Partner Location Page. Partner Locations are an integral part of TrashMob.eco. A partner location can be as simple as the location of a dumpster that TrashMob.eco volunteers can use to dispose of litter during an event, or an office to contact to arrange for hauling of picked litter, or a place to pick up supplies or starter kits. A partner can have many locations, but must always have at least 1.
                            </p>
                            <p>
                                On the Partner Locations sub page, click the Add Location button
                            </p>
                            <p>
                                Choose a name and set the location via the map. Make this setting as accurate as possible, especially if the location is a drop off point for picked litter. Public Notes will be visible to the general public when the event lead is looking for partners, and for any event attendees who need information about the location. This should include hours of operation, or any special instructions to follow when using this location. Private notes are only for communication with TrashMob staff or for internal use for the partner location to communicate back to the headquarters.
                            </p>
                            <p>
                                Once you have set the location, save the changes. You will now be able to Add Partner Location Contacts. These are the people who will receive emails of get phone calls from Event Leads when events are set up near you. It is recommended to set up an email distribution group for each location internally instead of listing individuals to prevent messages from getting lost when people leave your organization. Save changes when complete.
                            </p>
                            <p>
                                Next, you will want to specify which services are available at each location. There are currently 4 choices:
                                <ol>
                                    <li>Hauling</li>
                                    <li>Disposal</li>
                                    <li>Starter Kits</li>
                                    <li>Supplies</li>
                                </ol>
                            </p>
                            <p>
                                You don't have to provide all services. Just set up the ones you have capabilities to handle. If you would like to provide starter kits, the basics are a 5 gallon plastic bucket, a grabber, a reflective safety vest, 30 gallon trash bags, and gloves. You can either obtain these yourself, or work with TrashMob.eco staff to put the kits together.
                            </p>
                            <p>
                                If you are offering hauling services, you will be notified via email to the location contacts after events when there are piles of litter to be picked up, with information about where the piles are. Please make sure you have the appropriate hauling/safety vehicles before signing up to provide this this service.
                            </p>
                            <p>
                                You can also set a flag on the service to indicate whether or not service requests are auto-approved. When an event lead requests the service, they will automatically be told their request was approved, without an admin approving the request manually.
                            </p>
                            <p>
                                You can also set a flag that indicates whether or not you need advance notification for the request. If, for instance, you are offering starter kits no matter where or when within you community, and the event lead can just show up and get the kits at any time, then set this flag to false. If, however, you need to make sure crews are available to haul litter before you agree to handle the request for hauling, set this to true to make sure you have advance notice before an event so the bags don't just sit by the roadside indefinitely.
                            </p>
                            <p>
                                Once you have configured you location, make sure you set the Partner Location status to active and save changes.
                            </p>
                            <p>
                                Finally, go back to the Partner Dashboard, and set the Partner Status to active and save changes. Your locations will now be offered to event leads for the services you have set up.
                            </p>
                            <p>
                                Becoming a partner takes a little bit of work, but helping your community get and stay clean can be extremely rewarding. It doesn't require a huge outlay of cash up front (unless you want to put together a large collection of starter kits). A few minutes a month can go a long way to generating goodwill within the community, and helping to create a cleaner planet!
                            </p>
                        </div>
                    </Col>
                </Row>
            </Container>
        </>
    );
}