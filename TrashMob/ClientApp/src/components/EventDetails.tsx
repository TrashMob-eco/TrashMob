import { Component } from 'react';
import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import { Guid } from "guid-typescript";
import EventData from './Models/EventData';  

interface EventDetailsDataState {
    title: string;
    loading: boolean;
    eventData: EventData;
    eventId: Guid;
}

export interface MatchParams {
    eventId: string;
}

export class EventDetails extends Component<RouteComponentProps<MatchParams>, EventDetailsDataState> {
    constructor(props: RouteComponentProps<MatchParams>) {
        super(props);
        this.state = {
            title: "", loading: true, eventData: new EventData(), eventId: Guid.create()
        };

        var eventId = this.props.match.params["eventId"];

        if (eventId != null) {
            fetch('api/Events/' + eventId, {})
                .then(response => response.json() as Promise<EventData>)
                .then(data => {
                    this.setState({ title: "Edit", loading: false, eventData: data });
                });
        }
        else {
            this.state = { title: "Details", loading: false, eventData: new EventData(), eventId: Guid.create() };
        }
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderCreateForm();

        return <div>
            <h1>{this.state.title}</h1>
            <h3>Event</h3>
            <hr />
            {contents}
        </div>;
    }

    // Returns the HTML Form to the render() method.  
    private renderCreateForm() {
        return (
            <div>
                <div className="form-group row" >
                    <input type="hidden" name="Id" value={this.state.eventData.id.toString()} />
                </div>
                < div className="form-group row" >
                    <label className="control-label col-md-12" htmlFor="Name">Name</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.name} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Description">Description</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.description} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventDate">EventDate</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.eventDate?.toDateString()} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventType">Event Type</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.eventTypeId} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="StreetAddress">StreetAddress</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.streetAddress} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="City">City</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.city} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="stateProvince">State / Province</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.stateProvince} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Country">Country</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.country} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="ZipCode">Zip Code</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.zipCode} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Latitude">Latitude</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.latitude} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Longitude">Longitude</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.longitude} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="GPSCoords">GPS Coords</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.gpscoords} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="MaxNumberOfParticipants">Max Number Of Participants</label>
                    <div className="col-md-4">
                        <label className="form-control" defaultValue={this.state.eventData.maxNumberOfParticipants} />
                    </div>
                </div >
            </div >
        )
    }
}
