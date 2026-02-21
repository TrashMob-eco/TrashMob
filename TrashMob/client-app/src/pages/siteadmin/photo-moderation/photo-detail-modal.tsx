import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { CheckCircle, XCircle, Flag, ExternalLink, Image } from 'lucide-react';
import {
    PhotoModerationItem,
    PhotoType,
    ApprovePhoto,
    RejectPhoto,
    DismissFlag,
    GetPendingPhotos,
    GetFlaggedPhotos,
    GetModeratedPhotos,
} from '@/services/photo-moderation';

interface PhotoDetailModalProps {
    photo: PhotoModerationItem | null;
    open: boolean;
    onOpenChange: (open: boolean) => void;
    tab: 'pending' | 'flagged' | 'moderated';
}

const REJECTION_REASONS = [
    'Inappropriate content',
    'Violates community guidelines',
    'Copyright violation',
    'Spam or advertising',
    'Poor quality / unrelated',
    'Contains personal information',
    'Other',
];

const statusLabels: Record<number, { label: string; color: string }> = {
    0: { label: 'Pending', color: 'bg-yellow-500' },
    1: { label: 'Approved', color: 'bg-green-500' },
    2: { label: 'Rejected', color: 'bg-red-500' },
};

const photoTypeBadge: Record<string, { label: string; color: string }> = {
    LitterImage: { label: 'Litter Image', color: 'bg-blue-500' },
    TeamPhoto: { label: 'Team Photo', color: 'bg-purple-500' },
    EventPhoto: { label: 'Event Photo', color: 'bg-green-600' },
    PartnerPhoto: { label: 'Community Photo', color: 'bg-orange-500' },
};

export const PhotoDetailModal = ({ photo, open, onOpenChange, tab }: PhotoDetailModalProps) => {
    const queryClient = useQueryClient();
    const [rejectReason, setRejectReason] = useState<string>('');
    const [showRejectForm, setShowRejectForm] = useState(false);

    const invalidateQueries = () => {
        queryClient.invalidateQueries({ queryKey: GetPendingPhotos().key });
        queryClient.invalidateQueries({ queryKey: GetFlaggedPhotos().key });
        queryClient.invalidateQueries({ queryKey: GetModeratedPhotos().key });
    };

    const approveMutation = useMutation({
        mutationKey: ApprovePhoto().key,
        mutationFn: ApprovePhoto().service,
        onSuccess: () => {
            invalidateQueries();
            onOpenChange(false);
        },
    });

    const rejectMutation = useMutation({
        mutationKey: RejectPhoto().key,
        mutationFn: (params: { photoType: PhotoType; id: string; reason: string }) =>
            RejectPhoto().service({ photoType: params.photoType, id: params.id }, { reason: params.reason }),
        onSuccess: () => {
            invalidateQueries();
            setShowRejectForm(false);
            setRejectReason('');
            onOpenChange(false);
        },
    });

    const dismissMutation = useMutation({
        mutationKey: DismissFlag().key,
        mutationFn: DismissFlag().service,
        onSuccess: () => {
            invalidateQueries();
            onOpenChange(false);
        },
    });

    const handleApprove = () => {
        if (!photo) return;
        approveMutation.mutate({ photoType: photo.photoType, id: photo.photoId });
    };

    const handleReject = () => {
        if (!photo || !rejectReason) return;
        rejectMutation.mutate({
            photoType: photo.photoType,
            id: photo.photoId,
            reason: rejectReason,
        });
    };

    const handleDismiss = () => {
        if (!photo) return;
        dismissMutation.mutate({ photoType: photo.photoType, id: photo.photoId });
    };

    const handleClose = () => {
        setShowRejectForm(false);
        setRejectReason('');
        onOpenChange(false);
    };

    if (!photo) return null;

    const isProcessing = approveMutation.isPending || rejectMutation.isPending || dismissMutation.isPending;
    const statusInfo = statusLabels[photo.moderationStatus] || { label: 'Unknown', color: 'bg-gray-500' };

    return (
        <Dialog open={open} onOpenChange={handleClose}>
            <DialogContent className='sm:max-w-[700px] max-h-[90vh] overflow-y-auto'>
                <DialogHeader>
                    <DialogTitle className='flex items-center gap-2'>
                        Photo Details
                        <Badge className={photoTypeBadge[photo.photoType]?.color || 'bg-gray-500'}>
                            {photoTypeBadge[photo.photoType]?.label || photo.photoType}
                        </Badge>
                        {tab === 'moderated' && <Badge className={statusInfo.color}>{statusInfo.label}</Badge>}
                    </DialogTitle>
                    <DialogDescription>Review photo details and take moderation action</DialogDescription>
                </DialogHeader>

                <div className='grid gap-4'>
                    {/* Photo Preview */}
                    <div className='flex justify-center bg-gray-100 rounded-lg p-4'>
                        {photo.imageUrl ? (
                            <img
                                src={photo.imageUrl}
                                alt='Content pending moderation review'
                                className='max-h-80 object-contain rounded'
                            />
                        ) : (
                            <div className='w-full h-48 flex items-center justify-center text-gray-400'>
                                <Image className='h-16 w-16' />
                            </div>
                        )}
                    </div>

                    {/* Photo Details */}
                    <div className='grid grid-cols-2 gap-4 text-sm'>
                        <div>
                            <Label className='text-muted-foreground'>Uploaded By</Label>
                            <p className='font-medium'>{photo.uploaderName || 'Unknown'}</p>
                            {photo.uploaderEmail ? (
                                <p className='text-muted-foreground text-xs'>{photo.uploaderEmail}</p>
                            ) : null}
                        </div>
                        <div>
                            <Label className='text-muted-foreground'>Uploaded Date</Label>
                            <p className='font-medium'>
                                {photo.uploadedDate ? new Date(photo.uploadedDate).toLocaleString() : '-'}
                            </p>
                        </div>

                        {/* Context - Litter Report or Team */}
                        {photo.litterReportName ? (
                            <div className='col-span-2'>
                                <Label className='text-muted-foreground'>Litter Report</Label>
                                <p className='font-medium flex items-center gap-1'>
                                    {photo.litterReportName}
                                    {photo.litterReportId ? (
                                        <a
                                            href={`/litterreports/${photo.litterReportId}`}
                                            target='_blank'
                                            rel='noopener noreferrer'
                                            className='text-primary hover:underline'
                                        >
                                            <ExternalLink className='h-3 w-3' />
                                        </a>
                                    ) : null}
                                </p>
                            </div>
                        ) : null}
                        {photo.teamName ? (
                            <div className='col-span-2'>
                                <Label className='text-muted-foreground'>Team</Label>
                                <p className='font-medium flex items-center gap-1'>
                                    {photo.teamName}
                                    {photo.teamId ? (
                                        <a
                                            href={`/teams/${photo.teamId}`}
                                            target='_blank'
                                            rel='noopener noreferrer'
                                            className='text-primary hover:underline'
                                        >
                                            <ExternalLink className='h-3 w-3' />
                                        </a>
                                    ) : null}
                                </p>
                            </div>
                        ) : null}
                        {photo.eventName ? (
                            <div className='col-span-2'>
                                <Label className='text-muted-foreground'>Event</Label>
                                <p className='font-medium flex items-center gap-1'>
                                    {photo.eventName}
                                    {photo.eventId ? (
                                        <a
                                            href={`/eventdetails/${photo.eventId}`}
                                            target='_blank'
                                            rel='noopener noreferrer'
                                            className='text-primary hover:underline'
                                        >
                                            <ExternalLink className='h-3 w-3' />
                                        </a>
                                    ) : null}
                                </p>
                            </div>
                        ) : null}
                        {photo.partnerName ? (
                            <div className='col-span-2'>
                                <Label className='text-muted-foreground'>Community</Label>
                                <p className='font-medium'>{photo.partnerName}</p>
                            </div>
                        ) : null}

                        {/* Caption if available */}
                        {photo.caption ? (
                            <div className='col-span-2'>
                                <Label className='text-muted-foreground'>Caption</Label>
                                <p className='font-medium'>{photo.caption}</p>
                            </div>
                        ) : null}

                        {/* Flag info for flagged photos */}
                        {photo.inReview && photo.flagReason ? (
                            <div className='col-span-2 bg-yellow-50 p-3 rounded-md border border-yellow-200'>
                                <Label className='text-yellow-700'>Flag Reason</Label>
                                <p className='font-medium text-yellow-900'>{photo.flagReason}</p>
                                {photo.flaggedDate ? (
                                    <p className='text-xs text-yellow-600 mt-1'>
                                        Flagged on {new Date(photo.flaggedDate).toLocaleString()}
                                    </p>
                                ) : null}
                            </div>
                        ) : null}

                        {/* Moderation info for moderated photos */}
                        {tab === 'moderated' && photo.moderatedDate ? (
                            <div className='col-span-2 bg-gray-50 p-3 rounded-md border'>
                                <div className='flex justify-between'>
                                    <div>
                                        <Label className='text-muted-foreground'>Moderated By</Label>
                                        <p className='font-medium'>{photo.moderatedByName || 'Unknown'}</p>
                                    </div>
                                    <div>
                                        <Label className='text-muted-foreground'>Moderated Date</Label>
                                        <p className='font-medium'>{new Date(photo.moderatedDate).toLocaleString()}</p>
                                    </div>
                                </div>
                                {photo.moderationReason ? (
                                    <div className='mt-2'>
                                        <Label className='text-muted-foreground'>Reason</Label>
                                        <p className='font-medium'>{photo.moderationReason}</p>
                                    </div>
                                ) : null}
                            </div>
                        ) : null}
                    </div>

                    {/* Rejection Form */}
                    {showRejectForm ? (
                        <div className='border-t pt-4'>
                            <Label htmlFor='reject-reason'>Rejection Reason</Label>
                            <Select value={rejectReason} onValueChange={setRejectReason}>
                                <SelectTrigger id='reject-reason' className='mt-1'>
                                    <SelectValue placeholder='Select a reason...' />
                                </SelectTrigger>
                                <SelectContent>
                                    {REJECTION_REASONS.map((reason) => (
                                        <SelectItem key={reason} value={reason}>
                                            {reason}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                        </div>
                    ) : null}
                </div>

                <DialogFooter className='flex-col sm:flex-row gap-2'>
                    {/* Show actions only for pending/flagged photos */}
                    {(tab === 'pending' || tab === 'flagged') && !showRejectForm && (
                        <>
                            <Button
                                variant='outline'
                                onClick={handleApprove}
                                disabled={isProcessing}
                                className='text-green-600 hover:text-green-700 hover:bg-green-50'
                            >
                                <CheckCircle className='mr-2 h-4 w-4' />
                                Approve
                            </Button>
                            <Button
                                variant='outline'
                                onClick={() => setShowRejectForm(true)}
                                disabled={isProcessing}
                                className='text-destructive hover:text-destructive hover:bg-red-50'
                            >
                                <XCircle className='mr-2 h-4 w-4' />
                                Reject
                            </Button>
                            {tab === 'flagged' && (
                                <Button variant='outline' onClick={handleDismiss} disabled={isProcessing}>
                                    <Flag className='mr-2 h-4 w-4' />
                                    Dismiss Flag
                                </Button>
                            )}
                        </>
                    )}

                    {showRejectForm ? (
                        <>
                            <Button variant='outline' onClick={() => setShowRejectForm(false)} disabled={isProcessing}>
                                Cancel
                            </Button>
                            <Button
                                variant='destructive'
                                onClick={handleReject}
                                disabled={isProcessing || !rejectReason}
                            >
                                <XCircle className='mr-2 h-4 w-4' />
                                Confirm Rejection
                            </Button>
                        </>
                    ) : null}

                    {tab === 'moderated' && (
                        <Button variant='outline' onClick={handleClose}>
                            Close
                        </Button>
                    )}
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
