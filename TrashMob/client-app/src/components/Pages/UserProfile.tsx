import { ChangeEvent, FC, FormEvent, useEffect, useState } from 'react';
import UserData from '../Models/UserData';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../../store/ToolTips";
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Button, Col, Container, Form, Image, ModalBody, Row } from 'react-bootstrap';
import { Modal } from 'reactstrap';
import * as MapStore from '../../store/MapStore';
import { getKey } from '../../store/MapStore';
import AddressData from '../Models/AddressData';
import { data } from 'azure-maps-control';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import * as Constants from '../Models/Constants';
import MapControllerSinglePoint from '../MapControllerSinglePoint';
import globes from '../assets/gettingStarted/globes.png';
import infoCycle from '../assets/info-circle.svg';

interface UserProfileProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
    onUserUpdated: any;
}

const UserProfile: FC<UserProfileProps> = (props) => {
    const userId = props.currentUser.id;
    const [isDataLoaded, setIsDataLoaded] = useState<boolean>(false);
    const [userName, setUserName] = useState<string>("");
    const [givenName, setGivenName] = useState<string>("");
    const [surname, setSurname] = useState<string>("");
    const [email, setEmail] = useState<string>();
    const [city, setCity] = useState<string>();
    const [radiusType, setRadiusType] = useState<string>("");
    const [country, setCountry] = useState<string>();
    const [region, setRegion] = useState<string>();
    const [postalCode, setPostalCode] = useState<string>();
    const [dateAgreedToTrashMobWaiver, setDateAgreedToTrashMobWaiver] = useState<Date>(new Date());
    const [trashMobWaiverVersion, setTrashMobWaiverVersion] = useState<string>("");
    const [memberSince, setMemberSince] = useState<Date>(new Date());
    const [maxEventsRadiusErrors, setMaxEventsRadiusErrors] = useState<string>("");
    const [userNameErrors, setUserNameErrors] = useState<string>("");
    const [givenNameErrors, setGivenNameErrors] = useState<string>("");
    const [surNameErrors, setSurNameErrors] = useState<string>("");
    const [longitude, setLongitude] = useState<number>(0);
    const [latitude, setLatitude] = useState<number>(0);
    const [prefersMetric, setPrefersMetric] = useState<boolean>(false);
    const [travelLimitForLocalEvents, setTravelLimitForLocalEvents] = useState<number>(10);
    const [isOpen, setIsOpen] = useState(false);
    const [travelLimitForLocalEventsErrors, setTravelLimitForLocalEventsErrors] = useState<string>("");
    const [center, setCenter] = useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = useState<boolean>(false);
    const [mapOptions, setMapOptions] = useState<IAzureMapOptions>();
    const [eventName, setEventName] = useState<string>("User's Base Location");
    const [isSaveEnabled, setIsSaveEnabled] = useState<boolean>(false);
    const [formSubmitted, setFormSubmitted] = useState<boolean>(false);
    const [formSubmitErrors, setFormSubmitErrors] = useState<string>("");
    const [units, setUnits] = useState<string[]>([]);

    useEffect(() => {

        window.scrollTo(0, 0);

        setUnits(["mi", "km"]);
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
                        setUserName(data.userName);
                        setGivenName(data.givenName);
                        setSurname(data.surName);
                        setEmail(data.email);
                        setCity(data.city);
                        setCountry(data.country);
                        setRegion(data.region);
                        setPostalCode(data.postalCode);
                        setDateAgreedToTrashMobWaiver(data.dateAgreedToTrashMobWaiver);
                        setTrashMobWaiverVersion(data.trashMobWaiverVersion);
                        setMemberSince(data.memberSince);
                        setLatitude(data.latitude);
                        setLongitude(data.longitude);
                        setPrefersMetric(data.prefersMetric);
                        setTravelLimitForLocalEvents(data.travelLimitForLocalEvents);
                        setMaxEventsRadiusErrors("");
                        setUserNameErrors("");
                        setGivenNameErrors("");
                        setSurNameErrors("");
                        setTravelLimitForLocalEventsErrors("");

                        if (data.prefersMetric) {
                            setRadiusType("km");
                        }
                        else {
                            setRadiusType("mi");
                        }


                        setIsDataLoaded(true);
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
            travelLimitForLocalEventsErrors !== "") {
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
        userData.userName = userName ?? "";
        userData.givenName = givenName ?? "";
        userData.surName = surname ?? "";
        userData.email = email ?? "";
        userData.city = city ?? "";
        userData.region = region ?? "";
        userData.country = country ?? "";
        userData.postalCode = postalCode ?? "";
        userData.dateAgreedToTrashMobWaiver = new Date(dateAgreedToTrashMobWaiver);
        userData.memberSince = new Date(memberSince);
        userData.latitude = latitude;
        userData.longitude = longitude;
        userData.prefersMetric = prefersMetric;
        userData.travelLimitForLocalEvents = travelLimitForLocalEvents;
        userData.trashMobWaiverVersion = trashMobWaiverVersion;

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
                    props.onUserUpdated();
                }
                else {
                    setFormSubmitErrors("Unknown error occured while checking user name. Please try again. Error Code: " + response.status);
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
                    setUserNameErrors("Unknown error occured while checking user name. Please try again. Error Code: " + response.status);
                }

                validateForm();
            })
        })
    }

    const handleGivenNameChanged = (val: string) => {
        setGivenName(val);
        validateForm();
    }

    const handleSurnameChanged = (val: string) => {
        setSurname(val);
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

    const handleRadiusTypeChanged = (val: string) => {
        if (val === "mi") {
            setPrefersMetric(false);
        }
        else {
            setPrefersMetric(true);
        }

        validateForm();
    }

    const renderUserNameToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileUserName}</Tooltip>
    }

    const renderFirstNameToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileFirstName}</Tooltip>
    }

    const renderLastNameToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileLastName}</Tooltip>
    }

    const renderEmailToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileEmail}</Tooltip>
    }

    const renderCityToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileCity}</Tooltip>
    }

    const renderRegionToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileRegion}</Tooltip>
    }

    const renderPostalCodeToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfilePostalCode}</Tooltip>
    }

    const renderTravelLimitForLocalEventsToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.UserProfileTravelLimitForLocalEvents}</Tooltip>
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
                <Modal isOpen={isOpen} centered onrequestclose={togglemodal} contentlabel="Delete Account?" fade={true} size={"lg"}>
                    <ModalBody>
                        <h2 className='fw-500'>Delete your account?</h2>
                        <p className='p-18'>
                            Are you sure you want to delete your account? This action cannot be undone and you will not be able to reactivate your account, view your past events, or continue building your stats.
                        </p>
                        <div className='d-flex justify-content-end'>
                            <Button className="action h-49 p-18" onClick={() => {
                                togglemodal();
                            }
                            }>
                                Cancel
                            </Button>
                            <Button variant="outline" className='ml-2 border-danger text-danger h-49' onClick={() => {
                                togglemodal();
                                deleteAccount();
                            }
                            }>
                                Delete
                            </Button>
                        </div>
                    </ModalBody>
                </Modal>

                <Container className='bg-white p-4 rounded mt-5'>
                    <h4 className='fw-600 color-primary my-3 main-header'>Account</h4>
                    <Form>
                        <Form.Row>
                            <Col lg={6}>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderUserNameToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="UserName">User Name</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light p-18 h-60' name="userName" defaultValue={userName} onChange={(val) => handleUserNameChanged(val.target.value)} maxLength={parseInt('32')} required />
                                    <span style={{ color: "red" }}>{userNameErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="email">Email <img className='m-0 ml-2' src={infoCycle} alt="info" /></Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light p-18 h-60' disabled defaultValue={email} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderFirstNameToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="FirstName">First Name</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light p-18 h-60' name="firstName" defaultValue={givenName} onChange={(val) => handleGivenNameChanged(val.target.value)} maxLength={parseInt('32')} />
                                    <span style={{ color: "red" }}>{givenNameErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderLastNameToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="lastName">Last Name</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light p-18 h-60' name="lastName" defaultValue={surname} onChange={(val) => handleSurnameChanged(val.target.value)} maxLength={parseInt('32')} />
                                    <span style={{ color: "red" }}>{surNameErrors}</span>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Label className="control-label font-weight-bold h5" htmlFor="Password">Password</Form.Label>
                                <Form.Group>
                                </Form.Group>
                                <Button variant="outline" className='text-center p-18 h-49'>
                                    Reset password
                                </Button>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group className='text-right'>
                                    <Button className="action h-49 p-18" onClick={(e) => handleCancel(e)}>Discard</Button>
                                    <Button disabled={!isSaveEnabled} type="submit" className="action btn-outline ml-2 h-49" variant="outline-primary">Save</Button>
                                </Form.Group>
                                <span>{formSubmitted ? 'Saved!' : ''}</span>
                                <span>{formSubmitErrors ? formSubmitErrors : ''}</span>
                            </Col>
                        </Form.Row>
                    </Form>
                </Container>

                <Container className='p-4 bg-white mt-5 rounded'>
                    <h4 className='fw-600 color-primary my-3 main-header'>Location preferences</h4>
                    <Form onSubmit={handleSave}>
                        <Form.Row>
                            <AzureMapsProvider>
                                <>
                                    <MapControllerSinglePoint center={center} isEventDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={eventName} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} isDraggable={true} eventDate={new Date()} />
                                </>
                            </AzureMapsProvider>
                        </Form.Row>
                        <Form.Row className='mt-4'>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderTravelLimitForLocalEventsToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="maxEventsRadius">Maximum event radius <img className='m-0 ml-2' src={infoCycle} alt="info" /></Form.Label>
                                    </OverlayTrigger>
                                    <Row>
                                        <Col xs={8}>
                                            <Form.Control type="number" className='border-0 bg-light p-18 h-60' w-100 name="maxEventsRadius" defaultValue={travelLimitForLocalEvents} onChange={(val) => handleTravelLimitForLocalEventsChanged(val.target.value)} maxLength={parseInt('32')} />
                                        </Col>
                                        <Col xs={4}>
                                            <select data-val="true" className='bg-light border-0 p-18 h-60 w-100 rounded p-2' name="radiusType" value={radiusType} onChange={(val) => handleRadiusTypeChanged(val.target.value)} required>
                                                <option value="">-- Select Units --</option>
                                                {
                                                    units.map(unit =>
                                                        <option key={unit} value={unit}>{unit}</option>
                                                )}
                                            </select>
                                        </Col>
                                    </Row>
                                    <span style={{ color: "red" }}>{maxEventsRadiusErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderCityToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="City">City</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light p-18 h-60' disabled name="city" defaultValue={city} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderRegionToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="region">State</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light p-18 h-60' disabled name="region" defaultValue={region} />
                                </Form.Group>
                            </Col>
                            <Col lg={6}>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderPostalCodeToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5" htmlFor="PostalCode">Postal Code</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light p-18 h-60' disabled name="postalCode" defaultValue={postalCode} />
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group className='text-right'>
                                    <Button className="action h-49 p-18" onClick={(e) => handleCancel(e)}>Discard</Button>
                                    <Button disabled={!isSaveEnabled} type="submit" className="action btn-outline ml-2 h-49" variant="outline-primary">Save</Button>
                                </Form.Group>
                                <span>{formSubmitted ? 'Saved!' : ''}</span>
                                <span>{formSubmitErrors ? formSubmitErrors : ''}</span>
                            </Col>
                        </Form.Row>
                    </Form>
                </Container>
                <Container className='p-0'>
                    <div className='d-flex justify-content-end'>
                        <Button className='mx-0 my-5 border border-danger text-danger h-49 p-18' variant="outline" onClick={(e) => handleDelete(e)}>Delete Account</Button>

                    </div>
                </Container>
            </div >
    );
}

export default withRouter(UserProfile);