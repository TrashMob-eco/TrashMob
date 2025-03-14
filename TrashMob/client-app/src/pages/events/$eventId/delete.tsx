import * as React from 'react';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Loader2 } from 'lucide-react';
import { useNavigate, useParams } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';

import { Button } from '@/components/ui/button';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Input } from '@/components/ui/input';
import { ShareToSocialsDialog } from '@/components/EventManagement/ShareToSocialsDialog';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';

import * as ToolTips from '@/store/ToolTips';
import * as SharingMessages from '@/store/SharingMessages';
import { DeleteEvent, GetUserEvents } from '@/services/events';
import { useGetEvent } from '@/hooks/useGetEvent';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';

const cancelEventSchema = z.object({
    cancellationReason: z.string().min(1),
});

export const CancelEvent: React.FC = () => {
    const { eventId } = useParams<{ eventId: string }>() as { eventId: string };
    const { currentUser } = useLogin();
    const { toast } = useToast();
    const { data: event } = useGetEvent(eventId);
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const deleteEvent = useMutation({
        mutationKey: DeleteEvent().key,
        mutationFn: DeleteEvent().service,
        onSuccess: async (_data, variables) => {
            console.log('onSuccess', { _data, variables });
            handleShowModal(true);

            toast({
                variant: 'destructive',
                title: 'Event cancelled',
                description: `Reason: ${variables.cancellationReason}`,
            });

            await queryClient.invalidateQueries({
                queryKey: GetUserEvents({ userId: currentUser.id }).key,
            });
        },
    });

    const form = useForm<z.infer<typeof cancelEventSchema>>({
        resolver: zodResolver(cancelEventSchema),
    });

    const onSubmit = (values: z.infer<typeof cancelEventSchema>) => {
        const { cancellationReason } = values;
        return deleteEvent.mutateAsync({ eventId, cancellationReason });
    };

    const [showModal, setShowSocialsModal] = React.useState<boolean>(false);

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        navigate(-1);
    }

    const handleShowModal = (showModal: boolean) => {
        // if modal is being dismissed, route user back to previous page
        if (!showModal) navigate(-1);
        else setShowSocialsModal(showModal);
    };

    const cancellationReason = form.watch('cancellationReason');

    return (
        <div className='tailwind'>
            {event ? (
                <ShareToSocialsDialog
                    eventToShare={event}
                    show={showModal}
                    handleShow={handleShowModal}
                    modalTitle='Share Event Cancellation'
                    message={SharingMessages.getCancellationMessage(event, cancellationReason)}
                    eventLink='https://www.trashmob.eco'
                    emailSubject='TrashMob Event Cancellation'
                />
            ) : null}
            <div className='w-full max-w-xl mx-auto py-16'>
                <Card>
                    <Form {...form}>
                        <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-2'>
                            <CardHeader>
                                <CardTitle>Cancel Event</CardTitle>
                                <CardDescription>
                                    Are you sure you want to cancel the Event <strong>{event?.name}</strong>?
                                </CardDescription>
                            </CardHeader>
                            <CardContent>
                                <FormField
                                    control={form.control}
                                    name='cancellationReason'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required tooltip={ToolTips.EventCancellationReason}>
                                                Reason for Cancelling the Event?
                                            </FormLabel>
                                            <FormControl>
                                                <Input {...field} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </CardContent>
                            <CardFooter className='flex gap-2 justify-end'>
                                <Button variant='outline' onClick={handleCancel}>
                                    Back
                                </Button>
                                <Button
                                    type='submit'
                                    disabled={!form.formState.isValid || deleteEvent.isLoading}
                                    variant='destructive'
                                >
                                    {deleteEvent.isLoading ? <Loader2 className='animate-spin' /> : null}
                                    Cancel Event
                                </Button>
                            </CardFooter>
                        </form>
                    </Form>
                </Card>
            </div>
        </div>
    );
};
