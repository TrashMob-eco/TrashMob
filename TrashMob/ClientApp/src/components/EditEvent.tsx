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
import { apiConfig, defaultHeaders, msalClient } from '../store/AuthStore';

interface EditEventDataState {
    title: string;
    loading: boolean;
    eventId: string;
    eventData: EventData;
    typeList: EventTypeData[];
    eventDate: Date;
    country: string;
    region: string;
}

interface MatchParams {
    eventId: string;
}

export interface EditEventProps extends RouteComponentProps<MatchParams> {
}

export class EditEvent extends Component<EditEventProps, EditEventDataState> {
    constructor(props: EditEventProps) {
        super(props);
        this.state = {
            title: "", loading: true, eventData: new EventData(), eventId: Guid.create().toString(), typeList: [], eventDate: new Date(), country: '', region: ''
        };

        const headers = defaultHeaders('GET');

        fetch('api/eventtypes', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                this.setState({ typeList: data });
            });

        var eventId = this.props.match.params["eventId"];

        // This will set state for Edit Event  
        if (eventId != null) {
            fetch('api/Events/' + eventId, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<EventData>)
                .then(data => {
                    this.setState({ title: "Edit", loading: false, eventData: data, country: data.country, region: data.region, eventDate: new Date(data.eventDate), eventId: data.id });
                });
        }

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

    handleEventDateChange = (passedDate: Date) => {
        this.setState({ eventDate: passedDate });
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
        eventData.id = this.state.eventId;
        eventData.name = form.get("name")?.toString() ?? "";
        eventData.description = form.get("description")?.toString() ?? "";
        eventData.eventDate = new Date(this.state.eventDate);

        var user = getUserFromCache();

        eventData.eventTypeId = form.get("eventTypeId")?.valueOf() as number ?? 0;
        eventData.streetAddress = form.get("streetAddress")?.toString() ?? "";
        eventData.city = form.get("city")?.toString() ?? "";
        eventData.region = this.state.region ?? "";
        eventData.country = this.state.country ?? "";
        eventData.postalCode = form.get("postalCode")?.toString() ?? "";
        eventData.latitude = form.get("latitude") != null ? parseFloat(form.get("latitude")?.toString()) : 0;
        eventData.longitude = form.get("longitude") != null ? parseFloat(form.get("longitude")?.toString()) : 0;
        eventData.maxNumberOfParticipants = form.get("maxNumberOfParticipants")?.valueOf() as number ?? 0;
        eventData.createdByUserId = this.state.eventData.createdByUserId;
        eventData.lastUpdatedByUserId = user.id;
        eventData.eventStatusId = this.state.eventData.eventStatusId;

        var data = JSON.stringify(eventData);

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = defaultHeaders('PUT');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('api/Events', {
                method: 'PUT',
                body: data,
                headers: headers
            }).then((response) => response.json() as Promise<number>)
                .then(() => { this.props.history.push("/mydashboard"); })
        })
    }

    // This will handle Cancel button click event.  
    private handleCancel(event: any) {
        event.preventDefault();
        this.props.history.push("/mydashboard");
    }

    // Returns the HTML Form to the render() method.  
    private renderCreateForm(typeList: Array<EventTypeData>) {
        const { country, region } = this.state;
        const data = this.state;
        return (
            <form onSubmit={this.handleSave} >
                <div className="form-group row" >
                    <input type="hidden" name="Id" value={this.state.eventData.id.toString()} />
                </div>
                < div className="form-group row" >
                    <label className=" control-label col-md-12" htmlFor="Name">Name</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="name" defaultValue={this.state.eventData.name} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Description">Description</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="description" defaultValue={this.state.eventData.description} required />
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
                        <select className="form-control" data-val="true" name="eventTypeId" defaultValue={this.state.eventData.eventTypeId} required>
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
                        <input className="form-control" type="text" name="streetAddress" defaultValue={this.state.eventData.streetAddress} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="City">City</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="city" defaultValue={this.state.eventData.city} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Country">Country</label>
                    <div className="col-md-4">
                        <CountryDropdown name="country" value={country} onChange={(val) => this.selectCountry(val)} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Region">Region</label>
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
                        <input className="form-control" type="text" name="postalCode" defaultValue={this.state.eventData.postalCode} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Latitude">Latitude</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="latitude" defaultValue={this.state.eventData.latitude} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Longitude">Longitude</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="longitude" defaultValue={this.state.eventData.longitude} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="MaxNumberOfParticipants">Max Number Of Participants</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="maxNumberOfParticipants" defaultValue={this.state.eventData.maxNumberOfParticipants} />
                    </div>
                </div >
                <div className="form-group">
                    <button type="submit" className="btn btn-default">Save</button>
                    <button className="btn" onClick={this.handleCancel}>Cancel</button>
                </div >
                <div>
                    <SingleEventMap eventData={data.eventData} />
                </div>
            </form >
        )
    }
}
