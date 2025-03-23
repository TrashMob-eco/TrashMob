import * as React from 'react';
import { Link } from 'react-router';
import events from '@/components/assets/faq/Event.svg';
import attendees from '@/components/assets/faq/Attendees.svg';
import volunteer from '@/components/assets/faq/volunteer.svg';
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
                answer: 'Not yet, but we are hard at work making one! Stay tuned!',
            },
        ],
    },
];

export const Faq: React.FC = () => {
    const [selectedGroupKey, setSelectedGroupKey] = React.useState<string>('Events');
    const faqGroup = faqs.find((group) => group.category === selectedGroupKey);

    return (
        <div className='tailwind'>
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
