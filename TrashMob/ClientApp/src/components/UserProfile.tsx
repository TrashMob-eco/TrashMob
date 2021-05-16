import * as React from 'react'
import UserData from './Models/UserData';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../store/ToolTips";
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { CountryDropdown, RegionDropdown } from 'react-country-region-selector';
import { PrivacyPolicyVersion } from './PrivacyPolicy';

interface UserProfileProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const UserProfile: React.FC<UserProfileProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [userId, setUserId] = React.useState<string>(props.currentUser.id);
    const [nameIdentifier, setNameIdentifier] = React.useState<string>("");
    const [userName, setUserName] = React.useState<string>("");
    const [givenName, setGivenName] = React.useState<string>("");
    const [surName, setSurName] = React.useState<string>("");
    const [email, setEmail] = React.useState<string>();
    const [city, setCity] = React.useState<string>();
    const [country, setCountry] = React.useState<string>();
    const [region, setRegion] = React.useState<string>();
    const [postalCode, setPostalCode] = React.useState<string>();
    const [dateAgreedToPrivacyPolicy, setDateAgreedToPrivacyPolicy] = React.useState<Date>(new Date());
    const [dateAgreedToTermsOfService, setDateAgreedToTermsOfService] = React.useState<Date>(new Date());
    const [privacyPolicyVersion, setPrivacyPolicyVersion] = React.useState<string>("");
    const [termsOfServiceVersion, setTermsOfServiceVersion] = React.useState<string>("");
    const [memberSince, setMemberSince] = React.useState<Date>(new Date());
    const [userNameErrors, setUserNameErrors] = React.useState<string>("");
    const [givenNameErrors, setGivenNameErrors] = React.useState<string>("");
    const [surNameErrors, setSurNameErrors] = React.useState<string>("");
    const [emailErrors, setEmailErrors] = React.useState<string>("");
    const [cityErrors, setCityErrors] = React.useState<string>("");
    const [countryErrors, setCountryErrors] = React.useState<string>("");
    const [regionErrors, setRegionErrors] = React.useState<string>("");
    const [postalCodeErrors, setPostalCodeErrors] = React.useState<string>("");

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');

        fetch('api/users/' + userId, {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<UserData>)
            .then(data => {
                setNameIdentifier(data.nameIdentifier);
                setUserName(data.userName);
                setGivenName(data.givenName);
                setSurName(data.surName);
                setEmail(data.email);
                setCity(data.city);
                setCountry(data.country);
                setRegion(data.region);
                setPostalCode(data.postalCode);
                setDateAgreedToPrivacyPolicy(data.dateAgreedToPrivacyPolicy);
                setDateAgreedToTermsOfService(data.dateAgreedToTermsOfService);
                setPrivacyPolicyVersion(data.privacyPolicyVersion);
                setTermsOfServiceVersion(data.termsOfServiceVersion);
                setMemberSince(data.memberSince);
                setIsDataLoaded(true);
            });
    }, [userId])

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push("/");
    }

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        if (userNameErrors !== "" ||
            givenNameErrors !== "" ||
            surNameErrors !== "" ||
            emailErrors !== "" ||
            cityErrors !== "" ||
            countryErrors !== "" ||
            regionErrors !== "" ||
            postalCodeErrors !== "") {
            return;
        }

        var userData = new UserData();

        userData.id = userId;
        userData.nameIdentifier = nameIdentifier;
        userData.userName = userName ?? "";
        userData.givenName = givenName ?? "";
        userData.surName = surName ?? "";
        userData.email = email ?? "";
        userData.city = city ?? "";
        userData.region = region ?? "";
        userData.country = country ?? "";
        userData.postalCode = postalCode ?? "";
        userData.dateAgreedToPrivacyPolicy = new Date(dateAgreedToPrivacyPolicy);
        userData.dateAgreedToTermsOfService = new Date(dateAgreedToTermsOfService);
        userData.privacyPolicyVersion = privacyPolicyVersion ?? "";
        userData.termsOfServiceVersion = termsOfServiceVersion;
        userData.memberSince = new Date(memberSince);

        var usrdata = JSON.stringify(userData);

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {

            const headers = getDefaultHeaders('PUT');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('api/Events', {
                method: 'PUT',
                headers: headers,
                body: usrdata,
            }).then(() => {
                props.history.push("/mydashboard");
            })
        })
    }

    function handleUserNameChanged(val: string) {
        setUserName(val);
    }

    function handleGivenNameChanged(val: string) {
        setGivenName(val);
    }

    function handleSurNameChanged(val: string) {
        setSurName(val);
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

    function handleCityChanged(val: string) {
        setCity(val);
    }

    function selectCountry(val: string) {
        setCountry(val);
    }

    function selectRegion(val: string) {
        setRegion(val);
    }

    function handlePostalCodeChanged(val: string) {
        setPostalCode(val);
    }

    function renderUserNameToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfileUserName}</Tooltip>
    }

    function renderGivenNameToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfileGivenName}</Tooltip>
    }

    function renderSurNameToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfileSurName}</Tooltip>
    }

    function renderEmailToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfileEmail}</Tooltip>
    }

    function renderCityToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfileCity}</Tooltip>
    }

    function renderCountryToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfileCountry}</Tooltip>
    }

    function renderRegionToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfileRegion}</Tooltip>
    }

    function renderPostalCodeToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfilePostalCode}</Tooltip>
    }

    function renderDateAgreedToPrivacyPolicyToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfileDateAgreedToPrivacyPolicy}</Tooltip>
    }

    function renderPrivacyPolicyVersionToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfilePrivacyPolicyVersion}</Tooltip>
    }

    function renderDateAgreedToTermsOfServiceToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfileDateAgreedToTermsOfService}</Tooltip>
    }

    function renderTermsOfServiceVersionToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfileTermsOfServiceVersion}</Tooltip>
    }

    function renderMemberSinceToolTip(props) {
        return <Tooltip {...props}>{ToolTips.UserProfileMemberSince}</Tooltip>
    }

    return (
        !isDataLoaded ? <div>Loading</div> :
            <div>
                <h1>User Profile</h1>
                <div className="container-fluid" >
                    <form onSubmit={handleSave} >
                        < div className="form-group row" >
                            <OverlayTrigger placement="top" overlay={renderUserNameToolTip}>
                                <label className=" control-label col-xs-2" htmlFor="UserName">Username:</label>
                            </OverlayTrigger>
                            <div className="col-md-4">
                                <input className="form-control" type="text" name="userName" defaultValue={userName} onChange={(val) => handleUserNameChanged(val.target.value)} maxLength={parseInt('32')} required />
                                <span style={{ color: "red" }}>{userNameErrors}</span>
                            </div>
                            <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                <label className="control-label col-xs-2" htmlFor="email">Email:</label>
                            </OverlayTrigger>
                            <div className="col-md-4">
                                <input className="form-control" type="text" name="email" defaultValue={email} onChange={(val) => handleEmailChanged(val.target.value)} maxLength={parseInt('64')} required />
                                <span style={{ color: "red" }}>{emailErrors}</span>
                            </div>
                        </div >
                        <div className="form-group row">
                            <OverlayTrigger placement="top" overlay={renderGivenNameToolTip}>
                                <label className="control-label col-xs-2" htmlFor="GivenName">Given Name:</label>
                            </OverlayTrigger>
                            <div className="col-md-4">
                                <input className="form-control" type="text" name="givenName" defaultValue={givenName} onChange={(val) => handleGivenNameChanged(val.target.value)} maxLength={parseInt('32')} />
                                <span style={{ color: "red" }}>{givenNameErrors}</span>
                            </div>
                            <OverlayTrigger placement="top" overlay={renderSurNameToolTip}>
                                <label className="control-label col-xs-2" htmlFor="SurName">Surname:</label>
                            </OverlayTrigger>
                            <div className="col-md-4">
                                <input className="form-control" type="text" name="surName" defaultValue={surName} onChange={(val) => handleSurNameChanged(val.target.value)} maxLength={parseInt('32')} />
                                <span style={{ color: "red" }}>{surNameErrors}</span>
                            </div>
                        </div >
                        <div className="form-group row">
                            <OverlayTrigger placement="top" overlay={renderCityToolTip}>
                                <label className="control-label col-xs-2" htmlFor="City">City:</label>
                            </OverlayTrigger>
                            <div className="col-xs-2">
                                <input className="form-control" type="text" name="city" defaultValue={city} onChange={(val) => handleCityChanged(val.target.value)} maxLength={parseInt('64')} />
                                <span style={{ color: "red" }}>{cityErrors}</span>
                            </div>
                            <OverlayTrigger placement="top" overlay={renderPostalCodeToolTip}>
                                <label className="control-label col-xs-2" htmlFor="PostalCode">Postal Code:</label>
                            </OverlayTrigger>
                            <div className="col-xs-2">
                                <input className="form-control" type="text" name="postalCode" defaultValue={postalCode} onChange={(val) => handlePostalCodeChanged(val.target.value)} maxLength={parseInt('25')} />
                                <span style={{ color: "red" }}>{postalCodeErrors}</span>
                            </div>
                        </div >
                        <div className="form-group row">
                            <OverlayTrigger placement="top" overlay={renderCountryToolTip}>
                                <label className="control-label col-xs-2" htmlFor="Country">Country:</label>
                            </OverlayTrigger>
                            <div className="col-xs-4">
                                <CountryDropdown name="country" value={country} onChange={(val) => selectCountry(val)} />
                                <span style={{ color: "red" }}>{countryErrors}</span>
                            </div>
                            <OverlayTrigger placement="top" overlay={renderRegionToolTip}>
                                <label className="control-label col-xs-2" htmlFor="region">Region:</label>
                            </OverlayTrigger>
                            <div className="col-xs-4">
                                <RegionDropdown
                                    country={country}
                                    value={region}
                                    onChange={(val) => selectRegion(val)} />
                                <span style={{ color: "red" }}>{regionErrors}</span>
                            </div>
                        </div >
                        <div className="form-group">
                            <button type="submit" className="btn btn-default">Save</button>
                        </div >
                        <div className="form-group row">
                            <OverlayTrigger placement="top" overlay={renderDateAgreedToPrivacyPolicyToolTip}>
                                <label className="control-label col-xs-2" htmlFor="dateAgreedToPrivacyPolicy">Date Agreed To Privacy Policy:</label>
                            </OverlayTrigger>
                            <div className="col-xs-2">
                                <label className="form-control">{dateAgreedToPrivacyPolicy ? dateAgreedToPrivacyPolicy.toString() : ""}</label>
                            </div>
                            <OverlayTrigger placement="top" overlay={renderPrivacyPolicyVersionToolTip}>
                                <label className="control-label col-xs-2" htmlFor="PrivacyPolicyVersion">Privacy Policy Version:</label>
                            </OverlayTrigger>
                            <div className="col-xs-2">
                                <label className="form-control">{privacyPolicyVersion}</label>
                            </div>
                        </div >
                        <div className="form-group row">
                            <OverlayTrigger placement="top" overlay={renderDateAgreedToTermsOfServiceToolTip}>
                                <label className="control-label col-xs-2" htmlFor="dateAgreedToTermsOfService">Date Agreed To Terms of Service:</label>
                            </OverlayTrigger>
                            <div className="col-xs-2">
                                <label className="form-control">{dateAgreedToTermsOfService ? dateAgreedToTermsOfService.toString() : ""}</label>
                            </div>
                            <OverlayTrigger placement="top" overlay={renderTermsOfServiceVersionToolTip}>
                                <label className="control-label col-xs-2" htmlFor="TermsOfServiceVersion">Terms Of Service Version:</label>
                            </OverlayTrigger>
                            <div className="col-xs-2">
                                <label className="form-control">{termsOfServiceVersion}</label>
                            </div>
                        </div >
                        <div className="form-group row">
                            <OverlayTrigger placement="top" overlay={renderMemberSinceToolTip}>
                                <label className="control-label col-xs-2" htmlFor="memberSince">Member Since:</label>
                            </OverlayTrigger>
                            <div className="col-xs-2">
                                <label className="form-control">{memberSince ? memberSince.toLocaleString() : "" }</label>
                            </div>
                        </div >
                        <div className="form-group">
                            <button disabled={emailErrors !== "" || userNameErrors !== ""} type="submit" className="action btn-default">Save</button>
                            <button className="action" onClick={(e) => handleCancel(e)}>Cancel</button>
                        </div >
                    </form >
                </div>
            </div>
    );
}

export default withRouter(UserProfile);