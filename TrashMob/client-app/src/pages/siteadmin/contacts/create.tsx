import { useCallback, useState } from 'react';
import { Link, useNavigate } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2, AlertTriangle } from 'lucide-react';
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
import { CreateContact, GetContacts } from '@/services/contacts';
import { ApiService } from '@/services';

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

export const SiteAdminContactCreate = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const [duplicateWarning, setDuplicateWarning] = useState<string | null>(null);

    const createContact = useMutation({
        mutationKey: CreateContact().key,
        mutationFn: CreateContact().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Contact created!' });
            queryClient.invalidateQueries({ queryKey: ['/contacts'], refetchType: 'all' });
            navigate('/siteadmin/contacts');
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

    const checkDuplicate = useCallback(async (email: string) => {
        if (!email) {
            setDuplicateWarning(null);
            return;
        }
        try {
            const res = await ApiService('protected').fetchData<ContactData[]>({
                url: `/contacts?search=${encodeURIComponent(email)}`,
                method: 'get',
            });
            const matches = res.data?.filter((c) => c.email?.toLowerCase() === email.toLowerCase()) || [];
            if (matches.length > 0) {
                const names = matches.map((c) => `${c.firstName} ${c.lastName}`).join(', ');
                setDuplicateWarning(`Possible duplicate: ${names}`);
            } else {
                setDuplicateWarning(null);
            }
        } catch {
            setDuplicateWarning(null);
        }
    }, []);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (values) => {
            const body = new ContactData();
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
            createContact.mutate(body);
        },
        [createContact],
    );

    const isSubmitting = createContact.isPending;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Add Contact</CardTitle>
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
                                    <Select onValueChange={field.onChange} defaultValue={field.value}>
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
                                        <Input
                                            {...field}
                                            type='email'
                                            placeholder='email@example.com'
                                            onBlur={(e) => {
                                                field.onBlur();
                                                checkDuplicate(e.target.value);
                                            }}
                                        />
                                    </FormControl>
                                    <FormMessage />
                                    {duplicateWarning ? (
                                        <p className='flex items-center gap-1 text-sm text-amber-600'>
                                            <AlertTriangle className='h-4 w-4' /> {duplicateWarning}
                                        </p>
                                    ) : null}
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
                                <Link to='/siteadmin/contacts'>Cancel</Link>
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
