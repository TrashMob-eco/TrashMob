import React, { FC } from 'react';
import { Col, Container, Image, Row } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import Ihca from '../assets/partnerships/logos/IHCA.png';
import TroutUnlimited from '../assets/partnerships/logos/TROUTUNLIMITEDLogo.png';
import Safetykits from '../assets/partnerships/Safetykits.svg';
import Supplies from '../assets/partnerships/Supplies.svg';
import TrashDisposal from '../assets/partnerships/TrashDisposal.svg';
import Dollarsign from '../assets/partnerships/dollarsign.svg';
import Garbage from '../assets/partnerships/garbage.png';

import { HeroSection } from '../Customization/HeroSection';

export const Partnerships: FC<any> = () => {
    React.useEffect(() => {
        window.scrollTo(0, 0);
    });

    return (
        <>
            <HeroSection Title='Partnerships' Description='Connecting you with volunteers.' />
            <Container className='py-5'>
                <Row>
                    <Col sm={7}>
                        <h1 className='fw-600'>What are partnerships?</h1>
                        <h4>
                            Partnering with local cities and businesses can connect TrashMob event volunteers with the
                            supplies and services they need.
                        </h4>
                        <p>
                            Partners can include cities, local businesses, and branches/locations of larger companies.
                            Services can include trash hauling and disposal locations, and supplies can include buckets,
                            grabber tools, and safety equipment. Looking for supplies and services for your next event?
                            Invite a partnership from your city! Have supplies and services to offer? Become a partner!
                        </p>
                    </Col>
                    <Col sm={5}>
                        <Row className='pl-3'>
                            <h1 className='fw-600'>Our Partners</h1>
                        </Row>
                        <Row className='pl-3'>
                            <a href='https://issaquahhighlands.com/' target='_blank' rel='noreferrer' className='m-2'>
                                <Image
                                    src={Ihca}
                                    alt='Issaquah Highlands Community Association'
                                    className='graphic-large mx-auto'
                                />
                            </a>
                            <a href='https://troutunlimited.org' target='_blank' rel='noreferrer' className='m-2'>
                                <Image src={TroutUnlimited} alt='Trout Unlimited' className='graphic-large mx-auto' />
                            </a>
                        </Row>
                    </Col>
                </Row>
            </Container>
            <div className='w-100 bg-white'>
                <Container className='py-5 px-0'>
                    <Row className='max-width-container mx-auto align-items-center'>
                        <Col sm={6}>
                            <div className='d-flex flex-column align-items-start'>
                                <p className='font-size-h4'>
                                    No partner for your event? Invite local government or business to join TrashMob.eco
                                    as a partner!
                                </p>
                                <Link className='btn btn-primary banner-button' to='/inviteapartner'>
                                    Invite a partner
                                </Link>
                            </div>
                        </Col>
                        <Col sm={6}>
                            <div className='d-flex flex-column align-items-start'>
                                <p className='font-size-h4'>
                                    Have supplies and services to offer? Submit an application to become a TrashMob.eco
                                    partner!
                                </p>
                                <Link className='btn btn-primary banner-button' to='/becomeapartner'>
                                    Become a partner
                                </Link>
                            </div>
                        </Col>
                    </Row>
                </Container>
            </div>
            <Container fluid className='text-center py-5 px-0'>
                <h2 className='font-weight-bold mb-3'>Partnerships support the volunteers</h2>
                <span>Services and supplies offered can include:</span>
                <Row className='justify-content-center'>
                    <Row xs={1} sm={2} md={4} className='w-50 mt-5 mx-auto justify-content-around    '>
                        <Col className='d-flex flex-column align-items-center mb-5 mb-md-0'>
                            <Image src={Safetykits} alt='Safety kits' className='graphic-large mt-0' />
                            <span className='font-weight-bold mt-2'>Safety gear and roadside signs</span>
                        </Col>
                        <Col className='d-flex flex-column align-items-center mb-5 mb-md-0'>
                            <Image src={Supplies} alt='Supplies' className='graphic-large mt-0' />
                            <span className='font-weight-bold mt-2'>Pickup supplies such as garbage bags</span>
                        </Col>
                        <Col className='d-flex flex-column align-items-center mb-5 mb-md-0'>
                            <Image src={TrashDisposal} alt='Trash Disposal & Hauling' className='graphic-large mt-0' />
                            <span className='font-weight-bold mt-2'>
                                Use of existing dumpsters and hauling of trash to disposal site
                            </span>
                        </Col>
                        <Col className='d-flex flex-column align-items-center mb-5 mb-md-0'>
                            <Image src={Dollarsign} alt='Dollar sign' className='graphic-large mt-0' />
                            <span className='font-weight-bold mt-2'>
                                <a href='https://www.trashmob.eco/donate'>Donations</a> to TrashMob.eco fund development
                                of our platform and programs
                            </span>
                        </Col>
                    </Row>
                </Row>
            </Container>

            <Container fluid className='bg-white py-5'>
                <Row className='container mx-auto px-0'>
                    <Col sm={7}>
                        <h1 className='fw-600'>Making the most out of partnerships</h1>
                        <p className='para'>
                            A successful clean up event depends upon a team of volunteers and the support of partners:
                            community businesses, organizations and governments. Volunteer organizers set an event
                            location, rally member support and utilize partnership provisions.
                        </p>
                        <p className='para'>
                            TrashMob administrators confirm, approve and connect partners with event organizers.
                            Partners and their form of support are indicated on event registration pages. Then local
                            teamwork commences! Event organizers and partners coordinate access of supplies, services
                            and instructions. Partners are selected upon availability and proximity to the event. Note:
                            Supplied services from a given partner may vary by location/branch.{' '}
                        </p>
                    </Col>
                    <Col sm={5} className='d-flex'>
                        <Image src={Garbage} alt='garbage bags being picked up' className='mh-100 mt-0 m-auto' />
                    </Col>
                </Row>
            </Container>
        </>
    );
};
