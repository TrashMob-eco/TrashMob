import { useState } from 'react';
import { Link, useParams } from 'react-router';
import moment from 'moment';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { ArrowLeft, ShieldPlus, X } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Badge } from '@/components/ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useToast } from '@/hooks/use-toast';
import { getErrorMessage } from '@/lib/api-errors';
import { GetUserById } from '@/services/users';
import { GetAllRoles, GetUserRoles, GrantUserRole, RevokeUserRole } from '@/services/roles';

/**
 * Per-user role management (Project 64 Phase 3). Shows the user's active role
 * grants plus a form to grant a new role (with optional expiry) and a revoke
 * action per row.
 *
 * Route: /siteadmin/users/:userId/roles
 * SiteAdmin-only — enforced by the backend RolesV2Controller.
 */
export const SiteAdminUserRoles = () => {
    const { userId } = useParams<{ userId: string }>() as { userId: string };
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const [selectedRoleName, setSelectedRoleName] = useState<string>('');
    const [expiryDate, setExpiryDate] = useState<string>('');

    const { data: user } = useQuery({
        queryKey: GetUserById({ userId }).key,
        queryFn: GetUserById({ userId }).service,
        select: (res) => res.data,
        enabled: !!userId,
    });

    const { data: allRoles } = useQuery({
        queryKey: GetAllRoles().key,
        queryFn: GetAllRoles().service,
        select: (res) => res.data,
    });

    const { data: grants } = useQuery({
        queryKey: GetUserRoles({ userId }).key,
        queryFn: GetUserRoles({ userId }).service,
        select: (res) => res.data,
        enabled: !!userId,
    });

    const grantMutation = useMutation({
        mutationKey: GrantUserRole({ userId }).key,
        mutationFn: GrantUserRole({ userId }).service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Role granted' });
            queryClient.invalidateQueries({ queryKey: GetUserRoles({ userId }).key });
            queryClient.invalidateQueries({ queryKey: GetAllRoles().key });
            setSelectedRoleName('');
            setExpiryDate('');
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Grant failed', description: getErrorMessage(error) });
        },
    });

    const revokeMutation = useMutation({
        mutationKey: RevokeUserRole().key,
        mutationFn: RevokeUserRole().service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Role revoked' });
            queryClient.invalidateQueries({ queryKey: GetUserRoles({ userId }).key });
            queryClient.invalidateQueries({ queryKey: GetAllRoles().key });
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Revoke failed', description: getErrorMessage(error) });
        },
    });

    const grantedRoleNames = new Set((grants || []).map((g) => g.roleName));
    const grantableRoles = (allRoles || []).filter((r) => r.isActive !== false && !grantedRoleNames.has(r.name));

    const submitGrant = () => {
        if (!selectedRoleName) return;
        if (!window.confirm(`Grant the "${selectedRoleName}" role to ${user?.userName || 'this user'}?`)) {
            return;
        }
        grantMutation.mutate({
            roleName: selectedRoleName,
            expiryDate: expiryDate ? new Date(expiryDate).toISOString() : null,
        });
    };

    const submitRevoke = (roleName: string) => {
        const reason = window.prompt(
            `Reason for revoking "${roleName}" from ${user?.userName || 'this user'}? (optional)`,
        );
        // A null prompt result means Cancel — bail out. Empty string means "no reason".
        if (reason === null) return;
        revokeMutation.mutate({ userId, roleName, reason: reason || undefined });
    };

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div className='flex items-center gap-3'>
                        <Button variant='ghost' size='icon' asChild>
                            <Link to={`/siteadmin/users/${userId}`}>
                                <ArrowLeft className='h-4 w-4' />
                            </Link>
                        </Button>
                        <div>
                            <CardTitle>Roles for {user?.userName || '…'}</CardTitle>
                            <CardDescription>
                                Grants and revocations trigger an email to the affected user. Changes take effect on the
                                user's next request.
                            </CardDescription>
                        </div>
                    </div>
                </CardHeader>
                <CardContent>
                    <h3 className='mb-3 text-sm font-medium text-muted-foreground'>Active grants</h3>
                    {(grants || []).length === 0 ? (
                        <p className='text-sm text-muted-foreground'>No roles currently granted.</p>
                    ) : (
                        <ul className='divide-y'>
                            {(grants || []).map((grant) => (
                                <li key={grant.id} className='flex items-center justify-between py-3'>
                                    <div>
                                        <div className='flex items-center gap-2'>
                                            <Badge>{grant.roleName}</Badge>
                                            {grant.expiryDate ? (
                                                <span className='text-xs text-muted-foreground'>
                                                    expires {moment(grant.expiryDate).format('MMM D, YYYY')}
                                                </span>
                                            ) : null}
                                        </div>
                                        <p className='mt-1 text-xs text-muted-foreground'>
                                            Granted {moment(grant.grantedDate).format('MMM D, YYYY')}
                                        </p>
                                    </div>
                                    <Button
                                        variant='ghost'
                                        size='sm'
                                        onClick={() => submitRevoke(grant.roleName)}
                                        disabled={revokeMutation.isPending}
                                    >
                                        <X className='mr-1 h-4 w-4' />
                                        Revoke
                                    </Button>
                                </li>
                            ))}
                        </ul>
                    )}
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Grant a role</CardTitle>
                    <CardDescription>Only seeded roles the user does not already hold are listed.</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='grid grid-cols-1 gap-4 md:grid-cols-3'>
                        <div>
                            <Label htmlFor='role-select'>Role</Label>
                            <Select value={selectedRoleName} onValueChange={setSelectedRoleName}>
                                <SelectTrigger id='role-select'>
                                    <SelectValue placeholder='Select a role…' />
                                </SelectTrigger>
                                <SelectContent>
                                    {grantableRoles.map((role) => (
                                        <SelectItem key={role.id} value={role.name}>
                                            {role.name}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                        </div>
                        <div>
                            <Label htmlFor='expiry-date'>Expiry date (optional)</Label>
                            <Input
                                id='expiry-date'
                                type='date'
                                value={expiryDate}
                                onChange={(e) => setExpiryDate(e.target.value)}
                            />
                        </div>
                        <div className='flex items-end'>
                            <Button onClick={submitGrant} disabled={!selectedRoleName || grantMutation.isPending}>
                                <ShieldPlus className='mr-1 h-4 w-4' />
                                Grant role
                            </Button>
                        </div>
                    </div>
                </CardContent>
            </Card>
        </div>
    );
};
