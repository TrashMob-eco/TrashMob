import { useCallback, useState } from 'react';
import { useParams } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Loader2, ClipboardList, Check, X, Users, AlertTriangle, Download, TrendingUp, Calendar } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogTrigger,
} from '@/components/ui/alert-dialog';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { useToast } from '@/hooks/use-toast';
import CommunityData from '@/components/Models/CommunityData';
import TeamAdoptionData, { AdoptionStatus } from '@/components/Models/TeamAdoptionData';
import { AdoptionComplianceStats } from '@/components/Models/AdoptionComplianceStats';
import { GetCommunityBySlug } from '@/services/communities';
import {
    GetPendingApplications,
    GetApprovedAdoptions,
    ApproveAdoption,
    RejectAdoption,
    GetDelinquentAdoptions,
    GetComplianceStats,
    ExportAdoptions,
} from '@/services/team-adoptions';

const statusColors: Record<AdoptionStatus, string> = {
    Pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    Approved: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    Rejected: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    Revoked: 'bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-200',
};

export const AdoptionsIndex = () => {
    const queryClient = useQueryClient();
    const { slug } = useParams<{ slug: string }>() as { slug: string };
    const { toast } = useToast();
    const [rejectDialogOpen, setRejectDialogOpen] = useState(false);
    const [selectedAdoptionId, setSelectedAdoptionId] = useState<string | null>(null);
    const [rejectionReason, setRejectionReason] = useState('');

    // Get community data
    const { data: community } = useQuery<AxiosResponse<CommunityData>, unknown, CommunityData>({
        queryKey: GetCommunityBySlug({ slug }).key,
        queryFn: GetCommunityBySlug({ slug }).service,
        select: (res) => res.data,
        enabled: !!slug,
    });

    // Get pending applications
    const { data: pendingApplications, isLoading: pendingLoading } = useQuery<
        AxiosResponse<TeamAdoptionData[]>,
        unknown,
        TeamAdoptionData[]
    >({
        queryKey: GetPendingApplications({ partnerId: community?.id || '' }).key,
        queryFn: GetPendingApplications({ partnerId: community?.id || '' }).service,
        select: (res) => res.data,
        enabled: !!community?.id,
    });

    // Get approved adoptions
    const { data: approvedAdoptions, isLoading: approvedLoading } = useQuery<
        AxiosResponse<TeamAdoptionData[]>,
        unknown,
        TeamAdoptionData[]
    >({
        queryKey: GetApprovedAdoptions({ partnerId: community?.id || '' }).key,
        queryFn: GetApprovedAdoptions({ partnerId: community?.id || '' }).service,
        select: (res) => res.data,
        enabled: !!community?.id,
    });

    // Get delinquent adoptions
    const { data: delinquentAdoptions, isLoading: delinquentLoading } = useQuery<
        AxiosResponse<TeamAdoptionData[]>,
        unknown,
        TeamAdoptionData[]
    >({
        queryKey: GetDelinquentAdoptions({ partnerId: community?.id || '' }).key,
        queryFn: GetDelinquentAdoptions({ partnerId: community?.id || '' }).service,
        select: (res) => res.data,
        enabled: !!community?.id,
    });

    // Get compliance stats
    const { data: complianceStats, isLoading: statsLoading } = useQuery<
        AxiosResponse<AdoptionComplianceStats>,
        unknown,
        AdoptionComplianceStats
    >({
        queryKey: GetComplianceStats({ partnerId: community?.id || '' }).key,
        queryFn: GetComplianceStats({ partnerId: community?.id || '' }).service,
        select: (res) => res.data,
        enabled: !!community?.id,
    });

    // Approve mutation
    const { mutate: approveAdoption, isPending: isApproving } = useMutation({
        mutationKey: ApproveAdoption().key,
        mutationFn: ApproveAdoption().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetPendingApplications({ partnerId: community?.id || '' }).key,
            });
            queryClient.invalidateQueries({
                queryKey: GetApprovedAdoptions({ partnerId: community?.id || '' }).key,
            });
            toast({
                variant: 'primary',
                title: 'Application approved',
                description: 'The adoption application has been approved.',
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to approve application. Please try again.',
            });
        },
    });

    // Reject mutation
    const { mutate: rejectAdoption, isPending: isRejecting } = useMutation({
        mutationKey: RejectAdoption().key,
        mutationFn: RejectAdoption().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetPendingApplications({ partnerId: community?.id || '' }).key,
            });
            setRejectDialogOpen(false);
            setSelectedAdoptionId(null);
            setRejectionReason('');
            toast({
                variant: 'primary',
                title: 'Application rejected',
                description: 'The adoption application has been rejected.',
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to reject application. Please try again.',
            });
        },
    });

    const handleApprove = useCallback(
        (adoptionId: string) => {
            if (!community?.id) return;
            approveAdoption({ partnerId: community.id, adoptionId });
        },
        [community?.id, approveAdoption],
    );

    const handleRejectClick = useCallback((adoptionId: string) => {
        setSelectedAdoptionId(adoptionId);
        setRejectionReason('');
        setRejectDialogOpen(true);
    }, []);

    const handleRejectConfirm = useCallback(() => {
        if (!community?.id || !selectedAdoptionId) return;
        rejectAdoption(
            { partnerId: community.id, adoptionId: selectedAdoptionId },
            { rejectionReason: rejectionReason || 'No reason provided' },
        );
    }, [community?.id, selectedAdoptionId, rejectionReason, rejectAdoption]);

    const formatDate = (date: Date | null | undefined) => {
        if (!date) return '-';
        return new Date(date).toLocaleDateString();
    };

    const handleExport = useCallback(async () => {
        if (!community?.id) return;
        try {
            const response = await ExportAdoptions({ partnerId: community.id }).service();
            const blob = response.data;
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `${community.name.replace(/ /g, '_')}_Adoptions_${new Date().toISOString().split('T')[0]}.csv`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
            toast({
                variant: 'primary',
                title: 'Export successful',
                description: 'Adoption data has been downloaded.',
            });
        } catch {
            toast({
                variant: 'destructive',
                title: 'Export failed',
                description: 'Failed to download adoption data. Please try again.',
            });
        }
    }, [community?.id, community?.name, toast]);

    if (pendingLoading || approvedLoading || delinquentLoading || statsLoading) {
        return (
            <div className='container py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='container py-8 space-y-6'>
            {/* Compliance Stats Dashboard */}
            {complianceStats ? (
                <div className='grid gap-4 md:grid-cols-4'>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>Total Adoptions</CardTitle>
                            <Users className='h-4 w-4 text-muted-foreground' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold'>{complianceStats.totalAdoptions}</div>
                            <p className='text-xs text-muted-foreground'>
                                {complianceStats.adoptedAreas} of {complianceStats.totalAvailableAreas} areas adopted
                            </p>
                        </CardContent>
                    </Card>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>Compliance Rate</CardTitle>
                            <TrendingUp className='h-4 w-4 text-muted-foreground' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold'>{complianceStats.complianceRate.toFixed(0)}%</div>
                            <p className='text-xs text-muted-foreground'>
                                {complianceStats.compliantAdoptions} compliant teams
                            </p>
                        </CardContent>
                    </Card>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>At Risk</CardTitle>
                            <AlertTriangle className='h-4 w-4 text-yellow-500' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold text-yellow-600'>{complianceStats.atRiskAdoptions}</div>
                            <p className='text-xs text-muted-foreground'>Approaching delinquency</p>
                        </CardContent>
                    </Card>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>Delinquent</CardTitle>
                            <AlertTriangle className='h-4 w-4 text-red-500' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold text-red-600'>{complianceStats.delinquentAdoptions}</div>
                            <p className='text-xs text-muted-foreground'>Need follow-up</p>
                        </CardContent>
                    </Card>
                </div>
            ) : null}

            <Card>
                <CardHeader>
                    <div className='flex items-center justify-between'>
                        <div>
                            <CardTitle className='flex items-center gap-2'>
                                <ClipboardList className='h-5 w-5' />
                                Adoption Applications
                            </CardTitle>
                            <CardDescription>
                                Review and manage team applications to adopt areas in your community.
                            </CardDescription>
                        </div>
                        <Button variant='outline' onClick={handleExport}>
                            <Download className='h-4 w-4 mr-2' />
                            Export CSV
                        </Button>
                    </div>
                </CardHeader>
                <CardContent>
                    <Tabs defaultValue='pending'>
                        <TabsList className='mb-4'>
                            <TabsTrigger value='pending'>Pending ({pendingApplications?.length || 0})</TabsTrigger>
                            <TabsTrigger value='approved'>Approved ({approvedAdoptions?.length || 0})</TabsTrigger>
                            <TabsTrigger value='delinquent' className='text-red-600'>
                                Delinquent ({delinquentAdoptions?.length || 0})
                            </TabsTrigger>
                        </TabsList>

                        <TabsContent value='pending'>
                            {pendingApplications && pendingApplications.length > 0 ? (
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead>Team</TableHead>
                                            <TableHead>Area</TableHead>
                                            <TableHead>Type</TableHead>
                                            <TableHead>Applied</TableHead>
                                            <TableHead>Notes</TableHead>
                                            <TableHead className='text-right'>Actions</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        {pendingApplications.map((adoption) => (
                                            <TableRow key={adoption.id}>
                                                <TableCell className='font-medium'>
                                                    <div className='flex items-center gap-2'>
                                                        <Users className='h-4 w-4 text-muted-foreground' />
                                                        {adoption.team?.name || 'Unknown Team'}
                                                    </div>
                                                </TableCell>
                                                <TableCell>{adoption.adoptableArea?.name || 'Unknown Area'}</TableCell>
                                                <TableCell>{adoption.adoptableArea?.areaType || '-'}</TableCell>
                                                <TableCell>{formatDate(adoption.applicationDate)}</TableCell>
                                                <TableCell className='max-w-xs truncate'>
                                                    {adoption.applicationNotes || '-'}
                                                </TableCell>
                                                <TableCell className='text-right'>
                                                    <div className='flex justify-end gap-2'>
                                                        <AlertDialog>
                                                            <AlertDialogTrigger asChild>
                                                                <Button
                                                                    variant='outline'
                                                                    size='sm'
                                                                    className='text-green-600'
                                                                    disabled={isApproving}
                                                                >
                                                                    <Check className='h-4 w-4 mr-1' />
                                                                    Approve
                                                                </Button>
                                                            </AlertDialogTrigger>
                                                            <AlertDialogContent>
                                                                <AlertDialogHeader>
                                                                    <AlertDialogTitle>
                                                                        Approve Application
                                                                    </AlertDialogTitle>
                                                                    <AlertDialogDescription>
                                                                        Are you sure you want to approve{' '}
                                                                        <strong>{adoption.team?.name}</strong>'s
                                                                        application to adopt{' '}
                                                                        <strong>{adoption.adoptableArea?.name}</strong>?
                                                                        The team will be notified and committed to the
                                                                        area's cleanup requirements.
                                                                    </AlertDialogDescription>
                                                                </AlertDialogHeader>
                                                                <AlertDialogFooter>
                                                                    <AlertDialogCancel>Cancel</AlertDialogCancel>
                                                                    <AlertDialogAction
                                                                        onClick={() => handleApprove(adoption.id)}
                                                                    >
                                                                        Approve
                                                                    </AlertDialogAction>
                                                                </AlertDialogFooter>
                                                            </AlertDialogContent>
                                                        </AlertDialog>
                                                        <Button
                                                            variant='outline'
                                                            size='sm'
                                                            className='text-red-600'
                                                            disabled={isRejecting}
                                                            onClick={() => handleRejectClick(adoption.id)}
                                                        >
                                                            <X className='h-4 w-4 mr-1' />
                                                            Reject
                                                        </Button>
                                                    </div>
                                                </TableCell>
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            ) : (
                                <div className='text-center py-12'>
                                    <ClipboardList className='h-12 w-12 mx-auto text-muted-foreground mb-4' />
                                    <h3 className='text-lg font-medium mb-2'>No pending applications</h3>
                                    <p className='text-muted-foreground'>
                                        Teams will appear here when they apply to adopt areas in your community.
                                    </p>
                                </div>
                            )}
                        </TabsContent>

                        <TabsContent value='approved'>
                            {approvedAdoptions && approvedAdoptions.length > 0 ? (
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead>Team</TableHead>
                                            <TableHead>Area</TableHead>
                                            <TableHead>Type</TableHead>
                                            <TableHead>Compliance</TableHead>
                                            <TableHead>Events</TableHead>
                                            <TableHead>Last Event</TableHead>
                                            <TableHead>Approved</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        {approvedAdoptions.map((adoption) => (
                                            <TableRow key={adoption.id}>
                                                <TableCell className='font-medium'>
                                                    <div className='flex items-center gap-2'>
                                                        <Users className='h-4 w-4 text-muted-foreground' />
                                                        {adoption.team?.name || 'Unknown Team'}
                                                    </div>
                                                </TableCell>
                                                <TableCell>{adoption.adoptableArea?.name || 'Unknown Area'}</TableCell>
                                                <TableCell>{adoption.adoptableArea?.areaType || '-'}</TableCell>
                                                <TableCell>
                                                    {adoption.isCompliant ? (
                                                        <Badge className='bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'>
                                                            <Check className='h-3 w-3 mr-1' />
                                                            Compliant
                                                        </Badge>
                                                    ) : (
                                                        <Badge className='bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'>
                                                            <AlertTriangle className='h-3 w-3 mr-1' />
                                                            Non-compliant
                                                        </Badge>
                                                    )}
                                                </TableCell>
                                                <TableCell>{adoption.eventCount}</TableCell>
                                                <TableCell>{formatDate(adoption.lastEventDate)}</TableCell>
                                                <TableCell>{formatDate(adoption.reviewedDate)}</TableCell>
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            ) : (
                                <div className='text-center py-12'>
                                    <Users className='h-12 w-12 mx-auto text-muted-foreground mb-4' />
                                    <h3 className='text-lg font-medium mb-2'>No approved adoptions yet</h3>
                                    <p className='text-muted-foreground'>
                                        Approved adoptions will appear here after you review pending applications.
                                    </p>
                                </div>
                            )}
                        </TabsContent>

                        <TabsContent value='delinquent'>
                            {delinquentAdoptions && delinquentAdoptions.length > 0 ? (
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead>Team</TableHead>
                                            <TableHead>Area</TableHead>
                                            <TableHead>Type</TableHead>
                                            <TableHead>Events</TableHead>
                                            <TableHead>Last Event</TableHead>
                                            <TableHead>Days Overdue</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        {delinquentAdoptions.map((adoption) => {
                                            const lastEvent = adoption.lastEventDate
                                                ? new Date(adoption.lastEventDate)
                                                : null;
                                            const daysSinceEvent = lastEvent
                                                ? Math.floor((Date.now() - lastEvent.getTime()) / (1000 * 60 * 60 * 24))
                                                : null;
                                            const requiredFrequency =
                                                adoption.adoptableArea?.cleanupFrequencyDays || 90;
                                            const daysOverdue =
                                                daysSinceEvent !== null ? daysSinceEvent - requiredFrequency : null;
                                            return (
                                                <TableRow key={adoption.id}>
                                                    <TableCell className='font-medium'>
                                                        <div className='flex items-center gap-2'>
                                                            <Users className='h-4 w-4 text-muted-foreground' />
                                                            {adoption.team?.name || 'Unknown Team'}
                                                        </div>
                                                    </TableCell>
                                                    <TableCell>
                                                        {adoption.adoptableArea?.name || 'Unknown Area'}
                                                    </TableCell>
                                                    <TableCell>{adoption.adoptableArea?.areaType || '-'}</TableCell>
                                                    <TableCell>{adoption.eventCount}</TableCell>
                                                    <TableCell>
                                                        <div className='flex items-center gap-2'>
                                                            <Calendar className='h-4 w-4 text-muted-foreground' />
                                                            {formatDate(adoption.lastEventDate)}
                                                        </div>
                                                    </TableCell>
                                                    <TableCell>
                                                        {daysOverdue !== null && daysOverdue > 0 ? (
                                                            <Badge className='bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'>
                                                                {daysOverdue} days overdue
                                                            </Badge>
                                                        ) : (
                                                            <Badge className='bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'>
                                                                No events yet
                                                            </Badge>
                                                        )}
                                                    </TableCell>
                                                </TableRow>
                                            );
                                        })}
                                    </TableBody>
                                </Table>
                            ) : (
                                <div className='text-center py-12'>
                                    <Check className='h-12 w-12 mx-auto text-green-500 mb-4' />
                                    <h3 className='text-lg font-medium mb-2'>All teams are compliant!</h3>
                                    <p className='text-muted-foreground'>
                                        No teams are currently overdue on their cleanup requirements.
                                    </p>
                                </div>
                            )}
                        </TabsContent>
                    </Tabs>
                </CardContent>
            </Card>

            {/* Reject Dialog */}
            <Dialog open={rejectDialogOpen} onOpenChange={setRejectDialogOpen}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>Reject Application</DialogTitle>
                        <DialogDescription>
                            Please provide a reason for rejecting this application. The team will be notified.
                        </DialogDescription>
                    </DialogHeader>
                    <div className='space-y-4 py-4'>
                        <div className='space-y-2'>
                            <Label htmlFor='rejection-reason'>Rejection Reason</Label>
                            <Textarea
                                id='rejection-reason'
                                placeholder='Enter the reason for rejection...'
                                value={rejectionReason}
                                onChange={(e) => setRejectionReason(e.target.value)}
                                className='h-24'
                            />
                        </div>
                    </div>
                    <DialogFooter>
                        <Button variant='outline' onClick={() => setRejectDialogOpen(false)}>
                            Cancel
                        </Button>
                        <Button variant='destructive' onClick={handleRejectConfirm} disabled={isRejecting}>
                            {isRejecting ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                            Reject Application
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </div>
    );
};

export default AdoptionsIndex;
