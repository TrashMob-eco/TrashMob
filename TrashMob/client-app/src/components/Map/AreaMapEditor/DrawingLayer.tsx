import { useEffect, useRef, useCallback } from 'react';
import { useMap, useMapsLibrary } from '@vis.gl/react-google-maps';
import { DrawingMode } from './AreaMapToolbar';
import {
    parseGeoJson,
    polygonCoordsToPath,
    lineStringCoordsToPath,
    polygonToGeoJson,
    polylineToGeoJson,
    computeCentroid,
    computeBoundingBox,
    AreaBoundingBox,
} from '@/lib/geojson';

const SHAPE_STYLE = {
    strokeColor: '#10B981',
    strokeOpacity: 0.9,
    strokeWeight: 2,
    fillColor: '#10B981',
    fillOpacity: 0.2,
};

interface DrawingLayerProps {
    mapId: string;
    initialGeoJson: string;
    drawingMode: DrawingMode;
    onGeometryChange: (geoJson: string, centroid: { lat: number; lng: number }, bbox: AreaBoundingBox | null) => void;
    onGeometryCleared: () => void;
    onShapePresenceChange: (hasShape: boolean) => void;
}

export const DrawingLayer = ({
    mapId,
    initialGeoJson,
    drawingMode,
    onGeometryChange,
    onGeometryCleared,
    onShapePresenceChange,
}: DrawingLayerProps) => {
    const map = useMap(mapId);
    const drawing = useMapsLibrary('drawing');

    const drawingManagerRef = useRef<google.maps.drawing.DrawingManager | null>(null);
    const currentShapeRef = useRef<google.maps.Polygon | google.maps.Polyline | null>(null);
    const shapeTypeRef = useRef<'polygon' | 'polyline' | null>(null);
    const pathListenersRef = useRef<google.maps.MapsEventListener[]>([]);
    const initializedRef = useRef(false);

    const clearPathListeners = useCallback(() => {
        pathListenersRef.current.forEach((l) => l.remove());
        pathListenersRef.current = [];
    }, []);

    const emitChange = useCallback(
        (shape: google.maps.Polygon | google.maps.Polyline, type: 'polygon' | 'polyline') => {
            let geoJson: string;
            let path: google.maps.LatLngLiteral[];

            if (type === 'polygon') {
                const poly = shape as google.maps.Polygon;
                geoJson = polygonToGeoJson(poly);
                path = poly
                    .getPath()
                    .getArray()
                    .map((p) => ({ lat: p.lat(), lng: p.lng() }));
            } else {
                const line = shape as google.maps.Polyline;
                geoJson = polylineToGeoJson(line);
                path = line
                    .getPath()
                    .getArray()
                    .map((p) => ({ lat: p.lat(), lng: p.lng() }));
            }

            onGeometryChange(geoJson, computeCentroid(path), computeBoundingBox(path));
        },
        [onGeometryChange],
    );

    const attachPathListeners = useCallback(
        (shape: google.maps.Polygon | google.maps.Polyline, type: 'polygon' | 'polyline') => {
            clearPathListeners();
            const path = shape.getPath();
            const handler = () => emitChange(shape, type);
            pathListenersRef.current.push(
                google.maps.event.addListener(path, 'set_at', handler),
                google.maps.event.addListener(path, 'insert_at', handler),
                google.maps.event.addListener(path, 'remove_at', handler),
            );
        },
        [clearPathListeners, emitChange],
    );

    const removeCurrentShape = useCallback(() => {
        clearPathListeners();
        if (currentShapeRef.current) {
            currentShapeRef.current.setMap(null);
            currentShapeRef.current = null;
            shapeTypeRef.current = null;
            onShapePresenceChange(false);
        }
    }, [clearPathListeners, onShapePresenceChange]);

    const setCurrentShape = useCallback(
        (shape: google.maps.Polygon | google.maps.Polyline, type: 'polygon' | 'polyline') => {
            currentShapeRef.current = shape;
            shapeTypeRef.current = type;
            onShapePresenceChange(true);
            emitChange(shape, type);
        },
        [onShapePresenceChange, emitChange],
    );

    // Initialize DrawingManager
    useEffect(() => {
        if (!map || !drawing) return;

        if (!drawingManagerRef.current) {
            const dm = new drawing.DrawingManager({
                map,
                drawingMode: null,
                drawingControl: false,
                polygonOptions: { ...SHAPE_STYLE, editable: false, draggable: false },
                polylineOptions: { strokeColor: SHAPE_STYLE.strokeColor, strokeOpacity: SHAPE_STYLE.strokeOpacity, strokeWeight: 3, editable: false },
            });

            google.maps.event.addListener(dm, 'overlaycomplete', (e: google.maps.drawing.OverlayCompleteEvent) => {
                dm.setDrawingMode(null);

                if (e.type === google.maps.drawing.OverlayType.POLYGON) {
                    const poly = e.overlay as google.maps.Polygon;
                    setCurrentShape(poly, 'polygon');
                } else if (e.type === google.maps.drawing.OverlayType.POLYLINE) {
                    const line = e.overlay as google.maps.Polyline;
                    setCurrentShape(line, 'polyline');
                }
            });

            drawingManagerRef.current = dm;
        }

        return () => {
            if (drawingManagerRef.current) {
                drawingManagerRef.current.setMap(null);
                drawingManagerRef.current = null;
            }
        };
    }, [map, drawing, setCurrentShape]);

    // Sync drawing mode
    useEffect(() => {
        const dm = drawingManagerRef.current;
        if (!dm) return;

        if (drawingMode === 'polygon') {
            dm.setDrawingMode(google.maps.drawing.OverlayType.POLYGON);
        } else if (drawingMode === 'polyline') {
            dm.setDrawingMode(google.maps.drawing.OverlayType.POLYLINE);
        } else {
            dm.setDrawingMode(null);
        }

        // Toggle editable on current shape
        if (currentShapeRef.current) {
            const editable = drawingMode === 'edit';
            currentShapeRef.current.setEditable(editable);
            if (editable && shapeTypeRef.current) {
                attachPathListeners(currentShapeRef.current, shapeTypeRef.current);
            } else {
                clearPathListeners();
            }
        }
    }, [drawingMode, attachPathListeners, clearPathListeners]);

    // Render initial GeoJSON
    useEffect(() => {
        if (!map || initializedRef.current) return;
        if (!initialGeoJson) {
            initializedRef.current = true;
            return;
        }

        const parsed = parseGeoJson(initialGeoJson);
        if (!parsed) {
            initializedRef.current = true;
            return;
        }

        let shape: google.maps.Polygon | google.maps.Polyline;
        let type: 'polygon' | 'polyline';
        let path: google.maps.LatLngLiteral[];

        if (parsed.type === 'Polygon') {
            path = polygonCoordsToPath(parsed.coordinates);
            if (path.length < 3) {
                initializedRef.current = true;
                return;
            }
            shape = new google.maps.Polygon({ paths: path, map, ...SHAPE_STYLE, editable: false, draggable: false });
            type = 'polygon';
        } else {
            path = lineStringCoordsToPath(parsed.coordinates);
            if (path.length < 2) {
                initializedRef.current = true;
                return;
            }
            shape = new google.maps.Polyline({
                path,
                map,
                strokeColor: SHAPE_STYLE.strokeColor,
                strokeOpacity: SHAPE_STYLE.strokeOpacity,
                strokeWeight: 3,
                editable: false,
            });
            type = 'polyline';
        }

        currentShapeRef.current = shape;
        shapeTypeRef.current = type;
        onShapePresenceChange(true);

        // Fit map to shape bounds
        const bounds = new google.maps.LatLngBounds();
        path.forEach((p) => bounds.extend(p));
        if (!bounds.isEmpty()) {
            map.fitBounds(bounds, { top: 50, right: 50, bottom: 50, left: 50 });
        }

        initializedRef.current = true;
    }, [map, initialGeoJson, onShapePresenceChange]);

    // Expose delete for parent
    useEffect(() => {
        if (drawingMode === 'delete') {
            removeCurrentShape();
            onGeometryCleared();
        }
    }, [drawingMode, removeCurrentShape, onGeometryCleared]);

    return null;
};
