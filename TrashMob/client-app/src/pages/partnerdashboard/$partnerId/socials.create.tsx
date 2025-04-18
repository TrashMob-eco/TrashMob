import { useCallback, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { Input } from '@/components/ui/input';
import * as ToolTips from '@/store/ToolTips';
import { useLogin } from '@/hooks/useLogin';
import {
    GetPartnerSocialMediaAccountsByPartnerId,
    GetSocialMediaAccountTypes,
    CreatePartnerSocialMediaAccount,
} from '@/services/social-media';
import PartnerSocialMediaAccountData from '@/components/Models/PartnerSocialMediaAccountData';
import { Checkbox } from '@/components/ui/checkbox';

interface FormInputs {
    accountIdentifier: string;
    socialMediaAccountTypeId: number;
    isActive: boolean;
}

const formSchema = z.object({
    accountIdentifier: z.string({ required_error: 'Identifier cannot be blank.' }),
    socialMediaAccountTypeId: z.number(),
    isActive: z.boolean(),
});

export const PartnerSocialAcccountCreate = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as {
        partnerId: string;
    };
    const { currentUser } = useLogin();

    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const { data: accountTypes } = useQuery({
        queryKey: GetSocialMediaAccountTypes().key,
        queryFn: GetSocialMediaAccountTypes().service,
        select: (res) => res.data,
    });

    const onCreateSuccess = (_data: unknown, variables: PartnerSocialMediaAccountData) => {
        toast({
            variant: 'primary',
            title: 'Account saved!',
            description: '',
        });
        queryClient.invalidateQueries({
            queryKey: GetPartnerSocialMediaAccountsByPartnerId({ partnerId }).key,
            refetchType: 'all',
        });

        navigate(`/partnerdashboard/${partnerId}/socials`);
    };

    const createPartnerSocialMediaAccount = useMutation({
        mutationKey: CreatePartnerSocialMediaAccount().key,
        mutationFn: CreatePartnerSocialMediaAccount().service,
        onSuccess: onCreateSuccess,
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            accountIdentifier: '',
            socialMediaAccountTypeId: 1,
            isActive: true,
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            const body = new PartnerSocialMediaAccountData();
            body.partnerId = partnerId;
            body.accountIdentifier = formValues.accountIdentifier;
            body.socialMediaAccountTypeId = formValues.socialMediaAccountTypeId;
            body.createdByUserId = currentUser.id;
            body.lastUpdatedByUserId = currentUser.id;
            createPartnerSocialMediaAccount.mutate(body);
        },
        [partnerId],
    );

    const isSubmitting = createPartnerSocialMediaAccount.isLoading;

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                <FormField
                    control={form.control}
                    name='socialMediaAccountTypeId'
                    render={({ field }) => (
                        <FormItem className='col-span-4'>
                            <FormLabel tooltip={ToolTips.SocialMediaAccountType} required>
                                Account Type
                            </FormLabel>
                            <FormControl>
                                <Select value={`${field.value}`} onValueChange={(val) => field.onChange(Number(val))}>
                                    <SelectTrigger className='w-full'>
                                        <SelectValue placeholder='Service Type' />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {(accountTypes || []).map((at) => (
                                            <SelectItem key={`${at.id}`} value={`${at.id}`}>
                                                {at.name}
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
                    name='accountIdentifier'
                    render={({ field }) => (
                        <FormItem className='col-span-5'>
                            <FormLabel tooltip={ToolTips.SocialMediaAccountName} required>
                                Account Name
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
                    name='isActive'
                    render={({ field }) => (
                        <FormItem className='col-span-3'>
                            <FormLabel> </FormLabel>
                            <FormControl>
                                <div className='flex items-center space-x-2 h-9'>
                                    <Checkbox id='isActive' checked={field.value} onCheckedChange={field.onChange} />
                                    <label
                                        htmlFor='isActive'
                                        className='text-sm mb-0 font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70'
                                    >
                                        Is Active
                                    </label>
                                </div>
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <div className='col-span-12 flex justify-end gap-2'>
                    <Button variant='secondary' data-test='cancel' asChild>
                        <Link to={`/partnerdashboard/${partnerId}/socials`}>Cancel</Link>
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
