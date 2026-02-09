// Area measurement utilities using Google Maps Geometry library
// Requires google.maps.geometry to be loaded via useMapsLibrary('geometry')

export function computePolygonArea(path: google.maps.LatLngLiteral[]): { sqFt: number; acres: number } {
    const latLngs = path.map((p) => new google.maps.LatLng(p.lat, p.lng));
    const areaSqMeters = google.maps.geometry.spherical.computeArea(latLngs);
    const sqFt = areaSqMeters * 10.7639;
    const acres = sqFt / 43560;
    return { sqFt, acres };
}

export function computeLineLength(path: google.maps.LatLngLiteral[]): { feet: number; miles: number } {
    const latLngs = path.map((p) => new google.maps.LatLng(p.lat, p.lng));
    const meters = google.maps.geometry.spherical.computeLength(latLngs);
    const feet = meters * 3.28084;
    const miles = feet / 5280;
    return { feet, miles };
}

export function formatMeasurement(type: 'polygon' | 'polyline', path: google.maps.LatLngLiteral[]): string {
    if (type === 'polygon') {
        const { sqFt, acres } = computePolygonArea(path);
        if (acres >= 1) {
            return `${acres.toFixed(2)} acres (${sqFt.toLocaleString(undefined, { maximumFractionDigits: 0 })} sq ft)`;
        }
        return `${sqFt.toLocaleString(undefined, { maximumFractionDigits: 0 })} sq ft`;
    } else {
        const { feet, miles } = computeLineLength(path);
        if (miles >= 0.1) {
            return `${miles.toFixed(2)} mi (${feet.toLocaleString(undefined, { maximumFractionDigits: 0 })} ft)`;
        }
        return `${feet.toLocaleString(undefined, { maximumFractionDigits: 0 })} ft`;
    }
}
