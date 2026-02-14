import { useMemo, useRef, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { AdvancedMarker, InfoWindow } from '@vis.gl/react-google-maps';
import { MapPin, Calendar, Eye, Plus, List, Map } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent } from '@/components/ui/card';
import { DataTable, DataTableColumnHeader } from '@/components/ui/data-table';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItemAlt, SelectTrigger, SelectValue } from '@/components/ui/select';
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group';
import { GoogleMapWithKey as GoogleMap } from '@/components/Map/GoogleMap';
import { LitterReportPin, litterReportColors } from '@/components/litterreports/litter-report-pin';
import {
    LitterReportInfoWindowHeader,
    LitterReportInfoWindowContent,
} from '@/components/litterreports/litter-report-info-window';
import LitterReportData from '@/components/Models/LitterReportData';
import {
    LitterReportStatusEnum,
    LitterReportStatusLabels,
    LitterReportStatusColors,
} from '@/components/Models/LitterReportStatus';
import { GetNotCancelledLitterReports } from '@/services/litter-report';
import { getLastDaysTimerange, getLastMonthsTimerange, getAllCompletedTimerange } from '@/pages/_home/utils/timerange';

const formatDate = (date: Date | null) => {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
    });
};

const getLocation = (report: LitterReportData) => {
    const firstImage = report.litterImages?.[0];
    if (!firstImage) return '-';
    const parts = [firstImage.city, firstImage.region].filter(Boolean);
    return parts.join(', ') || '-';
};

const getLitterReportColor = (statusId: number): string => {
    switch (statusId) {
        case LitterReportStatusEnum.New:
            return litterReportColors.new;
        case LitterReportStatusEnum.Assigned:
            return litterReportColors.assigned;
        case LitterReportStatusEnum.Cleaned:
            return litterReportColors.cleaned;
        case LitterReportStatusEnum.Cancelled:
            return litterReportColors.cancelled;
        default:
            return litterReportColors.new;
    }
};

const statusOptions = [
    { value: 'all', label: 'All Statuses' },
    { value: '1', label: 'New' },
    { value: '2', label: 'Assigned' },
    { value: '3', label: 'Cleaned' },
];

const dateOptions = [
    { value: getLastDaysTimerange(7), label: 'Last 7 days' },
    { value: getLastDaysTimerange(30), label: 'Last 30 days' },
    { value: getLastDaysTimerange(90), label: 'Last 90 days' },
    { value: getLastMonthsTimerange(6), label: 'Last 6 months' },
    { value: getLastMonthsTimerange(12), label: 'Last year' },
    { value: getAllCompletedTimerange(), label: 'All time' },
];

const columns: ColumnDef<LitterReportData>[] = [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
        cell: ({ row }) => (
            <Link to={`/litterreports/${row.original.id}`} className='text-primary hover:underline font-medium'>
                {row.original.name || 'Untitled Report'}
            </Link>
        ),
    },
    {
        id: 'location',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Location' />,
        cell: ({ row }) => (
            <div className='flex items-center gap-1'>
                <MapPin className='h-4 w-4 text-muted-foreground' />
                <span>{getLocation(row.original)}</span>
            </div>
        ),
    },
    {
        accessorKey: 'litterReportStatusId',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Status' />,
        cell: ({ row }) => {
            const statusId = row.original.litterReportStatusId as LitterReportStatusEnum;
            const label = LitterReportStatusLabels[statusId] || 'Unknown';
            const colorClass = LitterReportStatusColors[statusId] || 'bg-gray-500';
            return (
                <Badge variant='outline' className={`${colorClass} text-white border-0`}>
                    {label}
                </Badge>
            );
        },
    },
    {
        accessorKey: 'createdDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Reported' />,
        cell: ({ row }) => (
            <div className='flex items-center gap-1'>
                <Calendar className='h-4 w-4 text-muted-foreground' />
                <span>{formatDate(row.original.createdDate)}</span>
            </div>
        ),
    },
    {
        id: 'images',
        header: 'Photos',
        cell: ({ row }) => <span>{row.original.litterImages?.length || 0}</span>,
    },
    {
        id: 'actions',
        header: '',
        cell: ({ row }) => (
            <Button variant='ghost' size='sm' asChild>
                <Link to={`/litterreports/${row.original.id}`}>
                    <Eye className='h-4 w-4 mr-1' /> View
                </Link>
            </Button>
        ),
    },
];

export const LitterReportsPage = () => {
    const [statusFilter, setStatusFilter] = useState<string>('all');
    const [dateFilter, setDateFilter] = useState<string>(getAllCompletedTimerange());
    const [view, setView] = useState<string>('list');

    const { data: litterReports, isLoading } = useQuery<AxiosResponse<LitterReportData[]>, unknown, LitterReportData[]>(
        {
            queryKey: GetNotCancelledLitterReports().key,
            queryFn: GetNotCancelledLitterReports().service,
            select: (res) => res.data,
        },
    );

    // Client-side filtering
    const filteredReports = useMemo(() => {
        if (!litterReports) return [];
        const [startDate, endDate] = dateFilter.split('|');
        return litterReports.filter((report) => {
            if (statusFilter !== 'all' && report.litterReportStatusId !== Number(statusFilter)) return false;
            if (report.createdDate) {
                const created = new Date(report.createdDate);
                if (created < new Date(startDate) || created > new Date(endDate)) return false;
            }
            return true;
        });
    }, [litterReports, statusFilter, dateFilter]);

    // Map markers: only reports with lat/lng
    const reportsWithLocation = useMemo(
        () =>
            filteredReports
                .map((report) => {
                    const img = report.litterImages?.find((i) => i.latitude && i.longitude);
                    if (!img) return null;
                    return { ...report, latitude: img.latitude!, longitude: img.longitude! };
                })
                .filter(Boolean) as (LitterReportData & { latitude: number; longitude: number })[],
        [filteredReports],
    );

    // Info window state
    const markersRef = useRef<Record<string, google.maps.marker.AdvancedMarkerElement>>({});
    const [showingReportId, setShowingReportId] = useState<string>('');
    const showingReport = reportsWithLocation.find((r) => r.id === showingReportId);

    return (
        <div>
            <HeroSection Title='Litter Reports' Description='Help us identify areas that need cleaning.' />
            <div className='container py-8'>
                {/* Filter bar */}
                <div className='flex flex-col sm:flex-row gap-3 mb-4'>
                    <Select value={statusFilter} onValueChange={setStatusFilter}>
                        <SelectTrigger className='w-44'>
                            <SelectValue placeholder='Status' />
                        </SelectTrigger>
                        <SelectContent>
                            {statusOptions.map((opt) => (
                                <SelectItemAlt key={opt.value} value={opt.value}>
                                    {opt.label}
                                </SelectItemAlt>
                            ))}
                        </SelectContent>
                    </Select>
                    <Select value={dateFilter} onValueChange={setDateFilter}>
                        <SelectTrigger className='w-44'>
                            <SelectValue placeholder='Date' />
                        </SelectTrigger>
                        <SelectContent>
                            {dateOptions.map((opt) => (
                                <SelectItemAlt key={opt.value} value={opt.value}>
                                    {opt.label}
                                </SelectItemAlt>
                            ))}
                        </SelectContent>
                    </Select>
                    <div className='flex-1' />
                    <ToggleGroup value={view} onValueChange={(v) => v && setView(v)} type='single' variant='outline'>
                        <ToggleGroupItem
                            value='list'
                            className='data-[state=on]:bg-primary data-[state=on]:text-primary-foreground'
                        >
                            <List />
                        </ToggleGroupItem>
                        <ToggleGroupItem
                            value='map'
                            className='data-[state=on]:bg-primary data-[state=on]:text-primary-foreground'
                        >
                            <Map />
                        </ToggleGroupItem>
                    </ToggleGroup>
                    <Button asChild>
                        <Link to='/litterreports/create'>
                            <Plus className='h-4 w-4 mr-2' /> Report Litter
                        </Link>
                    </Button>
                </div>

                {/* Result count */}
                <div className='text-sm text-muted-foreground mb-3'>
                    {filteredReports.length} report{filteredReports.length !== 1 ? 's' : ''} found
                </div>

                {isLoading ? (
                    <div className='text-center py-8'>Loading litter reports...</div>
                ) : view === 'map' ? (
                    <>
                        {/* Color legend */}
                        <div className='flex flex-wrap items-center gap-4 mb-3 text-sm'>
                            <div className='flex items-center gap-1.5'>
                                <div
                                    className='w-3 h-3 rounded-full'
                                    style={{ backgroundColor: litterReportColors.new }}
                                />
                                <span>New</span>
                            </div>
                            <div className='flex items-center gap-1.5'>
                                <div
                                    className='w-3 h-3 rounded-full'
                                    style={{ backgroundColor: litterReportColors.assigned }}
                                />
                                <span>Assigned</span>
                            </div>
                            <div className='flex items-center gap-1.5'>
                                <div
                                    className='w-3 h-3 rounded-full'
                                    style={{ backgroundColor: litterReportColors.cleaned }}
                                />
                                <span>Cleaned</span>
                            </div>
                        </div>
                        <div className='rounded-lg overflow-hidden border'>
                            <GoogleMap>
                                {reportsWithLocation.map((report) => (
                                    <AdvancedMarker
                                        key={report.id}
                                        ref={(el) => {
                                            markersRef.current[report.id] = el!;
                                        }}
                                        position={{ lat: report.latitude, lng: report.longitude }}
                                        onMouseEnter={() => setShowingReportId(report.id)}
                                    >
                                        <LitterReportPin
                                            color={getLitterReportColor(report.litterReportStatusId)}
                                            size={32}
                                        />
                                    </AdvancedMarker>
                                ))}
                                {showingReport ? (
                                    <InfoWindow
                                        anchor={markersRef.current[showingReportId]}
                                        headerContent={<LitterReportInfoWindowHeader name={showingReport.name} />}
                                        onClose={() => setShowingReportId('')}
                                    >
                                        <LitterReportInfoWindowContent report={showingReport} />
                                    </InfoWindow>
                                ) : null}
                            </GoogleMap>
                        </div>
                    </>
                ) : (
                    <Card>
                        <CardContent className='pt-6'>
                            <DataTable columns={columns} data={filteredReports} />
                        </CardContent>
                    </Card>
                )}
            </div>
        </div>
    );
};

export default LitterReportsPage;
