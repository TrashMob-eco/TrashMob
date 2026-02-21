import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
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
import { SendTestEmail } from '@/services/newsletters';

interface TestSendDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    newsletterId: string | null;
}

export const TestSendDialog = ({ open, onOpenChange, newsletterId }: TestSendDialogProps) => {
    const [testEmail, setTestEmail] = useState('');
    const [error, setError] = useState('');

    const testSendMutation = useMutation({
        mutationKey: SendTestEmail().key,
        mutationFn: ({ id, emails }: { id: string; emails: string[] }) => SendTestEmail().service({ id }, { emails }),
        onSuccess: () => {
            onOpenChange(false);
            setTestEmail('');
            setError('');
        },
        onError: (err: Error) => {
            setError(err.message || 'Failed to send test email');
        },
    });

    const handleSendTest = () => {
        if (!newsletterId || !testEmail) return;

        setError('');
        const emails = testEmail
            .split(/[\n,]/)
            .map((e) => e.trim())
            .filter((e) => e.length > 0);

        if (emails.length === 0) {
            setError('Please enter at least one email address');
            return;
        }

        const invalidEmails = emails.filter((e) => !e.includes('@'));
        if (invalidEmails.length > 0) {
            setError('Invalid email addresses: ' + invalidEmails.slice(0, 3).join(', '));
            return;
        }

        testSendMutation.mutate({ id: newsletterId, emails });
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Send Test Email</DialogTitle>
                    <DialogDescription>
                        Send a test email to verify the newsletter content before sending to all recipients.
                    </DialogDescription>
                </DialogHeader>

                <div className='grid gap-4 py-4'>
                    <div className='space-y-2'>
                        <Label htmlFor='testEmail'>Email Addresses</Label>
                        <Input
                            id='testEmail'
                            type='text'
                            value={testEmail}
                            onChange={(e) => setTestEmail(e.target.value)}
                            placeholder='test@example.com'
                        />
                        <p className='text-xs text-muted-foreground'>
                            Enter one or more email addresses, separated by commas
                        </p>
                    </div>
                    {error ? <p className='text-sm text-red-600'>{error}</p> : null}
                </div>

                <DialogFooter>
                    <Button variant='outline' onClick={() => onOpenChange(false)}>
                        Cancel
                    </Button>
                    <Button onClick={handleSendTest} disabled={testSendMutation.isPending || !testEmail}>
                        {testSendMutation.isPending ? 'Sending...' : 'Send Test'}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
