import { useMemo } from 'react';
import { Link } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Plus } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useToast } from '@/hooks/use-toast';
import { DeleteGrant, GetGrants } from '@/services/grants';
import { getColumns, type GrantRow } from './columns';

export const SiteAdminGrants = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const { data: grants } = useQuery({
        queryKey: GetGrants().key,
        queryFn: GetGrants().service,
        select: (res) => res.data,
    });

    const deleteGrant = useMutation({
        mutationKey: DeleteGrant().key,
        mutationFn: DeleteGrant().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/grants'], refetchType: 'all' });
            toast({ variant: 'default', title: 'Grant deleted' });
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Failed to delete grant. Please try again.' });
        },
    });

    const handleDelete = (id: string) => {
        if (!window.confirm('Delete this grant?')) return;
        deleteGrant.mutate({ id });
    };

    const columns = useMemo(() => getColumns({ onDelete: handleDelete }), []);

    const rows: GrantRow[] = useMemo(
        () =>
            (grants || []).map((g) => ({
                id: g.id,
                funderName: g.funderName,
                programName: g.programName,
                status: g.status,
                submissionDeadline: g.submissionDeadline,
                amountMin: g.amountMin,
                amountMax: g.amountMax,
                amountAwarded: g.amountAwarded,
            })),
        [grants],
    );

    const filterByStatus = (statuses: number[] | null) =>
        statuses === null ? rows : rows.filter((r) => statuses.includes(r.status));

    return (
        <Tabs defaultValue='all'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div>
                        <CardTitle>Grants</CardTitle>
                        <p className='text-sm text-muted-foreground mt-1'>{(grants || []).length} total grants</p>
                    </div>
                    <Button asChild>
                        <Link to='/siteadmin/grants/create'>
                            <Plus /> Add Grant
                        </Link>
                    </Button>
                </CardHeader>
                <CardContent>
                    <TabsList className='mb-4'>
                        <TabsTrigger value='all'>All</TabsTrigger>
                        <TabsTrigger value='prospect'>Prospect</TabsTrigger>
                        <TabsTrigger value='inprogress'>In Progress</TabsTrigger>
                        <TabsTrigger value='awarded'>Awarded</TabsTrigger>
                        <TabsTrigger value='reporting'>Reporting</TabsTrigger>
                        <TabsTrigger value='closed'>Closed/Declined</TabsTrigger>
                    </TabsList>
                    <TabsContent value='all'>
                        <DataTable
                            columns={columns}
                            data={filterByStatus(null)}
                            enableSearch
                            searchPlaceholder='Search grants...'
                            searchColumns={['funderName', 'programName']}
                        />
                    </TabsContent>
                    <TabsContent value='prospect'>
                        <DataTable columns={columns} data={filterByStatus([1])} />
                    </TabsContent>
                    <TabsContent value='inprogress'>
                        <DataTable columns={columns} data={filterByStatus([2, 3])} />
                    </TabsContent>
                    <TabsContent value='awarded'>
                        <DataTable columns={columns} data={filterByStatus([4])} />
                    </TabsContent>
                    <TabsContent value='reporting'>
                        <DataTable columns={columns} data={filterByStatus([6, 7])} />
                    </TabsContent>
                    <TabsContent value='closed'>
                        <DataTable columns={columns} data={filterByStatus([5, 8])} />
                    </TabsContent>
                </CardContent>
            </Card>
        </Tabs>
    );
};
