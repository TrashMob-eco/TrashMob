import * as React from 'react';
import attitude from './assets/gettingStarted/attitude.png';
import bucket from './assets/gettingStarted/bucket.png';
import highways from './assets/gettingStarted/highways.png';
import picker from './assets/gettingStarted/picker.png';
import trashcangroup from './assets/gettingStarted/trashcangroup.png';
import wear from './assets/gettingStarted/wear.png';
import workgloves from './assets/gettingStarted/workgloves.png';
import { Link } from 'react-router-dom';
import { Col, Container, Row, Image } from 'react-bootstrap';

export const GettingStarted: React.FC = () => {
    return (
        <>
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
                            <span className='mt-4'>While not essential, we recommend a grabber tool because they help make grabbing trash easier on our bodies.  We recommend ones with a pistol grip, like the <a href='https://ungerconsumer.com/product/grabber-plus/'>
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
                                                <span>Start with a park-based event. With little to no cars, nearby garbage cans, and high community exposure, this is a great way to ease in.</span>
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
                                                <span>Set a goal. For example, aim for 2 buckets per person. Smart small, and recognize an area won’t be litter-free in 30 minutes.</span>
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
                                Wear clothes you won’t mind getting dirty or possibly torn. We strongly encourage a reflection vest for clean ups along roadsides.
                            </p>
                            <p className="font-weight-light">
                                Long sleeves and jeans that keep branches and thorns off are best. Most importantly, wear good footwear that are preferably water resistant and thick.
                                Old tennis shoes and hiking boots work well. Shorts and t-shirts are sometimes enough for some urban pickups, but remember to dress safely and for the weather.
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
                                Please check with your Department of Transportation before creating a highway cleanup event, and follow all their guidance. Safety is the number one priority!
                            </p>
                            <p className="font-weight-light">
                                In America, the highways are notorious for litter. People tend to throw trash out of their windows in areas they don’t live near, and unsecured loads tend to fly
                                out of trucks at highway speeds. With vehicles racing by at 75mph, they are also the most dangerous places to pick up trash. Because of this danger,
                                most states have formed Adopt-a-Highway programs which provide guidance, training, and safety equipment for those who want to work these tough environments.
                                Please check with your Deparment of Transportation before creating a highway cleanup event, and follow all of their guidance. Safety is the number one priority!
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
        // <div>
        //     <div className="card pop card-image-right" style={{backgroundImage: 'url(' + equipment + ')'}}>
        //         <h1>Getting Started</h1>
        //         <h2>The Basics</h2>
        //         <p>There are really only three things you absolutely need to have to get started working with a TrashMob:</p>
        //         <ol>
        //             <li>Work gloves</li>
        //             <li>A bucket</li>
        //             <li>A good attitude</li>
        //         </ol>
        //         <p>
        //             Really, that's all you need. A single trip to the hardware store, and you're ready to go. For the gloves, we recommend <a target="_blank" rel="noreferrer" href="https://amzn.to/3fRslpm">Rubber Latex Double coated work gloves</a>. There's a good chance you will find glass or something sharp out there, and these gloves will offer some protection from that. It's also likely that either the outside of the trash will be wet, or there will be something inside
        //                 the trash that is wet or icky. Get the gloves.
        //         </p>
        //         <p>
        //             As for the bucket, any 5 gallon pail will do. If you don't want to buy one, there are lots out there in the restaurant and construction industries that you can up-cycle. And while you can use a plastic bag, we've found that we spend a
        //             lot of time in the bushes, and a plastic bag gets caught on branches and will tear. Use the bucket for gathering, then transfer to the garbage bags for loading/hauling.
        //         </p>
        //         <p>
        //             Your attitude is just as important as your tools. A positive attitude increases the chances that other people will join you and stay longer, and decreases the likelyhood of people littering in the first place.
        //         </p>
        //     </div>
        //     <div className="card ">
        //         <h2>What to Wear</h2>
        //         <p>What you wear in a TrashMob isn't just about fashion, though I can understand why you may think it is. Wear clothes you won't mind getting dirty or possibly torn. This is also not the time for shorts and a t-shirt, unless you're strictly doing an urban pickup.
        //         A long sleeve shirt keeps the branches and thorns off. Jeans work best to shed prickers and mud. Most importantly, good footwear is key: hiking boots with thick soles a little water resistance are best; old running shoes you aren't afraid to get dirty are next up.
        //         If you come out in sandals, you'll probably regret it.
        //             </p>
        //         <p>
        //             If you are working along a roadside, we <b>strongly urge you</b> to get a <a target="_blank" rel="noreferrer" href="https://amzn.to/34whOuc">reflective vest</a>. They can save your life.
        //                 </p>
        //     </div>
        //     <div className="card  card-image-left" style={{backgroundImage: 'url(' + picker + ')'}}>
        //         <h2>What About Those Grabber Things?</h2>
        //         <p>You'll see a lot of people using grabbers to pick litter. They save a ton of bending over when picking, and really facilitate getting small pieces of plastic off the ground, or trash out of a bush. They aren't essential, but we always carry one when we're gathering, and it
        //             allows us to last a lot longer, as the constant bending-over can be exhausting. That said, there are few things to keep in mind when buying and using one:</p>
        //         <ol>
        //             <li>You get what you pay for. We've tried the $10 ones and they work, but they don't last long. You'll want to treat them very gently. They're not meant to be used as a cane, or as a prybar, or to pick up bricks with.</li>
        //             <li>Get one with a pistol grip. You may squeeze that trigger two hundred times, and ergonomics are your friend.</li>
        //             <li>We're constantly on the lookout for new and better ones, but so far, the one we've had the most success with is the <a target="_blank" rel="noreferrer" href="https://amzn.to/3fS7Mch">Unger Grabber Plus Reacher</a>. It's a little heavier than the
        //                         cheap ones, but so far so good with it. If you find a better one, please let us know.</li>
        //         </ol>
        //     </div>
        //     <div className='green-flood-image' style={{backgroundImage: 'url(' + highways + ')'}} ></div>
        //     <div className="card pop">
        //         <h2>What about the Highways?</h2>
        //         <p>In America, the highways are nortorious for litter. People tend to throw trash out their windows in areas they don't live near, and unsecured loads tend to fly out of trucks at highway speeds. Our highways can look like thousand mile long
        //                 trash dumps.</p>
        //         <p>But they are also, undoubtedly, the most dangerous places to pick up trash. With vehicles racing by at 75 MPH, driven by someone reading a text or making a phone call or eating lunch, the difference between life an death is a fraction of
        //         a second. Because of this danger, most states have formed Adopt-a-Highway programs which provide guidance, training, and safety equipment for those who want to work these tough environments. Please check with your Department of Transportation
        //                 before creating a highway cleanup event, and follow all of their guidance. <b>Safety is the number one priority</b>. If you aren't able to follow their guidelines, please choose a different type of cleanup to do.
        //         </p>
        //     </div>
        //     <div className="card pop">
        //         <h2>Joining a TrashMob</h2>
        //         <p>A couple of tips to make your first TrashMob a success:</p>
        //         <ol>
        //             <li>Stay local. Take a walk to your nearest park, even if it is near pristine, and pick up what you can. Everything matters. If you are driving an hour to an event, you're likely burning more energy to get there than is needed, and passing by lots of opportunities closer to home. That's
        //                     not to say that destination events aren't worthwhile -- they are -- but start local and work with your own community first.</li>
        //             <li>Start with a park-based event. Starting at a park takes some of the biggest challenges out of your way right at the start:
        //                     <ol>
        //                     <li>Parks are pretty serene places, so you don't have to worry about traffic zooming by.</li>
        //                     <li>Parks usually have garbage cans nearby to put the litter in so you don't have to haul it away.</li>
        //                     <li>Other people will see you picking up trash. This is <i>so</i> important. When someone sees another person picking up trash, they are much less likely to litter in the future, especially kids! People will also thank you for doing it, and that feels awesome.
        //                             And some people will, because they saw you doing it, start doing it themselves. This is so crucial to our mission. The more people we can have picking up litter, the more this effort goes viral.</li>
        //                     <li>Parks usually have parking nearby. Parking for other types of events can be a real challenge.</li>
        //                 </ol>
        //             </li>
        //             <li>Recruit one friend or family member to join you. Doing this with builds a greater feeling of enjoyment and achievement, and you're more likely to want to do it again.</li>
        //             <li>Set a goal for how much trash you want to pick up. We aim for 2 buckets per person. Depending on the density of the area you are working, that may take an hour, or may take 5 minutes. But don't set out to clean an entire state park with 1-2 people in one day. Start small.</li>
        //             <li>Most of all, <b>Be safe!</b> No piece of litter is worth risking your health or well-being.</li>
        //         </ol>
        //         <div className='carousel-inner' style={{backgroundImage: 'url(' + joinmob + ')', height: '560px'}} ></div>
        //     </div>
        // </div>
    );
}

