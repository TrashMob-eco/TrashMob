import { useEffect, useRef } from 'react';
import { useMap } from '@vis.gl/react-google-maps';

interface CommunityBoundsOverlayProps {
    boundsNorth: number;
    boundsSouth: number;
    boundsEast: number;
    boundsWest: number;
    mapId: string;
}

export const CommunityBoundsOverlay = ({
    boundsNorth,
    boundsSouth,
    boundsEast,
    boundsWest,
    mapId,
}: CommunityBoundsOverlayProps) => {
    const map = useMap(mapId);
    const rectangleRef = useRef<google.maps.Rectangle | null>(null);

    useEffect(() => {
        if (!map) return;

        if (!rectangleRef.current) {
            rectangleRef.current = new google.maps.Rectangle({
                map,
                strokeColor: '#9CA3AF',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#9CA3AF',
                fillOpacity: 0.05,
                clickable: false,
            });
        }

        rectangleRef.current.setBounds({
            north: boundsNorth,
            south: boundsSouth,
            east: boundsEast,
            west: boundsWest,
        });

        return () => {
            if (rectangleRef.current) {
                rectangleRef.current.setMap(null);
                rectangleRef.current = null;
            }
        };
    }, [map, boundsNorth, boundsSouth, boundsEast, boundsWest]);

    return null;
};
