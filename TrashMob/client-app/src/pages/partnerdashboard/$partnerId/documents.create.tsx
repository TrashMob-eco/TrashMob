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
import { CreatePartnerDocument, GetPartnerDocumentsByPartnerId } from '@/services/documents';

import PartnerDocumentData from '@/components/Models/PartnerDocumentData';

interface FormInputs {
    name: string;
    url: string;
}

const formSchema = z.object({
    name: z.string({ required_error: 'Name cannot be blank.' }),
    url: z.string().url(),
});

export const PartnerDocumentCreate = () => {
    const { partnerId, documentId } = useParams<{ partnerId: string; documentId: string }>() as {
        partnerId: string;
        documentId: string;
    };
    const { currentUser } = useLogin();

    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const onUpdateSuccess = useCallback(
        (_data: unknown, variables: PartnerDocumentData) => {
            toast({
                variant: 'primary',
                title: 'Document added!',
                description: '',
            });
            queryClient.invalidateQueries({
                queryKey: GetPartnerDocumentsByPartnerId({ partnerId }).key,
                refetchType: 'all',
            });

            navigate(`/partnerdashboard/${partnerId}/documents`);
        },
        [partnerId],
    );

    const createPartnerDocument = useMutation({
        mutationKey: CreatePartnerDocument().key,
        mutationFn: CreatePartnerDocument().service,
        onSuccess: onUpdateSuccess,
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            url: '',
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            const body = new PartnerDocumentData();
            body.id = documentId;
            body.partnerId = partnerId;
            body.name = formValues.name ?? '';
            body.url = formValues.url ?? '';
            body.lastUpdatedByUserId = currentUser.id;
            body.createdByUserId = currentUser.id;
            createPartnerDocument.mutate(body);
        },
        [partnerId],
    );

    const isSubmitting = createPartnerDocument.isLoading;

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                <FormField
                    control={form.control}
                    name='name'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel tooltip={ToolTips.PartnerDocumentName} required>
                                Document Name
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
                    name='url'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel tooltip={ToolTips.PartnerDocumentUrl} required>
                                Document Url
                            </FormLabel>
                            <FormControl>
                                <Input {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <div className='col-span-12 flex justify-end gap-2'>
                    <Button variant='secondary' data-test='cancel' asChild>
                        <Link to={`/partnerdashboard/${partnerId}/documents`}>Cancel</Link>
                    </Button>
                    <Button type='submit' disabled={isSubmitting}>
                        {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                        Add
                    </Button>
                </div>
            </form>
        </Form>
    );
};
