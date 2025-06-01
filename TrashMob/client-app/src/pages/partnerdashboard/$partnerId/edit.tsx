import { useCallback, useEffect } from 'react';
import moment from 'moment';
import { useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useToast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import * as ToolTips from '@/store/ToolTips';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { GetPartnerById, GetPartnerStatuses, GetPartnerTypes, UpdatePartner } from '@/services/partners';
import PartnerData from '@/components/Models/PartnerData';
import { SidebarLayout } from '../../layouts/_layout.sidebar';

interface FormInputs {
    name: string;
    website: string;
    partnerStatusId: number;
    partnerTypeId: number;
    publicNotes: string;
    privateNotes: string;
    createdDate: string;
    lastUpdatedDate: string;
}

const formSchema = z.object({
    name: z.string({ required_error: 'Name cannot be blank.' }),
    website: z.string().url({ message: 'Please enter valid website.' }).optional(),
    partnerStatusId: z.number(),
    partnerTypeId: z.number(),
    publicNotes: z.string({ required_error: 'Notes cannot be empty' }),
    privateNotes: z.string().optional(),
});

const useGetPartnerStatuses = () =>
    useQuery({
        queryKey: GetPartnerStatuses().key,
        queryFn: GetPartnerStatuses().service,
        select: (res) => res.data,
    });

const useGetPartnerTypes = () =>
    useQuery({
        queryKey: GetPartnerTypes().key,
        queryFn: GetPartnerTypes().service,
        select: (res) => res.data,
    });

const useGetPartnerById = (partnerId: string) =>
    useQuery({
        queryKey: GetPartnerById({ partnerId }).key,
        queryFn: GetPartnerById({ partnerId }).service,
        select: (res) => res.data,
    });

export const PartnerEdit = () => {
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();

    const { data: currentValues } = useGetPartnerById(partnerId);
    const { data: partnerTypes } = useGetPartnerTypes();
    const { data: partnerStatuses } = useGetPartnerStatuses();

    const { mutate, isLoading: isSubmitting } = useMutation({
        mutationKey: UpdatePartner().key,
        mutationFn: UpdatePartner().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetPartnerById({ partnerId }).key,
                refetchType: 'all',
            });

            toast({
                variant: 'primary',
                title: 'Saved!',
                description: '',
            });
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: currentValues?.name,
            website: currentValues?.website,
            partnerTypeId: currentValues?.partnerTypeId,
            partnerStatusId: currentValues?.partnerStatusId,
            publicNotes: currentValues?.publicNotes,
            privateNotes: currentValues?.privateNotes,
        },
    });

    useEffect(() => {
        if (currentValues) {
            form.reset({
                ...currentValues,
                createdDate: moment(currentValues.createdDate).format('lll'),
                lastUpdatedDate: moment(currentValues.lastUpdatedDate).format('lll'),
            });
        }
    }, [currentValues]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!currentValues) return;

            const body = new PartnerData();
            body.id = partnerId;
            body.name = formValues.name ?? '';
            body.website = formValues.website ?? '';
            body.partnerStatusId = formValues.partnerStatusId ?? 2;
            body.publicNotes = formValues.publicNotes;
            body.privateNotes = formValues.privateNotes;
            body.partnerTypeId = formValues.partnerTypeId;
            body.createdByUserId = currentValues.createdByUserId;
            body.createdDate = currentValues.createdDate;

            mutate(body);
        },
        [currentValues, partnerId],
    );

    return (
        <SidebarLayout
            title='Edit Partner Information'
            description='This page allows you to add basic details about your organization. Public notes may be shown to TrashMob.eco users on the partnership page. Think of this as a blurb or a tag line you may want to add to let users know more about your organization in general.'
        >
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                    <FormField
                        control={form.control}
                        name='name'
                        render={({ field }) => (
                            <FormItem className='col-span-6'>
                                <FormLabel tooltip={ToolTips.PartnerName} required>
                                    Partner Name
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
                        name='website'
                        render={({ field }) => (
                            <FormItem className='col-span-6'>
                                <FormLabel tooltip={ToolTips.PartnerWebsite} required>
                                    Website
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
                        name='partnerStatusId'
                        render={({ field }) => (
                            <FormItem className='col-span-6'>
                                <FormLabel tooltip={ToolTips.PartnerStatus} required>
                                    Partner Status
                                </FormLabel>
                                <FormControl>
                                    <Select
                                        value={`${field.value}`}
                                        onValueChange={(val) => field.onChange(Number(val))}
                                    >
                                        <SelectTrigger>
                                            <SelectValue placeholder='-- Select Partner Status --' />
                                        </SelectTrigger>
                                        <SelectContent>
                                            {partnerStatuses?.map((status) => (
                                                <SelectItem key={status.id} value={`${status.id}`}>
                                                    {status.name}
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
                        name='partnerTypeId'
                        render={({ field }) => (
                            <FormItem className='col-span-6'>
                                <FormLabel tooltip={ToolTips.PartnerType} required>
                                    Partner Type
                                </FormLabel>
                                <FormControl>
                                    <Select
                                        value={`${field.value}`}
                                        onValueChange={(val) => field.onChange(Number(val))}
                                    >
                                        <SelectTrigger>
                                            <SelectValue placeholder='-- Select Partner Type --' />
                                        </SelectTrigger>
                                        <SelectContent>
                                            {partnerTypes?.map((type) => (
                                                <SelectItem key={type.id} value={`${type.id}`}>
                                                    {type.name}
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
                        name='publicNotes'
                        render={({ field }) => (
                            <FormItem className='col-span-12'>
                                <FormLabel tooltip={ToolTips.PartnerPublicNotes} required>
                                    Public Notes
                                </FormLabel>
                                <FormControl>
                                    <Textarea {...field} maxLength={2048} className='h-24' />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                    <FormField
                        control={form.control}
                        name='privateNotes'
                        render={({ field }) => (
                            <FormItem className='col-span-12'>
                                <FormLabel tooltip={ToolTips.PartnerPrivateNotes}>Private Notes</FormLabel>
                                <FormControl>
                                    <Textarea {...field} maxLength={2048} className='h-24' />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                    <FormField
                        control={form.control}
                        name='createdDate'
                        render={({ field }) => (
                            <FormItem className='col-span-6'>
                                <FormLabel tooltip={ToolTips.PartnerCreatedDate}>Created Date</FormLabel>
                                <FormControl>
                                    <Input {...field} disabled />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                    <FormField
                        control={form.control}
                        name='lastUpdatedDate'
                        render={({ field }) => (
                            <FormItem className='col-span-6'>
                                <FormLabel tooltip={ToolTips.PartnerLastUpdatedDate}>Last Update Date</FormLabel>
                                <FormControl>
                                    <Input {...field} disabled />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                    <div className='col-span-12 flex justify-end gap-2'>
                        <Button variant='secondary'>Cancel</Button>
                        <Button type='submit' disabled={isSubmitting}>
                            {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                            Save
                        </Button>
                    </div>
                </form>
            </Form>
        </SidebarLayout>
    );
};
