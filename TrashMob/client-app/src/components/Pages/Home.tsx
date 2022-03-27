import * as React from 'react'

import { MainEvents } from '../MainEvents';
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import EventData from '../Models/EventData';
import EventTypeData from '../Models/EventTypeData';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import { data, setView } from 'azure-maps-control';
import * as MapStore from '../../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from '../MapController';
import UserData from '../Models/UserData';
import { Button, Modal } from 'reactstrap';
import { CurrentTermsOfServiceVersion } from '../TermsOfService';
import { CurrentPrivacyPolicyVersion } from '../PrivacyPolicy';
import { Col, Container, Form, Image, Row } from 'react-bootstrap';
import Drawings from '../assets/Drawings.png';
import Trash from '../assets/jeremy-bezanger-u5mCQ-c5oSI-unsplash2.jpg';
import Globe2 from '../assets/globe2.png';
import Logo from '../assets/logo.svg';
import Calendar from '../assets/Calendar.svg';
import Trashbag from '../assets/Trashbag.svg';
import Person from '../assets/Person.svg';
import Clock from '../assets/Clock.svg';
import { GettingStartedSection } from '../GettingStartedSection';

export interface HomeProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
    onUserUpdated: any;
}

const Home: React.FC<HomeProps> = (props) => {
    const [eventList, setEventList] = React.useState<EventData[]>([]);
    const [eventTypeList, setEventTypeList] = React.useState<EventTypeData[]>([]);
    const [myAttendanceList, setMyAttendanceList] = React.useState<EventData[]>([]);
    const [isEventDataLoaded, setIsEventDataLoaded] = React.useState(false);
    const [isUserEventDataLoaded, setIsUserEventDataLoaded] = React.useState(false);
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState(false);
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [currentUser, setCurrentUser] = React.useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);
    const [agree, setAgree] = React.useState(false);
    const [isOpen, setIsOpen] = React.useState(false);
    const [eventView, setEventView] = React.useState<string>('map');

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');
        fetch('/api/eventtypes', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setEventTypeList(data);
            });

        fetch('/api/Events/active', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<EventData[]>)
            .then(data => {
                setEventList(data);
                setIsEventDataLoaded(true);
            });

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })

        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                var point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point)
            });
        } else {
            console.log("Not Available");
        }
    }, [])

    React.useEffect(() => {

        setCurrentUser(props.currentUser);
        setIsUserLoaded(props.isUserLoaded);

        if (!props.isUserLoaded || !props.currentUser) {
            return;
        }

        // If the user is logged in, get the events they are attending
        var accounts = msalClient.getAllAccounts();

        if (accounts !== null && accounts.length > 0) {
            var request = {
                scopes: apiConfig.b2cScopes,
                account: accounts[0]
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/events/eventsuserisattending/' + props.currentUser.id, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<EventData[]>)
                    .then(data => {
                        setMyAttendanceList(data);
                        setIsUserEventDataLoaded(true);
                    })
            });
        }
    }, [props.isUserLoaded, props.currentUser]);

    React.useEffect(() => {
        if (!isUserLoaded || !currentUser) {
            return;
        }

        var isPrivacyPolicyOutOfDate = currentUser.dateAgreedToPrivacyPolicy < CurrentPrivacyPolicyVersion.versionDate;
        var isTermsOfServiceOutOfDate = currentUser.dateAgreedToTermsOfService < CurrentTermsOfServiceVersion.versionDate;

        if (isPrivacyPolicyOutOfDate || isTermsOfServiceOutOfDate || (currentUser.termsOfServiceVersion === "") || (currentUser.privacyPolicyVersion === "")) {
            setIsOpen(true);
        }
    }, [isUserLoaded, currentUser]);

    function handleLocationChange(point: data.Position) {
        // do nothing
    }

    function handleAttendanceChanged() {
        setIsUserEventDataLoaded(false);

        if (!props.isUserLoaded || !props.currentUser) {
            return;
        }

        // If the user is logged in, get the events they are attending
        var accounts = msalClient.getAllAccounts();

        if (accounts !== null && accounts.length > 0) {
            var request = {
                scopes: apiConfig.b2cScopes,
                account: accounts[0]
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/events/eventsuserisattending/' + props.currentUser.id, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<EventData[]>)
                    .then(data => {
                        setMyAttendanceList(data);
                        setIsUserEventDataLoaded(true);
                    })
            });
        }
    }

    function checkboxhandler() {
        // if agree === true, it will be set to false
        // if agree === false, it will be set to true
        setAgree(!agree);
    }

    function togglemodal() {
        setIsOpen(!isOpen);
    }

    function handleDetailsSelected(eventId: string) {
        props.history.push("eventdetails/" + eventId);
    }

    function updateAgreements(tosVersion: string, privacyVersion: string) {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/Users/' + currentUser.id, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<UserData> | null)
                .then(user => {
                    if (user) {
                        user.dateAgreedToPrivacyPolicy = new Date();
                        user.dateAgreedToTermsOfService = new Date();
                        user.termsOfServiceVersion = tosVersion;
                        user.privacyPolicyVersion = privacyVersion;
                        fetch('/api/Users/', {
                            method: 'PUT',
                            headers: headers,
                            body: JSON.stringify(user)
                        })
                            .then(response => response.json() as Promise<UserData>)
                            .then(data => {
                                setCurrentUser(data);
                                props.onUserUpdated();
                                if (!currentUser.userName) {
                                    props.history.push("/userprofile");
                                }
                            })
                    }
                })
        })
    }

    function handleEventView(view: string) {
        setEventView(view);
    }

    return (
        <>
            <Modal isOpen={isOpen} onrequestclose={togglemodal} contentlabel="Accept Terms of Use" fade={true} style={{ width: "500px", display: "block" }}>
                <div className="container p-4">
                    <Form>
                        <Form.Row>
                            <Form.Group>
                                <Form.Label className="control-label">I have reviewed and I agree to the TrashMob.eco <Link to='./termsofservice'>Terms of Use</Link> and the TrashMob.eco <Link to='./privacypolicy'>Privacy Policy</Link>.</Form.Label>
                                <Form.Check id="agree" onChange={checkboxhandler} label="Yes" />
                            </Form.Group>
                        </Form.Row>
                        <Form.Row>
                            <Button disabled={!agree} className="action" onClick={() => {
                                updateAgreements(CurrentTermsOfServiceVersion.versionId, CurrentPrivacyPolicyVersion.versionId);
                                togglemodal();
                            }
                            }>
                                I Agree
                            </Button>
                        </Form.Row>
                    </Form>
                </div>
            </Modal>
            <Container fluid>
                <Row className="shadow position-relative" >
                    <Col className="d-flex flex-column px-0 py-4 pl-lg-5" sm={6} style={{ zIndex: 1 }}>
                        <div className="ml-sm-2 ml-lg-5 pl-sm-3 pl-md-5 mt-md-5 mb-md-2">
                            <img src={Logo} alt="TrashMob.eco logo" className="banner-logo"></img>
                            <h3 className="ml-md-4 mt-4 mb-4 mb-md-5 font-weight-bold font-size-xl banner-heading pl-3">Meet up. Clean up. Feel good.</h3>
                            <Link className="btn btn-primary ml-5 py-md-3 banner-button" to="/gettingstarted">Join us today</Link>
                        </div>
                    </Col>
                    <img src={Globe2} className="position-absolute p-0 m-0 h-100 banner-globe" alt="Image of globe" ></img>
                </Row>
            </Container>
            <div className="bg-white pb-4"><Image src={Drawings} alt="Drawings of trash" /></div>
            <Container fluid className="bg-white">
                <Row className="py-5 d-flex justify-content-around">
                    <div className="mx-auto d-flex flex-column flex-lg-row container-lg px-0">
                        <div className="d-flex flex-column w-50 mx-auto pr-lg-4">
                            <h4 className="mt-0 font-weight-bold">What is a TrashMob?</h4>
                            <p>A TrashMob is a group of citizens who are willing to take a hour or two out of their lives to get together and clean up their communities. Start your impact today.</p>
                            <Link className="mt-2 btn btn-primary btn-128" to="/aboutus" role="button">Learn more</Link>
                        </div>
                        <div className="w-50 d-flex align-items-center mx-auto mt-4 mt-md-0"><Image src={Trash} alt="Hands putting trash in trash bag" /></div>
                    </div>
                </Row>
            </Container>
            <Container className="d-flex justify-content-around my-5 py-5 flex-column flex-md-row">
                <div className="d-flex flex-column justify-content-center text-center">
                    <img src={Calendar} alt="Calendar icon" className="w-auto mx-auto mb-3" />
                    <span className="font-weight-bold font-size-lg">8</span>
                    <span className="font-weight-bold">Events Hosted</span>
                </div>
                <div className="d-flex flex-column justify-content-center text-center">
                    <img src={Trashbag} alt="Trashbag icon" className="w-auto mx-auto mb-3" />
                    <span className="font-weight-bold font-size-lg">8</span>
                    <span className="font-weight-bold">Bags Collected</span>
                </div>
                <div className="d-flex flex-column justify-content-center text-center">
                    <img src={Person} alt="Person icon" className="w-auto mx-auto mb-3" />
                    <span className="font-weight-bold font-size-lg">8</span>
                    <span className="font-weight-bold">Participants</span>
                </div>
                <div className="d-flex flex-column justify-content-center text-center">
                    <img src={Clock} alt="Clock icon" className="w-auto mx-auto mb-3" />
                    <span className="font-weight-bold font-size-lg">8</span>
                    <span className="font-weight-bold">Hours Spent</span>
                </div>
            </Container>
            <Container fluid className="bg-white p-md-5">
                <div className="max-width-container mx-auto">
                    <div className="d-flex justify-content-between mb-4 flex-wrap flex-md-nowrap">
                        <h3 className="font-weight-bold flex-grow-1">Upcoming Events</h3>
                        <div className="d-flex align-items-center mt-4">
                            <label className="pr-3 mb-0">
                                <input type="radio" className="mb-0 radio" name="Event view" value="map" onChange={e => handleEventView(e.target.value)} checked={eventView === "map"}></input>
                                <span className="px-2">Map view</span>
                            </label>
                            <label className="mb-0">
                                <input type="radio" className="mb-0 radio" name="Event view" value="list" onChange={e => handleEventView(e.target.value)} checked={eventView === "list"}></input>
                                <span className="px-2">List view</span>
                            </label>
                        </div>
                    </div>
                    {eventView === 'map' ? (
                        <>
                            <Link to="/manageeventdashboard">Create a New Event</Link>
                            <div className="w-100 m-0">
                                <AzureMapsProvider>
                                    <>
                                        <MapController center={center} multipleEvents={eventList} myAttendanceList={myAttendanceList} isUserEventDataLoaded={isUserEventDataLoaded} isEventDataLoaded={isEventDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} onAttendanceChanged={handleAttendanceChanged} onDetailsSelected={handleDetailsSelected} />
                                    </>
                                </AzureMapsProvider>
                            </div>
                        </>
                    ) : (
                        <div className="container-lg">
                            <MainEvents eventList={eventList} eventTypeList={eventTypeList} myAttendanceList={myAttendanceList} isEventDataLoaded={isEventDataLoaded} isUserEventDataLoaded={isUserEventDataLoaded} isUserLoaded={isUserLoaded} currentUser={currentUser} onAttendanceChanged={handleAttendanceChanged} />
                        </div>
                    )}
                </div>
            </Container>
            <GettingStartedSection />
        </>
    );
}

export default withRouter(Home);