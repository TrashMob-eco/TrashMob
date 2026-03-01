import { useCallback, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Loader2, Plus } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { useToast } from '@/hooks/use-toast';
import ContactTagData from '@/components/Models/ContactTagData';
import {
    CreateContactTag,
    DeleteContactTag,
    GetContactTags,
    UpdateContactTag,
} from '@/services/contacts';
import { getColumns } from './columns';

const tagSchema = z.object({
    name: z.string().min(1, 'Name is required'),
    color: z.string(),
});

type TagFormInputs = z.infer<typeof tagSchema>;

export const SiteAdminContactTags = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [dialogOpen, setDialogOpen] = useState(false);
    const [editingTag, setEditingTag] = useState<ContactTagData | null>(null);

    const { data: tags } = useQuery({
        queryKey: GetContactTags().key,
        queryFn: GetContactTags().service,
        select: (res) => res.data,
    });

    const createTag = useMutation({
        mutationKey: CreateContactTag().key,
        mutationFn: CreateContactTag().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/contacttags'], refetchType: 'all' });
            toast({ variant: 'primary', title: 'Tag created' });
            closeDialog();
        },
    });

    const updateTag = useMutation({
        mutationKey: UpdateContactTag().key,
        mutationFn: UpdateContactTag().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/contacttags'], refetchType: 'all' });
            toast({ variant: 'primary', title: 'Tag updated' });
            closeDialog();
        },
    });

    const deleteTag = useMutation({
        mutationKey: DeleteContactTag().key,
        mutationFn: DeleteContactTag().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/contacttags'], refetchType: 'all' });
            toast({ variant: 'default', title: 'Tag deleted' });
        },
    });

    const form = useForm<TagFormInputs>({
        resolver: zodResolver(tagSchema),
        defaultValues: { name: '', color: '#6366f1' },
    });

    const closeDialog = () => {
        setDialogOpen(false);
        setEditingTag(null);
        form.reset({ name: '', color: '#6366f1' });
    };

    const openCreate = () => {
        setEditingTag(null);
        form.reset({ name: '', color: '#6366f1' });
        setDialogOpen(true);
    };

    const openEdit = (tag: ContactTagData) => {
        setEditingTag(tag);
        form.reset({ name: tag.name, color: tag.color || '#6366f1' });
        setDialogOpen(true);
    };

    const handleDelete = (id: string, name: string) => {
        if (!window.confirm(`Are you sure you want to delete the tag "${name}"?`)) return;
        deleteTag.mutate({ id });
    };

    const onSubmit = useCallback(
        (values: TagFormInputs) => {
            if (editingTag) {
                const body = new ContactTagData();
                body.id = editingTag.id;
                body.name = values.name;
                body.color = values.color;
                body.createdByUserId = editingTag.createdByUserId;
                body.createdDate = editingTag.createdDate;
                updateTag.mutate(body);
            } else {
                const body = new ContactTagData();
                body.name = values.name;
                body.color = values.color;
                createTag.mutate(body);
            }
        },
        [editingTag, createTag, updateTag],
    );

    const columns = getColumns({ onEdit: openEdit, onDelete: handleDelete });
    const isSubmitting = createTag.isPending || updateTag.isPending;

    return (
        <>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <CardTitle>Contact Tags ({(tags || []).length})</CardTitle>
                    <Button onClick={openCreate}>
                        <Plus /> Add Tag
                    </Button>
                </CardHeader>
                <CardContent>
                    <DataTable columns={columns} data={tags || []} enableSearch searchPlaceholder='Search tags...' searchColumns={['name']} />
                </CardContent>
            </Card>

            <Dialog open={dialogOpen} onOpenChange={(open) => !open && closeDialog()}>
                <DialogContent className='sm:max-w-[400px]' onOpenAutoFocus={(e) => e.preventDefault()}>
                    <DialogHeader>
                        <DialogTitle>{editingTag ? 'Edit Tag' : 'Create Tag'}</DialogTitle>
                    </DialogHeader>
                    <Form {...form}>
                        <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
                            <FormField
                                control={form.control}
                                name='name'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Name</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='Tag name' />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='color'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Color</FormLabel>
                                        <FormControl>
                                            <Input {...field} type='color' className='h-10 w-20 p-1' />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <div className='flex justify-end gap-2'>
                                <Button type='button' variant='secondary' onClick={closeDialog}>
                                    Cancel
                                </Button>
                                <Button type='submit' disabled={isSubmitting}>
                                    {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                                    {editingTag ? 'Update' : 'Create'}
                                </Button>
                            </div>
                        </form>
                    </Form>
                </DialogContent>
            </Dialog>
        </>
    );
};
