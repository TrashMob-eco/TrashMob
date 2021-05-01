import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import EventData from './Models/EventData';
import UserData from './Models/UserData';
import EventTypeData from './Models/EventTypeData';
import { getDefaultHeaders } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';

export interface MatchParams {
    eventId: string;
}

export interface EventDetailsProps extends RouteComponentProps<MatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventDetails: React.FC<EventDetailsProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [eventId, setEventId] = React.useState<string>(props.match.params["eventId"]);
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
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);;
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();;
    const [eventList, setEventList] = React.useState<EventData[]>([]);;
    const [userList, setUserList] = React.useState<UserData[]>([]);;

    const headers = getDefaultHeaders('GET');

    fetch('api/eventtypes', {
        method: 'GET',
        headers: headers,
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            setEventTypeList(data);
        });

    if (eventId != null) {
        fetch('api/Events/' + eventId, {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<EventData>)
            .then(eventData => {
                setEventId(eventData.id);
                setEventName(eventData.name);
                setDescription(eventData.description);
                setEventDate(new Date(eventData.eventDate));
                setEventTypeId(eventData.eventTypeId);
                setStreetAddress(eventData.streetAddress);
                setCity(eventData.city);
                setCountry(eventData.country);
                setRegion(eventData.region);
                setPostalCode(eventData.postalCode);
                setLatitude(eventData.latitude);
                setLongitude(eventData.longitude);
                setMaxNumberOfParticipants(eventData.maxNumberOfParticipants);
                setCenter(new data.Position(eventData.longitude, eventData.latitude));
                setIsDataLoaded(false);
            });

        fetch('api/eventattendees/' + eventId, {
            method: 'GET',
        })
            .then(response => response.json() as Promise<UserData[]>)
            .then(data => {
                setUserList(data);
                setIsDataLoaded(true);
            });
    }

    MapStore.getOption().then(opts => {
        setMapOptions(opts);
        setIsMapKeyLoaded(true);
    })


    function handleLocationChange(point: data.Position) {
        // do nothing
    }

    function renderUsersTable(users: UserData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tabelLabel">
                    <thead>
                        <tr>
                            <th>First Name</th>
                            <th>City</th>
                            <th>Country</th>
                            <th>Member Since</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map(user =>
                            <tr key={user.id.toString()}>
                                <td>{user.givenName}</td>
                                <td>{user.city}</td>
                                <td>{user.country}</td>
                                <td>{new Date(user.memberSince).toLocaleDateString()}</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderEvent() {

        return (
            <div>
                <div className="form-group row" >
                    <input type="hidden" name="Id" value={eventId.toString()} />
                </div>
                < div className="form-group row" >
                    <label className="control-label col-xs-2" htmlFor="Name">Name:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{eventName}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="EventDate">EventDate:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{eventDate.toLocaleString()}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="EventType">Event Type:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{getEventType(eventTypeList, eventTypeId)}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-xs-2" htmlFor="Description">Description:</label>
                    <div className="col-md-10">
                        <textarea className="form-control" name="description" defaultValue={description} rows={5} cols={5} readOnly />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-xs-2" htmlFor="StreetAddress">Street Address:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{streetAddress}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="City">City:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{city}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="postalCode">Postal Code:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{postalCode}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-xs-2" htmlFor="stateProvince">Region:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{region}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="Country">Country:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{country}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-xs-2" htmlFor="Latitude">Latitude:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{latitude}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="Longitude">Longitude:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{longitude}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="MaxNumberOfParticipants">Max Number Of Participants:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{maxNumberOfParticipants}</label>
                    </div>
                </div >
                <div>
                    {renderUsersTable(userList)}
                </div>
                <div>
                    <AzureMapsProvider>
                        <>
                            <MapController center={center} multipleEvents={eventList} isDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={eventName} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUserId={props.currentUser.id} />
                        </>
                    </AzureMapsProvider>
                </div>
            </div >
        )
    }

    let contents = isDataLoaded
        ? renderEvent()
        : <p><em>Loading...</em></p>;

    return <div>
        <hr />
        {contents}
    </div>;
}
