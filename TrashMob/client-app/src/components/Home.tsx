import * as React from 'react'

import { MainEvents } from './MainEvents';
import { MainCarousel } from './MainCarousel';
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';
import UserData from './Models/UserData';
import { Button, Modal } from 'reactstrap';
import { CurrentTermsOfServiceVersion } from './TermsOfService';
import { CurrentPrivacyPolicyVersion } from './PrivacyPolicy';
import { Form } from 'react-bootstrap';

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

    return (
        <div>
            <div>
                <Modal isOpen={isOpen} onrequestclose={togglemodal} contentlabel="Accept Terms of Use" fade={true} style={{ width: "500px", display: "block" }}>
                    <div className="container">
                        <Form>
                            <Form.Row>
                                <Form.Group>
                                    <Form.Label>I have reviewed and I agree to the TrashMob.eco <Link to='./termsofservice'>Terms of Use</Link> and the TrashMob.eco <Link to='./privacypolicy'>Privacy Policy</Link>.</Form.Label>
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
            </div>
            <div className="card">
                <h1>Welcome to the TrashMob.eco BETA!</h1>
                <p><small>We're still working out the kinks, but we'd love to get your feedback! Please send us any comments you may have via our Contact Us page, or reach out to us on <a href="https://www.twitter.com/TrashMobEco">Twitter</a>. If you're a developer
                    or designer and interested in improving TrashMob, we'd appreciate the help!</small>
                </p>
            </div>
            <div className="card">
                <MainCarousel />
            </div>
            <div className="card pop">
                <div>
                    <Link to="/createevent">Create a New Event</Link>
                </div>
                <div style={{ width: 100 + '%', margin: '0' }}>
                    <MainEvents eventList={eventList} eventTypeList={eventTypeList} myAttendanceList={myAttendanceList} isEventDataLoaded={isEventDataLoaded} isUserEventDataLoaded={isUserEventDataLoaded} isUserLoaded={isUserLoaded} currentUser={currentUser} onAttendanceChanged={handleAttendanceChanged} />
                </div>
                <div style={{ width: 100 + '%', margin: '0' }}>
                    <AzureMapsProvider>
                        <>
                            <MapController center={center} multipleEvents={eventList} isEventDataLoaded={isEventDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                        </>
                    </AzureMapsProvider>
                </div>
            </div>
        </div>
    );
}

export default withRouter(Home);