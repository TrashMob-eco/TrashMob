import { useEffect, useRef } from 'react';
import { useMap } from '@vis.gl/react-google-maps';
import { GoogleMapWithKey as GoogleMap } from '../Map/GoogleMap';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { CommunityBoundsOverlay } from '@/components/Map/CommunityBoundsOverlay';

interface BoundsPreviewMapProps {
    centerLat: number;
    centerLng: number;
    boundsNorth?: number | null;
    boundsSouth?: number | null;
    boundsEast?: number | null;
    boundsWest?: number | null;
    /** GeoJSON polygon of the actual geographic boundary. */
    boundaryGeoJson?: string;
}

const MAP_ID = 'boundsPreviewMap';

/** Fallback: draws a simple rectangle when no GeoJSON polygon is available. */
const BoundsRectangle = ({
    boundsNorth,
    boundsSouth,
    boundsEast,
    boundsWest,
}: {
    boundsNorth: number;
    boundsSouth: number;
    boundsEast: number;
    boundsWest: number;
}) => {
    const map = useMap(MAP_ID);
    const rectangleRef = useRef<google.maps.Rectangle | null>(null);

    useEffect(() => {
        if (!map) return;

        if (!rectangleRef.current) {
            rectangleRef.current = new google.maps.Rectangle({
                map,
                strokeColor: '#3B82F6',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#3B82F6',
                fillOpacity: 0.15,
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

export const BoundsPreviewMap = (props: BoundsPreviewMapProps) => {
    const { centerLat, centerLng, boundsNorth, boundsSouth, boundsEast, boundsWest, boundaryGeoJson } = props;

    const hasBounds = boundsNorth != null && boundsSouth != null && boundsEast != null && boundsWest != null;
    const hasGeoJson = !!boundaryGeoJson;

    return (
        <Card>
            <CardHeader className='pb-3'>
                <CardTitle className='text-lg'>Boundary Preview</CardTitle>
            </CardHeader>
            <CardContent>
                <div className='h-[300px] rounded-lg overflow-hidden'>
                    <GoogleMap
                        id={MAP_ID}
                        gestureHandling='cooperative'
                        defaultCenter={{ lat: centerLat, lng: centerLng }}
                        {...(hasBounds
                            ? {
                                  defaultBounds: {
                                      north: boundsNorth!,
                                      south: boundsSouth!,
                                      east: boundsEast!,
                                      west: boundsWest!,
                                      padding: 40,
                                  },
                              }
                            : { defaultZoom: 11 })}
                    >
                        {hasGeoJson ? (
                            <CommunityBoundsOverlay mapId={MAP_ID} geoJson={boundaryGeoJson!} />
                        ) : hasBounds ? (
                            <BoundsRectangle
                                boundsNorth={boundsNorth!}
                                boundsSouth={boundsSouth!}
                                boundsEast={boundsEast!}
                                boundsWest={boundsWest!}
                            />
                        ) : null}
                    </GoogleMap>
                </div>
                {!hasBounds && !hasGeoJson ? (
                    <p className='text-sm text-muted-foreground mt-2'>
                        Use Detect Boundary to see a preview of your community&rsquo;s geographic area.
                    </p>
                ) : null}
            </CardContent>
        </Card>
    );
};
