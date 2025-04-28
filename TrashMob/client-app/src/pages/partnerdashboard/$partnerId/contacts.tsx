import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Card, CardHeader, CardContent, CardTitle, CardDescription } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import {
    DeletePartnerContactByContactId,
    DeletePartnerLocationContactByContactId,
    GetPartnerContactsByPartnerId,
    GetPartnerLocationContactsByLocationId,
} from '@/services/contact';
import { useMutation, useQueries, useQuery, useQueryClient } from '@tanstack/react-query';
import { Ellipsis, Pencil, SquareX, Plus } from 'lucide-react';
import { Link, Outlet, useMatch, useNavigate, useParams } from 'react-router';
import { SidebarLayout } from '../../layouts/_layout.sidebar';
import { useCallback, useState } from 'react';
import { useGetPartnerLocations } from '@/hooks/useGetPartnerLocations';
import PartnerLocationContactData from '@/components/Models/PartnerLocationContactData';
import { Badge } from '@/components/ui/badge';
import { PartnerContactType } from '@/enums/PartnerContactType';
import { getIndexedColor } from '@/lib/color';

const formatPhone = (phone: string) => {
    function splitIntoChunks(str: string, size: number): string[] {
        return str.match(new RegExp(`.{1,${size}}`, 'g')) || [];
    }
    return splitIntoChunks(`+${phone}`, 3).join(' ');
};

const useGetOrganizationWideContacts = (partnerId: string) => {
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

const useDeletePartnerLocationContactByContactId = () => {
    return useMutation({
        mutationKey: DeletePartnerLocationContactByContactId().key,
        mutationFn: DeletePartnerLocationContactByContactId().service,
    });
};

export const PartnerContacts = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const isEdit = useMatch(`/partnerdashboard/:partnerId/contacts/:type?/:contactId/edit`);
    const isCreate = useMatch(`/partnerdashboard/:partnerId/contacts/create`);

    const { data: locations } = useGetPartnerLocations({ partnerId });
    const { data: organizationWideContacts } = useGetOrganizationWideContacts(partnerId);

    const getLocation = useCallback(
        (locationId: string) => {
            return (locations || []).find((loc) => loc.id === locationId);
        },
        [locations],
    );

    const contactsByLocation = useQueries({
        queries: (locations || []).map((location, locIndex) => {
            const badgeColor = getIndexedColor(locIndex);
            return {
                queryKey: GetPartnerLocationContactsByLocationId({ locationId: location.id }).key,
                queryFn: GetPartnerLocationContactsByLocationId({ locationId: location.id }).service,
                select: (res) => (res.data || []).map((item) => ({ ...item, badgeColor })),
            };
        }),
    });

    const contacts = contactsByLocation.map((loc) => loc.data || []).flat();

    const [isDeletingId, setIsDeletingId] = useState<string | null>(null);
    const deletePartnerContact = useDeletePartnerContactByContactId();
    const deletePartnerLocationContact = useDeletePartnerLocationContactByContactId();

    function removeContact(partnerContactId: string, name: string) {
        if (!window.confirm(`Please confirm that you want to remove contact: '${name}' from this Partner ?`)) return;
        setIsDeletingId(partnerContactId);

        deletePartnerContact
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

    function removeLocationContact(partnerContactId: string, name: string, locationId: string, locationName: string) {
        if (!window.confirm(`Please confirm that you want to remove contact: '${name}' from ${locationName}?`)) return;
        setIsDeletingId(partnerContactId);

        deletePartnerLocationContact
            .mutateAsync({ contactId: partnerContactId })
            .then(async () => {
                return queryClient.invalidateQueries({
                    queryKey: GetPartnerLocationContactsByLocationId({ locationId }).key,
                    refetchType: 'all',
                });
            })
            .then(() => {
                setIsDeletingId(null);
            });
    }

    return (
        <SidebarLayout
            description='This page allows you to add more contacts to this partner so you can share the load of responding to questions for this partner.'
            title='Edit Partner Contacts'
            useDefaultCard={false}
        >
            <div className='space-y-4'>
                <Card>
                    <CardHeader>
                        <CardTitle>Organization-wide Contacts</CardTitle>
                        <CardDescription>
                            This information may be displayed in the partnerships page on TrashMob.eco, but is also used
                            by the TrashMob site administrators to contact your organization during setup and during
                            times where issues have arisen.
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
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
                                {(organizationWideContacts || []).map((row) => (
                                    <TableRow className={isDeletingId === row?.id ? 'opacity-20' : ''} key={row.name}>
                                        <TableCell>{row.name}</TableCell>
                                        <TableCell>{row.email}</TableCell>
                                        <TableCell>{formatPhone(row.phone)}</TableCell>
                                        <TableCell>
                                            <DropdownMenu>
                                                <DropdownMenuTrigger asChild>
                                                    <Button size='icon' variant='ghost'>
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
                                        <Button asChild className='w-full' variant='ghost'>
                                            <Link to='create'>
                                                <Plus /> Add Organization Contact
                                            </Link>
                                        </Button>
                                    </TableCell>
                                </TableRow>
                            </TableBody>
                        </Table>
                    </CardContent>
                </Card>
                <Card>
                    <CardHeader>
                        <CardTitle>Location-specific Contacts</CardTitle>
                        <CardDescription>
                            These are contacts for a particular location of your organization. These addresses will be
                            sent emails when a TrashMob.eco user chooses to use the services offered by this location.
                            This will allow you to accept or decline the request so that the user knows the status of
                            their requests.
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Location</TableHead>
                                    <TableHead>Name</TableHead>
                                    <TableHead>Email</TableHead>
                                    <TableHead>Phone</TableHead>
                                    <TableHead>Actions</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {(contacts || []).map((row) => {
                                    const location = getLocation(row.partnerLocationId);
                                    return (
                                        <TableRow key={row.id}>
                                            <TableCell>
                                                <Badge className={row.badgeColor}>{location?.name}</Badge>
                                            </TableCell>
                                            <TableCell>{row.name}</TableCell>
                                            <TableCell>{row.email}</TableCell>
                                            <TableCell>{formatPhone(row.phone)}</TableCell>
                                            <TableCell>
                                                <DropdownMenu>
                                                    <DropdownMenuTrigger asChild>
                                                        <Button size='icon' variant='ghost'>
                                                            <Ellipsis />
                                                        </Button>
                                                    </DropdownMenuTrigger>
                                                    <DropdownMenuContent className='w-56'>
                                                        <DropdownMenuItem asChild>
                                                            <Link to={`by-location/${row.id}/edit`}>
                                                                <Pencil />
                                                                Edit Contact
                                                            </Link>
                                                        </DropdownMenuItem>
                                                        <DropdownMenuItem
                                                            onClick={() =>
                                                                removeLocationContact(
                                                                    row.id,
                                                                    row.name,
                                                                    row.partnerLocationId,
                                                                    location?.name,
                                                                )
                                                            }
                                                        >
                                                            <SquareX />
                                                            Remove Contact
                                                        </DropdownMenuItem>
                                                    </DropdownMenuContent>
                                                </DropdownMenu>
                                            </TableCell>
                                        </TableRow>
                                    );
                                })}
                                <TableRow>
                                    <TableCell colSpan={5}>
                                        <Button asChild className='w-full' variant='ghost'>
                                            <Link to={`create?contactType=${PartnerContactType.LOCATION_SPECIFIC}`}>
                                                <Plus /> Add Location-specific Contact
                                            </Link>
                                        </Button>
                                    </TableCell>
                                </TableRow>
                            </TableBody>
                        </Table>
                    </CardContent>
                </Card>
                <Dialog onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/contacts`)} open={!!isEdit}>
                    <DialogContent className='sm:max-w-[680px]' onOpenAutoFocus={(e) => e.preventDefault()}>
                        <DialogHeader>
                            <DialogTitle>Edit Contact</DialogTitle>
                        </DialogHeader>
                        <div>
                            <Outlet />
                        </div>
                    </DialogContent>
                </Dialog>
                <Dialog onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/contacts`)} open={!!isCreate}>
                    <DialogContent className='sm:max-w-[680px]' onOpenAutoFocus={(e) => e.preventDefault()}>
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
