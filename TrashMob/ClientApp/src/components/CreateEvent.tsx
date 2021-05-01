import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import { Guid } from "guid-typescript";
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
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';
import UserData from './Models/UserData';

interface CreateEventProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const CreateEvent: React.FC<CreateEventProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [eventId, setEventId] = React.useState<string>(Guid.create().toString());
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
    const [eventDateErrors, setEventDateErrors] = React.useState<string>();
    const [latitudeErrors, setLatitudeErrors] = React.useState<string>();
    const [longitudeErrors, setLongitudeErrors] = React.useState<string>();
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);;
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();;
    const [eventList, setEventList] = React.useState<EventData[]>([]);;

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

    React.useEffect(() => {
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

    function selectEventType(val: string) {
        setEventTypeId(parseInt(val));
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
                <form onSubmit={handleSave} >
                    <div className="form-group row" >
                        <input type="hidden" name="Id" value={eventId.toString()} />
                    </div>
                    < div className="form-group row" >
                        <label className=" control-label col-xs-2" htmlFor="Name">Name:</label>
                        <div className="col-md-4">
                            <input className="form-control" type="text" name="name" defaultValue={eventName} onChange={(val) => handleEventNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                        </div>
                        <label className="control-label col-xs-2" htmlFor="EventDate">EventDate:</label>
                        <div className="col-xs-2">
                            <DateTimePicker name="eventDate" onChange={handleEventDateChange} value={eventDate} />
                            <span style={{ color: "red" }}>{eventDateErrors}</span>
                        </div>
                        <label className="control-label col-xs-2" htmlFor="EventType">Event Type:</label>
                        <div className="col-xs-2">
                            <select className="form-control" data-val="true" name="eventTypeId" defaultValue={eventTypeId} onChange={(val) => selectEventType(val.target.value)} required>
                                <option value="">-- Select Event Type --</option>
                                {typeList.map(type =>
                                    <option key={type.id} value={type.id}>{type.name}</option>
                                )}
                            </select>
                        </div>
                    </div >
                    <div className="form-group row">
                        <label className="control-label col-md-12">Describe the event so attendees know what kind of gear to bring and where exactly to meet up.</label>
                        <label className="control-label col-xs-2" htmlFor="Description">Description:</label>
                        <div className="col-md-10">
                            <textarea className="form-control" name="description" defaultValue={description} onChange={(val) => handleDescriptionChanged(val.target.value)} maxLength={parseInt('2048')} rows={5} cols={5} required />
                        </div>
                    </div >
                    <div className="form-group row">
                        <label className="control-label col-xs-2" htmlFor="StreetAddress">Street Address:</label>
                        <div className="col-md-4">
                            <input className="form-control" type="text" name="streetAddress" defaultValue={streetAddress} onChange={(val) => handleStreetAddressChanged(val.target.value)} maxLength={parseInt('256')} />
                        </div>
                        <label className="control-label col-xs-2" htmlFor="City">City:</label>
                        <div className="col-xs-2">
                            <input className="form-control" type="text" name="city" defaultValue={city} onChange={(val) => handleCityChanged(val.target.value)} maxLength={parseInt('256')} required />
                        </div>
                        <label className="control-label col-xs-2" htmlFor="PostalCode">Postal Code:</label>
                        <div className="col-xs-2">
                            <input className="form-control" type="text" name="postalCode" defaultValue={postalCode} onChange={(val) => handlePostalCodeChanged(val.target.value)} maxLength={parseInt('25')} />
                        </div>
                    </div >
                    <div className="form-group row">
                        <label className="control-label col-md-12">Choose a country, and the list of states/provinces/regions will auto-fill</label>
                        <label className="control-label col-xs-2" htmlFor="Country">Country:</label>
                        <div className="col-xs-4">
                            <CountryDropdown name="country" value={country} onChange={(val) => selectCountry(val)} />
                        </div>
                        <label className="control-label col-xs-2" htmlFor="region">Region:</label>
                        <div className="col-xs-4">
                            <RegionDropdown
                                country={country}
                                value={region}
                                onChange={(val) => selectRegion(val)} />
                        </div>
                    </div >
                    <div className="form-group row">
                        <div>
                            <label className="control-label col-md-12">To set or change the latitude and longitude of an event, click the location on the map where you want attendees to meet, and the values will be updated. Don't foget to save your changes before leaving the page!</label>
                        </div>
                        <label className="control-label col-xs-2" htmlFor="Latitude">Latitude:</label>
                        <div className="col-xs-2">
                            <input className="form-control" type="text" name="latitude" value={latitude} onChange={(val) => handleLatitudeChanged(val.target.value)} />
                            <span style={{ color: "red" }}>{latitudeErrors}</span>
                        </div>
                        <label className="control-label col-xs-2" htmlFor="Longitude">Longitude:</label>
                        <div className="col-xs-2">
                            <input className="form-control" type="text" name="longitude" value={longitude} onChange={(val) => handleLongitudeChanged(val.target.value)} />
                            <span style={{ color: "red" }}>{longitudeErrors}</span>
                        </div>
                        <label className="control-label col-xs-2" htmlFor="MaxNumberOfParticipants">Max Number Of Participants:</label>
                        <div className="col-xs-2">
                            <input className="form-control" type="text" name="maxNumberOfParticipants" defaultValue={maxNumberOfParticipants} onChange={(val) => handleMaxNumberOfParticipantsChanged(val.target.value)} />
                        </div>
                    </div >
                    <div className="form-group">
                        <button type="submit" className="btn btn-default">Save</button>
                        <button className="btn" onClick={(e) => handleCancel(e)}>Cancel</button>
                    </div >
                    <div>
                        <AzureMapsProvider>
                            <>
                                <MapController center={center} multipleEvents={eventList} isDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={eventName} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUserId={props.currentUser.id} />
                            </>
                        </AzureMapsProvider>
                    </div>
                </form >
            </div>
        )
    }

    var contents = isDataLoaded
        ? renderCreateForm(eventTypeList)
        : <p><em>Loading...</em></p>;

    return (
        <div>
            <h3>Event</h3>
            <hr />
            {contents}
        </div>);
}

export default withRouter(CreateEvent);