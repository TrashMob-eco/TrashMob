import { useParams, Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { MapPin, ArrowLeft, Loader2, ExternalLink, Building2 } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import CommunityData from '@/components/Models/CommunityData';
import { GetCommunityBySlug } from '@/services/communities';

const getLocation = (community: CommunityData) => {
    const parts = [community.city, community.region, community.country].filter(Boolean);
    return parts.join(', ') || 'Location not specified';
};

export const CommunityDetailPage = () => {
    const { slug } = useParams<{ slug: string }>() as { slug: string };

    const { data: community, isLoading } = useQuery<AxiosResponse<CommunityData>, unknown, CommunityData>({
        queryKey: GetCommunityBySlug({ slug }).key,
        queryFn: GetCommunityBySlug({ slug }).service,
        select: (res) => res.data,
        enabled: !!slug,
    });

    if (isLoading) {
        return (
            <div>
                <HeroSection Title='Community' Description='Loading...' />
                <div className='container py-8 text-center'>
                    <Loader2 className='h-8 w-8 animate-spin mx-auto' />
                </div>
            </div>
        );
    }

    if (!community) {
        return (
            <div>
                <HeroSection Title='Community' Description='Not Found' />
                <div className='container py-8 text-center'>
                    <p className='mb-4'>This community could not be found.</p>
                    <Button asChild>
                        <Link to='/communities'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Communities
                        </Link>
                    </Button>
                </div>
            </div>
        );
    }

    // Apply custom branding colors
    const brandingStyles = {
        '--community-primary': community.brandingPrimaryColor || '#3B82F6',
        '--community-secondary': community.brandingSecondaryColor || '#1E40AF',
    } as React.CSSProperties;

    return (
        <div style={brandingStyles}>
            {/* Custom banner if available */}
            {community.bannerImageUrl ? (
                <div
                    className='h-64 bg-cover bg-center'
                    style={{ backgroundImage: `url(${community.bannerImageUrl})` }}
                >
                    <div className='h-full bg-black/40 flex items-end'>
                        <div className='container pb-8'>
                            <h1 className='text-4xl font-bold text-white'>{community.name}</h1>
                            {community.tagline ? <p className='text-xl text-white/90 mt-2'>{community.tagline}</p> : null}
                        </div>
                    </div>
                </div>
            ) : (
                <HeroSection Title={community.name} Description={community.tagline || 'Community details'} />
            )}

            <div className='container py-8'>
                <div className='mb-4'>
                    <Button variant='outline' asChild>
                        <Link to='/communities'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Communities
                        </Link>
                    </Button>
                </div>

                <div className='grid grid-cols-1 lg:grid-cols-3 gap-6'>
                    {/* Main content */}
                    <div className='lg:col-span-2 space-y-6'>
                        <Card>
                            <CardHeader>
                                <div className='flex items-center gap-4'>
                                    {community.bannerImageUrl ? (
                                        <img
                                            src={community.bannerImageUrl}
                                            alt={`${community.name} logo`}
                                            className='w-16 h-16 rounded-lg object-cover border'
                                        />
                                    ) : (
                                        <div className='w-16 h-16 rounded-lg bg-muted flex items-center justify-center'>
                                            <Building2 className='h-8 w-8 text-muted-foreground' />
                                        </div>
                                    )}
                                    <CardTitle className='text-2xl'>{community.name}</CardTitle>
                                </div>
                            </CardHeader>
                            <CardContent>
                                <div className='space-y-4'>
                                    {community.publicNotes ? (
                                        <div>
                                            <h3 className='font-semibold mb-2'>About</h3>
                                            <p className='text-muted-foreground whitespace-pre-wrap'>
                                                {community.publicNotes}
                                            </p>
                                        </div>
                                    ) : null}
                                    <div className='flex items-center gap-2 text-muted-foreground'>
                                        <MapPin className='h-4 w-4' />
                                        <span>{getLocation(community)}</span>
                                    </div>
                                </div>
                            </CardContent>
                        </Card>

                        {/* Future: Events in this community */}
                        {/* Future: Teams in this community */}
                    </div>

                    {/* Sidebar */}
                    <div className='space-y-6'>
                        <Card>
                            <CardHeader>
                                <CardTitle>Community Info</CardTitle>
                            </CardHeader>
                            <CardContent className='space-y-3'>
                                <div className='flex justify-between'>
                                    <span className='text-muted-foreground'>Location</span>
                                    <span className='font-medium text-right'>{getLocation(community)}</span>
                                </div>
                                {community.website ? <div className='flex justify-between items-center'>
                                        <span className='text-muted-foreground'>Website</span>
                                        <a
                                            href={community.website}
                                            target='_blank'
                                            rel='noopener noreferrer'
                                            className='flex items-center gap-1 text-primary hover:underline'
                                        >
                                            Visit <ExternalLink className='h-3 w-3' />
                                        </a>
                                    </div> : null}
                            </CardContent>
                        </Card>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default CommunityDetailPage;
