import * as React from 'react';
import attitude from '../assets/gettingStarted/attitude.png';
import bucket from '../assets/gettingStarted/bucket.png';
import highways from '../assets/gettingStarted/highways.png';
import picker from '../assets/gettingStarted/picker.png';
import trashcangroup from '../assets/gettingStarted/trashcangroup.png';
import wear from '../assets/gettingStarted/wear.png';
import workgloves from '../assets/gettingStarted/workgloves.png';
import globes from '../assets/gettingStarted/globes.png';
import { Link } from 'react-router-dom';
import { Col, Container, Row, Image } from 'react-bootstrap';

export const GettingStarted: React.FC = () => {
    return (
        <>
            <Container fluid className='bg-grass'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className='font-weight-bold'>Getting Started</h1>
                        <p className="font-weight-bold">
                            Tips and tricks to get you out there.
                        </p>
                    </Col>
                    <Col md={5}>
                        <Image src={globes} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>
            <Container fluid className="bg-white">
                <Row className="text-center pt-5 ">
                    <Col md>
                        <h2 className='font-weight-bold'>The Basics</h2>
                    </Col>
                </Row>
                <Row className="text-center mt-3">
                    <Col md={2}></Col>
                    <Col md={2}>
                        <div className="d-flex flex-column">
                            <img src={workgloves} className="graphic-large mx-auto" alt="Work gloves"></img>
                            <h6 className="font-weight-bold mt-2">Work gloves</h6>
                            <span className='mt-4'>We recommend <a href='https://www.homedepot.com/b/Workwear-Work-Gloves/Latex/N-5yc1vZc260Z1z0z9o0'>Rubber Latex Double coated work gloves</a>. These will protect you from anything sharp, wet, or icky. </span>
                        </div>
                    </Col>
                    <Col md={2}>
                        <div className="d-flex flex-column">
                            <img src={bucket} className="graphic-large mx-auto" alt="bucket"></img>
                            <h6 className="font-weight-bold mt-2">A bucket</h6>
                            <span className='mt-4'>Any 5 gallon pail will do. If you don’t want to buy one, many restaurants and construction industries give out up-cycled ones. We don’t recommend plastic bags since they tend to tear.</span>
                        </div>
                    </Col>
                    <Col md={2}>
                        <div className="d-flex flex-column">
                            <img src={picker} className="graphic-large mx-auto" alt="picker"></img>
                            <h6 className="font-weight-bold mt-2">A grabber tool</h6>
                            <span className='mt-4'>While not essential, we recommend a grabber tool because they help make grabbing trash easier on our bodies. We like ones with a pistol grip, like the <a href='https://ungerconsumer.com/product/grabber-plus/'>
                                Unger Grabber Plus Reacher</a>.</span>
                        </div>
                    </Col>
                    <Col md={2}>
                        <div className="d-flex flex-column">
                            <img src={attitude} className="graphic-large mx-auto" alt="attitude"></img>
                            <h6 className="font-weight-bold mt-2">A good attitude</h6>
                            <span className='mt-4'>Your attitude is just as important as your tools. A positive attitude increases the chances that other people will join your group, and improving our communities works best with others.</span>
                        </div>
                    </Col>
                    <Col md={2}></Col>

                </Row>
                <Row className='mt-4'>
                    <Col md>
                        <div className="w-100 d-flex align-items-center" style={{ backgroundImage: `url(${trashcangroup})`, backgroundRepeat: "no-repeat", backgroundSize: "cover", backgroundPosition: "center" }} >
                            <div className="text-white bg-black w-50 border-rounded-lg my-5 mx-auto" style={{ opacity: 0.95 }}>
                                <div className="p-5">
                                    <h4>TrashMob tips</h4>
                                    <ol className="list-unstyled mt-5">
                                        <li className="mb-4">
                                            <div className="d-flex align-top">
                                                <span className="mr-3 font-weight-bold font-size-lg">1</span>
                                                <span>Stay local. It will save you time and energy. You don’t have to travel to the dirtiest highway to make a difference!</span>
                                            </div>
                                        </li>
                                        <li className="mb-4">
                                            <div className="d-flex align-top">
                                                <span className="mr-3 font-weight-bold font-size-lg">2</span>
                                                <span>Start with a park-based event. With few cars, nearby garbage cans, and high community exposure, this is a great way to ease in.</span>
                                            </div>
                                        </li>
                                        <li className="mb-4">
                                            <div className="d-flex align-top">
                                                <span className="mr-3 font-weight-bold font-size-lg">3</span>
                                                <span>Recruit one friend or family member. Having someone join you can build a greater sense of accomplishment.</span>
                                            </div>
                                        </li>
                                        <li className="mb-4">
                                            <div className="d-flex align-top">
                                                <span className="mr-3 font-weight-bold font-size-lg">4</span>
                                                <span>Set a goal. For example, aim for 2 buckets per person. Start small, and recognize an area won’t be litter-free in 30 minutes.</span>
                                            </div>
                                        </li>
                                        <li className="mb-4">
                                            <div className="d-flex align-top">
                                                <span className="mr-3 font-weight-bold font-size-lg">5</span>
                                                <span>Be safe. No piece of litter is worth risking your health or well-being.</span>
                                            </div>
                                        </li>
                                    </ol>
                                </div>
                            </div>
                        </div>
                    </Col>
                </Row>
                <Row className='mt-4'>
                    <Col md={2}></Col>
                    <Col md={4}>
                        <div className="px-5 mb-5">
                            <h2>What do I wear?</h2>
                            <p className="font-weight-bold">
                                Wear clothes you won’t mind getting dirty or possibly torn. We strongly encourage a reflective vest for clean ups along roadsides.
                            </p>
                            <p className="font-weight-light">
                                Long sleeves and jeans that keep branches and thorns off are best. Most importantly, wear good footwear that are preferably water resistant and thick.
                                Hiking boots work well, as do old tennis shoes. Shorts and t-shirts are sometimes enough for some urban pickups, but remember to dress for safety and for the weather.
                            </p>
                        </div>
                    </Col>
                    <Col md={4}>
                        <div className="px-5 pt-5">
                            <Image src={wear} alt="wear" className="h-75" />
                        </div>
                    </Col>
                    <Col md={2}></Col>
                </Row>
            </Container>
            <Container fluid>
                <Row className='mt-4'>
                    <Col md={2}></Col>
                    <Col md={4}>
                        <div className="px-5">
                            <h2>But what about the highways?</h2>
                            <p className="font-weight-bold">
                                Please check with your Department of Transportation before creating a highway cleanup event, and follow all of their guidance. Safety is the number one priority!
                            </p>
                            <p className="font-weight-light">
                                In America, the highways are notorious for litter. People tend to throw trash out of their windows in areas they don’t live near, and unsecured loads will fly
                                off trucks at highway speeds. With vehicles racing by at 75mph, they are also the most dangerous places to pick up trash. Because of this danger,
                                most states have formed Adopt-a-Highway programs which provide guidance, training, and safety equipment for those who want to work these tough environments.
                            </p>
                            <p className="font-weight-light">
                                And never, ever, pick on or beside railways.
                            </p>
                        </div>
                    </Col>
                    <Col md={4}>
                        <div className="px-5 pt-5">
                            <Image src={highways} alt="highways" className="h-75" />
                        </div>
                    </Col>
                    <Col md={2}></Col>
                </Row>
            </Container>
            <Container fluid className="bg-white">
                <Row className="text-center pt-5">
                    <Col md>
                        <div className="d-flex flex-column">
                            <h2 className='font-weight-bold'>Ready to go?</h2>
                            <span>Find your first event now.</span>
                            <div className="px-5 mb-5">
                                <Link className="mt-2 btn btn-primary" to="/" role="button">Find events</Link>
                            </div>
                        </div>
                    </Col>
                </Row>
            </Container>
        </>        
    );
}

