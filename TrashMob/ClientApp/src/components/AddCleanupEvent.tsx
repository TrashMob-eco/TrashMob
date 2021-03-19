import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { CleanupEventData } from './FetchCleanupEvent';  

interface AddCleanupEventDataState {
    title: string;
    loading: boolean;
    eventData: CleanupEventData;
}

export class AddCleanupEvent extends React.Component<RouteComponentProps<{}>, AddCleanupEventDataState> {
    constructor(props) {
        super(props);

        this.state = { title: "", loading: true, eventData: new CleanupEventData() };

        var eventId = this.props.match.params["eventId"];

        // This will set state for Edit CleanupEvent  
        if (eventId > 0) {
            fetch('api/CleanupEvents/' + eventId)
                .then(response => response.json() as Promise<CleanupEventData>)
                .then(data => {
                    this.setState({ title: "Edit", loading: false, eventData: data });
                });
        }

        // This will set state for Add CleanupEvent  
        else {
            this.state = { title: "Create", loading: false, eventData: new CleanupEventData() };
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
            <h3>CleanupEvent</h3>
            <hr />
            {contents}
        </div>;
    }

    // This will handle the submit form event.  
    private handleSave(event) {
        event.preventDefault();
        const data = new FormData(event.target);

        // PUT request for Edit CleanupEvent.  
        if (this.state.eventData.cleanupEventId) {
            fetch('api/CleanupEvents', {
                method: 'PUT',
                body: data,

            }).then((response) => response.json())
                .then((responseJson) => {
                    this.props.history.push("/fetchCleanupEvent");
                })
        }

        // POST request for Add CleanupEvent.  
        else {
            fetch('api/CleanupEvents', {
                method: 'POST',
                body: data,

            }).then((response) => response.json())
                .then((responseJson) => {
                    this.props.history.push("/fetchCleanupEvent");
                })
        }
    }

    // This will handle Cancel button click event.  
    private handleCancel(e) {
        e.preventDefault();
        this.props.history.push("/fetchCleanupEvent");
    }

    // Returns the HTML Form to the render() method.  
    private renderCreateForm() {
        return (
            <form onSubmit={this.handleSave} >
                <div className="form-group row" >
                    <input type="hidden" name="CleanupEventId" value={this.state.eventData.cleanupEventId} />
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