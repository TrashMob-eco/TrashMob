import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Guid } from "guid-typescript";
import { EventData } from './FetchEvents';  

interface AddEventDataState {
    title: string;
    loading: boolean;
    eventData: EventData;
    statusList: StatusData[];
    typeList: TypeData[];
    eventId: Guid;
}

interface MatchParams {
    eventId: string;
}

export class AddEvent extends React.Component<RouteComponentProps<MatchParams>, AddEventDataState> {
    constructor(props: RouteComponentProps<MatchParams>) {
        super(props);
        this.state = {
            title: "", loading: true, eventData: new EventData(), eventId: Guid.create(), statusList: [], typeList: [] };

        fetch('api/eventStatuses', {
            method: 'GET',
            headers: {
                Allow: 'GET',
                Accept: 'application/json',
                'Content-Type': 'application/json'
                },
            })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                this.setState({ statusList: data });
            });  

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
            this.state = { title: "Create", loading: false, eventData: new EventData(), eventId: Guid.create(), statusList: [], typeList: [] };
        }

        // This binding is necessary to make "this" work in the callback  
        this.handleSave = this.handleSave.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderCreateForm(this.state.statusList, this.state.typeList);

        return <div>
            <h1>{this.state.title}</h1>
            <h3>Event</h3>
            <hr />
            {contents}
        </div>;
    }

    private serializeForm(form: HTMLFormElement) {
        let obj: { [key: string]: string } = {};
        var formData = new FormData(form);
        for (var keyd of Array.from<string>(formData.keys())) {
            var value = formData.get(keyd);
            if (value && typeof value === 'string') {
                obj[keyd] = value;
            }
        }
        return obj;
    };

    // This will handle the submit form event.  
    private handleSave(event : any) {
        event.preventDefault();

        const data = JSON.stringify(this.serializeForm(event.target));

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
    private renderCreateForm(statusList: Array<StatusData>, typeList: Array<TypeData>) {
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
                        <input className="form-control" type="text" name="eventDate" defaultValue={this.state.eventData.eventDate.toString()} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventType">Event Type</label>
                    <div className="col-md-4">
                        <select className="form-control" data-val="true" name="EventType" defaultValue={this.state.eventData.eventTypeId} required>
                            <option value="">-- Select Event Type --</option>
                            {typeList.map(type =>
                                <option key={type.id} value={type.name}>{type.name}</option>
                            )}
                        </select>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="EventStatusId">Event Status</label>
                    <div className="col-md-4">
                        <select className="form-control" data-val="true" name="EventStatus" defaultValue={this.state.eventData.eventStatusId} required>
                            <option value="">-- Select Event Status --</option>
                            {statusList.map(status =>
                                <option key={status.id} value={status.name}>{status.name}</option>
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
                    <label className="control-label col-md-12" htmlFor="CreatedBy">Created By</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="createdBy" defaultValue={this.state.eventData.createdByUserId} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="CreatedDate">Created Date</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="createdDate" defaultValue={this.state.eventData.createdDate.toString()} required />
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
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="LastUpdatedBy">Last Updated By</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="lastUpdatedBy" defaultValue={this.state.eventData.lastUpdatedByUserId} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="LastUpdatedDate">Last Updated Date</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="lastUpdatedDate" defaultValue={this.state.eventData.lastUpdatedDate.toString()} required />
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

export class StatusData {
    id: number = 0;
    name: string = "";
    description: string = "";
    displayOrder: number = 0;
}

export class TypeData {
    id: number = 0;
    name: string = "";
    description: string = "";
    displayOrder: number = 0;
    isActive: boolean = true;
}