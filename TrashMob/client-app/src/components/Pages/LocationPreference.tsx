import { ChangeEvent, FC, FormEvent, useEffect, useState, useCallback, useRef } from 'react';
import OverlayTrigger from 'react-bootstrap/OverlayTrigger';
import Tooltip from 'react-bootstrap/Tooltip';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Button, Col, Container, Form, Row } from 'react-bootstrap';
import { useMutation, useQuery } from '@tanstack/react-query';
import { APIProvider, Map, useMap } from '@vis.gl/react-google-maps';

import UserData from '../Models/UserData';
import * as ToolTips from '../../store/ToolTips';
import * as MapStore from '../../store/MapStore';
import infoCycle from '../assets/info-circle.svg';

import { HeroSection } from '../Customization/HeroSection';
import { GetUserById, UpdateUser } from '../../services/users';
import { Services } from '../../config/services.config';
import { MarkerWithInfoWindow } from '../Map';
import { AzureSearchLocationInput, SearchLocationOption } from '../Map/AzureSearchLocationInput';
import { GetGoogleMapApiKey } from '../../services/maps';
import { useAzureMapSearchAddressReverse } from '../../hooks/useAzureMapSearchAddressReverse';

interface LocationPreferenceProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
    onUserUpdated: any;
}

const LocationPreference: FC<LocationPreferenceProps> = (props) => {
    const userId = props.currentUser.id;
    const [isDataLoaded, setIsDataLoaded] = useState<boolean>(false);
    const [userName, setUserName] = useState<string>('');
    const [email, setEmail] = useState<string>();
    const [city, setCity] = useState<string>();
    const [radiusType, setRadiusType] = useState<string>('');
    const [country, setCountry] = useState<string>();
    const [region, setRegion] = useState<string>();
    const [postalCode, setPostalCode] = useState<string>();
    const [dateAgreedToTrashMobWaiver, setDateAgreedToTrashMobWaiver] = useState<Date>(new Date());
    const [trashMobWaiverVersion, setTrashMobWaiverVersion] = useState<string>('');
    const [memberSince, setMemberSince] = useState<Date>(new Date());
    const [maxEventsRadiusErrors, setMaxEventsRadiusErrors] = useState<string>('');
    const [longitude, setLongitude] = useState<number>(0);
    const [latitude, setLatitude] = useState<number>(0);
    const [prefersMetric, setPrefersMetric] = useState<boolean>(false);
    const [travelLimitForLocalEvents, setTravelLimitForLocalEvents] = useState<number>(10);
    const [travelLimitForLocalEventsErrors, setTravelLimitForLocalEventsErrors] = useState<string>('');

    const [center, setCenter] = useState<google.maps.LatLngLiteral>({
        lat: MapStore.defaultLongitude,
        lng: MapStore.defaultLatitude,
    });
    const [azureSubscriptionKey, setAzureSubscriptionKey] = useState<string>();
    const [isSaveEnabled, setIsSaveEnabled] = useState<boolean>(false);
    const [formSubmitted, setFormSubmitted] = useState<boolean>(false);
    const [formSubmitErrors, setFormSubmitErrors] = useState<string>('');
    const [units, setUnits] = useState<string[]>([]);

    const getUserById = useQuery({
        queryKey: GetUserById({ userId }).key,
        queryFn: GetUserById({ userId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const updateUser = useMutation({
        mutationKey: UpdateUser().key,
        mutationFn: UpdateUser().service,
    });

    const { refetch: refetchAddressReverse } = useAzureMapSearchAddressReverse(
        {
            lat: latitude,
            long: longitude,
            azureKey: azureSubscriptionKey || '',
        },
        { enabled: false },
    );

    useEffect(() => {
        window.scrollTo(0, 0);
        setUnits(['mi', 'km']);
        if (props.isUserLoaded && !isDataLoaded) {
            getUserById.refetch().then((res) => {
                if (res.data === undefined || res.data.data === null) return;

                setUserName(res.data.data.userName);
                setEmail(res.data.data.email);
                setCity(res.data.data.city);
                setCountry(res.data.data.country);
                setRegion(res.data.data.region);
                setPostalCode(res.data.data.postalCode);
                setDateAgreedToTrashMobWaiver(res.data.data.dateAgreedToTrashMobWaiver);
                setTrashMobWaiverVersion(res.data.data.trashMobWaiverVersion);
                setMemberSince(res.data.data.memberSince);
                setLatitude(res.data.data.latitude);
                setLongitude(res.data.data.longitude);
                setPrefersMetric(res.data.data.prefersMetric);
                setTravelLimitForLocalEvents(Math.max(res.data.data.travelLimitForLocalEvents, 1));
                setMaxEventsRadiusErrors('');
                setTravelLimitForLocalEventsErrors('');
                setRadiusType(res.data.data.prefersMetric ? 'km' : 'mi');
                setIsDataLoaded(true);
            });

            setIsDataLoaded(true);
        }

        MapStore.getOption().then((opts) => {
            setAzureSubscriptionKey(opts.subscriptionKey);
        });

        if ('geolocation' in navigator) {
            navigator.geolocation.getCurrentPosition((position) => {
                setCenter({ lat: position.coords.latitude, lng: position.coords.longitude });
            });
        }
    }, [userId, props.isUserLoaded, isDataLoaded]);

    // This will handle Cancel button click event.
    const handleCancel = (event: FormEvent<HTMLElement>) => {
        event.preventDefault();
        props.history.push('/');
    };

    useEffect(() => {
        if (travelLimitForLocalEventsErrors !== '') {
            setIsSaveEnabled(false);
        } else {
            setIsSaveEnabled(true);
        }
    }, [travelLimitForLocalEventsErrors]);

    // This will handle the submit form event.
    const handleSave = (event: ChangeEvent<HTMLFormElement>) => {
        event.preventDefault();
        if (!isSaveEnabled) return;

        setIsSaveEnabled(false);
        const body = new UserData();
        body.id = userId;
        body.userName = userName ?? '';
        body.email = email ?? '';
        body.city = city ?? '';
        body.region = region ?? '';
        body.country = country ?? '';
        body.postalCode = postalCode ?? '';
        body.dateAgreedToTrashMobWaiver = new Date(dateAgreedToTrashMobWaiver);
        body.memberSince = new Date(memberSince);
        body.latitude = latitude;
        body.longitude = longitude;
        body.prefersMetric = prefersMetric;
        body.travelLimitForLocalEvents = travelLimitForLocalEvents;
        body.trashMobWaiverVersion = trashMobWaiverVersion;

        updateUser.mutateAsync(body).then((res) => {
            if (res.status !== 200) {
                setFormSubmitErrors(
                    `Unknown error occured while checking user name. Please try again. Error Code: ${res.status}`,
                );
            } else {
                setFormSubmitted(true);
                props.onUserUpdated();
            }
        });
    };

    const handleTravelLimitForLocalEventsChanged = (val: string) => {
        try {
            if (val) {
                const intVal = parseInt(val);

                if (intVal <= 0 || intVal > 1000) {
                    setTravelLimitForLocalEventsErrors(
                        'Travel limit must be greater than or equal to 0 and less than 1000.',
                    );
                } else {
                    setTravelLimitForLocalEvents(intVal);
                    setTravelLimitForLocalEventsErrors('');
                }
            } else {
                setTravelLimitForLocalEvents(1);
            }
        } catch {
            setTravelLimitForLocalEventsErrors('Travel limit must be a valid number.');
        }
    };

    const handleRadiusTypeChanged = (val: string) => {
        if (val === 'mi') {
            setPrefersMetric(false);
            setRadiusType('mi');
        } else {
            setPrefersMetric(true);
            setRadiusType('km');
        }
    };

    const renderCityToolTip = (props: any) => <Tooltip {...props}>{ToolTips.LocationPreferenceCity}</Tooltip>;

    const renderRegionToolTip = (props: any) => <Tooltip {...props}>{ToolTips.LocationPreferenceRegion}</Tooltip>;

    const renderPostalCodeToolTip = (props: any) => (
        <Tooltip {...props}>{ToolTips.LocationPreferencePostalCode}</Tooltip>
    );

    const renderTravelLimitForLocalEventsToolTip = (props: any) => (
        <Tooltip {...props}>{ToolTips.LocationPreferenceTravelLimitForLocalEvents}</Tooltip>
    );

    const map = useMap();
    const radiusRef = useRef<google.maps.Circle>();

    // On Map Initialized, add circle polygon
    useEffect(() => {
        if (!map) return;

        const radiusCircle = new google.maps.Circle({
            strokeColor: '#96ba00',
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: '#96ba00',
            fillOpacity: 0.2,
            map,
        });
        radiusRef.current = radiusCircle;
    }, [map]);

    // On radius, lat, lng changed, update radius polygon
    useEffect(() => {
        if (map && radiusRef.current) {
            radiusRef.current.setCenter({ lat: latitude, lng: longitude });

            // Note: radius unit is meter.
            radiusRef.current.setRadius(travelLimitForLocalEvents * (radiusType === 'km' ? 1000 : 1600));
        }
    }, [map, radiusRef, latitude, longitude, travelLimitForLocalEvents, radiusType]);

    const handleSelectSearchLocation = useCallback(
        async (location: SearchLocationOption) => {
            const { lat, lon } = location.position;
            setLatitude(lat);
            setLongitude(lon);

            // side effect: Move Map Center
            if (map) map.panTo({ lat, lng: lon });
        },
        [map],
    );

    const handleMarkerDragEnd = useCallback(
        async (e: google.maps.MapMouseEvent) => {
            if (e.latLng) {
                const lat = e.latLng.lat();
                const lng = e.latLng.lng();
                setLatitude(lat);
                setLongitude(lng);

                // Note: Map center does not move, only save new marker position
            }
        },
        [map],
    );

    // on Marker moved (latitude + longitude changed), do reverse search lat,lng to address
    useEffect(() => {
        const searchAddressReverse = async () => {
            const { data } = await refetchAddressReverse();

            const firstResult = data?.addresses[0];
            if (firstResult) {
                setCity(firstResult.address.municipality);
                setCountry(firstResult.address.country);
                setRegion(firstResult.address.countrySubdivisionName);
                setPostalCode(firstResult.address.postalCode);
            }
        };
        if (latitude && longitude) searchAddressReverse();
    }, [latitude, longitude]);

    const date = new Date().toLocaleDateString([], { month: 'long', day: '2-digit', year: 'numeric' });
    const time = new Date().toLocaleTimeString([], { timeZoneName: 'short' });

    return !isDataLoaded ? (
        <div>Loading</div>
    ) : (
        <div>
            <HeroSection Title='Set your location' Description='Get notified for events near you!'></HeroSection>
            <Container className='p-4 bg-white mt-5 rounded'>
                <h4 className='fw-600 color-primary my-3 main-header'>Location preferences</h4>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <div style={{ position: 'relative', width: '100%' }}>
                            <Map
                                mapId='6f295631d841c617'
                                gestureHandling={'greedy'}
                                disableDefaultUI={true}
                                style={{ width: '100%', height: '500px' }}
                                defaultCenter={center}
                                defaultZoom={MapStore.defaultUserLocationZoom}
                            >
                                <MarkerWithInfoWindow
                                    position={{ lat: latitude, lng: longitude }}
                                    draggable={true}
                                    onDragEnd={handleMarkerDragEnd}
                                    infoWindowTrigger='hover'
                                    infoWindowProps={{
                                        headerDisabled: true,
                                    }}
                                    infoWindowContent={
                                        <>
                                            <h5
                                                className='font-weight-bold'
                                                style={{ fontSize: '18px', marginTop: '0.5rem' }}
                                            >
                                                User's Base Location
                                            </h5>
                                            <p>
                                                <span className='font-weight-bold'>Event Date:</span> {date}
                                                <br />
                                                <span className='font-weight-bold'>Time: </span> {time}
                                            </p>
                                        </>
                                    }
                                />
                            </Map>

                            {azureSubscriptionKey && (
                                <div style={{ position: 'absolute', top: 8, left: 8 }}>
                                    <AzureSearchLocationInput
                                        azureKey={azureSubscriptionKey}
                                        onSelectLocation={handleSelectSearchLocation}
                                    />
                                </div>
                            )}
                        </div>
                    </Form.Row>
                    <Form.Row className='mt-4'>
                        <Col lg={6}>
                            <Form.Group>
                                <OverlayTrigger placement='top' overlay={renderTravelLimitForLocalEventsToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='maxEventsRadius'>
                                        Maximum event radius <img className='m-0 ml-2' src={infoCycle} alt='info' />
                                    </Form.Label>
                                </OverlayTrigger>
                                <Row>
                                    <Col xs={8}>
                                        <Form.Control
                                            type='number'
                                            className='border-0 bg-light p-18 h-60 w-100'
                                            name='maxEventsRadius'
                                            value={travelLimitForLocalEvents}
                                            onChange={(val) => handleTravelLimitForLocalEventsChanged(val.target.value)}
                                            maxLength={parseInt('32')}
                                        />
                                    </Col>
                                    <Col xs={4}>
                                        <select
                                            data-val='true'
                                            className='bg-light border-0 p-18 h-60 w-100 rounded p-2'
                                            name='radiusType'
                                            value={radiusType}
                                            onChange={(val) => handleRadiusTypeChanged(val.target.value)}
                                            required
                                        >
                                            <option value=''>-- Select Units --</option>
                                            {units.map((unit) => (
                                                <option key={unit} value={unit}>
                                                    {unit}
                                                </option>
                                            ))}
                                        </select>
                                    </Col>
                                </Row>
                                <span style={{ color: 'red' }}>{maxEventsRadiusErrors}</span>
                            </Form.Group>
                        </Col>
                        <Col lg={6}>
                            <Form.Group>
                                <OverlayTrigger placement='top' overlay={renderCityToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='City'>
                                        City
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    className='border-0 bg-light p-18 h-60'
                                    disabled
                                    name='city'
                                    defaultValue={city}
                                />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col lg={6}>
                            <Form.Group>
                                <OverlayTrigger placement='top' overlay={renderRegionToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='region'>
                                        State
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    className='border-0 bg-light p-18 h-60'
                                    disabled
                                    name='region'
                                    defaultValue={region}
                                />
                            </Form.Group>
                        </Col>
                        <Col lg={6}>
                            <Form.Group>
                                <OverlayTrigger placement='top' overlay={renderPostalCodeToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='PostalCode'>
                                        Postal Code
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    className='border-0 bg-light p-18 h-60'
                                    disabled
                                    name='postalCode'
                                    defaultValue={postalCode}
                                />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group className='text-right'>
                                <Button className='action h-49 p-18' onClick={(e) => handleCancel(e)}>
                                    Discard
                                </Button>
                                <Button
                                    disabled={!isSaveEnabled}
                                    type='submit'
                                    className='action btn-outline ml-2 h-49'
                                    variant='outline-primary'
                                >
                                    Save
                                </Button>
                            </Form.Group>
                            <span>{formSubmitted ? 'Saved!' : ''}</span>
                            <span>{formSubmitErrors ? formSubmitErrors : ''}</span>
                        </Col>
                    </Form.Row>
                </Form>
            </Container>
        </div>
    );
};

const LocationPreferenceWrapper = (props: LocationPreferenceProps) => {
    const { data: googleApiKey, isLoading } = useQuery({
        queryKey: GetGoogleMapApiKey().key,
        queryFn: GetGoogleMapApiKey().service,
        select: (res) => res.data,
    });

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <LocationPreference {...props} />
        </APIProvider>
    );
};

export default withRouter(LocationPreferenceWrapper);

