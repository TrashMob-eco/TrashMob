import { ChangeEvent, FC, FormEvent, useEffect, useState } from 'react';
import UserData from '../Models/UserData';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../../store/ToolTips";
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { CountryDropdown, RegionDropdown } from 'react-country-region-selector';
import { Button, Col, Container, Form, Image, Row, ToggleButton } from 'react-bootstrap';
import { Modal } from 'reactstrap';
import * as MapStore from '../../store/MapStore';
import { getKey } from '../../store/MapStore';
import AddressData from '../Models/AddressData';
import { data } from 'azure-maps-control';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import * as Constants from '../Models/Constants';
import MapControllerSinglePoint from '../MapControllerSinglePoint';
import EventMediaData from '../Models/EventMediaData';
import MediaTypeData from '../Models/MediaTypeData';
import { getMediaType } from '../../store/mediaTypeHelper';
import globes from '../assets/gettingStarted/globes.png';

interface UserProfileProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const UserProfile: FC<UserProfileProps> = (props) => {
    const userId = props.currentUser.id;
    const [isDataLoaded, setIsDataLoaded] = useState<boolean>(false);
    const [nameIdentifier, setNameIdentifier] = useState<string>("");
    const [userName, setUserName] = useState<string>("");
    const [sourceSystemUserName, setSourceSystemUserName] = useState<string>("");
    const [givenName, setGivenName] = useState<string>("");
    const [surName, setSurName] = useState<string>("");
    const [email, setEmail] = useState<string>();
    const [city, setCity] = useState<string>();
    const [country, setCountry] = useState<string>();
    const [region, setRegion] = useState<string>();
    const [postalCode, setPostalCode] = useState<string>();
    const [dateAgreedToPrivacyPolicy, setDateAgreedToPrivacyPolicy] = useState<Date>(new Date());
    const [dateAgreedToTermsOfService, setDateAgreedToTermsOfService] = useState<Date>(new Date());
    const [privacyPolicyVersion, setPrivacyPolicyVersion] = useState<string>("");
    const [termsOfServiceVersion, setTermsOfServiceVersion] = useState<string>("");
    const [memberSince, setMemberSince] = useState<Date>(new Date());
    const [userNameErrors, setUserNameErrors] = useState<string>("");
    const [givenNameErrors, setGivenNameErrors] = useState<string>("");
    const [surNameErrors, setSurNameErrors] = useState<string>("");
    const [cityErrors, setCityErrors] = useState<string>("");
    const [countryErrors, setCountryErrors] = useState<string>("");
    const [regionErrors, setRegionErrors] = useState<string>("");
    const [postalCodeErrors, setPostalCodeErrors] = useState<string>("");
    const [longitude, setLongitude] = useState<number>(0);
    const [latitude, setLatitude] = useState<number>(0);
    const [prefersMetric, setPrefersMetric] = useState<boolean>(false);
    const [isOptedOutOfAllEmails, setIsOptedOutOfAllEmails] = useState<boolean>(false);
    const [travelLimitForLocalEvents, setTravelLimitForLocalEvents] = useState<number>(10);
    const [isOpen, setIsOpen] = useState(false);
    const [latitudeErrors, setLatitudeErrors] = useState<string>("");
    const [longitudeErrors, setLongitudeErrors] = useState<string>("");
    const [travelLimitForLocalEventsErrors, setTravelLimitForLocalEventsErrors] = useState<string>("");
    const [center, setCenter] = useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = useState<boolean>(false);
    const [mapOptions, setMapOptions] = useState<IAzureMapOptions>();
    const [eventName, setEventName] = useState<string>("User's Base Location");
    const [eventMedias, setEventMedias] = useState<EventMediaData[]>([]);
    const [mediaTypeList, setMediaTypeList] = useState<MediaTypeData[]>([]);
    const [isSaveEnabled, setIsSaveEnabled] = useState<boolean>(false);
    const [formSubmitted, setFormSubmitted] = useState<boolean>(false);
    const [formSubmitErrors, setFormSubmitErrors] = useState<string>("");

    useEffect(() => {
        if (props.isUserLoaded && !isDataLoaded) {
            const account = msalClient.getAllAccounts()[0];
            setEventName("User's Base Location");

            const request = {
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
                const point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point)
            });
        }
    }, [userId, props.isUserLoaded, isDataLoaded])

    const togglemodal = () => {
        setIsOpen(!isOpen);
    }

    // This will handle Cancel button click event.  
    const handleCancel = (event: FormEvent<HTMLElement>) => {
        event.preventDefault();
        props.history.push("/");
    }

    const handleDelete = (event: FormEvent<HTMLElement>) => {
        event.preventDefault();
        setIsOpen(true);
    }

    // This will handle the delete account
    const deleteAccount = () => {

        const account = msalClient.getAllAccounts()[0];

        const request = {
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

    const validateForm = () => {
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
    const handleSave = (event: ChangeEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        const userData = new UserData();

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

        const usrdata = JSON.stringify(userData);

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];

        const request = {
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
            }).then(response => {
                if (response.status === 200) {
                    setFormSubmitted(true);
                }
                else {
                    setFormSubmitErrors("Unknown error occured while hecking user name. Please try again. Error Code: " + response.status);
                }
            });
        })
    }

    const handleUserNameChanged = (val: string) => {
        if (!val || val.length === 0) {
            setUserNameErrors("Username cannot be empty. Username can consist of Letters A-Z (upper or lowercase), Numbers (0-9), and underscores (_)");
            validateForm();
            return;
        }

        const pattern = new RegExp(Constants.RegexUserName);

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
        const request = {
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

    const handleGivenNameChanged = (val: string) => {
        setGivenName(val);
        validateForm();
    }

    const handleSurNameChanged = (val: string) => {
        setSurName(val);
        validateForm();
    }

    const handleCityChanged = (val: string) => {
        setCity(val);
        validateForm();
    }

    const selectCountry = (val: string) => {
        setCountry(val);
        validateForm();
    }

    const selectRegion = (val: string) => {
        setRegion(val);
        validateForm();
    }

    const handlePostalCodeChanged = (val: string) => {
        setPostalCode(val);
        validateForm();
    }

    const handleLatitudeChanged = (val: string) => {
        try {
            if (val) {
                const floatVal = parseFloat(val);

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

    const handleLongitudeChanged = (val: string) => {
        try {
            if (val) {
                const floatVal = parseFloat(val);

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

    const handleTravelLimitForLocalEventsChanged = (val: string) => {
        try {
            if (val) {
                const intVal = parseInt(val);

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

    const renderUserNameToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileUserName}</Tooltip>
    }

    const renderGivenNameToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileGivenName}</Tooltip>
    }

    const renderSurNameToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileSurName}</Tooltip>
    }

    const renderEmailToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileEmail}</Tooltip>
    }

    const renderCityToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileCity}</Tooltip>
    }

    const renderCountryToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileCountry}</Tooltip>
    }

    const renderRegionToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileRegion}</Tooltip>
    }

    const renderPostalCodeToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfilePostalCode}</Tooltip>
    }

    const renderDateAgreedToPrivacyPolicyToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileDateAgreedToPrivacyPolicy}</Tooltip>
    }

    const renderPrivacyPolicyVersionToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfilePrivacyPolicyVersion}</Tooltip>
    }

    const renderDateAgreedToTermsOfServiceToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileDateAgreedToTermsOfService}</Tooltip>
    }

    const renderTermsOfServiceVersionToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileTermsOfServiceVersion}</Tooltip>
    }

    const renderMemberSinceToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileMemberSince}</Tooltip>
    }

    const renderSourceSystemUserNameToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileSourceSystemUserName}</Tooltip>
    }

    const renderTravelLimitForLocalEventsToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileTravelLimitForLocalEvents}</Tooltip>
    }

    const renderUserLatitudeToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileLatitude}</Tooltip>
    }

    const renderUserLongitudeToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileLongitude}</Tooltip>
    }

    const renderPreferMetricToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfilePreferMetric}</Tooltip>
    }

    const handleLocationChange = (point: data.Position) => {
        // In an Azure Map point, the longitude is the first position, and latitude is second
        setLatitude(point[1]);
        setLongitude(point[0]);
        const locationString = point[1] + ',' + point[0]
        const headers = getDefaultHeaders('GET');

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

    const onEventMediasUpdated = ()=> {
        const account = msalClient.getAllAccounts()[0];

        const request = {
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

    const removeEventMedia = (eventMediaId: string, mediaUrl: string) => {
        if (!window.confirm("Please confirm that you want to remove Event Media with Url: '" + mediaUrl + "' as a media from this Event?"))
            return;
        else {
            const account = msalClient.getAllAccounts()[0];

            const request = {
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
                <Container fluid className='bg-grass shadow'>
                    <Row className="text-center pt-0">
                        <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                            <h1 className='font-weight-bold'>About TrashMob</h1>
                            <p className="font-weight-bold">Ideas Inspired by simple action.</p>
                        </Col>
                        <Col md={5}>
                            <Image src={globes} alt="globes" className="h-100 mt-0" />
                        </Col>
                    </Row>
                </Container>
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
                <div className="container-fluid p-5" >
                    <Form onSubmit={handleSave} >
                        <Row className="p-5">
                            <Col lg={8}>
                                <Form.Row>
                                    <Col>
                                        <Form.Group className="required">
                                            <OverlayTrigger placement="top" overlay={renderUserNameToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="UserName">User Name:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" name="userName" defaultValue={userName} onChange={(val) => handleUserNameChanged(val.target.value)} maxLength={parseInt('32')} required />
                                            <span style={{ color: "red" }}>{userNameErrors}</span>
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="email">Email:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" disabled defaultValue={email} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderGivenNameToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="GivenName">Given Name:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" name="givenName" defaultValue={givenName} onChange={(val) => handleGivenNameChanged(val.target.value)} maxLength={parseInt('32')} />
                                            <span style={{ color: "red" }}>{givenNameErrors}</span>
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderSurNameToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="SurName">Surname:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" name="surName" defaultValue={surName} onChange={(val) => handleSurNameChanged(val.target.value)} maxLength={parseInt('32')} />
                                            <span style={{ color: "red" }}>{surNameErrors}</span>
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderCityToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="City">City:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control className="form-control" type="text" disabled name="city" defaultValue={city} onChange={(val) => handleCityChanged(val.target.value)} maxLength={parseInt('64')} />
                                            <span style={{ color: "red" }}>{cityErrors}</span>
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderPostalCodeToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="PostalCode">Postal Code:</Form.Label>
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
                                                <Form.Label className="control-label font-weight-bold" htmlFor="Country">Country:</Form.Label>
                                            </OverlayTrigger>
                                            <CountryDropdown disabled name="country" value={country ?? ""} onChange={(val) => selectCountry(val)} />
                                            <span style={{ color: "red" }}>{countryErrors}</span>
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderRegionToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="region">Region:</Form.Label>
                                            </OverlayTrigger>
                                            <RegionDropdown disabled
                                                country={country ?? ""}
                                                value={region ?? ""}
                                                onChange={(val) => selectRegion(val)}
                                                classes="w-100"
                                            />
                                            <span style={{ color: "red" }}>{regionErrors}</span>
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderUserLatitudeToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="Latitude">Latitude:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" disabled name="latitude" value={latitude} onChange={(val) => handleLatitudeChanged(val.target.value)} />
                                            <span style={{ color: "red" }}>{latitudeErrors}</span>
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderUserLongitudeToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="Longitude">Longitude:</Form.Label>
                                            </OverlayTrigger >
                                            <Form.Control type="text" disabled name="longitude" value={longitude} onChange={(val) => handleLongitudeChanged(val.target.value)} />
                                            <span style={{ color: "red" }}>{longitudeErrors}</span>
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderTravelLimitForLocalEventsToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="TravelLimitForLocalEvents">Maximum travel distance for events:</Form.Label>
                                            </OverlayTrigger >
                                            <Form.Control type="text" name="travelLimitForLocalEvents" defaultValue={travelLimitForLocalEvents} onChange={(val) => handleTravelLimitForLocalEventsChanged(val.target.value)} />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderPreferMetricToolTip}>
                                                <Form.Label className="control-label font-weight-bold w-100" htmlFor="PreferMetric">Use Metric System:</Form.Label>
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
                                        <hr />
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderDateAgreedToPrivacyPolicyToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="dateAgreedToPrivacyPolicy">Date Agreed To Privacy Policy:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" disabled defaultValue={dateAgreedToPrivacyPolicy ? dateAgreedToPrivacyPolicy.toString() : ""} />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderPrivacyPolicyVersionToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="PrivacyPolicyVersion">Privacy Policy Version:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" disabled defaultValue={privacyPolicyVersion} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderDateAgreedToTermsOfServiceToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="dateAgreedToTermsOfService">Date Agreed To Terms of Service:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" disabled defaultValue={dateAgreedToTermsOfService ? dateAgreedToTermsOfService.toString() : ""} />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderTermsOfServiceVersionToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="TermsOfServiceVersion">Terms Of Service Version:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" disabled defaultValue={termsOfServiceVersion} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderMemberSinceToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="memberSince">Member Since:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" disabled defaultValue={memberSince ? memberSince.toLocaleString() : ""} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderSourceSystemUserNameToolTip}>
                                                <Form.Label className="control-label font-weight-bold" htmlFor="memberSince">Source System User Name:</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" disabled defaultValue={sourceSystemUserName} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <Button disabled={!isSaveEnabled} type="submit" className="action btn-outline mr-2" variant="outline-primary">Save</Button>
                                            <Button className="action" onChange={(e) => handleCancel(e)}>Cancel</Button>
                                        </Form.Group>
                                        <span>{formSubmitted ? 'Saved!' : ''}</span>
                                        <span>{formSubmitErrors ? formSubmitErrors : ''}</span>
                                    </Col>
                                </Form.Row>
                            </Col>
                            <Col lg={4}>
                                <div className="bg-white px-4 py-3 d-flex justify-content-between flex-column h-25">
                                    <div>
                                        <p className="font-size-lg color-primary">Delete my account</p>
                                        <p className="font-grey">If you delete your account, you won't be able to reactivate it, or retrieve any of your content or events.</p>
                                    </div>
                                    <Button variant="danger" onChange={(e) => handleDelete(e)}>Delete Account</Button>
                                </div>
                            </Col>

                        </Row>
                        <Row className="container p-5">
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
                        </Row>
                    </Form >
                </div>
            </div >
    );
}

export default withRouter(UserProfile);