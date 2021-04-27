import { Component } from 'react';
import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import { Guid } from "guid-typescript";
import EventData from './Models/EventData';
import DateTimePicker from 'react-datetime-picker';
import { getUserFromCache } from '../store/accountHandler';
import { getKey } from '../store/MapStore';
import EventTypeData from './Models/EventTypeData';
import { CountryDropdown, RegionDropdown } from 'react-country-region-selector';
import { apiConfig, defaultHeaders, msalClient } from '../store/AuthStore';
import { data } from 'azure-maps-control';
import AddressData from './Models/AddressData';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';

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
    center: data.Position;
    isKeyLoaded: boolean;
    mapOptions: IAzureMapOptions;
    eventList: EventData[];
    currentUserId: string;
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
            longitudeErrors: '',
            center: new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude),
            isKeyLoaded: false,
            mapOptions: null,
            eventList: [],
            currentUserId: getUserFromCache().id
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
                .then(eventData => {
                    this.setState({
                        title: "Edit",
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
        }

        // This binding is necessary to make "this" work in the callback  
        this.handleSave = this.handleSave.bind(this);
        this.handleCancel = this.handleCancel.bind(this);

        MapStore.getOption().then(opts => {
            this.setState({ mapOptions: opts });
            this.setState({ isKeyLoaded: true });
        })
    }

    componentDidMount() {
        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                var point = new data.Position(position.coords.longitude, position.coords.latitude);
                this.setState({ center: point })
            });
        } else {
            console.log("Not Available");
        }
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
        try {
            var floatVal = parseFloat(val);

            if (floatVal < -90 || floatVal > 90) {
                this.setState({ latitudeErrors: "Latitude must be => -90 and <= 90" });
            }
            else {
                this.setState({ latitude: floatVal });
                this.setState({ latitudeErrors: "" });
            }
        }
        catch { }
    }

    handleLongitudeChanged = (val: string) => {
        try {
            var floatVal = parseFloat(val);

            if (floatVal < -180 || floatVal > 180) {
                this.setState({ longitudeErrors: "Longitude must be >= -180 and <= 180" });
            }
            else {
                this.setState({ longitude: floatVal });
                this.setState({ longitudeErrors: "" });
            }
        }
        catch { }
    }

    selectEventType(val: string) {
        this.setState({ eventTypeId: parseInt(val) });
    }

    handleLocationChange = (point: data.Position) => {
        // In an Azure Map point, the longitude is the first position, and latitude is second
        this.setState({ latitude: point[1] });
        this.setState({ longitude: point[0] });
        var locationString = point[1] + ',' + point[0]
        var headers = defaultHeaders('GET');

        getKey()
            .then(key => {
                fetch('https://atlas.microsoft.com/search/address/reverse/json?subscription-key=' + key + '&api-version=1.0&query=' + locationString, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<AddressData>)
                    .then(data => {
                        this.setState({ streetAddress: data.addresses[0].address.streetNameAndNumber });
                        this.setState({ city: data.addresses[0].address.municipality });
                        this.setState({ country: data.addresses[0].address.country });
                        this.setState({ region: data.addresses[0].address.countrySubdivisionName });
                        this.setState({ postalCode: data.addresses[0].address.postalCode });
                    })
            })
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
        const { country, region, eventName, longitude, latitude } = this.state;
        return (
            <div className="container-fluid">
                <form onSubmit={this.handleSave} >
                    <div className="form-group row" >
                        <input type="hidden" name="Id" value={this.state.eventId.toString()} />
                    </div>
                    < div className="form-group row" >
                        <label className=" control-label col-xs-2" htmlFor="Name">Name:</label>
                        <div className="col-xs-2">
                            <input className="form-control" type="text" name="name" defaultValue={this.state.eventName} onChange={(val) => this.handleEventNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                        </div>
                        <label className="control-label col-xs-2" htmlFor="EventDate">EventDate:</label>
                        <div className="col-xs-2">
                            <DateTimePicker name="eventDate" onChange={this.handleEventDateChange} value={this.state.eventDate} />
                            <span style={{ color: "red" }}>{this.state.eventDateErrors}</span>
                        </div>
                        <label className="control-label col-xs-2" htmlFor="EventType">Event Type:</label>
                        <div className="col-xs-2">
                            <select className="form-control" data-val="true" name="eventTypeId" defaultValue={this.state.eventTypeId} required>
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
                            <textarea className="form-control" name="description" defaultValue={this.state.description} onChange={(val) => this.handleDescriptionChanged(val.target.value)} maxLength={parseInt('2048')} rows={5} cols={5} required />
                        </div>
                    </div >
                    <div className="form-group row">
                        <label className="control-label col-xs-2" htmlFor="StreetAddress">Street Address:</label>
                        <div className="col-md-4">
                            <input className="form-control" type="text" name="streetAddress" value={this.state.streetAddress} onChange={(val) => this.handleStreetAddressChanged(val.target.value)} maxLength={parseInt('256')} />
                        </div>
                        <label className="control-label col-xs-2" htmlFor="City">City:</label>
                        <div className="col-xs-2">
                            <input className="form-control" type="text" name="city" value={this.state.city} onChange={(val) => this.handleCityChanged(val.target.value)} maxLength={parseInt('256')} required />
                        </div>
                        <label className="control-label col-xs-2" htmlFor="PostalCode">Postal Code:</label>
                        <div className="col-xs-2">
                            <input className="form-control" type="text" name="postalCode" value={this.state.postalCode} onChange={(val) => this.handlePostalCodeChanged(val.target.value)} maxLength={parseInt('25')} />
                        </div>
                    </div >
                    <div className="form-group row">
                        <label className="control-label col-md-12">Choose a country, and the list of states/provinces/regions will auto-fill</label>
                        <label className="control-label col-xs-2" htmlFor="Country">Country:</label>
                        <div className="col-xs-2">
                            <CountryDropdown name="country" value={country} onChange={(val) => this.selectCountry(val)} />
                        </div>
                        <label className="control-label col-xs-2" htmlFor="Region">Region:</label>
                        <div className="col-xs-2">
                            <RegionDropdown
                                country={country}
                                value={region}
                                onChange={(val) => this.selectRegion(val)} />
                        </div>
                    </div >
                    <div className="form-group row">
                        <div>
                            <label className="control-label col-md-12">To set or change the latitude and longitude of an event, click the location on the map where you want attendees to meet, and the values will be updated. Don't foget to save your changes before leaving the page!</label>
                        </div>
                        <label className="control-label col-xs-2" htmlFor="Latitude">Latitude:</label>
                        <div className="col-xs-2">
                            <input className="form-control" type="text" name="latitude" value={this.state.latitude} onChange={(val) => this.handleLatitudeChanged(val.target.value)} />
                            <span style={{ color: "red" }}>{this.state.latitudeErrors}</span>
                        </div>
                        <label className="control-label col-xs-2" htmlFor="Longitude">Longitude:</label>
                        <div className="col-xs-2">
                            <input className="form-control" type="text" name="longitude" value={this.state.longitude} onChange={(val) => this.handleLongitudeChanged(val.target.value)} />
                            <span style={{ color: "red" }}>{this.state.longitudeErrors}</span>
                        </div>
                        <label className="control-label col-xs-2" htmlFor="MaxNumberOfParticipants">Max Number Of Participants:</label>
                        <div className="col-xs-2">
                            <input className="form-control" type="text" name="maxNumberOfParticipants" defaultValue={this.state.maxNumberOfParticipants} onChange={(val) => this.handleMaxNumberOfParticipantsChanged(val.target.value)} />
                        </div>
                    </div >
                    <div className="form-group">
                        <button type="submit" className="action btn-default">Save</button>
                        <button className="action" onClick={(e) => this.handleCancel(e)}>Cancel</button>
                    </div >
                    <div>
                        <AzureMapsProvider>
                            <>
                                <MapController center={this.state.center} multipleEvents={this.state.eventList} loading={this.state.loading} mapOptions={this.state.mapOptions} isKeyLoaded={this.state.isKeyLoaded} eventName={eventName} latitude={latitude} longitude={longitude} onLocationChange={this.handleLocationChange} currentUserId={this.state.currentUserId} />
                            </>
                        </AzureMapsProvider>
                    </div>
                </form >
            </div>
        )
    }
}
