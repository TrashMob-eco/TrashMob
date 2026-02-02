import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Flag, Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import { FlagPhoto, PhotoType } from '@/services/photo-moderation';

interface ReportPhotoButtonProps {
    photoId: string;
    photoType: PhotoType;
    variant?: 'default' | 'ghost' | 'outline' | 'icon';
    size?: 'default' | 'sm' | 'lg' | 'icon';
    className?: string;
}

const FLAG_REASONS = [
    'Inappropriate content',
    'Offensive or harmful',
    'Copyright violation',
    'Spam or advertising',
    'Not related to litter/cleanup',
    'Contains personal information',
    'Other',
];

export const ReportPhotoButton = ({
    photoId,
    photoType,
    variant = 'ghost',
    size = 'sm',
    className,
}: ReportPhotoButtonProps) => {
    const { toast } = useToast();
    const [isOpen, setIsOpen] = useState(false);
    const [reason, setReason] = useState('');
    const [details, setDetails] = useState('');

    const flagMutation = useMutation({
        mutationKey: FlagPhoto().key,
        mutationFn: (params: { photoType: PhotoType; id: string; reason: string }) =>
            FlagPhoto().service({ photoType: params.photoType, id: params.id }, { reason: params.reason }),
        onSuccess: () => {
            toast({
                title: 'Report submitted',
                description: 'Thank you for reporting this content. Our team will review it shortly.',
            });
            handleClose();
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to submit report. Please try again.',
            });
        },
    });

    const handleClose = () => {
        setIsOpen(false);
        setReason('');
        setDetails('');
    };

    const handleSubmit = () => {
        if (!reason) return;

        const fullReason = details ? `${reason}: ${details}` : reason;
        flagMutation.mutate({
            photoType,
            id: photoId,
            reason: fullReason,
        });
    };

    return (
        <>
            <Button variant={variant} size={size} onClick={() => setIsOpen(true)} className={className} title='Report'>
                <Flag className='h-4 w-4' />
                {size !== 'icon' && <span className='ml-1'>Report</span>}
            </Button>

            <Dialog open={isOpen} onOpenChange={handleClose}>
                <DialogContent className='sm:max-w-[425px]'>
                    <DialogHeader>
                        <DialogTitle>Report Content</DialogTitle>
                        <DialogDescription>
                            Help us keep TrashMob safe by reporting inappropriate content. Our moderation team will
                            review your report.
                        </DialogDescription>
                    </DialogHeader>

                    <div className='grid gap-4 py-4'>
                        <div className='grid gap-2'>
                            <Label htmlFor='reason'>Reason for reporting</Label>
                            <Select value={reason} onValueChange={setReason}>
                                <SelectTrigger id='reason'>
                                    <SelectValue placeholder='Select a reason...' />
                                </SelectTrigger>
                                <SelectContent>
                                    {FLAG_REASONS.map((r) => (
                                        <SelectItem key={r} value={r}>
                                            {r}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                        </div>

                        {reason === 'Other' && (
                            <div className='grid gap-2'>
                                <Label htmlFor='details'>Additional details</Label>
                                <Textarea
                                    id='details'
                                    placeholder='Please describe why you are reporting this content...'
                                    value={details}
                                    onChange={(e) => setDetails(e.target.value)}
                                    rows={3}
                                />
                            </div>
                        )}
                    </div>

                    <DialogFooter>
                        <Button variant='outline' onClick={handleClose} disabled={flagMutation.isPending}>
                            Cancel
                        </Button>
                        <Button
                            onClick={handleSubmit}
                            disabled={!reason || (reason === 'Other' && !details) || flagMutation.isPending}
                        >
                            {flagMutation.isPending ? <Loader2 className='h-4 w-4 mr-2 animate-spin' /> : null}
                            Submit Report
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </>
    );
};
