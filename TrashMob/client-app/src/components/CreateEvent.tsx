import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import EventData from './Models/EventData';
import DateTimePicker from 'react-datetime-picker';
import EventTypeData from './Models/EventTypeData';
import { CountryDropdown, RegionDropdown } from 'react-country-region-selector';
import { withRouter } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { data } from 'azure-maps-control';
import { getKey } from '../store/MapStore';
import AddressData from './Models/AddressData';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from '@ambientlight/react-azure-maps';
import MapController from './MapController';
import UserData from './Models/UserData';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../store/ToolTips";
import { Button, Col, Form } from 'react-bootstrap';

interface CreateEventProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const CreateEvent: React.FC<CreateEventProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [eventName, setEventName] = React.useState<string>("New Event");
    const [description, setDescription] = React.useState<string>();
    const [eventDate, setEventDate] = React.useState<Date>(new Date());
    const [eventTypeId, setEventTypeId] = React.useState<number>(0);
    const [streetAddress, setStreetAddress] = React.useState<string>();
    const [city, setCity] = React.useState<string>();
    const [country, setCountry] = React.useState<string>();
    const [region, setRegion] = React.useState<string>();
    const [postalCode, setPostalCode] = React.useState<string>();
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [maxNumberOfParticipants, setMaxNumberOfParticipants] = React.useState<number>(0);
    const [eventTypeList, setEventTypeList] = React.useState<EventTypeData[]>([]);
    const [eventDateErrors, setEventDateErrors] = React.useState<string>("");
    const [latitudeErrors, setLatitudeErrors] = React.useState<string>("");
    const [longitudeErrors, setLongitudeErrors] = React.useState<string>("");
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');

        fetch('api/eventtypes', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setEventTypeList(data);
                setIsDataLoaded(true);
            });

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
    }, [])


    function handleEventNameChanged(val: string) {
        setEventName(val);
    }

    function handleDescriptionChanged(val: string) {
        setDescription(val);
    }

    function handleStreetAddressChanged(val: string) {
        setStreetAddress(val);
    }

    function handleCityChanged(val: string) {
        setCity(val);
    }

    function selectCountry(val: string) {
        setCountry(val);
    }

    function selectRegion(val: string) {
        setRegion(val);
    }

    function handlePostalCodeChanged(val: string) {
        setPostalCode(val);
    }

    function handleMaxNumberOfParticipantsChanged(val: string) {
        setMaxNumberOfParticipants(parseInt(val));
    }

    function handleLatitudeChanged(val: string) {
        try {
            var floatVal = parseFloat(val);

            if (floatVal < -90 || floatVal > 90) {
                setLatitudeErrors("Latitude must be => -90 and <= 90");
            }
            else {
                setLatitude(floatVal);
                setLatitudeErrors("");
            }
        }
        catch { }
    }

    function handleLongitudeChanged(val: string) {
        try {
            var floatVal = parseFloat(val);

            if (floatVal < -180 || floatVal > 180) {
                setLongitudeErrors("Longitude must be >= -180 and <= 180");
            }
            else {
                setLongitude(floatVal);
                setLongitudeErrors("");
            }
        }
        catch { }
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

    function renderStreetAddressToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventStreetAddress}</Tooltip>
    }

    function renderCityToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventCity}</Tooltip>
    }

    function renderCountryToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventCountry}</Tooltip>
    }

    function renderRegionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventRegion}</Tooltip>
    }

    function renderPostalCodeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventPostalCode}</Tooltip>
    }

    function renderMaxNumberOfParticipantsToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventMaxNumberOfParticipants}</Tooltip>
    }

    function renderLatitudeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventLatitude}</Tooltip>
    }

    function renderLongitudeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventLongitude}</Tooltip>
    }

    function renderEventTypeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventType}</Tooltip>
    }

    function renderEventDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventDate}</Tooltip>
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

    function handleEventDateChange(passedDate: Date) {
        if (passedDate < new Date()) {
            setEventDateErrors("Event cannot be in the past");
        }
        else {
            setEventDateErrors("");
        }

        setEventDate(passedDate);
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push("/mydashboard");
    }

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        if (eventDateErrors !== "") {
            return;
        }

        var eventData = new EventData();

        eventData.name = eventName ?? "";
        eventData.description = description ?? "";
        eventData.eventDate = new Date(eventDate);
        eventData.eventTypeId = eventTypeId ?? 0;
        eventData.streetAddress = streetAddress ?? "";
        eventData.city = city ?? "";
        eventData.region = region ?? "";
        eventData.country = country ?? "";
        eventData.postalCode = postalCode ?? "";
        eventData.latitude = latitude ?? 0;
        eventData.longitude = longitude ?? 0;
        eventData.maxNumberOfParticipants = maxNumberOfParticipants ?? 0;

        eventData.createdByUserId = props.currentUser.id;
        eventData.lastUpdatedByUserId = props.currentUser.id;

        var evtdata = JSON.stringify(eventData);

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {

            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('api/Events', {
                method: 'POST',
                headers: headers,
                body: evtdata,
            }).then(() => {
                props.history.push("/mydashboard");
            })
        })
    }

    // Returns the HTML Form to the render() method.  
    function renderCreateForm(typeList: Array<EventTypeData>) {
        return (
            <div className="container-fluid" >
                <Form onSubmit={handleSave} >
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderEventNameToolTip}>
                                    <Form.Label htmlFor="Name">Name:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="name" defaultValue={eventName} onChange={(val) => handleEventNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderEventDateToolTip}>
                                    <Form.Label htmlFor="EventDate">EventDate:</Form.Label >
                                </OverlayTrigger>
                                <div>
                                    <DateTimePicker name="eventDate" onChange={handleEventDateChange} value={eventDate} />
                                    <span style={{ color: "red" }}>{eventDateErrors}</span>
                                </div>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderEventTypeToolTip}>
                                    <Form.Label htmlFor="EventType">Event Type:</Form.Label>
                                </OverlayTrigger>
                                <div>
                                    <select data-val="true" name="eventTypeId" defaultValue={eventTypeId} onChange={(val) => selectEventType(val.target.value)} required>
                                        <option value="">-- Select Event Type --</option>
                                        {typeList.map(type =>
                                            <option key={type.id} value={type.id}>{type.name}</option>
                                        )}
                                    </select>
                                </div>
                            </Form.Group>
                        </Col>
                    </Form.Row >
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderDescriptionToolTip}>
                                    <Form.Label htmlFor="Description">Description:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control as="textarea" name="description" defaultValue={description} onChange={(val) => handleDescriptionChanged(val.target.value)} maxLength={parseInt('2048')} rows={5} cols={5} required />
                            </Form.Group>
                        </Col>
                    </Form.Row >
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderStreetAddressToolTip}>
                                    <Form.Label htmlFor="StreetAddress">Street Address:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="streetAddress" defaultValue={streetAddress} onChange={(val) => handleStreetAddressChanged(val.target.value)} maxLength={parseInt('256')} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderCityToolTip}>
                                    <Form.Label htmlFor="City">City:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="city" defaultValue={city} onChange={(val) => handleCityChanged(val.target.value)} maxLength={parseInt('256')} required />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderPostalCodeToolTip}>
                                    <Form.Label htmlFor="PostalCode">Postal Code:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="postalCode" defaultValue={postalCode} onChange={(val) => handlePostalCodeChanged(val.target.value)} maxLength={parseInt('25')} />
                            </Form.Group>
                        </Col>
                    </Form.Row >
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderCountryToolTip}>
                                    <Form.Label htmlFor="Country">Country:</Form.Label>
                                </OverlayTrigger>
                                <div>
                                    <CountryDropdown name="country" value={country ?? ""} onChange={(val) => selectCountry(val)} />
                                </div>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderRegionToolTip}>
                                    <Form.Label htmlFor="region">Region:</Form.Label>
                                </OverlayTrigger>
                                <div>
                                    <RegionDropdown
                                        country={country ?? ""}
                                        value={region ?? ""}
                                        onChange={(val) => selectRegion(val)} />
                                </div>
                            </Form.Group>
                        </Col>
                    </Form.Row >
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderLatitudeToolTip}>
                                    <Form.Label htmlFor="Latitude">Latitude:</Form.Label >
                                </OverlayTrigger>
                                <Form.Control type="text" name="latitude" value={latitude} onChange={(val) => handleLatitudeChanged(val.target.value)} />
                                <span style={{ color: "red" }}>{latitudeErrors}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderLongitudeToolTip}>
                                    <Form.Label htmlFor="Longitude">Longitude:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="longitude" value={longitude} onChange={(val) => handleLongitudeChanged(val.target.value)} />
                                <span style={{ color: "red" }}>{longitudeErrors}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderMaxNumberOfParticipantsToolTip}>
                                    <Form.Label className="control-label col-xs-2" htmlFor="MaxNumberOfParticipants">Max Number Of Participants:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="maxNumberOfParticipants" defaultValue={maxNumberOfParticipants} onChange={(val) => handleMaxNumberOfParticipantsChanged(val.target.value)} />
                            </Form.Group>
                        </Col>
                    </Form.Row >
                    <Form.Row>
                        <Form.Group>
                            <Button type="submit" className="btn btn-default">Save</Button>
                            <Button className="action" onClick={(e: any) => handleCancel(e)}>Cancel</Button>
                        </Form.Group>
                    </Form.Row >
                    <label>Click on the map to set the location for your event. The location fields above will be automatically populated.</label>
                    <div>
                        <AzureMapsProvider>
                            <>
                                <MapController center={center} multipleEvents={[]} isEventDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={eventName} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
                            </>
                        </AzureMapsProvider>
                    </div>
                </Form >
            </div >
        )
    }

    var contents = isDataLoaded
        ? renderCreateForm(eventTypeList)
        : <p><em>Loading...</em></p>;

    return (
        <div>
            <h3>Create a New Event</h3>
            <hr />
            {contents}
        </div>);
}

export default withRouter(CreateEvent);