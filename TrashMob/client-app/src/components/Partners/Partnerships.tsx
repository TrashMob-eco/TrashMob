import { FC } from 'react'
import { Col, Container, Image, Row } from 'react-bootstrap';
import globes from '../assets/gettingStarted/globes.png';
import { Link } from 'react-router-dom';

export const Partnerships: FC = () => {
    return (
        <>
            <Container fluid className='bg-grass mb-5'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className="font-weight-bold">Partnerships</h1>
                        <p className="font-weight-bold">Connecting you to nearby services.</p>
                    </Col>
                    <Col md={5}>
                        <Image src={globes} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>
            <Container className="py-5">
                <h2 className='font-weight-bold'>What are partnerships?</h2>

                <p>Partnering with local cities and businesses can connect TrashMob event attendees and creators with the supplies and services they need.</p>

                <p>Partners can include cities, local businesses, and branches/locations of larger companies. Services can include trash hauling and disposal locations, and supplies can include buckets, grabber tools, and safety equipment.</p>

                <p>Looking for supplies and services for your next event? Request a partnership from your city! Have supplies and services to offer? Become a partner!</p>

                <h2 className='font-weight-bold'>Benefits of partnerships?</h2>

                <p>Services and supplies offered can include</p>
                
                <ul>
                    <li>Trash Hauling</li>
                    <li>Trash Disposal</li>
                    <li>Supplies</li>
                    <li>Starter Kits</li>
                </ul>

                <h2 className='font-weight-bold'>Making the most of partnerships</h2>

                <p>After setting a location for an event, recommended partners and their services will be revealed depending on the event’s radius and range of the partner’s supplies.</p>

                <p>After selecting one or more of the services, it will go to the partner for confirmation. Once approved, event attendees will see the services and supplies list when they register for the event. Following the instructions given by the partner, both event creators and attendees can access the supplied services and resources.</p>

                <p>If there is no partner listed, request a partnership for your area and the TrashMob team will reach out to onboard the partner if they’re interested.</p>

                <p>Have supplies and services to offer? Become a partner yourself!</p>
            </Container>
            <Container fluid className="bg-white">
                <Row className="text-center pt-5">
                    <Col md>
                        <div className="d-flex flex-column">
                            <div className="px-5 mb-5">
                                <Link className="mt-2 btn btn-primary" to="/requestapartner" role="button">Request a partnership</Link>
                            </div>
                            <div className="px-5 mb-5">
                                <Link className="mt-2 btn btn-primary" to="/becomeapartner" role="button">Become a partner</Link>
                            </div>
                        </div>
                    </Col>
                </Row>
            </Container>
        </>
    );
}
