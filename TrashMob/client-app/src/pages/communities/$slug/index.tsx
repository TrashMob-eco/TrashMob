import { useState } from 'react';
import { useParams, Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { MapPin, ArrowLeft, Loader2, ExternalLink, Building2, Settings, Share2 } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { useLogin } from '@/hooks/useLogin';
import CommunityData from '@/components/Models/CommunityData';
import EventData from '@/components/Models/EventData';
import TeamData from '@/components/Models/TeamData';
import LitterReportData from '@/components/Models/LitterReportData';
import StatsData from '@/components/Models/StatsData';
import {
    GetCommunityBySlug,
    GetCommunityEvents,
    GetCommunityTeams,
    GetCommunityLitterReports,
    GetCommunityStats,
} from '@/services/communities';
import { GetAvailableAreas } from '@/services/adoptable-areas';
import AdoptableAreaData from '@/components/Models/AdoptableAreaData';
import { CommunityStatsWidget } from '@/components/communities/community-stats-widget';
import { CommunityContactCard } from '@/components/communities/community-contact-card';
import { CommunityEventsSection } from '@/components/communities/community-events-section';
import { CommunityTeamsSection } from '@/components/communities/community-teams-section';
import { CommunityAreasSection } from '@/components/communities/community-areas-section';
import { CommunityDetailMap } from '@/components/communities/community-detail-map';
import { CommunityPhotoGallery } from '@/components/communities/CommunityPhotoGallery';
import { CommunityPhotoUploader } from '@/components/communities/CommunityPhotoUploader';
import { ShareDialog } from '@/components/sharing';
import { getCommunityShareableContent, getCommunityShareMessage } from '@/lib/sharing-messages';
import { getLocation, getRegionTypeLabel } from '@/lib/community-utils';

export const CommunityDetailPage = () => {
    const { slug } = useParams<{ slug: string }>() as { slug: string };
    const { currentUser, isUserLoaded } = useLogin();
    const [showPhotoUploader, setShowPhotoUploader] = useState(false);
    const [showShareDialog, setShowShareDialog] = useState(false);

    const { data: community, isLoading } = useQuery<AxiosResponse<CommunityData>, unknown, CommunityData>({
        queryKey: GetCommunityBySlug({ slug }).key,
        queryFn: GetCommunityBySlug({ slug }).service,
        select: (res) => res.data,
        enabled: !!slug,
    });

    const { data: events = [], isLoading: eventsLoading } = useQuery<AxiosResponse<EventData[]>, unknown, EventData[]>({
        queryKey: GetCommunityEvents({ slug }).key,
        queryFn: GetCommunityEvents({ slug }).service,
        select: (res) => res.data,
        enabled: !!slug && !!community,
    });

    const { data: teams = [], isLoading: teamsLoading } = useQuery<AxiosResponse<TeamData[]>, unknown, TeamData[]>({
        queryKey: GetCommunityTeams({ slug }).key,
        queryFn: GetCommunityTeams({ slug }).service,
        select: (res) => res.data,
        enabled: !!slug && !!community,
    });

    const { data: litterReports = [] } = useQuery<AxiosResponse<LitterReportData[]>, unknown, LitterReportData[]>({
        queryKey: GetCommunityLitterReports({ slug }).key,
        queryFn: GetCommunityLitterReports({ slug }).service,
        select: (res) => res.data,
        enabled: !!slug && !!community,
    });

    const { data: stats } = useQuery<AxiosResponse<StatsData>, unknown, StatsData>({
        queryKey: GetCommunityStats({ slug }).key,
        queryFn: GetCommunityStats({ slug }).service,
        select: (res) => res.data,
        enabled: !!slug && !!community,
    });

    const { data: areas = [], isLoading: areasLoading } = useQuery<
        AxiosResponse<AdoptableAreaData[]>,
        unknown,
        AdoptableAreaData[]
    >({
        queryKey: GetAvailableAreas({ partnerId: community?.id ?? '' }).key,
        queryFn: GetAvailableAreas({ partnerId: community?.id ?? '' }).service,
        select: (res) => res.data,
        enabled: !!community?.id,
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

    const hasLocation = community.latitude && community.longitude;

    return (
        <div style={brandingStyles}>
            {/* Custom banner if available */}
            {community.bannerImageUrl ? (
                <div
                    className='h-64 bg-cover bg-center relative'
                    style={{ backgroundImage: `url(${community.bannerImageUrl})` }}
                >
                    <div className='h-full bg-black/40 flex items-end'>
                        <div className='container pb-8 flex items-end gap-4'>
                            {community.logoUrl ? (
                                <img
                                    src={community.logoUrl}
                                    alt={`${community.name} logo`}
                                    className='w-20 h-20 rounded-lg object-cover border-2 border-white shadow-lg'
                                />
                            ) : null}
                            <div>
                                <h1 className='text-4xl font-bold text-white'>{community.name}</h1>
                                {community.tagline ? (
                                    <p className='text-xl text-white/90 mt-2'>{community.tagline}</p>
                                ) : null}
                            </div>
                        </div>
                    </div>
                </div>
            ) : (
                <HeroSection Title={community.name} Description={community.tagline || 'Community details'} />
            )}

            <div className='container py-8'>
                <div className='mb-4 flex justify-between items-center'>
                    <Button variant='outline' asChild>
                        <Link to='/communities'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Communities
                        </Link>
                    </Button>
                    <div className='flex gap-2'>
                        <Button variant='outline' onClick={() => setShowShareDialog(true)}>
                            <Share2 className='h-4 w-4 mr-2' /> Share
                        </Button>
                        {isUserLoaded && currentUser.id ? (
                            <Button variant='outline' asChild>
                                <Link to={`/partnerdashboard/${community?.id}/community`}>
                                    <Settings className='h-4 w-4 mr-2' /> Admin
                                </Link>
                            </Button>
                        ) : null}
                    </div>
                </div>

                <div className='grid grid-cols-1 lg:grid-cols-3 gap-6'>
                    {/* Main content */}
                    <div className='lg:col-span-2 space-y-6'>
                        {/* About Card */}
                        <Card>
                            <CardHeader>
                                <div className='flex items-center gap-4'>
                                    {community.logoUrl ? (
                                        <img
                                            src={community.logoUrl}
                                            alt={`${community.name} logo`}
                                            className='w-16 h-16 rounded-lg object-cover border'
                                        />
                                    ) : community.bannerImageUrl ? (
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
                                        {getRegionTypeLabel(community.regionType) ? (
                                            <Badge variant='secondary'>
                                                {getRegionTypeLabel(community.regionType)} Community
                                            </Badge>
                                        ) : null}
                                    </div>
                                </div>
                            </CardContent>
                        </Card>

                        {/* Community Map */}
                        {hasLocation ? (
                            <CommunityDetailMap
                                events={events}
                                teams={teams}
                                litterReports={litterReports}
                                centerLat={community.latitude!}
                                centerLng={community.longitude!}
                                boundsNorth={community.boundsNorth}
                                boundsSouth={community.boundsSouth}
                                boundsEast={community.boundsEast}
                                boundsWest={community.boundsWest}
                                boundaryGeoJson={community.boundaryGeoJson}
                            />
                        ) : null}

                        {/* Events Section */}
                        <CommunityEventsSection events={events} isLoading={eventsLoading} />

                        {/* Teams Section */}
                        <CommunityTeamsSection teams={teams} isLoading={teamsLoading} />

                        {/* Adoptable Areas Section */}
                        <CommunityAreasSection
                            areas={areas}
                            isLoading={areasLoading}
                            communityId={community.id}
                            boundaryGeoJson={community.boundaryGeoJson}
                        />

                        {/* Photo Gallery Section */}
                        <CommunityPhotoGallery
                            slug={slug}
                            canUpload={isUserLoaded ? !!currentUser.id : null}
                            canDelete={isUserLoaded ? !!currentUser.id : null}
                            onUploadClick={() => setShowPhotoUploader(true)}
                        />
                    </div>

                    {/* Sidebar */}
                    <div className='space-y-6'>
                        {/* Stats Widget */}
                        {stats ? <CommunityStatsWidget stats={stats} /> : null}

                        {/* Contact Card */}
                        <CommunityContactCard
                            contactEmail={community.contactEmail}
                            contactPhone={community.contactPhone}
                            physicalAddress={community.physicalAddress}
                            website={community.website}
                        />

                        {/* Community Info Card */}
                        <Card>
                            <CardHeader>
                                <CardTitle>Community Info</CardTitle>
                            </CardHeader>
                            <CardContent className='space-y-3'>
                                <div className='flex justify-between'>
                                    <span className='text-muted-foreground'>Location</span>
                                    <span className='font-medium text-right'>{getLocation(community)}</span>
                                </div>
                                {community.website ? (
                                    <div className='flex justify-between items-center'>
                                        <span className='text-muted-foreground'>Website</span>
                                        <a
                                            href={
                                                community.website.startsWith('http')
                                                    ? community.website
                                                    : `https://${community.website}`
                                            }
                                            target='_blank'
                                            rel='noopener noreferrer'
                                            className='flex items-center gap-1 text-primary hover:underline'
                                        >
                                            Visit <ExternalLink className='h-3 w-3' />
                                        </a>
                                    </div>
                                ) : null}
                            </CardContent>
                        </Card>
                    </div>
                </div>
            </div>

            {/* Photo Uploader Modal */}
            {isUserLoaded && currentUser.id ? (
                <CommunityPhotoUploader slug={slug} open={showPhotoUploader} onOpenChange={setShowPhotoUploader} />
            ) : null}

            {/* Share Dialog */}
            <ShareDialog
                content={getCommunityShareableContent(community)}
                open={showShareDialog}
                onOpenChange={setShowShareDialog}
                message={getCommunityShareMessage(community)}
            />
        </div>
    );
};

export default CommunityDetailPage;
