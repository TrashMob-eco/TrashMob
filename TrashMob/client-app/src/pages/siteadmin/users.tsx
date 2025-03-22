import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Ellipsis, SquareX } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DeleteUserById, GetAllUsers } from '@/services/users';

export const SiteAdminUsers = () => {
    const queryClient = useQueryClient();

    const { data: users } = useQuery({
        queryKey: GetAllUsers().key,
        queryFn: GetAllUsers().service,
        select: (res) => res.data,
    });

    const deleteUserById = useMutation({
        mutationKey: DeleteUserById().key,
        mutationFn: DeleteUserById().service,
    });

    const [isDeletingId, setIsDeletingId] = useState<string | null>(null);
    const removeUser = (userId: string, userName: string) => {
        setIsDeletingId(userId);
        if (!window.confirm(`Are you sure you want to delete user with name: ${userName}?`)) return;

        deleteUserById
            .mutateAsync({ id: userId })
            .then(async () => {
                return queryClient.invalidateQueries({
                    queryKey: GetAllUsers().key,
                    refetchType: 'all',
                });
            })
            .then(() => {
                setIsDeletingId(null);
            });
    };

    return (
        <Card>
            <CardHeader>
                <CardTitle>Users</CardTitle>
            </CardHeader>
            <CardContent>
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>UserName</TableHead>
                            <TableHead>Email</TableHead>
                            <TableHead>City</TableHead>
                            <TableHead>Region</TableHead>
                            <TableHead>Country</TableHead>
                            <TableHead>Postal Code</TableHead>
                            <TableHead>Actions</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(users || []).map((user) => (
                            <TableRow key={user.id} className={isDeletingId === user.id ? 'opacity-20' : ''}>
                                <TableCell>{user.userName}</TableCell>
                                <TableCell>{user.email}</TableCell>
                                <TableCell>{user.city}</TableCell>
                                <TableCell>{user.region}</TableCell>
                                <TableCell>{user.country}</TableCell>
                                <TableCell>{user.postalCode}</TableCell>
                                <TableCell>
                                    <DropdownMenu>
                                        <DropdownMenuTrigger asChild>
                                            <Button variant='ghost' size='icon'>
                                                <Ellipsis />
                                            </Button>
                                        </DropdownMenuTrigger>
                                        <DropdownMenuContent className='w-56'>
                                            <DropdownMenuItem onClick={() => removeUser(user.id, user.userName)}>
                                                <SquareX />
                                                Delete User
                                            </DropdownMenuItem>
                                        </DropdownMenuContent>
                                    </DropdownMenu>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </CardContent>
        </Card>
    );
};
