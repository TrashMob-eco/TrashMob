import { Component } from 'react';
import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import { Guid } from "guid-typescript";
import EventData from './Models/EventData';
import SingleEventMap from './SingleEventMap';

interface EventDetailsDataState {
    title: string;
    loading: boolean;
    eventData: EventData;
    eventId: Guid;
    eventDate: string
}

export interface MatchParams {
    eventId: string;
}

export class EventDetails extends Component<RouteComponentProps<MatchParams>, EventDetailsDataState> {
    constructor(props: RouteComponentProps<MatchParams>) {
        super(props);
        this.state = {
            title: "", loading: true, eventData: new EventData(), eventId: Guid.create(), eventDate: new Date().toDateString()
        };

        var eventId = this.props.match.params["eventId"];

        if (eventId != null) {
            fetch('api/Events/' + eventId, {})
                .then(response => response.json() as Promise<EventData>)
                .then(data => {
                    this.setState({ title: "Event Details", loading: false, eventData: data, eventDate: new Date(data.eventDate).toDateString() });
                });
        }
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
                        <label className="form-control">{this.state.eventData.eventTypeId}</label>
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
                        <label className="form-control">{this.state.eventData.stateProvince}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Country">Country</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.country}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="ZipCode">Zip Code</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.zipCode}</label>
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
                    <label className="control-label col-md-12" htmlFor="GPSCoords">GPS Coords</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.gpscoords}</label>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="MaxNumberOfParticipants">Max Number Of Participants</label>
                    <div className="col-md-4">
                        <label className="form-control">{this.state.eventData.maxNumberOfParticipants}</label>
                    </div>
                </div >
                <div>
                    <SingleEventMap />
                </div>
            </div >

        )
    }
}
