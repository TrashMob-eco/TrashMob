import { useMemo } from 'react';
import { Link } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Plus } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useToast } from '@/hooks/use-toast';
import { DeleteDonation, GetContacts, GetDonations } from '@/services/contacts';
import { getColumns, type DonationRow } from './columns';

export const SiteAdminDonations = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();

    const { data: donations } = useQuery({
        queryKey: GetDonations().key,
        queryFn: GetDonations().service,
        select: (res) => res.data,
    });

    const { data: contacts } = useQuery({
        queryKey: GetContacts().key,
        queryFn: GetContacts().service,
        select: (res) => res.data,
    });

    const deleteDonation = useMutation({
        mutationKey: DeleteDonation().key,
        mutationFn: DeleteDonation().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/donations'], refetchType: 'all' });
            toast({ variant: 'default', title: 'Donation deleted' });
        },
    });

    const handleDelete = (id: string) => {
        if (!window.confirm('Delete this donation?')) return;
        deleteDonation.mutate({ id });
    };

    const columns = useMemo(() => getColumns({ onDelete: handleDelete }), []);

    const contactMap = useMemo(() => {
        const map = new Map<string, string>();
        (contacts || []).forEach((c) => {
            map.set(c.id, [c.firstName, c.lastName].filter(Boolean).join(' '));
        });
        return map;
    }, [contacts]);

    const rows: DonationRow[] = useMemo(
        () =>
            (donations || []).map((d) => ({
                id: d.id,
                contactId: d.contactId,
                contactName: contactMap.get(d.contactId) || '—',
                amount: d.amount,
                donationDate: d.donationDate,
                donationType: d.donationType,
                campaign: d.campaign,
                receiptSent: d.receiptSent,
                thankYouSent: d.thankYouSent,
            })),
        [donations, contactMap],
    );

    const filterByType = (type: number | null) => (type === null ? rows : rows.filter((r) => r.donationType === type));

    return (
        <Tabs defaultValue='all'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div>
                        <CardTitle>Donations</CardTitle>
                        <p className='text-sm text-muted-foreground mt-1'>{(donations || []).length} total donations</p>
                    </div>
                    <Button asChild>
                        <Link to='/siteadmin/donations/create'>
                            <Plus /> Add Donation
                        </Link>
                    </Button>
                </CardHeader>
                <CardContent>
                    <TabsList className='mb-4'>
                        <TabsTrigger value='all'>All</TabsTrigger>
                        <TabsTrigger value='cash'>Cash</TabsTrigger>
                        <TabsTrigger value='check'>Check</TabsTrigger>
                        <TabsTrigger value='credit'>Credit Card</TabsTrigger>
                        <TabsTrigger value='inkind'>In-Kind</TabsTrigger>
                        <TabsTrigger value='matching'>Matching</TabsTrigger>
                    </TabsList>
                    <TabsContent value='all'>
                        <DataTable
                            columns={columns}
                            data={filterByType(null)}
                            enableSearch
                            searchPlaceholder='Search donations...'
                            searchColumns={['contactName', 'campaign']}
                        />
                    </TabsContent>
                    <TabsContent value='cash'>
                        <DataTable columns={columns} data={filterByType(1)} />
                    </TabsContent>
                    <TabsContent value='check'>
                        <DataTable columns={columns} data={filterByType(2)} />
                    </TabsContent>
                    <TabsContent value='credit'>
                        <DataTable columns={columns} data={filterByType(3)} />
                    </TabsContent>
                    <TabsContent value='inkind'>
                        <DataTable columns={columns} data={filterByType(4)} />
                    </TabsContent>
                    <TabsContent value='matching'>
                        <DataTable columns={columns} data={filterByType(5)} />
                    </TabsContent>
                </CardContent>
            </Card>
        </Tabs>
    );
};
