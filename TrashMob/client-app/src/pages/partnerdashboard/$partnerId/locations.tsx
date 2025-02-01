import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { GetPartnerContactsByPartnerId } from '@/services/contact';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Ellipsis, Pencil, SquareX, Plus } from 'lucide-react';
import { Link, Outlet, useMatch, useNavigate, useParams } from 'react-router';
import { SidebarLayout } from './_layout.sidebar';
import { useState } from 'react';
import { DeletePartnerLocation, GetLocationsByPartner } from '@/services/locations';
import { Badge } from '@/components/ui/badge';

const useGetLocationsByPartnerId = (partnerId: string) => {
    return useQuery({
        queryKey: GetLocationsByPartner({ partnerId }).key,
        queryFn: GetLocationsByPartner({ partnerId }).service,
        select: (res) => res.data,
    });
};

const useDeletePartnerLocationByLocationId = () => {
    return useMutation({
        mutationKey: DeletePartnerLocation().key,
        mutationFn: DeletePartnerLocation().service,
    });
};

export const PartnerLocations = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const isEdit = useMatch(`/partnerdashboard/:partnerId/locations/:locationId/edit`);
    const isCreate = useMatch(`/partnerdashboard/:partnerId/locations/create`);

    const { data: rows } = useGetLocationsByPartnerId(partnerId);

    const [isDeletingId, setIsDeletingId] = useState<string | null>(null);
    const deletePartnerLocationByLocationId = useDeletePartnerLocationByLocationId();

    function removeLocation(partnerLocationId: string, name: string) {
        if (!window.confirm(`Please confirm that you want to remove location: '${name}' from this Partner ?`)) return;
        setIsDeletingId(partnerLocationId);

        deletePartnerLocationByLocationId
            .mutateAsync({ locationId: partnerLocationId })
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
            title='Edit Partner Locations'
            description='A partner location can be thought of as an instance of a business franchise, or the location of a municipal office or yard. You can have as many locations within a community as you want to set up. Each location can offer different services, and have different contact information associated with it. For instance, City Hall may provide starter kits and supplies, but only the public utilities yard offers hauling and disposal. A partner location must have at least one contact set up in order to be ready for events to use them. It must also be Active.'
        >
            <div>
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Name</TableHead>
                            <TableHead>City</TableHead>
                            <TableHead>Region</TableHead>
                            <TableHead>Status</TableHead>
                            <TableHead>Ready?</TableHead>
                            <TableHead>Actions</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(rows || []).map((row) => (
                            <TableRow key={row.id} className={isDeletingId === row.id ? 'opacity-20' : ''}>
                                <TableCell>{row.name}</TableCell>
                                <TableCell>{row.city}</TableCell>
                                <TableCell>{row.region}</TableCell>
                                <TableCell>
                                    {row.isActive ? (
                                        <Badge variant='success'>Active</Badge>
                                    ) : (
                                        <Badge variant='secondary'>Active</Badge>
                                    )}
                                </TableCell>
                                <TableCell>
                                    {row.partnerLocationContacts && row.partnerLocationContacts.length > 0
                                        ? 'Yes'
                                        : 'No'}
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
                                                    Edit Location
                                                </Link>
                                            </DropdownMenuItem>
                                            <DropdownMenuItem onClick={() => removeLocation(row.id, row.name)}>
                                                <SquareX />
                                                Remove Location
                                            </DropdownMenuItem>
                                        </DropdownMenuContent>
                                    </DropdownMenu>
                                </TableCell>
                            </TableRow>
                        ))}
                        <TableRow>
                            <TableCell colSpan={6}>
                                <Button variant='ghost' className='w-full' asChild>
                                    <Link to='create'>
                                        <Plus /> Add Location
                                    </Link>
                                </Button>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
                <Dialog open={!!isEdit} onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/locations`)}>
                    <DialogContent
                        className='sm:max-w-[600px] overflow-y-scroll max-h-screen'
                        onOpenAutoFocus={(e) => e.preventDefault()}
                    >
                        <DialogHeader>
                            <DialogTitle>Edit Location</DialogTitle>
                        </DialogHeader>
                        <div>
                            <Outlet />
                        </div>
                    </DialogContent>
                </Dialog>
                <Dialog open={!!isCreate} onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/locations`)}>
                    <DialogContent
                        className='sm:max-w-[600px] overflow-y-scroll max-h-screen'
                        onOpenAutoFocus={(e) => e.preventDefault()}
                    >
                        <DialogHeader>
                            <DialogTitle>Create Location</DialogTitle>
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
