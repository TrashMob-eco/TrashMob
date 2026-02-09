import { useMemo } from 'react';
import { useParams, Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Loader2, History, Download, MapPin, ClipboardList, AlertTriangle } from 'lucide-react';

import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import SponsoredAdoptionData from '@/components/Models/SponsoredAdoptionData';
import ProfessionalCleanupLogData from '@/components/Models/ProfessionalCleanupLogData';
import { GetSponsorAdoptions, GetSponsorCleanupLogs } from '@/services/sponsor-portal';

export const SponsorDashboard = () => {
    const { sponsorId } = useParams<{ sponsorId: string }>() as { sponsorId: string };

    const { data: adoptions, isLoading: adoptionsLoading } = useQuery<
        AxiosResponse<SponsoredAdoptionData[]>,
        unknown,
        SponsoredAdoptionData[]
    >({
        queryKey: GetSponsorAdoptions({ sponsorId }).key,
        queryFn: GetSponsorAdoptions({ sponsorId }).service,
        select: (res) => res.data,
        enabled: !!sponsorId,
    });

    const { data: cleanupLogs, isLoading: logsLoading } = useQuery<
        AxiosResponse<ProfessionalCleanupLogData[]>,
        unknown,
        ProfessionalCleanupLogData[]
    >({
        queryKey: GetSponsorCleanupLogs({ sponsorId }).key,
        queryFn: GetSponsorCleanupLogs({ sponsorId }).service,
        select: (res) => res.data,
        enabled: !!sponsorId,
    });

    // Compute compliance status for each adoption
    const adoptionStatuses = useMemo(() => {
        if (!adoptions || !cleanupLogs) return [];
        const now = new Date();

        return adoptions.map((adoption) => {
            const logsForAdoption = cleanupLogs.filter((l) => l.sponsoredAdoptionId === adoption.id);
            const lastLog = logsForAdoption.length > 0 ? logsForAdoption[0] : null;
            let nextDue: Date | null = null;
            let isOverdue = false;

            if (lastLog) {
                nextDue = new Date(lastLog.cleanupDate);
                nextDue.setDate(nextDue.getDate() + adoption.cleanupFrequencyDays);
                isOverdue = nextDue < now;
            } else {
                nextDue = new Date(adoption.startDate);
                nextDue.setDate(nextDue.getDate() + adoption.cleanupFrequencyDays);
                isOverdue = nextDue < now;
            }

            return { adoption, lastLog, nextDue, isOverdue };
        });
    }, [adoptions, cleanupLogs]);

    const recentLogs = useMemo(() => (cleanupLogs || []).slice(0, 5), [cleanupLogs]);

    const formatDate = (dateStr: string | Date | null | undefined) => {
        if (!dateStr) return '-';
        return new Date(dateStr).toLocaleDateString();
    };

    if (adoptionsLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='space-y-6'>
            {/* Quick Actions */}
            <Card>
                <CardContent className='pt-6'>
                    <div className='flex flex-col sm:flex-row gap-4'>
                        <Button variant='outline' asChild size='lg' className='h-12'>
                            <Link to={`/sponsordashboard/${sponsorId}/history`}>
                                <History className='h-5 w-5 mr-2' />
                                View Full History
                            </Link>
                        </Button>
                        <Button variant='outline' asChild size='lg' className='h-12'>
                            <Link to={`/sponsordashboard/${sponsorId}/reports`}>
                                <Download className='h-5 w-5 mr-2' />
                                Download Report
                            </Link>
                        </Button>
                    </div>
                </CardContent>
            </Card>

            {/* Adopted Segments */}
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <MapPin className='h-5 w-5' />
                        Adopted Segments
                    </CardTitle>
                    <CardDescription>Segments sponsored by your organization.</CardDescription>
                </CardHeader>
                <CardContent>
                    {adoptionStatuses.length > 0 ? (
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Area</TableHead>
                                    <TableHead>Company</TableHead>
                                    <TableHead>Frequency</TableHead>
                                    <TableHead>Next Due</TableHead>
                                    <TableHead>Status</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {adoptionStatuses.map(({ adoption, nextDue, isOverdue }) => (
                                    <TableRow key={adoption.id}>
                                        <TableCell className='font-medium'>
                                            {adoption.adoptableArea?.name || '-'}
                                        </TableCell>
                                        <TableCell>{adoption.professionalCompany?.name || '-'}</TableCell>
                                        <TableCell>Every {adoption.cleanupFrequencyDays} days</TableCell>
                                        <TableCell>{formatDate(nextDue)}</TableCell>
                                        <TableCell>
                                            {isOverdue ? (
                                                <Badge className='bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'>
                                                    <AlertTriangle className='h-3 w-3 mr-1' />
                                                    Overdue
                                                </Badge>
                                            ) : (
                                                <Badge className='bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'>
                                                    On Schedule
                                                </Badge>
                                            )}
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    ) : (
                        <div className='text-center py-8 text-muted-foreground'>
                            <MapPin className='h-12 w-12 mx-auto mb-4 opacity-50' />
                            <p className='font-medium'>No adopted segments</p>
                            <p className='text-sm'>No sponsored adoptions found for this sponsor.</p>
                        </div>
                    )}
                </CardContent>
            </Card>

            {/* Recent Activity */}
            <Card>
                <CardHeader>
                    <div className='flex items-center justify-between'>
                        <div>
                            <CardTitle className='flex items-center gap-2'>
                                <ClipboardList className='h-5 w-5' />
                                Recent Activity
                            </CardTitle>
                            <CardDescription>Latest cleanup logs.</CardDescription>
                        </div>
                        {(cleanupLogs || []).length > 5 ? (
                            <Button variant='outline' size='sm' asChild>
                                <Link to={`/sponsordashboard/${sponsorId}/history`}>View All</Link>
                            </Button>
                        ) : null}
                    </div>
                </CardHeader>
                <CardContent>
                    {logsLoading ? (
                        <div className='py-4 text-center'>
                            <Loader2 className='h-6 w-6 animate-spin mx-auto' />
                        </div>
                    ) : recentLogs.length > 0 ? (
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Date</TableHead>
                                    <TableHead>Area</TableHead>
                                    <TableHead>Bags</TableHead>
                                    <TableHead>Weight (lbs)</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {recentLogs.map((log) => (
                                    <TableRow key={log.id}>
                                        <TableCell>{formatDate(log.cleanupDate)}</TableCell>
                                        <TableCell>{log.sponsoredAdoption?.adoptableArea?.name || '-'}</TableCell>
                                        <TableCell>{log.bagsCollected}</TableCell>
                                        <TableCell>
                                            {log.weightInPounds != null ? log.weightInPounds.toFixed(1) : '-'}
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    ) : (
                        <div className='text-center py-8 text-muted-foreground'>
                            <p>No cleanup logs recorded yet.</p>
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
};

export default SponsorDashboard;
