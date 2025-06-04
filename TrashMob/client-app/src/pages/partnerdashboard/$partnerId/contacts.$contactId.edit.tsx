import { useCallback, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient, UseQueryOptions } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import PhoneInput from 'react-phone-input-2';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import * as ToolTips from '@/store/ToolTips';
import {
    GetPartnerContactsByContactId,
    GetPartnerContactsByPartnerId,
    GetPartnerLocationContactByContactId,
    GetPartnerLocationContactsByLocationId,
    UpdatePartnerContact,
    UpdatePartnerLocationContact,
} from '@/services/contact';
import PartnerContactData from '@/components/Models/PartnerContactData';
import { useLogin } from '@/hooks/useLogin';
import PartnerLocationContactData from '@/components/Models/PartnerLocationContactData';
import { PartnerContactType } from '@/enums/PartnerContactType';
import { useGetPartnerLocations } from '@/hooks/useGetPartnerLocations';
import { AxiosResponse } from 'axios';

interface FormInputs {
    partnerLocationId: string;
    name: string;
    email: string;
    phone: string;
    notes: string;
    lastUpdatedDate: string;
}

const formSchema = z.object({
    partnerLocationId: z.string().optional(),
    name: z.string({ required_error: 'Name cannot be blank.' }),
    email: z.string().email(),
    phone: z.string().optional(),
    notes: z
        .string({ required_error: 'Notes cannot be empty.' })
        .min(1, 'Notes cannot be empty.')
        .max(1000, 'Notes cannot be more than 1000 characters long'),
});

const useGetPartnerContactById = (type: PartnerContactType, contactId: string) => {
    const queryOptions =
        type === PartnerContactType.LOCATION_SPECIFIC
            ? ({
                  queryKey: GetPartnerLocationContactByContactId({ contactId }).key,
                  queryFn: GetPartnerLocationContactByContactId({ contactId }).service,
                  select: (res: AxiosResponse<PartnerLocationContactData>) => res.data,
              } as UseQueryOptions)
            : ({
                  queryKey: GetPartnerContactsByContactId({ contactId }).key,
                  queryFn: GetPartnerContactsByContactId({ contactId }).service,
                  select: (res: AxiosResponse<PartnerContactData>) => res.data,
              } as UseQueryOptions);

    return useQuery(queryOptions);
};

interface PartnerContactEditProps {
    type: PartnerContactType;
}

function isPartnerLocationContact(
    contact?: PartnerContactData | PartnerLocationContactData,
): contact is PartnerLocationContactData {
    if (!contact) return false;
    return 'partnerLocationId' in contact && typeof contact.partnerLocationId === 'string';
}

export const PartnerContactEdit = (props: PartnerContactEditProps) => {
    const { type: contactType } = props;
    const { partnerId, contactId } = useParams<{ partnerId: string; contactId: string }>() as {
        partnerId: string;
        contactId: string;
    };
    const { currentUser } = useLogin();
    const { data: locations } = useGetPartnerLocations({ partnerId });

    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const { data: currentValues, isLoading } = useGetPartnerContactById(contactType, contactId);
    const locationId = isPartnerLocationContact(currentValues) ? currentValues?.partnerLocationId : '';

    const onUpdateSuccess = (_data: unknown, variables: PartnerContactData | PartnerLocationContactData) => {
        const locationId = isPartnerLocationContact(variables) ? variables.partnerLocationId : '';
        toast({
            variant: 'primary',
            title: 'Contact saved!',
            description: '',
        });
        queryClient.invalidateQueries({
            queryKey: GetPartnerContactsByPartnerId({ partnerId }).key,
            refetchType: 'all',
        });
        if (locationId) {
            queryClient.invalidateQueries({
                queryKey: GetPartnerLocationContactsByLocationId({
                    locationId,
                }).key,
                refetchType: 'all',
            });
        }
        navigate(`/partnerdashboard/${partnerId}/contacts`);
    };

    const updatePartnerContact = useMutation({
        mutationKey: UpdatePartnerContact().key,
        mutationFn: UpdatePartnerContact().service,
        onSuccess: onUpdateSuccess,
    });

    const updatePartnerLocationContact = useMutation({
        mutationKey: UpdatePartnerLocationContact().key,
        mutationFn: UpdatePartnerLocationContact().service,
        onSuccess: onUpdateSuccess,
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: currentValues?.name,
            phone: currentValues?.phone,
            email: currentValues?.email,
            partnerLocationId: locationId,
        },
    });

    useEffect(() => {
        if (currentValues) {
            form.reset({
                ...currentValues,
                lastUpdatedDate: `${currentValues?.lastUpdatedDate}`,
            });
        }
    }, [currentValues]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!currentValues) return;

            if (contactType === PartnerContactType.ORGANIZATION_WIDE) {
                const body = new PartnerContactData();
                body.id = contactId;
                body.partnerId = partnerId;
                body.name = formValues.name ?? '';
                body.email = formValues.email ?? '';
                body.phone = formValues.phone ?? '';
                body.notes = formValues.notes ?? '';
                body.lastUpdatedByUserId = currentUser.id;
                updatePartnerContact.mutate(body);
            } else if (contactType === PartnerContactType.LOCATION_SPECIFIC) {
                const body = new PartnerLocationContactData();
                body.id = contactId;
                body.partnerLocationId = formValues.partnerLocationId;
                body.name = formValues.name ?? '';
                body.email = formValues.email ?? '';
                body.phone = formValues.phone ?? '';
                body.notes = formValues.notes ?? '';
                body.lastUpdatedByUserId = currentUser.id;
                updatePartnerLocationContact.mutate(body);
            }
        },
        [currentValues, contactType, partnerId],
    );

    const isSubmitting = updatePartnerContact.isLoading || updatePartnerLocationContact.isLoading;

    if (isLoading) {
        return <Loader2 className='animate-spin mx-auto my-10' />;
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                <FormField
                    control={form.control}
                    name='partnerLocationId'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel>This contact is for</FormLabel>
                            <FormControl>
                                <Select
                                    value={field.value}
                                    onValueChange={field.onChange}
                                    disabled={contactType === PartnerContactType.ORGANIZATION_WIDE}
                                >
                                    <SelectTrigger className='w-full'>
                                        <SelectValue placeholder='Organization-wide' />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {(locations || []).map((loc) => (
                                            <SelectItem key={`${loc.id}`} value={`${loc.id}`}>
                                                {loc.name}
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
                    name='name'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel tooltip={ToolTips.PartnerContactName} required>
                                Name
                            </FormLabel>
                            <FormControl>
                                <Input {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='email'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel tooltip={ToolTips.PartnerContactEmail} required>
                                Email
                            </FormLabel>
                            <FormControl>
                                <Input {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='phone'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel tooltip={ToolTips.PartnerContactPhone}>Phone</FormLabel>
                            <FormControl>
                                <PhoneInput
                                    country='us'
                                    value={field.value}
                                    onChange={field.onChange}
                                    inputProps={{ ref: field.ref }}
                                />
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
                            <FormLabel tooltip={ToolTips.PartnerContactNotes} required>
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
                        <Link to={`/partnerdashboard/${partnerId}/contacts`}>Cancel</Link>
                    </Button>
                    <Button type='submit' disabled={isSubmitting}>
                        {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                        Save
                    </Button>
                </div>
            </form>
        </Form>
    );
};
