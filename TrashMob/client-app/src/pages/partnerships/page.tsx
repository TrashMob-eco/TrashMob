import React, { FC } from 'react';
import { Link } from 'react-router';
import Ihca from '@/components/assets/partnerships/logos/IHCA.png';
import TroutUnlimited from '@/components/assets/partnerships/logos/TROUTUNLIMITEDLogo.png';
import Safetykits from '@/components/assets/partnerships/Safetykits.svg';
import Supplies from '@/components/assets/partnerships/Supplies.svg';
import TrashDisposal from '@/components/assets/partnerships/TrashDisposal.svg';
import Dollarsign from '@/components/assets/partnerships/dollarsign.svg';
import Garbage from '@/components/assets/partnerships/garbage.png';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Button } from '@/components/ui/button';

export const Partnerships: FC<any> = () => {
    return (
        <div>
            <HeroSection Title='Partnerships' Description='Connecting you with volunteers.' />
            <section className='py-10'>
                <div className='container py-5'>
                    <div className='grid grid-cols-12 gap-4'>
                        <div className='col-span-12 sm:col-span-7 space-y-4'>
                            <h1 className='font-semibold text-4xl mb-2'>What are partnerships?</h1>
                            <h4>
                                Partnering with local cities and businesses can connect TrashMob event volunteers with
                                the supplies and services they need.
                            </h4>
                            <p>
                                Partners can include cities, local businesses, and branches/locations of larger
                                companies. Services can include trash hauling and disposal locations, and supplies can
                                include buckets, grabber tools, and safety equipment. Looking for supplies and services
                                for your next event? Invite a partnership from your city! Have supplies and services to
                                offer? Become a partner!
                            </p>
                        </div>
                        <div className='col-span-12 sm:col-span-5 text-center sm:text-left'>
                            <h1 className='font-semibold text-4xl mb-2'>Our Partners</h1>
                            <div className='flex flex-row justify-center sm:justify-start'>
                                <a
                                    href='https://issaquahhighlands.com/'
                                    target='_blank'
                                    rel='noreferrer'
                                    className='m-2'
                                >
                                    <img
                                        src={Ihca}
                                        alt='Issaquah Highlands Community Association'
                                        className='h-20 mx-auto'
                                    />
                                </a>
                                <a href='https://troutunlimited.org' target='_blank' rel='noreferrer' className='m-2'>
                                    <img src={TroutUnlimited} alt='Trout Unlimited' className='h-20 mx-auto' />
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            <section className='bg-white'>
                <div className='container py-16'>
                    <div className='grid grid-cols-12 gap-x-4 gap-y-8'>
                        <div className='col-span-12 sm:col-span-6'>
                            <div className='flex flex-col space-y-4 items-center text-center sm:items-start sm:!text-left'>
                                <p className='text-2xl'>
                                    No partner for your event? Invite local government or business to join TrashMob.eco
                                    as a partner!
                                </p>
                                <Button asChild>
                                    <Link to='/inviteapartner'>Invite a partner</Link>
                                </Button>
                            </div>
                        </div>
                        <div className='col-span-12 sm:col-span-6'>
                            <div className='flex flex-col space-y-4 items-center text-center sm:items-start sm:!text-left'>
                                <p className='text-2xl'>
                                    Have supplies and services to offer? Submit an application to become a TrashMob.eco
                                    partner!
                                </p>
                                <Button asChild>
                                    <Link to='/becomeapartner'>Become a partner</Link>
                                </Button>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            <section className='py-16'>
                <div className='text-center px-0'>
                    <h2 className='font-bold text-4xl mb-3'>Partnerships support the volunteers</h2>
                    <span>Services and supplies offered can include:</span>
                    <div className='w-1/2 mx-auto grid grid-cols-4 !mt-12'>
                        <div className='col-span-4 sm:col-span-2 md:col-span-1'>
                            <div className='flex flex-col items-center !m-8'>
                                <img src={Safetykits} alt='Safety kits' className='mt-0' />
                                <span className='font-bold mt-2'>Safety gear and roadside signs</span>
                            </div>
                        </div>
                        <div className='col-span-4 sm:col-span-2 md:col-span-1'>
                            <div className='flex flex-col items-center !m-8'>
                                <img src={Supplies} alt='Supplies' className='mt-0' />
                                <span className='font-bold mt-2'>Pickup supplies such as garbage bags</span>
                            </div>
                        </div>
                        <div className='col-span-4 sm:col-span-2 md:col-span-1'>
                            <div className='flex flex-col items-center !m-8'>
                                <img src={TrashDisposal} alt='Trash Disposal & Hauling' className='mt-0' />
                                <span className='font-bold mt-2'>
                                    Use of existing dumpsters and hauling of trash to disposal site
                                </span>
                            </div>
                        </div>
                        <div className='col-span-4 sm:col-span-2 md:col-span-1'>
                            <div className='flex flex-col items-center !m-8'>
                                <img src={Dollarsign} alt='Dollar sign' className='mt-0' />
                                <span className='font-bold mt-2'>
                                    <a href='https://www.trashmob.eco/donate'>Donations</a> to TrashMob.eco fund
                                    development of our platform and programs
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            <section className='bg-card py-10'>
                <div className='container'>
                    <div className='grid grid-cols-12 gap-8'>
                        <div className='col-span-12 sm:col-span-7 space-y-4'>
                            <h1 className='font-semibold text-4xl'>Making the most out of partnerships</h1>
                            <p>
                                A successful clean up event depends upon a team of volunteers and the support of
                                partners: community businesses, organizations and governments. Volunteer organizers set
                                an event location, rally member support and utilize partnership provisions.
                            </p>
                            <p>
                                TrashMob administrators confirm, approve and connect partners with event organizers.
                                Partners and their form of support are indicated on event registration pages. Then local
                                teamwork commences! Event organizers and partners coordinate access of supplies,
                                services and instructions. Partners are selected upon availability and proximity to the
                                event. Note: Supplied services from a given partner may vary by location/branch.{' '}
                            </p>
                        </div>
                        <div className='col-span-12 sm:col-span-5'>
                            <img src={Garbage} alt='garbage bags being picked up' className='mt-0 m-auto' />
                        </div>
                    </div>
                </div>
            </section>
        </div>
    );
};
