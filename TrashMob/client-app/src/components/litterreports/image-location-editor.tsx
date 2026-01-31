import { useCallback, useEffect, useState } from 'react';
import { AdvancedMarker, MapMouseEvent, useMap } from '@vis.gl/react-google-maps';
import toNumber from 'lodash/toNumber';
import { Loader2 } from 'lucide-react';
import { GoogleMapWithKey as GoogleMap } from '@/components/Map/GoogleMap';
import { AzureSearchLocationInput, SearchLocationOption } from '@/components/Map/AzureSearchLocationInput';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { AzureMapSearchAddressReverse } from '@/services/maps';
import * as MapStore from '@/store/MapStore';
import type { ImageWithLocation } from './image-uploader';

interface ImageLocationEditorProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    image: ImageWithLocation | null;
    onSave: (imageId: string, location: Partial<ImageWithLocation>) => void;
}

export const ImageLocationEditor = ({ open, onOpenChange, image, onSave }: ImageLocationEditorProps) => {
    const [azureSubscriptionKey, setAzureSubscriptionKey] = useState<string>('');
    const [latitude, setLatitude] = useState<number | null>(null);
    const [longitude, setLongitude] = useState<number | null>(null);
    const [streetAddress, setStreetAddress] = useState('');
    const [city, setCity] = useState('');
    const [region, setRegion] = useState('');
    const [country, setCountry] = useState('');
    const [postalCode, setPostalCode] = useState('');
    const [isReverseGeocoding, setIsReverseGeocoding] = useState(false);

    // Load Azure key
    useEffect(() => {
        MapStore.getOption().then((opts) => {
            setAzureSubscriptionKey(opts.subscriptionKey);
        });
    }, []);

    // Reset state when image changes
    useEffect(() => {
        if (image) {
            setLatitude(image.latitude);
            setLongitude(image.longitude);
            setStreetAddress(image.streetAddress);
            setCity(image.city);
            setRegion(image.region);
            setCountry(image.country);
            setPostalCode(image.postalCode);
        }
    }, [image]);

    const reverseGeocode = useCallback(
        async (lat: number, lng: number) => {
            if (!azureSubscriptionKey) return;

            setIsReverseGeocoding(true);
            try {
                const response = await AzureMapSearchAddressReverse().service({
                    azureKey: azureSubscriptionKey,
                    lat,
                    long: lng,
                });

                const result = response.data.addresses[0];
                if (result) {
                    setStreetAddress(result.address?.streetName || '');
                    setCity(result.address?.municipality || '');
                    setRegion(result.address?.countrySubdivision || '');
                    setCountry(result.address?.country || '');
                    setPostalCode(result.address?.postalCode || '');
                }
            } catch (error) {
                console.error('Reverse geocoding failed:', error);
            } finally {
                setIsReverseGeocoding(false);
            }
        },
        [azureSubscriptionKey],
    );

    const handleSelectSearchLocation = useCallback(
        (location: SearchLocationOption) => {
            const lat = location.position.lat;
            const lng = location.position.lon;
            setLatitude(lat);
            setLongitude(lng);
            reverseGeocode(lat, lng);
        },
        [reverseGeocode],
    );

    const handleMapClick = useCallback(
        (e: MapMouseEvent) => {
            if (e.detail.latLng) {
                const lat = e.detail.latLng.lat;
                const lng = e.detail.latLng.lng;
                setLatitude(lat);
                setLongitude(lng);
                reverseGeocode(lat, lng);
            }
        },
        [reverseGeocode],
    );

    const handleMarkerDragEnd = useCallback(
        (e: google.maps.MapMouseEvent) => {
            if (e.latLng) {
                const lat = e.latLng.lat();
                const lng = e.latLng.lng();
                setLatitude(lat);
                setLongitude(lng);
                reverseGeocode(lat, lng);
            }
        },
        [reverseGeocode],
    );

    const handleSave = () => {
        if (!image || latitude === null || longitude === null) return;

        onSave(image.id, {
            latitude,
            longitude,
            streetAddress,
            city,
            region,
            country,
            postalCode,
            locationSource: 'manual',
            isLoadingLocation: false,
        });
        onOpenChange(false);
    };

    const isValid = latitude !== null && longitude !== null;
    const defaultCenter = latitude && longitude ? { lat: latitude, lng: longitude } : { lat: 47.6062, lng: -122.3321 };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className='max-w-3xl'>
                <DialogHeader>
                    <DialogTitle>Set Image Location</DialogTitle>
                </DialogHeader>

                <div className='space-y-4'>
                    {/* Map with search */}
                    <div className='relative w-full h-[300px]'>
                        <GoogleMap
                            defaultCenter={defaultCenter}
                            defaultZoom={latitude && longitude ? 15 : 10}
                            onClick={handleMapClick}
                            gestureHandling='greedy'
                        >
                            {latitude && longitude ? (
                                <AdvancedMarker position={{ lat: latitude, lng: longitude }} draggable onDragEnd={handleMarkerDragEnd} />
                            ) : null}
                        </GoogleMap>
                        {azureSubscriptionKey ? (
                            <div className='absolute top-2 left-2 z-10'>
                                <AzureSearchLocationInput
                                    azureKey={azureSubscriptionKey}
                                    onSelectLocation={handleSelectSearchLocation}
                                    placeholder='Search for a location...'
                                />
                            </div>
                        ) : null}
                    </div>

                    <p className='text-sm text-muted-foreground'>Click on the map or search to set the location where this photo was taken.</p>

                    {/* Address fields (read-only) */}
                    {isReverseGeocoding ? (
                        <div className='flex items-center gap-2 text-sm text-muted-foreground'>
                            <Loader2 className='h-4 w-4 animate-spin' />
                            Looking up address...
                        </div>
                    ) : isValid ? (
                        <div className='grid grid-cols-2 gap-4'>
                            <div className='col-span-2'>
                                <Label>Street Address</Label>
                                <Input value={streetAddress} disabled className='mt-1' />
                            </div>
                            <div>
                                <Label>City</Label>
                                <Input value={city} disabled className='mt-1' />
                            </div>
                            <div>
                                <Label>Region</Label>
                                <Input value={region} disabled className='mt-1' />
                            </div>
                            <div>
                                <Label>Country</Label>
                                <Input value={country} disabled className='mt-1' />
                            </div>
                            <div>
                                <Label>Postal Code</Label>
                                <Input value={postalCode} disabled className='mt-1' />
                            </div>
                        </div>
                    ) : null}
                </div>

                <DialogFooter>
                    <Button variant='outline' onClick={() => onOpenChange(false)}>
                        Cancel
                    </Button>
                    <Button onClick={handleSave} disabled={!isValid}>
                        Save Location
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
