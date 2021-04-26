import { Component } from 'react';
import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import { Guid } from "guid-typescript";
import EventData from './Models/EventData';
import UserData from './Models/UserData';
import EventTypeData from './Models/EventTypeData';
import { defaultHeaders } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';

export interface EventDetailsDataState {
    title: string;
    loading: boolean;
    eventId: string;
    eventName: string;
    description: string;
    eventDate: Date;
    eventTypeId: number;
    streetAddress: string;
    city: string;
    country: string;
    region: string;
    postalCode: string;
    latitude: number;
    longitude: number;
    maxNumberOfParticipants: number;
    createdByUserId: string;
    eventStatusId: number;
    typeList: EventTypeData[];
    eventDateErrors: string;
    latitudeErrors: string;
    longitudeErrors: string;
    center: data.Position;
    isKeyLoaded: boolean;
    mapOptions: IAzureMapOptions;
    eventList: EventData[];
    eventTypeList: EventTypeData[];
    userList: UserData[];
}

export interface MatchParams {
    eventId: string;
}

export class EventDetails extends Component<RouteComponentProps<MatchParams>, EventDetailsDataState> {
    constructor(props: RouteComponentProps<MatchParams>) {
        super(props);
        this.state = {
            title: "",
            loading: true,
            eventId: Guid.create().toString(),
            eventName: "",
            description: "",
            eventDate: new Date(),
            eventTypeId: 0,
            streetAddress: '',
            city: '',
            country: '',
            region: '',
            postalCode: '',
            latitude: 0,
            longitude: 0,
            maxNumberOfParticipants: 0,
            createdByUserId: '',
            eventStatusId: 0,
            typeList: [],
            eventDateErrors: '',
            latitudeErrors: '',
            longitudeErrors: '',
            center: new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude),
            isKeyLoaded: false,
            mapOptions: null,
            eventList: [],
            eventTypeList: [],
            userList: []
        };

        const headers = defaultHeaders('GET');

        fetch('api/eventtypes', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                this.setState({ eventTypeList: data });
            });

        var eventId = this.props.match.params["eventId"];

        if (eventId != null) {
            fetch('api/Events/' + eventId, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<EventData>)
                .then(eventData => {
                    this.setState({
                        title: "Event Details",
                        loading: false,
                        eventId: eventData.id,
                        eventName: eventData.name,
                        description: eventData.description,
                        eventDate: new Date(eventData.eventDate),
                        eventTypeId: eventData.eventTypeId,
                        streetAddress: eventData.streetAddress,
                        city: eventData.city,
                        country: eventData.country,
                        region: eventData.region,
                        postalCode: eventData.postalCode,
                        latitude: eventData.latitude,
                        longitude: eventData.longitude,
                        maxNumberOfParticipants: eventData.maxNumberOfParticipants,
                        createdByUserId: eventData.createdByUserId,
                        eventStatusId: eventData.eventStatusId,
                        center: new data.Position(eventData.longitude, eventData.latitude)
                    });
                });

            fetch('api/eventattendees/' + eventId, {
                method: 'GET',
            })
                .then(response => response.json() as Promise<UserData[]>)
                .then(data => {
                    this.setState({ userList: data, loading: false });
                });
        }

        MapStore.getOption().then(opts => {
            this.setState({ mapOptions: opts });
            this.setState({ isKeyLoaded: true });
        })
    }

    handleLocationChange = (point: data.Position) => {
    }

    private renderUsersTable(users: UserData[]) {
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

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderEvent();

        return <div>
            <h1>{this.state.title}</h1>
            <hr />
            {contents}
        </div>;
    }

    private renderEvent() {

        return (
            <div>
                <div className="form-group row" >
                    <input type="hidden" name="Id" value={this.state.eventId.toString()} />
                </div>
                < div className="form-group row" >
                    <label className="control-label col-xs-2" htmlFor="Name">Name:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{this.state.eventName}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="EventDate">EventDate:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{this.state.eventDate.toLocaleString()}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="EventType">Event Type:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{getEventType(this.state.eventTypeList, this.state.eventTypeId)}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-xs-2" htmlFor="Description">Description:</label>
                    <div className="col-md-10">
                        <textarea className="form-control" name="description" defaultValue={this.state.description} rows={5} cols={5} readOnly />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-xs-2" htmlFor="StreetAddress">Street Address:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{this.state.streetAddress}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="City">City:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{this.state.city}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="postalCode">Postal Code:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{this.state.postalCode}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-xs-2" htmlFor="stateProvince">Region:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{this.state.region}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="Country">Country:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{this.state.country}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-xs-2" htmlFor="Latitude">Latitude:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{this.state.latitude}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="Longitude">Longitude:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{this.state.longitude}</label>
                    </div>
                    <label className="control-label col-xs-2" htmlFor="MaxNumberOfParticipants">Max Number Of Participants:</label>
                    <div className="col-xs-2">
                        <label className="form-control">{this.state.maxNumberOfParticipants}</label>
                    </div>
                </div >
                <div>
                    {this.renderUsersTable(this.state.userList)}
                </div>
                <div>
                    <AzureMapsProvider>
                        <>
                            <MapController center={this.state.center} multipleEvents={this.state.eventList} loading={this.state.loading} mapOptions={this.state.mapOptions} isKeyLoaded={this.state.isKeyLoaded} eventName={this.state.eventName} latitude={this.state.latitude} longitude={this.state.longitude} onLocationChange={this.handleLocationChange} />
                        </>
                    </AzureMapsProvider>
                </div>
            </div >
        )
    }
}
