import { Link, useNavigate, useParams, useSearchParams } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Checkbox } from '@/components/ui/checkbox';
import { Textarea } from '@/components/ui/textarea';
import * as ToolTips from '@/store/ToolTips';

import { CreateLocationService, GetLocationsByPartner } from '@/services/locations';
import PartnerLocationServiceData from '@/components/Models/PartnerLocationServiceData';
import { useGetServiceTypes } from '@/hooks/useGetPartnerServiceTypes';
import { useLogin } from '@/hooks/useLogin';

interface FormInputs {
    serviceTypeId: string;
    notes: string;
    isAutoApproved: boolean;
    isAdvanceNoticeRequired: boolean;
}

const formSchema = z.object({
    serviceTypeId: z.string({ required_error: 'Service Type cannot be empty.' }),
    notes: z.string().optional(),
    isAutoApproved: z.boolean(),
    isAdvanceNoticeRequired: z.boolean(),
});

interface PartnerServiceCreateProps {}

export const PartnerLocationServiceCreate = (props: PartnerServiceCreateProps) => {
    const { currentUser } = useLogin();
    const { partnerId, locationId } = useParams<{ partnerId: string; locationId: string }>() as {
        partnerId: string;
        locationId: string;
    };
    const [searchParams] = useSearchParams();
    const serviceTypeId = searchParams.get('serviceTypeId') as string;
    const { data: serviceTypes } = useGetServiceTypes();

    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const { mutate, isLoading: isSubmitting } = useMutation({
        mutationKey: CreateLocationService().key,
        mutationFn: CreateLocationService().service,
        onSuccess: () => {
            toast({
                variant: 'primary',
                title: 'Location created!',
                description: '',
            });
            queryClient.invalidateQueries({
                queryKey: GetLocationsByPartner({ partnerId }).key,
                refetchType: 'all',
            });
            navigate(`/partnerdashboard/${partnerId}/locations`);
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: { serviceTypeId },
    });

    const onSubmit: SubmitHandler<FormInputs> = (formValues) => {
        const body = new PartnerLocationServiceData();
        body.partnerLocationId = locationId;
        body.serviceTypeId = Number(formValues.serviceTypeId) || 0;
        body.notes = formValues.notes;
        body.isAutoApproved = formValues.isAutoApproved;
        body.isAdvanceNoticeRequired = formValues.isAdvanceNoticeRequired;
        body.createdByUserId = currentUser.id;
        mutate(body);
    };

    return (
        <Form {...form}>
            <form className='grid grid-cols-12 gap-4' onSubmit={form.handleSubmit(onSubmit)}>
                <FormField
                    control={form.control}
                    name='serviceTypeId'
                    render={({ field }) => (
                        <FormItem className='col-span-4'>
                            <FormLabel required tooltip={ToolTips.PartnerContactName}>
                                Service Type
                            </FormLabel>
                            <FormControl>
                                <Select
                                    disabled
                                    onValueChange={(val) => field.onChange(Number(val))}
                                    value={`${field.value}`}
                                >
                                    <SelectTrigger>
                                        <SelectValue placeholder='-- Select Partner Status --' />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {(serviceTypes || []).map((serviceType) => (
                                            <SelectItem key={serviceType.id} value={`${serviceType.id}`}>
                                                {serviceType.name}
                                            </SelectItem>
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
                        <FormItem className='col-span-4'>
                            <FormLabel> </FormLabel>
                            <FormControl>
                                <div className='flex items-center space-x-2 h-9'>
                                    <Checkbox
                                        checked={field.value}
                                        id='isAutoApproved'
                                        onCheckedChange={field.onChange}
                                    />
                                    <label
                                        className='text-sm mb-0 font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70'
                                        htmlFor='isAutoApproved'
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
                        <FormItem className='col-span-4'>
                            <FormLabel> </FormLabel>
                            <FormControl>
                                <div className='flex items-center space-x-2 h-9'>
                                    <Checkbox
                                        checked={field.value}
                                        id='isAutoApproved'
                                        onCheckedChange={field.onChange}
                                    />
                                    <label
                                        className='text-sm mb-0 font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70'
                                        htmlFor='isAutoApproved'
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
                            <FormLabel required tooltip={ToolTips.PartnerLocationPublicNotes}>
                                Notes
                            </FormLabel>
                            <FormControl>
                                <Textarea {...field} className='h-24' maxLength={1000} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <div className='col-span-12 flex justify-end gap-2'>
                    <Button asChild data-test='cancel' variant='secondary'>
                        <Link to={`/partnerdashboard/${partnerId}/locations`}>Cancel</Link>
                    </Button>
                    <Button disabled={isSubmitting} type='submit'>
                        {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                        Enable
                    </Button>
                </div>
            </form>
        </Form>
    );
};
