import * as React from 'react'
import UserData from './Models/UserData';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../store/ToolTips";
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { CountryDropdown, RegionDropdown } from 'react-country-region-selector';
import { Modal } from 'reactstrap';

interface UserProfileProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const UserProfile: React.FC<UserProfileProps> = (props) => {
    const userId = props.currentUser.id;
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [nameIdentifier, setNameIdentifier] = React.useState<string>("");
    const [userName, setUserName] = React.useState<string>("");
    const [sourceSystemUserName, setSourceSystemUserName] = React.useState<string>("");
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
    const [cityErrors, setCityErrors] = React.useState<string>("");
    const [countryErrors, setCountryErrors] = React.useState<string>("");
    const [regionErrors, setRegionErrors] = React.useState<string>("");
    const [postalCodeErrors, setPostalCodeErrors] = React.useState<string>("");
    const [isOpen, setIsOpen] = React.useState(false);

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
                setSourceSystemUserName(data.sourceSystemUserName);

                setUserNameErrors("");
                setGivenNameErrors("");
                setSurNameErrors("");
                setCityErrors("");
                setCountryErrors("");
                setRegionErrors("");
                setPostalCodeErrors("");

                setIsDataLoaded(true);
            });
    }, [userId])

    function togglemodal() {
        setIsOpen(!isOpen);
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push("/");
    }

    function handleDelete(event: any) {
        event.preventDefault();
        setIsOpen(true);
    }

    // This will handle the delete account
    function deleteAccount() {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {

            const headers = getDefaultHeaders('DELETE');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('api/users/' + userId, {
                method: 'DELETE',
                headers: headers
            }).then(() => {
                msalClient.logoutRedirect();
                props.history.push("/");
            })
        })
    }

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        if (userNameErrors !== "" ||
            givenNameErrors !== "" ||
            surNameErrors !== "" ||
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

            fetch('api/users', {
                method: 'PUT',
                headers: headers,
                body: usrdata,
            }).then(() => {
                props.history.push("/");
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

    function renderUserNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileUserName}</Tooltip>
    }

    function renderGivenNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileGivenName}</Tooltip>
    }

    function renderSurNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileSurName}</Tooltip>
    }

    function renderEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileEmail}</Tooltip>
    }

    function renderCityToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileCity}</Tooltip>
    }

    function renderCountryToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileCountry}</Tooltip>
    }

    function renderRegionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileRegion}</Tooltip>
    }

    function renderPostalCodeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfilePostalCode}</Tooltip>
    }

    function renderDateAgreedToPrivacyPolicyToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileDateAgreedToPrivacyPolicy}</Tooltip>
    }

    function renderPrivacyPolicyVersionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfilePrivacyPolicyVersion}</Tooltip>
    }

    function renderDateAgreedToTermsOfServiceToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileDateAgreedToTermsOfService}</Tooltip>
    }

    function renderTermsOfServiceVersionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileTermsOfServiceVersion}</Tooltip>
    }

    function renderMemberSinceToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileMemberSince}</Tooltip>
    }

    function renderSourceSystemUserNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileSourceSystemUserName}</Tooltip>
    }

    return (
        !isDataLoaded ? <div>Loading</div> :
            <div>
                <h1>User Profile</h1>
                <div>
                    <Modal isOpen={isOpen} onrequestclose={togglemodal} contentlabel="Delete Account?" fade={true} style={{ width: "300px", display: "block" }}>
                        <div className="container">
                            <span>
                                <label>Are you sure you want to delete your account and all your events? Deleted accounts cannot be recovered!</label>
                            </span>

                            <div>
                                <button className="action" onClick={() => {
                                    togglemodal();
                                    deleteAccount();
                                }
                                }>
                                    Yes, Delete My Account
                            </button>
                                <button className="action" onClick={() => {
                                    togglemodal();
                                }
                                }>
                                    Cancel
                            </button>
                            </div>
                        </div>
                    </Modal>
                </div>
                <div className="container-fluid" >
                    <form onSubmit={handleSave} >
                        <div>
                            <button className="action" onClick={(e) => handleDelete(e)}>Delete Account</button>
                        </div>
                        <div>
                            <h2>Enter a User Name below to help other recognize you!</h2>
                        </div>
                        <div className="form-group row">
                            <OverlayTrigger placement="top" overlay={renderUserNameToolTip}>
                                <label className=" control-label col-xs-2" htmlFor="UserName">User Name:</label>
                            </OverlayTrigger>
                            <div className="col-md-4">
                                <input className="form-control" type="text" name="userName" defaultValue={userName} onChange={(val) => handleUserNameChanged(val.target.value)} maxLength={parseInt('32')} required />
                                <span style={{ color: "red" }}>{userNameErrors}</span>
                            </div>
                            <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                <label className="control-label col-xs-2" htmlFor="email">Email:</label>
                            </OverlayTrigger>
                            <div className="col-xs-2">
                                <label className="form-control">{email}</label>
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
                                <CountryDropdown name="country" value={country ?? ""} onChange={(val) => selectCountry(val)} />
                                <span style={{ color: "red" }}>{countryErrors}</span>
                            </div>
                            <OverlayTrigger placement="top" overlay={renderRegionToolTip}>
                                <label className="control-label col-xs-2" htmlFor="region">Region:</label>
                            </OverlayTrigger>
                            <div className="col-xs-4">
                                <RegionDropdown
                                    country={country ?? ""}
                                    value={region ?? ""}
                                    onChange={(val) => selectRegion(val)} />
                                <span style={{ color: "red" }}>{regionErrors}</span>
                            </div>
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
                        <div className="form-group row">
                            <OverlayTrigger placement="top" overlay={renderSourceSystemUserNameToolTip}>
                                <label className="control-label col-xs-2" htmlFor="memberSince">Source System User Name:</label>
                            </OverlayTrigger>
                            <div className="col-xs-2">
                                <label className="form-control">{sourceSystemUserName}</label>
                            </div>
                        </div >
                        <div className="form-group">
                            <button disabled={userNameErrors !== ""} type="submit" className="action btn-default">Save</button>
                            <button className="action" onClick={(e) => handleCancel(e)}>Cancel</button>
                        </div >
                    </form >
                </div>
            </div>
    );
}

export default withRouter(UserProfile);