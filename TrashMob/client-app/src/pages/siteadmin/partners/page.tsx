import { useMutation, useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { getColumns } from './columns';
import { DeletePartnerById, GetPartners } from '@/services/partners';
import { useToast } from '@/hooks/use-toast';

export const SiteAdminPartners = () => {
    const { toast } = useToast();
    const { data: partners } = useQuery({
        queryKey: GetPartners().key,
        queryFn: GetPartners().service,
        select: (res) => res.data,
    });

    const deletePartnerById = useMutation({
        mutationKey: DeletePartnerById().key,
        mutationFn: DeletePartnerById().service,
        onSuccess: () => {
            toast({
                variant: 'default',
                title: 'Partner deleted',
            });
        },
    });

    function handleDelete(id: string, name: string) {
        if (!window.confirm(`Are you sure you want to delete partner with name: ${name}`)) return;
        deletePartnerById.mutate({ id });
    }

    const columns = getColumns({ onDelete: handleDelete });

    const len = (partners || []).length;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Partners ({len})</CardTitle>
            </CardHeader>
            <CardContent>
                <DataTable columns={columns} data={partners || []} />
            </CardContent>
        </Card>
    );
};
