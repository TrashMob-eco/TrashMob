import { FC } from 'react'
import { Col, Container, Image, Row } from 'react-bootstrap';
import globes from '../assets/gettingStarted/globes.png';
import heroImg from '../assets/partnerships/whatIsPartnerships.svg';
import Safetykits from '../assets/partnerships/Safetykits.svg';
import Supplies from '../assets/partnerships/Supplies.svg';
import TrashDisposal from '../assets/partnerships/TrashDisposal.svg';
import Dollarsign from '../assets/partnerships/TrashDisposal.svg';
import Garbage from '../assets/partnerships/garbage.svg';
import { Link } from 'react-router-dom';

export const Partnerships: FC<any> = () => {

    return (
        <>
            <Container fluid className='bg-grass mb-5'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className="font-weight-bold">Partnerships</h1>
                        <p className="font-weight-bold">Connecting you with volunteers.</p>
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
                        <h4>Partnering with local cities and businesses can connect TrashMob event volunteers with the supplies and services they need.</h4>
                        <p className='para'>Partners can include cities, local businesses, and branches/locations of larger companies. Services can include trash hauling and disposal locations, and supplies can include buckets, grabber tools, and safety equipment.
                            Looking for supplies and services for your next event? Invite a partnership from your city! Have supplies and services to offer? Become a partner!</p>
                    </Col>
                    <Col sm={5}>
                        <Image src={heroImg} alt="garbage being loaded into garbage truck" className="mt-0 h-100" />
                    </Col>
                </Row>
            </Container>
            <div className='w-100 bg-white'>
                <Container className='py-5'>
                    <Row>
                        <Col sm={6}>
                            <div className='align-items-center'>
                                <h4>No partner for your event? Invite local government or business to join TrashMob.eco as a partner!</h4>
                            </div>
                            <div className='align-items-center'>
                                <Link className="btn btn-primary banner-button" to="/requestapartner">Invite a partner</Link>
                            </div>
                        </Col>
                        <Col sm={6}>
                            <div className='align-items-center'>
                                <h4>
                                    Have supplies and services to offer? Submit an application to become a TrashMob.eco partner!
                                </h4>
                            </div>
                            <div className='align-items-center'>
                                <p>
                                    <Link className="btn btn-primary banner-button" to="/becomeapartner">Become a partner</Link>
                                </p>
                            </div>
                        </Col>
                    </Row>
                </Container>
            </div>
            <Container fluid className='text-center py-5'>
                <h1 className='fw-600'>Partnerships support the volunteers</h1>
                <h4>Services and supplies offered can include:</h4>
                <Row className='w-50 mt-5 mx-auto'>
                    <Col>
                        <Image src={Safetykits} alt="Safety kits" className="mt-0" />
                        <h4>Safety gear and roadside signs</h4>
                    </Col>
                    <Col>
                        <Image src={Supplies} alt="Supplies" className="mt-0" />
                        <h4>Pickup supplies such as garbage bags</h4>
                    </Col>
                    <Col>
                        <Image src={TrashDisposal} alt="Trash Disposal & Hauling" className="mt-0" />
                        <h4>Use of existing dumpsters and hauling of trash to disposal site</h4>
                    </Col>
                    <Col>
                        <Image src={Dollarsign} alt="Dollar sign" className="mt-0" />
                        <h4><a href="https://www.trashmob.eco/donate">Donations</a> to TrashMob.eco fund development of our platform and programs</h4>
                    </Col>
                </Row>
            </Container>

            <Container className='bg-white py-5'>
                <Row>
                    <Col sm={7}>
                        <h1 className='fw-600'>Making the most out of partnerships</h1>
                        <p className='para'>A successful clean up event depends upon a team of volunteers and the support of partners: community businesses, organizations and governments.  Volunteer organizers set an event location, rally member support and utilize partnership provisions.</p>
                        <p className='para'>TrashMob administrators confirm, approve and connect partners with event organizers. Partners and their form of support are indicated on event registration pages. Then local teamwork commences!  Event organizers and partners coordinate access of supplies, services and instructions. Partners are selected upon availability and proximity to the event. Note: Supplied services from a given partner may vary by location/branch. </p>
                    </Col>
                    <Col sm={5}>
                        <Image src={Garbage} alt="garbage bags being picked up" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>
        </>
    );
}
