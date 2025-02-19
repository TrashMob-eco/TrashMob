import { useCallback } from 'react';
import { Link, useNavigate, useParams, useSearchParams } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Loader2 } from 'lucide-react';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { Checkbox } from '@/components/ui/checkbox';
import { Textarea } from '@/components/ui/textarea';
import * as ToolTips from '@/store/ToolTips';

import { CreateLocationService, GetPartnerLocationsServicesByLocationId } from '@/services/locations';
import PartnerLocationServiceData from '@/components/Models/PartnerLocationServiceData';
import { useLogin } from '@/hooks/useLogin';
import { useGetPartnerServiceTypes } from '@/hooks/useGetPartnerServiceTypes';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useGetPartnerLocations } from '@/hooks/useGetPartnerLocations';

interface FormInputs {
    serviceTypeId: number;
    partnerLocationId: string;
    notes: string;
    isAutoApproved: boolean;
    isAdvanceNoticeRequired: boolean;
}

const formSchema = z.object({
    serviceTypeId: z.number({ required_error: 'Service Type cannot be empty.' }),
    partnerLocationId: z.string({ required_error: 'Location cannot be empty.' }),
    notes: z.string().optional(),
    isAutoApproved: z.boolean(),
    isAdvanceNoticeRequired: z.boolean(),
});

export const PartnerServiceEnable = () => {
    const { currentUser } = useLogin();
    const { partnerId } = useParams<{ partnerId: string }>() as {
        partnerId: string;
    };
    const [searchParams] = useSearchParams();
    const locationId = searchParams.get('locationId') as string;
    const serviceTypeId = Number(searchParams.get('serviceTypeId')) as number;
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const { data: serviceTypes } = useGetPartnerServiceTypes();
    const { data: locations } = useGetPartnerLocations({ partnerId });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            serviceTypeId,
            partnerLocationId: locationId,
            isAutoApproved: false,
            isAdvanceNoticeRequired: false,
        },
    });

    const { mutate, isLoading: isSubmitting } = useMutation({
        mutationKey: CreateLocationService().key,
        mutationFn: CreateLocationService().service,
        onSuccess: () => {
            toast({
                variant: 'primary',
                title: 'Service enabled!',
                description: '',
            });
            queryClient.invalidateQueries({
                queryKey: GetPartnerLocationsServicesByLocationId({ locationId }).key,
                refetchType: 'all',
            });
            navigate(`/partnerdashboard/${partnerId}/services`);
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            const body = new PartnerLocationServiceData();
            body.partnerLocationId = formValues.partnerLocationId;
            body.partnerId = partnerId;
            body.serviceTypeId = formValues.serviceTypeId;
            body.notes = formValues.notes;
            body.isAutoApproved = formValues.isAutoApproved;
            body.isAdvanceNoticeRequired = formValues.isAdvanceNoticeRequired;
            body.createdByUserId = currentUser.id;
            body.lastUpdatedByUserId = currentUser.id;
            mutate(body);
        },
        [partnerId, currentUser.id],
    );

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                <FormField
                    control={form.control}
                    name='partnerLocationId'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel tooltip={ToolTips.PartnerContactName} required>
                                Location
                            </FormLabel>
                            <FormControl>
                                <Select value={`${field.value}`} onValueChange={field.onChange} disabled>
                                    <SelectTrigger className='w-full'>
                                        <SelectValue placeholder='Location' />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {(locations || []).map((loc) => (
                                            <SelectItem value={`${loc.id}`}>{loc.name}</SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='serviceTypeId'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel tooltip={ToolTips.PartnerContactName} required>
                                Service Type
                            </FormLabel>
                            <FormControl>
                                <Select value={`${field.value}`} onValueChange={field.onChange} disabled>
                                    <SelectTrigger className='w-full'>
                                        <SelectValue placeholder='Service Type' />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {(serviceTypes || []).map((st) => (
                                            <SelectItem value={`${st.id}`}>{st.name}</SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='isAutoApproved'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormControl>
                                <div className='flex items-center space-x-2 h-9'>
                                    <Checkbox
                                        id='isAutoApproved'
                                        checked={field.value}
                                        onCheckedChange={field.onChange}
                                    />
                                    <label
                                        htmlFor='isAutoApproved'
                                        className='text-sm mb-0 font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70'
                                    >
                                        Auto Approved
                                    </label>
                                </div>
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='isAdvanceNoticeRequired'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormControl>
                                <div className='flex items-center space-x-2 h-9'>
                                    <Checkbox
                                        id='isAutoApproved'
                                        checked={field.value}
                                        onCheckedChange={field.onChange}
                                    />
                                    <label
                                        htmlFor='isAutoApproved'
                                        className='text-sm mb-0 font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70'
                                    >
                                        Advance Notice Required
                                    </label>
                                </div>
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
                            <FormLabel tooltip={ToolTips.PartnerLocationPublicNotes} required>
                                Notes
                            </FormLabel>
                            <FormControl>
                                <Textarea {...field} maxLength={1000} className='h-24' />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <div className='col-span-12 flex justify-end gap-2'>
                    <Button variant='secondary' data-test='cancel' asChild>
                        <Link to={`/partnerdashboard/${partnerId}/services`}>Cancel</Link>
                    </Button>
                    <Button type='submit' disabled={isSubmitting}>
                        {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                        Enable Service
                    </Button>
                </div>
            </form>
        </Form>
    );
};
