import { useState } from 'react';
import { Link } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Plus } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useToast } from '@/hooks/use-toast';
import { CONTACT_TYPES } from '@/components/contacts/contact-constants';
import { DeleteContact, GetContacts } from '@/services/contacts';
import { getColumns } from './columns';

export const SiteAdminContacts = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [typeFilter, setTypeFilter] = useState<number | undefined>(undefined);

    const queryParams = typeFilter !== undefined ? { contactType: typeFilter } : undefined;

    const { data: contacts } = useQuery({
        queryKey: GetContacts(queryParams).key,
        queryFn: GetContacts(queryParams).service,
        select: (res) => res.data,
    });

    const deleteContact = useMutation({
        mutationKey: DeleteContact().key,
        mutationFn: DeleteContact().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/contacts'], refetchType: 'all' });
            toast({ variant: 'default', title: 'Contact deleted' });
        },
    });

    const handleDelete = (id: string, name: string) => {
        if (!window.confirm(`Are you sure you want to delete "${name}"?`)) return;
        deleteContact.mutate({ id });
    };

    const columns = getColumns({ onDelete: handleDelete });

    return (
        <Card>
            <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle>Contacts ({(contacts || []).length})</CardTitle>
                <Button asChild>
                    <Link to='/siteadmin/contacts/create'>
                        <Plus /> Add Contact
                    </Link>
                </Button>
            </CardHeader>
            <CardContent className='space-y-4'>
                <Tabs
                    value={typeFilter?.toString() ?? 'all'}
                    onValueChange={(v) => setTypeFilter(v === 'all' ? undefined : parseInt(v))}
                >
                    <TabsList>
                        <TabsTrigger value='all'>All</TabsTrigger>
                        {CONTACT_TYPES.map((t) => (
                            <TabsTrigger key={t.value} value={t.value.toString()}>
                                {t.label}
                            </TabsTrigger>
                        ))}
                    </TabsList>
                </Tabs>
                <DataTable
                    columns={columns}
                    data={contacts || []}
                    enableSearch
                    searchPlaceholder='Search contacts...'
                    searchColumns={['lastName', 'email', 'organizationName']}
                />
            </CardContent>
        </Card>
    );
};
