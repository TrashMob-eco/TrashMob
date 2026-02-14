import { useEffect, useRef } from 'react';
import { useMap } from '@vis.gl/react-google-maps';

/** Renders a circle overlay on the map. Must be a child of GoogleMap/APIProvider. */
export const MapCircle = ({
    center,
    radiusMeters,
}: {
    center: { lat: number; lng: number };
    radiusMeters: number;
}) => {
    const map = useMap();
    const circleRef = useRef<google.maps.Circle>();

    useEffect(() => {
        if (!map) return;

        if (!circleRef.current) {
            circleRef.current = new google.maps.Circle({
                strokeColor: '#005C4C',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#005C4C',
                fillOpacity: 0.2,
                clickable: false,
                map,
            });
        }

        circleRef.current.setCenter(center);
        circleRef.current.setRadius(radiusMeters);
    }, [map, center.lat, center.lng, radiusMeters]);

    useEffect(() => {
        return () => {
            circleRef.current?.setMap(null);
        };
    }, []);

    return null;
};
