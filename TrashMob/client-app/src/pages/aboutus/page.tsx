import * as React from 'react';
import { Link } from 'react-router';
import padStart from 'lodash/padStart';
import { HeroSection } from '@/components/Customization/HeroSection';
import { GettingStartSection } from '@/pages/_home/getting-start-section';
import { Button } from '@/components/ui/button';

export const AboutUs: React.FC = () => {
    return (
        <div>
            <HeroSection Title='About Us' Description='Learn about the TrashMob movement.' />

            {/* What is a TrashMob? */}
            <section className='bg-card'>
                <div className='container py-24!'>
                    <div className='flex flex-col md:flex-row gap-8 items-center'>
                        <div className='w-full md:basis-1/2 space-y-4'>
                            <h2 className='font-semibold'>What is a TrashMob?</h2>
                            <h4 className='mt-5!'>
                                A TrashMob is a group of citizens who are willing to take an hour or two out of their
                                lives to get together and clean up their communities.
                            </h4>
                            <p className='font-light'>
                                Whether the motivation is to better your local community, connect with like-minded
                                individuals, or improve your own mental health and wellbeing, TrashMob provides an
                                avenue for accomplishing these goals. To participate, all it takes is the willingness to
                                get your hands a little dirty and a desire to leave the world better than how you found
                                it. Whether it's your neighborhood, a park, a stream, a road, or even a parking lot of a
                                big box store, all litter being cleaned up contributes to our goal of making this planet
                                of ours a little better for the next generation.
                            </p>
                            <Button className='mt-2' asChild>
                                <Link to='/'>Find an event</Link>
                            </Button>
                        </div>
                        <div className='w-full md:basis-1/2 flex justify-center'>
                            <img src='/img/trashcan-artwork.png' alt='trash cans' className='max-h-[400px]' />
                        </div>
                    </div>
                </div>
            </section>

            {/* Benefits of joining TrashMob */}
            <section className='bg-background'>
                <div className='container py-24!'>
                    <h2 className='font-semibold mb-6'>Benefits of joining TrashMob</h2>
                    <ol className='list-none px-0 space-y-4'>
                        {[
                            'TrashMobs allow you to connect with your local community and foster relationships built on positive changes.',
                            'TrashMobs clean up our parks, streams, and neighborhoods which benefits our Earth and our communities.',
                            'A TrashMob can tackle highly polluted areas in a shorter time than individuals which improves morale and satisfaction.',
                            'TrashMobs can garner attention from neighbors, friends, and local governments that can spur on more cleanups.',
                            'TrashMobs can acquire municipal support which can help with the hauling of gathered trash and providing supplies.',
                        ].map((line, index) => (
                            <li key={index}>
                                <div className='flex align-top'>
                                    <span className='mr-3 font-semibold text-2xl text-primary'>{index + 1}</span>
                                    <span>{line}</span>
                                </div>
                            </li>
                        ))}
                    </ol>
                </div>
            </section>

            {/* The Journey */}
            <section className='bg-card'>
                <div className='container py-24!'>
                    <h2 className='font-semibold text-center mb-6'>The Journey</h2>
                    <div id='timeline-steps' className='flex justify-center flex-wrap'>
                        {[
                            {
                                title: 'The Inspiration that florished',
                                body: 'TrashMob founder, Joe Beernink, first gained inspiration for TrashMob from Microsoft colleagues. An idea formed for a project that would bring people together to take small, positive actions that would cascade into meaningful long term effects.',
                            },
                            {
                                title: 'Starting Out',
                                body: 'Joe, passionate about pollution and climate change, took inspiration from Edgar McGregor in California who spent over 580 days cleaning up a park in his community. He began cleaning up his own local parks, and others soon began to take notice.',
                            },
                            {
                                title: 'Connecting others',
                                body: 'After realizing cleaning up would be too much to do on his own, Joe saw potential in connecting others. He knew that technology had potential to fix these human problems, and assembled a team to help bring his vision to life.',
                            },
                            {
                                title: 'TrashMob was Born',
                                body: 'Today, TrashMob has provided communities the opportunity to create and participate in TrashMobs of their own. The TrashMob team is continuously coming up with more ways to grow the TrashMob community to achieve the common goal of bettering our community!',
                            },
                        ].map((item, index) => (
                            <div className='timeline-step relative' key={index}>
                                <div className='timeline-content w-56 flex flex-col items-center text-center relative z-10'>
                                    <div className='inner-circle bg-primary text-primary-foreground w-12 h-12 rounded-full flex items-center justify-center font-bold text-base relative'>
                                        {padStart(`${index + 1}`, 2, '0')}
                                    </div>
                                    <h6 className='font-semibold mt-3 mb-1 px-3'>{item.title}</h6>
                                    <p className='px-3'>{item.body}</p>
                                </div>
                                {index !== 0 ? (
                                    <div className='h-[2px] bg-primary absolute z-0 top-[24px] left-0 right-50 w-1/2' />
                                ) : null}
                                {index !== 3 ? (
                                    <div className='h-[2px] bg-primary absolute z-0 top-[24px] right-0 left-50 w-1/2' />
                                ) : null}
                            </div>
                        ))}
                    </div>
                </div>
            </section>

            <GettingStartSection />
        </div>
    );
};
