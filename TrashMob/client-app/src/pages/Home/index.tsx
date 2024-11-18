import UserData from '@/components/Models/UserData';
import { StatsSection } from './StatsSection';
import { EventSection } from './EventSection';
import { GettingStartSection } from './GettingStartSection';
import { HeroSection } from './HeroSection';
import { WhatIsTrashmobSection } from './WhatIsTrashmobSection';

interface HomeProps {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const Home = (props: HomeProps) => {
    return (
        <div className='tailwind'>
            <HeroSection />
            <WhatIsTrashmobSection />
            <StatsSection />
            <EventSection currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            <GettingStartSection />
        </div>
    );
};
