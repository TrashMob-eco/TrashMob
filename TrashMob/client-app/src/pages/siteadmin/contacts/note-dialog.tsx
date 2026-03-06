import { useCallback, useEffect } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import { NOTE_TYPES } from '@/components/contacts/contact-constants';
import ContactNoteData from '@/components/Models/ContactNoteData';
import { CreateContactNote, UpdateContactNote } from '@/services/contacts';

const noteSchema = z.object({
    noteType: z.string(),
    subject: z.string(),
    body: z.string().min(1, 'Note body is required'),
});

type NoteFormInputs = z.infer<typeof noteSchema>;

interface NoteDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    contactId: string;
    editingNote?: ContactNoteData | null;
}

export const NoteDialog = ({ open, onOpenChange, contactId, editingNote }: NoteDialogProps) => {
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const createNote = useMutation({
        mutationKey: CreateContactNote().key,
        mutationFn: CreateContactNote().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/contactnotes', contactId], refetchType: 'all' });
            toast({ variant: 'primary', title: 'Note added' });
            onOpenChange(false);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to add note. Please try again.' });
        },
    });

    const updateNote = useMutation({
        mutationKey: UpdateContactNote().key,
        mutationFn: UpdateContactNote().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/contactnotes', contactId], refetchType: 'all' });
            toast({ variant: 'primary', title: 'Note updated' });
            onOpenChange(false);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to update note. Please try again.' });
        },
    });

    const form = useForm<NoteFormInputs>({
        resolver: zodResolver(noteSchema),
        defaultValues: { noteType: '1', subject: '', body: '' },
    });

    useEffect(() => {
        if (open) {
            if (editingNote) {
                form.reset({
                    noteType: (editingNote.noteType || 1).toString(),
                    subject: editingNote.subject || '',
                    body: editingNote.body || '',
                });
            } else {
                form.reset({ noteType: '1', subject: '', body: '' });
            }
        }
    }, [open, editingNote, form]);

    const onSubmit: SubmitHandler<NoteFormInputs> = useCallback(
        (values) => {
            const body = new ContactNoteData();
            body.contactId = contactId;
            body.noteType = parseInt(values.noteType);
            body.subject = values.subject;
            body.body = values.body;

            if (editingNote) {
                body.id = editingNote.id;
                body.createdByUserId = editingNote.createdByUserId;
                body.createdDate = editingNote.createdDate;
                updateNote.mutate(body);
            } else {
                createNote.mutate(body);
            }
        },
        [contactId, editingNote, createNote, updateNote],
    );

    const isSubmitting = createNote.isPending || updateNote.isPending;

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className='sm:max-w-[500px]' onOpenAutoFocus={(e) => e.preventDefault()}>
                <DialogHeader>
                    <DialogTitle>{editingNote ? 'Edit Note' : 'Add Note'}</DialogTitle>
                </DialogHeader>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
                        <FormField
                            control={form.control}
                            name='noteType'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Type</FormLabel>
                                    <Select onValueChange={field.onChange} value={field.value}>
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {NOTE_TYPES.map((t) => (
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
                            name='subject'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Subject</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Note subject (optional)' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='body'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel required>Note</FormLabel>
                                    <FormControl>
                                        <Textarea {...field} placeholder='Write your note...' rows={5} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <div className='flex justify-end gap-2'>
                            <Button type='button' variant='secondary' onClick={() => onOpenChange(false)}>
                                Cancel
                            </Button>
                            <Button type='submit' disabled={isSubmitting}>
                                {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                                {editingNote ? 'Update' : 'Add'}
                            </Button>
                        </div>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    );
};
