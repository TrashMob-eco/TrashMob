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
import { CONTACT_TYPES } from '@/components/contacts/contact-constants';
import ContactData from '@/components/Models/ContactData';
import { GetContactById, UpdateContact } from '@/services/contacts';

const formSchema = z.object({
    firstName: z.string().min(1, 'First name is required'),
    lastName: z.string().min(1, 'Last name is required'),
    email: z.string().email('Invalid email').or(z.literal('')),
    phone: z.string(),
    organizationName: z.string(),
    title: z.string(),
    address: z.string(),
    city: z.string(),
    region: z.string(),
    postalCode: z.string(),
    country: z.string(),
    contactType: z.string(),
    source: z.string(),
    notes: z.string(),
    isActive: z.boolean(),
});

type FormInputs = z.infer<typeof formSchema>;

export const SiteAdminContactEdit = () => {
    const { contactId } = useParams<{ contactId: string }>() as { contactId: string };
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const { data: contact } = useQuery({
        queryKey: GetContactById({ id: contactId }).key,
        queryFn: GetContactById({ id: contactId }).service,
        select: (res) => res.data,
        enabled: !!contactId,
    });

    const updateContact = useMutation({
        mutationKey: UpdateContact().key,
        mutationFn: UpdateContact().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Contact updated!' });
            queryClient.invalidateQueries({ queryKey: ['/contacts'], refetchType: 'all' });
            navigate(`/siteadmin/contacts/${contactId}`);
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            firstName: '',
            lastName: '',
            email: '',
            phone: '',
            organizationName: '',
            title: '',
            address: '',
            city: '',
            region: '',
            postalCode: '',
            country: '',
            contactType: '1',
            source: '',
            notes: '',
            isActive: true,
        },
    });

    useEffect(() => {
        if (contact) {
            form.reset({
                firstName: contact.firstName || '',
                lastName: contact.lastName || '',
                email: contact.email || '',
                phone: contact.phone || '',
                organizationName: contact.organizationName || '',
                title: contact.title || '',
                address: contact.address || '',
                city: contact.city || '',
                region: contact.region || '',
                postalCode: contact.postalCode || '',
                country: contact.country || '',
                contactType: (contact.contactType || 1).toString(),
                source: contact.source || '',
                notes: contact.notes || '',
                isActive: contact.isActive ?? true,
            });
        }
    }, [contact, form]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (values) => {
            const body = new ContactData();
            body.id = contactId;
            body.firstName = values.firstName;
            body.lastName = values.lastName;
            body.email = values.email;
            body.phone = values.phone;
            body.organizationName = values.organizationName;
            body.title = values.title;
            body.address = values.address;
            body.city = values.city;
            body.region = values.region;
            body.postalCode = values.postalCode;
            body.country = values.country;
            body.contactType = parseInt(values.contactType);
            body.source = values.source;
            body.notes = values.notes;
            body.isActive = values.isActive;
            if (contact) {
                body.createdByUserId = contact.createdByUserId;
                body.createdDate = contact.createdDate;
                body.userId = contact.userId;
                body.partnerId = contact.partnerId;
            }
            updateContact.mutate(body);
        },
        [contactId, contact, updateContact],
    );

    const isSubmitting = updateContact.isPending;

    if (!contact) {
        return (
            <div className='flex justify-center items-center py-16'>
                <Loader2 className='animate-spin mr-2' /> Loading...
            </div>
        );
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle>Edit Contact</CardTitle>
            </CardHeader>
            <CardContent>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                        <FormField
                            control={form.control}
                            name='firstName'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel required>First Name</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='First name' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='lastName'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel required>Last Name</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Last name' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='contactType'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Type</FormLabel>
                                    <Select onValueChange={field.onChange} value={field.value}>
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {CONTACT_TYPES.map((t) => (
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
                            name='email'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-6'>
                                    <FormLabel>Email</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='email' placeholder='email@example.com' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='phone'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-6'>
                                    <FormLabel>Phone</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Phone number' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='organizationName'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-6'>
                                    <FormLabel>Organization</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Organization name' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='title'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-6'>
                                    <FormLabel>Title</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Job title' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='address'
                            render={({ field }) => (
                                <FormItem className='col-span-12'>
                                    <FormLabel>Address</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Street address' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='city'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>City</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='City' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='region'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>State / Region</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='State' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='postalCode'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Postal Code</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Zip / Postal code' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='country'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Country</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Country' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='source'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4'>
                                    <FormLabel>Source</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='How did you find them?' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='isActive'
                            render={({ field }) => (
                                <FormItem className='col-span-12 md:col-span-4 flex items-center gap-2 pt-8'>
                                    <FormControl>
                                        <Switch checked={field.value} onCheckedChange={field.onChange} />
                                    </FormControl>
                                    <FormLabel className='!mt-0'>Active</FormLabel>
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
                                <Link to={`/siteadmin/contacts/${contactId}`}>Cancel</Link>
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
