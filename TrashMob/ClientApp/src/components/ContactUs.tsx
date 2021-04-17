import { Component } from 'react';
import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import ContactRequestData from './Models/ContactRequestData';
import { withRouter } from 'react-router-dom';
import { loadCaptchaEnginge, LoadCanvasTemplateNoReload, validateCaptcha } from 'react-simple-captcha';
import { defaultHeaders } from '../store/AuthStore';

interface ContactRequestDataState {
    title: string;
    loading: boolean;
    contactRequestData: ContactRequestData;
}

interface Props extends RouteComponentProps<any> {
}

class ContactUs extends Component<Props, ContactRequestDataState> {
    constructor(props: Props) {
        super(props);
        this.state = {
            title: "", loading: true, contactRequestData: new ContactRequestData()
        };

        // This binding is necessary to make "this" work in the callback  
        this.handleSave = this.handleSave.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
    }

    // This will handle the submit form event.  
    private handleSave(event: any) {
        event.preventDefault();

        const form = new FormData(event.target);

        var user_captcha_value = form.get("user_captcha_input")?.toString() ?? "";

        if (validateCaptcha(user_captcha_value) === true) {

            var contactRequestData = new ContactRequestData();
            contactRequestData.name = form.get("name")?.toString() ?? "";
            contactRequestData.email = form.get("email")?.toString() ?? "";
            contactRequestData.message = form.get("message")?.toString() ?? "";

            var data = JSON.stringify(contactRequestData);

            const headers = defaultHeaders('POST');

            fetch('api/ContactRequest', {
                method: 'POST',
                body: data,
                headers: headers,
            }).then((response) => response.json())
                .then((responseJson) => {
                    this.props.history.push("/");
                })
        }

        else {
            alert('Captcha Does Not Match');
        }
    }

    // This will handle Cancel button click event.  
    private handleCancel(event: any) {
        event.preventDefault();
        this.props.history.push("/");
    }

    componentDidMount() {
        loadCaptchaEnginge(6);
    };

    // Returns the HTML Form to the render() method.  
    render() {
        return (
            <form onSubmit={this.handleSave} >
                <div className="form-group row" >
                    <input type="hidden" name="Id" value={this.state.contactRequestData.id.toString()} />
                </div>
                < div className="form-group row" >
                    <label className=" control-label col-md-12" htmlFor="Name">Name</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="name" defaultValue={this.state.contactRequestData.name} required />
                    </div>
                </div >
                < div className="form-group row" >
                    <label className=" control-label col-md-12" htmlFor="Email">Email</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="email" defaultValue={this.state.contactRequestData.email} required />
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Message">Message</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="message" defaultValue={this.state.contactRequestData.message} required />
                    </div>
                </div >
                <div>
                    <LoadCanvasTemplateNoReload />
                </div>
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Captcha">CAPTCHA Value</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="user_captcha_input" required />
                    </div>
                </div >
                <div className="form-group">
                    <button type="submit" className="btn btn-default">Save</button>
                    <button className="btn" onClick={(e) => this.handleCancel(e)}>Cancel</button>
                </div >
            </form >
        )
    }
}

export default withRouter(ContactUs);