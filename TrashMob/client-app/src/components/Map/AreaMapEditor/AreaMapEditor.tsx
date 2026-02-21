import { useState, useCallback, useMemo, useEffect } from 'react';
import { ChevronDown, ChevronUp, MapPin, AlertTriangle, LocateFixed } from 'lucide-react';
import { useMap } from '@vis.gl/react-google-maps';
import { GoogleMapWithKey as GoogleMap } from '@/components/Map/GoogleMap';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { AreaMapToolbar, DrawingMode } from './AreaMapToolbar';
import { DrawingLayer } from './DrawingLayer';
import { CommunityBoundsOverlay } from './CommunityBoundsOverlay';
import { ExistingAreasOverlay } from './ExistingAreasOverlay';
import { AreaStatusLegend } from './AreaStatusLegend';
import { AiSuggestPanel } from './AiSuggestPanel';
import { AreaBoundingBox, parseGeoJson, polygonCoordsToPath, lineStringCoordsToPath } from '@/lib/geojson';
import AdoptableAreaData from '@/components/Models/AdoptableAreaData';

const MAP_ID = 'areaMapEditor';

interface CommunityBounds {
    boundsNorth: number | null;
    boundsSouth: number | null;
    boundsEast: number | null;
    boundsWest: number | null;
}

export interface SuggestionMetadata {
    suggestedName?: string;
    suggestedAreaType?: string;
    description?: string;
}

interface AreaMapEditorProps {
    value: string;
    onChange: (geoJson: string) => void;
    onBoundsChange?: (bbox: AreaBoundingBox | null) => void;
    onSuggestionMetadata?: (meta: SuggestionMetadata) => void;
    communityBounds?: CommunityBounds;
    communityCenter?: { lat: number; lng: number } | null;
    existingAreas?: AdoptableAreaData[];
    currentAreaId?: string;
    partnerId?: string;
    communityName?: string;
}

const EMPTY_AREAS: AdoptableAreaData[] = [];

function useOverlapDetection(currentGeoJson: string, existingAreas: AdoptableAreaData[], excludeId?: string) {
    return useMemo(() => {
        if (!currentGeoJson || !existingAreas.length) return [];

        const parsed = parseGeoJson(currentGeoJson);
        if (!parsed || parsed.type !== 'Polygon') return [];

        const currentPath = polygonCoordsToPath(parsed.coordinates);
        if (currentPath.length < 3) return [];

        if (typeof google === 'undefined' || !google.maps?.geometry?.poly?.containsLocation) return [];

        const currentPoly = new google.maps.Polygon({ paths: currentPath });

        return existingAreas
            .filter((area) => {
                if (area.id === excludeId || !area.geoJson) return false;
                const areaParsed = parseGeoJson(area.geoJson);
                if (!areaParsed || areaParsed.type !== 'Polygon') return false;
                const areaPath = polygonCoordsToPath(areaParsed.coordinates);
                if (areaPath.length < 3) return false;

                const existingPoly = new google.maps.Polygon({ paths: areaPath });

                const currentInExisting = currentPath.some((pt) =>
                    google.maps.geometry.poly.containsLocation(new google.maps.LatLng(pt.lat, pt.lng), existingPoly),
                );
                const existingInCurrent = areaPath.some((pt) =>
                    google.maps.geometry.poly.containsLocation(new google.maps.LatLng(pt.lat, pt.lng), currentPoly),
                );

                return currentInExisting || existingInCurrent;
            })
            .map((area) => area.name);
    }, [currentGeoJson, existingAreas, excludeId]);
}

export const AreaMapEditor = ({
    value,
    onChange,
    onBoundsChange,
    onSuggestionMetadata,
    communityBounds,
    communityCenter,
    existingAreas = EMPTY_AREAS,
    currentAreaId,
    partnerId,
    communityName,
}: AreaMapEditorProps) => {
    const [drawingMode, setDrawingMode] = useState<DrawingMode>(null);
    const [hasShape, setHasShape] = useState(false);
    const [showRawGeoJson, setShowRawGeoJson] = useState(false);
    const [measurement, setMeasurement] = useState<string | null>(null);
    const [aiPreviewGeoJson, setAiPreviewGeoJson] = useState<string | null>(null);
    const [mapViewportBounds, setMapViewportBounds] = useState<{
        north: number;
        south: number;
        east: number;
        west: number;
    } | null>(null);

    const hasCommunityBounds =
        communityBounds?.boundsNorth != null &&
        communityBounds?.boundsSouth != null &&
        communityBounds?.boundsEast != null &&
        communityBounds?.boundsWest != null;

    const defaultCenter = communityCenter ?? { lat: 47.6062, lng: -122.3321 };

    const overlappingNames = useOverlapDetection(value, existingAreas, currentAreaId);

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

    const handleSuggestionAccepted = useCallback(
        (geoJson: string, suggestedName?: string, suggestedAreaType?: string, userDescription?: string) => {
            setAiPreviewGeoJson(null);
            onChange(geoJson);
            onSuggestionMetadata?.({ suggestedName, suggestedAreaType, description: userDescription });
        },
        [onChange, onSuggestionMetadata],
    );

    const handleRecenter = useCallback(() => {
        if (!value) return;
        const parsed = parseGeoJson(value);
        if (!parsed) return;
        const path =
            parsed.type === 'Polygon'
                ? polygonCoordsToPath(parsed.coordinates)
                : lineStringCoordsToPath(parsed.coordinates);
        if (path.length === 0) return;
        window.dispatchEvent(new CustomEvent('areamap:fitbounds', { detail: { path } }));
    }, [value]);

    return (
        <div className='space-y-0'>
            {partnerId ? (
                <AiSuggestPanel
                    partnerId={partnerId}
                    communityCenter={communityCenter}
                    communityName={communityName}
                    mapBounds={mapViewportBounds}
                    onSuggestionPreview={setAiPreviewGeoJson}
                    onSuggestionAccepted={handleSuggestionAccepted}
                    onRequestEditMode={() => setDrawingMode('edit')}
                />
            ) : null}
            <AreaMapToolbar
                activeMode={drawingMode}
                hasShape={hasShape}
                onModeChange={handleModeChange}
                onDelete={handleDelete}
                onAddressSearch={(lat, lng) => {
                    // We dispatch a custom event that the MapPanHandler picks up
                    window.dispatchEvent(new CustomEvent('areamap:panto', { detail: { lat, lng } }));
                }}
            />
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
                    {existingAreas.length > 0 ? (
                        <ExistingAreasOverlay mapId={MAP_ID} areas={existingAreas} excludeAreaId={currentAreaId} />
                    ) : null}
                    <DrawingLayer
                        mapId={MAP_ID}
                        initialGeoJson={value}
                        drawingMode={drawingMode}
                        onGeometryChange={handleGeometryChange}
                        onGeometryCleared={handleGeometryCleared}
                        onShapePresenceChange={setHasShape}
                        onMeasurementChange={setMeasurement}
                        injectGeoJson={aiPreviewGeoJson}
                    />
                    <MapPanHandler mapId={MAP_ID} />
                    <MapBoundsTracker mapId={MAP_ID} onBoundsChange={setMapViewportBounds} />
                </GoogleMap>
            </div>
            {overlappingNames.length > 0 ? (
                <div className='flex items-center gap-2 px-3 py-2 bg-yellow-50 border-x border-yellow-200 text-yellow-800 text-sm'>
                    <AlertTriangle className='h-4 w-4 shrink-0' />
                    <span>
                        Overlaps with: <strong>{overlappingNames.join(', ')}</strong>
                    </span>
                </div>
            ) : null}
            <div className='flex items-center justify-between p-2 bg-muted rounded-b-lg'>
                <span className='text-sm text-muted-foreground flex items-center gap-1'>
                    <MapPin className='h-3.5 w-3.5' />
                    {hasShape && measurement
                        ? measurement
                        : hasShape
                          ? 'Geometry defined'
                          : 'No geometry — draw a shape above'}
                </span>
                <div className='flex items-center gap-3'>
                    {hasShape ? (
                        <Button type='button' variant='ghost' size='sm' className='text-xs' onClick={handleRecenter}>
                            <LocateFixed className='h-3 w-3 mr-1' />
                            Recenter
                        </Button>
                    ) : null}
                    {existingAreas.length > 0 ? <AreaStatusLegend /> : null}
                    <Button
                        type='button'
                        variant='ghost'
                        size='sm'
                        className='text-xs'
                        onClick={() => setShowRawGeoJson(!showRawGeoJson)}
                    >
                        {showRawGeoJson ? (
                            <ChevronUp className='h-3 w-3 mr-1' />
                        ) : (
                            <ChevronDown className='h-3 w-3 mr-1' />
                        )}
                        {showRawGeoJson ? 'Hide' : 'Show'} raw GeoJSON
                    </Button>
                </div>
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

/** Tracks the current map viewport bounds and reports them to the parent */
const MapBoundsTracker = ({
    mapId,
    onBoundsChange,
}: {
    mapId: string;
    onBoundsChange: (bounds: { north: number; south: number; east: number; west: number } | null) => void;
}) => {
    const map = useMap(mapId);

    useEffect(() => {
        if (!map) return;
        const listener = map.addListener('idle', () => {
            const b = map.getBounds();
            if (b) {
                const ne = b.getNorthEast();
                const sw = b.getSouthWest();
                onBoundsChange({ north: ne.lat(), south: sw.lat(), east: ne.lng(), west: sw.lng() });
            }
        });
        return () => listener.remove();
    }, [map, onBoundsChange]);

    return null;
};

/** Listens for custom pan-to and fit-bounds events and moves the map */
const MapPanHandler = ({ mapId }: { mapId: string }) => {
    const map = useMap(mapId);

    useEffect(() => {
        const panHandler = (e: Event) => {
            const { lat, lng } = (e as CustomEvent<{ lat: number; lng: number }>).detail;
            if (map) {
                map.panTo({ lat, lng });
                map.setZoom(17);
            }
        };
        const fitHandler = (e: Event) => {
            const { path } = (e as CustomEvent<{ path: google.maps.LatLngLiteral[] }>).detail;
            if (map && path?.length > 0) {
                const bounds = new google.maps.LatLngBounds();
                path.forEach((p: google.maps.LatLngLiteral) => bounds.extend(p));
                map.fitBounds(bounds, 40);
            }
        };
        window.addEventListener('areamap:panto', panHandler);
        window.addEventListener('areamap:fitbounds', fitHandler);
        return () => {
            window.removeEventListener('areamap:panto', panHandler);
            window.removeEventListener('areamap:fitbounds', fitHandler);
        };
    }, [map]);

    return null;
};
