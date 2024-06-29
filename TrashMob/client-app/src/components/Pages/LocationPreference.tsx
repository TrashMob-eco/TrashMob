import { ChangeEvent, FC, FormEvent, useEffect, useState } from 'react';
import UserData from '../Models/UserData';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../../store/ToolTips";
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from '../../store/AuthStore';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Button, Col, Container, Form, Row } from 'react-bootstrap';
import * as MapStore from '../../store/MapStore';
import { getKey } from '../../store/MapStore';
import AddressData from '../Models/AddressData';
import { data } from 'azure-maps-control';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapControllerSinglePoint from '../MapControllerSinglePoint';
import infoCycle from '../assets/info-circle.svg';
import React from 'react';
import { HeroSection } from '../Customization/HeroSection';
import { useMutation, useQuery } from '@tanstack/react-query';
import { GetUserById, UpdateUser } from '../../services/users';
import { Services } from '../../config/services.config';
import { AzureMapSearchAddressReverse } from '../../services/maps';

interface LocationPreferenceProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
    onUserUpdated: any;
}

const LocationPreference: FC<LocationPreferenceProps> = (props) => {
    const userId = props.currentUser.id;
    const [isDataLoaded, setIsDataLoaded] = useState<boolean>(false);
    const [userName, setUserName] = useState<string>("");
    const [email, setEmail] = useState<string>();
    const [city, setCity] = useState<string>();
    const [radiusType, setRadiusType] = useState<string>("");
    const [country, setCountry] = useState<string>();
    const [region, setRegion] = useState<string>();
    const [postalCode, setPostalCode] = useState<string>();
    const [dateAgreedToTrashMobWaiver, setDateAgreedToTrashMobWaiver] = useState<Date>(new Date());
    const [trashMobWaiverVersion, setTrashMobWaiverVersion] = useState<string>("");
    const [memberSince, setMemberSince] = useState<Date>(new Date());
    const [maxEventsRadiusErrors, setMaxEventsRadiusErrors] = useState<string>("");
    const [longitude, setLongitude] = useState<number>(0);
    const [latitude, setLatitude] = useState<number>(0);
    const [prefersMetric, setPrefersMetric] = useState<boolean>(false);
    const [travelLimitForLocalEvents, setTravelLimitForLocalEvents] = useState<number>(10);
    const [travelLimitForLocalEventsErrors, setTravelLimitForLocalEventsErrors] = useState<string>("");
    const [center, setCenter] = useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = useState<boolean>(false);
    const [mapOptions, setMapOptions] = useState<IAzureMapOptions>();
    const [eventName, setEventName] = useState<string>("User's Base Location");
    const [isSaveEnabled, setIsSaveEnabled] = useState<boolean>(false);
    const [formSubmitted, setFormSubmitted] = useState<boolean>(false);
    const [formSubmitErrors, setFormSubmitErrors] = useState<string>("");
    const [units, setUnits] = useState<string[]>([]);

    const getUserById = useQuery({ 
        queryKey: GetUserById({ userId: userId }).key,
        queryFn: GetUserById({ userId: userId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false
    });

    const updateUser = useMutation({ 
        mutationKey: UpdateUser().key,
        mutationFn: UpdateUser().service,
    });
    
    const azureMapSearchAddressReverse = useMutation({ 
        mutationKey: AzureMapSearchAddressReverse().key,
        mutationFn: AzureMapSearchAddressReverse().service,
    });

    useEffect(() => {
        window.scrollTo(0, 0);
        setUnits(["mi", "km"]);
        if (props.isUserLoaded && !isDataLoaded) {
            setEventName("User's Base Location");
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
                setTravelLimitForLocalEvents(res.data.data.travelLimitForLocalEvents);
                setMaxEventsRadiusErrors("");
                setTravelLimitForLocalEventsErrors("");
                setRadiusType(res.data.data.prefersMetric ? 'km' : 'mi')
                setIsDataLoaded(true);
            })

            setIsDataLoaded(true);
        }

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })

        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                const point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point)
            });
        }
    }, [userId, props.isUserLoaded, isDataLoaded])

    // This will handle Cancel button click event.  
    const handleCancel = (event: FormEvent<HTMLElement>) => {
        event.preventDefault();
        props.history.push("/");
    }

    React.useEffect(() => {
        if (travelLimitForLocalEventsErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
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
        body.userName = userName ?? "";
        body.email = email ?? "";
        body.city = city ?? "";
        body.region = region ?? "";
        body.country = country ?? "";
        body.postalCode = postalCode ?? "";
        body.dateAgreedToTrashMobWaiver = new Date(dateAgreedToTrashMobWaiver);
        body.memberSince = new Date(memberSince);
        body.latitude = latitude;
        body.longitude = longitude;
        body.prefersMetric = prefersMetric;
        body.travelLimitForLocalEvents = travelLimitForLocalEvents;
        body.trashMobWaiverVersion = trashMobWaiverVersion;

        updateUser.mutateAsync(body).then(res => {
            if (res.status !== 200) setFormSubmitErrors("Unknown error occured while checking user name. Please try again. Error Code: " + res.status);
            else {
                setFormSubmitted(true);
                props.onUserUpdated();
            }
        })
    }

    const handleTravelLimitForLocalEventsChanged = (val: string) => {
        try {
            if (val) {
                const intVal = parseInt(val);

                if (intVal <= 0 || intVal > 1000) {
                    setTravelLimitForLocalEventsErrors("Travel limit must be greater than or equal to 0 and less than 1000.")
                }
                else {
                    setTravelLimitForLocalEvents(intVal);
                    setTravelLimitForLocalEventsErrors("");
                }
            }
            else {
                setTravelLimitForLocalEvents(1);
            }
        }
        catch {
            setTravelLimitForLocalEventsErrors("Travel limit must be a valid number.");
        }
    }

    const handleRadiusTypeChanged = (val: string) => {
        if (val === "mi") {
            setPrefersMetric(false);
        }
        else {
            setPrefersMetric(true);
        }
    }

    const renderCityToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.LocationPreferenceCity}</Tooltip>
    }

    const renderRegionToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.LocationPreferenceRegion}</Tooltip>
    }

    const renderPostalCodeToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.LocationPreferencePostalCode}</Tooltip>
    }

    const renderTravelLimitForLocalEventsToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.LocationPreferenceTravelLimitForLocalEvents}</Tooltip>
    }

    const handleLocationChange = async (point: data.Position) => {
        setLatitude(point[1]);
        setLongitude(point[0]);
        const azureKey = await getKey();
        azureMapSearchAddressReverse.mutateAsync({ azureKey, lat: point[0], long: point[1] }).then((res) => {
            setCity(res.data.addresses[0].address.municipality);
            setCountry(res.data.addresses[0].address.country);
            setRegion(res.data.addresses[0].address.countrySubdivisionName);
            setPostalCode(res.data.addresses[0].address.postalCode);
        })
    }

    return (
        !isDataLoaded ? <div>Loading</div> :
            <div>
                <HeroSection Title='Set your location' Description='Get notified for events near you!'></HeroSection>
                <Container className='p-4 bg-white mt-5 rounded'>
                    <h4 className='fw-600 color-primary my-3 main-header'>Location preferences</h4>
                    <Form onSubmit={handleSave}>
                        <Form.Row>
                            <AzureMapsProvider>
                                <>
                                    <MapControllerSinglePoint center={center} isEventDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={eventName} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} isDraggable={true} eventDate={new Date()} />
                                </>
                            </AzureMapsProvider>
                        </Form.Row>
                        <Form.Row className='mt-4'>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderTravelLimitForLocalEventsToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="maxEventsRadius">Maximum event radius <img className='m-0 ml-2' src={infoCycle} alt="info" /></Form.Label>
                                    </OverlayTrigger>
                                    <Row>
                                        <Col xs={8}>
                                            <Form.Control type="number" className='border-0 bg-light p-18 h-60' w-100 name="maxEventsRadius" defaultValue={travelLimitForLocalEvents} onChange={(val) => handleTravelLimitForLocalEventsChanged(val.target.value)} maxLength={parseInt('32')} />
                                        </Col>
                                        <Col xs={4}>
                                            <select data-val="true" className='bg-light border-0 p-18 h-60 w-100 rounded p-2' name="radiusType" value={radiusType} onChange={(val) => handleRadiusTypeChanged(val.target.value)} required>
                                                <option value="">-- Select Units --</option>
                                                {
                                                    units.map(unit =>
                                                        <option key={unit} value={unit}>{unit}</option>
                                                )}
                                            </select>
                                        </Col>
                                    </Row>
                                    <span style={{ color: "red" }}>{maxEventsRadiusErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderCityToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="City">City</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light p-18 h-60' disabled name="city" defaultValue={city} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderRegionToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="region">State</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light p-18 h-60' disabled name="region" defaultValue={region} />
                                </Form.Group>
                            </Col>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderPostalCodeToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="PostalCode">Postal Code</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light p-18 h-60' disabled name="postalCode" defaultValue={postalCode} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group className='text-right'>
                                    <Button className="action h-49 p-18" onClick={(e) => handleCancel(e)}>Discard</Button>
                                    <Button disabled={!isSaveEnabled} type="submit" className="action btn-outline ml-2 h-49" variant="outline-primary">Save</Button>
                                </Form.Group>
                                <span>{formSubmitted ? 'Saved!' : ''}</span>
                                <span>{formSubmitErrors ? formSubmitErrors : ''}</span>
                            </Col>
                        </Form.Row>
                    </Form>
                </Container>
            </div >
    );
}

export default withRouter(LocationPreference);