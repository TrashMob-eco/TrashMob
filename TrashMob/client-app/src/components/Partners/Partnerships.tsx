import { FC } from 'react'
import { Col, Container, Image, Row } from 'react-bootstrap';
import globes from '../assets/gettingStarted/globes.png';
import heroImg from '../assets/partnerships/whatIsPartnerships.svg';
import Safetykits from '../assets/partnerships/Safetykits.svg';
import Supplies from '../assets/partnerships/Supplies.svg';
import TrashDisposal from '../assets/partnerships/TrashDisposal.svg';
import Garbage from '../assets/partnerships/garbage.svg';
import { Link } from 'react-router-dom';

export const Partnerships: FC<any> = () => {
    
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
            <Container className='py-5'>
                <Row>
                    <Col sm={7}>
                        <h1 className='fw-600'>What are partnerships?</h1>
                        <h4>Partnering with local cities and businesses can connect TrashMob event attendees and creators with the supplies and services they need.</h4>
                        <p className='para'>Partners can include cities, local businesses, and branches/locations of larger companies. Services can include trash hauling and disposal locations, and supplies can include buckets, grabber tools, and safety equipment.
                            Looking for supplies and services for your next event? Invite a partnership from your city! Have supplies and services to offer? Become a partner!</p>
                        <Link className="btn btn-primary banner-button" to="/requestapartner">Send invitation to join TrashMob.eco as a partner</Link>
                        <Link className="btn btn-primary banner-button" to="/becomeapartner">Apply to become a partner</Link>
                    </Col>
                    <Col sm={5}>
                        <Image src={heroImg} alt="globes" className="mt-0 h-100" />
                    </Col>
                </Row>
            </Container>

            <Container fluid className='bg-white text-center py-5'>
                <h1 className='fw-600'>Benefits of partnerships</h1>
                <h4>Services and supplies offered can include:</h4>
                <Row className='w-50 mt-5 mx-auto'>
                    <Col>
                        <Image src={Safetykits} alt="Safetykits" className="mt-0" />
                        <h4>Safety kits</h4>
                    </Col>
                    <Col>
                        <Image src={Supplies} alt="Supplies" className="mt-0" />
                        <h4>Supplies</h4>
                    </Col>
                    <Col>
                        <Image src={TrashDisposal} alt="TrashDisposal" className="mt-0" />
                        <h4>Trash disposal</h4>
                    </Col>
                </Row>
            </Container>

            <Container className='py-5'>
                <Row>
                    <Col sm={7}>
                        <h1 className='fw-600'>Making the most out of partnerships</h1>
                        <h4>After setting a location for an event, recommended partners and their services will be revealed depending on the event’s radius and range of the partner’s supplies.</h4>
                        <p className='para'>After selecting one or more of the services, it will go to the partner for confirmation. Once approved, event attendees will see the services and supplies list when they register for the event. Following the instructions given by the partner, both event creators and attendees can access the supplied services and resources. Note that supplied services from a given partner may vary by location/branch.
                            If there is no partner listed, request a partnership for your area and the TrashMob team will reach out to onboard the partner if they’re interested.
                            Have supplies and services to offer? Become a partner yourself!</p>
                        <Link className="btn btn-primary banner-button" to="/requestapartner">Send invitation to join TrashMob.eco as a partner</Link>
                        <Link className="btn btn-primary banner-button" to="/becomeapartner">Apply to become a partner</Link>
                    </Col>
                    <Col sm={5}>
                        <Image src={Garbage} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>

            <div className='w-100 bg-white'>
                <Container className='py-5'>
                    <div className='d-flex justify-content-between align-items-center'>
                        <Link className="btn btn-primary banner-button" to="/requestapartner">Send invitation to join TrashMob.eco as a partner</Link>
                        <Link className="btn btn-primary banner-button" to="/becomeapartner">Apply to become a partner</Link>
                    </div>
                </Container>
            </div>
        </>
    );
}
