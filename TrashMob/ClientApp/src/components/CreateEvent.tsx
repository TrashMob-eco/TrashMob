import { Component } from 'react';
import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import { Guid } from "guid-typescript";
import EventData from './Models/EventData';
import DateTimePicker from 'react-datetime-picker';
import { getUserFromCache } from '../store/accountHandler';
import EventTypeData from './Models/EventTypeData';
import { CountryDropdown, RegionDropdown } from 'react-country-region-selector';
import SingleEventMap from './SingleEventMap';
import { withRouter } from 'react-router-dom';
import { apiConfig, defaultHeaders, msalClient } from '../store/AuthStore';
import { data } from 'azure-maps-control';

interface CreateEventDataState {
    title: string;
    loading: boolean;
    eventData: EventData;
    typeList: EventTypeData[];
    eventId: Guid;
    eventName: string;
    eventDate: Date;
    country: string;
    region: string;
    latitude: number;
    longitude: number;
}

interface Props extends RouteComponentProps<any> {
}

class CreateEvent extends Component<Props, CreateEventDataState> {
    constructor(props: Props) {
        super(props);
        this.state = {
            title: "", loading: true, eventData: new EventData(), eventId: Guid.create(), eventName: "New Event", typeList: [], eventDate: new Date(), country: '', region: '', latitude: 0, longitude: 0
        };

        const headers = defaultHeaders('GET');

        fetch('api/eventtypes', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                this.setState({ typeList: data });
            });

        this.state = { title: "Create", loading: false, eventData: new EventData(), eventId: Guid.create(), eventName: "New Event", typeList: [], eventDate: new Date(), country: '', region: '', latitude: 0, longitude: 0 };

        // This binding is necessary to make "this" work in the callback  
        this.handleSave = this.handleSave.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
    }

    selectCountry(val: string) {
        this.setState({ country: val });
    }

    selectRegion(val: string) {
        this.setState({ region: val });
    }

    handleLocationChange = (point: data.Position) => {
        this.setState({ latitude: point[0] });
        this.setState({ longitude: point[1] });
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderCreateForm(this.state.typeList);

        return <div>
            <h1>{this.state.title}</h1>
            <h3>Event</h3>
            <hr />
            {contents}
        </div>;
    }

    // This will handle the submit form event.  
    private handleSave(event: any) {
        event.preventDefault();

        const form = new FormData(event.target);

        var eventData = new EventData();
        eventData.name = this.state.eventName ?? "";
        eventData.description = form.get("description")?.toString() ?? "";
        eventData.eventDate = new Date(this.state.eventDate);
        var user = getUserFromCache();

        eventData.eventTypeId = form.get("eventTypeId")?.valueOf() as number ?? 0;
        eventData.streetAddress = form.get("streetAddress")?.toString() ?? "";
        eventData.city = form.get("city")?.toString() ?? "";
        eventData.region = this.state.region ?? "";
        eventData.country = this.state.country ?? "";
        eventData.postalCode = form.get("postalCode")?.toString() ?? "";
        eventData.latitude = this.state.latitude ?? 0;
        eventData.longitude = this.state.longitude ?? 0;
        eventData.maxNumberOfParticipants = form.get("maxNumberOfParticipants")?.valueOf() as number ?? 0;
        eventData.createdByUserId = user.id;
        eventData.lastUpdatedByUserId = user.id;

        var data = JSON.stringify(eventData);

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {

            const headers = defaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('api/Events', {
                method: 'POST',
                headers: headers,
                body: data,
            }).then((response) => response.json())
                .then(() => {
                    this.props.history.push("/mydashboard");
                })
        })
    }

    // This will handle Cancel button click event.  
    private handleCancel(event: any) {
        event.preventDefault();
        this.props.history.push("/mydashboard");
    }

    handleEventDateChange = (passedDate: Date) => {
        this.setState({ eventDate: passedDate });
    }

    // Returns the HTML Form to the render() method.  
    private renderCreateForm(typeList: Array<EventTypeData>) {
        const { country, region, loading, eventName, latitude, longitude } = this.state;
        return (
            <form onSubmit={this.handleSave} >
                <div className="form-group row" >
                    <input type="hidden" name="Id" value={this.state.eventData.id.toString()} />
                </div>
                < div className="form-group row" >
                    <label className=" control-label col-md-12" htmlFor="Name">Name</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="name" value={this.state.eventName} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Description">Description</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="description" value={this.state.eventData.description} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventDate">EventDate</label>
                    <div className="col-md-4">
                        <DateTimePicker name="eventDate" onChange={this.handleEventDateChange} value={this.state.eventDate} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventType">Event Type</label>
                    <div className="col-md-4">
                        <select className="form-control" data-val="true" name="eventTypeId" value={this.state.eventData.eventTypeId} required>
                            <option value="">-- Select Event Type --</option>
                            {typeList.map(type =>
                                <option key={type.id} value={type.id}>{type.name}</option>
                            )}
                        </select>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="StreetAddress">StreetAddress</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="streetAddress" value={this.state.eventData.streetAddress} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="City">City</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="city" value={this.state.eventData.city} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Country">Country</label>
                    <div className="col-md-4">
                        <CountryDropdown name="country" value={country} onChange={(val) => this.selectCountry(val)} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="region">Region</label>
                    <div className="col-md-4">
                        <RegionDropdown
                            country={country}
                            value={region}
                            onChange={(val) => this.selectRegion(val)} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="PostalCode">Postal Code</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="postalCode" value={this.state.eventData.postalCode} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Latitude">Latitude</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="latitude" value={this.state.latitude} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Longitude">Longitude</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="longitude" value={this.state.longitude} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="MaxNumberOfParticipants">Max Number Of Participants</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="maxNumberOfParticipants" value={this.state.eventData.maxNumberOfParticipants} />
                    </div>
                </div >
                <div className="form-group">
                    <button type="submit" className="btn btn-default">Save</button>
                    <button className="btn" onClick={(e) => this.handleCancel(e)}>Cancel</button>
                </div >
                <div>
                    <SingleEventMap eventName={eventName} latitude={latitude} longitude={longitude} loading={loading} onLocationChange={this.handleLocationChange} />
                </div>
            </form >
        )
    }
}

export default withRouter(CreateEvent);