import { useMemo } from 'react';
import { Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { ExternalLink, Loader2, MapPin, Plus } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import LitterReportData from '@/components/Models/LitterReportData';
import {
    LitterReportStatusEnum,
    LitterReportStatusLabels,
    LitterReportStatusColors,
} from '@/components/Models/LitterReportStatus';
import { GetNewLitterReports } from '@/services/litter-report';
import { calculateDistanceMiles } from '@/lib/distance';

interface NearbyLitterReportsWidgetProps {
    userLocation: { lat: number; lng: number };
    radiusMiles: number;
}

export const NearbyLitterReportsWidget = ({ userLocation, radiusMiles }: NearbyLitterReportsWidgetProps) => {
    const { data: litterReports, isLoading } = useQuery<AxiosResponse<LitterReportData[]>, unknown, LitterReportData[]>({
        queryKey: GetNewLitterReports().key,
        queryFn: GetNewLitterReports().service,
        select: (res) => res.data,
    });

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
            .sort((a, b) => a.distance - b.distance)
            .slice(0, 5); // Show only top 5 closest
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
                    {nearbyReports.map(({ report, distance }) => {
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
