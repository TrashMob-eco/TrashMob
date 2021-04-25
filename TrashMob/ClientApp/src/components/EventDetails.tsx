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
    eventData: EventData;
    eventTypeList: EventTypeData[];
    userList: UserData[];
    eventId: Guid;
    eventDate: string
    center: data.Position;
    isKeyLoaded: boolean;
    mapOptions: IAzureMapOptions;
    eventList: EventData[];
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
            eventData: new EventData(),
            eventId: Guid.create(),
            eventDate: new Date().toDateString(),
            userList: [],
            eventTypeList: [],
            center: new data.Position(MapStore.defaultLatitude, MapStore.defaultLongitude),
            isKeyLoaded: false,
            mapOptions: null,
            eventList: []
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
                    this.setState({ title: "Event Details", loading: false, eventData: eventData, eventDate: new Date(eventData.eventDate).toDateString(), center: new data.Position(eventData.longitude, eventData.latitude)});
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
                                <td>{user.memberSince}</td>
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
                <div>
                    <button onClick={() => this.props.history.goBack}>Go Back</button>
                </div>
                <div className="form-group row" >
                    <input type="hidden" name="Id" value={this.state.eventData.id.toString()} />
                </div>
                < div className="form-group row" >
                    <label className="control-label col-md-12" htmlFor="Name">Name</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.name}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Description">Description</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.description}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventDate">EventDate</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventDate}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventType">Event Type</label>
                    <div className="col-md-4">
                        <label className="form-control">{getEventType(this.state.eventTypeList, this.state.eventData.eventTypeId)}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="StreetAddress">StreetAddress</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.streetAddress}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="City">City</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.city}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="stateProvince">State / Province</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.region}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Country">Country</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.country}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="postalCode">Postal Code</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.postalCode}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Latitude">Latitude</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.latitude}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Longitude">Longitude</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.longitude}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="MaxNumberOfParticipants">Max Number Of Participants</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.maxNumberOfParticipants}</label>
                    </div>
                </div >
                <div>
                    {this.renderUsersTable(this.state.userList)}
                </div>
                <div>
                    <button onClick={() => this.props.history.goBack}>Go Back</button>
                </div>
                <div>
                    <AzureMapsProvider>
                        <>
                            <MapController center={this.state.center} multipleEvents={this.state.eventList} loading={this.state.loading} mapOptions={this.state.mapOptions} isKeyLoaded={this.state.isKeyLoaded} eventName={this.state.eventData.name} latitude={this.state.eventData.latitude} longitude={this.state.eventData.longitude} onLocationChange={this.handleLocationChange} />
                        </>
                    </AzureMapsProvider>
                </div>
            </div >
        )
    }
}
