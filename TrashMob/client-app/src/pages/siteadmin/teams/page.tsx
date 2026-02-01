import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { getColumns } from './columns';
import { DeleteTeam, GetAllTeams, ReactivateTeam } from '@/services/teams';
import { useToast } from '@/hooks/use-toast';

export const SiteAdminTeams = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const { data: teams } = useQuery({
        queryKey: GetAllTeams().key,
        queryFn: GetAllTeams().service,
        select: (res) => res.data,
    });

    const deleteTeam = useMutation({
        mutationKey: DeleteTeam().key,
        mutationFn: DeleteTeam().service,
        onSuccess: () => {
            toast({
                variant: 'default',
                title: 'Team deleted',
            });
            queryClient.invalidateQueries({ queryKey: GetAllTeams().key });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Failed to delete team',
            });
        },
    });

    const reactivateTeam = useMutation({
        mutationKey: ReactivateTeam().key,
        mutationFn: ReactivateTeam().service,
        onSuccess: () => {
            toast({
                variant: 'default',
                title: 'Team reactivated',
            });
            queryClient.invalidateQueries({ queryKey: GetAllTeams().key });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Failed to reactivate team',
            });
        },
    });

    function handleDelete(id: string, name: string) {
        if (!window.confirm(`Are you sure you want to delete team: ${name}?`)) return;
        deleteTeam.mutate({ teamId: id });
    }

    function handleReactivate(id: string, name: string) {
        if (!window.confirm(`Are you sure you want to reactivate team: ${name}?`)) return;
        reactivateTeam.mutate({ teamId: id });
    }

    const columns = getColumns({ onDelete: handleDelete, onReactivate: handleReactivate });

    const len = (teams || []).length;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Teams ({len})</CardTitle>
            </CardHeader>
            <CardContent>
                <DataTable columns={columns} data={teams || []} />
            </CardContent>
        </Card>
    );
};
