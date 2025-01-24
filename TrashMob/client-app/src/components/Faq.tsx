import * as React from 'react';
import { Col, Container, Image, Row } from 'react-bootstrap';
import { Link } from 'react-router';
import events from './assets/faq/Event.svg';
import attendees from './assets/faq/Attendees.svg';
import volunteer from './assets/faq/volunteer.svg';
import { HeroSection } from './Customization/HeroSection';

const tabs = [
    {
        title: 'Events',
        desc: 'Registration, creation, and location',
        icon: events,
        value: 1,
    },
    {
        title: 'User profile',
        desc: 'Account and privacy',
        icon: attendees,
        value: 2,
    },
    {
        title: 'TrashMob',
        desc: 'About and volunteering',
        icon: volunteer,
        value: 3,
    },
];

const Events: React.FC = () => {
    return (
        <div className='active-border rounded mt-4 bg-white'>
            <h3 className='pl-4 font-weight-500 color'>Events</h3>
            <Row className='p-4'>
                <Col md={6}>
                    <h5 className='font-weight-500'>How do I register for an event?</h5>
                    <p className='para'>
                        Browse the available events and click register. If you have not created an account, you will be
                        asked to create one to ensure the safety and integrity of TrashMob events. View your registered
                        events in your user dashboard.
                    </p>
                    <h5 className='font-weight-500 mt-5'>Can I go to an event if I’m not signed up in advance? </h5>
                    <p className='para'>
                        It depends. Some events are capacity constrained due to safety gear, special training, and
                        roadway space and require registration in advance. We suggest contacting the event organizer to
                        get an appropriate answer for your situation. If you are unable to attend the event, we
                        recommend taking that same time and picking up litter in a local park or nearby area. Every
                        piece matters!
                    </p>
                    <h5 className='font-weight-500 mt-5'>How do I create an event?</h5>
                    <p className='para'>
                        Create an event <a href='https://www.trashmob.eco/manageeventdashboard>'>here</a> by naming your
                        event, setting a location, and filling in relevant details like date, time, and a description.
                        Share your created event across your network by viewing the event details and clicking “share”.
                        View your created events in your user dashboard.{' '}
                    </p>
                    <h5 className='font-weight-500'>Why do I have to be a registered user to create an event?</h5>
                    <p className='para'>
                        We want to ensure that all events are legitimate and properly led, and anonymously user led
                        events have the potential to create security and safety problems.
                    </p>
                </Col>
                <Col md={6}>
                    <h5 className='font-weight-500 mt-5'>
                        Why does TrashMob ask for latitude and longitude values when creating an event?{' '}
                    </h5>
                    <p className='para'>
                        Certain locations like parking lots in state parks do not have street addresses. Using a
                        latitude and longitude value for every event ensures we have an accurate location so all
                        registered attendees show up in the right spot.{' '}
                    </p>
                    <h5 className='font-weight-500 mt-5'>How do I set the location for an event?</h5>
                    <p className='m-0'>There are 2 ways to set the location for your event:</p>
                    <ol>
                        <li>
                            Find the map pin that notes your starting location. Click and drag that to the location you
                            want to start your pickup at.{' '}
                        </li>
                        <li>
                            If you don’t see a pin on the map, type in the name of the city closest to your desired
                            event location in the search box and select the city. Then drag the pin to the exact meetup
                            location for your event.{' '}
                        </li>
                    </ol>
                    <h5 className='font-weight-500 mt-5'>What is the difference between a Private and Public Event?</h5>
                    <p className='m-0'>
                        When you create a public event, anyone can see it and register for it on the site, and the
                        date/time of the event must be in the future.
                    </p>
                    <p className='m-0'>
                        When you create a private event, only you can see it, and the date may be in the past or in the
                        future. This is sometimes useful when you have done some unplanned picking on your own and want
                        to track your effort after the fact. Eventually, you will be able to send invitations to
                        specific people for a private event.
                    </p>
                </Col>
            </Row>
        </div>
    );
};
const UserProfile: React.FC = () => (
    <div className='active-border rounded mt-4 bg-white'>
        <h3 className='pl-4 font-weight-500 color'>User profile</h3>
        <Row className='p-4'>
            <Col md={6}>
                <h5 className='font-weight-500'>Why does TrashMob ask for my email address?</h5>
                <p className='para'>
                    We ask for your email address so we can contact you regarding events on this website. We’ll never
                    give your email address to outside agencies (unless required by law) or to other users. See our{' '}
                    <a href='https://www.trashmob.eco/privacypolicy>'>privacy policy</a> for more.
                </p>
                <h5 className='font-weight-500 mt-5'>What does TrashMob do with my profile and event data?</h5>
                <p className='para'>
                    Profile data is used to help event owners have an accurate list of registered attendees and their
                    contact information. Event data is used to add to TrashMob’s total stats. Please read our{' '}
                    <a href='https://www.trashmob.eco/privacypolicy>'>privacy policy</a> and{' '}
                    <a href='https://www.trashmob.eco/termsofservice'>terms of service</a>.
                </p>
            </Col>
            <Col md={6}>
                <h5 className='font-weight-500 mt-5'>Can I change my base location?</h5>
                <p className='para'>
                    Yes. Simply go to your{' '}
                    <a href='https://www.trashmob.eco/locationpreference>'>Location Preference</a> page and drag the pin
                    for your current location to wherever you want your base location to be. If you set the notification
                    distance from this point, then you will be notified for any events that are created within that
                    radius of your base location.
                </p>
            </Col>
        </Row>
    </div>
);
const Volunteer: React.FC = () => (
    <div className='active-border rounded mt-4 bg-white'>
        <h3 className='pl-4 font-weight-500 color'>TrashMob</h3>
        <Row className='p-4'>
            <Col md={6}>
                <h5 className='font-weight-500'>Is TrashMob a nonprofit organization?</h5>
                <p className='para'>Yes! We received our 501(3)(c) status in April of 2022. </p>
                <h5 className='font-weight-500 mt-5'>Can I donate to TrashMob? </h5>
                <p className='para'>
                    Yes! Check out our <a href='https://www.trashmob.eco/donate'>Donate</a> page!
                </p>
                <h5 className='font-weight-500 mt-5'>Does signing up for TrashMob cost anything?</h5>
                <p className='para'>
                    No! The TrashMob website is free to use and there’s no cost associated with setting up or joining
                    TrashMobs.
                </p>
                <h5 className='font-weight-500 mt-5'>Can I report a website bug?</h5>
                <p className='para'>
                    Yes, you can! Please <a href='https://www.trashmob.eco/contactus'>Contact us</a> and let us know
                    what you found. Thank you!
                </p>
            </Col>
            <Col md={6}>
                <h5 className='font-weight-500'>Is the TrashMob team looking for support? </h5>
                <p className='para'>
                    Yes! We are always looking for new team members to help us improve TrashMob.eco and implement new
                    features. If you or someone you know has any skills in design, back or front-end development,
                    program management, or applicable non-technical skills, please Contact us!
                </p>
                <h5 className='font-weight-500 mt-5'>Does TrashMob have a mobile app? </h5>
                <p className='para'>Not yet, but we are hard at work making one! Stay tuned!</p>
            </Col>
        </Row>
    </div>
);

export const Faq: React.FC = () => {
    const [selectedTab, setSelectedTab] = React.useState(0);

    return (
        <>
            <HeroSection Title='FAQ' Description='We’ve got you covered.' />
            <Container className='my-5 pt-5'>
                <div className='faq-tabs-wrapper'>
                    {tabs?.map((tab, ind) => (
                        <div
                            key={ind}
                            className={`faq-tab ${selectedTab !== 0 && 'tab-select'} px-4 pt-4 pb-0 bg-white rounded ${selectedTab === tab?.value ? 'active-border' : 'border border-white'}`}
                            onClick={() => setSelectedTab(tab?.value)}
                        >
                            <h1 className='m-0'>{tab?.title}</h1>
                            <h5 className='color'>{tab?.desc}</h5>
                            <Image src={tab?.icon} alt='icon' className='mt-0 float-right' />
                        </div>
                    ))}
                </div>

                {selectedTab === 1 && <Events />}
                {selectedTab === 2 && <UserProfile />}
                {selectedTab === 3 && <Volunteer />}
            </Container>
            <Container fluid className='bg-white py-5 text-center'>
                <h1 className='font-weight-500 text-center'>Are we missing something?</h1>
                <h4 className='text-center'>Let us know by reaching out.</h4>
                <Link className='btn btn-primary ml-5 py-md-3 banner-button' to='/contactus'>
                    Contact Us
                </Link>
            </Container>
        </>
    );
};
