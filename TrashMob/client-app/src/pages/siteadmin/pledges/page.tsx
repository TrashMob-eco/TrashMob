import { useMemo } from 'react';
import { Link } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Plus } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useToast } from '@/hooks/use-toast';
import { DeletePledge, GetContacts, GetPledges } from '@/services/contacts';
import { getColumns, type PledgeRow } from './columns';

export const SiteAdminPledges = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const { data: pledges } = useQuery({
        queryKey: GetPledges().key,
        queryFn: GetPledges().service,
        select: (res) => res.data,
    });

    const { data: contacts } = useQuery({
        queryKey: GetContacts().key,
        queryFn: GetContacts().service,
        select: (res) => res.data,
    });

    const deletePledge = useMutation({
        mutationKey: DeletePledge().key,
        mutationFn: DeletePledge().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/pledges'], refetchType: 'all' });
            toast({ variant: 'default', title: 'Pledge deleted' });
        },
    });

    const handleDelete = (id: string) => {
        if (!window.confirm('Delete this pledge?')) return;
        deletePledge.mutate({ id });
    };

    const columns = useMemo(() => getColumns({ onDelete: handleDelete }), []);

    const contactMap = useMemo(() => {
        const map = new Map<string, string>();
        (contacts || []).forEach((c) => {
            map.set(c.id, [c.firstName, c.lastName].filter(Boolean).join(' '));
        });
        return map;
    }, [contacts]);

    const rows: PledgeRow[] = useMemo(
        () =>
            (pledges || []).map((p) => ({
                id: p.id,
                contactId: p.contactId,
                contactName: contactMap.get(p.contactId) || '—',
                totalAmount: p.totalAmount,
                startDate: p.startDate,
                endDate: p.endDate,
                frequency: p.frequency,
                status: p.status,
            })),
        [pledges, contactMap],
    );

    const filterByStatus = (status: number | null) =>
        status === null ? rows : rows.filter((r) => r.status === status);

    return (
        <Tabs defaultValue='all'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div>
                        <CardTitle>Pledges</CardTitle>
                        <p className='text-sm text-muted-foreground mt-1'>
                            {(pledges || []).length} total pledges
                        </p>
                    </div>
                    <Button asChild>
                        <Link to='/siteadmin/pledges/create'>
                            <Plus /> Add Pledge
                        </Link>
                    </Button>
                </CardHeader>
                <CardContent>
                    <TabsList className='mb-4'>
                        <TabsTrigger value='all'>All</TabsTrigger>
                        <TabsTrigger value='active'>Active</TabsTrigger>
                        <TabsTrigger value='fulfilled'>Fulfilled</TabsTrigger>
                        <TabsTrigger value='lapsed'>Lapsed</TabsTrigger>
                        <TabsTrigger value='cancelled'>Cancelled</TabsTrigger>
                    </TabsList>
                    <TabsContent value='all'>
                        <DataTable
                            columns={columns}
                            data={filterByStatus(null)}
                            enableSearch
                            searchPlaceholder='Search pledges...'
                            searchColumns={['contactName']}
                        />
                    </TabsContent>
                    <TabsContent value='active'>
                        <DataTable columns={columns} data={filterByStatus(1)} />
                    </TabsContent>
                    <TabsContent value='fulfilled'>
                        <DataTable columns={columns} data={filterByStatus(2)} />
                    </TabsContent>
                    <TabsContent value='lapsed'>
                        <DataTable columns={columns} data={filterByStatus(3)} />
                    </TabsContent>
                    <TabsContent value='cancelled'>
                        <DataTable columns={columns} data={filterByStatus(4)} />
                    </TabsContent>
                </CardContent>
            </Card>
        </Tabs>
    );
};
