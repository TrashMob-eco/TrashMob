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
    name: string;
    email: string;
    emailErrors: string;
    message: string;
    messageErrors: string;
}

interface Props extends RouteComponentProps<any> {
}

class ContactUs extends Component<Props, ContactRequestDataState> {
    constructor(props: Props) {
        super(props);
        this.state = { title: "", loading: true, name: '', email: '', emailErrors: '', message: '', messageErrors: '' };

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
            contactRequestData.name = this.state.name ?? "";
            contactRequestData.email = this.state.email ?? "";
            contactRequestData.message = this.state.message ?? "";

            var data = JSON.stringify(contactRequestData);

            const headers = defaultHeaders('POST');

            fetch('api/ContactRequest', {
                method: 'POST',
                body: data,
                headers: headers,
            }).then((response) => {
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

    handleNameChanged = (val: string) => {
        this.setState({ name: val });
    }

    handleEmailChanged = (val: string) => {
        var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);

        if (!pattern.test(val)) {
            this.setState({ emailErrors: "Please enter valid email address." });
        }
        else {
            this.setState({ emailErrors: "" });
            this.setState({ email: val });
        }
    }

    handleMessageChanged = (val: string) => {
        if (val.length < 0 || val.length > 1000) {
            this.setState({ messageErrors: "Message cannot be empty and cannot be more than 1000 characters long." });
        }
        else {
            this.setState({ messageErrors: "" });
            this.setState({ message: val });
        }
    }

    componentDidMount() {
        loadCaptchaEnginge(6);
    };

    // Returns the HTML Form to the render() method.  
    render() {
        return (
            <div>
                <h1>Contact Us</h1>
                <div>
                    Have a question for the TrashMob team? Or an idea to make this site better? Or just want to tell us you love us? Drop us a note!
                </div>
                <form onSubmit={this.handleSave} >
                    < div className="form-group row" >
                        <label className=" control-label col-md-12" htmlFor="Name">Name</label>
                        <div className="col-md-4">
                            <input className="form-control" type="text" name="name" defaultValue={this.state.name} required />
                        </div>
                    </div >
                    < div className="form-group row" >
                        <label className=" control-label col-md-12" htmlFor="Email">Email</label>
                        <div className="col-md-4">
                            <input className="form-control" type="text" name="email" defaultValue={this.state.email} required />
                            <span style={{ color: "red" }}>{this.state.emailErrors}</span>
                        </div>
                    </div >
                    <div className="form-group row">
                        <label className="control-label col-md-12" htmlFor="Message">Message</label>
                        <div className="col-md-4">
                            <input className="form-control" type="text" name="message" defaultValue={this.state.message} required />
                            <span style={{ color: "red" }}>{this.state.messageErrors}</span>
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
            </div>
        )
    }
}

export default withRouter(ContactUs);