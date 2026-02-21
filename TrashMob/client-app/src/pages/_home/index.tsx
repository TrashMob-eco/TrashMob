import { StatsSection } from './stats-section';
import { EventSection } from './event-section';
import { FeaturedCommunitiesSection } from './featured-communities-section';
import { GettingStartSection } from './getting-start-section';
import { HeroSection } from './hero-section';
import { WhatIsTrashmobSection } from './whatistrashmob-section';

export const Home = () => {
    return (
        <>
            <HeroSection />
            <WhatIsTrashmobSection />
            <StatsSection />
            <FeaturedCommunitiesSection />
            <EventSection />
            <GettingStartSection />
        </>
    );
};
