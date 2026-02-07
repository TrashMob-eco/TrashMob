import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Checkbox } from '@/components/ui/checkbox';
import { useToast } from '@/hooks/use-toast';
import { ConvertProspectToPartner, GetCommunityProspectById } from '@/services/community-prospects';

interface ConvertToPartnerDialogProps {
    prospectId: string;
    prospectName: string;
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

export const ConvertToPartnerDialog = ({ prospectId, prospectName, open, onOpenChange }: ConvertToPartnerDialogProps) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [partnerTypeId, setPartnerTypeId] = useState(1);
    const [sendWelcomeEmail, setSendWelcomeEmail] = useState(true);

    const convert = useMutation({
        mutationKey: ConvertProspectToPartner().key,
        mutationFn: ConvertProspectToPartner().service,
        onSuccess: (res) => {
            const result = res.data;
            if (result.success) {
                toast({ variant: 'primary', title: 'Prospect converted to partner successfully' });
                queryClient.invalidateQueries({
                    queryKey: GetCommunityProspectById({ id: prospectId }).key,
                    refetchType: 'all',
                });
                onOpenChange(false);
            } else {
                toast({ variant: 'destructive', title: 'Conversion failed', description: result.errorMessage });
            }
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to convert prospect' });
        },
    });

    function handleConvert() {
        convert.mutate({ id: prospectId, partnerTypeId, sendWelcomeEmail });
    }

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Convert to Partner</DialogTitle>
                    <DialogDescription>
                        Convert <strong>{prospectName}</strong> from a prospect into a TrashMob partner organization.
                    </DialogDescription>
                </DialogHeader>
                <div className='space-y-4'>
                    <div>
                        <label className='text-sm font-medium'>Partner Type</label>
                        <Select value={String(partnerTypeId)} onValueChange={(v) => setPartnerTypeId(Number(v))}>
                            <SelectTrigger>
                                <SelectValue />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value='1'>Government</SelectItem>
                                <SelectItem value='2'>Business</SelectItem>
                            </SelectContent>
                        </Select>
                    </div>
                    <div className='flex items-center gap-2'>
                        <Checkbox
                            id='sendWelcome'
                            checked={sendWelcomeEmail}
                            onCheckedChange={(checked) => setSendWelcomeEmail(checked === true)}
                        />
                        <label htmlFor='sendWelcome' className='text-sm'>
                            Send welcome email to contact
                        </label>
                    </div>
                </div>
                <DialogFooter>
                    <Button variant='outline' onClick={() => onOpenChange(false)}>
                        Cancel
                    </Button>
                    <Button onClick={handleConvert} disabled={convert.isPending}>
                        {convert.isPending ? 'Converting...' : 'Convert to Partner'}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
