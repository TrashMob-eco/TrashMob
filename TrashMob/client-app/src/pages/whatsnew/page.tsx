import * as React from 'react';
import { Link } from 'react-router';
import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import {
    Users,
    Building2,
    Trophy,
    Scale,
    MapPin,
    UserCheck,
    MessageSquare,
    Map,
    Camera,
    Mail,
    FileText,
    Wrench,
    Shield,
    Route,
    Smartphone,
    Newspaper,
    BarChart3,
} from 'lucide-react';

interface FeatureCardProps {
    icon: React.ReactNode;
    title: string;
    description: string;
    link?: string;
    linkText?: string;
}

const FeatureCard: React.FC<FeatureCardProps> = ({ icon, title, description, link, linkText }) => (
    <Card className='h-full'>
        <CardHeader className='pb-3'>
            <div className='flex items-center gap-3'>
                <div className='p-2 bg-primary/10 rounded-lg text-primary'>{icon}</div>
                <CardTitle className='text-lg'>{title}</CardTitle>
            </div>
        </CardHeader>
        <CardContent>
            <CardDescription className='text-sm text-foreground/75'>{description}</CardDescription>
            {link ? (
                <Button asChild variant='link' className='px-0 mt-2'>
                    <Link to={link}>{linkText || 'Learn more'}</Link>
                </Button>
            ) : null}
        </CardContent>
    </Card>
);

const completedFeatures = [
    {
        icon: <Users className='h-5 w-5' />,
        title: 'Volunteer Teams',
        description:
            'Create or join teams to organize cleanups with friends, coworkers, or neighbors. Track your collective impact and build lasting volunteer communities.',
        link: '/teams',
        linkText: 'Browse Teams',
    },
    {
        icon: <Building2 className='h-5 w-5' />,
        title: 'Community Pages',
        description:
            'Cities and organizations can now have branded community pages showcasing local events, teams, and impact statistics. A central hub for regional cleanup efforts.',
        link: '/communities',
        linkText: 'Explore Communities',
    },
    {
        icon: <Trophy className='h-5 w-5' />,
        title: 'Leaderboards & Achievements',
        description:
            'See how you stack up! Track top volunteers by bags collected, events attended, and hours contributed. Earn achievement badges for reaching milestones.',
        link: '/leaderboards',
        linkText: 'View Leaderboards',
    },
    {
        icon: <Scale className='h-5 w-5' />,
        title: 'Weight Tracking',
        description:
            "Record the actual weight of litter collected at events. Get more accurate impact data beyond just bag counts to show the real difference you're making.",
    },
    {
        icon: <MapPin className='h-5 w-5' />,
        title: 'Adopt-A-Location',
        description:
            'Claim a spot to keep clean on an ongoing basis. Perfect for adopting a park, street corner, or trail that you pass regularly.',
    },
    {
        icon: <UserCheck className='h-5 w-5' />,
        title: 'Event Co-Leads',
        description:
            'Share event management with multiple co-leads. Delegate responsibilities so events can run smoothly even if the original organizer is unavailable.',
    },
    {
        icon: <MessageSquare className='h-5 w-5' />,
        title: 'In-App Feedback',
        description:
            'Share your ideas and report issues directly from the app. Your feedback helps us prioritize improvements and squash bugs faster.',
    },
    {
        icon: <Map className='h-5 w-5' />,
        title: 'Partner Location Map',
        description:
            'Find partner drop-off points, supply pickup locations, and service areas on an interactive map. Get the support you need for your cleanup events.',
    },
    {
        icon: <Camera className='h-5 w-5' />,
        title: 'Photo Moderation',
        description:
            'Enhanced safety with admin review of uploaded photos. Keeps inappropriate content off the platform while showcasing your cleanup achievements.',
    },
    {
        icon: <Mail className='h-5 w-5' />,
        title: 'Bulk Email Invites',
        description:
            'Invite multiple people to your events at once. Great for spreading the word to groups, clubs, or organization mailing lists.',
    },
    {
        icon: <FileText className='h-5 w-5' />,
        title: 'Web Litter Reports',
        description:
            'Report litter locations directly from the web app with the same features as mobile. Spot something? Report it so others can help clean it up.',
        link: '/litterreports/create',
        linkText: 'Report Litter',
    },
];

const upcomingFeatures = [
    {
        icon: <Shield className='h-5 w-5' />,
        title: 'Enhanced Waivers',
        description:
            'Community-specific waivers, improved minor coverage, and streamlined signing process. Better protection for volunteers and partners.',
        status: 'In Progress',
    },
    {
        icon: <Newspaper className='h-5 w-5' />,
        title: 'Monthly Newsletter',
        description:
            'Stay connected with TrashMob news, featured events, and volunteer spotlights delivered to your inbox.',
        status: 'In Progress',
    },
    {
        icon: <BarChart3 className='h-5 w-5' />,
        title: 'Attendee Metrics',
        description:
            'Track individual volunteer contributions per event. See who collected the most, stayed the longest, and made the biggest impact.',
        status: 'In Progress',
    },
    {
        icon: <Wrench className='h-5 w-5' />,
        title: 'Modern Authentication',
        description:
            'Migrating to Microsoft Entra External ID for improved security, SSO support, and better partner integrations.',
        status: 'Planning',
    },
    {
        icon: <Route className='h-5 w-5' />,
        title: 'Route Tracing',
        description:
            'Track the routes you walk during cleanups. Visualize coverage, avoid duplicate efforts, and see area completion over time.',
        status: 'Planning',
    },
    {
        icon: <Smartphone className='h-5 w-5' />,
        title: 'Mobile Improvements',
        description:
            'Stability improvements, better error handling, and new features coming to the iOS and Android apps.',
        status: 'In Development',
    },
];

export const WhatsNew: React.FC = () => {
    return (
        <div>
            <HeroSection Title="What's New" Description='See what we have been working on in 2026!' />

            <section className='container py-12'>
                <h2 className='font-semibold text-3xl text-center mb-3'>New Features in 2026</h2>
                <p className='text-center text-muted-foreground max-w-2xl mx-auto mb-8'>
                    We've been busy building features to help you make an even bigger impact. Here's what's new:
                </p>
                <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6'>
                    {completedFeatures.map((feature, index) => (
                        <FeatureCard key={index} {...feature} />
                    ))}
                </div>
            </section>

            <section className='bg-card py-12'>
                <div className='container'>
                    <h2 className='font-semibold text-3xl text-center mb-3'>In The Works</h2>
                    <p className='text-center text-muted-foreground max-w-2xl mx-auto mb-8'>
                        Here's a peek at what our team is currently building. These features are coming soon!
                    </p>
                    <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6'>
                        {upcomingFeatures.map((feature, index) => (
                            <Card key={index} className='h-full border-dashed'>
                                <CardHeader className='pb-3'>
                                    <div className='flex items-center justify-between'>
                                        <div className='flex items-center gap-3'>
                                            <div className='p-2 bg-muted rounded-lg text-muted-foreground'>
                                                {feature.icon}
                                            </div>
                                            <CardTitle className='text-lg'>{feature.title}</CardTitle>
                                        </div>
                                        <span className='text-xs bg-primary/10 text-primary px-2 py-1 rounded-full'>
                                            {feature.status}
                                        </span>
                                    </div>
                                </CardHeader>
                                <CardContent>
                                    <CardDescription className='text-sm text-foreground/75'>
                                        {feature.description}
                                    </CardDescription>
                                </CardContent>
                            </Card>
                        ))}
                    </div>
                </div>
            </section>

            <section className='py-12'>
                <div className='container text-center'>
                    <h2 className='font-semibold text-2xl mb-4'>Have an idea?</h2>
                    <p className='text-muted-foreground mb-6'>
                        We love hearing from our community. Share your feature ideas or report issues.
                    </p>
                    <div className='flex flex-wrap justify-center gap-4'>
                        <Button asChild>
                            <Link to='/contactus'>Contact Us</Link>
                        </Button>
                        <Button asChild variant='outline'>
                            <a
                                href='https://github.com/TrashMob-eco/TrashMob/issues'
                                target='_blank'
                                rel='noopener noreferrer'
                            >
                                GitHub Issues
                            </a>
                        </Button>
                    </div>
                </div>
            </section>
        </div>
    );
};
