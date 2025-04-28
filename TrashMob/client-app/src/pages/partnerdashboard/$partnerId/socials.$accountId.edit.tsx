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
    GetPartnerSocialMediaAccount,
    GetPartnerSocialMediaAccountsByPartnerId,
    GetSocialMediaAccountTypes,
    UpdatePartnerSocialMediaAccount,
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

export const PartnerSocialAcccountEdit = () => {
    const { partnerId, accountId } = useParams<{ partnerId: string; accountId: string }>() as {
        partnerId: string;
        accountId: string;
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

    const { data: currentValues, isLoading } = useQuery({
        queryKey: GetPartnerSocialMediaAccount({ partnerAccountId: accountId }).key,
        queryFn: GetPartnerSocialMediaAccount({ partnerAccountId: accountId }).service,
        select: (res) => res.data,
    });

    const onUpdateSuccess = (_data: unknown, variables: PartnerSocialMediaAccountData) => {
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

    const updatePartnerSocialMediaAccount = useMutation({
        mutationKey: UpdatePartnerSocialMediaAccount().key,
        mutationFn: UpdatePartnerSocialMediaAccount().service,
        onSuccess: onUpdateSuccess,
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            accountIdentifier: currentValues?.accountIdentifier,
            socialMediaAccountTypeId: currentValues?.socialMediaAccountTypeId,
        },
    });

    useEffect(() => {
        if (currentValues) {
            form.reset(currentValues);
        }
    }, [currentValues]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!currentValues) return;

            const body = new PartnerSocialMediaAccountData();
            body.id = accountId;
            body.partnerId = partnerId;
            body.accountIdentifier = formValues.accountIdentifier;
            body.socialMediaAccountTypeId = formValues.socialMediaAccountTypeId;
            body.lastUpdatedByUserId = currentUser.id;
            updatePartnerSocialMediaAccount.mutate(body);
        },
        [currentValues, partnerId],
    );

    const isSubmitting = updatePartnerSocialMediaAccount.isLoading;

    if (isLoading) {
        return <Loader2 className='animate-spin mx-auto my-10' />;
    }

    return (
        <Form {...form}>
            <form className='grid grid-cols-12 gap-4' onSubmit={form.handleSubmit(onSubmit)}>
                <FormField
                    control={form.control}
                    name='socialMediaAccountTypeId'
                    render={({ field }) => (
                        <FormItem className='col-span-4'>
                            <FormLabel required tooltip={ToolTips.SocialMediaAccountType}>
                                Account Type
                            </FormLabel>
                            <FormControl>
                                <Select onValueChange={(val) => field.onChange(Number(val))} value={`${field.value}`}>
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
                            <FormLabel required tooltip={ToolTips.SocialMediaAccountName}>
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
                                    <Checkbox checked={field.value} id='isActive' onCheckedChange={field.onChange} />
                                    <label
                                        className='text-sm mb-0 font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70'
                                        htmlFor='isActive'
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
                    <Button asChild variant='secondary'>
                        <Link to={`/partnerdashboard/${partnerId}/socials`}>Cancel</Link>
                    </Button>
                    <Button disabled={isSubmitting} type='submit'>
                        {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                        Save
                    </Button>
                </div>
            </form>
        </Form>
    );
};
