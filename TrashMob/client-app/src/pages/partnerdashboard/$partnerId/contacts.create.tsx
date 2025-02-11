import { useCallback, useEffect } from 'react';
import { Link, useNavigate, useParams, useSearchParams } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import PhoneInput from 'react-phone-input-2';
import { Label } from "@/components/ui/label"
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group"
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import * as ToolTips from '@/store/ToolTips';
import {
    CreatePartnerContact,
    CreatePartnerLocationContact,
    GetPartnerContactsByPartnerId,
    GetPartnerLocationContactsByLocationId,
} from '@/services/contact';
import PartnerContactData from '@/components/Models/PartnerContactData';
import * as Constants from '@/components/Models/Constants';
import { useLogin } from '@/hooks/useLogin';
import { Card, CardContent } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { useGetPartnerLocations } from '@/hooks/useGetPartnerLocations';
import PartnerLocationContactData from '@/components/Models/PartnerLocationContactData';
import { PartnerContactType } from '@/enums/PartnerContactType';

function isPartnerLocationContact(contact?: PartnerContactData | PartnerLocationContactData): contact is PartnerLocationContactData {
    if (!contact) return false
    return "partnerLocationId" in contact && typeof contact.partnerLocationId === "string"
}

interface FormInputs {
    contactType: PartnerContactType
    locationId: string;
    name: string;
    email: string;
    phone: string;
    notes: string;
    lastUpdatedDate: string;
}

const formSchema = z.object({
    contactType: z.string(),
    locationId: z.string(),
    name: z.string({ required_error: 'Name cannot be blank.' }),
    email: z.string().email(),
    phone: z.string().regex(Constants.RegexPhoneNumber, { message: 'Please enter a valid phone number.' }),
    notes: z
        .string({ required_error: 'Notes cannot be empty.' })
        .min(1, 'Notes cannot be empty.')
        .max(1000, 'Notes cannot be more than 1000 characters long'),
});

export const PartnerContactCreate = () => {
    const { partnerId, contactId } = useParams<{ partnerId: string; contactId: string }>() as {
        partnerId: string;
        contactId: string;
    };
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const [searchParams] = useSearchParams();
    const defaultContactType = searchParams.get('contactType') as PartnerContactType || PartnerContactType.ORGANIZATION_WIDE

    const { currentUser } = useLogin();
    const { toast } = useToast();

    const { data: locations } = useGetPartnerLocations({ partnerId });

    const onCreateSuccess = (_data: unknown, variables: PartnerContactData | PartnerLocationContactData) => {
        const locationId = isPartnerLocationContact(variables) ? variables.partnerLocationId : ''
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
                queryKey: GetPartnerLocationContactsByLocationId({ locationId }).key,
                refetchType: 'all',
            });
        }
        navigate(`/partnerdashboard/${partnerId}/contacts`);
    }

    const createPartnerContact = useMutation({
        mutationKey: CreatePartnerContact().key,
        mutationFn: CreatePartnerContact().service,
        onSuccess: onCreateSuccess,
    });

    const createPartnerLocationContact = useMutation({
        mutationKey: CreatePartnerLocationContact().key,
        mutationFn: CreatePartnerLocationContact().service,
        onSuccess: onCreateSuccess,
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            contactType: defaultContactType,
            locationId: ''
        }
    });

    const contactType = form.watch('contactType')

    useEffect(() => {
        if (contactType === PartnerContactType.ORGANIZATION_WIDE) {
            form.setValue('locationId', '')
        } else if (contactType === PartnerContactType.LOCATION_SPECIFIC) {
            if (locations && locations.length > 0) {
                form.setValue('locationId', locations[0].id)
            }
        }
    }, [contactType, locations])

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (formValues.contactType === PartnerContactType.ORGANIZATION_WIDE) {
                const body = new PartnerContactData();
                body.id = contactId;
                body.partnerId = partnerId;
                body.name = formValues.name ?? '';
                body.email = formValues.email ?? '';
                body.phone = formValues.phone ?? '';
                body.notes = formValues.notes ?? '';
                body.createdByUserId = currentUser.id;
                createPartnerContact.mutate(body);
            } else if (formValues.contactType === PartnerContactType.LOCATION_SPECIFIC) {
                const body = new PartnerLocationContactData();
                body.partnerLocationId = formValues.locationId;
                body.name = formValues.name ?? '';
                body.email = formValues.email ?? '';
                body.phone = formValues.phone ?? '';
                body.notes = formValues.notes ?? '';
                body.createdByUserId = currentUser.id;
                createPartnerLocationContact.mutate(body);
            }
        },
        [currentUser?.id, partnerId],
    );

    const isSubmitting = createPartnerContact.isLoading || createPartnerLocationContact.isLoading

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                <FormField
                    control={form.control}
                    name='contactType'
                    render={({ field }) => (
                        <FormItem className='col-span-12 pt-3'>
                            <Card>
                                <CardContent className="px-4 py-3">
                                    <RadioGroup value={field.value} onValueChange={field.onChange} className='flex'>
                                        <div className="basis-1/2 flex items-start space-x-2">
                                            <RadioGroupItem value={PartnerContactType.ORGANIZATION_WIDE} id={PartnerContactType.ORGANIZATION_WIDE} className="mt-2" />
                                            <Label htmlFor={PartnerContactType.ORGANIZATION_WIDE} className="!mx-4">
                                               <div>Organization-wide Contact</div>
                                               <div className="text-sm text-muted font-normal mt-1">This contact applies to the entire organization.</div>
                                           </Label>
                                        </div>
                                        <Separator orientation="vertical" className="min-h-[64px]" />
                                        <div className="basis-1/2 flex items-start space-x-2">
                                            <RadioGroupItem value={PartnerContactType.LOCATION_SPECIFIC} id={PartnerContactType.LOCATION_SPECIFIC} className="mt-2" />
                                            <Label htmlFor={PartnerContactType.LOCATION_SPECIFIC} className="!mx-4">
                                               <div>Location-specific Contact</div>
                                               <div className="text-sm text-muted font-normal mt-1">This contact is for a specific location.</div>
                                           </Label>
                                        </div>
                                    </RadioGroup>
                                </CardContent>
                            </Card> 
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='locationId'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel>
                                This contact is for
                            </FormLabel>
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
                            <FormLabel tooltip={ToolTips.PartnerContactPhone} required>
                                Phone
                            </FormLabel>
                            <FormControl>
                               <PhoneInput country='us' value={field.value} onChange={field.onChange} inputProps={{ ref: field.ref }} />
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
