import { StatsSection } from './stats-section';
import { EventSection } from './event-section';
import { GettingStartSection } from './getting-start-section';
import { HeroSection } from './hero-section';
import { WhatIsTrashmobSection } from './whatistrashmob-section';

interface HomeProps {}

export const Home = (props: HomeProps) => {
    return (
        <div className='tailwind'>
            <HeroSection />
            <WhatIsTrashmobSection />
            <StatsSection />
            <EventSection />
            <GettingStartSection />
        </div>
    );
};
