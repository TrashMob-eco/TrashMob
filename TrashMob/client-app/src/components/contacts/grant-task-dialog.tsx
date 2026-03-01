import { useCallback, useEffect } from 'react';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import GrantTaskData from '@/components/Models/GrantTaskData';

const taskSchema = z.object({
    title: z.string().min(1, 'Title is required'),
    dueDate: z.string(),
    notes: z.string(),
});

type TaskFormInputs = z.infer<typeof taskSchema>;

interface GrantTaskDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    task: GrantTaskData | null;
    isPending: boolean;
    onSubmit: (values: { title: string; dueDate: string | null; notes: string }) => void;
}

export const GrantTaskDialog = ({ open, onOpenChange, task, isPending, onSubmit }: GrantTaskDialogProps) => {
    const form = useForm<TaskFormInputs>({
        resolver: zodResolver(taskSchema),
        defaultValues: { title: '', dueDate: '', notes: '' },
    });

    useEffect(() => {
        if (open) {
            form.reset({
                title: task?.title || '',
                dueDate: task?.dueDate ? task.dueDate.split('T')[0] : '',
                notes: task?.notes || '',
            });
        }
    }, [open, task, form]);

    const handleSubmit: SubmitHandler<TaskFormInputs> = useCallback(
        (values) => {
            onSubmit({
                title: values.title,
                dueDate: values.dueDate || null,
                notes: values.notes,
            });
        },
        [onSubmit],
    );

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className='sm:max-w-[450px]'>
                <DialogHeader>
                    <DialogTitle>{task ? 'Edit Task' : 'Add Task'}</DialogTitle>
                </DialogHeader>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(handleSubmit)} className='space-y-4'>
                        <FormField
                            control={form.control}
                            name='title'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Title</FormLabel>
                                    <FormControl>
                                        <Input {...field} placeholder='Task description' />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name='dueDate'
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Due Date</FormLabel>
                                    <FormControl>
                                        <Input {...field} type='date' />
                                    </FormControl>
                                    <FormMessage />
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
                                        <Textarea {...field} placeholder='Additional notes...' rows={3} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <div className='flex justify-end gap-2'>
                            <Button type='button' variant='secondary' onClick={() => onOpenChange(false)}>
                                Cancel
                            </Button>
                            <Button type='submit' disabled={isPending}>
                                {isPending ? <Loader2 className='animate-spin' /> : null}
                                {task ? 'Save' : 'Add'}
                            </Button>
                        </div>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    );
};
