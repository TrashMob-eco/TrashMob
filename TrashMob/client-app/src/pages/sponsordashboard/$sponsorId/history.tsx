import { useMemo } from 'react';
import { useParams, Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Loader2, ArrowLeft, ClipboardList, Package, Scale } from 'lucide-react';

import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import ProfessionalCleanupLogData from '@/components/Models/ProfessionalCleanupLogData';
import { GetSponsorCleanupLogs } from '@/services/sponsor-portal';

export const SponsorCleanupHistory = () => {
    const { sponsorId } = useParams<{ sponsorId: string }>() as { sponsorId: string };

    const { data: cleanupLogs, isLoading } = useQuery<
        AxiosResponse<ProfessionalCleanupLogData[]>,
        unknown,
        ProfessionalCleanupLogData[]
    >({
        queryKey: GetSponsorCleanupLogs({ sponsorId }).key,
        queryFn: GetSponsorCleanupLogs({ sponsorId }).service,
        select: (res) => res.data,
        enabled: !!sponsorId,
    });

    const stats = useMemo(() => {
        const logs = cleanupLogs || [];
        return {
            totalCleanups: logs.length,
            totalBags: logs.reduce((sum, l) => sum + l.bagsCollected, 0),
            totalWeight: logs.reduce((sum, l) => sum + (l.weightInPounds ?? 0), 0),
        };
    }, [cleanupLogs]);

    const formatDate = (dateStr: string | null | undefined) => {
        if (!dateStr) return '-';
        return new Date(dateStr).toLocaleDateString();
    };

    if (isLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='space-y-6'>
            <div>
                <Button variant='ghost' size='sm' asChild>
                    <Link to={`/sponsordashboard/${sponsorId}`}>
                        <ArrowLeft className='h-4 w-4 mr-2' />
                        Back to Dashboard
                    </Link>
                </Button>
            </div>

            {/* Summary Stats */}
            <div className='grid grid-cols-1 md:grid-cols-3 gap-4'>
                <Card>
                    <CardHeader className='flex flex-row items-center justify-between pb-2'>
                        <CardTitle className='text-sm font-medium'>Total Cleanups</CardTitle>
                        <ClipboardList className='h-4 w-4 text-muted-foreground' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{stats.totalCleanups}</div>
                    </CardContent>
                </Card>
                <Card>
                    <CardHeader className='flex flex-row items-center justify-between pb-2'>
                        <CardTitle className='text-sm font-medium'>Total Bags</CardTitle>
                        <Package className='h-4 w-4 text-muted-foreground' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{stats.totalBags}</div>
                    </CardContent>
                </Card>
                <Card>
                    <CardHeader className='flex flex-row items-center justify-between pb-2'>
                        <CardTitle className='text-sm font-medium'>Total Weight (lbs)</CardTitle>
                        <Scale className='h-4 w-4 text-muted-foreground' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{stats.totalWeight.toFixed(1)}</div>
                    </CardContent>
                </Card>
            </div>

            {/* Cleanup Logs Table */}
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <ClipboardList className='h-5 w-5' />
                        Cleanup History
                    </CardTitle>
                    <CardDescription>
                        Complete log of all professional cleanups for your sponsored segments.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    {cleanupLogs && cleanupLogs.length > 0 ? (
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Date</TableHead>
                                    <TableHead>Area</TableHead>
                                    <TableHead>Duration (min)</TableHead>
                                    <TableHead>Bags</TableHead>
                                    <TableHead>Weight (lbs)</TableHead>
                                    <TableHead>Notes</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {cleanupLogs.map((log) => (
                                    <TableRow key={log.id}>
                                        <TableCell>{formatDate(log.cleanupDate)}</TableCell>
                                        <TableCell>{log.sponsoredAdoption?.adoptableArea?.name || '-'}</TableCell>
                                        <TableCell>{log.durationMinutes}</TableCell>
                                        <TableCell>{log.bagsCollected}</TableCell>
                                        <TableCell>
                                            {log.weightInPounds != null ? log.weightInPounds.toFixed(1) : '-'}
                                        </TableCell>
                                        <TableCell className='max-w-xs truncate'>{log.notes || '-'}</TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    ) : (
                        <div className='text-center py-8 text-muted-foreground'>
                            <ClipboardList className='h-12 w-12 mx-auto mb-4 opacity-50' />
                            <p className='font-medium'>No cleanup logs yet</p>
                            <p className='text-sm'>
                                Cleanup logs will appear here once professional companies log their work.
                            </p>
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
};

export default SponsorCleanupHistory;
