import { useCallback, useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Loader2, ArrowLeft, UserPlus, Trash2 } from 'lucide-react';

import { Form, FormControl, FormField, FormItem, FormMessage, FormDescription } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Switch } from '@/components/ui/switch';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
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
    AlertDialogTrigger,
} from '@/components/ui/alert-dialog';
import { useToast } from '@/hooks/use-toast';
import ProfessionalCompanyData from '@/components/Models/ProfessionalCompanyData';
import {
    GetProfessionalCompany,
    GetProfessionalCompanies,
    UpdateProfessionalCompany,
    GetCompanyUsers,
    AssignCompanyUser,
    RemoveCompanyUser,
    CompanyUserData,
} from '@/services/professional-companies';

interface FormInputs {
    name: string;
    contactEmail: string;
    contactPhone: string;
    isActive: boolean;
}

const formSchema = z.object({
    name: z.string().min(1, 'Name is required').max(200, 'Name must be less than 200 characters'),
    contactEmail: z.string().max(256).email('Must be a valid email').or(z.literal('')),
    contactPhone: z.string().max(30),
    isActive: z.boolean(),
});

export const PartnerCommunityCompanyEdit = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId, companyId } = useParams<{ partnerId: string; companyId: string }>() as {
        partnerId: string;
        companyId: string;
    };
    const { toast } = useToast();
    const [addUserDialogOpen, setAddUserDialogOpen] = useState(false);
    const [newUserId, setNewUserId] = useState('');

    const { data: company, isLoading } = useQuery<
        AxiosResponse<ProfessionalCompanyData>,
        unknown,
        ProfessionalCompanyData
    >({
        queryKey: GetProfessionalCompany({ partnerId, companyId }).key,
        queryFn: GetProfessionalCompany({ partnerId, companyId }).service,
        select: (res) => res.data,
        enabled: !!partnerId && !!companyId,
    });

    const { data: users, isLoading: usersLoading } = useQuery<
        AxiosResponse<CompanyUserData[]>,
        unknown,
        CompanyUserData[]
    >({
        queryKey: GetCompanyUsers({ partnerId, companyId }).key,
        queryFn: GetCompanyUsers({ partnerId, companyId }).service,
        select: (res) => res.data,
        enabled: !!partnerId && !!companyId,
    });

    const { mutate: updateCompany, isPending: isSubmitting } = useMutation({
        mutationKey: UpdateProfessionalCompany().key,
        mutationFn: (body: ProfessionalCompanyData) =>
            UpdateProfessionalCompany().service({ partnerId, companyId }, body),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetProfessionalCompanies({ partnerId }).key });
            queryClient.invalidateQueries({ queryKey: GetProfessionalCompany({ partnerId, companyId }).key });
            toast({
                variant: 'primary',
                title: 'Company updated!',
                description: 'The professional company has been updated successfully.',
            });
            navigate(`/partnerdashboard/${partnerId}/community/companies`);
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to update company. Please try again.',
            });
        },
    });

    const { mutate: assignUser, isPending: isAssigning } = useMutation({
        mutationKey: AssignCompanyUser().key,
        mutationFn: (body: { professionalCompanyId: string; userId: string }) =>
            AssignCompanyUser().service({ partnerId, companyId }, body),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetCompanyUsers({ partnerId, companyId }).key });
            toast({
                variant: 'primary',
                title: 'User assigned',
                description: 'The user has been assigned to this company.',
            });
            setAddUserDialogOpen(false);
            setNewUserId('');
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to assign user. Please check the User ID and try again.',
            });
        },
    });

    const { mutate: removeUser, isPending: isRemoving } = useMutation({
        mutationKey: RemoveCompanyUser().key,
        mutationFn: RemoveCompanyUser().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetCompanyUsers({ partnerId, companyId }).key });
            toast({
                variant: 'primary',
                title: 'User removed',
                description: 'The user has been removed from this company.',
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to remove user. Please try again.',
            });
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            name: '',
            contactEmail: '',
            contactPhone: '',
            isActive: true,
        },
    });

    useEffect(() => {
        if (company) {
            form.reset({
                name: company.name,
                contactEmail: company.contactEmail,
                contactPhone: company.contactPhone,
                isActive: company.isActive,
            });
        }
    }, [company, form]);

    const onSubmit: SubmitHandler<FormInputs> = useCallback(
        (formValues) => {
            if (!partnerId || !company) return;

            const updated: ProfessionalCompanyData = {
                ...company,
                name: formValues.name,
                contactEmail: formValues.contactEmail,
                contactPhone: formValues.contactPhone,
                isActive: formValues.isActive,
            };

            updateCompany(updated);
        },
        [partnerId, company, updateCompany],
    );

    const handleAssignUser = useCallback(() => {
        if (!newUserId.trim()) return;
        assignUser({ professionalCompanyId: companyId, userId: newUserId.trim() });
    }, [companyId, newUserId, assignUser]);

    const handleRemoveUser = useCallback(
        (userId: string) => {
            removeUser({ partnerId, companyId, userId });
        },
        [partnerId, companyId, removeUser],
    );

    if (isLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='py-8'>
            <div className='mb-6'>
                <Button variant='ghost' size='sm' asChild>
                    <Link to={`/partnerdashboard/${partnerId}/community/companies`}>
                        <ArrowLeft className='h-4 w-4 mr-2' />
                        Back to Companies
                    </Link>
                </Button>
            </div>

            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-6'>
                    <Card>
                        <CardHeader>
                            <CardTitle>Edit Professional Company</CardTitle>
                            <CardDescription>Update company information.</CardDescription>
                        </CardHeader>
                        <CardContent className='space-y-4'>
                            <FormField
                                control={form.control}
                                name='name'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel required>Company Name</FormLabel>
                                        <FormControl>
                                            <Input {...field} placeholder='e.g., CleanPro Services' />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                <FormField
                                    control={form.control}
                                    name='contactEmail'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Contact Email</FormLabel>
                                            <FormControl>
                                                <Input {...field} type='email' placeholder='contact@company.com' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='contactPhone'
                                    render={({ field }) => (
                                        <FormItem>
                                            <FormLabel>Contact Phone</FormLabel>
                                            <FormControl>
                                                <Input {...field} placeholder='(555) 123-4567' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                            </div>
                            <FormField
                                control={form.control}
                                name='isActive'
                                render={({ field }) => (
                                    <FormItem className='flex flex-row items-center justify-between rounded-lg border p-4'>
                                        <div className='space-y-0.5'>
                                            <FormLabel>Active</FormLabel>
                                            <FormDescription>
                                                Inactive companies cannot be assigned to new sponsored adoptions.
                                            </FormDescription>
                                        </div>
                                        <FormControl>
                                            <Switch checked={field.value} onCheckedChange={field.onChange} />
                                        </FormControl>
                                    </FormItem>
                                )}
                            />
                        </CardContent>
                    </Card>

                    <div className='flex justify-end gap-2'>
                        <Button
                            type='button'
                            variant='outline'
                            onClick={() => navigate(`/partnerdashboard/${partnerId}/community/companies`)}
                        >
                            Cancel
                        </Button>
                        <Button type='submit' disabled={isSubmitting}>
                            {isSubmitting ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                            Save Changes
                        </Button>
                    </div>
                </form>
            </Form>

            <Card className='mt-6'>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div>
                        <CardTitle>Company Users</CardTitle>
                        <CardDescription>
                            Users assigned to this company can log cleanup activities for sponsored adoptions.
                        </CardDescription>
                    </div>
                    <Dialog open={addUserDialogOpen} onOpenChange={setAddUserDialogOpen}>
                        <DialogTrigger asChild>
                            <Button variant='outline' size='sm'>
                                <UserPlus className='h-4 w-4 mr-2' />
                                Add User
                            </Button>
                        </DialogTrigger>
                        <DialogContent>
                            <DialogHeader>
                                <DialogTitle>Add User to Company</DialogTitle>
                                <DialogDescription>
                                    Enter the User ID to assign them to this professional company.
                                </DialogDescription>
                            </DialogHeader>
                            <div className='py-4'>
                                <Input
                                    value={newUserId}
                                    onChange={(e) => setNewUserId(e.target.value)}
                                    placeholder='Enter User ID'
                                />
                            </div>
                            <DialogFooter>
                                <Button
                                    variant='outline'
                                    onClick={() => {
                                        setAddUserDialogOpen(false);
                                        setNewUserId('');
                                    }}
                                >
                                    Cancel
                                </Button>
                                <Button onClick={handleAssignUser} disabled={isAssigning || !newUserId.trim()}>
                                    {isAssigning ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                                    Assign User
                                </Button>
                            </DialogFooter>
                        </DialogContent>
                    </Dialog>
                </CardHeader>
                <CardContent>
                    {usersLoading ? (
                        <div className='py-4 text-center'>
                            <Loader2 className='h-6 w-6 animate-spin mx-auto' />
                        </div>
                    ) : users && users.length > 0 ? (
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Username</TableHead>
                                    <TableHead>Email</TableHead>
                                    <TableHead className='text-right'>Actions</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {users.map((user) => (
                                    <TableRow key={user.id}>
                                        <TableCell className='font-medium'>{user.userName}</TableCell>
                                        <TableCell>{user.email}</TableCell>
                                        <TableCell className='text-right'>
                                            <AlertDialog>
                                                <AlertDialogTrigger asChild>
                                                    <Button variant='outline' size='sm' disabled={isRemoving}>
                                                        <Trash2 className='h-4 w-4' />
                                                    </Button>
                                                </AlertDialogTrigger>
                                                <AlertDialogContent>
                                                    <AlertDialogHeader>
                                                        <AlertDialogTitle>Remove User</AlertDialogTitle>
                                                        <AlertDialogDescription>
                                                            Are you sure you want to remove &quot;
                                                            {user.userName}&quot; from this company? They will no longer
                                                            be able to log cleanup activities.
                                                        </AlertDialogDescription>
                                                    </AlertDialogHeader>
                                                    <AlertDialogFooter>
                                                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                                                        <AlertDialogAction onClick={() => handleRemoveUser(user.id)}>
                                                            Remove
                                                        </AlertDialogAction>
                                                    </AlertDialogFooter>
                                                </AlertDialogContent>
                                            </AlertDialog>
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    ) : (
                        <div className='text-center py-8 text-muted-foreground'>
                            <p>No users assigned to this company yet.</p>
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
};

export default PartnerCommunityCompanyEdit;
