import { FC, useState } from 'react';
import { useQuery, useQueries, useQueryClient, useMutation } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { format, parseISO } from 'date-fns';
import { Baby, Pencil, Trash2, Plus, Mail, RotateCw, X, ShieldCheck } from 'lucide-react';
import { AxiosResponse } from 'axios';

import { Alert, AlertTitle, AlertDescription } from '@/components/ui/alert';
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
import {
    GetDependentInvitations,
    CreateDependentInvitation,
    CancelDependentInvitation,
    ResendDependentInvitation,
    DependentInvitationData,
} from '@/services/dependent-invitations';
import { InitiateChildConsent } from '@/services/privo-consent';

interface MyDependentsCardProps {
    userId: string;
    isIdentityVerified?: boolean;
}

const relationships = ['Parent', 'Legal Guardian', 'Grandparent', 'Authorized Supervisor', 'Other'] as const;

const dependentFormSchema = z.object({
    firstName: z.string().min(1, 'First name is required'),
    lastName: z.string().min(1, 'Last name is required'),
    dateOfBirth: z.string().min(1, 'Date of birth is required'),
    relationship: z.string().min(1, 'Relationship is required'),
    medicalNotes: z.string(),
    emergencyContactPhone: z.string(),
    email: z.string().email('Must be a valid email').or(z.literal('')),
});

type DependentFormInputs = z.infer<typeof dependentFormSchema>;

export const MyDependentsCard: FC<MyDependentsCardProps> = ({ userId, isIdentityVerified = false }) => {
    const queryClient = useQueryClient();
    const { toast } = useToast();
    const [showFormDialog, setShowFormDialog] = useState(false);
    const [editingDependent, setEditingDependent] = useState<DependentData | null>(null);
    const [deletingDependent, setDeletingDependent] = useState<DependentData | null>(null);
    const [invitingDependent, setInvitingDependent] = useState<DependentData | null>(null);
    const [inviteEmail, setInviteEmail] = useState('');

    const queryConfig = GetMyDependents({ userId });
    const { data: dependents } = useQuery<AxiosResponse<DependentData[]>, unknown, DependentData[]>({
        queryKey: queryConfig.key,
        queryFn: queryConfig.service,
        select: (res) => res.data,
        enabled: !!userId && userId !== '00000000-0000-0000-0000-000000000000',
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
            email: '',
        },
    });

    const invalidateDependents = () => {
        queryClient.invalidateQueries({ queryKey: ['/users', userId, 'dependents'], refetchType: 'all' });
    };

    const getAge = (dateOfBirth: string): number => {
        const dob = new Date(dateOfBirth);
        const today = new Date();
        let age = today.getFullYear() - dob.getFullYear();
        const monthDiff = today.getMonth() - dob.getMonth();
        if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < dob.getDate())) {
            age--;
        }
        return age;
    };

    // Track invitations per dependent using useQueries (handles dynamic array safely)
    const eligibleForInvite = (dependents || []).filter((dep) => dep.dateOfBirth && getAge(dep.dateOfBirth) >= 13);

    const invitationQueryResults = useQueries({
        queries: eligibleForInvite.map((dep) => {
            const config = GetDependentInvitations({ userId, dependentId: dep.id });
            return {
                queryKey: config.key,
                queryFn: config.service,
                enabled: !!userId && userId !== '00000000-0000-0000-0000-000000000000',
                select: (res: { data: DependentInvitationData[] }) => res.data,
            };
        }),
    });

    const invitationResults = new Map<string, DependentInvitationData[]>();
    eligibleForInvite.forEach((dep, i) => {
        const data = invitationQueryResults[i]?.data;
        if (data) {
            invitationResults.set(dep.id, data);
        }
    });

    const getActiveInvitation = (dependentId: string): DependentInvitationData | undefined => {
        const invitations = invitationResults.get(dependentId);
        if (!invitations) return undefined;
        // Status 2 = Sent (active), Status 3 = Accepted
        return invitations.find((i) => i.invitationStatusId === 2 || i.invitationStatusId === 3);
    };

    const createInviteMutation = useMutation({
        mutationFn: async () => {
            if (!invitingDependent) return;
            return CreateDependentInvitation({ userId, dependentId: invitingDependent.id }).service({
                email: inviteEmail,
            });
        },
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Invitation sent!' });
            queryClient.invalidateQueries({ queryKey: ['/dependentinvitations'] });
            setInvitingDependent(null);
            setInviteEmail('');
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to send invitation.' });
        },
    });

    const cancelInviteMutation = useMutation({
        mutationFn: async (invitationId: string) => {
            return CancelDependentInvitation({ invitationId }).service();
        },
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Invitation canceled.' });
            queryClient.invalidateQueries({ queryKey: ['/dependentinvitations'] });
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to cancel invitation.' });
        },
    });

    const resendInviteMutation = useMutation({
        mutationFn: async (invitationId: string) => {
            return ResendDependentInvitation({ invitationId }).service();
        },
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Invitation resent!' });
            queryClient.invalidateQueries({ queryKey: ['/dependentinvitations'] });
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to resend invitation.' });
        },
    });

    const startConsentMutation = useMutation({
        mutationFn: async (dependentId: string) => {
            return InitiateChildConsent({ dependentId }).service();
        },
        onSuccess: (res) => {
            const consent = res.data;
            if (consent.consentUrl) {
                window.location.href = consent.consentUrl;
            } else {
                toast({
                    variant: 'primary',
                    title: 'Consent request sent',
                    description: 'Check your email for a consent link from PRIVO.',
                });
                invalidateDependents();
            }
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Failed to start consent', description: error.message });
        },
    });

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
            email: dependent.email || '',
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
    const eligibleDependents = (dependents || []).filter(
        (dep) => dep.dateOfBirth && getAge(dep.dateOfBirth) >= 13 && !getActiveInvitation(dep.id),
    );
    const isSubmitting = addMutation.isPending || updateMutation.isPending;

    return (
        <>
            <Card className='mb-4'>
                <CardHeader>
                    <div className='flex flex-row items-center'>
                        <Baby className='inline-block h-5 w-5 mr-2 text-primary' />
                        <CardTitle className='grow text-primary'>My Dependents ({dependentCount})</CardTitle>
                        <Button
                            variant='outline'
                            size='sm'
                            onClick={openAddDialog}
                            disabled={!isIdentityVerified}
                            title={
                                isIdentityVerified ? 'Add a dependent' : 'Verify your identity first to add dependents'
                            }
                        >
                            <Plus className='h-4 w-4 mr-1' />
                            Add Dependent
                        </Button>
                    </div>
                </CardHeader>
                <CardContent>
                    {eligibleDependents.length > 0 && (
                        <Alert className='mb-4'>
                            <Mail className='h-4 w-4' />
                            <AlertTitle>Invitations Available</AlertTitle>
                            <AlertDescription>
                                {eligibleDependents.length === 1
                                    ? `${eligibleDependents[0].firstName} is old enough (13+) to create their own TrashMob account.`
                                    : `${eligibleDependents.length} of your dependents are old enough (13+) to create their own TrashMob accounts.`}{' '}
                                {isIdentityVerified ? (
                                    <>
                                        Use the <ShieldCheck className='inline h-3 w-3' /> button to start PRIVO
                                        parental consent, then <Mail className='inline h-3 w-3' /> to send an
                                        invitation.
                                    </>
                                ) : (
                                    <>
                                        You must{' '}
                                        <a href='/myprofile' className='underline font-medium'>
                                            verify your identity
                                        </a>{' '}
                                        first before adding dependents aged 13-17.
                                    </>
                                )}
                            </AlertDescription>
                        </Alert>
                    )}
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
                                        <th className='text-left py-2 px-2 font-medium'>Account</th>
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
                                            <td className='py-3 px-2'>
                                                {(() => {
                                                    const activeInvite = getActiveInvitation(dep.id);
                                                    if (activeInvite?.invitationStatusId === 3) {
                                                        return <Badge variant='default'>Account Created</Badge>;
                                                    }
                                                    if (activeInvite?.invitationStatusId === 2) {
                                                        return (
                                                            <div className='flex items-center gap-1'>
                                                                <Badge variant='outline'>Invited</Badge>
                                                                <Button
                                                                    variant='ghost'
                                                                    size='sm'
                                                                    onClick={() =>
                                                                        resendInviteMutation.mutate(activeInvite.id)
                                                                    }
                                                                    title='Resend invitation'
                                                                    disabled={resendInviteMutation.isPending}
                                                                >
                                                                    <RotateCw className='h-3 w-3' />
                                                                </Button>
                                                                <Button
                                                                    variant='ghost'
                                                                    size='sm'
                                                                    onClick={() =>
                                                                        cancelInviteMutation.mutate(activeInvite.id)
                                                                    }
                                                                    title='Cancel invitation'
                                                                    disabled={cancelInviteMutation.isPending}
                                                                >
                                                                    <X className='h-3 w-3 text-destructive' />
                                                                </Button>
                                                            </div>
                                                        );
                                                    }
                                                    if (dep.dateOfBirth && getAge(dep.dateOfBirth) >= 13) {
                                                        if (dep.privoConsentStatus === 1) {
                                                            return (
                                                                <Badge variant='outline' className='text-xs'>
                                                                    Consent Pending
                                                                </Badge>
                                                            );
                                                        }
                                                        if (dep.privoConsentStatus === 2) {
                                                            return (
                                                                <Badge variant='secondary' className='text-xs'>
                                                                    Consent Approved
                                                                </Badge>
                                                            );
                                                        }
                                                        return (
                                                            <Badge variant='secondary' className='text-xs'>
                                                                Needs Consent
                                                            </Badge>
                                                        );
                                                    }
                                                    return <span className='text-xs text-muted-foreground'>—</span>;
                                                })()}
                                            </td>
                                            <td className='py-3 px-2 text-right'>
                                                {dep.dateOfBirth &&
                                                getAge(dep.dateOfBirth) >= 13 &&
                                                !getActiveInvitation(dep.id) &&
                                                isIdentityVerified ? (
                                                    dep.privoConsentStatus === 2 ? (
                                                        <Button
                                                            variant='ghost'
                                                            size='sm'
                                                            onClick={() => {
                                                                setInvitingDependent(dep);
                                                                setInviteEmail('');
                                                            }}
                                                            title='Invite to create account'
                                                        >
                                                            <Mail className='h-4 w-4 text-primary' />
                                                        </Button>
                                                    ) : dep.privoConsentStatus !== 1 ? (
                                                        <Button
                                                            variant='ghost'
                                                            size='sm'
                                                            onClick={() => startConsentMutation.mutate(dep.id)}
                                                            disabled={startConsentMutation.isPending}
                                                            title='Start parental consent via PRIVO'
                                                        >
                                                            <ShieldCheck className='h-4 w-4 text-amber-600' />
                                                        </Button>
                                                    ) : null
                                                ) : null}
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
                                name='email'
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Email (optional)</FormLabel>
                                        <FormControl>
                                            <Input type='email' placeholder='child@example.com' {...field} />
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

            {/* Invite Dialog */}
            <Dialog
                open={!!invitingDependent}
                onOpenChange={(open) => {
                    if (!open) {
                        setInvitingDependent(null);
                        setInviteEmail('');
                    }
                }}
            >
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>Invite to Create Account</DialogTitle>
                        <DialogDescription>
                            Send {invitingDependent?.firstName} an email invitation to create their own TrashMob
                            account. They must be 13 or older.
                        </DialogDescription>
                    </DialogHeader>
                    <div className='space-y-4'>
                        <div>
                            <label htmlFor='invite-email' className='text-sm font-medium'>
                                Email Address
                            </label>
                            <Input
                                id='invite-email'
                                type='email'
                                placeholder="Enter dependent's email"
                                value={inviteEmail}
                                onChange={(e) => setInviteEmail(e.target.value)}
                            />
                            <p className='text-xs text-muted-foreground mt-1'>
                                The invitation will be sent to this email. The link expires in 30 days.
                            </p>
                        </div>
                    </div>
                    <DialogFooter>
                        <Button
                            variant='outline'
                            onClick={() => {
                                setInvitingDependent(null);
                                setInviteEmail('');
                            }}
                        >
                            Cancel
                        </Button>
                        <Button
                            onClick={() => createInviteMutation.mutate()}
                            disabled={!inviteEmail || createInviteMutation.isPending}
                        >
                            <Mail className='h-4 w-4 mr-1' />
                            {createInviteMutation.isPending ? 'Sending...' : 'Send Invitation'}
                        </Button>
                    </DialogFooter>
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
