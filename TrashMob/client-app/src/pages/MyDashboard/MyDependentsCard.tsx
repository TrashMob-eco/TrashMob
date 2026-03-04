import { FC, useState } from 'react';
import { useQuery, useQueryClient, useMutation } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { format, parseISO } from 'date-fns';
import { Baby, Pencil, Trash2, Plus } from 'lucide-react';
import { AxiosResponse } from 'axios';

import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { useToast } from '@/hooks/use-toast';
import DependentData from '@/components/Models/DependentData';
import { GetMyDependents, AddDependent, UpdateDependent, DeleteDependent } from '@/services/dependents';

interface MyDependentsCardProps {
    userId: string;
}

const relationships = ['Parent', 'Legal Guardian', 'Grandparent', 'Authorized Supervisor', 'Other'] as const;

const dependentFormSchema = z.object({
    firstName: z.string().min(1, 'First name is required'),
    lastName: z.string().min(1, 'Last name is required'),
    dateOfBirth: z.string().min(1, 'Date of birth is required'),
    relationship: z.string().min(1, 'Relationship is required'),
    medicalNotes: z.string(),
    emergencyContactPhone: z.string(),
});

type DependentFormInputs = z.infer<typeof dependentFormSchema>;

export const MyDependentsCard: FC<MyDependentsCardProps> = ({ userId }) => {
    const queryClient = useQueryClient();
    const { toast } = useToast();
    const [showFormDialog, setShowFormDialog] = useState(false);
    const [editingDependent, setEditingDependent] = useState<DependentData | null>(null);
    const [deletingDependent, setDeletingDependent] = useState<DependentData | null>(null);

    const queryConfig = GetMyDependents({ userId });
    const { data: dependents } = useQuery<AxiosResponse<DependentData[]>, unknown, DependentData[]>({
        queryKey: queryConfig.key,
        queryFn: queryConfig.service,
        select: (res) => res.data,
        enabled: !!userId,
    });

    const form = useForm<DependentFormInputs>({
        resolver: zodResolver(dependentFormSchema),
        defaultValues: {
            firstName: '',
            lastName: '',
            dateOfBirth: '',
            relationship: 'Parent',
            medicalNotes: '',
            emergencyContactPhone: '',
        },
    });

    const invalidateDependents = () => {
        queryClient.invalidateQueries({ queryKey: ['/users', userId, 'dependents'], refetchType: 'all' });
    };

    const addMutation = useMutation({
        mutationFn: AddDependent({ userId }).service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Dependent added!' });
            invalidateDependents();
            closeFormDialog();
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to add dependent.' });
        },
    });

    const updateMutation = useMutation({
        mutationFn: UpdateDependent({ userId }).service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Dependent updated!' });
            invalidateDependents();
            closeFormDialog();
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to update dependent.' });
        },
    });

    const deleteMutation = useMutation({
        mutationFn: async () => {
            if (!deletingDependent) return;
            return DeleteDependent({ userId, dependentId: deletingDependent.id }).service();
        },
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Dependent removed.' });
            invalidateDependents();
            setDeletingDependent(null);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to remove dependent.' });
        },
    });

    const openAddDialog = () => {
        setEditingDependent(null);
        form.reset({
            firstName: '',
            lastName: '',
            dateOfBirth: '',
            relationship: 'Parent',
            medicalNotes: '',
            emergencyContactPhone: '',
        });
        setShowFormDialog(true);
    };

    const openEditDialog = (dependent: DependentData) => {
        setEditingDependent(dependent);
        form.reset({
            firstName: dependent.firstName,
            lastName: dependent.lastName,
            dateOfBirth: dependent.dateOfBirth ? dependent.dateOfBirth.split('T')[0] : '',
            relationship: dependent.relationship,
            medicalNotes: dependent.medicalNotes,
            emergencyContactPhone: dependent.emergencyContactPhone,
        });
        setShowFormDialog(true);
    };

    const closeFormDialog = () => {
        setShowFormDialog(false);
        setEditingDependent(null);
        form.reset();
    };

    const onSubmit = (values: DependentFormInputs) => {
        const data: DependentData = {
            ...new DependentData(),
            ...values,
            parentUserId: userId,
        };

        if (editingDependent) {
            data.id = editingDependent.id;
            data.isActive = editingDependent.isActive;
            updateMutation.mutate(data);
        } else {
            addMutation.mutate(data);
        }
    };

    const dependentCount = (dependents || []).length;
    const isSubmitting = addMutation.isPending || updateMutation.isPending;

    return (
        <>
            <Card className='mb-4'>
                <CardHeader>
                    <div className='flex flex-row items-center'>
                        <Baby className='inline-block h-5 w-5 mr-2 text-primary' />
                        <CardTitle className='grow text-primary'>My Dependents ({dependentCount})</CardTitle>
                        <Button variant='outline' size='sm' onClick={openAddDialog}>
                            <Plus className='h-4 w-4 mr-1' />
                            Add Dependent
                        </Button>
                    </div>
                </CardHeader>
                <CardContent>
                    {dependentCount === 0 ? (
                        <p className='text-muted-foreground text-center py-4'>
                            No dependents added yet. Add a dependent minor to register them for events.
                        </p>
                    ) : (
                        <div className='overflow-auto'>
                            <table className='w-full'>
                                <thead>
                                    <tr className='border-b'>
                                        <th className='text-left py-2 px-2 font-medium'>Name</th>
                                        <th className='text-left py-2 px-2 font-medium'>Relationship</th>
                                        <th className='text-left py-2 px-2 font-medium'>Date of Birth</th>
                                        <th className='text-right py-2 px-2 font-medium'>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {(dependents || []).map((dep) => (
                                        <tr key={dep.id} className='border-b last:border-0'>
                                            <td className='py-3 px-2'>
                                                <div className='font-medium'>
                                                    {dep.firstName} {dep.lastName}
                                                </div>
                                                {dep.medicalNotes ? (
                                                    <div className='text-sm text-muted-foreground'>
                                                        Medical notes on file
                                                    </div>
                                                ) : null}
                                            </td>
                                            <td className='py-3 px-2'>
                                                <Badge variant='secondary'>{dep.relationship}</Badge>
                                            </td>
                                            <td className='py-3 px-2 text-sm'>
                                                {dep.dateOfBirth
                                                    ? format(parseISO(dep.dateOfBirth), 'MMM d, yyyy')
                                                    : '—'}
                                            </td>
                                            <td className='py-3 px-2 text-right'>
                                                <Button
                                                    variant='ghost'
                                                    size='sm'
                                                    onClick={() => openEditDialog(dep)}
                                                    title='Edit'
                                                >
                                                    <Pencil className='h-4 w-4' />
                                                </Button>
                                                <Button
                                                    variant='ghost'
                                                    size='sm'
                                                    onClick={() => setDeletingDependent(dep)}
                                                    title='Remove'
                                                >
                                                    <Trash2 className='h-4 w-4 text-destructive' />
                                                </Button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </CardContent>
            </Card>

            {/* Add/Edit Dialog */}
            <Dialog open={showFormDialog} onOpenChange={(open) => !open && closeFormDialog()}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>{editingDependent ? 'Edit Dependent' : 'Add Dependent'}</DialogTitle>
                        <DialogDescription>
                            {editingDependent
                                ? 'Update the details for this dependent minor.'
                                : 'Add a dependent minor you may bring to cleanup events.'}
                        </DialogDescription>
                    </DialogHeader>
                    <Form {...form}>
                        <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
                            <div className='grid grid-cols-2 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='firstName'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>First Name</FormLabel>
                                            <FormControl>
                                                <Input {...field} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='lastName'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel required>Last Name</FormLabel>
                                            <FormControl>
                                                <Input {...field} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </div>
                            <FormField
                                control={form.control}
                                name='dateOfBirth'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel required>Date of Birth</FormLabel>
                                        <FormControl>
                                            <Input type='date' {...field} />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='relationship'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel required>Relationship</FormLabel>
                                        <Select onValueChange={field.onChange} value={field.value}>
                                            <FormControl>
                                                <SelectTrigger>
                                                    <SelectValue placeholder='Select relationship' />
                                                </SelectTrigger>
                                            </FormControl>
                                            <SelectContent>
                                                {relationships.map((r) => (
                                                    <SelectItem key={r} value={r}>
                                                        {r}
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
                                name='emergencyContactPhone'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Emergency Contact Phone</FormLabel>
                                        <FormControl>
                                            <Input type='tel' placeholder='(555) 123-4567' {...field} />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name='medicalNotes'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Medical Notes</FormLabel>
                                        <FormControl>
                                            <Textarea
                                                placeholder='Allergies, medications, or other medical information...'
                                                {...field}
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <DialogFooter>
                                <Button type='button' variant='outline' onClick={closeFormDialog}>
                                    Cancel
                                </Button>
                                <Button type='submit' disabled={isSubmitting}>
                                    {isSubmitting ? 'Saving...' : editingDependent ? 'Save Changes' : 'Add Dependent'}
                                </Button>
                            </DialogFooter>
                        </form>
                    </Form>
                </DialogContent>
            </Dialog>

            {/* Delete Confirmation Dialog */}
            <AlertDialog open={!!deletingDependent} onOpenChange={(open) => !open && setDeletingDependent(null)}>
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>Remove Dependent</AlertDialogTitle>
                        <AlertDialogDescription>
                            Are you sure you want to remove {deletingDependent?.firstName} {deletingDependent?.lastName}
                            ? This will also remove them from any event registrations.
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                        <AlertDialogAction
                            onClick={() => deleteMutation.mutate()}
                            className='bg-destructive text-destructive-foreground hover:bg-destructive/90'
                        >
                            {deleteMutation.isPending ? 'Removing...' : 'Remove'}
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </>
    );
};
