import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Guid } from "guid-typescript";
import { EventData } from './FetchEvents';  
import DateTimePicker from 'react-datetime-picker';

interface AddEventDataState {
    title: string;
    loading: boolean;
    eventData: EventData;
    typeList: TypeData[];
    eventId: Guid;
    eventDate: Date;
}

interface MatchParams {
    eventId: string;
}

export class AddEvent extends React.Component<RouteComponentProps<MatchParams>, AddEventDataState> {
    constructor(props: RouteComponentProps<MatchParams>) {
        super(props);
        this.state = {
            title: "", loading: true, eventData: new EventData(), eventId: Guid.create(), typeList: [], eventDate: new Date()
        };

        fetch('api/eventtypes', {
            method: 'GET',
            headers: {
                Allow: 'GET',
                Accept: 'application/json',
                'Content-Type': 'application/json'
                },
            })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                this.setState({ typeList: data });
            });  

        var eventId = this.props.match.params["eventId"];

        // This will set state for Edit Event  
        if (eventId != null) {
            fetch('api/Events/' + eventId, {})
                .then(response => response.json() as Promise<EventData>)
                .then(data => {
                    this.setState({ title: "Edit", loading: false, eventData: data });
                });
        }

        // This will set state for Add Event  
        else {
            this.state = { title: "Create", loading: false, eventData: new EventData(), eventId: Guid.create(), typeList: [], eventDate: new Date() };
        }

        // This binding is necessary to make "this" work in the callback  
        this.handleSave = this.handleSave.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
    }

    handleEventDateChange = (eventDate: Date) => {
        this.setState(prevState => ({ eventData: { ...prevState.eventData, eventDate: eventDate }}));
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
    private handleSave(event : any) {
        event.preventDefault();

        Date.prototype.toJSON = function () { return this.toISOString(); }

        const jsonform = new FormData(event.target);
        const obj = Object.fromEntries(jsonform.entries());
        var data = JSON.stringify(obj);

        // PUT request for Edit Event.  
        if (this.state.eventData.id.toString() !== Guid.EMPTY) {
            fetch('api/Events', {
                method: 'PUT',
                body: data,
                headers: {
                    Allow: 'POST, PUT',
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
            }).then((response) => response.json())
                .then((responseJson) => {
                    this.props.history.push("/fetchEvents");
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
                    'Content-Type': 'application/json'
                },
            }).then((response) => response.json())
                .then((responseJson) => {
                    this.props.history.push("/fetchEvents");
                })
        }
    }

    // This will handle Cancel button click event.  
    private handleCancel(event: any) {
        event.preventDefault();
        this.props.history.push("/fetchEvents");
    }

    // Returns the HTML Form to the render() method.  
    private renderCreateForm(typeList: Array<TypeData>) {
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
                        <DateTimePicker selected={this.state.eventData.eventDate} name="eventDate" onChange={this.handleEventDateChange} value={this.state.eventData.eventDate} />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventType">Event Type</label>
                    <div className="col-md-4">
                        <select className="form-control" data-val="true" name="eventTypeId" defaultValue={this.state.eventData.eventTypeId} required>
                            <option value="">-- Select Event Type --</option>
                            {typeList.map(type =>
                                <option key={type.id} value={type.name}>{type.name}</option>
                            )}
                        </select>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="StreetAddress">StreetAddress</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="streetAddress" defaultValue={this.state.eventData.streetAddress} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="City">City</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="city" defaultValue={this.state.eventData.city} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="stateProvince">State / Province</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="stateProvince" defaultValue={this.state.eventData.stateProvince} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Country">Country</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="country" defaultValue={this.state.eventData.country} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="ZipCode">Zip Code</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="zipCode" defaultValue={this.state.eventData.zipCode} required />
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
                    <label className="control-label col-md-12" htmlFor="GPSCoords">GPS Coords</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="gpsCoords" defaultValue={this.state.eventData.gpscoords} required />
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

export class TypeData {
    id: number = 0;
    name: string = "";
    description: string = "";
    displayOrder: number = 0;
    isActive: boolean = true;
}