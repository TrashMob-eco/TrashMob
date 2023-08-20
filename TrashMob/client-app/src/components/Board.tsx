import * as React from 'react';
import { Col, Container, Image, Row } from 'react-bootstrap';
import linkedIn from './assets/card/linkedin.svg';
import JoeBeernink from './assets/boardOfDirectors/JoeBeernink.jpg';
import DarylBarber from './assets/boardOfDirectors/darylbarber.jpg';
import JakeDiliberto from './assets/boardOfDirectors/JakeDiliberto.svg';
import KevinGleason from './assets/boardOfDirectors/KevinGleason.svg';
import SandraMau from './assets/boardOfDirectors/SandraMau.png';
import ValerieWilden from './assets/boardOfDirectors/ValerieWilden.svg';
import SofiaSamuels from './assets/boardOfDirectors/SofiaSamuels.png';
import { HeroSection } from './Customization/HeroSection'

export const Board: React.FC = () => {

    React.useEffect(() => {
        window.scrollTo(0, 0);
    })

    return (
        <>
            <HeroSection Title='Board of Directors' Description='Meet our team!'></HeroSection>
            <Container className='my-5 pb-5'>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={JoeBeernink} alt="Joe Beernink" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Joe Beernink</h2>
                                <a href="https://www.linkedin.com/in/joebeernink/"><Image src={linkedIn} alt="linkedin icon" className="h-100 mt-0 object-fit-cover" /></a>
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Board President</h5>
                            <p className='font-size-sm color-grey'>Joe Beernink is a software developer with over 25 years of industry experience developing mission-critical software.</p>
                            <p className='font-size-sm color-grey'>Joe grew up on a small farm in Southern Ontario, Canada, working and playing in the great outdoors, graduated with a degree in Space Science from York University in Toronto in 1994, and moved to the US in 1996. He previously lived in Michigan and Colorado before making Washington State his home in 1999.</p>
                            <p className='font-size-sm color-grey'>In 2021, Joe was inspired by Edgar McGregor, a climate activist in California, to get out and start cleaning up his community. After seeing just how much work needed to be done, Joe envisioned a website that enabled like-minded people to get out and start cleaning the environment together, and the idea for TrashMob.eco was born.</p>
                            <p className='font-size-sm color-grey'>Joe resides in Issaquah, WA with his 2 kids.</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={DarylBarber} alt="Daryl Barber" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Daryl Barber</h2>
                                <a href="https://www.linkedin.com/in/daryl-r-barber-9abb8123/"><Image src={linkedIn} alt="linkedin icon" className="h-100 mt-0 object-fit-cover" /></a>
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Board Treasurer</h5>
                            <p className='font-size-sm color-grey'>Daryl R. Barber is a Finance Professional with extensive experience at highly complex public and
                                private-equity owned companies within a broad range of businesses, including industrials,
                                chemicals, and software technology and services. Daryl specializes in finance strategies, included
                                in treasury, investor relations, M&A, financial planning & analysis, audit, and corporate and
                                business controllership. Daryl provides consulting services, currently acting as interim Chief
                                Financial Officer and interim Controller and Treasurer for two technology driven companies. </p>
                            <p className='font-size-sm color-grey'>In addition to this finance background, Daryl has experience with several 501(c)(3) organizations,
                                all of which provide assistance to the needy and disadvantaged through education, health, and
                                other human services.
                            </p>
                            <p className='font-size-sm color-grey'>Having completed his undergraduate studies at the University of Hartford and his graduate studies
                                at Fairleigh Dickinson University, Daryl now resides, with his wife, three children, and a beagle,
                                in Malvern, Pennsylvania.
                            </p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <a href="https://www.linkedin.com/in/valerie-day-wilden-283a13b5/"><Image src={ValerieWilden} alt="Valerie Wilden" className="h-100 mt-0 object-fit-cover rounded" /></a>
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Valerie Wilden</h2>
                                <Image src={linkedIn} alt="linkedIn icon" className="h-100 mt-0 object-fit-cover" />
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Board Secretary</h5>
                            <p className='font-size-sm color-grey'>Valerie Wilden is principal consultant for Vivid Communication, the public relations and marketing agency she founded after 35+ years of media and pr, operations, government affairs, crisis management and fundraising for Pennsylvania’s largest nonprofit healthcare organization of its kind. Its uniquely diverse nature also required  Mrs. Wilden to spearhead growth-related communication, planning and volunteer relations for entities that supported its core medical and charitable mission:  a performing arts center, a multitude of outreach programs, special events including renowned VIPS, an auto-repair service, resale shops and the award winning five-state PRESENTS FOR PATIENTS® program, of which Valerie was a television spokesperson.</p>
                            <p className='font-size-sm color-grey'>
                                Now at Vivid Communication, she consults with charity and for-profit organizations by writing marketing plans, boosting social media, creating promotions and guiding efforts toward highest net revenue potential. She is a three-term trustee of Westminster College, where she earned her Bachelor of Arts in English. Upon graduating with a Master of Arts in Journalism and Mass Communication from Point Park University, she taught corporate writing there.
                            </p>
                            <p className='font-size-sm color-grey'>She and her husband, Greg, live in Wexford, a northern suburb of Pittsburgh, Pennsylvania and are parents of Alyssa, Scott, and Dayne.</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={JakeDiliberto} alt="Jake Diliberto" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Jake Diliberto</h2>
                                <a href="https://www.linkedin.com/in/jakediliberto/"><Image src={linkedIn} alt="linkedIn icon" className="h-100 mt-0 object-fit-cover" /></a>
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Member at large</h5>
                            <p className='font-size-sm color-grey'>Jake Diliberto is a senior operations professional having managed a (NYSE) fortune 500 portfolio. He has proven expertise in transformational management with comprehensive experience managing multiple operating units, driving change management, project management, process reengineering, resource optimization, and systems implementations.</p>
                            <p className='font-size-sm color-grey'>Jake also has veteran/military front-line field leadership expertise, having directed and motivated teams in high pressure settings with real-time consequences.</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={KevinGleason} alt="Kevin Gleason" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Kevin Gleason</h2>
                                <a href="https://www.linkedin.com/in/kevin-gleason-78a9236/"><Image src={linkedIn} alt="linkedIn icon" className="h-100 mt-0 object-fit-cover" /></a>
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Member at large</h5>
                            <p className='font-size-sm color-grey'>Kevin Gleason currently is Vice President New York Life and Chief Compliance Officer at MainStay Funds and Index IQ ETFs.  He is a seasoned legal and compliance professional with over 25 years of experience working for 5 Fortune Five Hundred diversified financial services organizations.</p>
                            <p className='font-size-sm color-grey'>
                                Mr. Gleason has advised and transacted business globally on 5 continents including across Europe, the Middle East, Asia, and South America.  He has counseled C-suite executives and boards of directors on the creation of compliance and ethics programs; the development of controls, training, testing, conflicts identification, and risk assessments; and the structuring of governance frameworks.
                            </p>
                            <p className='font-size-sm color-grey'>Mr. Gleason has a law degree and a masters in financial services law.  He has earned an MBA from The University of Chicago and BA from University of Notre Dame.  He is or has been a board member at Arizona Science Center, National Society of Compliance Professionals, and Journal of Financial Compliance. He is a frequent speaker at and contributor to industry events and publications.</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={SandraMau} alt="Sandra Mau" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Sandra Mau</h2>
                                <a href="https://www.linkedin.com/in/sandramau/"><Image src={linkedIn} alt="linkedIn icon" className="h-100 mt-0 object-fit-cover" /></a>
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Member at large</h5>
                            <p className='font-size-sm color-grey'>Sandra is VP of Product for Cloud Solutions at Clarivate (NYSE:CLVT). Prior to joining Clarivate via acquisition, she was the CEO and Founder of TrademarkVision, an award-winning AI/Computer Vision startup doing visual brand protection.</p>
                            <p className='font-size-sm color-grey'>Sandra is very active in supporting tech and startup communities. She was the Founding Chair of IEEE QLD Women in Engineering, and listed as one of Australia's Top 50 Female Programmers by Pollenizer 2014, and one of Australia's Top 100 Most Influential Engineers by Engineer's Australia 2015. She's also a regular participant in hackathons including past GovHacks and International Women's Day. She was recognised in 2018 by Pittsburgh Business Times with the Pittsburgh Innovator Award and by QUT with the Innovation and Entrepreneurship Outstanding Alumni Award.</p>
                            <p className='font-size-sm color-grey'>She holds a Masters in Robotics from Carnegie Mellon University, a Bachelors in Engineering Science (Aerospace) from University of Toronto, and an MBA from Queensland University of Technology (QUT).</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={SofiaSamuels} alt="Sofia Samuels" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Sofia Samuels</h2>
                                <a href="https://www.linkedin.com/in/sofiasamuels/"><Image src={linkedIn} alt="linkedIn icon" className="h-100 mt-0 object-fit-cover" /></a>
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Member at large</h5>
                            <p className='font-size-sm color-grey'>Sofia Samuels is Co-Founder and Chief Strategist at Innovista Partners.</p>
                            <p className='font-size-sm color-grey'>Prior to co-founding Innovista Partners, Ms. Samuels served as Vice President, Marketing & Communications for Ruan Transportation Management Systems, Vice President, Marketing Services for Sachs Horace Janson, Chief Marketing Officer for the American Welding Society (AWS) and held a variety of marketing and business development roles at Ryder System Inc., United Parcel Service, Carnival Cruise Lines and DHL.   Her professional experience spans a broad blend of B2B and B2C expertise with specialties including integrated marketing, sales and customer service leadership, eCommerce, and publishing/editorial.</p>
                            <p className='font-size-sm color-grey'>Sofia has been recognized as one of South Florida’s Top Black Corporate Executives and was honored by Legacy Magazine as one of South Florida’s Top 25 Most Influential and Prominent Black Women in Business. Ms. Samuels received her M.B.A. from the University of Connecticut and currently resides in Miami Beach, Florida.</p>
                        </Col>
                    </Row>
                </div>
            </Container>
        </>
    );
}

