import { useMemo } from 'react';
import { useParams, Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Loader2, ClipboardPlus, MapPin, ClipboardList, AlertTriangle } from 'lucide-react';

import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import SponsoredAdoptionData from '@/components/Models/SponsoredAdoptionData';
import ProfessionalCleanupLogData from '@/components/Models/ProfessionalCleanupLogData';
import { GetCompanyAssignments, GetCompanyCleanupLogs } from '@/services/professional-company-portal';

export const CompanyDashboard = () => {
    const { companyId } = useParams<{ companyId: string }>() as { companyId: string };

    const { data: assignments, isLoading: assignmentsLoading } = useQuery<
        AxiosResponse<SponsoredAdoptionData[]>,
        unknown,
        SponsoredAdoptionData[]
    >({
        queryKey: GetCompanyAssignments({ companyId }).key,
        queryFn: GetCompanyAssignments({ companyId }).service,
        select: (res) => res.data,
        enabled: !!companyId,
    });

    const { data: cleanupLogs, isLoading: logsLoading } = useQuery<
        AxiosResponse<ProfessionalCleanupLogData[]>,
        unknown,
        ProfessionalCleanupLogData[]
    >({
        queryKey: GetCompanyCleanupLogs({ companyId }).key,
        queryFn: GetCompanyCleanupLogs({ companyId }).service,
        select: (res) => res.data,
        enabled: !!companyId,
    });

    // Compute schedule status for each assignment
    const assignmentStatuses = useMemo(() => {
        if (!assignments || !cleanupLogs) return [];
        const now = new Date();

        return assignments.map((adoption) => {
            const logsForAdoption = cleanupLogs.filter((l) => l.sponsoredAdoptionId === adoption.id);
            const lastLog = logsForAdoption.length > 0 ? logsForAdoption[0] : null; // logs are ordered desc
            let nextDue: Date | null = null;
            let isOverdue = false;

            if (lastLog) {
                nextDue = new Date(lastLog.cleanupDate);
                nextDue.setDate(nextDue.getDate() + adoption.cleanupFrequencyDays);
                isOverdue = nextDue < now;
            } else {
                // No cleanups yet — consider overdue from start date
                nextDue = new Date(adoption.startDate);
                nextDue.setDate(nextDue.getDate() + adoption.cleanupFrequencyDays);
                isOverdue = nextDue < now;
            }

            return { adoption, lastLog, nextDue, isOverdue };
        });
    }, [assignments, cleanupLogs]);

    const recentLogs = useMemo(() => (cleanupLogs || []).slice(0, 5), [cleanupLogs]);

    const formatDate = (dateStr: string | Date | null | undefined) => {
        if (!dateStr) return '-';
        return new Date(dateStr).toLocaleDateString();
    };

    if (assignmentsLoading) {
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
                        <Button asChild size='lg' className='h-12'>
                            <Link to={`/companydashboard/${companyId}/log-cleanup`}>
                                <ClipboardPlus className='h-5 w-5 mr-2' />
                                Log a Cleanup
                            </Link>
                        </Button>
                        <Button variant='outline' asChild size='lg' className='h-12'>
                            <Link to={`/companydashboard/${companyId}/history`}>
                                <ClipboardList className='h-5 w-5 mr-2' />
                                View Full History
                            </Link>
                        </Button>
                    </div>
                </CardContent>
            </Card>

            {/* Assigned Segments */}
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <MapPin className='h-5 w-5' />
                        Assigned Segments
                    </CardTitle>
                    <CardDescription>Active sponsored adoptions assigned to your company.</CardDescription>
                </CardHeader>
                <CardContent>
                    {assignmentStatuses.length > 0 ? (
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Area</TableHead>
                                    <TableHead>Sponsor</TableHead>
                                    <TableHead>Frequency</TableHead>
                                    <TableHead>Next Due</TableHead>
                                    <TableHead>Status</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {assignmentStatuses.map(({ adoption, nextDue, isOverdue }) => (
                                    <TableRow key={adoption.id}>
                                        <TableCell className='font-medium'>
                                            {adoption.adoptableArea?.name || '-'}
                                        </TableCell>
                                        <TableCell>{adoption.sponsor?.name || '-'}</TableCell>
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
                            <p className='font-medium'>No active assignments</p>
                            <p className='text-sm'>Your company has no active sponsored adoption assignments yet.</p>
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
                                <Link to={`/companydashboard/${companyId}/history`}>View All</Link>
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

export default CompanyDashboard;
