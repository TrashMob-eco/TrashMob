import { useCallback } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { Loader2 } from 'lucide-react';
import { useNavigate } from 'react-router';
import { useMutation } from '@tanstack/react-query';

import * as ToolTips from '@/store/ToolTips';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import MessageRequestData from '@/components/Models/MessageRequestData';
import { CreateMessageRequest } from '@/services/message';
import { Button } from '@/components/ui/button';

const sendNotificationSchema = z.object({
    name: z.string().min(1, 'Name cannot be empty').max(64, 'Name must be less than 64 characters'),
    message: z
        .string()
        .min(1, 'Message cannot be empty')
        .max(1000, 'Message cannot be more than 1000 characters long.'),
});

export const SiteAdminSendNotification = () => {
    const navigate = useNavigate();
    const { toast } = useToast();

    const createMessageRequest = useMutation({
        mutationKey: CreateMessageRequest().key,
        mutationFn: CreateMessageRequest().service,
        onSuccess: () => {
            toast({
                variant: 'primary',
                title: 'Message sent',
            });
        },
    });

    const form = useForm<z.infer<typeof sendNotificationSchema>>({
        resolver: zodResolver(sendNotificationSchema),
    });

    const onSubmit = useCallback(async (values: z.infer<typeof sendNotificationSchema>) => {
        const body = new MessageRequestData();
        body.userName = values.name ?? '';
        body.message = values.message ?? '';
        return createMessageRequest.mutate(body);
    }, []);

    return (
        <Card className='max-w-2xl mx-auto'>
            <CardHeader>
                <CardTitle>Send Notification</CardTitle>
                <CardDescription>
                    Enter the User Name you wish to message and the message you wish to send them.
                </CardDescription>
            </CardHeader>
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)}>
                    <CardContent className='space-y-4'>
                        <FormField
                            control={form.control}
                            name='name'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel required tooltip={ToolTips.AdminSendNotificationUserName}>
                                        Name
                                    </FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Enter Name' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='message'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel required tooltip={ToolTips.AdminSendNotificationMessage}>
                                        Message
                                    </FormLabel>
                                    <FormControl>
                                        <Textarea {...field} placeholder='Enter message' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                    </CardContent>
                    <CardFooter className='flex gap-2 justify-end'>
                        <Button onClick={() => navigate('/')} variant='outline'>
                            Cancel
                        </Button>
                        <Button disabled={createMessageRequest.isLoading} type='submit' variant='default'>
                            {createMessageRequest.isLoading ? <Loader2 className='animate-spin' /> : null}
                            Send
                        </Button>
                    </CardFooter>
                </form>
            </Form>
        </Card>
    );
};
