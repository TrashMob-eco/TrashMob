import { useCallback, useRef, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage, FormDescription } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { FileUp, Loader2, X } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import * as ToolTips from '@/store/ToolTips';
import { useLogin } from '@/hooks/useLogin';
import { useFeatureMetrics } from '@/hooks/useFeatureMetrics';
import { CreatePartnerDocument, GetPartnerDocumentsByPartnerId, UploadPartnerDocument } from '@/services/documents';

import PartnerDocumentData from '@/components/Models/PartnerDocumentData';

const documentTypeOptions = [
    { value: '0', label: 'Other' },
    { value: '1', label: 'Agreement' },
    { value: '2', label: 'Contract' },
    { value: '3', label: 'Report' },
    { value: '4', label: 'Insurance' },
    { value: '5', label: 'Certificate' },
];

const acceptedFileTypes = '.pdf,.docx,.xlsx,.png,.jpg,.jpeg';
const maxFileSizeMB = 25;
const maxFileSizeBytes = maxFileSizeMB * 1024 * 1024;

const uploadSchema = z.object({
    name: z.string().min(1, 'Name is required'),
    documentTypeId: z.string(),
    expirationDate: z.string().optional(),
});

const urlSchema = z.object({
    name: z.string().min(1, 'Name is required'),
    url: z.string().url('Please enter a valid URL'),
    documentTypeId: z.string(),
    expirationDate: z.string().optional(),
});

function formatFileSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

export const PartnerDocumentCreate = () => {
    const { partnerId, documentId } = useParams<{ partnerId: string; documentId: string }>() as {
        partnerId: string;
        documentId: string;
    };
    const { currentUser } = useLogin();
    const { toast } = useToast();
    const { track } = useFeatureMetrics();
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const fileInputRef = useRef<HTMLInputElement>(null);

    const [mode, setMode] = useState<'upload' | 'url'>('upload');
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [fileError, setFileError] = useState('');

    const onSuccess = useCallback(() => {
        track({ category: 'Partner', action: 'Create', target: 'Document', properties: { mode } });
        toast({ variant: 'primary', title: 'Document added!', description: '' });
        queryClient.invalidateQueries({
            queryKey: GetPartnerDocumentsByPartnerId({ partnerId }).key,
            refetchType: 'all',
        });
        navigate(`/partnerdashboard/${partnerId}/documents`);
    }, [partnerId, mode, toast, track, queryClient, navigate]);

    const uploadMutation = useMutation({
        mutationKey: UploadPartnerDocument().key,
        mutationFn: UploadPartnerDocument().service,
        onSuccess,
    });

    const createMutation = useMutation({
        mutationKey: CreatePartnerDocument().key,
        mutationFn: CreatePartnerDocument().service,
        onSuccess,
    });

    const uploadForm = useForm<z.infer<typeof uploadSchema>>({
        resolver: zodResolver(uploadSchema),
        defaultValues: { name: '', documentTypeId: '0', expirationDate: '' },
    });

    const urlForm = useForm<z.infer<typeof urlSchema>>({
        resolver: zodResolver(urlSchema),
        defaultValues: { name: '', url: '', documentTypeId: '0', expirationDate: '' },
    });

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFileError('');
        const file = e.target.files?.[0];
        if (!file) {
            setSelectedFile(null);
            return;
        }
        if (file.size > maxFileSizeBytes) {
            setFileError(`File is too large. Maximum size is ${maxFileSizeMB} MB.`);
            setSelectedFile(null);
            return;
        }
        setSelectedFile(file);
    };

    const clearFile = () => {
        setSelectedFile(null);
        setFileError('');
        if (fileInputRef.current) fileInputRef.current.value = '';
    };

    const onUploadSubmit: SubmitHandler<z.infer<typeof uploadSchema>> = useCallback(
        (formValues) => {
            if (!selectedFile) {
                setFileError('Please select a file to upload.');
                return;
            }
            uploadMutation.mutate({
                partnerId,
                name: formValues.name,
                documentTypeId: parseInt(formValues.documentTypeId, 10),
                expirationDate: formValues.expirationDate || undefined,
                file: selectedFile,
            });
        },
        [partnerId, selectedFile, uploadMutation],
    );

    const onUrlSubmit: SubmitHandler<z.infer<typeof urlSchema>> = useCallback(
        (formValues) => {
            const body = new PartnerDocumentData();
            body.id = documentId;
            body.partnerId = partnerId;
            body.name = formValues.name;
            body.url = formValues.url;
            body.documentTypeId = parseInt(formValues.documentTypeId, 10);
            body.expirationDate = formValues.expirationDate ? new Date(formValues.expirationDate) : null;
            body.lastUpdatedByUserId = currentUser.id;
            body.createdByUserId = currentUser.id;
            createMutation.mutate(body);
        },
        [partnerId, documentId, currentUser.id, createMutation],
    );

    const isSubmitting = uploadMutation.isPending || createMutation.isPending;

    return (
        <Tabs value={mode} onValueChange={(v) => setMode(v as 'upload' | 'url')}>
            <TabsList className='mb-4 w-full'>
                <TabsTrigger value='upload' className='flex-1'>
                    Upload File
                </TabsTrigger>
                <TabsTrigger value='url' className='flex-1'>
                    External URL
                </TabsTrigger>
            </TabsList>

            <TabsContent value='upload'>
                <Form {...uploadForm}>
                    <form onSubmit={uploadForm.handleSubmit(onUploadSubmit)} className='grid grid-cols-12 gap-4'>
                        <FormField
                            control={uploadForm.control}
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
                            control={uploadForm.control}
                            name='documentTypeId'
                            render={({ field }) => (
                                <FormItem className='col-span-12'>
                                    <FormLabel>Document Type</FormLabel>
                                    <Select onValueChange={field.onChange} defaultValue={field.value}>
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
                        <div className='col-span-12'>
                            <FormLabel>File</FormLabel>
                            <div className='mt-1'>
                                {selectedFile ? (
                                    <div className='flex items-center gap-2 rounded-md border p-3'>
                                        <FileUp className='h-4 w-4 shrink-0 text-muted-foreground' />
                                        <div className='min-w-0 flex-1'>
                                            <p className='truncate text-sm font-medium'>{selectedFile.name}</p>
                                            <p className='text-xs text-muted-foreground'>
                                                {formatFileSize(selectedFile.size)}
                                            </p>
                                        </div>
                                        <Button type='button' variant='ghost' size='icon' onClick={clearFile}>
                                            <X className='h-4 w-4' />
                                        </Button>
                                    </div>
                                ) : (
                                    <Input
                                        ref={fileInputRef}
                                        type='file'
                                        accept={acceptedFileTypes}
                                        onChange={handleFileChange}
                                    />
                                )}
                                {fileError ? <p className='mt-1 text-sm text-red-600'>{fileError}</p> : null}
                                <FormDescription className='mt-1'>
                                    PDF, Word, Excel, PNG, or JPEG. Max {maxFileSizeMB} MB.
                                </FormDescription>
                            </div>
                        </div>
                        <FormField
                            control={uploadForm.control}
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
                                Upload
                            </Button>
                        </div>
                    </form>
                </Form>
            </TabsContent>

            <TabsContent value='url'>
                <Form {...urlForm}>
                    <form onSubmit={urlForm.handleSubmit(onUrlSubmit)} className='grid grid-cols-12 gap-4'>
                        <FormField
                            control={urlForm.control}
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
                            control={urlForm.control}
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
                        <FormField
                            control={urlForm.control}
                            name='documentTypeId'
                            render={({ field }) => (
                                <FormItem className='col-span-12'>
                                    <FormLabel>Document Type</FormLabel>
                                    <Select onValueChange={field.onChange} defaultValue={field.value}>
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
                            control={urlForm.control}
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
                                Add
                            </Button>
                        </div>
                    </form>
                </Form>
            </TabsContent>
        </Tabs>
    );
};
