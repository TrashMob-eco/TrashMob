import { Button } from '@/components/ui/button';
import { Logo } from '@/components/Logo';

export const HeroSection = () => {
    return (
        <section
            id='hero-section'
            className="lg:bg-[url('/img/globe2-minified.png')] lg:bg-[position:130%_0%] xl:bg-[position:105%_0%] bg-no-repeat bg-contain"
        >
            <div className='container px-8 py-32 lg:min-h-[50vh] lg:max-h-[600px]'>
                <div className='w-[600px] max-w-full flex flex-col items-center md:items-start'>
                    <div className='w-full my-16'>
                        <Logo className='w-[600px] max-w-full' showTagline />
                    </div>
                    <Button size='lg'>Join us today</Button>
                    <div className='flex flex-row gap-1 items-center mt-16'>
                        <a href='https://play.google.com/store/apps/details?id=eco.trashmob.trashmobmobileapp'>
                            <img
                                className='android mt-0 -ml-2 h-14'
                                alt='Get it on Google Play'
                                src='https://play.google.com/intl/en_us/badges/images/generic/en_badge_web_generic.png'
                            />
                        </a>

                        <a href='https://apps.apple.com/us/app/trashmob/id1599996743?itscg=30200&itsct=apps_box_badge&mttnsubad=1599996743'>
                            <img
                                src='https://toolbox.marketingtools.apple.com/api/v2/badges/download-on-the-app-store/black/en-us?releaseDate=1682899200'
                                alt='Download on the App Store'
                                className='m-0 h-10'
                            />
                        </a>
                    </div>
                </div>
            </div>
        </section>
    );
};
