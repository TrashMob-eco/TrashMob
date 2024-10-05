import * as React from 'react';
import { Link } from 'react-router-dom';
import { Col, Container, Row, Image } from 'react-bootstrap';
import attitude from '../assets/gettingStarted/attitude.png';
import bucket from '../assets/gettingStarted/bucket.png';
import highways from '../assets/gettingStarted/highways.png';
import picker from '../assets/gettingStarted/picker.png';
import trashcangroup from '../assets/gettingStarted/trashcangroup.png';
import workgloves from '../assets/gettingStarted/workgloves.png';
import { HeroSection } from '../Customization/HeroSection';

export const GettingStarted: React.FC = () => {
    React.useEffect(() => {
        window.scrollTo(0, 0);
    });

    return (
        <>
            <HeroSection Title='Getting Started' Description='Tips and tricks to get you out there.' />
            <Container fluid className='bg-white'>
                <Row className='text-center py-5 '>
                    <Col md>
                        <h2 className='font-weight-bold'>The Basics</h2>
                    </Col>
                </Row>
                <Row className='w-100 text-center d-flex justify-content-center m-auto'>
                    <Col md={3} className='d-flex justify-content-center'>
                        <div className='d-flex flex-column w-100 mb-5' style={{ maxWidth: '300px' }}>
                            <img src={workgloves} className='graphic-large mx-auto' alt='Work gloves' />
                            <h6 className='font-weight-bold mt-2'>Work gloves</h6>
                            <span className='mt-4'>
                                We recommend{' '}
                                <a href='https://www.homedepot.com/b/Workwear-Work-Gloves/Latex/N-5yc1vZc260Z1z0z9o0'>
                                    Rubber Latex Double coated work gloves
                                </a>
                                . These will protect you from anything sharp, wet, or icky.{' '}
                            </span>
                        </div>
                    </Col>
                    <Col md={3} className='d-flex justify-content-center'>
                        <div className='d-flex flex-column w-100 mb-5' style={{ maxWidth: '300px' }}>
                            <img src={bucket} className='graphic-large mx-auto' alt='bucket' />
                            <h6 className='font-weight-bold mt-2'>A bucket</h6>
                            <span className='mt-4'>
                                Any 5 gallon pail will do. If you don’t want to buy one, many restaurants and
                                construction industries give out up-cycled ones. We don’t recommend plastic bags since
                                they tend to tear.
                            </span>
                        </div>
                    </Col>
                    <Col md={3} className='d-flex justify-content-center'>
                        <div className='d-flex flex-column w-100 mb-5' style={{ maxWidth: '300px' }}>
                            <img src={picker} className='graphic-large mx-auto' alt='picker' />
                            <h6 className='font-weight-bold mt-2'>A grabber tool</h6>
                            <span className='mt-4'>
                                While not essential, we recommend a grabber tool because they help make grabbing trash
                                easier on our bodies. We like ones with a pistol grip, like the{' '}
                                <a href='https://ungerconsumer.com/product/grabber-plus/'>Unger Grabber Plus Reacher</a>
                                .
                            </span>
                        </div>
                    </Col>
                    <Col md={3} className='d-flex justify-content-center'>
                        <div className='d-flex flex-column w-100 mb-5' style={{ maxWidth: '300px' }}>
                            <img src={attitude} className='graphic-large mx-auto' alt='attitude' />
                            <h6 className='font-weight-bold mt-2'>A good attitude</h6>
                            <span className='mt-4'>
                                Your attitude is just as important as your tools. A positive attitude increases the
                                chances that other people will join your group, and improving our communities works best
                                with others.
                            </span>
                        </div>
                    </Col>
                </Row>
                <Row className='mt-4'>
                    <Col md>
                        <div
                            className='w-100 d-flex align-items-center'
                            style={{
                                backgroundImage: `url(${trashcangroup})`,
                                backgroundRepeat: 'no-repeat',
                                backgroundSize: 'cover',
                                backgroundPosition: 'center',
                            }}
                        >
                            <div
                                className='container text-white bg-black border-rounded-lg my-3 my-sm-5 mx-4 mx-sm-auto px-4'
                                style={{ opacity: 0.95 }}
                            >
                                <div className='p-md-5'>
                                    <h4>TrashMob tips</h4>
                                    <ol className='list-unstyled mt-5'>
                                        <li className='mb-4'>
                                            <div className='d-flex align-top'>
                                                <span className='mr-3 font-weight-bold font-size-lg'>1</span>
                                                <span>
                                                    Stay local. It will save you time and energy. You don’t have to
                                                    travel to the dirtiest highway to make a difference!
                                                </span>
                                            </div>
                                        </li>
                                        <li className='mb-4'>
                                            <div className='d-flex align-top'>
                                                <span className='mr-3 font-weight-bold font-size-lg'>2</span>
                                                <span>
                                                    Start with a park-based event. With few cars, nearby garbage cans,
                                                    and high community exposure, this is a great way to ease in.
                                                </span>
                                            </div>
                                        </li>
                                        <li className='mb-4'>
                                            <div className='d-flex align-top'>
                                                <span className='mr-3 font-weight-bold font-size-lg'>3</span>
                                                <span>
                                                    Recruit one friend or family member. Having someone join you can
                                                    build a greater sense of accomplishment.
                                                </span>
                                            </div>
                                        </li>
                                        <li className='mb-4'>
                                            <div className='d-flex align-top'>
                                                <span className='mr-3 font-weight-bold font-size-lg'>4</span>
                                                <span>
                                                    Set a goal. For example, aim for 2 buckets per person. Start small,
                                                    and recognize an area won’t be litter-free in 30 minutes.
                                                </span>
                                            </div>
                                        </li>
                                        <li className='mb-4'>
                                            <div className='d-flex align-top'>
                                                <span className='mr-3 font-weight-bold font-size-lg'>5</span>
                                                <span>
                                                    Be safe. No piece of litter is worth risking your health or
                                                    well-being.
                                                </span>
                                            </div>
                                        </li>
                                    </ol>
                                </div>
                            </div>
                        </div>
                    </Col>
                </Row>
            </Container>
            <div className='bg-white'>
                <Container className='py-5'>
                    <Row className='py-5'>
                        <Col md={6}>
                            <div className=''>
                                <h1 className='fw-600'>Safety is Essential!</h1>
                                <h4 className='mt-5'>
                                    All TrashMob.eco event leads and attendees are required to watch our safety video.
                                    Please take a few minutes to review it now!
                                </h4>
                            </div>
                        </Col>
                        <Col md={6} className='align-content-center mt-4'>
                            <div className='embed-responsive embed-responsive-16by9'>
                                <iframe
                                    className='embed-responsive-item'
                                    width='560'
                                    height='315'
                                    src='https://www.youtube.com/embed/naMY0kfyERc'
                                    title='YouTube video player'
                                    allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share'
                                    allowFullScreen
                                />
                            </div>
                        </Col>
                    </Row>
                </Container>
            </div>
            <Container className='py-5'>
                <Row className='py-5 align-items-center'>
                    <Col md={6}>
                        <div>
                            <h1 className='fw-600'>But what about the highways?</h1>
                            <h4 className='mt-5'>
                                Please check with your Department of Transportation before creating a highway cleanup
                                event, and follow all of their guidance. Safety is the number one priority!
                            </h4>
                            <p className='font-weight-light'>
                                In America, the highways are notorious for litter. People tend to throw trash out of
                                their windows in areas they don’t live near, and unsecured loads will fly off trucks at
                                highway speeds. With vehicles racing by at 75mph, they are also the most dangerous
                                places to pick up trash. Because of this danger, most states have formed Adopt-a-Highway
                                programs which provide guidance, training, and safety equipment for those who want to
                                work these tough environments.
                            </p>
                            <p className='font-weight-light'>And never, ever, pick on or beside railways.</p>
                        </div>
                    </Col>
                    <Col md={6}>
                        <Image src={highways} alt='highway overpasses' className='img-fluid' />
                    </Col>
                </Row>
            </Container>
            <Container fluid className='bg-white'>
                <Row className='text-center py-5'>
                    <Col md>
                        <div className='d-flex flex-column'>
                            <h2 className='font-weight-bold'>Ready to go?</h2>
                            <span>Find your first event now.</span>
                            <div className='px-5 mb-5'>
                                <Link className='mt-2 btn btn-primary' to='/' role='button'>
                                    Find events
                                </Link>
                            </div>
                        </div>
                    </Col>
                </Row>
            </Container>
        </>
    );
};
