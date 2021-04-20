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
import { data } from 'azure-maps-control';

interface EditEventDataState {
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
            title: "Edit",
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
            longitudeErrors: ''

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
                    this.setState({
                        title: "Edit",
                        loading: false,
                        eventId: data.id,
                        eventName: data.name,
                        description: data.description,
                        eventDate: new Date(data.eventDate),
                        eventTypeId: data.eventTypeId,
                        streetAddress: data.streetAddress,
                        city: data.city,
                        country: data.country,
                        region: data.region,
                        postalCode: data.postalCode,
                        latitude: data.latitude,
                        longitude: data.longitude,
                        maxNumberOfParticipants: data.maxNumberOfParticipants,
                        createdByUserId: data.createdByUserId,
                        eventStatusId: data.eventStatusId,
                    });
                });
        }

        // This binding is necessary to make "this" work in the callback  
        this.handleSave = this.handleSave.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
    }

    handleEventNameChanged = (val: string) => {
        this.setState({ eventName: val });
    }

    handleDescriptionChanged = (val: string) => {
        this.setState({ description: val });
    }

    handleStreetAddressChanged = (val: string) => {
        this.setState({ streetAddress: val });
    }

    handleCityChanged = (val: string) => {
        this.setState({ city: val });
    }

    selectCountry(val: string) {
        this.setState({ country: val });
    }

    selectRegion(val: string) {
        this.setState({ region: val });
    }

    handlePostalCodeChanged = (val: string) => {
        this.setState({ postalCode: val });
    }

    handleMaxNumberOfParticipantsChanged = (val: string) => {
        this.setState({ maxNumberOfParticipants: parseInt(val) });
    }

    handleLatitudeChanged = (val: string) => {
        var floatVal = parseFloat(val);

        if (floatVal < -90 || floatVal > 90) {
            this.setState({ latitudeErrors: "Latitude must be => -90 and <= 90" });
        }
        else {
            this.setState({ latitude: floatVal });
            this.setState({ latitudeErrors: "" });
        }
    }

    handleLongitudeChanged = (val: string) => {
        var floatVal = parseFloat(val);

        if (floatVal < -180 || floatVal > 180) {
            this.setState({ longitudeErrors: "Longitude must be >= -180 and <= 180" });
        }
        else {
            this.setState({ longitude: floatVal });
            this.setState({ longitudeErrors: "" });
        }
    }

    selectEventType(val: string) {
        this.setState({ eventTypeId: parseInt(val) });
    }

    handleLocationChange = (point: data.Position) => {
        this.setState({ latitude: point[0] });
        this.setState({ longitude: point[1] });
    }

    handleEventDateChange = (passedDate: Date) => {
        if (passedDate < new Date()) {
            this.setState({ eventDateErrors: "Event cannot be in the past" });
        }
        else {
            this.setState({ eventDateErrors: "" });
        }

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

        if (this.state.eventDateErrors !== "") {
            return;
        }

        var eventData = new EventData();
        eventData.id = this.state.eventId;
        eventData.name = this.state.eventName ?? "";
        eventData.description = this.state.description ?? "";
        eventData.eventDate = new Date(this.state.eventDate);
        eventData.eventTypeId = this.state.eventTypeId ?? 0;
        eventData.streetAddress = this.state.streetAddress ?? "";
        eventData.city = this.state.city ?? "";
        eventData.region = this.state.region ?? "";
        eventData.country = this.state.country ?? "";
        eventData.postalCode = this.state.postalCode ?? "";
        eventData.latitude = this.state.latitude ?? 0;
        eventData.longitude = this.state.longitude ?? 0;
        eventData.maxNumberOfParticipants = this.state.maxNumberOfParticipants ?? 0;
        eventData.createdByUserId = this.state.createdByUserId;
        eventData.eventStatusId = this.state.eventStatusId;

        var user = getUserFromCache();
        eventData.lastUpdatedByUserId = user.id;

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
        const { country, region, eventName, loading, longitude, latitude } = this.state;
        return (
            <form onSubmit={this.handleSave} >
                <div className="form-group row" >
                    <input type="hidden" name="Id" value={this.state.eventId.toString()} />
                </div>
                < div className="form-group row" >
                    <label className=" control-label col-md-12" htmlFor="Name">Name</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="name" defaultValue={this.state.eventName} onChange={(val) => this.handleEventNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Description">Description</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="description" defaultValue={this.state.description} onChange={(val) => this.handleDescriptionChanged(val.target.value)} maxLength={parseInt('2048')} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventDate">EventDate</label>
                    <div className="col-md-4">
                        <DateTimePicker name="eventDate" onChange={this.handleEventDateChange} value={this.state.eventDate} />
                        <span style={{ color: "red" }}>{this.state.eventDateErrors}</span>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventType">Event Type</label>
                    <div className="col-md-4">
                        <select className="form-control" data-val="true" name="eventTypeId" defaultValue={this.state.eventTypeId} required>
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
                        <input className="form-control" type="text" name="streetAddress" defaultValue={this.state.streetAddress} onChange={(val) => this.handleStreetAddressChanged(val.target.value)} maxLength={parseInt('256')} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="City">City</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="city" defaultValue={this.state.city} onChange={(val) => this.handleCityChanged(val.target.value)} maxLength={parseInt('256')} required />
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
                        <input className="form-control" type="text" name="postalCode" defaultValue={this.state.postalCode} onChange={(val) => this.handlePostalCodeChanged(val.target.value)} maxLength={parseInt('25')} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Latitude">Latitude</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="latitude" defaultValue={this.state.latitude} onChange={(val) => this.handleLatitudeChanged(val.target.value)} />
                        <span style={{ color: "red" }}>{this.state.latitudeErrors}</span>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Longitude">Longitude</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="longitude" defaultValue={this.state.longitude} onChange={(val) => this.handleLongitudeChanged(val.target.value)} />
                        <span style={{ color: "red" }}>{this.state.longitudeErrors}</span>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="MaxNumberOfParticipants">Max Number Of Participants</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="maxNumberOfParticipants" defaultValue={this.state.maxNumberOfParticipants} onChange={(val) => this.handleMaxNumberOfParticipantsChanged(val.target.value)} />
                    </div>
                </div >
                <div className="form-group">
                    <button type="submit" className="btn btn-default">Save</button>
                    <button className="btn" onClick={(e) => this.handleCancel(e)}>Cancel</button>
                </div >
                <div>
                    To set or change the latitude and longitude of an event, click the location on the map where you want attendees to meet, and the values will be updated. Don't foget to save your changes before leaving the page!
                </div>
                <div>
                    <SingleEventMap eventName={eventName} loading={loading} latitude={latitude} longitude={longitude} onLocationChange={this.handleLocationChange} />
                </div>
            </form >
        )
    }
}
