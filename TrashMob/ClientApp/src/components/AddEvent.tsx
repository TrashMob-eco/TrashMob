import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Guid } from "guid-typescript";
import { EventData } from './FetchEvent';  
import authService from './api-authorization/AuthorizeService'

interface AddEventDataState {
    title: string;
    loading: boolean;
    eventData: EventData;
    eventId: Guid;
}

interface MatchParams {
    eventId: string;
}

export class AddEvent extends React.Component<RouteComponentProps<MatchParams>, AddEventDataState> {
    constructor(props: RouteComponentProps<MatchParams>) {
        super(props);
        const token = authService.getAccessToken();
        this.state = { title: "", loading: true, eventData: new EventData(), eventId: Guid.create() };

        var eventId = this.props.match.params["eventId"];

        // This will set state for Edit Event  
        if (eventId != null) {
            fetch('api/Events/' + eventId, {
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            })
                .then(response => response.json() as Promise<EventData>)
                .then(data => {
                    this.setState({ title: "Edit", loading: false, eventData: data });
                });
        }

        // This will set state for Add Event  
        else {
            this.state = { title: "Create", loading: false, eventData: new EventData(), eventId: Guid.create() };
        }

        // This binding is necessary to make "this" work in the callback  
        this.handleSave = this.handleSave.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
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

    // This will handle the submit form event.  
    private handleSave(event: any) {
        event.preventDefault();
        const data = new FormData(event.target);
        const token = authService.getAccessToken();

        // PUT request for Edit Event.  
        if (this.state.eventData.Id) {
            fetch('api/Events', {
                method: 'PUT',
                body: data,
                headers: {
                    Allow: 'POST',
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
            }).then((response) => response.json())
                .then((responseJson) => {
                    this.props.history.push("/fetchEvent");
                })
        }

        // POST request for Add Event.  
        else {
            fetch('api/Events', {
                method: 'POST',
                body: data,
                headers: {
                    Allow: 'POST',
                    Accept: 'application/json, text/plain',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
            }).then((response) => response.json())
                .then((responseJson) => {
                    this.props.history.push("/fetchEvent");
                })
        }
    }

    // This will handle Cancel button click event.  
    private handleCancel(event: any) {
        event.preventDefault();
        this.props.history.push("/fetchEvent");
    }

    // Returns the HTML Form to the render() method.  
    private renderCreateForm() {
        return (
            <form onSubmit={this.handleSave} >
                <div className="form-group row" >
                    <input type="hidden" name="Id" value={this.state.eventData.Id.toString()} />
                </div>
                < div className="form-group row" >
                    <label className=" control-label col-md-12" htmlFor="Name">Name</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="name" defaultValue={this.state.eventData.name} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventDate">EventDate</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="eventDate" required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Description">Description</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="description" defaultValue={this.state.eventData.description} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="ContactPhone">Contact Phone</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="contactPhone" defaultValue={this.state.eventData.contactPhone} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Country">Country</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="counntry" defaultValue={this.state.eventData.country} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Address">Address</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="address" defaultValue={this.state.eventData.address} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Latitude">Latitude</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="latitude" defaultValue={this.state.eventData.latitude} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Longitude">Longitude</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="longitude" defaultValue={this.state.eventData.longitude} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="MaxNumberOfParticipants">Max Number Of Participants</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="maxNumberOfParticipants" defaultValue={this.state.eventData.maxNumberOfParticipants} required />
                    </div>
                </div >

                <div className="form-group">
                    <button type="submit" className="btn btn-default">Save</button>
                    <button className="btn" onClick={this.handleCancel}>Cancel</button>
                </div >
            </form >
        )
    }
}