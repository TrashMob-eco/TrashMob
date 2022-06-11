import { FC, useEffect, useState } from 'react'
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import { Col, Container, Dropdown, Image, Row } from 'react-bootstrap';
import EventData from '../Models/EventData';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import { data } from 'azure-maps-control';
import * as MapStore from '../../store/MapStore';
import MapControllerPointCollection from '../MapControllerPointCollection';
import UserData from '../Models/UserData';
import globes from '../assets/gettingStarted/globes.png';
import { Table } from '../Table';
import twofigure from '../assets/card/twofigure.svg';
import calendarclock from '../assets/card/calendarclock.svg';
import bucketplus from '../assets/card/bucketplus.svg';
import { Eye, PersonX, Link as LinkIcon, Pencil } from 'react-bootstrap-icons';
import StatsData from '../Models/StatsData';

interface MyDashboardProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const MyDashboard: FC<MyDashboardProps> = (props) => {
    const [myEventList, setMyEventList] = useState<EventData[]>([]);
    const [isEventDataLoaded, setIsEventDataLoaded] = useState<boolean>(false);
    const [center, setCenter] = useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = useState<boolean>(false);
    const [mapOptions, setMapOptions] = useState<IAzureMapOptions>();
    const [currentUser, setCurrentUser] = useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = useState<boolean>(props.isUserLoaded);
    const [reloadEvents, setReloadEvents] = useState<number>(0);
    const [upcomingEventsMapView, setUpcomingEventsMapView] = useState<boolean>(false);
    const [pastEventsMapView, setPastEventsMapView] = useState<boolean>(false);
    const [copied, setCopied] = useState(false);
    const [totalBags, setTotalBags] = useState<number>(0);
    const [totalHours, setTotalHours] = useState<number>(0);
    const [totalEvents, setTotalEvents] = useState<number>(0);

    useEffect(() => {
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
    }, []);

    useEffect(() => {
        if (props.isUserLoaded) {

            setCurrentUser(props.currentUser);
            setIsUserLoaded(props.isUserLoaded);

            setIsEventDataLoaded(false);
            const account = msalClient.getAllAccounts()[0];

            const request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/events/userevents/' + props.currentUser.id + '/false', {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<EventData[]>)
                    .then(data => {
                        setMyEventList(data);
                        setIsEventDataLoaded(true);
                    });

                fetch('/api/stats/' + props.currentUser.id, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<StatsData>)
                    .then(data => {
                        setTotalBags(data.totalBags);
                        setTotalHours(data.totalHours);
                        setTotalEvents(data.totalEvents);
                    }
                    )
            });
        }
    }, [reloadEvents, props.currentUser, props.currentUser.id, props.isUserLoaded]);

    const handleLocationChange = (point: data.Position) => {
        // do nothing
    }

    const handleAttendanceChanged = (point: data.Position) => {
        // do nothing
    }

    const handleDetailsSelected = (eventId: string) => {
        props.history.push("eventdetails/" + eventId);
    }

    const handleReloadEvents = () => {
        // A trick to force the reload as needed.
        setReloadEvents(reloadEvents + 1);
    }

    const handleEventView = (view: string, table: string) => {
        if (table === 'Upcoming events') {
            if (view === 'list') {
                return setUpcomingEventsMapView(false);
            }
            return setUpcomingEventsMapView(true);
        } else {
            if (view === 'list') {
                return setPastEventsMapView(false);
            }
            return setPastEventsMapView(true);
        }
    }

    const handleCopyLink = (eventId: string) => {
        navigator.clipboard.writeText(window.location.origin + '/eventdetails/' + eventId);
        setCopied(true);
        setTimeout(() => {
            setCopied(false);
        }, 2000)
    }

    const handleUnregisterEvent = (id: string, name: string) => {
        if (!window.confirm("Do you want to remove yourself from this event: " + name + "?"))
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

                fetch('/api/EventAttendees/' + id + '/' + props.currentUser.id, {
                    method: 'delete',
                    headers: headers
                }).then(() => { handleReloadEvents(); })
            });
        }
    }

    const attendeeActionDropdownList = (eventId: string) => {
        return (
            <>
                <Dropdown.Item href={'/eventdetails/' + eventId}><Eye />View event</Dropdown.Item>
                <Dropdown.Item onClick={() => handleUnregisterEvent(eventId, props.currentUser.givenName)}><PersonX />Unregister for event</Dropdown.Item>
                <Dropdown.Item onClick={() => handleCopyLink(eventId)}><LinkIcon />{copied ? 'Copied!' : 'Copy event link'}</Dropdown.Item>
            </>
        )
    }

    const eventOwnerActionDropdownList = (eventId: string) => {
        return (
            <>
                <Dropdown.Item href={'./'}><Pencil />Manage event</Dropdown.Item>
                <Dropdown.Item href={'/eventdetails/' + eventId}><Eye />View event</Dropdown.Item>
                <Dropdown.Item onClick={() => handleCopyLink(eventId)}><LinkIcon />{copied ? 'Copied!' : 'Copy event link'}</Dropdown.Item>
            </>
        )
    }

    const UpcomingEventsTable = () => {
        const headerTitles = ['Name', 'Role', 'Date', 'Time', 'Location', 'Action']
        return (
            <div className="bg-white p-3 px-4">
                <Table columnHeaders={headerTitles} >
                    {myEventList.sort((a, b) => (a.eventDate < b.eventDate) ? 1 : -1).map(event => {
                        if (new Date(event.eventDate) >= new Date()) {
                            return (
                                <tr key={event.id.toString()}>
                                    <td>{event.name}</td>
                                    <td>{event.createdByUserId === props.currentUser.id ? 'Lead' : ' Attendee'}</td>
                                    <td>{new Date(event.eventDate).toLocaleDateString("en-us", {
                                        year: "numeric",
                                        month: "2-digit",
                                        day: "2-digit"
                                    })}</td>
                                    <td>{new Date(event.eventDate).toLocaleTimeString("en-us", { hour12: true, hour: 'numeric', minute: '2-digit' })}</td>
                                    <td>{event.streetAddress}, {event.city}</td>
                                    <td className="btn py-0">
                                        <Dropdown role="menuitem">
                                            <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                            <Dropdown.Menu id="share-menu">
                                                {event.createdByUserId === props.currentUser.id ? eventOwnerActionDropdownList(event.id) : attendeeActionDropdownList(event.id)}
                                            </Dropdown.Menu>
                                        </Dropdown>
                                    </td>
                                </tr>
                            )
                        } else {
                            return (<></>)
                        }
                    }
                    )}
                </Table>
            </div >
        );
    }

    const PastEventsTable = () => {
        const headerTitles = ['Name', 'Role', 'Date', 'Time', 'Location', 'Action']
        return (
            <div className="bg-white p-3 px-4">
                <Table columnHeaders={headerTitles}>
                    {pastEventsMapView ?
                        <AzureMapsProvider>
                            <MapControllerPointCollection center={center} multipleEvents={myEventList} isEventDataLoaded={isEventDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} onAttendanceChanged={handleAttendanceChanged} myAttendanceList={myEventList} isUserEventDataLoaded={isEventDataLoaded} onDetailsSelected={handleDetailsSelected} history={props.history} location={props.location} match={props.match} />
                        </AzureMapsProvider>
                        : myEventList.sort((a, b) => (a.eventDate < b.eventDate) ? 1 : -1).map(event => {
                            if (new Date(event.eventDate) < new Date()) {
                                return (
                                    <tr key={event.id.toString()}>
                                        <td>{event.name}</td>
                                        <td>{event.createdByUserId === props.currentUser.id ? 'Lead' : ' Attendee'}</td>
                                        <td>{new Date(event.eventDate).toLocaleDateString("en-us", {
                                            year: "numeric",
                                            month: "2-digit",
                                            day: "2-digit"
                                        })}</td>
                                        <td>{new Date(event.eventDate).toLocaleTimeString("en-us", { hour12: true, hour: 'numeric', minute: '2-digit' })}</td>
                                        <td>{event.streetAddress}, {event.city}</td>
                                        <td className="btn py-0">
                                            <Dropdown role="menuitem">
                                                <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                                <Dropdown.Menu id="share-menu">
                                                    {event.createdByUserId === props.currentUser.id ? eventOwnerActionDropdownList(event.id) : attendeeActionDropdownList(event.id)}
                                                </Dropdown.Menu>
                                            </Dropdown>
                                        </td>
                                    </tr>
                                )
                            } else {
                                return (<></>)
                            }
                        }
                        )}
                </Table>
            </div >
        )
    }

    return (
        <>
            <Container fluid className='bg-grass'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className="font-weight-bold">Dashboard</h1>
                        <p className="font-weight-bold">See how much you've done!</p>
                    </Col>
                    <Col md={5}>
                        <Image src={globes} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>
            <Container className="mt-5 pb-5" >
                <Row className="pt-5">
                    <Col>
                        <div className="d-flex bg-white">
                            <Col className="ml-3">
                                <p className="card-title">Events</p>
                                <p className="card-statistic color-primary mt-0">{totalEvents}</p>
                            </Col>
                            <Col className="d-flex justify-content-end">
                                <Image src={twofigure} alt="person silouhette icons" className="card-icon align-self-end mr-3"></Image>
                            </Col>
                        </div>
                    </Col>
                    <Col>
                        <div className="d-flex bg-white">
                            <Col className="ml-3">
                                <p className="card-title">Hours</p>
                                <p className="card-statistic color-primary mt-0">{totalHours}</p>
                            </Col>
                            <Col className="d-flex justify-content-end">
                                <Image src={calendarclock} alt="calendar clock icons" className="card-icon align-self-end mr-3"></Image>
                            </Col>
                        </div>
                    </Col>
                    <Col>
                        <div className="d-flex bg-white">
                            <Col className="ml-3">
                                <p className="card-title">Bags</p>
                                <p className="card-statistic color-primary mt-0">{totalBags}</p>
                            </Col>
                            <Col className="d-flex justify-content-end">
                                <Image src={bucketplus} alt="add bucket icons" className="card-icon align-self-end mr-3"></Image>
                            </Col>
                        </div>
                    </Col>
                </Row>
            </Container>
            <Container>
                <div className="d-flex my-5 mb-4 justify-content-between">
                    <h4 className="font-weight-bold mr-2 mt-0 text-decoration-underline">My Events ({myEventList.length})</h4>
                    <Link className="btn btn-primary banner-button" to="/manageeventdashboard">Create Event</Link>
                </div>
                <div className="mb-4 bg-white">
                    <>
                        <div className="d-flex justify-content-between px-4">
                            <p className="color-primary font-weight-bold pt-3">{'Upcoming events'} ({myEventList.filter(event => new Date(event.eventDate) >= new Date()).length})</p>
                            <div className="d-flex align-items-center mt-4">
                                <label className="mr-2">
                                    <input type="radio" className="mb-0 radio" name="Event view" value="list" onChange={e => handleEventView(e.target.value, 'Upcoming events')} checked={!upcomingEventsMapView}></input>
                                    <span className="px-2">List view</span>
                                </label>
                                <label className="pr-3">
                                    <input type="radio" className="mb-0 radio" name="Event view" value="map" onChange={e => handleEventView(e.target.value, 'Upcoming events')} checked={upcomingEventsMapView}></input>
                                    <span className="px-2">Map view</span>
                                </label>
                            </div>
                        </div>
                    </>
                    {upcomingEventsMapView ?
                        <>
                            <AzureMapsProvider>
                                <MapControllerPointCollection center={center} multipleEvents={myEventList} isEventDataLoaded={isEventDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} onAttendanceChanged={handleAttendanceChanged} myAttendanceList={myEventList} isUserEventDataLoaded={isEventDataLoaded} onDetailsSelected={handleDetailsSelected} history={props.history} location={props.location} match={props.match} />
                            </AzureMapsProvider>
                        </>
                        : <UpcomingEventsTable />}
                </div>
                <div className="mb-4 bg-white">
                    <div className="d-flex justify-content-between px-4">
                        <p className="color-primary font-weight-bold pt-3">{'Past events'} ({myEventList.filter(event => new Date(event.eventDate) < new Date()).length})</p>
                        <div className="d-flex align-items-center mt-4">
                            <label className="mr-2">
                                <input type="radio" className="mb-0 radio" name="Past event view" value="list" onChange={e => handleEventView(e.target.value, 'Past events')} checked={!pastEventsMapView}></input>
                                <span className="px-2">List view</span>
                            </label>
                            <label className="pr-3">
                                <input type="radio" className="mb-0 radio" name="Past event view" value="map" onChange={e => handleEventView(e.target.value, 'Past events')} checked={pastEventsMapView}></input>
                                <span className="px-2">Map view</span>
                            </label>
                        </div>
                    </div>
                    {pastEventsMapView ?
                        <AzureMapsProvider>
                            <MapControllerPointCollection center={center} multipleEvents={myEventList} isEventDataLoaded={isEventDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} onAttendanceChanged={handleAttendanceChanged} myAttendanceList={myEventList} isUserEventDataLoaded={isEventDataLoaded} onDetailsSelected={handleDetailsSelected} history={props.history} location={props.location} match={props.match} />
                        </AzureMapsProvider>
                        : <PastEventsTable />}
                </div>
            </Container>
        </>
    );
}

export default withRouter(MyDashboard);