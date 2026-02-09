import { useState, useCallback } from 'react';
import { ChevronDown, ChevronUp, MapPin } from 'lucide-react';
import { GoogleMapWithKey as GoogleMap } from '@/components/Map/GoogleMap';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { AreaMapToolbar, DrawingMode } from './AreaMapToolbar';
import { DrawingLayer } from './DrawingLayer';
import { CommunityBoundsOverlay } from './CommunityBoundsOverlay';
import { AreaBoundingBox } from '@/lib/geojson';

const MAP_ID = 'areaMapEditor';

interface CommunityBounds {
    boundsNorth: number | null;
    boundsSouth: number | null;
    boundsEast: number | null;
    boundsWest: number | null;
}

interface AreaMapEditorProps {
    value: string;
    onChange: (geoJson: string) => void;
    onBoundsChange?: (bbox: AreaBoundingBox | null) => void;
    communityBounds?: CommunityBounds;
    communityCenter?: { lat: number; lng: number } | null;
}

export const AreaMapEditor = ({ value, onChange, onBoundsChange, communityBounds, communityCenter }: AreaMapEditorProps) => {
    const [drawingMode, setDrawingMode] = useState<DrawingMode>(null);
    const [hasShape, setHasShape] = useState(false);
    const [showRawGeoJson, setShowRawGeoJson] = useState(false);

    const hasCommunityBounds =
        communityBounds?.boundsNorth != null &&
        communityBounds?.boundsSouth != null &&
        communityBounds?.boundsEast != null &&
        communityBounds?.boundsWest != null;

    const defaultCenter = communityCenter ?? { lat: 47.6062, lng: -122.3321 };

    const handleGeometryChange = useCallback(
        (geoJson: string, _centroid: { lat: number; lng: number }, bbox: AreaBoundingBox | null) => {
            onChange(geoJson);
            onBoundsChange?.(bbox);
        },
        [onChange, onBoundsChange],
    );

    const handleGeometryCleared = useCallback(() => {
        onChange('');
        onBoundsChange?.(null);
    }, [onChange, onBoundsChange]);

    const handleModeChange = useCallback((mode: DrawingMode) => {
        setDrawingMode(mode);
    }, []);

    const handleDelete = useCallback(() => {
        setDrawingMode('delete');
        // Reset to null after triggering delete
        setTimeout(() => setDrawingMode(null), 0);
    }, []);

    return (
        <div className='space-y-0'>
            <AreaMapToolbar activeMode={drawingMode} hasShape={hasShape} onModeChange={handleModeChange} onDelete={handleDelete} />
            <div className='h-[400px] overflow-hidden'>
                <GoogleMap
                    id={MAP_ID}
                    gestureHandling='greedy'
                    defaultCenter={defaultCenter}
                    {...(hasCommunityBounds
                        ? {
                              defaultBounds: {
                                  north: communityBounds!.boundsNorth!,
                                  south: communityBounds!.boundsSouth!,
                                  east: communityBounds!.boundsEast!,
                                  west: communityBounds!.boundsWest!,
                                  padding: 40,
                              },
                          }
                        : { defaultZoom: 13 })}
                    style={{ width: '100%', height: '400px' }}
                >
                    {hasCommunityBounds ? (
                        <CommunityBoundsOverlay
                            mapId={MAP_ID}
                            boundsNorth={communityBounds!.boundsNorth!}
                            boundsSouth={communityBounds!.boundsSouth!}
                            boundsEast={communityBounds!.boundsEast!}
                            boundsWest={communityBounds!.boundsWest!}
                        />
                    ) : null}
                    <DrawingLayer
                        mapId={MAP_ID}
                        initialGeoJson={value}
                        drawingMode={drawingMode}
                        onGeometryChange={handleGeometryChange}
                        onGeometryCleared={handleGeometryCleared}
                        onShapePresenceChange={setHasShape}
                    />
                </GoogleMap>
            </div>
            <div className='flex items-center justify-between p-2 bg-muted rounded-b-lg'>
                <span className='text-sm text-muted-foreground flex items-center gap-1'>
                    <MapPin className='h-3.5 w-3.5' />
                    {hasShape ? 'Geometry defined' : 'No geometry — draw a shape above'}
                </span>
                <Button
                    type='button'
                    variant='ghost'
                    size='sm'
                    className='text-xs'
                    onClick={() => setShowRawGeoJson(!showRawGeoJson)}
                >
                    {showRawGeoJson ? <ChevronUp className='h-3 w-3 mr-1' /> : <ChevronDown className='h-3 w-3 mr-1' />}
                    {showRawGeoJson ? 'Hide' : 'Show'} raw GeoJSON
                </Button>
            </div>
            {showRawGeoJson ? (
                <Textarea
                    value={value}
                    onChange={(e) => onChange(e.target.value)}
                    placeholder='{"type": "Polygon", "coordinates": [...]}'
                    className='h-32 font-mono text-sm rounded-t-none'
                />
            ) : null}
        </div>
    );
};
