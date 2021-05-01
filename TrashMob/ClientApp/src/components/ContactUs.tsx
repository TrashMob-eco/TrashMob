import * as React from 'react'
import ContactRequestData from './Models/ContactRequestData';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { loadCaptchaEnginge, LoadCanvasTemplateNoReload, validateCaptcha } from 'react-simple-captcha';
import { getDefaultHeaders } from '../store/AuthStore';

interface ContactUsProps extends RouteComponentProps<any> {
}

export const ContactUs: React.FC<ContactUsProps> = (props) => {
    const [name, setName] = React.useState<string>();
    const [email, setEmail] = React.useState<string>();
    const [emailErrors, setEmailErrors] = React.useState<string>();
    const [message, setMessage] = React.useState<string>();
    const [messageErrors, setMessageErrors] = React.useState<string>();

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        const form = new FormData(event.target);

        var user_captcha_value = form.get("user_captcha_input")?.toString() ?? "";

        if (validateCaptcha(user_captcha_value) === true) {

            var contactRequestData = new ContactRequestData();
            contactRequestData.name = this.state.name ?? "";
            contactRequestData.email = this.state.email ?? "";
            contactRequestData.message = this.state.message ?? "";

            var data = JSON.stringify(contactRequestData);

            const headers = getDefaultHeaders('POST');

            fetch('api/ContactRequest', {
                method: 'POST',
                body: data,
                headers: headers,
            }).then(() => {
                this.props.history.push("/");
            })
        }

        else {
            alert('Captcha Does Not Match');
        }
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push("/");
    }

    function handleNameChanged(val: string) {
        setName(val);
    }

    function handleEmailChanged(val: string) {
        var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);

        if (!pattern.test(val)) {
            setEmailErrors("Please enter valid email address.");
        }
        else {
            setEmailErrors("");
            setEmail(val);
        }
    }

    function handleMessageChanged(val: string) {
        if (val.length < 0 || val.length > 1000) {
            setMessageErrors("Message cannot be empty and cannot be more than 1000 characters long.");
        }
        else {
            setMessageErrors("");
            setMessage(val);
        }
    }

    React.useEffect(() => {
        loadCaptchaEnginge(6);
    });

    return (
        <div className="container-fluid">
            <h1>Contact Us</h1>
            <div>
                Have a question for the TrashMob team? Or an idea to make this site better? Or just want to tell us you love us? Drop us a note!
                </div>
            <form onSubmit={handleSave} >
                < div className="form-group row" >
                    <label className=" control-label col-xs-1" htmlFor="Name">Name:</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="name" defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required />
                    </div>
                    <label className=" control-label col-xs-1" htmlFor="Email">Email:</label>
                    <div className="col-md-4">
                        <input className="form-control" type="text" name="email" defaultValue={email} maxLength={parseInt('64')} onChange={(val) => handleEmailChanged(val.target.value)} required />
                        <span style={{ color: "red" }}>{emailErrors}</span>
                    </div>
                </div >
                <div className="form-group row">
                    <label className="control-label col-md-12" htmlFor="Message">Message:</label>
                    <div className="col-md-12">
                        <textarea className="form-control" name="message" defaultValue={message} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleMessageChanged(val.target.value)} required />
                        <span style={{ color: "red" }}>{messageErrors}</span>
                    </div>
                </div >
                <div>
                    <LoadCanvasTemplateNoReload />
                </div>
                <div className="form-group row">
                    <label className="control-label col-xs-2" htmlFor="Captcha">CAPTCHA Value:</label>
                    <div className="col-xs-2">
                        <input className="form-control" type="text" name="user_captcha_input" required />
                    </div>
                </div >
                <div className="form-group">
                    <button type="submit" className="action btn-default">Save</button>
                    <button className="action" onClick={(e) => handleCancel(e)}>Cancel</button>
                </div >
            </form >
        </div>
    )
}

export default withRouter(ContactUs);