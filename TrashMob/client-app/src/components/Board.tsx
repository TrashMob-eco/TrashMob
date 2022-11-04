import * as React from 'react';
import { Col, Container, Image, Row } from 'react-bootstrap';
import globes from './assets/gettingStarted/globes.png';
import linkedIn from './assets/card/linkedin.svg';
import JoeBeernik from './assets/boardOfDirectors/JoeBeernik.svg';
import MikeRosen from './assets/boardOfDirectors/MikeRosen.svg';
import DebiEinmo from './assets/boardOfDirectors/DebiEinmo.svg';
import TerriRegister from './assets/boardOfDirectors/TerriRegister.svg';
import DarrylWalter from './assets/boardOfDirectors/DarrylWalter.svg';
import JakeDiliberto from './assets/boardOfDirectors/JakeDiliberto.svg';
import JeremiahSteen from './assets/boardOfDirectors/JeremiahSteen.svg';
import KevinGleason from './assets/boardOfDirectors/KevinGleason.svg';

export const Board: React.FC = () => {
    return (
        <>
            <Container fluid className='bg-grass'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className='font-weight-bold'>Board of Directors</h1>
                        <h4 className="font-weight-bold">
                            Meet our crew!
                        </h4>
                    </Col>
                    <Col md={5}>
                        <Image src={globes} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>
            <Container className='my-5'>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={JoeBeernik} alt="globes" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Joe Beernik</h2>
                                <Image src={linkedIn} alt="globes" className="h-100 mt-0 object-fit-cover" />
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>President</h5>
                            <p className='font-size-sm color-grey'>Joe Beernink is a software developer with over 25 years of industry experience developing mission-critical software.</p>
                            <p className='font-size-sm color-grey'>Joe grew up on a small farm in Southern Ontario, Canada, working and playing in the great outdoors, graduated with a degree in Space Science from York University in Toronto in 1994, and moved to the US in 1996. He previously lived in Michigan and Colorado before making Washington State his home in 1999.</p>
                            <p className='font-size-sm color-grey'>In 2021, Joe was inspired by Edgar McGregor, a climate activist in California, to get out and start cleaning up his community. After seeing just how much work needed to be done, Joe envisioned a website that enabled like-minded people to get out and start cleaning the environment together, and the idea for TrashMob.eco was born.</p>
                            <p className='font-size-sm color-grey'>Joe currently works for Microsoft in Redmond, WA and resides in Issaquah, WA with his 2 kids.</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={MikeRosen} alt="globes" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Mike Rosen</h2>
                                <Image src={linkedIn} alt="globes" className="h-100 mt-0 object-fit-cover" />
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Vice President</h5>
                            <p className='font-size-sm color-grey'>Mike Rosen is a creative, insights-driven, digital-first senior executive with more than 25-years of experience leading integrated marketing and PR agencies, nonprofit organizations, brands, businesses, and individuals to significantly increase their ability to provide sustained value to clients, customers, donors, community members, and partners.</p>
                            <p className='font-size-sm color-grey'>
                                Mike is chief marketing officer of the Guide Dog Foundation and America's VetDogs, sister nonprofit organizations that train and place assistance dogs with individuals who are blind or have low vision, and to veterans, active-duty service members, and first responders with disabilities. For much of the last decade, he has served in revenue generation, marketing, and fundraising leadership roles for two vital nonprofits — Keep America Beautiful and Fairfield County’s Community Foundation — focused on community improvement, litter abatement, recycling education, social and environmental justice, economic opportunity, affordable healthcare and housing, and racial, gender, and education equity. He spent the first twenty years of his career helping to build two integrated marketing and PR agencies, working with some extraordinary brands, businesses, and personalities.
                            </p>
                            <p className='font-size-sm color-grey'>
                                A “full stack” marketing communications, business development, and fundraising leader, Mike is an entrepreneurial, employee-centric individual, with a consistent record of generating record revenues, protecting and fortifying reputation, building brands and community, connecting brands with social purpose, motivating colleagues, and outperforming competition. He is steadfast about creating and contributing to winning organizational cultures, fighting for racial and gender equity, advancing social and environmental justice, and in the power of creativity, collaboration, and community to drive action, influence perceptions, and create meaningful and lasting change.
                            </p>
                            <p className='font-size-sm color-grey'>Mike lives in Connecticut with his wife and two children where he served as a three-term elected member of the Town of Southbury Board of Selectman and four years as a Parks and Recreation Commissioner.</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={DebiEinmo} alt="globes" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Debi Einmo</h2>
                                <Image src={linkedIn} alt="globes" className="h-100 mt-0 object-fit-cover" />
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Treasurer</h5>
                            <p className='font-size-sm color-grey'>TBD</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={TerriRegister} alt="globes" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Terri Register</h2>
                                <Image src={linkedIn} alt="globes" className="h-100 mt-0 object-fit-cover" />
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Secretary</h5>
                            <p className='font-size-sm color-grey'>Terri Register, from North Carolina, has served in the nonprofit sector in many roles including Administrator, Development Lead, Capacity Builder, Trainer, and Board Member. Her areas of focus are Sustainability, Economic Development, and Human Services. Her areas of expertise include administration, grantmaking, board development, and impact evaluation.</p>
                            <p className='font-size-sm color-grey'>
                                She has coordinated initiatives in North America, Central America, the Caribbean, and Oceania; and is seeking to expand her work internationally into South American, Middle Eastern, South Asian, and African nations. She has also implemented administrative systems and internal processes to combat silos and to increase efficiency in achieving and monitoring impact. She has helped organizations position themselves for receiving and maintaining funding. She has increased the capacity and visibility of several organizations by engaging new prospective partners.</p>
                            <p className='font-size-sm color-grey'>
                                Terri works as a Program Officer for Carrot.net, a company that provides end-to-end services for competitions that engage fresh perspectives, produce new solutions, and drive tangible results. Carrot's clients include corporations, government entities, nonprofit foundations, and other philanthropic organizations and individuals who offer awards ranging from up to $100 million to tackle some of the world's biggest problems in areas including economic opportunity, gender and racial equity, aging, democracy, health and wellness, space and technology, housing and homelessness, the environment and more.  </p>
                            <p className='font-size-sm color-grey'>
                                In her spare time, she enjoys community volunteering, hiking mountains, traveling abroad, reading nonfiction, painting landscapes, running 5K's, learning languages, watching arthouse films, and trying new recipes.</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={DarrylWalter} alt="globes" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Darryl Walter</h2>
                                <Image src={linkedIn} alt="globes" className="h-100 mt-0 object-fit-cover" />
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Member at large</h5>
                            <p className='font-size-sm color-grey'>Darryl Walter has over 25 years of membership marketing and nonprofit fundraising experience with such diverse organizations as the Solid Waste Association of North America (SWANA), The Wildlife Society (TWS), American Association for the Advancement of Science (AAAS), and Special Olympics Inc (SOI).</p>
                            <p className='font-size-sm color-grey'>
                                Under his guidance, SWANA and TWS reached record membership. Darryl’s professional experience includes all aspects of direct marketing as well as conference planning.</p>
                            <p className='font-size-sm color-grey'>
                                Darryl is a long suffering Cleveland sports fan and lives in Bethesda, MD with his wife and is incredibly proud of his three adult children.</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={JakeDiliberto} alt="globes" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Jake Diliberto</h2>
                                <Image src={linkedIn} alt="globes" className="h-100 mt-0 object-fit-cover" />
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Member at large</h5>
                            <p className='font-size-sm color-grey'>Content TBD</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={JeremiahSteen} alt="globes" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Jeremiah Steen</h2>
                                <Image src={linkedIn} alt="globes" className="h-100 mt-0 object-fit-cover" />
                            </div>
                            <h5 className='my-3 fw-500 color-grey'>Member at large</h5>
                            <p className='font-size-sm color-grey'>Content TBD</p>
                        </Col>
                    </Row>
                </div>
                <div className='p-4 directorCard'>
                    <Row>
                        <Col md={5}>
                            <Image src={KevinGleason} alt="globes" className="h-100 mt-0 object-fit-cover rounded" />
                        </Col>
                        <Col md={7}>
                            <div className='d-flex justify-content-between align-items-center'>
                                <h2 className='m-0 fw-500 font-size-xl color-primary '>Kevin Gleason</h2>
                                <Image src={linkedIn} alt="globes" className="h-100 mt-0 object-fit-cover" />
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
            </Container>
        </>
    );
}

