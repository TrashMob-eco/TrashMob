import { useRef, useState } from 'react';
import { AdvancedMarker, InfoWindow, MapProps } from '@vis.gl/react-google-maps';
import { GoogleMapWithKey as GoogleMap } from '../Map/GoogleMap';
import { CommunityInfoWindowHeader, CommunityInfoWindowContent } from './community-info-window';
import CommunityData from '../Models/CommunityData';
import { useIsInViewport } from '@/hooks/useIsInViewport';
import { cn } from '@/lib/utils';
import { CommunityPin } from './community-pin';

const communityColor = '#059669'; // Emerald for communities

interface CommunitiesMapProps extends MapProps {
    id?: string;
    communities: CommunityData[];
}

export const CommunitiesMap = (props: CommunitiesMapProps) => {
    const { id, communities, gestureHandling, ...rest } = props;

    // Filter communities that have valid coordinates
    const communitiesWithLocation = communities.filter(
        (community) =>
            community.latitude !== null &&
            community.longitude !== null &&
            community.latitude !== 0 &&
            community.longitude !== 0,
    );

    const markersRef = useRef<Record<string, google.maps.marker.AdvancedMarkerElement>>({});
    const [showingCommunitySlug, setShowingCommunitySlug] = useState<string>('');
    const showingCommunity = communitiesWithLocation.find((community) => community.slug === showingCommunitySlug);
    const { ref, isInViewPort } = useIsInViewport();

    const handleMarkerHover = (slug: string) => {
        setShowingCommunitySlug(slug);
    };

    // Calculate center from communities or use default
    const getDefaultCenter = () => {
        if (communitiesWithLocation.length === 0) {
            return { lat: 39.8283, lng: -98.5795 }; // Center of US
        }

        const avgLat =
            communitiesWithLocation.reduce((sum, community) => sum + (community.latitude || 0), 0) /
            communitiesWithLocation.length;
        const avgLng =
            communitiesWithLocation.reduce((sum, community) => sum + (community.longitude || 0), 0) /
            communitiesWithLocation.length;
        return { lat: avgLat, lng: avgLng };
    };

    return (
        <div ref={ref}>
            <GoogleMap
                id={id}
                gestureHandling={gestureHandling}
                defaultCenter={getDefaultCenter()}
                defaultZoom={communitiesWithLocation.length > 0 ? 4 : 3}
                {...rest}
            >
                {communitiesWithLocation.map((community) => (
                    <AdvancedMarker
                        key={community.slug}
                        ref={(el) => {
                            markersRef.current[community.slug] = el!;
                        }}
                        className={cn({
                            'animate-[bounce_1s_both_3s]': isInViewPort,
                        })}
                        position={{ lat: community.latitude!, lng: community.longitude! }}
                        onMouseEnter={() => handleMarkerHover(community.slug)}
                    >
                        <CommunityPin color={communityColor} size={40} />
                    </AdvancedMarker>
                ))}

                {showingCommunity ? (
                    <InfoWindow
                        anchor={markersRef.current[showingCommunitySlug]}
                        headerContent={
                            <CommunityInfoWindowHeader
                                name={showingCommunity.name}
                                bannerUrl={showingCommunity.bannerImageUrl}
                            />
                        }
                        onClose={() => setShowingCommunitySlug('')}
                    >
                        <CommunityInfoWindowContent community={showingCommunity} />
                    </InfoWindow>
                ) : null}
            </GoogleMap>
        </div>
    );
};
