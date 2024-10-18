import * as React from 'react';
import { Button, Col, Form, OverlayTrigger, ToggleButton, Tooltip } from 'react-bootstrap';
import { Guid } from 'guid-typescript';
import { useMutation, useQuery } from '@tanstack/react-query';
import UserData from '../Models/UserData';
import * as ToolTips from '../../store/ToolTips';
import PartnerLocationData from '../Models/PartnerLocationData';
import * as MapStore from '../../store/MapStore';
import { CreatePartnerLocations, GetPartnerLocations, UpdatePartnerLocations } from '../../services/locations';
import { Services } from '../../config/services.config';
import { AzureMapSearchAddressReverse } from '../../services/maps';
import { GoogleMap } from '../Map/GoogleMap';
import { APIProvider, MapMouseEvent, Marker, useMap } from '@vis.gl/react-google-maps';
import { useGetGoogleMapApiKey } from '../../hooks/useGetGoogleMapApiKey';
import { useAzureMapSearchAddressReverse } from '../../hooks/useAzureMapSearchAddressReverse';
import { AzureSearchLocationInput, SearchLocationOption } from '../Map/AzureSearchLocationInput';

export interface PartnerLocationEditDataProps {
    partnerId: string;
    partnerLocationId: string;
    onCancel: any;
    onSave: any;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const PartnerLocationEdit: React.FC<PartnerLocationEditDataProps> = (props) => {
    const [partnerLocationId, setPartnerLocationId] = React.useState<string>(Guid.EMPTY);
    const [locationName, setLocationName] = React.useState<string>('');
    const [locationNameErrors, setLocationNameErrors] = React.useState<string>('');
    const [publicNotes, setPublicNotes] = React.useState<string>('');
    const [publicNotesErrors, setPublicNotesErrors] = React.useState<string>('');
    const [privateNotes, setPrivateNotes] = React.useState<string>('');
    const [isPartnerLocationActive, setIsPartnerLocationActive] = React.useState<boolean>(true);
    const [streetAddress, setStreetAddress] = React.useState<string>('');
    const [city, setCity] = React.useState<string>('');
    const [country, setCountry] = React.useState<string>('');
    const [region, setRegion] = React.useState<string>('');
    const [postalCode, setPostalCode] = React.useState<string>('');
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>();
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [isPartnerLocationDataLoaded, setIsPartnerLocationDataLoaded] = React.useState<boolean>(false);

    const getPartnerLocations = useQuery({
        queryKey: GetPartnerLocations({ locationId: props.partnerLocationId }).key,
        queryFn: GetPartnerLocations({ locationId: props.partnerLocationId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const createPartnerLocations = useMutation({
        mutationKey: CreatePartnerLocations().key,
        mutationFn: CreatePartnerLocations().service,
    });

    const updatePartnerLocations = useMutation({
        mutationKey: UpdatePartnerLocations().key,
        mutationFn: UpdatePartnerLocations().service,
    });

    const azureMapSearchAddressReverse = useMutation({
        mutationKey: AzureMapSearchAddressReverse().key,
        mutationFn: AzureMapSearchAddressReverse().service,
    });

    React.useEffect(() => {
        if (props.isUserLoaded && props.partnerLocationId === Guid.EMPTY) setIsPartnerLocationDataLoaded(true);
        else if (props.isUserLoaded && props.partnerLocationId && props.partnerLocationId !== Guid.EMPTY) {
            getPartnerLocations.refetch().then((res) => {
                if (res.data === undefined) return;
                setPartnerLocationId(res.data?.data.id);
                setLocationName(res.data?.data.name);
                setStreetAddress(res.data?.data.streetAddress);
                setCity(res.data?.data.city);
                setCountry(res.data?.data.country);
                setRegion(res.data?.data.region);
                setPostalCode(res.data?.data.postalCode);
                setLatitude(res.data?.data.latitude);
                setLongitude(res.data?.data.longitude);
                setIsPartnerLocationActive(res.data?.data.isActive);
                setCreatedByUserId(res.data?.data.createdByUserId);
                setCreatedDate(new Date(res.data?.data.createdDate));
                setLastUpdatedDate(new Date(res.data?.data.lastUpdatedDate));
                setPublicNotes(res.data?.data.publicNotes);
                setIsPartnerLocationDataLoaded(true);
            });
        }

        MapStore.getOption().then((opts) => {
            setAzureSubscriptionKey(opts.subscriptionKey);
        });

    }, [props.currentUser, props.partnerLocationId, props.isUserLoaded, props.partnerId]);

    function handleLocationNameChanged(locationName: string) {
        if (locationName === '') {
            setLocationNameErrors('Location Name cannot be empty.');
        } else {
            setLocationName(locationName);
            setLocationNameErrors('');
        }
    }

    function handlePublicNotesChanged(notes: string) {
        if (notes === '') {
            setPublicNotesErrors('Public notes cannot be empty.');
        } else {
            setPublicNotes(notes);
            setPublicNotesErrors('');
        }
    }

    function handleIsPartnerLocationActiveChanged(val: boolean) {
        setIsPartnerLocationActive(val);
    }

    function handlePrivateNotesChanged(notes: string) {
        setPrivateNotes(notes);
    }

    function renderPartnerLocationNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationName}</Tooltip>;
    }

    function renderStreetAddressToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationStreetAddress}</Tooltip>;
    }

    function renderCityToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationCity}</Tooltip>;
    }

    function renderCountryToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationCountry}</Tooltip>;
    }

    function renderRegionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationRegion}</Tooltip>;
    }

    function renderPostalCodeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationPostalCode}</Tooltip>;
    }

    function renderIsPartnerLocationActiveToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationIsPartnerLocationActive}</Tooltip>;
    }

    function renderPublicNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationPublicNotes}</Tooltip>;
    }

    function renderPrivateNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationPrivateNotes}</Tooltip>;
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerCreatedDate}</Tooltip>;
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLastUpdatedDate}</Tooltip>;
    }

    React.useEffect(() => {
        if (publicNotes === '' || publicNotesErrors !== '' || country === '') {
            setIsSaveEnabled(false);
        } else {
            setIsSaveEnabled(true);
        }
    }, [publicNotes, publicNotesErrors, country]);

    async function handleSave(event: any) {
        event.preventDefault();

        if (!isSaveEnabled) return;
        setIsSaveEnabled(false);

        const body = new PartnerLocationData();
        body.id = partnerLocationId;
        body.partnerId = props.partnerId;
        body.name = locationName ?? '';
        body.streetAddress = streetAddress ?? '';
        body.city = city ?? '';
        body.region = region ?? '';
        body.country = country ?? '';
        body.postalCode = postalCode ?? '';
        body.latitude = latitude ?? 0;
        body.longitude = longitude ?? 0;
        body.isActive = isPartnerLocationActive;
        body.publicNotes = publicNotes ?? '';
        body.privateNotes = privateNotes ?? '';
        body.createdByUserId = createdByUserId ?? props.currentUser.id;
        body.createdDate = createdDate;

        if (partnerLocationId === Guid.EMPTY) await createPartnerLocations.mutateAsync(body);
        else await updatePartnerLocations.mutateAsync(body);

        props.onSave();
    }

    const map = useMap()

    const handleSelectSearchLocation = React.useCallback(
        async (location: SearchLocationOption) => {
            const { lat, lon } = location.position;
            setLatitude(lat);
            setLongitude(lon);

            // side effect: Move Map Center
            if (map) map.panTo({ lat, lng: lon });
        },
        [map],
    );

    const handleClickMap = React.useCallback((e: MapMouseEvent) => {
        if (e.detail.latLng) {
            const lat = e.detail.latLng.lat;
            const lng = e.detail.latLng.lng;
            setLatitude(lat);
            setLongitude(lng);
        }
    }, [])

    const handleMarkerDragEnd = React.useCallback((e: google.maps.MapMouseEvent) => {
        if (e.latLng) {
            const lat = e.latLng.lat();
            const lng = e.latLng.lng();
            setLatitude(lat);
            setLongitude(lng);
        }
    }, [])

    const [azureSubscriptionKey, setAzureSubscriptionKey] = React.useState<string>();
    const { refetch: refetchAddressReverse } = useAzureMapSearchAddressReverse(
        {
            lat: latitude,
            long: longitude,
            azureKey: azureSubscriptionKey || '',
        },
        { enabled: false },
    );

    // on Marker moved (latitude + longitude changed), do reverse search lat,lng to address
    React.useEffect(() => {
        const searchAddressReverse = async () => {
            const { data } = await refetchAddressReverse();

            const firstResult = data?.addresses[0];
            if (firstResult) {
                setStreetAddress(firstResult.address.streetNameAndNumber)
                setCity(firstResult.address.municipality);
                setCountry(firstResult.address.country);
                setRegion(firstResult.address.countrySubdivisionName);
                setPostalCode(firstResult.address.postalCode);
            }
        };
        if (latitude && longitude) searchAddressReverse();
    }, [latitude, longitude]);

    const defaultCenter = { lat: MapStore.defaultLatitude, lng: MapStore.defaultLongitude }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        setLocationName('');
        setStreetAddress('');
        setCity('');
        setRegion('');
        setCountry('');
        setPostalCode('');
        setLatitude(0);
        setLongitude(0);
        setPrivateNotes('');
        setPublicNotes('');
        setCreatedByUserId(Guid.EMPTY);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        props.onCancel();
    }

    function renderEditLocation() {
        return (
            <div>
                <h2 className='color-primary mt-4 mb-5'>Edit Partner Location</h2>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <input type='hidden' name='Id' value={partnerLocationId} />
                    </Form.Row>
                    <Button disabled={!isSaveEnabled} type='submit' className='btn btn-default'>
                        Save
                    </Button>
                    <Button className='action' onClick={(e: any) => handleCancel(e)}>
                        Cancel
                    </Button>
                    <Form.Row>
                        <Col>
                            <Form.Group className='required'>
                                <OverlayTrigger placement='top' overlay={renderPartnerLocationNameToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='LocationName'>
                                        Location Name
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    name='locationName'
                                    defaultValue={locationName}
                                    onChange={(val) => handleLocationNameChanged(val.target.value)}
                                    maxLength={parseInt('64')}
                                    required
                                />
                                <span style={{ color: 'red' }}>{locationNameErrors}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement='top' overlay={renderIsPartnerLocationActiveToolTip}>
                                    <Form.Label
                                        className='control-label font-weight-bold h5'
                                        htmlFor='IsPartnerLocationActive'
                                    >
                                        Is Active
                                    </Form.Label>
                                </OverlayTrigger>
                                <ToggleButton
                                    type='checkbox'
                                    variant='outline-dark'
                                    checked={isPartnerLocationActive}
                                    value='1'
                                    onChange={(e) => handleIsPartnerLocationActiveChanged(e.currentTarget.checked)}
                                />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement='top' overlay={renderStreetAddressToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='StreetAddress'>
                                        Street Address
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    className='border-0 bg-light h-60 p-18'
                                    disabled
                                    name='streetAddress'
                                    value={streetAddress}
                                />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group className='required'>
                                <OverlayTrigger placement='top' overlay={renderCityToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='City'>
                                        City
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    className='border-0 bg-light h-60 p-18'
                                    disabled
                                    name='city'
                                    value={city}
                                />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement='top' overlay={renderPostalCodeToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='PostalCode'>
                                        Postal Code
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    className='border-0 bg-light h-60 p-18'
                                    disabled
                                    name='postalCode'
                                    value={postalCode}
                                />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group className='required'>
                                <OverlayTrigger placement='top' overlay={renderRegionToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='Region'>
                                        Region
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    className='border-0 bg-light h-60 p-18'
                                    disabled
                                    name='region'
                                    value={region}
                                />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group className='required'>
                                <OverlayTrigger placement='top' overlay={renderCountryToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='Country'>
                                        Country
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    className='border-0 bg-light h-60 p-18'
                                    disabled
                                    name='country'
                                    value={country}
                                />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Group className='required'>
                        <OverlayTrigger placement='top' overlay={renderPublicNotesToolTip}>
                            <Form.Label className='control-label font-weight-bold h5'>Public Notes</Form.Label>
                        </OverlayTrigger>
                        <Form.Control
                            as='textarea'
                            defaultValue={publicNotes}
                            maxLength={parseInt('2048')}
                            rows={5}
                            cols={5}
                            onChange={(val) => handlePublicNotesChanged(val.target.value)}
                            required
                        />
                        <span style={{ color: 'red' }}>{publicNotesErrors}</span>
                    </Form.Group>
                    <Form.Group>
                        <OverlayTrigger placement='top' overlay={renderPrivateNotesToolTip}>
                            <Form.Label className='control-label font-weight-bold h5'>Private Notes</Form.Label>
                        </OverlayTrigger>
                        <Form.Control
                            as='textarea'
                            defaultValue={privateNotes}
                            maxLength={parseInt('2048')}
                            rows={5}
                            cols={5}
                            onChange={(val) => handlePrivateNotesChanged(val.target.value)}
                        />
                    </Form.Group>
                    <Form.Row>
                        <Form.Label>
                            Click on the map to set the location for your Partner. The location fields above will be
                            automatically populated.
                        </Form.Label>
                    </Form.Row>
                    <Form.Row>
                        <div style={{ position: 'relative', width: '100%' }}>
                            <GoogleMap defaultCenter={defaultCenter} onClick={handleClickMap}>
                                <Marker
                                    position={(latitude && longitude) ? {
                                        lat: latitude,
                                        lng: longitude
                                    } : defaultCenter}
                                    draggable
                                    onDragEnd={handleMarkerDragEnd}
                                />
                            </GoogleMap>
                            {azureSubscriptionKey ? (
                                <div style={{ position: 'absolute', top: 8, left: 8 }}>
                                    <AzureSearchLocationInput
                                        azureKey={azureSubscriptionKey}
                                        onSelectLocation={handleSelectSearchLocation}
                                    />
                                </div>
                            ) : null}
                        </div>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement='top' overlay={renderCreatedDateToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='createdDate'>
                                        Created Date
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    className='border-0 bg-light h-60 p-18'
                                    disabled
                                    name='createdDate'
                                    value={createdDate ? createdDate.toLocaleString() : ''}
                                />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement='top' overlay={renderLastUpdatedDateToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='lastUpdatedDate'>
                                        Last Updated Date
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    className='border-0 bg-light h-60 p-18'
                                    disabled
                                    name='lastUpdatedDate'
                                    value={lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ''}
                                />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    return (
        <div>
            {!isPartnerLocationDataLoaded && (
                <p>
                    <em>Loading...</em>
                </p>
            )}
            {isPartnerLocationDataLoaded ? renderEditLocation() : null}
        </div>
    );
};

const PartnerLocationEditWrapper = (props: PartnerLocationEditDataProps) => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey()

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <PartnerLocationEdit {...props} />
        </APIProvider>
    );
};

export default PartnerLocationEditWrapper