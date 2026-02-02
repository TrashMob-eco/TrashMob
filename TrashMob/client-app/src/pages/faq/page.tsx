import * as React from 'react';
import { Link } from 'react-router';
import events from '@/components/assets/faq/Event.svg';
import attendees from '@/components/assets/faq/Attendees.svg';
import volunteer from '@/components/assets/faq/volunteer.svg';
import teams from '@/components/assets/card/twofigure.svg';
import communities from '@/components/assets/home/Person.svg';
import { HeroSection } from '@/components/Customization/HeroSection';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';

const faqs = [
    {
        category: 'Events',
        desc: 'Registration, creation, and location',
        icon: events,
        questions: [
            {
                question: 'How do I register for an event?',
                answer: 'Browse the available events and click register. If you have not created an account, you will be asked to create one to ensure the safety and integrity of TrashMob events. View your registered events in your user dashboard.',
            },
            {
                question: "Can I go to an event if I'm not signed up in advance?",
                answer: 'It depends. Some events are capacity constrained due to safety gear, special training, and roadway space and require registration in advance. We suggest contacting the event organizer to get an appropriate answer for your situation. If you are unable to attend the event, we recommend taking that same time and picking up litter in a local park or nearby area. Every piece matters!',
            },
            {
                question: 'How do I create an event?',
                answer: 'Create an event <a href="https://www.trashmob.eco/events/create">here</a> by naming your event, setting a location, and filling in relevant details like date, time, and a description. Share your created event across your network by viewing the event details and clicking "share". View your created events in your user dashboard.',
            },
            {
                question: 'Why do I have to be a registered user to create an event?',
                answer: 'We want to ensure that all events are legitimate and properly led, and anonymously user-led events have the potential to create security and safety problems.',
            },
            {
                question: 'Why does TrashMob ask for latitude and longitude values when creating an event?',
                answer: 'Certain locations like parking lots in state parks do not have street addresses. Using a latitude and longitude value for every event ensures we have an accurate location so all registered attendees show up in the right spot.',
            },
            {
                question: 'How do I set the location for an event?',
                answer: '<p>There are 2 ways to set the location for your event:</p><ul class="list-decimal pl-8"><li>Find the map pin that notes your starting location. Click and drag that to the location you want to start your pickup at.</li><li>If you don’t see a pin on the map, type in the name of the city closest to your desired event location in the search box and select the city. Then drag the pin to the exact meetup location for your event.</li></ul>',
            },
            {
                question: 'What is the difference between a Private and Public Event?',
                answer: 'When you create a public event, anyone can see it and register for it on the site, and the date/time of the event must be in the future. When you create a private event, only you can see it, and the date may be in the past or in the future. This is sometimes useful when you have done some unplanned picking on your own and want to track your effort after the fact. Eventually, you will be able to send invitations to specific people for a private event.',
            },
        ],
    },
    {
        category: 'User Profile',
        desc: 'Account and privacy',
        icon: attendees,
        questions: [
            {
                question: 'Why does TrashMob ask for my email address?',
                answer: 'We ask for your email address so we can contact you regarding events on this website. We’ll never give your email address to outside agencies (unless required by law) or to other users. See our <a href="https://www.trashmob.eco/privacypolicy">privacy policy</a> for more.',
            },
            {
                question: 'What does TrashMob do with my profile and event data?',
                answer: 'Profile data is used to help event owners have an accurate list of registered attendees and their contact information. Event data is used to add to TrashMob’s total stats. Please read our <a href="https://www.trashmob.eco/privacypolicy">privacy policy</a> and <a href="https://www.trashmob.eco/termsofservice">terms of service</a>.',
            },
            {
                question: 'Can I change my base location?',
                answer: 'Yes. Simply go to your <a href="https://www.trashmob.eco/locationpreference">Location Preference</a> page and drag the pin for your current location to wherever you want your base location to be. If you set the notification distance from this point, then you will be notified for any events that are created within that radius of your base location.',
            },
        ],
    },
    {
        category: 'Teams',
        desc: 'Creating and joining teams',
        icon: teams,
        questions: [
            {
                question: 'What is a Team?',
                answer: 'A Team is a group of volunteers who work together on cleanup events. Teams can be public (visible to everyone and open for join requests) or private (invite-only). Teams help you organize recurring cleanups with a consistent group, track collective impact, and build community.',
            },
            {
                question: 'How do I create a Team?',
                answer: 'Go to the <a href="https://www.trashmob.eco/teams">Teams page</a> and click "Create Team". You\'ll need to provide a team name, description, and set your team\'s location. You can also choose whether your team is public or private, and whether new members require approval to join.',
            },
            {
                question: 'What is the difference between public and private Teams?',
                answer: 'Public teams are visible on the Teams map and in search results. Anyone can request to join a public team. Private teams are invite-only and are not visible on the map or in search results. Only team leads can invite new members to private teams.',
            },
            {
                question: 'How do I join a Team?',
                answer: 'Browse the <a href="https://www.trashmob.eco/teams">Teams page</a> to find public teams in your area. Click on a team to view its details, then click "Request to Join". The team lead will review your request and approve or decline it. For private teams, you\'ll need an invitation from a team lead.',
            },
            {
                question: 'Can I be on multiple Teams?',
                answer: 'Yes! There is no limit to the number of teams you can join. This allows you to participate in different cleanup efforts across your community.',
            },
            {
                question: 'What can Team Leads do?',
                answer: 'Team leads can edit team details, invite new members, approve join requests, promote other members to co-lead status, remove members, upload team photos and a team logo, and manage team events. A team can have multiple leads to help share the responsibility.',
            },
            {
                question: 'How do I upload Team Photos?',
                answer: 'Team leads can upload photos to showcase their team\'s cleanup activities. Go to your team\'s detail page, click "Manage Team", then scroll to the "Team Photos" section. Click "Upload Photo" to add images (JPEG, PNG, etc., up to 10MB). Photos will be visible on your team\'s public page to help attract new members and celebrate your team\'s impact!',
            },
            {
                question: 'How do I add a Team Logo?',
                answer: 'Team leads can upload a logo or avatar image for their team. Go to your team\'s detail page, click "Manage Team", then find the "Team Logo" section. Click "Upload Logo" to add an image (recommended: square image, at least 200x200 pixels, up to 5MB). The logo will be displayed on team cards, the team detail page, and map popups.',
            },
            {
                question: 'How do I leave a Team?',
                answer: 'Go to your <a href="https://www.trashmob.eco/mydashboard">Dashboard</a> and find the team in "My Teams". Click on the team, then use the "Leave Team" option. Note: If you are the only team lead, you must either promote another member to lead or contact TrashMob support.',
            },
        ],
    },
    {
        category: 'Communities',
        desc: 'Local community partnerships',
        icon: communities,
        questions: [
            {
                question: 'What is a Community?',
                answer: 'A Community in TrashMob represents a local area or city that has partnered with TrashMob to organize cleanup efforts. Communities have their own branded pages showcasing local events, teams, impact statistics, and information about the partnership. Examples include cities, neighborhoods, or regional organizations.',
            },
            {
                question: 'How do I find a Community?',
                answer: 'Visit the <a href="https://www.trashmob.eco/communities">Communities page</a> to browse all active communities. You can search by name or location to find communities near you. Each community card shows key information like location and a brief description.',
            },
            {
                question: 'What can I see on a Community page?',
                answer: 'Community pages display: the community\'s banner and branding, an about section describing the partnership, contact information, upcoming and past events in the area, teams operating in the community, impact statistics (bags collected, volunteers, hours), and links to the community\'s website and social media.',
            },
            {
                question: 'How are Communities different from Teams?',
                answer: 'Communities represent geographic areas or organizational partnerships (like a city or regional organization), while Teams are groups of individual volunteers. A Community can have multiple Teams operating within it. Communities are managed by partner organizations, whereas Teams are created and led by volunteers.',
            },
            {
                question: 'Can I join a Community?',
                answer: 'Communities are geographic partnerships, not membership groups. You automatically participate in a community\'s impact when you attend events or join teams in that area. To get more involved, look for teams in the community or contact the community administrators through the information on their page.',
            },
            {
                question: 'How do I become a Community partner?',
                answer: 'If you represent a city, organization, or regional group interested in partnering with TrashMob, please <a href="https://www.trashmob.eco/contactus">Contact us</a>. We\'ll work with you to set up a branded community page and discuss how we can support cleanup efforts in your area.',
            },
            {
                question: 'Who manages a Community?',
                answer: 'Communities are managed by designated Community Admins from the partner organization. Admins can update community information, branding, and content. They can also view statistics and activity within their community. If you need to contact a community admin, look for contact information on the community\'s page.',
            },
        ],
    },
    {
        category: 'TrashMob Organization',
        desc: 'About and volunteering',
        icon: volunteer,
        questions: [
            {
                question: 'Is TrashMob a nonprofit organization?',
                answer: 'Yes! We received our 501(3)(c) status in April of 2022.',
            },
            {
                question: 'Can I donate to TrashMob?',
                answer: 'Yes! Check out our <a href="https://www.trashmob.eco/donate">Donate</a> page!',
            },
            {
                question: 'Does signing up for TrashMob cost anything?',
                answer: 'No! The TrashMob website is free to use and there’s no cost associated with setting up or joining TrashMobs.',
            },
            {
                question: 'Can I report a website bug?',
                answer: 'Yes, you can! Please <a href="https://www.trashmob.eco/contactus">Contact us</a> and let us know what you found. Thank you!',
            },
            {
                question: 'Is the TrashMob team looking for support?',
                answer: 'Yes! We are always looking for new team members to help us improve TrashMob.eco and implement new features. If you or someone you know has any skills in design, back or front-end development, program management, or applicable non-technical skills, please Contact us!',
            },
            {
                question: 'Does TrashMob have a mobile app?',
                answer: 'Yes! The TrashMob app is available for both iOS and Android devices. Download it from the <a href="https://apps.apple.com/app/trashmob/id1599983258" target="_blank" rel="noopener noreferrer">App Store</a> or <a href="https://play.google.com/store/apps/details?id=eco.trashmob.trashmobmobileapp" target="_blank" rel="noopener noreferrer">Google Play</a> to find events, report litter, and track your impact on the go!',
            },
        ],
    },
];

export const Faq: React.FC = () => {
    const [selectedGroupKey, setSelectedGroupKey] = React.useState<string>('Events');
    const faqGroup = faqs.find((group) => group.category === selectedGroupKey);

    return (
        <div>
            <HeroSection Title='FAQ' Description='We’ve got you covered.' />
            <div className='container mx-auto my-5 pt-5 space-y-4'>
                <div className='flex flex-col sm:flex-row gap-4'>
                    {faqs.map((faqGroup, ind) => (
                        <Card
                            key={ind}
                            className={`grow basis-1/3 ${selectedGroupKey === faqGroup.category && 'border-primary'}`}
                            onClick={() => setSelectedGroupKey(faqGroup.category)}
                        >
                            <CardHeader>
                                <CardTitle>{faqGroup.category}</CardTitle>
                                <CardDescription>{faqGroup.desc}</CardDescription>
                            </CardHeader>
                            <div className='flex justify-end'>
                                <img src={faqGroup.icon} alt='icon' className='mt-0 max-w-24' />
                            </div>
                        </Card>
                    ))}
                </div>
                {faqGroup ? (
                    <Card>
                        <CardHeader>
                            <CardTitle>{faqGroup.category}</CardTitle>
                        </CardHeader>
                        <CardContent className='flex flex-col gap-4'>
                            {faqGroup.questions.map((item, i) => (
                                <div className='prose' key={`q-${i}`}>
                                    <h5>{item.question}</h5>
                                    <p
                                        className='text-base text-foreground/75'
                                        dangerouslySetInnerHTML={{ __html: item.answer }}
                                    />
                                </div>
                            ))}
                        </CardContent>
                    </Card>
                ) : null}
            </div>
            <div className='bg-card py-12 text-center space-y-4'>
                <h1 className='font-medium text-4xl text-center'>Are we missing something?</h1>
                <h4 className='text-center'>Let us know by reaching out.</h4>
                <Button asChild>
                    <Link to='/contactus'>Contact Us</Link>
                </Button>
            </div>
        </div>
    );
};
