import { Link } from 'react-router';
import { cn } from '@/lib/utils';
import { Logo } from '@/components/Logo';

// Mobile-friendly text size (min 14px for readability)
const navClassName = 'text-center md:text-start! text-white text-sm uppercase font-medium py-1';
// Touch-friendly social icons (min 44px for tap targets)
const socialClassName = 'w-11 h-11 bg-white rounded-md! flex justify-center items-center';

export const SiteFooter = () => {
    const footerNavs = [
        { href: '/aboutus', title: 'About us' },
        { href: '/board', title: 'Board of directors' },
        { href: '/partnerships', title: 'Partners' },
        { href: '/for-communities', title: 'For Communities' },
        { href: '/faq', title: 'FAQ' },
        { href: '/news', title: 'News' },
        { href: '/volunteeropportunities', title: 'Recruiting' },
        { href: '/contactus', title: 'Contact us' },
        { href: '/privacypolicy', title: 'Privacy policy' },
        { href: '/termsofservice', title: 'Terms of service' },
    ];

    return (
        <footer>
            <div className='bg-[#212529] relative py-10! overflow-hidden'>
                <Logo
                    className='w-[900px] absolute z-0 left-[20%] md:left-[40%] inset-y-1/2 -translate-y-100 opacity-10'
                    showTagline={false}
                    showBrandName={false}
                />
                <div className='container relative z-1'>
                    <div className='w-72 md:w-96 max-w-full mb-5 mx-auto md:mx-0!'>
                        <Logo fill='#fff' className='opacity-40' />
                    </div>
                    <div className='grid grid-cols-1 md:grid-rows-3 md:grid-cols-3 gap-4 gap-x-8 md:grid-flow-col'>
                        {footerNavs.map((nav) => {
                            return nav.href.startsWith('/') ? (
                                <Link to={nav.href} key={nav.href} className={cn(navClassName)}>
                                    {nav.title}
                                </Link>
                            ) : (
                                <a href={nav.href} className={cn(navClassName)}>
                                    {nav.title}
                                </a>
                            );
                        })}
                    </div>
                    <div className='border-t border-white mt-8! md:mt-12! text-white'>
                        <div className='flex flex-col md:flex-row gap-8'>
                            <div className='basis-3/4'>
                                <div className='text-[13px] font-[Montserrat] font-serif font-medium pt-[14px]'>
                                    <p className='mb-1 text-sm text-center md:text-start!'>
                                        Copyright &copy; {new Date().getFullYear()} TRASHMOB.ECO - All rights reserved.
                                    </p>
                                    <p className='my-2 text-xs text-center md:text-start!'>
                                        TrashMob is a{' '}
                                        <a
                                            href='/img/501c3Approval.png'
                                            target='_blank'
                                            rel='noreferrer noopener'
                                            className='underline hover:text-gray-300'
                                        >
                                            non-profit, 501(c)(3) organization
                                        </a>{' '}
                                        based in Washington State, USA.
                                    </p>
                                    <p className='my-2 text-xs text-center md:text-start!'>
                                        US Federal Employer Id (EIN): 88-1286026
                                    </p>
                                </div>
                            </div>
                            <div className='basis-1/4'>
                                <div className='flex flex-row justify-center md:justify-end mt-4! gap-3'>
                                    <a
                                        href='https://www.instagram.com/trashmobinfo'
                                        target='_blank'
                                        rel='noreferrer noopener'
                                        className={socialClassName}
                                        aria-label='Instagram'
                                    >
                                        <svg
                                            className='h-5 w-5 text-black'
                                            viewBox='0 0 24 24'
                                            fill='none'
                                            stroke='currentColor'
                                            strokeWidth='2'
                                            strokeLinecap='round'
                                            strokeLinejoin='round'
                                        >
                                            <rect width='20' height='20' x='2' y='2' rx='5' ry='5' />
                                            <path d='M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z' />
                                            <line x1='17.5' x2='17.51' y1='6.5' y2='6.5' />
                                        </svg>
                                    </a>
                                    <a
                                        href='https://www.youtube.com/channel/UC2LgFmXFCA8kdkxd4IJ51BA'
                                        target='_blank'
                                        rel='noreferrer noopener'
                                        className={socialClassName}
                                        aria-label='YouTube'
                                    >
                                        <svg
                                            className='h-5 w-5 text-black'
                                            viewBox='0 0 24 24'
                                            fill='none'
                                            stroke='currentColor'
                                            strokeWidth='2'
                                            strokeLinecap='round'
                                            strokeLinejoin='round'
                                        >
                                            <path d='M2.5 17a24.12 24.12 0 0 1 0-10 2 2 0 0 1 1.4-1.4 49.56 49.56 0 0 1 16.2 0A2 2 0 0 1 21.5 7a24.12 24.12 0 0 1 0 10 2 2 0 0 1-1.4 1.4 49.55 49.55 0 0 1-16.2 0A2 2 0 0 1 2.5 17' />
                                            <path d='m10 15 5-3-5-3z' />
                                        </svg>
                                    </a>
                                    <a
                                        href='https://www.facebook.com/trashmob.eco/'
                                        target='_blank'
                                        rel='noreferrer noopener'
                                        className={socialClassName}
                                        aria-label='Facebook'
                                    >
                                        <svg
                                            className='h-5 w-5 text-black'
                                            viewBox='0 0 24 24'
                                            fill='none'
                                            stroke='currentColor'
                                            strokeWidth='2'
                                            strokeLinecap='round'
                                            strokeLinejoin='round'
                                        >
                                            <path d='M18 2h-3a5 5 0 0 0-5 5v3H7v4h3v8h4v-8h3l1-4h-4V7a1 1 0 0 1 1-1h3z' />
                                        </svg>
                                    </a>
                                    <a
                                        href='https://twitter.com/TrashMobEco'
                                        target='_blank'
                                        rel='noreferrer noopener'
                                        className={socialClassName}
                                        aria-label='Twitter'
                                    >
                                        <svg
                                            className='h-5 w-5 text-black'
                                            viewBox='0 0 24 24'
                                            fill='none'
                                            stroke='currentColor'
                                            strokeWidth='2'
                                            strokeLinecap='round'
                                            strokeLinejoin='round'
                                        >
                                            <path d='M22 4s-.7 2.1-2 3.4c1.6 10-9.4 17.3-18 11.6 2.2.1 4.4-.6 6-2C3 15.5.5 9.6 3 5c2.2 2.6 5.6 4.1 9 4-.9-4.2 4-6.6 7-3.8 1.1 0 3-1.2 3-1.2z' />
                                        </svg>
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </footer>
    );
};
