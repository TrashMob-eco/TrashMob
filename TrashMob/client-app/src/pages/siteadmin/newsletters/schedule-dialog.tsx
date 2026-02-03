import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { ScheduleNewsletter } from '@/services/newsletters';

interface ScheduleDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    newsletterId: string | null;
}

export const ScheduleDialog = ({ open, onOpenChange, newsletterId }: ScheduleDialogProps) => {
    const queryClient = useQueryClient();
    const [scheduledDate, setScheduledDate] = useState('');
    const [scheduledTime, setScheduledTime] = useState('09:00');

    const scheduleMutation = useMutation({
        mutationKey: ScheduleNewsletter().key,
        mutationFn: ({ id, scheduledDate }: { id: string; scheduledDate: string }) =>
            ScheduleNewsletter().service({ id }, { scheduledDate }),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/admin/newsletters'] });
            onOpenChange(false);
            setScheduledDate('');
            setScheduledTime('09:00');
        },
    });

    const handleSchedule = () => {
        if (!newsletterId || !scheduledDate) return;

        const dateTime = new Date(`${scheduledDate}T${scheduledTime}`);
        scheduleMutation.mutate({
            id: newsletterId,
            scheduledDate: dateTime.toISOString(),
        });
    };

    // Get tomorrow's date as minimum
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const minDate = tomorrow.toISOString().split('T')[0];

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Schedule Newsletter</DialogTitle>
                    <DialogDescription>
                        Choose when you want this newsletter to be sent.
                    </DialogDescription>
                </DialogHeader>

                <div className='grid gap-4 py-4'>
                    <div className='space-y-2'>
                        <Label htmlFor='date'>Date</Label>
                        <Input
                            id='date'
                            type='date'
                            value={scheduledDate}
                            onChange={(e) => setScheduledDate(e.target.value)}
                            min={minDate}
                        />
                    </div>

                    <div className='space-y-2'>
                        <Label htmlFor='time'>Time</Label>
                        <Input
                            id='time'
                            type='time'
                            value={scheduledTime}
                            onChange={(e) => setScheduledTime(e.target.value)}
                        />
                    </div>
                </div>

                <DialogFooter>
                    <Button variant='outline' onClick={() => onOpenChange(false)}>
                        Cancel
                    </Button>
                    <Button onClick={handleSchedule} disabled={scheduleMutation.isPending || !scheduledDate}>
                        {scheduleMutation.isPending ? 'Scheduling...' : 'Schedule'}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
