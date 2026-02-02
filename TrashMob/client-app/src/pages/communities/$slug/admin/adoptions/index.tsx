import { useCallback, useState } from 'react';
import { useParams } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Loader2, ClipboardList, Check, X, Users } from 'lucide-react';

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
import { GetCommunityBySlug } from '@/services/communities';
import {
    GetPendingApplications,
    GetApprovedAdoptions,
    ApproveAdoption,
    RejectAdoption,
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

    if (pendingLoading || approvedLoading) {
        return (
            <div className='container py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='container py-8'>
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <ClipboardList className='h-5 w-5' />
                        Adoption Applications
                    </CardTitle>
                    <CardDescription>
                        Review and manage team applications to adopt areas in your community.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <Tabs defaultValue='pending'>
                        <TabsList className='mb-4'>
                            <TabsTrigger value='pending'>Pending ({pendingApplications?.length || 0})</TabsTrigger>
                            <TabsTrigger value='approved'>Approved ({approvedAdoptions?.length || 0})</TabsTrigger>
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
                                            <TableHead>Status</TableHead>
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
                                                    <Badge className={statusColors[adoption.status]}>
                                                        {adoption.status}
                                                    </Badge>
                                                </TableCell>
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
