import { useState, useMemo } from 'react';
import { Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { ExternalLink, Loader2, MapPin, Plus } from 'lucide-react';
import { AdvancedMarker, InfoWindow } from '@vis.gl/react-google-maps';

import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { GoogleMapWithKey } from '@/components/Map/GoogleMap';
import LitterReportData from '@/components/Models/LitterReportData';
import {
    LitterReportStatusEnum,
    LitterReportStatusLabels,
    LitterReportStatusColors,
} from '@/components/Models/LitterReportStatus';
import { GetNewLitterReports } from '@/services/litter-report';
import { calculateDistanceMiles } from '@/lib/distance';

const NEARBY_LITTER_MAP_ID = 'nearbyLitterReportsMap';

const NearbyLitterMarkerPin = () => (
    <svg width='28' height='36' viewBox='0 0 28 36' fill='none' xmlns='http://www.w3.org/2000/svg'>
        <path d='M14 0C6.268 0 0 6.268 0 14c0 10.5 14 22 14 22s14-11.5 14-22C28 6.268 21.732 0 14 0z' fill='#F97316' />
        <circle cx='14' cy='14' r='6' fill='white' />
    </svg>
);

interface NearbyLitterReportsWidgetProps {
    viewMode: 'table' | 'map';
    userLocation: { lat: number; lng: number };
    radiusMiles: number;
}

export const NearbyLitterReportsWidget = ({ viewMode, userLocation, radiusMiles }: NearbyLitterReportsWidgetProps) => {
    const [selected, setSelected] = useState<{ report: LitterReportData; distance: number } | null>(null);

    const { data: litterReports, isLoading } = useQuery<AxiosResponse<LitterReportData[]>, unknown, LitterReportData[]>(
        {
            queryKey: GetNewLitterReports().key,
            queryFn: GetNewLitterReports().service,
            select: (res) => res.data,
        },
    );

    // Filter reports by distance and sort by closest first
    const nearbyReports = useMemo(() => {
        if (!litterReports) return [];

        return litterReports
            .map((report) => {
                const firstImage = report.litterImages?.[0];
                if (!firstImage?.latitude || !firstImage?.longitude) return null;

                const distance = calculateDistanceMiles(
                    userLocation.lat,
                    userLocation.lng,
                    firstImage.latitude,
                    firstImage.longitude,
                );

                return { report, distance };
            })
            .filter((item): item is { report: LitterReportData; distance: number } => {
                return item !== null && item.distance <= radiusMiles;
            })
            .sort((a, b) => a.distance - b.distance);
    }, [litterReports, userLocation, radiusMiles]);

    const getLocation = (report: LitterReportData) => {
        const firstImage = report.litterImages?.[0];
        if (!firstImage) return '-';
        const parts = [firstImage.city, firstImage.region].filter(Boolean);
        return parts.join(', ') || '-';
    };

    if (isLoading) {
        return (
            <div className='flex items-center justify-center py-8'>
                <Loader2 className='h-6 w-6 animate-spin text-muted-foreground' />
            </div>
        );
    }

    if (nearbyReports.length === 0) {
        return (
            <div className='text-center text-muted-foreground py-8'>
                <p>No litter reports found within {radiusMiles} miles of your location.</p>
                <p className='text-sm mt-2'>
                    <Link to='/litterreports' className='text-primary hover:underline'>
                        Browse all litter reports
                    </Link>
                </p>
            </div>
        );
    }

    if (viewMode === 'map') {
        return (
            <div className='rounded-md overflow-hidden border'>
                <GoogleMapWithKey
                    id={NEARBY_LITTER_MAP_ID}
                    style={{ width: '100%', height: '400px' }}
                    defaultCenter={userLocation}
                    defaultZoom={11}
                >
                    {nearbyReports.map(({ report }) => {
                        const img = report.litterImages[0];
                        return (
                            <AdvancedMarker
                                key={report.id}
                                position={{ lat: img.latitude!, lng: img.longitude! }}
                                onClick={() =>
                                    setSelected(nearbyReports.find((r) => r.report.id === report.id) ?? null)
                                }
                            >
                                <NearbyLitterMarkerPin />
                            </AdvancedMarker>
                        );
                    })}

                    {selected ? (
                        <InfoWindow
                            position={{
                                lat: selected.report.litterImages[0].latitude!,
                                lng: selected.report.litterImages[0].longitude!,
                            }}
                            onCloseClick={() => setSelected(null)}
                        >
                            <div className='text-sm space-y-1 max-w-[200px]'>
                                <Link
                                    to={`/litterreports/${selected.report.id}`}
                                    className='font-semibold text-primary hover:underline block'
                                >
                                    {selected.report.name || 'Untitled Report'}
                                </Link>
                                <p className='text-muted-foreground'>{selected.distance.toFixed(1)} mi away</p>
                                <Badge
                                    variant='outline'
                                    className={`${LitterReportStatusColors[selected.report.litterReportStatusId as LitterReportStatusEnum] || 'bg-gray-500'} text-white border-0`}
                                >
                                    {LitterReportStatusLabels[
                                        selected.report.litterReportStatusId as LitterReportStatusEnum
                                    ] || 'Unknown'}
                                </Badge>
                            </div>
                        </InfoWindow>
                    ) : null}
                </GoogleMapWithKey>
            </div>
        );
    }

    // Table view - show top 5
    const displayReports = nearbyReports.slice(0, 5);

    return (
        <div className='overflow-auto'>
            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHead>Report</TableHead>
                        <TableHead>Distance</TableHead>
                        <TableHead>Status</TableHead>
                        <TableHead className='w-[100px]' />
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {displayReports.map(({ report, distance }) => {
                        const statusId = report.litterReportStatusId as LitterReportStatusEnum;

                        return (
                            <TableRow key={report.id}>
                                <TableCell>
                                    <Link
                                        to={`/litterreports/${report.id}`}
                                        className='text-primary hover:underline font-medium flex items-center gap-1'
                                    >
                                        {report.name || 'Untitled Report'}
                                        <ExternalLink className='h-3 w-3' />
                                    </Link>
                                    <div className='flex items-center gap-1 text-sm text-muted-foreground'>
                                        <MapPin className='h-3 w-3' />
                                        <span>{getLocation(report)}</span>
                                    </div>
                                </TableCell>
                                <TableCell className='text-muted-foreground'>{distance.toFixed(1)} mi</TableCell>
                                <TableCell>
                                    <Badge
                                        variant='outline'
                                        className={`${LitterReportStatusColors[statusId] || 'bg-gray-500'} text-white border-0`}
                                    >
                                        {LitterReportStatusLabels[statusId] || 'Unknown'}
                                    </Badge>
                                </TableCell>
                                <TableCell>
                                    <Button size='sm' variant='outline' asChild>
                                        <Link to='/events/create' state={{ fromLitterReport: report }}>
                                            <Plus className='h-4 w-4 mr-1' /> Event
                                        </Link>
                                    </Button>
                                </TableCell>
                            </TableRow>
                        );
                    })}
                </TableBody>
            </Table>
            <div className='mt-4 text-center'>
                <Link to='/litterreports' className='text-sm text-primary hover:underline'>
                    View all litter reports
                </Link>
            </div>
        </div>
    );
};
