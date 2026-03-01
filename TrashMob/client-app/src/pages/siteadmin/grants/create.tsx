import { useCallback } from 'react';
import { Link, useNavigate } from 'react-router';
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
import { GRANT_STATUSES } from '@/components/contacts/contact-constants';
import GrantData from '@/components/Models/GrantData';
import { CreateGrant } from '@/services/grants';
import { GetContacts } from '@/services/contacts';

const formSchema = z.object({
    funderName: z.string().min(1, 'Funder name is required'),
    programName: z.string(),
    description: z.string(),
    status: z.string(),
    amountMin: z.coerce.number().min(0).nullable(),
    amountMax: z.coerce.number().min(0).nullable(),
    amountAwarded: z.coerce.number().min(0).nullable(),
    submissionDeadline: z.string(),
    awardDate: z.string(),
    reportingDeadline: z.string(),
    renewalDate: z.string(),
    funderContactId: z.string(),
    grantUrl: z.string(),
    notes: z.string(),
});

type FormInputs = z.infer<typeof formSchema>;

export const SiteAdminGrantCreate = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const createGrant = useMutation({
        mutationKey: CreateGrant().key,
        mutationFn: CreateGrant().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Grant created!' });
            queryClient.invalidateQueries({ queryKey: ['/grants'], refetchType: 'all' });
            navigate('/siteadmin/grants');
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            funderName: '',
            programName: '',
            description: '',
            status: '1',
            amountMin: null,
            amountMax: null,
            amountAwarded: null,
            submissionDeadline: '',
            awardDate: '',
            reportingDeadline: '',
            renewalDate: '',
            funderContactId: '',
            grantUrl: '',
            notes: '',
        },
    });

    const { data: contacts } = useQuery({
        queryKey: GetContacts().key,
        queryFn: GetContacts().service,
        select: (res) => res.data,
    });

    const watchStatus = form.watch('status');
    const showAwardedFields = ['4', '6', '7'].includes(watchStatus);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (values) => {
            const body = new GrantData();
            body.funderName = values.funderName;
            body.programName = values.programName;
            body.description = values.description;
            body.status = parseInt(values.status);
            body.amountMin = values.amountMin;
            body.amountMax = values.amountMax;
            body.amountAwarded = showAwardedFields ? values.amountAwarded : null;
            body.submissionDeadline = values.submissionDeadline || null;
            body.awardDate = showAwardedFields && values.awardDate ? values.awardDate : null;
            body.reportingDeadline = showAwardedFields && values.reportingDeadline ? values.reportingDeadline : null;
            body.renewalDate = showAwardedFields && values.renewalDate ? values.renewalDate : null;
            body.funderContactId = values.funderContactId || null;
            body.grantUrl = values.grantUrl;
            body.notes = values.notes;
            createGrant.mutate(body);
        },
        [createGrant, showAwardedFields],
    );

    const isSubmitting = createGrant.isPending;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Add Grant</CardTitle>
            </CardHeader>
            <CardContent>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                        <FormField
                            control={form.control}
                            name='funderName'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-6'>
                                    <FormLabel required>Funder Name</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Foundation or agency name' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='programName'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-6'>
                                    <FormLabel>Program Name</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Specific grant program' />
                                    </FormControl>
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
                                            {GRANT_STATUSES.map((s) => (
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
                            name='submissionDeadline'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Submission Deadline</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='date' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='funderContactId'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Funder Contact</FormLabel>
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
                            name='amountMin'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Amount Min ($)</FormLabel>
                                    <FormControl>
                                        <Input
                                            {...field}
                                            type='number'
                                            step='0.01'
                                            min='0'
                                            placeholder='0.00'
                                            value={field.value ?? ''}
                                            onChange={(e) =>
                                                field.onChange(e.target.value === '' ? null : e.target.value)
                                            }
                                        />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='amountMax'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Amount Max ($)</FormLabel>
                                    <FormControl>
                                        <Input
                                            {...field}
                                            type='number'
                                            step='0.01'
                                            min='0'
                                            placeholder='0.00'
                                            value={field.value ?? ''}
                                            onChange={(e) =>
                                                field.onChange(e.target.value === '' ? null : e.target.value)
                                            }
                                        />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        {showAwardedFields ? (
                            <>
                                <FormField
                                    control={form.control}
                                    name='amountAwarded'
                                    render={({ field }) => (
                                        <FormItem className='col-span-12 md:col-span-4'>
                                            <FormLabel>Amount Awarded ($)</FormLabel>
                                            <FormControl>
                                                <Input
                                                    {...field}
                                                    type='number'
                                                    step='0.01'
                                                    min='0'
                                                    placeholder='0.00'
                                                    value={field.value ?? ''}
                                                    onChange={(e) =>
                                                        field.onChange(e.target.value === '' ? null : e.target.value)
                                                    }
                                                />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='awardDate'
                                    render={({ field }) => (
                                        <FormItem className='col-span-12 md:col-span-4'>
                                            <FormLabel>Award Date</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='date' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='reportingDeadline'
                                    render={({ field }) => (
                                        <FormItem className='col-span-12 md:col-span-4'>
                                            <FormLabel>Reporting Deadline</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='date' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='renewalDate'
                                    render={({ field }) => (
                                        <FormItem className='col-span-12 md:col-span-4'>
                                            <FormLabel>Renewal Date</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='date' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </>
                        ) : null}
                        <FormField
                            control={form.control}
                            name='grantUrl'
                            render={({ field }) => (
                                <FormItem className='col-span-12'>
                                    <FormLabel>Grant URL</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='url' placeholder='https://...' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='description'
                            render={({ field }) => (
                                <FormItem className='col-span-12'>
                                    <FormLabel>Description</FormLabel>
                                    <FormControl>
                                        <Textarea {...field} placeholder='Grant purpose and requirements...' rows={3} />
                                    </FormControl>
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
                                <Link to='/siteadmin/grants'>Cancel</Link>
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
