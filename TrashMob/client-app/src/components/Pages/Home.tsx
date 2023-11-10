import { FC, useEffect, useState } from 'react'
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { getDefaultHeaders } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Col, Container, Image, Row } from 'react-bootstrap';
import Drawings from '../assets/home/Drawings.png';
import Globe2 from '../assets/globe2.png';
import Logo from '../assets/logo.svg';
import Calendar from '../assets/home/Calendar.svg';
import Trashbag from '../assets/home/Trashbag.svg';
import Person from '../assets/home/Person.svg';
import Clock from '../assets/home/Clock.svg';
import { GettingStartedSection } from '../GettingStartedSection';
import StatsData from '../Models/StatsData';
import { Share } from 'react-bootstrap-icons';
import { SocialsModal } from '../EventManagement/ShareToSocialsModal';
import * as SharingMessages from '../../store/SharingMessages';
import {EventsSection} from '../EventsSection';
import { Button } from 'reactstrap';

export interface HomeProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
    onUserUpdated: any;
}

const Home: FC<HomeProps> = ({ isUserLoaded, currentUser, history, location, match }) => {
    const [totalBags, setTotalBags] = useState<number>(0);
    const [totalHours, setTotalHours] = useState<number>(0);
    const [totalEvents, setTotalEvents] = useState<number>(0);
    const [totalParticipants, setTotalParticipants] = useState<number>(0);
    const [showModal, setShowSocialsModal] = useState<boolean>(false);

    useEffect(() => {

        window.scrollTo(0, 0);

        const headers = getDefaultHeaders('GET');

        fetch('/api/stats', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<StatsData>)
            .then(data => {
                setTotalBags(data.totalBags);
                setTotalHours(data.totalHours);
                setTotalEvents(data.totalEvents);
                setTotalParticipants(data.totalParticipants);
            });
    }, [isUserLoaded, currentUser])

    const handleShowModal = (showModal: boolean) => {
        setShowSocialsModal(showModal)
    }

    return (
        <>
            <Container fluid>
                <SocialsModal show={showModal} handleShow={handleShowModal} modalTitle='Invite a friend to join TrashMob.eco' eventLink='https://www.trashmob.eco' emailSubject='Join TrashMob.eco to help clean up the planet!' message={SharingMessages.InvitationMessage} />
                <Row className="shadow position-relative" >
                    <Col className="d-flex flex-column px-0 py-4 pl-lg-5" sm={6} style={{ zIndex: 1 }}>
                        <div className="ml-sm-2 ml-lg-5 pl-sm-3 pl-md-5 mt-md-5 mb-md-2">
                            <img src={Logo} alt="TrashMob.eco logo" className="banner-logo"></img>
                            <h3 className="ml-md-4 mt-4 mb-4 mb-md-5 font-weight-bold font-size-xl banner-heading pl-3">Meet up. Clean up. Feel good.</h3>
                            <Link className="btn btn-primary ml-5 py-md-3 banner-button" to="/gettingstarted">Join us today</Link>
                            <Button className="btn btn-primary ml-5 py-md-3 banner-button" onClick={() => { handleShowModal(true) }}>
                                <Share className="mr-2" />
                                Invite a friend
                            </Button>
                        </div>
                    </Col>
                    <img src={Globe2} className="position-absolute p-0 m-0 h-100 banner-globe" alt="Globe" ></img>
                </Row>
            </Container>
            <div className='bg-white'>
                <Container className='py-5'>
                    <Row className='py-5'>
                        <Col md={6}>
                            <h1 className="mt-0 font-weight-bold">What is a TrashMob?</h1>
                            <h4 className='my-5'>A TrashMob is a group of citizens who are willing to take an hour or two out of their lives to get together and clean up their communities. Start your impact today.</h4>
                            <Link className="mt-5 btn btn-primary btn-128" to="/aboutus" role="button">Learn more</Link>
                        </Col>
                        <Col md={6}>
                            <iframe width="560" height="315" src="https://www.youtube.com/embed/ylOBeVHRtuM" title="YouTube video player" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" allowFullScreen></iframe>
                        </Col>
                    </Row>
                </Container>
            </div>
            <Container className="d-flex justify-content-around my-5 py-5 flex-column flex-md-row">
                <div className="d-flex flex-column justify-content-center text-center">
                    <img src={Calendar} alt="Calendar icon" className="w-auto mx-auto mb-3" />
                    <span className="font-weight-bold font-size-lg">{totalEvents}</span>
                    <span className="font-weight-bold">Events Hosted</span>
                </div>
                <div className="d-flex flex-column justify-content-center text-center">
                    <img src={Trashbag} alt="Trashbag icon" className="w-auto mx-auto mb-3" />
                    <span className="font-weight-bold font-size-lg">{totalBags}</span>
                    <span className="font-weight-bold">Bags Collected</span>
                </div>
                <div className="d-flex flex-column justify-content-center text-center">
                    <img src={Person} alt="Person icon" className="w-auto mx-auto mb-3" />
                    <span className="font-weight-bold font-size-lg">{totalParticipants}</span>
                    <span className="font-weight-bold">Participants</span>
                </div>
                <div className="d-flex flex-column justify-content-center text-center">
                    <img src={Clock} alt="Clock icon" className="w-auto mx-auto mb-3" />
                    <span className="font-weight-bold font-size-lg">{totalHours}</span>
                    <span className="font-weight-bold">Hours Spent</span>
                </div>
            </Container>
            <EventsSection currentUser={currentUser} isUserLoaded={isUserLoaded} history={history} location={location} match={match}></EventsSection>
            <GettingStartedSection />
        </>
    );
}

export default withRouter(Home);
