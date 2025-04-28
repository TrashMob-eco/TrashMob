import { useCallback } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { Input } from '@/components/ui/input';
import * as ToolTips from '@/store/ToolTips';
import { useLogin } from '@/hooks/useLogin';
import PartnerAdminInvitationData from '@/components/Models/PartnerAdminInvitationData';
import { CreatePartnerAdminInvitation, GetPartnerAdminInvitationsByPartnerId } from '@/services/invitations';

interface FormInputs {
    email: string;
}

const formSchema = z.object({
    email: z.string().email(),
});

export const PartnerAdminInvite = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as {
        partnerId: string;
    };
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { currentUser } = useLogin();
    const { toast } = useToast();

    const onInviteSuccess = (_data: unknown, variables: PartnerAdminInvitationData) => {
        toast({
            variant: 'primary',
            title: 'Admin invited!',
            description: '',
        });
        queryClient.invalidateQueries({
            queryKey: GetPartnerAdminInvitationsByPartnerId({ partnerId }).key,
            refetchType: 'all',
        });
        navigate(`/partnerdashboard/${partnerId}/admins`);
    };

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: { email: '' },
    });

    const createPartnerAdminInvitation = useMutation({
        mutationKey: CreatePartnerAdminInvitation().key,
        mutationFn: CreatePartnerAdminInvitation().service,
        onSuccess: onInviteSuccess,
        onError: () => {
            form.setError('email', { message: 'Cannot invite user, possibly user is not yet a trashmob user' });
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            const body = new PartnerAdminInvitationData();
            body.partnerId = partnerId;
            body.email = formValues.email ?? '';
            body.invitationStatusId = 1;
            body.createdByUserId = currentUser?.id;
            body.lastUpdatedByUserId = currentUser?.id;
            createPartnerAdminInvitation.mutate(body);
        },
        [currentUser?.id, partnerId],
    );

    const isSubmitting = createPartnerAdminInvitation.isLoading;

    return (
        <Form {...form}>
            <form className='grid grid-cols-12 gap-4' onSubmit={form.handleSubmit(onSubmit)}>
                <FormField
                    control={form.control}
                    name='email'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel required tooltip={ToolTips.PartnerUserNameSearch}>
                                Enter the Email to Send Invitation to
                            </FormLabel>
                            <FormControl>
                                <Input {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <div className='col-span-12 flex justify-end gap-2'>
                    <Button asChild data-test='cancel' variant='secondary'>
                        <Link to={`/partnerdashboard/${partnerId}/admins`}>Cancel</Link>
                    </Button>
                    <Button disabled={isSubmitting} type='submit'>
                        {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                        Send Invite
                    </Button>
                </div>
            </form>
        </Form>
    );
};
