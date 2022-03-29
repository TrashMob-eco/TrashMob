import * as React from 'react'
import UserData from './Models/UserData';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../store/ToolTips";
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { CountryDropdown, RegionDropdown } from 'react-country-region-selector';
import { Button, Col, Form, ToggleButton } from 'react-bootstrap';
import { Modal } from 'reactstrap';
import * as MapStore from '../store/MapStore';
import { getKey } from '../store/MapStore';
import AddressData from './Models/AddressData';
import { data } from 'azure-maps-control';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import * as Constants from './Models/Constants';
import MapControllerSinglePoint from './MapControllerSinglePoint';
import EventMediaData from './Models/EventMediaData';
import MediaTypeData from './Models/MediaTypeData';
import { getMediaType } from '../store/mediaTypeHelper';

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
    const [longitude, setLongitude] = React.useState<number>(0);
    const [latitude, setLatitude] = React.useState<number>(0);
    const [prefersMetric, setPrefersMetric] = React.useState<boolean>(false);
    const [isOptedOutOfAllEmails, setIsOptedOutOfAllEmails] = React.useState<boolean>(false);
    const [travelLimitForLocalEvents, setTravelLimitForLocalEvents] = React.useState<number>(10);
    const [isOpen, setIsOpen] = React.useState(false);
    const [latitudeErrors, setLatitudeErrors] = React.useState<string>("");
    const [longitudeErrors, setLongitudeErrors] = React.useState<string>("");
    const [travelLimitForLocalEventsErrors, setTravelLimitForLocalEventsErrors] = React.useState<string>("");
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [eventName, setEventName] = React.useState<string>("User's Base Location");
    const [eventMedias, setEventMedias] = React.useState<EventMediaData[]>([]);
    const [mediaTypeList, setMediaTypeList] = React.useState<MediaTypeData[]>([]);
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

    React.useEffect(() => {

        if (props.isUserLoaded && !isDataLoaded) {
            const account = msalClient.getAllAccounts()[0];
            setEventName("User's Base Location");

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/users/' + userId, {
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
                        setLatitude(data.latitude);
                        setLongitude(data.longitude);
                        setIsOptedOutOfAllEmails(data.isOptedOutOfAllEmails);
                        setPrefersMetric(data.prefersMetric);
                        setTravelLimitForLocalEvents(data.travelLimitForLocalEvents);

                        setUserNameErrors("");
                        setGivenNameErrors("");
                        setSurNameErrors("");
                        setCityErrors("");
                        setCountryErrors("");
                        setRegionErrors("");
                        setPostalCodeErrors("");
                        setLatitudeErrors("");
                        setLongitudeErrors("");
                        setTravelLimitForLocalEventsErrors("");

                        setIsDataLoaded(true);
                    });

                fetch('/api/mediatypes', {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<Array<any>>)
                    .then(data => {
                        setMediaTypeList(data);
                        setIsDataLoaded(true);
                    })
                    .then(() => {
                    }).then(() => {
                        fetch('/api/eventmedias/byUserId/' + userId, {
                            method: 'GET',
                            headers: headers,
                        })
                            .then(response => response.json() as Promise<Array<EventMediaData>>)
                            .then(data => {
                                setEventMedias(data);
                            });
                    });
                setIsDataLoaded(true);
            });
        }

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })

        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                var point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point)
            });
        }
    }, [userId, props.isUserLoaded, isDataLoaded])

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

            fetch('/api/users/' + userId, {
                method: 'DELETE',
                headers: headers
            }).then(() => {
                msalClient.logoutRedirect();
                props.history.push("/");
            })
        })
    }

    function validateForm() {
        if (userNameErrors !== "" ||
            givenNameErrors !== "" ||
            surNameErrors !== "" ||
            cityErrors !== "" ||
            countryErrors !== "" ||
            regionErrors !== "" ||
            latitudeErrors !== "" ||
            longitudeErrors !== "" ||
            travelLimitForLocalEventsErrors !== "" ||
            postalCodeErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

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
        userData.latitude = latitude;
        userData.longitude = longitude;
        userData.isOptedOutOfAllEmails = isOptedOutOfAllEmails;
        userData.prefersMetric = prefersMetric;
        userData.travelLimitForLocalEvents = travelLimitForLocalEvents;

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

            fetch('/api/users', {
                method: 'PUT',
                headers: headers,
                body: usrdata,
            });
        })
    }

    function handleUserNameChanged(val: string) {

        if (!val || val.length === 0) {
            setUserNameErrors("Username cannot be empty. Username can consist of Letters A-Z (upper or lowercase), Numbers (0-9), and underscores (_)");
            validateForm();
            return;
        }

        var pattern = new RegExp(Constants.RegexUserName);

        if (!pattern.test(val)) {
            setUserNameErrors("Please enter a valid Username. Username can consist of Letters A-Z (upper or lowercase), Numbers (0-9), and underscores (_)");
            validateForm();
            return;
        }
        else {
            setUserNameErrors("");
        }

        const account = msalClient.getAllAccounts()[0];

        // Verify that this username is unique
        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/users/verifyunique/' + userId + '/' + val, {
                method: 'GET',
                headers: headers,
            }).then(response => {
                if (response.status === 200) {
                    setUserNameErrors("");
                    setUserName(val);
                }
                else if (response.status === 409) {
                    setUserNameErrors("This username is already in use. Please choose a different name.");
                }
                else {
                    setUserNameErrors("Unknown error occured while hecking user name. Please try again. Error Code: " + response.status);
                }

                validateForm();
            })
        })
    }

    function handleGivenNameChanged(val: string) {
        setGivenName(val);
        validateForm();
    }

    function handleSurNameChanged(val: string) {
        setSurName(val);
        validateForm();
    }

    function handleCityChanged(val: string) {
        setCity(val);
        validateForm();
    }

    function selectCountry(val: string) {
        setCountry(val);
        validateForm();
    }

    function selectRegion(val: string) {
        setRegion(val);
        validateForm();
    }

    function handlePostalCodeChanged(val: string) {
        setPostalCode(val);
        validateForm();
    }

    function handleLatitudeChanged(val: string) {
        try {
            if (val) {
                var floatVal = parseFloat(val);

                if (floatVal < -90 || floatVal > 90) {
                    setLatitudeErrors("Latitude must be => -90 and <= 90");
                }
                else {
                    setLatitude(floatVal);
                    setLatitudeErrors("");
                }
            }
            else {
                setLatitudeErrors("Latitude must be => -90 and <= 90");
            }
        }
        catch {
            setLatitudeErrors("Latitude must be a valid number.");
        }

        validateForm();
    }

    function handleLongitudeChanged(val: string) {
        try {
            if (val) {
                var floatVal = parseFloat(val);

                if (floatVal < -180 || floatVal > 180) {
                    setLongitudeErrors("Longitude must be >= -180 and <= 180");
                }
                else {
                    setLongitude(floatVal);
                    setLongitudeErrors("");
                }
            }
            else {
                setLongitudeErrors("Longitude must be >= -180 and <= 180");
            }
        }
        catch {
            setLongitudeErrors("Longitude must be a valid number.");
        }

        validateForm();
    }

    function handleTravelLimitForLocalEventsChanged(val: string) {
        try {
            if (val) {
                var intVal = parseInt(val);

                if (intVal <= 0 || intVal > 1000) {
                    setTravelLimitForLocalEventsErrors("Travel limit must be greater than or equal to 0 and less than 1000.")
                }
                else {
                    setTravelLimitForLocalEvents(intVal);
                    setTravelLimitForLocalEventsErrors("");
                }
            }
            else {
                setTravelLimitForLocalEvents(1);
            }
        }
        catch {
            setTravelLimitForLocalEventsErrors("Travel limit must be a valid number.");
        }

        validateForm();
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

    function renderTravelLimitForLocalEventsToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileTravelLimitForLocalEvents}</Tooltip>
    }

    function renderUserLatitudeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileLatitude}</Tooltip>
    }

    function renderUserLongitudeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfileLongitude}</Tooltip>
    }

    function renderPreferMetricToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.UserProfilePreferMetric}</Tooltip>
    }

    function handleLocationChange(point: data.Position) {
        // In an Azure Map point, the longitude is the first position, and latitude is second
        setLatitude(point[1]);
        setLongitude(point[0]);
        var locationString = point[1] + ',' + point[0]
        var headers = getDefaultHeaders('GET');

        getKey()
            .then(key => {
                fetch('https://atlas.microsoft.com/search/address/reverse/json?subscription-key=' + key + '&api-version=1.0&query=' + locationString, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<AddressData>)
                    .then(data => {
                        setCity(data.addresses[0].address.municipality);
                        setCountry(data.addresses[0].address.country);
                        setRegion(data.addresses[0].address.countrySubdivisionName);
                        setPostalCode(data.addresses[0].address.postalCode);
                        validateForm();
                    })
            }
        )
    }

    function onEventMediasUpdated() {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);
            fetch('/api/eventmedias/byUserId/' + userId, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<Array<EventMediaData>>)
                .then(data => {
                    setEventMedias(data);
                });
        })
    }

    function removeEventMedia(eventMediaId: string, mediaUrl: string) {
        if (!window.confirm("Please confirm that you want to remove Event Media with Url: '" + mediaUrl + "' as a media from this Event?"))
            return;
        else {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('DELETE');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/eventMedias/' + eventMediaId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        onEventMediasUpdated()
                    });
            });
        }
    }

    return (
        !isDataLoaded ? <div>Loading</div> :
            <div>
                <h3>User Profile</h3>
                <div>
                    <Modal isOpen={isOpen} onrequestclose={togglemodal} contentlabel="Delete Account?" fade={true} style={{ width: "300px", display: "block" }}>
                        <Form>
                            <Form.Row>
                                <h3>Are you sure you want to delete your account and all your events? Deleted accounts cannot be recovered!</h3>
                            </Form.Row>
                            <Form.Row>
                                <Button variant="danger" onClick={() => {
                                    togglemodal();
                                    deleteAccount();
                                }
                                }>
                                    Yes, Delete My Account
                                </Button>
                                <Button className="action" onClick={() => {
                                    togglemodal();
                                }
                                }>
                                    Cancel
                                </Button>
                            </Form.Row>
                        </Form>
                    </Modal>
                </div>
                <div className="container-fluid" >
                    <Form onSubmit={handleSave} >
                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <Button disabled={!isSaveEnabled} type="submit" className="action btn-default">Save</Button>
                                    <Button className="action" onClick={(e) => handleCancel(e)}>Cancel</Button>
                                </Form.Group>
                            </Col>
                            <Col>
                                <Button variant="danger" onClick={(e) => handleDelete(e)}>Delete Account</Button>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderUserNameToolTip}>
                                        <Form.Label className="control-label" htmlFor="UserName">User Name:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" name="userName" defaultValue={userName} onChange={(val) => handleUserNameChanged(val.target.value)} maxLength={parseInt('32')} required />
                                    <span style={{ color: "red" }}>{userNameErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                        <Form.Label className="control-label" htmlFor="email">Email:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" disabled defaultValue={email} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderGivenNameToolTip}>
                                        <Form.Label className="control-label" htmlFor="GivenName">Given Name:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" name="givenName" defaultValue={givenName} onChange={(val) => handleGivenNameChanged(val.target.value)} maxLength={parseInt('32')} />
                                    <span style={{ color: "red" }}>{givenNameErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderSurNameToolTip}>
                                        <Form.Label className="control-label" htmlFor="SurName">Surname:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" name="surName" defaultValue={surName} onChange={(val) => handleSurNameChanged(val.target.value)} maxLength={parseInt('32')} />
                                    <span style={{ color: "red" }}>{surNameErrors}</span>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Form.Label>Either search for a location or click on the map to set your base location. This location will only be used to assist in locating events you wish to be notified about. The location fields below will be automatically populated.</Form.Label>
                        </Form.Row>
                        <Form.Row>
                            <AzureMapsProvider>
                                <>
                                    <MapControllerSinglePoint center={center} isEventDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={eventName} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} isDraggable={true} eventDate={new Date()} />
                                </>
                            </AzureMapsProvider>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderCityToolTip}>
                                        <Form.Label className="control-label" htmlFor="City">City:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control className="form-control" type="text" disabled name="city" defaultValue={city} onChange={(val) => handleCityChanged(val.target.value)} maxLength={parseInt('64')} />
                                    <span style={{ color: "red" }}>{cityErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderPostalCodeToolTip}>
                                        <Form.Label className="control-label" htmlFor="PostalCode">Postal Code:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" disabled name="postalCode" defaultValue={postalCode} onChange={(val) => handlePostalCodeChanged(val.target.value)} maxLength={parseInt('25')} />
                                    <span style={{ color: "red" }}>{postalCodeErrors}</span>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderCountryToolTip}>
                                        <Form.Label className="control-label" htmlFor="Country">Country:</Form.Label>
                                    </OverlayTrigger>
                                    <CountryDropdown disabled name="country" value={country ?? ""} onChange={(val) => selectCountry(val)} />
                                    <span style={{ color: "red" }}>{countryErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderRegionToolTip}>
                                        <Form.Label className="control-label" htmlFor="region">Region:</Form.Label>
                                    </OverlayTrigger>
                                    <RegionDropdown disabled
                                        country={country ?? ""}
                                        value={region ?? ""}
                                        onChange={(val) => selectRegion(val)} />
                                    <span style={{ color: "red" }}>{regionErrors}</span>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderUserLatitudeToolTip}>
                                        <Form.Label className="control-label" htmlFor="Latitude">Latitude:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" disabled name="latitude" value={latitude} onChange={(val) => handleLatitudeChanged(val.target.value)} />
                                    <span style={{ color: "red" }}>{latitudeErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderUserLongitudeToolTip}>
                                        <Form.Label className="control-label" htmlFor="Longitude">Longitude:</Form.Label>
                                    </OverlayTrigger >
                                    <Form.Control type="text" disabled name="longitude" value={longitude} onChange={(val) => handleLongitudeChanged(val.target.value)} />
                                    <span style={{ color: "red" }}>{longitudeErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderTravelLimitForLocalEventsToolTip}>
                                        <Form.Label className="control-label" htmlFor="TravelLimitForLocalEvents">Maximum travel distance for events:</Form.Label>
                                    </OverlayTrigger >
                                    <Form.Control type="text" name="travelLimitForLocalEvents" defaultValue={travelLimitForLocalEvents} onChange={(val) => handleTravelLimitForLocalEventsChanged(val.target.value)} />
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderPreferMetricToolTip}>
                                        <Form.Label className="control-label" htmlFor="PreferMetric">Use Metric System:</Form.Label>
                                    </OverlayTrigger >
                                    <ToggleButton
                                        type="checkbox"
                                        variant="outline-dark"
                                        checked={prefersMetric}
                                        value="1"
                                        onChange={(e) => setPrefersMetric(e.currentTarget.checked)}
                                    >
                                        Prefer Metric over Imperial
                                    </ToggleButton>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <hr />
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderDateAgreedToPrivacyPolicyToolTip}>
                                        <Form.Label className="control-label" htmlFor="dateAgreedToPrivacyPolicy">Date Agreed To Privacy Policy:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" disabled defaultValue={dateAgreedToPrivacyPolicy ? dateAgreedToPrivacyPolicy.toString() : ""} />
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderPrivacyPolicyVersionToolTip}>
                                        <Form.Label className="control-label" htmlFor="PrivacyPolicyVersion">Privacy Policy Version:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" disabled defaultValue={privacyPolicyVersion} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderDateAgreedToTermsOfServiceToolTip}>
                                        <Form.Label className="control-label" htmlFor="dateAgreedToTermsOfService">Date Agreed To Terms of Service:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" disabled defaultValue={dateAgreedToTermsOfService ? dateAgreedToTermsOfService.toString() : ""} />
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderTermsOfServiceVersionToolTip}>
                                        <Form.Label className="control-label" htmlFor="TermsOfServiceVersion">Terms Of Service Version:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" disabled defaultValue={termsOfServiceVersion} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderMemberSinceToolTip}>
                                        <Form.Label className="control-label" htmlFor="memberSince">Member Since:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" disabled defaultValue={memberSince ? memberSince.toLocaleString() : ""} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderSourceSystemUserNameToolTip}>
                                        <Form.Label className="control-label" htmlFor="memberSince">Source System User Name:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" disabled defaultValue={sourceSystemUserName} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                    </Form >
                </div>
                <div>
                    <h2>User Media</h2>
                    <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                        <thead>
                            <tr>
                                <th>Media Type</th>
                                <th>Media</th>
                            </tr>
                        </thead>
                        <tbody>
                            {eventMedias.map(eventMedia =>
                                <tr key={eventMedia.id.toString()}>
                                    <td>{eventMedia.mediaUrl}</td>
                                    <td>{getMediaType(mediaTypeList, eventMedia.mediaTypeId)}</td>
                                    <td>
                                        <Button className="action" onClick={() => removeEventMedia(eventMedia.id, eventMedia.mediaUrl)}>Remove Media</Button>
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </div>
            </div >
    );
}

export default withRouter(UserProfile);