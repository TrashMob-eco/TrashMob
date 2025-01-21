import * as React from 'react';
import { Ellipsis, PencilIcon, Plus, SquareX } from 'lucide-react'
import { Guid } from 'guid-typescript';
import { useMutation, useQuery } from '@tanstack/react-query';
import UserData from '../Models/UserData';
import PartnerLocationData from '../Models/PartnerLocationData';
import PartnerLocationEdit from './PartnerLocationEdit';
import { PartnerLocationServices } from './PartnerLocationServices';
import { PartnerLocationEventRequests } from './PartnerLocationEventRequests';
import { DeletePartnerLocation, GetLocationsByPartner } from '../../services/locations';
import { Services } from '../../config/services.config';
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
  } from "@/components/ui/dialog"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { Badge } from '@/components/ui/badge';

export interface PartnerLocationsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const PartnerLocations: React.FC<PartnerLocationsDataProps> = (props) => {
    const [partnerLocations, setPartnerLocations] = React.useState<PartnerLocationData[]>([]);
    const [isPartnerLocationDataLoaded, setIsPartnerLocationDataLoaded] = React.useState<boolean>(false);
    const [partnerLocationId, setPartnerLocationId] = React.useState<string>('');
    const [isEdit, setIsEdit] = React.useState<boolean>(false);
    const [isAdd, setIsAdd] = React.useState<boolean>(false);

    const getLocationsByPartner = useQuery({
        queryKey: GetLocationsByPartner({ partnerId: props.partnerId }).key,
        queryFn: GetLocationsByPartner({ partnerId: props.partnerId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const deletePartnerLocation = useMutation({
        mutationKey: DeletePartnerLocation().key,
        mutationFn: DeletePartnerLocation().service,
    });

    React.useEffect(() => {
        if (props.isUserLoaded) {
            getLocationsByPartner.refetch().then((res) => {
                setPartnerLocations(res.data?.data || []);
                setIsPartnerLocationDataLoaded(true);
            });
        }
    }, [props.currentUser, props.isUserLoaded, props.partnerId]);

    function removeLocation(locationId: string, name: string) {
        if (
            !window.confirm(
                `Please confirm that you want to remove Location with name: '${name}' as a location from this Partner?`,
            )
        )
            return;

        deletePartnerLocation.mutateAsync({ locationId }).then(() => {
            setIsPartnerLocationDataLoaded(false);
            getLocationsByPartner.refetch().then((res) => {
                setPartnerLocations(res.data?.data || []);
                setIsPartnerLocationDataLoaded(true);
            });
        });
    }

    function addLocation() {
        setPartnerLocationId(Guid.EMPTY);
        setIsAdd(true);
    }

    // This will handle Cancel button click event.
    function handleCancel() {
        setPartnerLocationId(Guid.EMPTY);
        setIsAdd(false);
        setIsEdit(false);
    }

    // This will handle Save button click event.
    function handleSave() {
        setPartnerLocationId(Guid.EMPTY);
        getLocationsByPartner.refetch().then((res) => {
            setPartnerLocations(res.data?.data || []);
            setIsPartnerLocationDataLoaded(true);
            setIsAdd(false);
            setIsEdit(false);
        });
    }

    function editLocation(partnerLocationId: string) {
        setPartnerLocationId(partnerLocationId);
        setIsEdit(true);
    }

    function renderEditPartnerLocation() {
        return (
            <div>
                <PartnerLocationEdit
                    partnerId={props.partnerId}
                    partnerLocationId={partnerLocationId}
                    currentUser={props.currentUser}
                    isUserLoaded={props.isUserLoaded}
                    onCancel={handleCancel}
                    onSave={handleSave}
                />
                <hr />
            </div>
        );
    }

    function renderPartnerLocationServices() {
        return (
            <div>
                <PartnerLocationServices
                    partnerLocationId={partnerLocationId}
                    currentUser={props.currentUser}
                    isUserLoaded={props.isUserLoaded}
                />
                <hr />
            </div>
        );
    }

    function renderPartnerLocationServicesHelp() {
        return (
            <div className='bg-white py-2 px-5 shadow-sm rounded'>
                <h2 className='color-primary mt-4 mb-5'>Edit Partner Location Services</h2>
                <p>
                    This page allows you set up the services offered by a partner location. That is, what capabilities
                    are you willing to provide to TrashMob.eco users to help them clean up the local community? This
                    support is crucial to the success of TrashMob.eco volunteers, and we appreciate your help!
                </p>
            </div>
        );
    }

    function renderPartnerLocationEventRequests() {
        return (
            <div>
                <PartnerLocationEventRequests
                    partnerLocationId={partnerLocationId}
                    currentUser={props.currentUser}
                    isUserLoaded={props.isUserLoaded}
                />
                <hr />
            </div>
        );
    }

    function renderPartnerLocationEventRequestsHelp() {
        return (
            <div className='bg-white py-2 px-5 shadow-sm rounded'>
                <h2 className='color-primary mt-4 mb-5'>Edit Partner Location Service Requests</h2>
                <p>
                    This page allows you to respond to requests from TrashMob.eco users to help them clean up the local
                    community. When a new event is set up, and a user selects one of your services the location contacts
                    will be notified to accept or decline the request here.
                </p>
            </div>
        );
    }

    return (
        <div className="container mx-auto">
            <div className="grid grid-cols-12 gap-8">
                <div className="col-span-12 lg:col-span-4">
                    <Card>
                        <CardHeader>
                            <CardTitle className="text-primary text-2xl">Edit Partner Locations</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <p>
                                A partner location can be thought of as an instance of a business franchise, or the location
                                of a municipal office or yard. You can have as many locations within a community as you want
                                to set up. Each location can offer different services, and have different contact
                                information associated with it. For instance, City Hall may provide starter kits and
                                supplies, but only the public utilities yard offers hauling and disposal.
                            </p>
                            <p>
                                A partner location must have at least one contact set up in order to be ready for events to
                                use them. It must also be Active.
                            </p>
                        </CardContent>
                    </Card>
                </div>
                <div className="col-span-12 lg:col-span-8">
                    <Card>
                        <CardHeader>
                            <CardTitle className="text-primary text-2xl">Partner Locations</CardTitle>
                        </CardHeader>
                        <CardContent>
                            {!isPartnerLocationDataLoaded && (
                                <p>
                                    <em>Loading...</em>
                                </p>
                            )}
                            {!isEdit && !isAdd && isPartnerLocationDataLoaded && partnerLocations
                                ? (
                                    <div>
                                        <Table className="w-full">
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
                                                {partnerLocations.map((location) => (
                                                    <TableRow key={location.id.toString()}>
                                                        <TableCell>{location.name}</TableCell>
                                                        <TableCell>{location.city}</TableCell>
                                                        <TableCell>{location.region}</TableCell>
                                                        <TableCell>{location.isActive ? <Badge variant="success">Active</Badge> : <Badge variant="secondary">Active</Badge>}</TableCell>
                                                        <TableCell>
                                                            {location.partnerLocationContacts && location.partnerLocationContacts.length > 0
                                                                ? 'Yes'
                                                                : 'No'}
                                                        </TableCell>
                                                        <TableCell>
                                                            <DropdownMenu>
                                                                <DropdownMenuTrigger asChild>
                                                                    <Button variant="ghost"><Ellipsis /></Button>
                                                                </DropdownMenuTrigger>
                                                                <DropdownMenuContent>
                                                                    <DropdownMenuItem onClick={() => editLocation(location.id)}>
                                                                        <PencilIcon />
                                                                        Manage Location
                                                                    </DropdownMenuItem>
                                                                    <DropdownMenuItem onClick={() => removeLocation(location.id, location.name)}>
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
                                                        <Button disabled={isAdd} variant="ghost" className="w-full" onClick={addLocation}>
                                                            <Plus /> 
                                                            Add Location
                                                        </Button>
                                                    </TableCell>
                                                </TableRow>
                                            </TableBody>
                                        </Table>
                                    </div>
                                ) : null}
                            {isEdit || isAdd ? renderEditPartnerLocation() : null}
                            <Dialog open={isAdd} onOpenChange={setIsAdd}>
                                <DialogContent className="min-w-[640px]">
                                    <DialogHeader>
                                        <DialogTitle>Add Location</DialogTitle>
                                    </DialogHeader>
                                    <div className="h-[500px] overflow-y-scroll">
                                        <PartnerLocationEdit
                                            partnerId={props.partnerId}
                                            partnerLocationId={partnerLocationId}
                                            currentUser={props.currentUser}
                                            isUserLoaded={props.isUserLoaded}
                                            onCancel={handleCancel}
                                            onSave={handleSave}
                                        />
                                    </div>
                                </DialogContent>
                            </Dialog>
                        </CardContent>
                    </Card>
                </div>
                <div className="col-span-12 lg:col-span-4">
                    {isEdit || isAdd ? renderPartnerLocationServicesHelp() : null}
                </div>
                <div className="col-span-12 lg:col-span-8">{isEdit || isAdd ? renderPartnerLocationServices() : null}</div>
                <div className="col-span-12 lg:col-span-4">
                    {isEdit || isAdd ? renderPartnerLocationEventRequestsHelp() : null}
                </div>
                <div className="col-span-12 lg:col-span-8">{isEdit || isAdd ? renderPartnerLocationEventRequests() : null}</div>
            </div>
        </div>
    );
};
