import { useState } from 'react';
import { Link } from 'react-router';
import { AdvancedMarker, InfoWindow } from '@vis.gl/react-google-maps';
import { GoogleMapWithKey } from '@/components/Map/GoogleMap';
import PickupLocationData from '@/components/Models/PickupLocationData';

const PICKUP_MAP_ID = 'pickupRequestsMap';

const PickupMarkerPin = () => (
    <svg width='28' height='36' viewBox='0 0 28 36' fill='none' xmlns='http://www.w3.org/2000/svg'>
        <path d='M14 0C6.268 0 0 6.268 0 14c0 10.5 14 22 14 22s14-11.5 14-22C28 6.268 21.732 0 14 0z' fill='#F59E0B' />
        <circle cx='14' cy='14' r='6' fill='white' />
    </svg>
);

interface PickupRequestsMapViewProps {
    pickups: PickupLocationData[];
}

export const PickupRequestsMapView = ({ pickups }: PickupRequestsMapViewProps) => {
    const [selected, setSelected] = useState<PickupLocationData | null>(null);

    const pickupsWithLocation = pickups.filter((p) => p.latitude !== 0 && p.longitude !== 0);

    if (pickupsWithLocation.length === 0) {
        return (
            <p className='text-sm text-muted-foreground py-4 text-center'>
                No pickup requests with location data available.
            </p>
        );
    }

    const avgLat = pickupsWithLocation.reduce((sum, p) => sum + p.latitude, 0) / pickupsWithLocation.length;
    const avgLng = pickupsWithLocation.reduce((sum, p) => sum + p.longitude, 0) / pickupsWithLocation.length;

    return (
        <div className='rounded-md overflow-hidden border'>
            <GoogleMapWithKey
                id={PICKUP_MAP_ID}
                style={{ width: '100%', height: '400px' }}
                defaultCenter={{ lat: avgLat, lng: avgLng }}
                defaultZoom={10}
            >
                {pickupsWithLocation.map((pickup) => (
                    <AdvancedMarker
                        key={pickup.id}
                        position={{ lat: pickup.latitude, lng: pickup.longitude }}
                        onClick={() => setSelected(pickup)}
                    >
                        <PickupMarkerPin />
                    </AdvancedMarker>
                ))}

                {selected ? (
                    <InfoWindow
                        position={{ lat: selected.latitude, lng: selected.longitude }}
                        onCloseClick={() => setSelected(null)}
                    >
                        <div className='text-sm space-y-1 max-w-[200px]'>
                            <p className='font-semibold'>{selected.name || selected.streetAddress}</p>
                            <p className='text-muted-foreground'>{selected.city}</p>
                            {selected.notes ? <p className='text-muted-foreground'>{selected.notes}</p> : null}
                            <Link
                                to={`/eventsummary/${selected.eventId}`}
                                className='text-primary hover:underline text-xs'
                            >
                                View Event Summary
                            </Link>
                        </div>
                    </InfoWindow>
                ) : null}
            </GoogleMapWithKey>
        </div>
    );
};
