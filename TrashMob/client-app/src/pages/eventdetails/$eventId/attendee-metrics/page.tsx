import { useCallback, useState, useMemo } from 'react';
import { useParams, useNavigate, Link } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse, AxiosError } from 'axios';
import moment from 'moment';
import {
    Loader2,
    ClipboardList,
    Check,
    X,
    Users,
    ArrowLeft,
    Scale,
    Clock,
    Package,
    Pencil,
    CheckCheck,
} from 'lucide-react';

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
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectItem, SelectContent, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useToast } from '@/hooks/use-toast';
import { useGetEvent } from '@/hooks/useGetEvent';
import { useLogin } from '@/hooks/useLogin';
import EventAttendeeMetricsData, { MetricsStatus } from '@/components/Models/EventAttendeeMetricsData';
import AttendeeMetricsTotals from '@/components/Models/AttendeeMetricsTotals';
import {
    GetAllMetrics,
    GetPendingMetrics,
    GetMetricsTotals,
    ApproveMetrics,
    RejectMetrics,
    AdjustMetrics,
    ApproveAllPending,
} from '@/services/event-attendee-metrics';
import { GetWeightUnits } from '@/services/weight-units';

const statusColors: Record<MetricsStatus, string> = {
    Pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    Approved: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    Rejected: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    Adjusted: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
};

export const AttendeeMetricsReview = () => {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const { eventId } = useParams<{ eventId: string }>() as { eventId: string };
    const { toast } = useToast();
    const { currentUser } = useLogin();

    // Dialog states
    const [rejectDialogOpen, setRejectDialogOpen] = useState(false);
    const [adjustDialogOpen, setAdjustDialogOpen] = useState(false);
    const [selectedMetrics, setSelectedMetrics] = useState<EventAttendeeMetricsData | null>(null);
    const [rejectionReason, setRejectionReason] = useState('');
    const [adjustmentReason, setAdjustmentReason] = useState('');
    const [adjustedValues, setAdjustedValues] = useState({
        bagsCollected: 0,
        pickedWeight: 0,
        pickedWeightUnitId: 1,
        durationMinutes: 0,
    });

    // Get event data
    const { data: event, isLoading: eventLoading } = useGetEvent(eventId);

    // Get weight units
    const { data: weightUnits } = useQuery({
        queryKey: GetWeightUnits().key,
        queryFn: GetWeightUnits().service,
        select: (res) => res.data,
    });

    // Get all metrics
    const {
        data: allMetrics,
        isLoading: metricsLoading,
        error: metricsError,
    } = useQuery<AxiosResponse<EventAttendeeMetricsData[]>, AxiosError, EventAttendeeMetricsData[]>({
        queryKey: GetAllMetrics({ eventId }).key,
        queryFn: GetAllMetrics({ eventId }).service,
        select: (res) => res.data,
        enabled: !!eventId,
    });

    // Get pending metrics
    const { data: pendingMetrics, isLoading: pendingLoading } = useQuery<
        AxiosResponse<EventAttendeeMetricsData[]>,
        AxiosError,
        EventAttendeeMetricsData[]
    >({
        queryKey: GetPendingMetrics({ eventId }).key,
        queryFn: GetPendingMetrics({ eventId }).service,
        select: (res) => res.data,
        enabled: !!eventId,
    });

    // Get totals
    const { data: totals, isLoading: totalsLoading } = useQuery<
        AxiosResponse<AttendeeMetricsTotals>,
        AxiosError,
        AttendeeMetricsTotals
    >({
        queryKey: GetMetricsTotals({ eventId }).key,
        queryFn: GetMetricsTotals({ eventId }).service,
        select: (res) => res.data,
        enabled: !!eventId,
    });

    // Filter metrics by status
    const approvedMetrics = useMemo(
        () => (allMetrics || []).filter((m) => m.status === 'Approved' || m.status === 'Adjusted'),
        [allMetrics],
    );
    const rejectedMetrics = useMemo(() => (allMetrics || []).filter((m) => m.status === 'Rejected'), [allMetrics]);

    // Approve mutation
    const { mutate: approveMetrics, isPending: isApproving } = useMutation({
        mutationKey: ApproveMetrics().key,
        mutationFn: ApproveMetrics().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetAllMetrics({ eventId }).key });
            queryClient.invalidateQueries({ queryKey: GetPendingMetrics({ eventId }).key });
            queryClient.invalidateQueries({ queryKey: GetMetricsTotals({ eventId }).key });
            toast({
                variant: 'primary',
                title: 'Metrics approved',
                description: 'The submission has been approved.',
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to approve metrics. Please try again.',
            });
        },
    });

    // Reject mutation
    const { mutate: rejectMetrics, isPending: isRejecting } = useMutation({
        mutationKey: RejectMetrics().key,
        mutationFn: RejectMetrics().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetAllMetrics({ eventId }).key });
            queryClient.invalidateQueries({ queryKey: GetPendingMetrics({ eventId }).key });
            setRejectDialogOpen(false);
            setSelectedMetrics(null);
            setRejectionReason('');
            toast({
                variant: 'primary',
                title: 'Metrics rejected',
                description: 'The submission has been rejected.',
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to reject metrics. Please try again.',
            });
        },
    });

    // Adjust mutation
    const { mutate: adjustMetrics, isPending: isAdjusting } = useMutation({
        mutationKey: AdjustMetrics().key,
        mutationFn: AdjustMetrics().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetAllMetrics({ eventId }).key });
            queryClient.invalidateQueries({ queryKey: GetPendingMetrics({ eventId }).key });
            queryClient.invalidateQueries({ queryKey: GetMetricsTotals({ eventId }).key });
            setAdjustDialogOpen(false);
            setSelectedMetrics(null);
            setAdjustmentReason('');
            toast({
                variant: 'primary',
                title: 'Metrics adjusted',
                description: 'The submission has been adjusted and approved.',
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to adjust metrics. Please try again.',
            });
        },
    });

    // Approve all mutation
    const { mutate: approveAll, isPending: isApprovingAll } = useMutation({
        mutationKey: ApproveAllPending().key,
        mutationFn: ApproveAllPending().service,
        onSuccess: (response) => {
            queryClient.invalidateQueries({ queryKey: GetAllMetrics({ eventId }).key });
            queryClient.invalidateQueries({ queryKey: GetPendingMetrics({ eventId }).key });
            queryClient.invalidateQueries({ queryKey: GetMetricsTotals({ eventId }).key });
            toast({
                variant: 'primary',
                title: 'All metrics approved',
                description: `${response.data} submissions have been approved.`,
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to approve all metrics. Please try again.',
            });
        },
    });

    const handleApprove = useCallback(
        (metricsId: string) => {
            approveMetrics({ eventId, metricsId });
        },
        [eventId, approveMetrics],
    );

    const handleRejectClick = useCallback((metrics: EventAttendeeMetricsData) => {
        setSelectedMetrics(metrics);
        setRejectionReason('');
        setRejectDialogOpen(true);
    }, []);

    const handleRejectConfirm = useCallback(() => {
        if (!selectedMetrics) return;
        rejectMetrics(
            { eventId, metricsId: selectedMetrics.id },
            { rejectionReason: rejectionReason || 'No reason provided' },
        );
    }, [eventId, selectedMetrics, rejectionReason, rejectMetrics]);

    const handleAdjustClick = useCallback((metrics: EventAttendeeMetricsData) => {
        setSelectedMetrics(metrics);
        setAdjustedValues({
            bagsCollected: metrics.bagsCollected ?? 0,
            pickedWeight: metrics.pickedWeight ?? 0,
            pickedWeightUnitId: metrics.pickedWeightUnitId ?? 1,
            durationMinutes: metrics.durationMinutes ?? 0,
        });
        setAdjustmentReason('');
        setAdjustDialogOpen(true);
    }, []);

    const handleAdjustConfirm = useCallback(() => {
        if (!selectedMetrics) return;
        adjustMetrics(
            { eventId, metricsId: selectedMetrics.id },
            {
                adjustedBagsCollected: adjustedValues.bagsCollected,
                adjustedPickedWeight: adjustedValues.pickedWeight,
                adjustedPickedWeightUnitId: adjustedValues.pickedWeightUnitId,
                adjustedDurationMinutes: adjustedValues.durationMinutes,
                adjustmentReason: adjustmentReason || 'Values adjusted by lead',
            },
        );
    }, [eventId, selectedMetrics, adjustedValues, adjustmentReason, adjustMetrics]);

    const handleApproveAll = useCallback(() => {
        approveAll({ eventId });
    }, [eventId, approveAll]);

    const formatDate = (date: Date | null | undefined) => {
        if (!date) return '-';
        return moment(date).format('MMM D, YYYY h:mm A');
    };

    const getWeightUnitName = (unitId: number | undefined) => {
        if (!unitId) return '';
        return weightUnits?.find((u) => u.id === unitId)?.name || '';
    };

    // Handle 403 error - user is not an event lead
    if (metricsError?.response?.status === 403) {
        return (
            <div className='container py-8'>
                <Card>
                    <CardContent className='py-12 text-center'>
                        <X className='h-12 w-12 mx-auto text-destructive mb-4' />
                        <h3 className='text-lg font-medium mb-2'>Access Denied</h3>
                        <p className='text-muted-foreground mb-4'>
                            You must be an event lead to review attendee metrics.
                        </p>
                        <Button variant='outline' asChild>
                            <Link to={`/eventdetails/${eventId}`}>
                                <ArrowLeft className='h-4 w-4 mr-2' />
                                Back to Event
                            </Link>
                        </Button>
                    </CardContent>
                </Card>
            </div>
        );
    }

    if (eventLoading || metricsLoading || pendingLoading || totalsLoading) {
        return (
            <div className='container py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='container py-8 space-y-6'>
            {/* Header */}
            <div className='flex items-center gap-4'>
                <Button variant='ghost' size='icon' asChild>
                    <Link to={`/eventdetails/${eventId}`}>
                        <ArrowLeft className='h-4 w-4' />
                    </Link>
                </Button>
                <div>
                    <h1 className='text-2xl font-bold'>Attendee Metrics Review</h1>
                    <p className='text-muted-foreground'>{event?.name}</p>
                </div>
            </div>

            {/* Stats Cards */}
            {totals ? (
                <div className='grid gap-4 md:grid-cols-4'>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>Total Submissions</CardTitle>
                            <Users className='h-4 w-4 text-muted-foreground' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold'>{totals.totalSubmissions}</div>
                            <p className='text-xs text-muted-foreground'>{totals.pendingSubmissions} pending review</p>
                        </CardContent>
                    </Card>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>Bags Collected</CardTitle>
                            <Package className='h-4 w-4 text-muted-foreground' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold'>{totals.totalBagsCollected}</div>
                            <p className='text-xs text-muted-foreground'>From approved submissions</p>
                        </CardContent>
                    </Card>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>Weight Collected</CardTitle>
                            <Scale className='h-4 w-4 text-muted-foreground' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold'>{totals.totalWeightPounds.toFixed(1)} lbs</div>
                            <p className='text-xs text-muted-foreground'>From approved submissions</p>
                        </CardContent>
                    </Card>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>Total Duration</CardTitle>
                            <Clock className='h-4 w-4 text-muted-foreground' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold'>
                                {Math.floor(totals.totalDurationMinutes / 60)}h {totals.totalDurationMinutes % 60}m
                            </div>
                            <p className='text-xs text-muted-foreground'>From approved submissions</p>
                        </CardContent>
                    </Card>
                </div>
            ) : null}

            {/* Metrics Table */}
            <Card>
                <CardHeader>
                    <div className='flex items-center justify-between'>
                        <div>
                            <CardTitle className='flex items-center gap-2'>
                                <ClipboardList className='h-5 w-5' />
                                Metrics Submissions
                            </CardTitle>
                            <CardDescription>Review and manage attendee-submitted metrics.</CardDescription>
                        </div>
                        {(pendingMetrics?.length ?? 0) > 0 && (
                            <AlertDialog>
                                <AlertDialogTrigger asChild>
                                    <Button variant='outline' disabled={isApprovingAll}>
                                        {isApprovingAll ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                                        <CheckCheck className='h-4 w-4 mr-2' />
                                        Approve All ({pendingMetrics?.length})
                                    </Button>
                                </AlertDialogTrigger>
                                <AlertDialogContent>
                                    <AlertDialogHeader>
                                        <AlertDialogTitle>Approve All Pending</AlertDialogTitle>
                                        <AlertDialogDescription>
                                            Are you sure you want to approve all {pendingMetrics?.length} pending
                                            submissions? This action cannot be undone.
                                        </AlertDialogDescription>
                                    </AlertDialogHeader>
                                    <AlertDialogFooter>
                                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                                        <AlertDialogAction onClick={handleApproveAll}>Approve All</AlertDialogAction>
                                    </AlertDialogFooter>
                                </AlertDialogContent>
                            </AlertDialog>
                        )}
                    </div>
                </CardHeader>
                <CardContent>
                    <Tabs defaultValue='pending'>
                        <TabsList className='mb-4'>
                            <TabsTrigger value='pending'>Pending ({pendingMetrics?.length || 0})</TabsTrigger>
                            <TabsTrigger value='approved'>Approved ({approvedMetrics?.length || 0})</TabsTrigger>
                            <TabsTrigger value='rejected'>Rejected ({rejectedMetrics?.length || 0})</TabsTrigger>
                        </TabsList>

                        <TabsContent value='pending'>
                            {pendingMetrics && pendingMetrics.length > 0 ? (
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead>Attendee</TableHead>
                                            <TableHead>Bags</TableHead>
                                            <TableHead>Weight</TableHead>
                                            <TableHead>Duration</TableHead>
                                            <TableHead>Notes</TableHead>
                                            <TableHead>Submitted</TableHead>
                                            <TableHead className='text-right'>Actions</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        {pendingMetrics.map((metrics) => (
                                            <TableRow key={metrics.id}>
                                                <TableCell className='font-medium'>
                                                    {metrics.userName || 'Unknown'}
                                                </TableCell>
                                                <TableCell>{metrics.bagsCollected ?? '-'}</TableCell>
                                                <TableCell>
                                                    {metrics.pickedWeight ?? '-'}{' '}
                                                    {getWeightUnitName(metrics.pickedWeightUnitId)}
                                                </TableCell>
                                                <TableCell>
                                                    {metrics.durationMinutes ? `${metrics.durationMinutes} min` : '-'}
                                                </TableCell>
                                                <TableCell className='max-w-xs truncate'>
                                                    {metrics.notes || '-'}
                                                </TableCell>
                                                <TableCell>{formatDate(metrics.createdDate)}</TableCell>
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
                                                                        Approve Submission
                                                                    </AlertDialogTitle>
                                                                    <AlertDialogDescription>
                                                                        Are you sure you want to approve this submission
                                                                        from <strong>{metrics.userName}</strong>?
                                                                    </AlertDialogDescription>
                                                                </AlertDialogHeader>
                                                                <AlertDialogFooter>
                                                                    <AlertDialogCancel>Cancel</AlertDialogCancel>
                                                                    <AlertDialogAction
                                                                        onClick={() => handleApprove(metrics.id)}
                                                                    >
                                                                        Approve
                                                                    </AlertDialogAction>
                                                                </AlertDialogFooter>
                                                            </AlertDialogContent>
                                                        </AlertDialog>
                                                        <Button
                                                            variant='outline'
                                                            size='sm'
                                                            onClick={() => handleAdjustClick(metrics)}
                                                        >
                                                            <Pencil className='h-4 w-4 mr-1' />
                                                            Adjust
                                                        </Button>
                                                        <Button
                                                            variant='outline'
                                                            size='sm'
                                                            className='text-red-600'
                                                            disabled={isRejecting}
                                                            onClick={() => handleRejectClick(metrics)}
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
                                    <h3 className='text-lg font-medium mb-2'>No pending submissions</h3>
                                    <p className='text-muted-foreground'>
                                        Attendee metrics submissions will appear here for your review.
                                    </p>
                                </div>
                            )}
                        </TabsContent>

                        <TabsContent value='approved'>
                            {approvedMetrics && approvedMetrics.length > 0 ? (
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead>Attendee</TableHead>
                                            <TableHead>Status</TableHead>
                                            <TableHead>Bags</TableHead>
                                            <TableHead>Weight</TableHead>
                                            <TableHead>Duration</TableHead>
                                            <TableHead>Reviewed</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        {approvedMetrics.map((metrics) => {
                                            const displayBags =
                                                metrics.status === 'Adjusted'
                                                    ? metrics.adjustedBagsCollected
                                                    : metrics.bagsCollected;
                                            const displayWeight =
                                                metrics.status === 'Adjusted'
                                                    ? metrics.adjustedPickedWeight
                                                    : metrics.pickedWeight;
                                            const displayWeightUnit =
                                                metrics.status === 'Adjusted'
                                                    ? metrics.adjustedPickedWeightUnitId
                                                    : metrics.pickedWeightUnitId;
                                            const displayDuration =
                                                metrics.status === 'Adjusted'
                                                    ? metrics.adjustedDurationMinutes
                                                    : metrics.durationMinutes;
                                            return (
                                                <TableRow key={metrics.id}>
                                                    <TableCell className='font-medium'>
                                                        {metrics.userName || 'Unknown'}
                                                    </TableCell>
                                                    <TableCell>
                                                        <Badge className={statusColors[metrics.status]}>
                                                            {metrics.status}
                                                        </Badge>
                                                    </TableCell>
                                                    <TableCell>{displayBags ?? '-'}</TableCell>
                                                    <TableCell>
                                                        {displayWeight ?? '-'} {getWeightUnitName(displayWeightUnit)}
                                                    </TableCell>
                                                    <TableCell>
                                                        {displayDuration ? `${displayDuration} min` : '-'}
                                                    </TableCell>
                                                    <TableCell>{formatDate(metrics.reviewedDate)}</TableCell>
                                                </TableRow>
                                            );
                                        })}
                                    </TableBody>
                                </Table>
                            ) : (
                                <div className='text-center py-12'>
                                    <Check className='h-12 w-12 mx-auto text-muted-foreground mb-4' />
                                    <h3 className='text-lg font-medium mb-2'>No approved submissions yet</h3>
                                    <p className='text-muted-foreground'>
                                        Approved submissions will appear here after review.
                                    </p>
                                </div>
                            )}
                        </TabsContent>

                        <TabsContent value='rejected'>
                            {rejectedMetrics && rejectedMetrics.length > 0 ? (
                                <Table>
                                    <TableHeader>
                                        <TableRow>
                                            <TableHead>Attendee</TableHead>
                                            <TableHead>Bags</TableHead>
                                            <TableHead>Weight</TableHead>
                                            <TableHead>Duration</TableHead>
                                            <TableHead>Rejection Reason</TableHead>
                                            <TableHead>Reviewed</TableHead>
                                        </TableRow>
                                    </TableHeader>
                                    <TableBody>
                                        {rejectedMetrics.map((metrics) => (
                                            <TableRow key={metrics.id}>
                                                <TableCell className='font-medium'>
                                                    {metrics.userName || 'Unknown'}
                                                </TableCell>
                                                <TableCell>{metrics.bagsCollected ?? '-'}</TableCell>
                                                <TableCell>
                                                    {metrics.pickedWeight ?? '-'}{' '}
                                                    {getWeightUnitName(metrics.pickedWeightUnitId)}
                                                </TableCell>
                                                <TableCell>
                                                    {metrics.durationMinutes ? `${metrics.durationMinutes} min` : '-'}
                                                </TableCell>
                                                <TableCell className='max-w-xs truncate'>
                                                    {metrics.rejectionReason || '-'}
                                                </TableCell>
                                                <TableCell>{formatDate(metrics.reviewedDate)}</TableCell>
                                            </TableRow>
                                        ))}
                                    </TableBody>
                                </Table>
                            ) : (
                                <div className='text-center py-12'>
                                    <X className='h-12 w-12 mx-auto text-muted-foreground mb-4' />
                                    <h3 className='text-lg font-medium mb-2'>No rejected submissions</h3>
                                    <p className='text-muted-foreground'>Rejected submissions will appear here.</p>
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
                        <DialogTitle>Reject Submission</DialogTitle>
                        <DialogDescription>
                            Please provide a reason for rejecting this submission. The attendee will be notified and can
                            resubmit.
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
                            Reject Submission
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>

            {/* Adjust Dialog */}
            <Dialog open={adjustDialogOpen} onOpenChange={setAdjustDialogOpen}>
                <DialogContent className='sm:max-w-[500px]'>
                    <DialogHeader>
                        <DialogTitle>Adjust Submission</DialogTitle>
                        <DialogDescription>
                            Modify the values and provide a reason. The adjusted values will be used in totals.
                        </DialogDescription>
                    </DialogHeader>
                    <div className='space-y-4 py-4'>
                        <div className='grid grid-cols-2 gap-4'>
                            <div className='space-y-2'>
                                <Label htmlFor='adjust-bags'>Bags Collected</Label>
                                <Input
                                    id='adjust-bags'
                                    type='number'
                                    min={0}
                                    value={adjustedValues.bagsCollected}
                                    onChange={(e) =>
                                        setAdjustedValues((prev) => ({
                                            ...prev,
                                            bagsCollected: e.target.valueAsNumber || 0,
                                        }))
                                    }
                                />
                            </div>
                            <div className='space-y-2'>
                                <Label htmlFor='adjust-duration'>Duration (minutes)</Label>
                                <Input
                                    id='adjust-duration'
                                    type='number'
                                    min={0}
                                    value={adjustedValues.durationMinutes}
                                    onChange={(e) =>
                                        setAdjustedValues((prev) => ({
                                            ...prev,
                                            durationMinutes: e.target.valueAsNumber || 0,
                                        }))
                                    }
                                />
                            </div>
                            <div className='space-y-2'>
                                <Label htmlFor='adjust-weight'>Weight</Label>
                                <Input
                                    id='adjust-weight'
                                    type='number'
                                    min={0}
                                    step={0.1}
                                    value={adjustedValues.pickedWeight}
                                    onChange={(e) =>
                                        setAdjustedValues((prev) => ({
                                            ...prev,
                                            pickedWeight: e.target.valueAsNumber || 0,
                                        }))
                                    }
                                />
                            </div>
                            <div className='space-y-2'>
                                <Label htmlFor='adjust-weight-unit'>Weight Unit</Label>
                                <Select
                                    value={`${adjustedValues.pickedWeightUnitId}`}
                                    onValueChange={(val) =>
                                        setAdjustedValues((prev) => ({
                                            ...prev,
                                            pickedWeightUnitId: Number(val),
                                        }))
                                    }
                                >
                                    <SelectTrigger id='adjust-weight-unit'>
                                        <SelectValue placeholder='Weight Unit' />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {(weightUnits || []).map((unit) => (
                                            <SelectItem key={unit.id} value={`${unit.id}`}>
                                                {unit.name}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            </div>
                        </div>
                        <div className='space-y-2'>
                            <Label htmlFor='adjustment-reason'>Reason for Adjustment</Label>
                            <Textarea
                                id='adjustment-reason'
                                placeholder='Why are these values being adjusted?'
                                value={adjustmentReason}
                                onChange={(e) => setAdjustmentReason(e.target.value)}
                                className='h-20'
                            />
                        </div>
                    </div>
                    <DialogFooter>
                        <Button variant='outline' onClick={() => setAdjustDialogOpen(false)}>
                            Cancel
                        </Button>
                        <Button onClick={handleAdjustConfirm} disabled={isAdjusting}>
                            {isAdjusting ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                            Save & Approve
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </div>
    );
};

export default AttendeeMetricsReview;
