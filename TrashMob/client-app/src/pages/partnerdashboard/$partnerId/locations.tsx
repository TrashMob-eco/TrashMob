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
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Ellipsis, Pencil, SquareX, Plus } from 'lucide-react';
import { Link, Outlet, useMatch, useNavigate, useParams } from 'react-router';
import { SidebarLayout } from '../../layouts/_layout.sidebar';
import { useState } from 'react';
import { DeletePartnerLocation } from '@/services/locations';
import { Badge } from '@/components/ui/badge';
import { useGetPartnerLocations } from '@/hooks/useGetPartnerLocations';

const useDeletePartnerLocationByLocationId = () => {
    return useMutation({
        mutationKey: DeletePartnerLocation().key,
        mutationFn: DeletePartnerLocation().service,
    });
};

export const PartnerLocations = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const isEdit = useMatch(`/partnerdashboard/:partnerId/locations/:locationId/edit`);
    const isCreate = useMatch(`/partnerdashboard/:partnerId/locations/create`);

    const { data: rows } = useGetPartnerLocations({ partnerId });

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
            description='A partner location can be thought of as an instance of a business franchise, or the location of a municipal office or yard. You can have as many locations within a community as you want to set up. Each location can offer different services, and have different contact information associated with it. For instance, City Hall may provide starter kits and supplies, but only the public utilities yard offers hauling and disposal. A partner location must have at least one contact set up in order to be ready for events to use them. It must also be Active.'
            title='Edit Partner Locations'
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
                            <TableRow className={isDeletingId === row.id ? 'opacity-20' : ''} key={row.id}>
                                <TableCell>{row.name}</TableCell>
                                <TableCell>{row.city}</TableCell>
                                <TableCell>{row.region}</TableCell>
                                <TableCell>
                                    {row.isActive ? (
                                        <Badge variant='success'>Active</Badge>
                                    ) : (
                                        <Badge variant='secondary'>Inactive</Badge>
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
                                            <Button size='icon' variant='ghost'>
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
                                <Button asChild className='w-full' variant='ghost'>
                                    <Link to='create'>
                                        <Plus /> Add Location
                                    </Link>
                                </Button>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
                <Dialog onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/locations`)} open={!!isEdit}>
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
                <Dialog onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/locations`)} open={!!isCreate}>
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
