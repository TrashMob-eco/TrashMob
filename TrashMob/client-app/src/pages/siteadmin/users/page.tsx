import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DeleteUserById, GetAllUsers } from '@/services/users';
import { DataTable } from '@/components/ui/data-table';
import { getColumns } from './columns';

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
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetAllUsers().key,
                refetchType: 'all',
            });
        },
    });

    const removeUser = (userId: string, userName: string) => {
        if (!window.confirm(`Are you sure you want to delete user with name: ${userName}?`)) return;
        deleteUserById.mutateAsync({ id: userId });
    };

    const columns = getColumns({ onDelete: removeUser });
    const len = (users || []).length;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Users ({len})</CardTitle>
            </CardHeader>
            <CardContent>
                <DataTable columns={columns} data={users || []} />
            </CardContent>
        </Card>
    );
};
