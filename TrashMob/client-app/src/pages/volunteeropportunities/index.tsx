import * as React from 'react';
import { Link } from 'react-router';
import { Button } from '@/components/ui/button';
import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible';

export const VolunteerOpportunities: React.FC = () => {
    const opportunities = [
        {
            title: 'UX/UI designer',
            shortDesc: 'Design for mobile app and website',
            preferredSkill: 'Figma, Responsive Web App Design, Mobile App Design',
            desc: `
        We've got a lot of features in the backlog we'd love to get done, but it all starts with
        design. If you have UX Design skills and want to help shape the website and mobile app
        as we continue to grow, send us a note, and we'll put your skills to good use!
      `,
        },
        {
            title: 'Web developer',
            shortDesc: 'Develop website with React JS and .NETCore',
            preferredSkill: 'ReactJS, C#, CSS, Azure Maps, AzureAD B2C, Github',
            desc: `
        The backlog of features we'd like to get done on the website is huge. Some are critical
        to the way we do help communities get onboarded to TrashMob.eco. Others add fun
        components that encourage more and repeated volunteer participation. If you've got the
        skills to turn UX Designs done in Figma into ReactJS code, or the skills to improve the
        performance, security, or maintainability of the backend services, we can always use
        good developers who are passionate about the environment and willing to learn.
      `,
        },
        {
            title: 'Mobile developer',
            shortDesc: 'Develop mobile app with .NET MAUI',
            preferredSkill: '.NET MAUI, Mobile Deployments to Apple Store and Google Play Store, C#',
            desc: `
        We're actively working on a TrashMob.eco mobile app, and could use more developers who
        know their way around building, testing, and deploying mobile app to the Apple and
        Google Play stores. We've got lots of cool features we want to add to the app, and need
        help getting all this done. If you have .NET MAUI skills, or want to learn, this is a
        great opportunity for developers passionate about the environment to dig in and lend a
        hand!
      `,
        },
        {
            title: 'Mobile product manager',
            shortDesc: 'Manage mobile app development',
            preferredSkill: 'Managing Mobile Deployments to Apple Store and Google Play Store, Mobile App Design',
            desc: `
        Getting a mobile app out the door involves more than just developers checking in code.
        Making sure all the boxes are checked on the Apple and Google Play stores is a lot of
        work. We're looking for someone who can drive the mobile app through the app stores, and
        work with the designers and devs to plot out a release strategy for more features and
        more bells and whistles. If this is something you've done before, and you'd love to help
        a non-profit get it's mobile app launched, send us a note!
      `,
        },
    ];

    return (
        <div className='tailwind'>
            <HeroSection Title='Recruiting' Description='We’d love to have you join us.' />
            <div className='container mx-auto'>
                <div className='grid grid-cols-12 gap-4'>
                    <div className='col-span-12'>
                        <div className='flex justify-between items-center my-4'>
                            <h1 className='m-0'>Open volunteer positions (4)</h1>
                            <Button asChild>
                                <Link to='/contactus'>Contact us</Link>
                            </Button>
                        </div>
                    </div>
                    <div className='col-span-12 lg:col-span-4'>
                        <Card>
                            <CardHeader>
                                <CardTitle>Looking to contribute to the growth of TrashMob.eco?</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <p className='p-18'>
                                    There are many ways to get involved in the growth of TrashMob.eco besides picking
                                    litter.
                                </p>
                                <p className='p-18'>
                                    On this page are a few ways you can contribute from the comfort of your own home! We
                                    encourage you to reach out even if you don't have all the preferred skills.
                                </p>
                                <p className='p-18'>
                                    If you are interested in any of these opportunities, contact us at{' '}
                                </p>
                                <p className='p-18 color-primary'>info@trashmob.eco.</p>
                            </CardContent>
                        </Card>
                    </div>
                    <div className='col-span-12 lg:col-span-8'>
                        {opportunities.map((opp, index) => (
                            <Card className='mb-4' key={`opportunity-${index}`}>
                                <CardHeader>
                                    <CardTitle>{opp.title}</CardTitle>
                                </CardHeader>
                                <CardContent>
                                    <p>{opp.shortDesc}</p>
                                    <p>Preferred skills: {opp.preferredSkill}</p>
                                    <Collapsible className='group'>
                                        <CollapsibleContent>{opp.desc}</CollapsibleContent>
                                        <CollapsibleTrigger>
                                            <span className='text-primary group-data-[state=open]:hidden'>
                                                See more
                                            </span>
                                            <span className='text-primary group-data-[state=closed]:hidden'>
                                                See less
                                            </span>
                                        </CollapsibleTrigger>
                                    </Collapsible>
                                </CardContent>
                            </Card>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
};
