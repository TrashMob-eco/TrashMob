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
import { Switch } from '@/components/ui/switch';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useToast } from '@/hooks/use-toast';
import { DONATION_TYPES, RECURRING_FREQUENCIES } from '@/components/contacts/contact-constants';
import DonationData from '@/components/Models/DonationData';
import { GetContacts, GetDonationById, GetPledgesByContact, UpdateDonation } from '@/services/contacts';

const formSchema = z.object({
    contactId: z.string().min(1, 'Contact is required'),
    amount: z.coerce.number().positive('Amount must be greater than 0'),
    donationDate: z.string().min(1, 'Donation date is required'),
    donationType: z.string(),
    campaign: z.string(),
    isRecurring: z.boolean(),
    recurringFrequency: z.string(),
    pledgeId: z.string(),
    inKindDescription: z.string(),
    matchingGiftEmployer: z.string(),
    notes: z.string(),
    receiptSent: z.boolean(),
    thankYouSent: z.boolean(),
});

type FormInputs = z.infer<typeof formSchema>;

export const SiteAdminDonationEdit = () => {
    const { donationId } = useParams<{ donationId: string }>() as { donationId: string };
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const { data: donation } = useQuery({
        queryKey: GetDonationById({ id: donationId }).key,
        queryFn: GetDonationById({ id: donationId }).service,
        select: (res) => res.data,
        enabled: !!donationId,
    });

    const updateDonation = useMutation({
        mutationKey: UpdateDonation().key,
        mutationFn: UpdateDonation().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Donation updated!' });
            queryClient.invalidateQueries({ queryKey: ['/donations'], refetchType: 'all' });
            navigate('/siteadmin/donations');
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            contactId: '',
            amount: 0,
            donationDate: '',
            donationType: '1',
            campaign: '',
            isRecurring: false,
            recurringFrequency: '2',
            pledgeId: '',
            inKindDescription: '',
            matchingGiftEmployer: '',
            notes: '',
            receiptSent: false,
            thankYouSent: false,
        },
    });

    useEffect(() => {
        if (donation) {
            form.reset({
                contactId: donation.contactId || '',
                amount: donation.amount,
                donationDate: donation.donationDate ? donation.donationDate.split('T')[0] : '',
                donationType: (donation.donationType || 1).toString(),
                campaign: donation.campaign || '',
                isRecurring: donation.isRecurring,
                recurringFrequency: (donation.recurringFrequency || 2).toString(),
                pledgeId: donation.pledgeId || '',
                inKindDescription: donation.inKindDescription || '',
                matchingGiftEmployer: donation.matchingGiftEmployer || '',
                notes: donation.notes || '',
                receiptSent: donation.receiptSent,
                thankYouSent: donation.thankYouSent,
            });
        }
    }, [donation, form]);

    const { data: contacts } = useQuery({
        queryKey: GetContacts().key,
        queryFn: GetContacts().service,
        select: (res) => res.data,
    });

    const watchContactId = form.watch('contactId');
    const watchDonationType = form.watch('donationType');
    const watchIsRecurring = form.watch('isRecurring');

    const { data: pledges } = useQuery({
        queryKey: GetPledgesByContact({ contactId: watchContactId }).key,
        queryFn: GetPledgesByContact({ contactId: watchContactId }).service,
        select: (res) => res.data,
        enabled: !!watchContactId,
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (values) => {
            const body = new DonationData();
            body.id = donationId;
            body.contactId = values.contactId;
            body.amount = values.amount;
            body.donationDate = values.donationDate;
            body.donationType = parseInt(values.donationType);
            body.campaign = values.campaign;
            body.isRecurring = values.isRecurring;
            body.recurringFrequency = values.isRecurring ? parseInt(values.recurringFrequency) : null;
            body.pledgeId = values.pledgeId || null;
            body.inKindDescription = values.inKindDescription;
            body.matchingGiftEmployer = values.matchingGiftEmployer;
            body.notes = values.notes;
            body.receiptSent = values.receiptSent;
            body.thankYouSent = values.thankYouSent;
            if (donation) {
                body.createdByUserId = donation.createdByUserId;
                body.createdDate = donation.createdDate;
            }
            updateDonation.mutate(body);
        },
        [donationId, donation, updateDonation],
    );

    const isSubmitting = updateDonation.isPending;

    if (!donation) {
        return (
            <div className='flex justify-center items-center py-16'>
                <Loader2 className='animate-spin mr-2' /> Loading...
            </div>
        );
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle>Edit Donation</CardTitle>
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
                            name='donationType'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-6'>
                                    <FormLabel>Type</FormLabel>
                                    <Select onValueChange={field.onChange} value={field.value}>
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {DONATION_TYPES.map((t) => (
                                                <SelectItem key={t.value} value={t.value.toString()}>
                                                    {t.label}
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
                            name='amount'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel required>Amount ($)</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='number' step='0.01' min='0' placeholder='0.00' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='donationDate'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel required>Date</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='date' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='campaign'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Campaign</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Campaign name' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        {watchDonationType === '4' ? (
                            <FormField
                                control={form.control}
                                name='inKindDescription'
                                render={({ field }) => (
                                    <FormItem className='col-span-12'>
                                        <FormLabel>In-Kind Description</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='Description of in-kind donation' />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        ) : null}
                        {watchDonationType === '5' ? (
                            <FormField
                                control={form.control}
                                name='matchingGiftEmployer'
                                render={({ field }) => (
                                    <FormItem className='col-span-12'>
                                        <FormLabel>Matching Gift Employer</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='Employer name' />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        ) : null}
                        <FormField
                            control={form.control}
                            name='isRecurring'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4 flex items-center gap-2 pt-8'>
                                    <FormControl>
                                        <Switch checked={field.value} onCheckedChange={field.onChange} />
                                    </FormControl>
                                    <FormLabel className='!mt-0'>Recurring</FormLabel>
                                </FormItem>
                            )}
                        />
                        {watchIsRecurring ? (
                            <FormField
                                control={form.control}
                                name='recurringFrequency'
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
                                                {RECURRING_FREQUENCIES.filter((f) => f.value !== 1).map((f) => (
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
                        ) : null}
                        {watchContactId && (pledges || []).length > 0 ? (
                            <FormField
                                control={form.control}
                                name='pledgeId'
                                render={({ field }) => (
                                    <FormItem className='col-span-12 md:col-span-4'>
                                        <FormLabel>Pledge</FormLabel>
                                        <Select onValueChange={field.onChange} value={field.value}>
                                            <FormControl>
                                                <SelectTrigger>
                                                    <SelectValue placeholder='None' />
                                                </SelectTrigger>
                                            </FormControl>
                                            <SelectContent>
                                                {(pledges || []).map((p) => (
                                                    <SelectItem key={p.id} value={p.id}>
                                                        ${p.totalAmount.toLocaleString()} (
                                                        {p.startDate
                                                            ? new Date(p.startDate).toLocaleDateString()
                                                            : '—'}
                                                        )
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        ) : null}
                        <FormField
                            control={form.control}
                            name='receiptSent'
                            render={({ field }) => (
                                <FormItem className='col-span-6 md:col-span-3 flex items-center gap-2 pt-8'>
                                    <FormControl>
                                        <Switch checked={field.value} onCheckedChange={field.onChange} />
                                    </FormControl>
                                    <FormLabel className='!mt-0'>Receipt Sent</FormLabel>
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='thankYouSent'
                            render={({ field }) => (
                                <FormItem className='col-span-6 md:col-span-3 flex items-center gap-2 pt-8'>
                                    <FormControl>
                                        <Switch checked={field.value} onCheckedChange={field.onChange} />
                                    </FormControl>
                                    <FormLabel className='!mt-0'>Thank You Sent</FormLabel>
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
                                <Link to='/siteadmin/donations'>Cancel</Link>
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
