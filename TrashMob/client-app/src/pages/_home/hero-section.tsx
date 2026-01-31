import { Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { Button } from '@/components/ui/button';
import { Logo } from '@/components/Logo';
import { useIsInViewport } from '@/hooks/useIsInViewport';
import { cn } from '@/lib/utils';
import { GetHeroSection, HeroSectionData } from '@/services/cms';
import { Services } from '@/config/services.config';

// Default fallback content (current hardcoded values)
const defaultHeroContent: HeroSectionData = {
    primaryButtonText: 'Join us today',
    primaryButtonLink: '/gettingstarted',
    secondaryButtonText: 'Report Litter',
    secondaryButtonLink: '/litterreports/create',
    googlePlayUrl: 'https://play.google.com/store/apps/details?id=eco.trashmob.trashmobmobileapp',
    appStoreUrl:
        'https://apps.apple.com/us/app/trashmob/id1599996743?itscg=30200&itsct=apps_box_badge&mttnsubad=1599996743',
};

export const HeroSection = () => {
    const { ref: viewportRef, isInViewPort } = useIsInViewport();

    const { data: cmsContent } = useQuery({
        queryKey: GetHeroSection().key,
        queryFn: GetHeroSection().service,
        staleTime: Services.CACHE.FOR_ONE_MINUTE * 5,
    });

    // Merge CMS content with defaults (CMS overrides defaults when available)
    const content = { ...defaultHeroContent, ...cmsContent };

    return (
        <section id='hero-section' ref={viewportRef} className='relative'>
            <div
                className={cn(
                    'absolute w-full h-full left-0 top-0 z-0',
                    "lg:bg-[url('/img/globe2-minified.png')] bg-no-repeat bg-contain",
                    'opacity-0 bg-position-[160%_0%]',
                    'transition-all duration-1000 delay-100 ease-out',
                    {
                        'opacity-100 lg:bg-position-[130%_0%]! xl:bg-position-[105%_0%]!': isInViewPort,
                    },
                )}
            />
            <div className='relative z-1 container px-8 py-32 lg:min-h-[50vh] lg:max-h-[600px]'>
                <div className='w-[600px] max-w-full flex flex-col items-center md:items-start'>
                    <div className='w-full my-16'>
                        <Logo
                            className={cn('w-[600px] max-w-full', 'transition-all duration-1000 ease-out delay-700', {
                                'opacity-100 translate-y-0': isInViewPort,
                                'opacity-0 translate-y-10': !isInViewPort,
                            })}
                            showTagline
                        />
                    </div>
                    <div
                        className={cn(
                            'flex flex-col sm:flex-row gap-3 transition-all duration-1000 ease-out delay-1000',
                            {
                                'opacity-100 translate-y-0': isInViewPort,
                                'opacity-0 translate-y-10': !isInViewPort,
                            },
                        )}
                    >
                        <Button size='lg' asChild>
                            <Link to={content.primaryButtonLink}>{content.primaryButtonText}</Link>
                        </Button>
                        {content.secondaryButtonText && content.secondaryButtonLink ? (
                            <Button size='lg' variant='outline' asChild>
                                <Link to={content.secondaryButtonLink}>{content.secondaryButtonText}</Link>
                            </Button>
                        ) : null}
                    </div>
                    <div
                        className={cn(
                            'flex flex-row gap-1 items-center mt-16',
                            'transition-all duration-700 ease-out delay-1000',
                            {
                                'opacity-100 translate-y-0': isInViewPort,
                                'opacity-0 translate-y-10': !isInViewPort,
                            },
                        )}
                    >
                        {content.googlePlayUrl ? (
                            <a href={content.googlePlayUrl}>
                                <img
                                    className='android mt-0 -ml-2 h-14'
                                    alt='Get it on Google Play'
                                    src='https://play.google.com/intl/en_us/badges/images/generic/en_badge_web_generic.png'
                                />
                            </a>
                        ) : null}
                        {content.appStoreUrl ? (
                            <a href={content.appStoreUrl}>
                                <img
                                    src='https://toolbox.marketingtools.apple.com/api/v2/badges/download-on-the-app-store/black/en-us?releaseDate=1682899200'
                                    alt='Download on the App Store'
                                    className='m-0 h-10'
                                />
                            </a>
                        ) : null}
                    </div>
                </div>
            </div>
        </section>
    );
};
