import * as React from 'react';
import { Button, Col, Container, Image, Row } from 'react-bootstrap';
import globes from './assets/gettingStarted/globes.png';

const ReadMore = ({ children }: any) => {
    const text = children;
    const [isReadMore, setIsReadMore] = React.useState(true);
    const toggleReadMore = () => {
        setIsReadMore(!isReadMore);
    };
    return (
        <>
            <p className="text">
                {isReadMore ? text.slice(0, 0) : text}
            </p>
            <p onClick={toggleReadMore} className="p-15 color-primary" role="button">
                {isReadMore ? "See more" : "See less"}
            </p>
        </>
    );
};


export const VolunteerOpportunities: React.FC = () => {
    return (
        <>
            <Container fluid className='bg-grass mb-5'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className="font-weight-bold">Recruiting</h1>
                        <p className="font-weight-bold">We’d love to have you join us.</p>
                    </Col>
                    <Col md={5}>
                        <Image src={globes} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>
            <Container>
                <Row>
                    <Col md={8} className="mb-2"><h1 className='m-0'>Open volunteer positions (6)</h1></Col>
                    <Col md={2}></Col>
                    <Col md={2} className="m-0 text-right mb-2"><Button variant="primary" className='text-center p-18 fw-600 px-3 h-49'>
                        Contact us
                    </Button></Col>
                </Row>
                {/* <div className='d-flex justify-content-between align-items-center mt-5'>
                    <h1 className='m-0'>Open volunteer positions (6)</h1>
                    <Button variant="primary" className='text-center p-18 fw-600 px-3 h-49'>
                        Contact us
                    </Button>
                </div> */}
                <Row className='my-5'>
                    <Col className='mb-2' lg={4}>
                        <div className='card-shadow bg-white rounded p-4 h-100'>
                            <h3 className='font-size-xl color-primary fw-500'>
                                Looking to contribute to the growth of TrashMob.eco?
                            </h3>
                            <p className='p-18'>There are many ways to get involved in the growth of TrashMob.eco besides picking litter.</p>
                            <p className='p-18'>On this page are a few ways you can contribute from the comfort of your own home! We encourage you to reach out even if you don't have all the preferred skills.</p>
                            <p className='p-18'>If you are interested in any of these opportunities, contact us at </p>
                            <p className='p-18 color-primary'>info@trashmob.eco.</p>
                        </div>
                    </Col>
                    <Col className='mb-2' lg={8}>
                        <div className='p-4 directorCard m-0 rounded-0 mb-1'>
                            <Row className='align-items-center'>
                                <Col md={6} className='mb-3'>
                                    <h3 className='fw-500 font-size-xl m-0'>UX/UI designer</h3>
                                </Col>
                                <Col md={6} className='text-right mb-3'>
                                    <Button variant="outline" className='text-center para px-3 h-49 event-list-event-type p-15 m-0'>
                                        Product
                                    </Button>
                                </Col>
                            </Row>
                            <p className='p-18'>
                                Design for mobile app and website
                            </p>
                            <p className='p-15'>Preferred skills: Figma</p>
                            <ReadMore>
                                Doggo ipsum borking doggo the neighborhood pupper wrinkler ruff doggo, shoob heck ur givin me a spook yapper, fluffer doge big ol pupper. Waggy wags fluffer very good spot big ol heckin.
                            </ReadMore>
                        </div>
                        <div className='p-4 directorCard m-0 rounded-0 mb-1'>
                            <Row className='align-items-center'>
                                <Col md={6} className='mb-3'>
                                    <h3 className='fw-500 font-size-xl m-0'>Web developer</h3>
                                </Col>
                                <Col md={6} className='text-right mb-3'>
                                    <Button variant="outline" className='text-center para px-3 h-49 event-list-event-type p-15 m-0'>
                                        Product
                                    </Button>
                                </Col>
                            </Row>
                            <p className='p-18'>
                                Develop website with React JS and .NETCore
                            </p>
                            <p className='p-15'>Preferred skills: ReactJS, CSS, Azure Maps, AzureAD B2C, and Github
                            </p>
                        </div>
                        <div className='p-4 directorCard m-0 rounded-0 mb-1'>
                            <Row className='align-items-center'>
                                <Col md={6} className='mb-3'>
                                    <h3 className='fw-500 font-size-xl m-0'>Mobile developer</h3>
                                </Col>
                                <Col md={6} className='text-right mb-3'>
                                    <Button variant="outline" className='text-center para px-3 h-49 event-list-event-type p-15 m-0'>
                                        Product
                                    </Button>
                                </Col>
                            </Row>
                            <p className='p-18'>
                                Develop mobile app with Xamarin
                            </p>
                            <p className='p-15'>Preferred skills: content TBD
                            </p>
                        </div>
                        <div className='p-4 directorCard m-0 rounded-0 mb-1'>
                            <Row className='align-items-center'>
                                <Col md={6} className='mb-3'>
                                    <h3 className='fw-500 font-size-xl m-0'>Mobile product manager</h3>
                                </Col>
                                <Col md={6} className='text-right mb-3'>
                                    <Button variant="outline" className='text-center para px-3 h-49 event-list-event-type p-15 m-0'>
                                        Product
                                    </Button>
                                </Col>
                            </Row>
                            <p className='p-18'>
                                Manage mobile app development
                            </p>
                            <p className='p-15'>Preferred skills: content TBD
                            </p>
                        </div>
                        <div className='p-4 directorCard m-0 rounded-0 mb-1'>
                            <Row className='align-items-center'>
                                <Col md={6} className='mb-3'>
                                    <h3 className='fw-500 font-size-xl m-0'>Accounting & finance</h3>
                                </Col>
                                <Col md={6} className='text-right mb-3'>
                                    <Button variant="outline" className='text-center para px-3 h-49 event-list-event-type p-15 m-0'>
                                        Business
                                    </Button>
                                </Col>
                            </Row>
                            <p className='p-18'>
                                Advise and create TrashMob.eco's financial system
                            </p>
                            <p className='p-15'>Preferred skills: treasury, bookeeping, and regulations knowledge
                            </p>
                            <ReadMore>
                                Doggo ipsum borking doggo the neighborhood pupper wrinkler ruff doggo, shoob heck ur givin me a spook yapper, fluffer doge big ol pupper. Waggy wags fluffer very good spot big ol heckin.
                            </ReadMore>
                        </div>
                        <div className='p-4 directorCard m-0 rounded-0'>
                            <Row className='align-items-center'>
                                <Col md={6} className='mb-3'>
                                    <h3 className='fw-500 font-size-xl m-0'>Local TrashMob representative</h3>
                                </Col>
                                <Col md={6} className='text-right mb-3'>
                                    <Button variant="outline" className='text-center para px-3 h-49 event-list-event-type p-15 m-0'>
                                        Business
                                    </Button>
                                </Col>
                            </Row>
                            <p className='p-18'>
                                Connect with local leaders, cities, and companies to organize events
                            </p>
                            <p className='p-15'>Preferred skills: city and corporate connections, regulations knowledge
                            </p>
                        </div>
                    </Col>
                </Row>
            </Container>
        </>
    );
}
