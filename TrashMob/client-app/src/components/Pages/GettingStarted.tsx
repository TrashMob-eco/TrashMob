import * as React from 'react';
import { Link } from 'react-router-dom';
import attitude from '../assets/gettingStarted/attitude.png';
import bucket from '../assets/gettingStarted/bucket.png';
import highways from '../assets/gettingStarted/highways.png';
import picker from '../assets/gettingStarted/picker.png';
import workgloves from '../assets/gettingStarted/workgloves.png';
import { HeroSection } from '../Customization/HeroSection';
import { Button } from '../ui/button';

export const GettingStarted: React.FC = () => {
    React.useEffect(() => {
        window.scrollTo(0, 0);
    });

    return (
        <div className='tailwind'>
            <HeroSection Title='Getting Started' Description='Tips and tricks to get you out there.' />
            <section className='bg-card !py-5 !px-4'>
                <h2 className='font-semibold text-center'>The Basics</h2>
                <div className='flex flex-col md:flex-row'>
                    <div className='basis-full md:basis-1/4'>
                        <div className='flex flex-col w-full max-w-[300px] mx-auto mb-5 text-center'>
                            <img src={workgloves} className='!w-25 !h-25 mx-auto' alt='Work gloves' />
                            <h6 className='font-semibold mt-2'>Work gloves</h6>
                            <span className='mt-4'>
                                We recommend{' '}
                                <a href='https://www.homedepot.com/b/Workwear-Work-Gloves/Latex/N-5yc1vZc260Z1z0z9o0'>
                                    Rubber Latex Double coated work gloves
                                </a>
                                . These will protect you from anything sharp, wet, or icky.{' '}
                            </span>
                        </div>
                    </div>
                    <div className='basis-full md:basis-1/4'>
                        <div className='flex flex-col w-full max-w-[300px] mx-auto mb-5 text-center'>
                            <img src={bucket} className='!w-25 !h-25 mx-auto' alt='bucket' />
                            <h6 className='font-semibold mt-2'>A bucket</h6>
                            <span className='mt-4'>
                                Any 5 gallon pail will do. If you don’t want to buy one, many restaurants and
                                construction industries give out up-cycled ones. We don’t recommend plastic bags since
                                they tend to tear.
                            </span>
                        </div>
                    </div>
                    <div className='basis-full md:basis-1/4'>
                        <div className='flex flex-col w-full max-w-[300px] mx-auto mb-5 text-center'>
                            <img src={picker} className='!w-25 !h-25 mx-auto' alt='bucket' />
                            <h6 className='font-semibold mt-2'>A grabber tool</h6>
                            <span className='mt-4'>
                                While not essential, we recommend a grabber tool because they help make grabbing trash
                                easier on our bodies. We like ones with a pistol grip, like the{' '}
                                <a href='https://ungerconsumer.com/product/grabber-plus/'>Unger Grabber Plus Reacher</a>
                                .
                            </span>
                        </div>
                    </div>
                    <div className='basis-full md:basis-1/4'>
                        <div className='flex flex-col w-full max-w-[300px] mx-auto mb-5 text-center'>
                            <img src={attitude} className='!w-25 !h-25 mx-auto' alt='bucket' />
                            <h6 className='font-semibold mt-2'>A good attitude</h6>
                            <span className='mt-4'>
                                Your attitude is just as important as your tools. A positive attitude increases the
                                chances that other people will join your group, and improving our communities works best
                                with others.
                            </span>
                        </div>
                    </div>
                </div>
            </section>
            <section className='!py-12 !px-6 bg-[url(/img/trashcangroup.jpg)] bg-no-repeat bg-cover bg-center'>
                <div className='container text-white bg-foreground/95 rounded-2xl !p-12'>
                    <h4>TrashMob tips</h4>
                    <ol className='list-none px-0 mt-5'>
                        <li className='mb-4'>
                            <div className='flex align-top'>
                                <span className='mr-3 font-semibold text-2xl'>1</span>
                                <span>
                                    Stay local. It will save you time and energy. You don’t have to travel to the
                                    dirtiest highway to make a difference!
                                </span>
                            </div>
                        </li>
                        <li className='mb-4'>
                            <div className='flex align-top'>
                                <span className='mr-3 font-semibold text-2xl'>2</span>
                                <span>
                                    Start with a park-based event. With few cars, nearby garbage cans, and high
                                    community exposure, this is a great way to ease in.
                                </span>
                            </div>
                        </li>
                        <li className='mb-4'>
                            <div className='flex align-top'>
                                <span className='mr-3 font-semibold text-2xl'>3</span>
                                <span>
                                    Recruit one friend or family member. Having someone join you can build a greater
                                    sense of accomplishment.
                                </span>
                            </div>
                        </li>
                        <li className='mb-4'>
                            <div className='flex align-top'>
                                <span className='mr-3 font-semibold text-2xl'>4</span>
                                <span>
                                    Set a goal. For example, aim for 2 buckets per person. Start small, and recognize an
                                    area won’t be litter-free in 30 minutes.
                                </span>
                            </div>
                        </li>
                        <li className='mb-4'>
                            <div className='flex align-top'>
                                <span className='mr-3 font-semibold text-2xl'>5</span>
                                <span>Be safe. No piece of litter is worth risking your health or well-being.</span>
                            </div>
                        </li>
                    </ol>
                </div>
            </section>
            <section className='bg-card'>
                <div className='container !py-24'>
                    <div className='flex flex-col md:flex-row gap-8 items-center'>
                        <div className='w-full md:basis-1/2'>
                            <h1 className='font-semibold'>Safety is Essential!</h1>
                            <h4 className='mt-5'>
                                All TrashMob.eco event leads and attendees are required to watch our safety video.
                                Please take a few minutes to review it now!
                            </h4>
                        </div>
                        <div className='w-full md:basis-1/2'>
                            <iframe
                                className='w-full aspect-video'
                                src='https://www.youtube.com/embed/naMY0kfyERc'
                                title='YouTube video player'
                                allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share'
                                allowFullScreen
                            />
                        </div>
                    </div>
                </div>
            </section>
            <section className='bg-background'>
                <div className='container !py-24'>
                    <div className='flex flex-col md:flex-row gap-8 items-center'>
                        <div className='w-full md:basis-1/2'>
                            <h1 className='font-semibold'>But what about the highways?</h1>
                            <h4 className='!mt-5'>
                                Please check with your Department of Transportation before creating a highway cleanup
                                event, and follow all of their guidance. Safety is the number one priority!
                            </h4>
                            <p className='font-light'>
                                In America, the highways are notorious for litter. People tend to throw trash out of
                                their windows in areas they don’t live near, and unsecured loads will fly off trucks at
                                highway speeds. With vehicles racing by at 75mph, they are also the most dangerous
                                places to pick up trash. Because of this danger, most states have formed Adopt-a-Highway
                                programs which provide guidance, training, and safety equipment for those who want to
                                work these tough environments.
                            </p>
                            <p className='font-light'>And never, ever, pick on or beside railways.</p>
                        </div>
                        <div className='w-full md:basis-1/2'>
                            <img src={highways} alt='highway overpasses' className='max-w-full' />
                        </div>
                    </div>
                </div>
            </section>
            <section className='bg-card !py-12 flex flex-col justify-center text-center'>
                <h2 className='font-semibold'>Ready to go?</h2>
                <span>Find your first event now.</span>
                <div className='px-5 mb-5'>
                    <Button asChild className='mt-2'>
                        <Link to='/'>Find events</Link>
                    </Button>
                </div>
            </section>
        </div>
    );
};
