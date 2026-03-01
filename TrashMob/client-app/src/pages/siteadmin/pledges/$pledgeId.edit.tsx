import { useCallback, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2 } from 'lucide-react';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useToast } from '@/hooks/use-toast';
import { PLEDGE_STATUSES, RECURRING_FREQUENCIES } from '@/components/contacts/contact-constants';
import PledgeData from '@/components/Models/PledgeData';
import { GetContacts, GetPledgeById, UpdatePledge } from '@/services/contacts';

const formSchema = z.object({
    contactId: z.string().min(1, 'Contact is required'),
    totalAmount: z.coerce.number().positive('Amount must be greater than 0'),
    startDate: z.string().min(1, 'Start date is required'),
    endDate: z.string(),
    frequency: z.string(),
    status: z.string(),
    notes: z.string(),
});

type FormInputs = z.infer<typeof formSchema>;

export const SiteAdminPledgeEdit = () => {
    const { pledgeId } = useParams<{ pledgeId: string }>() as { pledgeId: string };
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const { data: pledge } = useQuery({
        queryKey: GetPledgeById({ id: pledgeId }).key,
        queryFn: GetPledgeById({ id: pledgeId }).service,
        select: (res) => res.data,
        enabled: !!pledgeId,
    });

    const updatePledge = useMutation({
        mutationKey: UpdatePledge().key,
        mutationFn: UpdatePledge().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Pledge updated!' });
            queryClient.invalidateQueries({ queryKey: ['/pledges'], refetchType: 'all' });
            navigate('/siteadmin/pledges');
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            contactId: '',
            totalAmount: 0,
            startDate: '',
            endDate: '',
            frequency: '1',
            status: '1',
            notes: '',
        },
    });

    useEffect(() => {
        if (pledge) {
            form.reset({
                contactId: pledge.contactId || '',
                totalAmount: pledge.totalAmount,
                startDate: pledge.startDate ? pledge.startDate.split('T')[0] : '',
                endDate: pledge.endDate ? pledge.endDate.split('T')[0] : '',
                frequency: (pledge.frequency || 1).toString(),
                status: (pledge.status || 1).toString(),
                notes: pledge.notes || '',
            });
        }
    }, [pledge, form]);

    const { data: contacts } = useQuery({
        queryKey: GetContacts().key,
        queryFn: GetContacts().service,
        select: (res) => res.data,
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (values) => {
            const body = new PledgeData();
            body.id = pledgeId;
            body.contactId = values.contactId;
            body.totalAmount = values.totalAmount;
            body.startDate = values.startDate;
            body.endDate = values.endDate || null;
            body.frequency = parseInt(values.frequency);
            body.status = parseInt(values.status);
            body.notes = values.notes;
            if (pledge) {
                body.createdByUserId = pledge.createdByUserId;
                body.createdDate = pledge.createdDate;
            }
            updatePledge.mutate(body);
        },
        [pledgeId, pledge, updatePledge],
    );

    const isSubmitting = updatePledge.isPending;

    if (!pledge) {
        return (
            <div className='flex justify-center items-center py-16'>
                <Loader2 className='animate-spin mr-2' /> Loading...
            </div>
        );
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle>Edit Pledge</CardTitle>
            </CardHeader>
            <CardContent>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                        <FormField
                            control={form.control}
                            name='contactId'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-6'>
                                    <FormLabel required>Contact</FormLabel>
                                    <Select onValueChange={field.onChange} value={field.value}>
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue placeholder='Select a contact' />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {(contacts || []).map((c) => (
                                                <SelectItem key={c.id} value={c.id}>
                                                    {[c.firstName, c.lastName].filter(Boolean).join(' ')}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='totalAmount'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-6'>
                                    <FormLabel required>Total Amount ($)</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='number' step='0.01' min='0' placeholder='0.00' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='startDate'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel required>Start Date</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='date' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='endDate'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>End Date</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='date' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='frequency'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Frequency</FormLabel>
                                    <Select onValueChange={field.onChange} value={field.value}>
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {RECURRING_FREQUENCIES.map((f) => (
                                                <SelectItem key={f.value} value={f.value.toString()}>
                                                    {f.label}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='status'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Status</FormLabel>
                                    <Select onValueChange={field.onChange} value={field.value}>
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {PLEDGE_STATUSES.map((s) => (
                                                <SelectItem key={s.value} value={s.value.toString()}>
                                                    {s.label}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='notes'
                            render={({ field }) => (
                                <FormItem className='col-span-12'>
                                    <FormLabel>Notes</FormLabel>
                                    <FormControl>
                                        <Textarea {...field} placeholder='Additional notes...' rows={3} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <div className='col-span-12 flex justify-end gap-2'>
                            <Button variant='secondary' asChild>
                                <Link to='/siteadmin/pledges'>Cancel</Link>
                            </Button>
                            <Button type='submit' disabled={isSubmitting}>
                                {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                                Save
                            </Button>
                        </div>
                    </form>
                </Form>
            </CardContent>
        </Card>
    );
};
