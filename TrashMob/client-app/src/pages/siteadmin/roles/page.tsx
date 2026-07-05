import { useQuery } from '@tanstack/react-query';
import { Users } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { GetAllRoles, RoleDto } from '@/services/roles';

export const SiteAdminRoles = () => {
    const { data: roles, isLoading } = useQuery({
        queryKey: GetAllRoles().key,
        queryFn: GetAllRoles().service,
        select: (res) => res.data,
    });

    return (
        <Card>
            <CardHeader>
                <CardTitle>Roles</CardTitle>
                <CardDescription>
                    Roles are seeded from code. To grant or revoke a role, open a user's detail page from the Users
                    list.
                </CardDescription>
            </CardHeader>
            <CardContent>
                {isLoading ? (
                    <p className='text-sm text-muted-foreground'>Loading roles…</p>
                ) : (
                    <ul className='divide-y'>
                        {(roles || []).map((role: RoleDto) => (
                            <li key={role.id} className='py-3'>
                                <div className='flex items-center justify-between'>
                                    <div>
                                        <p className='font-medium'>{role.name}</p>
                                        {role.description ? (
                                            <p className='text-sm text-muted-foreground'>{role.description}</p>
                                        ) : null}
                                    </div>
                                    <div className='flex items-center gap-2 text-sm text-muted-foreground'>
                                        <Users className='h-4 w-4' />
                                        <span>
                                            {role.memberCount} {role.memberCount === 1 ? 'member' : 'members'}
                                        </span>
                                    </div>
                                </div>
                            </li>
                        ))}
                    </ul>
                )}
            </CardContent>
        </Card>
    );
};
