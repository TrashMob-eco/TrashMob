import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Badge } from '@/components/ui/badge';
import { useMutation, useQueries, useQuery, useQueryClient } from '@tanstack/react-query';
import { Ban, Check, Ellipsis, Pencil, ToggleRight } from 'lucide-react';
import { Link, Outlet, useMatch, useNavigate, useParams } from 'react-router';
import { SidebarLayout } from '../../layouts/_layout.sidebar';
import { useState } from 'react';
import {
    DeletePartnerLocationServiceByLocationIdAndServiceType,
    GetPartnerLocationsServicesByLocationId,
} from '@/services/locations';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useGetPartnerServiceTypes } from '@/hooks/useGetPartnerServiceTypes';
import { useGetPartnerLocations } from '@/hooks/useGetPartnerLocations';

const useDeleteLocationService = () => {
    return useMutation({
        mutationKey: DeletePartnerLocationServiceByLocationIdAndServiceType().key,
        mutationFn: DeletePartnerLocationServiceByLocationIdAndServiceType().service,
    });
};

export const PartnerServices = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const isEnabling = useMatch(`/partnerdashboard/:partnerId/services/enable`);
    const isEditing = useMatch(`/partnerdashboard/:partnerId/services/edit`);

    const { data: locations } = useGetPartnerLocations({ partnerId });
    const { data: serviceTypes } = useGetPartnerServiceTypes();

    const servicesByLocation = useQueries({
        queries: (locations || []).map((location) => ({
            queryKey: GetPartnerLocationsServicesByLocationId({ locationId: location.id }).key,
            queryFn: GetPartnerLocationsServicesByLocationId({ locationId: location.id }).service,
            select: (res) => res.data,
        })),
    });

    const [isDeletingId, setIsDeletingId] = useState<{ locationId: string; serviceTypeId: number } | null>(null);
    const deleteLocationService = useDeleteLocationService();

    function disableService(locationId: string, serviceTypeId: number) {
        if (!window.confirm(`Please confirm that you want to disable service: from this Location?`)) return;
        setIsDeletingId({ locationId, serviceTypeId });

        deleteLocationService
            .mutateAsync({
                locationId,
                serviceTypeId,
            })
            .then(async () => {
                return queryClient.invalidateQueries({
                    queryKey: GetPartnerLocationsServicesByLocationId({ locationId }).key,
                    refetchType: 'all',
                });
            })
            .then(() => {
                setIsDeletingId(null);
            });
    }

    return (
        <SidebarLayout
            title='Partner Services'
            description='This page allows you set up the services offered by a partner location. That is, what capabilities are you willing to provide to TrashMob.eco users to help them clean up the local community? This support is crucial to the success of TrashMob.eco volunteers, and we appreciate your help!'
            useDefaultCard={false}
        >
            <div className='space-y-4'>
                {(locations || []).map((location, locIndex) => (
                    <Card key={`location-${location.id}`}>
                        <CardHeader>
                            <CardTitle>{location.name}</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <Table>
                                <TableHeader>
                                    <TableRow>
                                        <TableHead>Service Type</TableHead>
                                        <TableHead className='text-center'>Status</TableHead>
                                        <TableHead>Note</TableHead>
                                        <TableHead className='text-right'>Actions</TableHead>
                                    </TableRow>
                                </TableHeader>
                                <TableBody>
                                    {(serviceTypes || []).map((serviceType) => {
                                        const locationServices = servicesByLocation[locIndex].data;
                                        const locationService = (locationServices || []).find(
                                            (ls) => ls.serviceTypeId === serviceType.id,
                                        );
                                        return (
                                            <TableRow
                                                key={serviceType.id}
                                                className={
                                                    isDeletingId?.locationId === location.id &&
                                                    isDeletingId?.serviceTypeId === serviceType.id
                                                        ? 'opacity-20'
                                                        : ''
                                                }
                                            >
                                                <TableCell>{serviceType.name}</TableCell>
                                                <TableCell className='text-center'>
                                                    {locationService ? (
                                                        <Badge variant='success'>
                                                            <Check /> Enabled
                                                        </Badge>
                                                    ) : (
                                                        <Badge variant='default' className='bg-white text-muted'>
                                                            <Ban />
                                                            Disabled
                                                        </Badge>
                                                    )}
                                                </TableCell>
                                                <TableCell>{locationService?.notes}</TableCell>
                                                <TableCell className='text-right'>
                                                    {!locationService ? (
                                                        <Button size='sm' variant='outline' asChild>
                                                            <Link
                                                                to={`enable?locationId=${location.id}&serviceTypeId=${serviceType.id}`}
                                                            >
                                                                Enable
                                                            </Link>
                                                        </Button>
                                                    ) : (
                                                        <DropdownMenu>
                                                            <DropdownMenuTrigger asChild>
                                                                <Button variant='ghost' size='icon'>
                                                                    <Ellipsis />
                                                                </Button>
                                                            </DropdownMenuTrigger>
                                                            <DropdownMenuContent className='w-56'>
                                                                <DropdownMenuItem
                                                                    onClick={() =>
                                                                        disableService(location.id, serviceType.id)
                                                                    }
                                                                >
                                                                    <ToggleRight />
                                                                    Disable Service
                                                                </DropdownMenuItem>
                                                                <DropdownMenuItem asChild>
                                                                    <Link
                                                                        to={`edit?locationId=${location.id}&serviceTypeId=${serviceType.id}`}
                                                                    >
                                                                        <Pencil />
                                                                        Edit Service
                                                                    </Link>
                                                                </DropdownMenuItem>
                                                            </DropdownMenuContent>
                                                        </DropdownMenu>
                                                    )}
                                                </TableCell>
                                            </TableRow>
                                        );
                                    })}
                                </TableBody>
                            </Table>
                        </CardContent>
                        <Dialog
                            open={!!isEnabling}
                            onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/services`)}
                        >
                            <DialogContent
                                className='sm:max-w-[600px] overflow-y-scroll max-h-screen'
                                onOpenAutoFocus={(e) => e.preventDefault()}
                            >
                                <DialogHeader>
                                    <DialogTitle>Enable Service</DialogTitle>
                                </DialogHeader>
                                <div>
                                    <Outlet />
                                </div>
                            </DialogContent>
                        </Dialog>
                        <Dialog
                            open={!!isEditing}
                            onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/services`)}
                        >
                            <DialogContent
                                className='sm:max-w-[600px] overflow-y-scroll max-h-screen'
                                onOpenAutoFocus={(e) => e.preventDefault()}
                            >
                                <DialogHeader>
                                    <DialogTitle>Edit Service</DialogTitle>
                                </DialogHeader>
                                <div>
                                    <Outlet />
                                </div>
                            </DialogContent>
                        </Dialog>
                    </Card>
                ))}
            </div>
        </SidebarLayout>
    );
};
