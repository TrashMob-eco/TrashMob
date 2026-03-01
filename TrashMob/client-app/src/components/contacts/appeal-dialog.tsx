import { useCallback, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import { SendAppeal } from '@/services/contacts';

const appealSchema = z.object({
    subject: z.string().min(1, 'Subject is required'),
    body: z.string().min(1, 'Message body is required'),
});

type AppealFormInputs = z.infer<typeof appealSchema>;

interface AppealDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    contactId: string;
    contactName: string;
    contactEmail: string;
}

export const AppealDialog = ({ open, onOpenChange, contactId, contactName, contactEmail }: AppealDialogProps) => {
    const { toast } = useToast();

    const sendAppeal = useMutation({
        mutationKey: SendAppeal().key,
        mutationFn: SendAppeal().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Appeal sent successfully' });
            onOpenChange(false);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to send appeal' });
        },
    });

    const form = useForm<AppealFormInputs>({
        resolver: zodResolver(appealSchema),
        defaultValues: { subject: '', body: '' },
    });

    useEffect(() => {
        if (open) {
            form.reset({ subject: '', body: '' });
        }
    }, [open, form]);

    const onSubmit: SubmitHandler<AppealFormInputs> = useCallback(
        (values) => {
            sendAppeal.mutate({
                contactId,
                subject: values.subject,
                body: values.body,
            });
        },
        [contactId, sendAppeal],
    );

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className='sm:max-w-[550px]' onOpenAutoFocus={(e) => e.preventDefault()}>
                <DialogHeader>
                    <DialogTitle>Send Appeal</DialogTitle>
                    <DialogDescription>
                        Send a fundraising appeal email to {contactName} ({contactEmail}).
                    </DialogDescription>
                </DialogHeader>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
                        <FormField
                            control={form.control}
                            name='subject'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Subject</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Email subject line' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='body'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Message</FormLabel>
                                    <FormControl>
                                        <Textarea
                                            {...field}
                                            placeholder='Write your appeal message...'
                                            rows={8}
                                        />
                                    </FormControl>
                                    <p className='text-xs text-muted-foreground mt-1'>
                                        The recipient&apos;s name will be automatically inserted in the greeting.
                                    </p>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <div className='flex justify-end gap-2'>
                            <Button type='button' variant='secondary' onClick={() => onOpenChange(false)}>
                                Cancel
                            </Button>
                            <Button type='submit' disabled={sendAppeal.isPending}>
                                {sendAppeal.isPending ? <Loader2 className='animate-spin' /> : null}
                                Send Appeal
                            </Button>
                        </div>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    );
};
