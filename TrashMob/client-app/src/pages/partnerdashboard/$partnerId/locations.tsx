import { useRef, useState } from 'react';
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
import { Ellipsis, Pencil, SquareX, Plus, Map, List, MapPin } from 'lucide-react';
import { Link, Outlet, useMatch, useNavigate, useParams } from 'react-router';
import { SidebarLayout } from '../../layouts/_layout.sidebar';
import { DeletePartnerLocation } from '@/services/locations';
import { Badge } from '@/components/ui/badge';
import { useGetPartnerLocations } from '@/hooks/useGetPartnerLocations';
import { GoogleMapWithKey as GoogleMap } from '@/components/Map/GoogleMap';
import { AdvancedMarker, InfoWindow } from '@vis.gl/react-google-maps';
import { LocationPin, locationPinColors } from '@/components/Partners/location-pin';
import PartnerLocationData from '@/components/Models/PartnerLocationData';
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group';

const useDeletePartnerLocationByLocationId = () => {
    return useMutation({
        mutationKey: DeletePartnerLocation().key,
        mutationFn: DeletePartnerLocation().service,
    });
};

type ViewMode = 'table' | 'map';

export const PartnerLocations = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const isEdit = useMatch(`/partnerdashboard/:partnerId/locations/:locationId/edit`);
    const isCreate = useMatch(`/partnerdashboard/:partnerId/locations/create`);

    const { data: rows } = useGetPartnerLocations({ partnerId });

    const [isDeletingId, setIsDeletingId] = useState<string | null>(null);
    const [viewMode, setViewMode] = useState<ViewMode>('table');
    const deletePartnerLocationByLocationId = useDeletePartnerLocationByLocationId();

    // Map state
    const markersRef = useRef<Record<string, google.maps.marker.AdvancedMarkerElement>>({});
    const [selectedLocationId, setSelectedLocationId] = useState<string>('');
    const selectedLocation = (rows || []).find((loc) => loc.id === selectedLocationId);

    // Filter locations with valid coordinates for map
    const locationsWithCoords = (rows || []).filter(
        (loc) => loc.latitude !== 0 && loc.longitude !== 0 && loc.latitude && loc.longitude,
    );

    // Calculate map bounds to fit all locations
    const getMapBounds = () => {
        if (locationsWithCoords.length === 0) {
            return { center: { lat: 47.6062, lng: -122.3321 }, zoom: 10 }; // Default to Seattle
        }
        if (locationsWithCoords.length === 1) {
            return {
                center: { lat: locationsWithCoords[0].latitude, lng: locationsWithCoords[0].longitude },
                zoom: 14,
            };
        }
        // Calculate center from all locations
        const avgLat = locationsWithCoords.reduce((sum, loc) => sum + loc.latitude, 0) / locationsWithCoords.length;
        const avgLng = locationsWithCoords.reduce((sum, loc) => sum + loc.longitude, 0) / locationsWithCoords.length;
        return { center: { lat: avgLat, lng: avgLng }, zoom: 10 };
    };

    const mapBounds = getMapBounds();

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

    const getPinColor = (location: PartnerLocationData) => {
        if (location.isActive) {
            return locationPinColors.active;
        }
        return locationPinColors.inactive;
    };

    const renderTableView = () => (
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
                                <Badge variant='secondary'>Inactive</Badge>
                            )}
                        </TableCell>
                        <TableCell>
                            {row.partnerLocationContacts && row.partnerLocationContacts.length > 0 ? 'Yes' : 'No'}
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
    );

    const renderMapView = () => (
        <div className='space-y-4'>
            <div className='h-[500px] rounded-lg overflow-hidden border'>
                {locationsWithCoords.length > 0 ? (
                    <GoogleMap defaultCenter={mapBounds.center} defaultZoom={mapBounds.zoom}>
                        {locationsWithCoords.map((location) => (
                            <AdvancedMarker
                                key={location.id}
                                ref={(el) => {
                                    markersRef.current[location.id] = el!;
                                }}
                                position={{ lat: location.latitude, lng: location.longitude }}
                                onClick={() => setSelectedLocationId(location.id)}
                            >
                                <LocationPin color={getPinColor(location)} size={40} />
                            </AdvancedMarker>
                        ))}

                        {selectedLocation && markersRef.current[selectedLocationId] ? (
                            <InfoWindow
                                anchor={markersRef.current[selectedLocationId]}
                                onClose={() => setSelectedLocationId('')}
                            >
                                <div className='p-2 min-w-[200px]'>
                                    <h3 className='font-semibold text-base mb-2'>{selectedLocation.name}</h3>
                                    <div className='text-sm text-muted-foreground space-y-1'>
                                        {selectedLocation.streetAddress ? (
                                            <p>{selectedLocation.streetAddress}</p>
                                        ) : null}
                                        <p>
                                            {selectedLocation.city}
                                            {selectedLocation.region ? `, ${selectedLocation.region}` : ''}
                                            {selectedLocation.postalCode ? ` ${selectedLocation.postalCode}` : ''}
                                        </p>
                                        <div className='flex items-center gap-2 mt-2'>
                                            {selectedLocation.isActive ? (
                                                <Badge variant='success' className='text-xs'>
                                                    Active
                                                </Badge>
                                            ) : (
                                                <Badge variant='secondary' className='text-xs'>
                                                    Inactive
                                                </Badge>
                                            )}
                                            {selectedLocation.partnerLocationContacts &&
                                            selectedLocation.partnerLocationContacts.length > 0 ? (
                                                <Badge variant='outline' className='text-xs'>
                                                    Ready
                                                </Badge>
                                            ) : null}
                                        </div>
                                    </div>
                                    <div className='mt-3 pt-2 border-t'>
                                        <Button variant='outline' size='sm' className='w-full' asChild>
                                            <Link to={`${selectedLocation.id}/edit`}>
                                                <Pencil className='h-3 w-3 mr-1' />
                                                Edit Location
                                            </Link>
                                        </Button>
                                    </div>
                                </div>
                            </InfoWindow>
                        ) : null}
                    </GoogleMap>
                ) : (
                    <div className='h-full flex flex-col items-center justify-center bg-muted/50'>
                        <MapPin className='h-12 w-12 text-muted-foreground mb-4' />
                        <p className='text-muted-foreground'>No locations with coordinates to display.</p>
                        <p className='text-sm text-muted-foreground'>Add a location to see it on the map.</p>
                    </div>
                )}
            </div>
            <Button variant='outline' asChild>
                <Link to='create'>
                    <Plus className='h-4 w-4 mr-2' /> Add Location
                </Link>
            </Button>
        </div>
    );

    return (
        <SidebarLayout
            title='Edit Partner Locations'
            description='A partner location can be thought of as an instance of a business franchise, or the location of a municipal office or yard. You can have as many locations within a community as you want to set up. Each location can offer different services, and have different contact information associated with it. For instance, City Hall may provide starter kits and supplies, but only the public utilities yard offers hauling and disposal. A partner location must have at least one contact set up in order to be ready for events to use them. It must also be Active.'
        >
            <div className='space-y-4'>
                <div className='flex justify-between items-center'>
                    <div className='text-sm text-muted-foreground'>
                        {(rows || []).length} location{(rows || []).length !== 1 ? 's' : ''}
                    </div>
                    <ToggleGroup
                        type='single'
                        value={viewMode}
                        onValueChange={(value) => value && setViewMode(value as ViewMode)}
                    >
                        <ToggleGroupItem value='table' aria-label='Table view'>
                            <List className='h-4 w-4' />
                        </ToggleGroupItem>
                        <ToggleGroupItem value='map' aria-label='Map view'>
                            <Map className='h-4 w-4' />
                        </ToggleGroupItem>
                    </ToggleGroup>
                </div>

                {viewMode === 'table' ? renderTableView() : renderMapView()}

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
