import { FC } from 'react';
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
            <section className='py-12'>
                <div className='container'>
                    <div className='grid grid-cols-1 md:grid-cols-2 gap-12 items-start'>
                        <div className='space-y-4'>
                            <h2 className='font-semibold text-3xl'>What are partnerships?</h2>
                            <p className='text-lg text-muted-foreground'>
                                Partnering with local cities and businesses can connect TrashMob event volunteers with
                                the supplies and services they need.
                            </p>
                            <p>
                                Partners can include cities, local businesses, and branches/locations of larger
                                companies. Services can include trash hauling and disposal locations, and supplies can
                                include buckets, grabber tools, and safety equipment. Looking for supplies and services
                                for your next event? Invite a partnership from your city! Have supplies and services to
                                offer? Become a partner!
                            </p>
                        </div>
                        <div>
                            <h2 className='font-semibold text-3xl mb-6 text-center md:text-left'>Our Partners</h2>
                            <div className='flex flex-wrap gap-6 justify-center md:justify-start items-center'>
                                <a href='https://issaquahhighlands.com/' target='_blank' rel='noreferrer noopener'>
                                    <img src={Ihca} alt='Issaquah Highlands Community Association' className='h-20' />
                                </a>
                                <a href='https://troutunlimited.org' target='_blank' rel='noreferrer noopener'>
                                    <img src={TroutUnlimited} alt='Trout Unlimited' className='h-20' />
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            <section className='bg-white'>
                <div className='container py-12'>
                    <div className='grid grid-cols-1 md:grid-cols-2 gap-8'>
                        <div className='flex flex-col space-y-3 items-center text-center md:items-start md:text-left!'>
                            <p className='text-lg font-medium'>
                                No partner for your event? Invite local government or business to join TrashMob.eco as a
                                partner!
                            </p>
                            <Button asChild>
                                <Link to='/inviteapartner'>Invite a partner</Link>
                            </Button>
                        </div>
                        <div className='flex flex-col space-y-3 items-center text-center md:items-start md:text-left!'>
                            <p className='text-lg font-medium'>
                                Have supplies and services to offer? Submit an application to become a TrashMob.eco
                                partner!
                            </p>
                            <Button asChild>
                                <Link to='/becomeapartner'>Become a partner</Link>
                            </Button>
                        </div>
                    </div>
                </div>
            </section>
            <section className='py-16'>
                <div className='container text-center'>
                    <h2 className='font-bold text-3xl mb-2'>Partnerships support the volunteers</h2>
                    <p className='text-muted-foreground'>Services and supplies offered can include:</p>
                    <div className='grid grid-cols-2 md:grid-cols-4 gap-8 mt-10 max-w-3xl mx-auto'>
                        <div className='flex flex-col items-center'>
                            <img src={Safetykits} alt='Safety kits' className='mt-0 h-16' />
                            <span className='font-semibold text-sm mt-3'>Safety gear and roadside signs</span>
                        </div>
                        <div className='flex flex-col items-center'>
                            <img src={Supplies} alt='Supplies' className='mt-0 h-16' />
                            <span className='font-semibold text-sm mt-3'>Pickup supplies such as garbage bags</span>
                        </div>
                        <div className='flex flex-col items-center'>
                            <img src={TrashDisposal} alt='Trash Disposal & Hauling' className='mt-0 h-16' />
                            <span className='font-semibold text-sm mt-3'>
                                Dumpsters and hauling of trash to disposal sites
                            </span>
                        </div>
                        <div className='flex flex-col items-center'>
                            <img src={Dollarsign} alt='Dollar sign' className='mt-0 h-16' />
                            <span className='font-semibold text-sm mt-3'>
                                <a href='https://www.trashmob.eco/donate'>Donations</a> to fund our platform and
                                programs
                            </span>
                        </div>
                    </div>
                </div>
            </section>
            <section className='bg-card py-12'>
                <div className='container'>
                    <div className='grid grid-cols-1 md:grid-cols-2 gap-8 items-center'>
                        <div className='space-y-4'>
                            <h2 className='font-semibold text-3xl'>Making the most out of partnerships</h2>
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
                        <div>
                            <img src={Garbage} alt='garbage bags being picked up' className='mt-0 mx-auto rounded-lg' />
                        </div>
                    </div>
                </div>
            </section>
            <section className='bg-muted py-12'>
                <div className='container text-center'>
                    <h2 className='font-semibold text-3xl mb-4'>Looking to start a community?</h2>
                    <p className='max-w-2xl mx-auto mb-6'>
                        Community partners are cities, counties, and nonprofits that want a branded TrashMob page with
                        volunteer engagement tools, adoption programs, and impact analytics — different from service
                        partners who provide supplies and hauling for events.
                    </p>
                    <Button asChild>
                        <Link to='/for-communities'>Learn about Communities</Link>
                    </Button>
                </div>
            </section>
        </div>
    );
};
