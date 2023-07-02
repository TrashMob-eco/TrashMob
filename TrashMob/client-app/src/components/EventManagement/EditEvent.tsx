import * as React from 'react'
import EventData from '../Models/EventData';
import EventTypeData from '../Models/EventTypeData';
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from '../../store/AuthStore';
import { data } from 'azure-maps-control';
import { getKey } from '../../store/MapStore';
import AddressData from '../Models/AddressData';
import * as MapStore from '../../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import UserData from '../Models/UserData';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../../store/ToolTips";
import { Button, Col, Container, Form, ToggleButton } from 'react-bootstrap';
import { Guid } from 'guid-typescript';
import MapControllerSinglePoint from '../MapControllerSinglePoint';
import { RouteComponentProps } from 'react-router-dom';
import { CurrentTrashMobWaiverVersion } from '../Waivers/Waivers';
import WaiverData from '../Models/WaiverData';
import moment from 'moment';
import { EventStatusActive } from '../Models/Constants';

export interface EditEventProps extends RouteComponentProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
    onEditCancel: any;
    onEditSave: any;
}

export const EditEvent: React.FC<EditEventProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [eventId, setEventId] = React.useState<string>(props.eventId);
    const [eventName, setEventName] = React.useState<string>("New Event");
    const [description, setDescription] = React.useState<string>("");
    const [eventDate, setEventDate] = React.useState<Date>(new Date());
    const [eventTime, setEventTime] = React.useState<string>(moment(new Date()).format('HH:mm'));
    const [absTime, setAbsTime] = React.useState<Date>(new Date());
    const [durationHours, setDurationHours] = React.useState<number>(1);
    const [durationMinutes, setDurationMinutes] = React.useState<number>(0);
    const [eventTypeId, setEventTypeId] = React.useState<number>(0);
    const [streetAddress, setStreetAddress] = React.useState<string>();
    const [city, setCity] = React.useState<string>("");
    const [country, setCountry] = React.useState<string>("");
    const [region, setRegion] = React.useState<string>("");
    const [postalCode, setPostalCode] = React.useState<string>("");
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [maxNumberOfParticipants, setMaxNumberOfParticipants] = React.useState<number>(0);
    const [isEventPublic, setIsEventPublic] = React.useState<boolean>(true);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>();
    const [eventStatusId, setEventStatusId] = React.useState<number>(EventStatusActive);
    const [eventTypeList, setEventTypeList] = React.useState<EventTypeData[]>([]);
    const [eventDateErrors, setEventDateErrors] = React.useState<string>("");
    const [maxNumberOfParticipantsErrors, setMaxNumberOfParticipantsErrors] = React.useState<string>("");
    const [durationHoursErrors, setDurationHoursErrors] = React.useState<string>("");
    const [durationMinutesErrors, setDurationMinutesErrors] = React.useState<string>("");
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    // const [title, setTitle] = React.useState<string>("Create Event");
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');

        fetch('/api/eventtypes', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setEventTypeList(data);
            })
            .then(() => {
                // This will set state for Edit Event  
                if (eventId !== null && eventId !== "" && eventId !== Guid.EMPTY) {
                    // setTitle("Edit Event");
                    fetch('/api/Events/' + eventId, {
                        method: 'GET',
                        headers: headers
                    })
                        .then(response => response.json() as Promise<EventData>)
                        .then(eventData => {
                            setEventId(eventData.id);
                            setEventName(eventData.name);
                            setDescription(eventData.description);
                            setEventDate(new Date(eventData.eventDate));
                            setAbsTime(new Date(eventData.eventDate));
                            setEventTime(new Date(eventData.eventDate).toTimeString());
                            setDurationHours(eventData.durationHours);
                            setDurationMinutes(eventData.durationMinutes);
                            setEventTypeId(eventData.eventTypeId);
                            setStreetAddress(eventData.streetAddress);
                            setCity(eventData.city);
                            setCountry(eventData.country);
                            setRegion(eventData.region);
                            setPostalCode(eventData.postalCode);
                            setLatitude(eventData.latitude);
                            setLongitude(eventData.longitude);
                            setMaxNumberOfParticipants(eventData.maxNumberOfParticipants);
                            setIsEventPublic(eventData.isEventPublic);
                            setCreatedByUserId(eventData.createdByUserId);
                            setEventStatusId(eventData.eventStatusId);
                            setCenter(new data.Position(eventData.longitude, eventData.latitude));
                            setIsDataLoaded(true);
                        });
                }
            });

        if (eventId === Guid.EMPTY) {

            fetch('/api/waivers/trashmob', {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<WaiverData>)
                .then(data => {

                    // Have user sign waiver if needed
                    const isTrashMobWaiverOutOfDate = new Date(props.currentUser.dateAgreedToTrashMobWaiver) < CurrentTrashMobWaiverVersion.versionDate;
                    if (data.isWaiverEnabled && (isTrashMobWaiverOutOfDate || (props.currentUser.trashMobWaiverVersion === ""))) {
                        sessionStorage.setItem('targetUrl', window.location.pathname);
                        props.history.push("/waivers");
                    }

                    setIsDataLoaded(true);
                })
        }

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })

        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                var point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point)
            });
        } else {
            console.log("Not Available");
        }
    }, [eventId, props.currentUser.dateAgreedToTrashMobWaiver, props.currentUser.trashMobWaiverVersion, props.history])

    React.useEffect(() => {
        if (eventName === "" ||
            eventDateErrors !== "" ||
            description === "" ||
            durationHoursErrors !== "" ||
            region === "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }, [eventName, eventDateErrors, description, durationHoursErrors, region]);

    function handleDurationHoursChanged(val: string) {
        try {
            if (val) {
                var hours = parseInt(val);

                if (hours < 0 || hours > 10) {
                    setDurationHoursErrors("Duration Hours must be greater than 0 and less than 10");
                }
                else {
                    setDurationHoursErrors("");
                    setDurationHours(hours);
                }
            }
            else {
                setDurationHours(0);
            }
        }
        catch { }
    }

    function handleDurationMinutesChanged(val: string) {
        try {
            if (val) {
                var minutes = parseInt(val);

                if (minutes < 0 || minutes > 59) {
                    setDurationMinutesErrors("Duration Minutes must be greater than 0 and less than 60");
                }
                else {
                    setDurationMinutesErrors("");
                    setDurationMinutes(minutes);
                }
            }
            else {
                setDurationMinutes(0);
            }
        }
        catch { }
    }

    function handleMaxNumberOfParticipantsChanged(val: string) {

        try {
            if (val) {
                var intVal = parseInt(val);

                if (intVal < 0) {
                    setMaxNumberOfParticipantsErrors("Max number of participants must be greater than or equal to zero.");
                }
                else {
                    setMaxNumberOfParticipantsErrors("")
                    setMaxNumberOfParticipants(parseInt(val));
                }
            }
        }
        catch {
            setMaxNumberOfParticipantsErrors("Invalid value specified for Max Number of Participants.");
        }
    }

    function handleLocationChange(point: data.Position) {
        // In an Azure Map point, the longitude is the first position, and latitude is second
        setLatitude(point[1]);
        setLongitude(point[0]);
        var locationString = point[1] + ',' + point[0]
        var headers = getDefaultHeaders('GET');

        getKey()
            .then(key => {
                fetch('https://atlas.microsoft.com/search/address/reverse/json?subscription-key=' + key + '&api-version=1.0&query=' + locationString, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<AddressData>)
                    .then(data => {
                        setStreetAddress(data.addresses[0].address.streetNameAndNumber);
                        setCity(data.addresses[0].address.municipality);
                        setCountry(data.addresses[0].address.country);
                        setRegion(data.addresses[0].address.countrySubdivisionName);
                        setPostalCode(data.addresses[0].address.postalCode);
                    })
            }
            )
    }

    function handleEventDateChanged(passedDate: string) {
        var abTime = new Date(passedDate + " " + eventTime);

        if (isEventPublic && abTime < new Date()) {
            setEventDateErrors("Public event cannot be in the past");
        }
        else {
            setEventDateErrors("");
        }

        setEventDate(abTime);
        setAbsTime(abTime);
    }

    function handleEventTimeChanged(passedTime: string) {
        var abTime = new Date(eventDate.toDateString() + " " + passedTime);
        
        if (isEventPublic && abTime < new Date()) {
            setEventDateErrors("Public event cannot be in the past");
        }
        else {
            setEventDateErrors("");
        }

        setEventTime(moment(abTime).format('HH:mm'))
        setEventDate(abTime);
        setAbsTime(abTime);
    }

    function handleIsEventPublicChanged(value: boolean) {
        setIsEventPublic(value);

        if (value && absTime < new Date()) {
            setEventDateErrors("Public event cannot be in the past");
        }
        else {
            setEventDateErrors("");
        }
    }

    function renderDescriptionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventDescription}</Tooltip>
    }

    function selectEventType(val: string) {
        setEventTypeId(parseInt(val));
    }

    function renderEventNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventName}</Tooltip>
    }

    function renderDurationHoursToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventDurationHours}</Tooltip>
    }

    function renderDurationMinutesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventDurationMinutes}</Tooltip>
    }

    function renderMaxNumberOfParticipantsToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventMaxNumberOfParticipants}</Tooltip>
    }

    function renderEventTypeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventType}</Tooltip>
    }

    function renderEventDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventDate}</Tooltip>
    }

    function renderEventTimeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventTime}</Tooltip>
    }

    function renderIsEventPublicToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventIsEventPublic}</Tooltip>
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        props.onEditCancel();
    }

    // This will handle the submit form event.  
    function handleSave(event: any) {

        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        var eventData = new EventData();
        var method = "POST";

        if (eventId && eventId !== Guid.EMPTY) {
            eventData.id = eventId;
            method = "PUT";
        }

        eventData.name = eventName ?? "";
        eventData.description = description ?? "";
        eventData.eventDate = new Date(absTime);
        eventData.durationHours = durationHours ?? 2;
        eventData.durationMinutes = durationMinutes ?? 0;
        eventData.eventTypeId = eventTypeId ?? 0;
        eventData.streetAddress = streetAddress ?? "";
        eventData.city = city ?? "";
        eventData.region = region ?? "";
        eventData.country = country ?? "";
        eventData.postalCode = postalCode ?? "";
        eventData.latitude = latitude ?? 0;
        eventData.longitude = longitude ?? 0;
        eventData.maxNumberOfParticipants = maxNumberOfParticipants ?? 0;
        eventData.isEventPublic = isEventPublic;
        eventData.createdByUserId = createdByUserId ?? props.currentUser.id;
        eventData.eventStatusId = eventStatusId;

        var evtdata = JSON.stringify(eventData);

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {

            if (!validateToken(tokenResponse.idTokenClaims)) {
                return;
            }

            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/Events', {
                method: method,
                headers: headers,
                body: evtdata,
            }).then(() => {
                props.onEditSave();
            });
        })
    }

    const dateForPicker = (dateString: Date) => {
        return moment(new Date(dateString)).format('YYYY-MM-DD')
    };

    const timeForPicker = (dateString: Date) => {
        return moment(new Date(dateString)).format('HH:mm')
    };

    // Returns the HTML Form to the render() method.  
    function renderCreateForm(typeList: Array<EventTypeData>) {
        return (
            <div>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <input type="hidden" name="Id" value={eventId.toString()} />
                    </Form.Row>
                    <Container className='p-4 bg-white rounded my-5'>
                        <h4 className='fw-600 color-primary my-4'>Event details</h4>
                        <Form.Row>
                            <Col lg={6}>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderEventNameToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="Name">Name</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light h-60 p-18' name="name" defaultValue={eventName} onChange={(val) => setEventName(val.target.value)} maxLength={parseInt('64')} required />
                                </Form.Group>
                            </Col>
                            <Col lg={6}>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderEventTypeToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="EventType">Type</Form.Label>
                                    </OverlayTrigger>

                                    <select data-val="true" className='bg-light border-0 p-18 h-60 w-100 rounded p-2' name="eventTypeId" defaultValue={eventTypeId} onChange={(val) => selectEventType(val.target.value)} required>
                                        <option value="">-- Select Event Type --</option>
                                        {typeList.sort((a, b) => (a.displayOrder > b.displayOrder) ? 1 : -1).map(type =>
                                            <option key={type.id} value={type.id}>{type.name}</option>
                                        )}
                                    </select>

                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col lg={6}>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderEventDateToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="EventDate">Date</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="date" className='border-0 bg-light h-60 p-18' name="eventDate" value={dateForPicker(absTime)} onChange={(val: any) => handleEventDateChanged(val.target.value)} />
                                    <span style={{ color: "red" }}>{eventDateErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col lg={6}>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderEventTimeToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="Event Time">Time</Form.Label>
                                    </OverlayTrigger>
                                    <div>
                                        <Form.Control type="time" className='border-0 bg-light h-60 p-18' size="sm" name="eventTime" value={timeForPicker(absTime)} onChange={(val) => handleEventTimeChanged(val.target.value)} />
                                    </div>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col lg={6}>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderDurationHoursToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="DurationHours">Expected Duration in Hours</Form.Label>
                                    </OverlayTrigger>
                                    <div>
                                        <Form.Control type="text" className='border-0 bg-light h-60 p-18' size="sm" name="durationHours" defaultValue={durationHours} onChange={(val) => handleDurationHoursChanged(val.target.value)} />
                                        <span style={{ color: "red" }}>{durationHoursErrors}</span>
                                    </div>
                                </Form.Group>
                            </Col>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderDurationMinutesToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="DurationMinutes">Additional Minutes</Form.Label>
                                    </OverlayTrigger>
                                    <div>
                                        <Form.Control type="text" className='border-0 bg-light h-60 p-18' size="sm" name="durationMinutes" defaultValue={durationMinutes} onChange={(val) => handleDurationMinutesChanged(val.target.value)} />
                                        <span style={{ color: "red" }}>{durationMinutesErrors}</span>
                                    </div>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderMaxNumberOfParticipantsToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="MaxNumberOfParticipants">Max Number Of Participants</Form.Label>
                                    </OverlayTrigger >
                                    <Form.Control type="text" className='border-0 bg-light h-60 p-18' name="maxNumberOfParticipants" defaultValue={maxNumberOfParticipants} onChange={(val) => handleMaxNumberOfParticipantsChanged(val.target.value)} />
                                    <span style={{ color: "red" }}>{maxNumberOfParticipantsErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col lg={6}>
                                <Form.Group>
                                    <div>
                                        <OverlayTrigger placement="top" overlay={renderIsEventPublicToolTip}>
                                            <Form.Label className="control-label font-weight-bold h5" htmlFor="isEventPublic">Public Event</Form.Label>
                                        </OverlayTrigger >
                                    </div>
                                    <div>
                                        <ToggleButton
                                            type="checkbox"
                                            variant="outline-dark"
                                            checked={isEventPublic}
                                            value="1"
                                            onChange={(e) => handleIsEventPublicChanged(e.currentTarget.checked)}
                                        />
                                    </div>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderDescriptionToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="Description">Description</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control as="textarea" className='border-0 bg-light h-60 p-18' name="description" defaultValue={description} onChange={(val) => setDescription(val.target.value)} maxLength={parseInt('2048')} rows={5} cols={5} required />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group className='text-right'>
                                    <Button className="action h-49 p-18" onClick={(e) => handleCancel(e)}>Discard</Button>
                                    <Button disabled={!isSaveEnabled} type="submit" className="action btn-outline ml-2 h-49" variant="outline-primary">Save</Button>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                    </Container>

                    <Container className='p-4 bg-white rounded my-5'>
                        <h4 className='fw-600 color-primary my-4'>Event location</h4>
                        <Form.Row>
                            <AzureMapsProvider>
                                <>
                                    <MapControllerSinglePoint center={center} isEventDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={eventName} eventDate={absTime} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} isDraggable={true} />
                                </>
                            </AzureMapsProvider>
                        </Form.Row>
                        <Form.Row className='mt-5'>
                            <Col lg={4}>
                                <Form.Group>
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="StreetAddress">Street Address</Form.Label>
                                    <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="streetAddress" value={streetAddress} />
                                </Form.Group>
                            </Col>
                            <Col lg={4}>
                                <Form.Group className="required">
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="City">City</Form.Label>
                                    <Form.Control className='border-0 bg-light h-60 p-18' disabled type="text" name="city" value={city} />
                                </Form.Group>
                            </Col>
                            <Col lg={4}>
                                <Form.Group>
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="PostalCode">Postal Code</Form.Label>
                                    <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="postalCode" value={postalCode} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row className='mt-4'>
                            <Col lg={4}>
                                <Form.Group className="required">
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="Country">Country</Form.Label>
                                    <Form.Control type="text" className='border-0 bg-light h-60 p-18' name="country" value={country ?? ""} />
                                </Form.Group>
                            </Col>
                            <Col lg={4}>
                                <Form.Group>
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="Latitude">Latitude</Form.Label>
                                    <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="latitude" value={latitude} />
                                </Form.Group>
                            </Col>
                            <Col lg={4}>
                                <Form.Group>
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="Longitude">Longitude</Form.Label>
                                    <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="longitude" value={longitude} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group className='text-right'>
                                    <Button className="action h-49 p-18" onClick={(e) => handleCancel(e)}>Discard</Button>
                                    <Button disabled={!isSaveEnabled} type="submit" className="action btn-outline ml-2 h-49" variant="outline-primary">Save</Button>
                                </Form.Group>
                            </Col>
                        </Form.Row>

                    </Container>
                </Form >
            </div>
        )

    }

    var contents = isDataLoaded && eventId
        ? renderCreateForm(eventTypeList)
        : <p><em>Loading...</em></p>;

    return <div>
        {contents}
    </div>;
}