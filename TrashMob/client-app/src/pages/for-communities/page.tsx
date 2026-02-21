import { FC, useState } from 'react';
import { Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { ChevronDown, Globe, Users, BarChart3, MapPin, Calendar, Mail, Shield, CheckCircle2 } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { GetFeaturedCommunities, GetCommunityPublicStats } from '@/services/communities';

// ============================================================================
// Value Proposition Cards
// ============================================================================

const valueProps = [
    {
        icon: Globe,
        title: 'Branded Community Page',
        description:
            'Get a custom-branded page for your city, county, or organization with your logo, colors, and messaging.',
    },
    {
        icon: Users,
        title: 'Recruit Volunteers',
        description:
            'Engage community members with event management, bulk invites, newsletters, and volunteer tracking tools.',
    },
    {
        icon: BarChart3,
        title: 'Impact Reporting',
        description:
            'Track events, bags collected, volunteer hours, and weight data with grant-ready analytics dashboards.',
    },
    {
        icon: MapPin,
        title: 'Adopt-a-Location',
        description:
            'Let volunteers and teams adopt specific areas with interactive maps, cleanup schedules, and compliance tracking.',
    },
];

// ============================================================================
// Feature Showcase Items
// ============================================================================

const features = [
    {
        icon: Globe,
        title: 'Custom Community Pages',
        description:
            'Your community gets a dedicated, branded page at trashmob.eco with your logo, colors, banner, and tagline. Showcase events, volunteer teams, litter reports, and impact stats all in one place.',
    },
    {
        icon: MapPin,
        title: 'Adopt-a-Location Programs',
        description:
            'Create adoptable areas with interactive maps and let volunteers or teams claim sections to maintain. Track compliance, set cleanup schedules, and manage sponsored adoptions from professional companies.',
    },
    {
        icon: BarChart3,
        title: 'Analytics & Heat Maps',
        description:
            'See your community impact at a glance with dashboards showing event counts, volunteer participation, bags collected, and weight data. Use heat maps to identify areas that need the most attention.',
    },
    {
        icon: Mail,
        title: 'Bulk Invites & Newsletters',
        description:
            'Grow your volunteer base with bulk email invitations. Send newsletters to keep your community engaged, announce upcoming events, and celebrate milestones.',
    },
    {
        icon: Calendar,
        title: 'Event Management',
        description:
            'Create and manage cleanup events with detailed location mapping, volunteer RSVPs, co-lead assignments, and post-event summaries including pickup location tracking and weight measurements.',
    },
    {
        icon: Shield,
        title: 'Sponsored Adoption Compliance',
        description:
            'Partner with local businesses as adoption sponsors. Track their compliance with cleanup commitments, generate reports, and manage professional company partnerships through a dedicated portal.',
    },
];

// ============================================================================
// Pricing Comparison
// ============================================================================

const pricingFeatures = [
    { name: 'Create and join events', free: true, community: true },
    { name: 'Track personal impact', free: true, community: true },
    { name: 'Litter reporting', free: true, community: true },
    { name: 'Join teams', free: true, community: true },
    { name: 'Branded community page', free: false, community: true },
    { name: 'Adopt-a-location programs', free: false, community: true },
    { name: 'Community analytics dashboard', free: false, community: true },
    { name: 'Bulk invites & newsletters', free: false, community: true },
    { name: 'Sponsored adoption management', free: false, community: true },
    { name: 'Professional company portal', free: false, community: true },
    { name: 'Custom branding & logo', free: false, community: true },
    { name: 'Grant-ready impact reports', free: false, community: true },
];

// ============================================================================
// FAQ Items
// ============================================================================

const faqItems = [
    {
        question: 'What is a TrashMob community?',
        answer: 'A TrashMob community is a city, county, nonprofit, or organization that partners with TrashMob.eco to get a branded presence on the platform. Communities get their own page, volunteer engagement tools, adopt-a-location programs, analytics dashboards, and more.',
    },
    {
        question: 'How much does it cost?',
        answer: 'TrashMob offers community subscriptions at different tiers. Contact us for current pricing information. All basic volunteer features (creating events, joining cleanups, tracking impact) remain free for everyone.',
    },
    {
        question: 'How long does setup take?',
        answer: 'After you submit your application, our team reviews it within a few business days. Once approved, we work with you to configure your branded page, set up your area boundaries, and customize your community settings. Most communities are up and running within a week of approval.',
    },
    {
        question: 'Can we customize our community page?',
        answer: 'Yes! You can customize your page with your organization logo, brand colors, banner image, tagline, and public description. Community admins have a dedicated dashboard to manage all content, areas, events, and volunteer programs.',
    },
    {
        question: 'What support do you provide?',
        answer: 'We provide onboarding assistance to help you set up your community page, configure adopt-a-location areas, and train your team on the platform. Ongoing support is available via email and our help center.',
    },
];

// ============================================================================
// FAQ Accordion Item
// ============================================================================

const FaqItem: FC<{ question: string; answer: string }> = ({ question, answer }) => {
    const [isOpen, setIsOpen] = useState(false);

    return (
        <div className='border-b'>
            <button
                type='button'
                className='flex w-full items-center justify-between py-4 text-left font-medium hover:underline'
                onClick={() => setIsOpen(!isOpen)}
            >
                {question}
                <ChevronDown className={`h-4 w-4 shrink-0 transition-transform ${isOpen ? 'rotate-180' : ''}`} />
            </button>
            {isOpen ? <div className='pb-4 text-muted-foreground'>{answer}</div> : null}
        </div>
    );
};

// ============================================================================
// Main Page Component
// ============================================================================

export const ForCommunitiesPage: FC = () => {
    const { data: featuredCommunities } = useQuery({
        queryKey: GetFeaturedCommunities().key,
        queryFn: GetFeaturedCommunities().service,
    });

    const { data: publicStats } = useQuery({
        queryKey: GetCommunityPublicStats().key,
        queryFn: GetCommunityPublicStats().service,
    });

    const hasFeaturedCommunities = featuredCommunities && featuredCommunities.length > 0;

    return (
        <div>
            {/* Hero Section with CTA */}
            <section className='bg-primary text-primary-foreground'>
                <div className='container relative py-10 md:py-14'>
                    <div className='relative z-10 max-w-2xl'>
                        <h1 className='font-bold text-2xl md:text-[40px] leading-tight md:leading-[50px] mb-3'>
                            Bring TrashMob to Your Community
                        </h1>
                        <p className='font-medium text-primary-foreground/90 mb-6'>
                            Organize cleanups, engage volunteers, and track your environmental impact with a branded
                            community page on TrashMob.eco.
                        </p>
                        <div className='flex flex-col sm:flex-row gap-3'>
                            <Button size='lg' variant='secondary' asChild>
                                <Link to='/becomeapartner?type=community'>Start Your Community</Link>
                            </Button>
                            <Button
                                size='lg'
                                variant='secondary'
                                className='bg-white text-primary hover:bg-white/90'
                                asChild
                            >
                                <Link to='/communities'>Browse Communities</Link>
                            </Button>
                        </div>
                    </div>
                </div>
            </section>

            {/* Value Proposition Cards */}
            <section className='py-16'>
                <div className='container'>
                    <h2 className='text-3xl font-bold text-center mb-2'>Why Partner with TrashMob?</h2>
                    <p className='text-center text-muted-foreground mb-10 max-w-2xl mx-auto'>
                        Everything you need to organize, track, and grow your community cleanup programs.
                    </p>
                    <div className='grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6'>
                        {valueProps.map((prop) => (
                            <Card key={prop.title} className='text-center'>
                                <CardHeader className='pb-2'>
                                    <div className='mx-auto mb-2 flex h-12 w-12 items-center justify-center rounded-full bg-primary/10'>
                                        <prop.icon className='h-6 w-6 text-primary' />
                                    </div>
                                    <CardTitle className='text-lg'>{prop.title}</CardTitle>
                                </CardHeader>
                                <CardContent>
                                    <p className='text-sm text-muted-foreground'>{prop.description}</p>
                                </CardContent>
                            </Card>
                        ))}
                    </div>
                </div>
            </section>

            {/* Public Stats (conditional) */}
            {publicStats && publicStats.totalCommunities > 0 ? (
                <section className='bg-primary text-primary-foreground py-12'>
                    <div className='container'>
                        <div className='grid grid-cols-1 sm:grid-cols-3 gap-8 text-center'>
                            <div>
                                <div className='text-4xl font-bold'>{publicStats.totalCommunities}</div>
                                <div className='text-primary-foreground/80'>Active Communities</div>
                            </div>
                            <div>
                                <div className='text-4xl font-bold'>{publicStats.totalCommunityEvents}</div>
                                <div className='text-primary-foreground/80'>Community Events</div>
                            </div>
                            <div>
                                <div className='text-4xl font-bold'>{publicStats.totalCommunityVolunteers}</div>
                                <div className='text-primary-foreground/80'>Volunteers Engaged</div>
                            </div>
                        </div>
                    </div>
                </section>
            ) : null}

            {/* Feature Showcase */}
            <section className='bg-card py-16'>
                <div className='container'>
                    <h2 className='text-3xl font-bold text-center mb-2'>Everything Your Community Needs</h2>
                    <p className='text-center text-muted-foreground mb-10 max-w-2xl mx-auto'>
                        A comprehensive platform for managing cleanups, engaging volunteers, and measuring impact.
                    </p>
                    <div className='grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8'>
                        {features.map((feature) => (
                            <div key={feature.title} className='space-y-3'>
                                <div className='flex items-center gap-3'>
                                    <div className='flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10'>
                                        <feature.icon className='h-5 w-5 text-primary' />
                                    </div>
                                    <h3 className='font-semibold text-lg'>{feature.title}</h3>
                                </div>
                                <p className='text-sm text-muted-foreground'>{feature.description}</p>
                            </div>
                        ))}
                    </div>
                </div>
            </section>

            {/* Pricing Comparison */}
            <section className='py-16'>
                <div className='container max-w-3xl'>
                    <h2 className='text-3xl font-bold text-center mb-2'>Free vs Community Tier</h2>
                    <p className='text-center text-muted-foreground mb-10'>
                        See what's included in a TrashMob community partnership.
                    </p>
                    <div className='overflow-x-auto'>
                        <table className='w-full'>
                            <thead>
                                <tr className='border-b'>
                                    <th className='text-left py-3 pr-4'>Feature</th>
                                    <th className='text-center py-3 px-4 w-28'>Free</th>
                                    <th className='text-center py-3 px-4 w-28 bg-primary/5 rounded-t-lg'>Community</th>
                                </tr>
                            </thead>
                            <tbody>
                                {pricingFeatures.map((feature) => (
                                    <tr key={feature.name} className='border-b'>
                                        <td className='py-3 pr-4 text-sm'>{feature.name}</td>
                                        <td className='text-center py-3 px-4'>
                                            {feature.free ? (
                                                <CheckCircle2 className='h-5 w-5 text-green-600 mx-auto' />
                                            ) : (
                                                <span className='text-muted-foreground'>—</span>
                                            )}
                                        </td>
                                        <td className='text-center py-3 px-4 bg-primary/5'>
                                            {feature.community ? (
                                                <CheckCircle2 className='h-5 w-5 text-green-600 mx-auto' />
                                            ) : (
                                                <span className='text-muted-foreground'>—</span>
                                            )}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            </section>

            {/* Community Success Stories / Placeholder */}
            <section className='bg-card py-16'>
                <div className='container'>
                    <h2 className='text-3xl font-bold text-center mb-2'>Community Success Stories</h2>
                    {hasFeaturedCommunities ? (
                        <>
                            <p className='text-center text-muted-foreground mb-10'>
                                See how communities across the country are using TrashMob to make a difference.
                            </p>
                            <div className='grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6'>
                                {featuredCommunities.map((community) => (
                                    <Card key={community.id}>
                                        <CardHeader className='text-center'>
                                            {community.logoUrl ? (
                                                <img
                                                    src={community.logoUrl}
                                                    alt={community.name}
                                                    className='h-16 w-16 mx-auto rounded-full object-cover mt-0'
                                                />
                                            ) : null}
                                            <CardTitle>{community.name}</CardTitle>
                                            {community.city && community.region ? (
                                                <p className='text-sm text-muted-foreground'>
                                                    {community.city}, {community.region}
                                                </p>
                                            ) : null}
                                        </CardHeader>
                                        <CardContent className='text-center'>
                                            {community.tagline ? (
                                                <p className='text-sm italic mb-4'>"{community.tagline}"</p>
                                            ) : null}
                                            <Button variant='outline' size='sm' asChild>
                                                <Link to={`/communities/${community.slug}`}>View Community</Link>
                                            </Button>
                                        </CardContent>
                                    </Card>
                                ))}
                            </div>
                        </>
                    ) : (
                        <div className='text-center max-w-lg mx-auto'>
                            <p className='text-muted-foreground mb-6'>
                                Be one of the first communities to join TrashMob! Early adopters help shape the platform
                                and get dedicated onboarding support.
                            </p>
                            <Button size='lg' asChild>
                                <Link to='/becomeapartner?type=community'>Be a Founding Community</Link>
                            </Button>
                        </div>
                    )}
                </div>
            </section>

            {/* Enrollment CTA */}
            <section className='bg-primary text-primary-foreground py-16'>
                <div className='container text-center'>
                    <h2 className='text-3xl font-bold mb-4'>Ready to Start Your Community?</h2>
                    <p className='mb-8 max-w-xl mx-auto text-primary-foreground/90'>
                        Submit an application and our team will review it within a few business days. We'll work with
                        you to set up your branded page and get you started.
                    </p>
                    <Button size='lg' variant='secondary' asChild>
                        <Link to='/becomeapartner?type=community'>Start Your Community</Link>
                    </Button>
                </div>
            </section>

            {/* FAQ Section */}
            <section className='py-16'>
                <div className='container max-w-3xl'>
                    <h2 className='text-3xl font-bold text-center mb-10'>Frequently Asked Questions</h2>
                    <div>
                        {faqItems.map((item) => (
                            <FaqItem key={item.question} question={item.question} answer={item.answer} />
                        ))}
                    </div>
                </div>
            </section>
        </div>
    );
};
