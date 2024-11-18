import { Link } from 'react-router-dom';
import { Button } from '@/components/ui/button';

export const WhatIsTrashmobSection = () => {
    return (
        <section id='introduction' className='bg-card'>
            <div className='mx-auto max-w-screen-xl py-8'>
                <div className='px-8 flex flex-col items-center md:grid md:auto-rows-auto md:grid-cols-5 md:gap-x-4'>
                    <div className='col-span-3'>
                        <h3 className='!mt-0 !mb-8 text-[40px] font-semibold'>What is a TrashMob?</h3>
                    </div>
                    <div className='row-span-4 col-start-4 col-span-2'>
                        <img
                            src='/img/jeremy-bezanger-u5mCQ-c5oSI-unsplash.jpg'
                            alt='What is Trashmob'
                            className='!mt-0'
                        />
                    </div>
                    <div className='row-span-3 col-start-1 col-span-3'>
                        <p className='!mt-8 md:!mt-0 !mb-16 text-center md:!text-left text-2xl font-normal'>
                            A TrashMob is a group of citizens who are willing to take an hour or two out of their lives
                            to get together and clean up their communities. Start your impact today.
                        </p>
                        <div className='flex flex-row justify-center md:justify-start gap-4'>
                            <Button asChild size='lg'>
                                <Link to='/aboutus'>Learn more</Link>
                            </Button>
                            <Button asChild size='lg'>
                                <a href='/#events'>View Upcoming Events</a>
                            </Button>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
};
