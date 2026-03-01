import { Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import DonationData from '@/components/Models/DonationData';
import { getDonationTypeLabel } from '@/components/contacts/contact-constants';

interface ReceiptDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    donation: DonationData | null;
    contactName: string;
    contactEmail: string;
    isPending: boolean;
    onConfirm: () => void;
}

export const ReceiptDialog = ({
    open,
    onOpenChange,
    donation,
    contactName,
    contactEmail,
    isPending,
    onConfirm,
}: ReceiptDialogProps) => {
    if (!donation) return null;

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className='sm:max-w-[450px]'>
                <DialogHeader>
                    <DialogTitle>Send Tax Receipt</DialogTitle>
                    <DialogDescription>
                        This will send a tax receipt email with a PDF attachment and log it as a contact note.
                    </DialogDescription>
                </DialogHeader>
                <div className='space-y-3 py-2'>
                    <div className='grid grid-cols-2 gap-2 text-sm'>
                        <div className='text-muted-foreground'>Recipient</div>
                        <div className='font-medium'>{contactName}</div>
                        <div className='text-muted-foreground'>Email</div>
                        <div className='font-medium'>{contactEmail}</div>
                        <div className='text-muted-foreground'>Amount</div>
                        <div className='font-medium'>${donation.amount.toLocaleString()}</div>
                        <div className='text-muted-foreground'>Date</div>
                        <div className='font-medium'>
                            {donation.donationDate
                                ? new Date(donation.donationDate).toLocaleDateString()
                                : '—'}
                        </div>
                        <div className='text-muted-foreground'>Type</div>
                        <div className='font-medium'>{getDonationTypeLabel(donation.donationType)}</div>
                    </div>
                </div>
                <DialogFooter>
                    <Button variant='secondary' onClick={() => onOpenChange(false)}>
                        Cancel
                    </Button>
                    <Button onClick={onConfirm} disabled={isPending}>
                        {isPending ? <Loader2 className='animate-spin' /> : null}
                        Send Receipt
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
