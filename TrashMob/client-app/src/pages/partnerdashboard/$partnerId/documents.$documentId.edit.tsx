import { useCallback, useEffect } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage, FormDescription } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { FileUp, Loader2 } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import * as ToolTips from '@/store/ToolTips';
import { useLogin } from '@/hooks/useLogin';
import {
    GetPartnerDocumentsByDocumentId,
    GetPartnerDocumentsByPartnerId,
    UpdatePartnerDocument,
} from '@/services/documents';

import PartnerDocumentData from '@/components/Models/PartnerDocumentData';

const documentTypeOptions = [
    { value: '0', label: 'Other' },
    { value: '1', label: 'Agreement' },
    { value: '2', label: 'Contract' },
    { value: '3', label: 'Report' },
    { value: '4', label: 'Insurance' },
    { value: '5', label: 'Certificate' },
];

interface FormInputs {
    name: string;
    url: string;
    documentTypeId: string;
    expirationDate: string;
}

const formSchema = z.object({
    name: z.string({ required_error: 'Name cannot be blank.' }),
    url: z.string().url().or(z.literal('')),
    documentTypeId: z.string(),
    expirationDate: z.string().optional(),
});

export const PartnerDocumentEdit = () => {
    const { partnerId, documentId } = useParams<{ partnerId: string; documentId: string }>() as {
        partnerId: string;
        documentId: string;
    };
    const { currentUser } = useLogin();

    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const { data: currentValues, isLoading } = useQuery({
        queryKey: GetPartnerDocumentsByDocumentId({ documentId }).key,
        queryFn: GetPartnerDocumentsByDocumentId({ documentId }).service,
        select: (res) => res.data,
    });

    const onUpdateSuccess = (_data: unknown, variables: PartnerDocumentData) => {
        toast({
            variant: 'primary',
            title: 'Document saved!',
            description: '',
        });
        queryClient.invalidateQueries({
            queryKey: GetPartnerDocumentsByPartnerId({ partnerId }).key,
            refetchType: 'all',
        });

        navigate(`/partnerdashboard/${partnerId}/documents`);
    };

    const updatePartnerDocument = useMutation({
        mutationKey: UpdatePartnerDocument().key,
        mutationFn: UpdatePartnerDocument().service,
        onSuccess: onUpdateSuccess,
    });

    const isUploaded = !!currentValues?.blobStoragePath;

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: currentValues?.name ?? '',
            url: currentValues?.url ?? '',
            documentTypeId: String(currentValues?.documentTypeId ?? 0),
            expirationDate: currentValues?.expirationDate
                ? new Date(currentValues.expirationDate).toISOString().split('T')[0]
                : '',
        },
    });

    useEffect(() => {
        if (currentValues) {
            form.reset({
                name: currentValues.name ?? '',
                url: currentValues.url ?? '',
                documentTypeId: String(currentValues.documentTypeId ?? 0),
                expirationDate: currentValues.expirationDate
                    ? new Date(currentValues.expirationDate).toISOString().split('T')[0]
                    : '',
            });
        }
    }, [currentValues]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!currentValues) return;

            const body = new PartnerDocumentData();
            body.id = documentId;
            body.partnerId = partnerId;
            body.name = formValues.name ?? '';
            body.url = formValues.url ?? '';
            body.documentTypeId = parseInt(formValues.documentTypeId, 10);
            body.expirationDate = formValues.expirationDate ? new Date(formValues.expirationDate) : null;
            body.blobStoragePath = currentValues.blobStoragePath ?? '';
            body.contentType = currentValues.contentType ?? '';
            body.fileSizeBytes = currentValues.fileSizeBytes ?? null;
            body.createdByUserId = currentValues.createdByUserId;
            body.lastUpdatedByUserId = currentUser.id;
            updatePartnerDocument.mutate(body);
        },
        [currentValues, partnerId, documentId, currentUser.id, updatePartnerDocument],
    );

    const isSubmitting = updatePartnerDocument.isPending;

    if (isLoading) {
        return <Loader2 className='animate-spin mx-auto my-10' />;
    }

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
                {isUploaded ? (
                    <div className='col-span-12'>
                        <FormLabel>Uploaded File</FormLabel>
                        <div className='mt-1 flex items-center gap-2 rounded-md border bg-muted/50 p-3'>
                            <FileUp className='h-4 w-4 shrink-0 text-muted-foreground' />
                            <p className='text-sm text-muted-foreground'>
                                Uploaded file — use the Download button from the document list to access.
                            </p>
                        </div>
                    </div>
                ) : (
                    <FormField
                        control={form.control}
                        name='url'
                        render={({ field }) => (
                            <FormItem className='col-span-12'>
                                <FormLabel tooltip={ToolTips.PartnerDocumentUrl} required>
                                    Document URL
                                </FormLabel>
                                <FormControl>
                                    <Input {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                )}
                <FormField
                    control={form.control}
                    name='documentTypeId'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel>Document Type</FormLabel>
                            <Select onValueChange={field.onChange} value={field.value}>
                                <FormControl>
                                    <SelectTrigger>
                                        <SelectValue placeholder='Select type' />
                                    </SelectTrigger>
                                </FormControl>
                                <SelectContent>
                                    {documentTypeOptions.map((opt) => (
                                        <SelectItem key={opt.value} value={opt.value}>
                                            {opt.label}
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
                    name='expirationDate'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel>Expiration Date (optional)</FormLabel>
                            <FormControl>
                                <Input type='date' {...field} />
                            </FormControl>
                            <FormDescription>
                                Set a date when this document expires (e.g., insurance renewal).
                            </FormDescription>
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
                        Save
                    </Button>
                </div>
            </form>
        </Form>
    );
};
