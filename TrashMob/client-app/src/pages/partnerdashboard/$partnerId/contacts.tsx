import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import PhoneInput from 'react-phone-input-2';
import { DeletePartnerContactByContactId, GetPartnerContactsByPartnerId } from '@/services/contact';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Ellipsis, Pencil, SquareX, Plus } from 'lucide-react';
import { Link, Outlet, useMatch, useNavigate, useParams } from 'react-router';
import { SidebarLayout } from './_layout.sidebar';
import { useState } from 'react';

const useGetPartnerContactsByPartnerId = (partnerId: string) => {
    return useQuery({
        queryKey: GetPartnerContactsByPartnerId({ partnerId }).key,
        queryFn: GetPartnerContactsByPartnerId({ partnerId }).service,
        select: (res) => res.data,
    });
};

const useDeletePartnerContactByContactId = () => {
    return useMutation({
        mutationKey: DeletePartnerContactByContactId().key,
        mutationFn: DeletePartnerContactByContactId().service,
    });
};

export const PartnerContacts = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const isEdit = useMatch(`/partnerdashboard/:partnerId/contacts/:contactId/edit`);
    const isCreate = useMatch(`/partnerdashboard/:partnerId/contacts/create`);

    const { data: rows } = useGetPartnerContactsByPartnerId(partnerId);

    const [isDeletingId, setIsDeletingId] = useState<string | null>(null);
    const deletePartnerContactByContactId = useDeletePartnerContactByContactId();

    function removeContact(partnerContactId: string, name: string) {
        if (!window.confirm(`Please confirm that you want to remove contact: '${name}' from this Partner ?`)) return;
        setIsDeletingId(partnerContactId);

        deletePartnerContactByContactId
            .mutateAsync({ contactId: partnerContactId })
            .then(async () => {
                return queryClient.invalidateQueries({
                    queryKey: GetPartnerContactsByPartnerId({ partnerId }).key,
                    refetchType: 'all',
                });
            })
            .then(() => {
                setIsDeletingId(null);
            });
    }

    return (
        <SidebarLayout
            title='Edit Partner Contacts'
            description='This page allows you to add more contacts to this partner so you can share the load of responding to questions for this partner. This information may be displayed in the partnerships page on TrashMob.eco, but is also used by the TrashMob site administrators to contact your organization during setup and during times where issues have arisen.'
        >
            <div>
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Name</TableHead>
                            <TableHead>Email</TableHead>
                            <TableHead>Phone</TableHead>
                            <TableHead>Actions</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(rows || []).map((row) => (
                            <TableRow key={row.id} className={isDeletingId === row.id ? 'opacity-20' : ''}>
                                <TableCell>{row.name}</TableCell>
                                <TableCell>{row.email}</TableCell>
                                <TableCell>
                                    <PhoneInput value={row.phone} disabled />
                                </TableCell>
                                <TableCell>
                                    <DropdownMenu>
                                        <DropdownMenuTrigger asChild>
                                            <Button variant='ghost' size='icon'>
                                                <Ellipsis />
                                            </Button>
                                        </DropdownMenuTrigger>
                                        <DropdownMenuContent className='w-56'>
                                            <DropdownMenuItem asChild>
                                                <Link to={`${row.id}/edit`}>
                                                    <Pencil />
                                                    Edit Contact
                                                </Link>
                                            </DropdownMenuItem>
                                            <DropdownMenuItem onClick={() => removeContact(row.id, row.name)}>
                                                <SquareX />
                                                Remove Contact
                                            </DropdownMenuItem>
                                        </DropdownMenuContent>
                                    </DropdownMenu>
                                </TableCell>
                            </TableRow>
                        ))}
                        <TableRow>
                            <TableCell colSpan={4}>
                                <Button variant='ghost' className='w-full' asChild>
                                    <Link to='create'>
                                        <Plus /> Add Contact
                                    </Link>
                                </Button>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
                <Dialog open={!!isEdit} onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/contacts`)}>
                    <DialogContent className='sm:max-w-[600px]' onOpenAutoFocus={(e) => e.preventDefault()}>
                        <DialogHeader>
                            <DialogTitle>Edit Contact</DialogTitle>
                        </DialogHeader>
                        <div>
                            <Outlet />
                        </div>
                    </DialogContent>
                </Dialog>
                <Dialog open={!!isCreate} onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/contacts`)}>
                    <DialogContent className='sm:max-w-[600px]' onOpenAutoFocus={(e) => e.preventDefault()}>
                        <DialogHeader>
                            <DialogTitle>Create Contact</DialogTitle>
                        </DialogHeader>
                        <div>
                            <Outlet />
                        </div>
                    </DialogContent>
                </Dialog>
            </div>
        </SidebarLayout>
    );
};
