import { useState } from 'react';
import { Link } from 'react-router';
import { AdvancedMarker, InfoWindow } from '@vis.gl/react-google-maps';
import { GoogleMapWithKey } from '@/components/Map/GoogleMap';
import LitterReportData from '@/components/Models/LitterReportData';
import { Badge } from '@/components/ui/badge';
import {
    LitterReportStatusEnum,
    LitterReportStatusLabels,
    LitterReportStatusColors,
} from '@/components/Models/LitterReportStatus';

const LITTER_MAP_ID = 'myLitterReportsMap';

const LitterMarkerPin = () => (
    <svg width='28' height='36' viewBox='0 0 28 36' fill='none' xmlns='http://www.w3.org/2000/svg'>
        <path d='M14 0C6.268 0 0 6.268 0 14c0 10.5 14 22 14 22s14-11.5 14-22C28 6.268 21.732 0 14 0z' fill='#EF4444' />
        <circle cx='14' cy='14' r='6' fill='white' />
    </svg>
);

interface LitterReportsMapViewProps {
    reports: LitterReportData[];
}

export const LitterReportsMapView = ({ reports }: LitterReportsMapViewProps) => {
    const [selected, setSelected] = useState<LitterReportData | null>(null);

    const reportsWithLocation = reports.filter((r) => {
        const img = r.litterImages?.[0];
        return img?.latitude && img?.longitude;
    });

    if (reportsWithLocation.length === 0) {
        return (
            <p className='text-sm text-muted-foreground py-4 text-center'>No reports with location data available.</p>
        );
    }

    const avgLat =
        reportsWithLocation.reduce((sum, r) => sum + (r.litterImages[0].latitude ?? 0), 0) / reportsWithLocation.length;
    const avgLng =
        reportsWithLocation.reduce((sum, r) => sum + (r.litterImages[0].longitude ?? 0), 0) /
        reportsWithLocation.length;

    const getLocation = (report: LitterReportData) => {
        const firstImage = report.litterImages?.[0];
        if (!firstImage) return '';
        const parts = [firstImage.city, firstImage.region].filter(Boolean);
        return parts.join(', ');
    };

    return (
        <div className='rounded-md overflow-hidden border'>
            <GoogleMapWithKey
                id={LITTER_MAP_ID}
                style={{ width: '100%', height: '400px' }}
                defaultCenter={{ lat: avgLat, lng: avgLng }}
                defaultZoom={10}
            >
                {reportsWithLocation.map((report) => (
                    <AdvancedMarker
                        key={report.id}
                        position={{
                            lat: report.litterImages[0].latitude!,
                            lng: report.litterImages[0].longitude!,
                        }}
                        onClick={() => setSelected(report)}
                    >
                        <LitterMarkerPin />
                    </AdvancedMarker>
                ))}

                {selected ? (
                    <InfoWindow
                        position={{
                            lat: selected.litterImages[0].latitude!,
                            lng: selected.litterImages[0].longitude!,
                        }}
                        onCloseClick={() => setSelected(null)}
                    >
                        <div className='text-sm space-y-1 max-w-[200px]'>
                            <Link
                                to={`/litterreports/${selected.id}`}
                                className='font-semibold text-primary hover:underline block'
                            >
                                {selected.name || 'Untitled Report'}
                            </Link>
                            {getLocation(selected) ? (
                                <p className='text-muted-foreground'>{getLocation(selected)}</p>
                            ) : null}
                            <Badge
                                variant='outline'
                                className={`${LitterReportStatusColors[selected.litterReportStatusId as LitterReportStatusEnum] || 'bg-gray-500'} text-white border-0`}
                            >
                                {LitterReportStatusLabels[selected.litterReportStatusId as LitterReportStatusEnum] ||
                                    'Unknown'}
                            </Badge>
                        </div>
                    </InfoWindow>
                ) : null}
            </GoogleMapWithKey>
        </div>
    );
};
