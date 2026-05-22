import { useEffect } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Checkbox } from '@/components/ui/checkbox';
import { useToast } from '@/hooks/use-toast';
import { getErrorMessage } from '@/lib/api-errors';
import {
    CreateProspectContact,
    GetCommunityProspectById,
    GetProspectContacts,
    UpdateProspectContact,
} from '@/services/community-prospects';
import ProspectContactData from '@/components/Models/ProspectContactData';
import { PROSPECT_CONTACT_STATUSES } from './prospect-contact-status-badge';

interface ProspectContactFormDialogProps {
    prospectId: string;
    open: boolean;
    onOpenChange: (open: boolean) => void;
    /** When provided, the dialog is in edit mode. Omit for create. */
    contact?: ProspectContactData;
    /** Other contacts on this prospect (for the "Referred by" dropdown). */
    referralCandidates: ProspectContactData[];
}

interface FormInputs {
    name: string;
    title: string;
    email: string;
    phone: string;
    role: string;
    contactStatus: string;
    isPrimary: boolean;
    referredByContactId: string;
    notes: string;
}

const NO_REFERRAL_VALUE = '__none__';

const formSchema = z.object({
    name: z.string().min(1, 'Name is required'),
    title: z.string(),
    email: z.string().email('Invalid email').or(z.literal('')),
    phone: z.string(),
    role: z.string(),
    contactStatus: z.string(),
    isPrimary: z.boolean(),
    referredByContactId: z.string(),
    notes: z.string(),
});

export function ProspectContactFormDialog({
    prospectId,
    open,
    onOpenChange,
    contact,
    referralCandidates,
}: ProspectContactFormDialogProps) {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const isEdit = !!contact;

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            title: '',
            email: '',
            phone: '',
            role: '',
            contactStatus: '0',
            isPrimary: false,
            referredByContactId: NO_REFERRAL_VALUE,
            notes: '',
        },
    });

    // When the dialog opens (or the contact prop changes), seed the form.
    useEffect(() => {
        if (!open) {
            return;
        }
        if (contact) {
            form.reset({
                name: contact.name ?? '',
                title: contact.title ?? '',
                email: contact.email ?? '',
                phone: contact.phone ?? '',
                role: contact.role ?? '',
                contactStatus: String(contact.contactStatus ?? 0),
                isPrimary: contact.isPrimary,
                referredByContactId: contact.referredByContactId ?? NO_REFERRAL_VALUE,
                notes: contact.notes ?? '',
            });
        } else {
            form.reset({
                name: '',
                title: '',
                email: '',
                phone: '',
                role: '',
                contactStatus: '0',
                isPrimary: referralCandidates.length === 0,
                referredByContactId: NO_REFERRAL_VALUE,
                notes: '',
            });
        }
    }, [open, contact, referralCandidates.length, form]);

    const invalidateAfterChange = () => {
        queryClient.invalidateQueries({
            queryKey: GetProspectContacts({ prospectId }).key,
            refetchType: 'all',
        });
        queryClient.invalidateQueries({
            queryKey: GetCommunityProspectById({ id: prospectId }).key,
            refetchType: 'all',
        });
    };

    const createContact = useMutation({
        mutationKey: CreateProspectContact({ prospectId }).key,
        mutationFn: CreateProspectContact({ prospectId }).service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Contact added' });
            invalidateAfterChange();
            onOpenChange(false);
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Failed to add contact', description: getErrorMessage(error) });
        },
    });

    const updateContact = useMutation({
        mutationKey: UpdateProspectContact({ prospectId, contactId: contact?.id ?? '' }).key,
        mutationFn: async (body: ProspectContactData) => {
            if (!contact) {
                throw new Error('No contact in edit mode');
            }
            return UpdateProspectContact({ prospectId, contactId: contact.id }).service(body);
        },
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Contact updated' });
            invalidateAfterChange();
            onOpenChange(false);
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Failed to update contact', description: getErrorMessage(error) });
        },
    });

    const isPending = createContact.isPending || updateContact.isPending;

    const onSubmit: SubmitHandler<FormInputs> = (values) => {
        const body = new ProspectContactData();
        body.id = contact?.id ?? '00000000-0000-0000-0000-000000000000';
        body.prospectId = prospectId;
        body.name = values.name;
        body.title = values.title || null;
        body.email = values.email || null;
        body.phone = values.phone || null;
        body.role = values.role || null;
        body.contactStatus = parseInt(values.contactStatus, 10);
        body.isPrimary = values.isPrimary;
        body.referredByContactId = values.referredByContactId === NO_REFERRAL_VALUE ? null : values.referredByContactId;
        body.notes = values.notes || null;

        if (isEdit) {
            updateContact.mutate(body);
        } else {
            createContact.mutate(body);
        }
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className='max-w-xl'>
                <DialogHeader>
                    <DialogTitle>{isEdit ? 'Edit Contact' : 'Add Contact'}</DialogTitle>
                </DialogHeader>
                <Form {...form}>
                    <form id='prospect-contact-form' onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
                        <FormField
                            control={form.control}
                            name='name'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Name *</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Jane Doe' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <div className='grid grid-cols-2 gap-4'>
                            <FormField
                                control={form.control}
                                name='title'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Title</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='Sustainability Coordinator' />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='role'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Role</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='Decision-maker, Gatekeeper, Referral...' />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </div>
                        <div className='grid grid-cols-2 gap-4'>
                            <FormField
                                control={form.control}
                                name='email'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Email</FormLabel>
                                        <FormControl>
                                            <Input {...field} type='email' placeholder='jane@city.gov' />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='phone'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Phone</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='(555) 555-5555' />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </div>
                        <div className='grid grid-cols-2 gap-4'>
                            <FormField
                                control={form.control}
                                name='contactStatus'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Status</FormLabel>
                                        <FormControl>
                                            <Select value={field.value} onValueChange={field.onChange}>
                                                <SelectTrigger>
                                                    <SelectValue />
                                                </SelectTrigger>
                                                <SelectContent>
                                                    {PROSPECT_CONTACT_STATUSES.map((s) => (
                                                        <SelectItem key={s.value} value={String(s.value)}>
                                                            {s.label}
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
                                name='referredByContactId'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Referred by</FormLabel>
                                        <FormControl>
                                            <Select value={field.value} onValueChange={field.onChange}>
                                                <SelectTrigger>
                                                    <SelectValue placeholder='None' />
                                                </SelectTrigger>
                                                <SelectContent>
                                                    <SelectItem value={NO_REFERRAL_VALUE}>None</SelectItem>
                                                    {referralCandidates
                                                        .filter((c) => c.id !== contact?.id)
                                                        .map((c) => (
                                                            <SelectItem key={c.id} value={c.id}>
                                                                {c.name}
                                                            </SelectItem>
                                                        ))}
                                                </SelectContent>
                                            </Select>
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </div>
                        <FormField
                            control={form.control}
                            name='isPrimary'
                            render={({ field }) => (
                                <FormItem className='flex flex-row items-center gap-2 space-y-0'>
                                    <FormControl>
                                        <Checkbox
                                            checked={field.value}
                                            onCheckedChange={(checked) => field.onChange(checked === true)}
                                        />
                                    </FormControl>
                                    <FormLabel className='mb-0'>
                                        Primary contact (demotes any existing primary)
                                    </FormLabel>
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='notes'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Notes</FormLabel>
                                    <FormControl>
                                        <Textarea {...field} rows={3} placeholder='Internal notes about this contact' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                    </form>
                </Form>
                <DialogFooter>
                    <Button variant='outline' onClick={() => onOpenChange(false)} disabled={isPending}>
                        Cancel
                    </Button>
                    <Button type='submit' form='prospect-contact-form' disabled={isPending}>
                        {isEdit ? 'Save' : 'Add Contact'}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}
