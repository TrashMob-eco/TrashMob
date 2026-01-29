import { StatsSection } from './stats-section';
import { EventSection } from './event-section';
import { GettingStartSection } from './getting-start-section';
import { HeroSection } from './hero-section';
import { WhatIsTrashmobSection } from './whatistrashmob-section';

export const Home = () => {
    return (
        <>
            <HeroSection />
            <WhatIsTrashmobSection />
            <StatsSection />
            <EventSection />
            <GettingStartSection />
        </>
    );
};
