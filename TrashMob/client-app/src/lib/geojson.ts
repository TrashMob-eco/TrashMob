// GeoJSON <-> Google Maps conversion utilities
// GeoJSON uses [lng, lat], Google Maps uses {lat, lng}

export interface GeoJsonGeometry {
    type: string;
    coordinates: number[][] | number[][][];
}

export interface GeoJsonPolygon extends GeoJsonGeometry {
    type: 'Polygon';
    coordinates: number[][][];
}

export interface GeoJsonLineString extends GeoJsonGeometry {
    type: 'LineString';
    coordinates: number[][];
}

export type ParsedGeoJson = GeoJsonPolygon | GeoJsonLineString;

export function parseGeoJson(raw: string): ParsedGeoJson | null {
    if (!raw.trim()) return null;
    try {
        const parsed = JSON.parse(raw);
        // Normalize PascalCase keys (e.g. from .NET serialization) to standard GeoJSON camelCase
        const type = parsed.type ?? parsed.Type;
        const coordinates = parsed.coordinates ?? parsed.Coordinates;
        if (type === 'Polygon' && Array.isArray(coordinates)) {
            return { type: 'Polygon', coordinates } as GeoJsonPolygon;
        }
        if (type === 'LineString' && Array.isArray(coordinates)) {
            return { type: 'LineString', coordinates } as GeoJsonLineString;
        }
        return null;
    } catch {
        return null;
    }
}

/** Convert GeoJSON Polygon ring [lng,lat][] to Google Maps path {lat,lng}[] — strips closing point */
export function polygonCoordsToPath(coords: number[][][]): google.maps.LatLngLiteral[] {
    const ring = coords[0]; // outer ring
    if (!ring || ring.length < 4) return [];
    // Strip closing point (last == first in GeoJSON)
    const open = ring.slice(0, -1);
    return open.map(([lng, lat]) => ({ lat, lng }));
}

/** Convert GeoJSON LineString [lng,lat][] to Google Maps path {lat,lng}[] */
export function lineStringCoordsToPath(coords: number[][]): google.maps.LatLngLiteral[] {
    return coords.map(([lng, lat]) => ({ lat, lng }));
}

/** Serialize a Google Maps Polygon to GeoJSON string — closes ring, swaps lat/lng to lng/lat */
export function polygonToGeoJson(polygon: google.maps.Polygon): string {
    const path = polygon.getPath().getArray();
    const coords = path.map((p) => [p.lng(), p.lat()]);
    // Close the ring
    if (coords.length > 0) {
        coords.push([...coords[0]]);
    }
    const geometry: GeoJsonPolygon = { type: 'Polygon', coordinates: [coords] };
    return JSON.stringify(geometry);
}

/** Serialize a Google Maps Polyline to GeoJSON string — swaps lat/lng to lng/lat */
export function polylineToGeoJson(polyline: google.maps.Polyline): string {
    const path = polyline.getPath().getArray();
    const coords = path.map((p) => [p.lng(), p.lat()]);
    const geometry: GeoJsonLineString = { type: 'LineString', coordinates: coords };
    return JSON.stringify(geometry);
}

/** Compute centroid of a path (arithmetic mean) */
export function computeCentroid(path: google.maps.LatLngLiteral[]): { lat: number; lng: number } {
    if (path.length === 0) return { lat: 0, lng: 0 };
    const sum = path.reduce((acc, p) => ({ lat: acc.lat + p.lat, lng: acc.lng + p.lng }), { lat: 0, lng: 0 });
    return { lat: sum.lat / path.length, lng: sum.lng / path.length };
}

export interface AreaBoundingBox {
    startLatitude: number;
    startLongitude: number;
    endLatitude: number;
    endLongitude: number;
}

/** Compute bounding box of a path */
export function computeBoundingBox(path: google.maps.LatLngLiteral[]): AreaBoundingBox | null {
    if (path.length === 0) return null;
    let minLat = Infinity,
        maxLat = -Infinity,
        minLng = Infinity,
        maxLng = -Infinity;
    for (const p of path) {
        if (p.lat < minLat) minLat = p.lat;
        if (p.lat > maxLat) maxLat = p.lat;
        if (p.lng < minLng) minLng = p.lng;
        if (p.lng > maxLng) maxLng = p.lng;
    }
    return { startLatitude: minLat, startLongitude: minLng, endLatitude: maxLat, endLongitude: maxLng };
}
