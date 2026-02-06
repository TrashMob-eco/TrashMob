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
        { href: '/faq', title: 'FAQ' },
        { href: '/volunteeropportunities', title: 'Recruiting' },
        { href: '/contactus', title: 'Contact us' },
        { href: '/privacypolicy', title: 'Privacy policy' },
        { href: '/termsofservice', title: 'Terms of service' },
        { href: '/waivers', title: 'Waivers' },
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
                    <div className='grid grid-rows-9 grid-cols-1 md:grid-rows-3 md:grid-cols-3 gap-4 gap-x-8 md:grid-flow-col'>
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
                                    >
                                        <i className='mt-0! fa-brands fa-instagram text-center text-black' />
                                    </a>
                                    <a
                                        href='https://www.youtube.com/channel/UC2LgFmXFCA8kdkxd4IJ51BA'
                                        target='_blank'
                                        rel='noreferrer noopener'
                                        className={socialClassName}
                                    >
                                        <i className='mt-0! fa-brands fa-youtube text-center text-black' />
                                    </a>
                                    <a
                                        href='https://www.facebook.com/trashmob.eco/'
                                        target='_blank'
                                        rel='noreferrer noopener'
                                        className={socialClassName}
                                    >
                                        <i className='mt-0! fab fa-facebook-f text-center text-black' />
                                    </a>
                                    <a
                                        href='https://twitter.com/TrashMobEco'
                                        target='_blank'
                                        rel='noreferrer noopener'
                                        className={socialClassName}
                                    >
                                        <i className='mt-0! fab fa-twitter text-center text-black' />
                                    </a>
                                    {/* <a
                                        href='https://profiles.eco/trashmob?ref=tm'
                                        target='_blank'
                                        rel='noreferrer noopener'
                                        className={socialClassName}
                                    >
                                        <img
                                            className='eco-trustmark'
                                            alt='.eco profile for trashmob.eco'
                                            src='https://trust.profiles.eco/trashmob/eco-button.svg?color=%23000000'
                                        />
                                    </a> 
                                    <a
                                        href='https://www.linkedin.com/company/76188984'
                                        target='_blank'
                                        rel='noreferrer noopener'
                                        className={socialClassName}
                                    >
                                        <i className='mt-0! fa-brands fa-linkedin' />
                                    </a> */}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </footer>
    );
};
