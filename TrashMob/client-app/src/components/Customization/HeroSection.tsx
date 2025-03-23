import { FC } from 'react';
import globes from '@/components/assets/gettingStarted/globes.png';

interface HeroSectionProps {
    Title: string;
    Description: string;
}

export const HeroSection: FC<HeroSectionProps> = ({ Title, Description }) => (
    <div className='tailwind'>
        <div className='bg-primary text-primary-foreground'>
            <div className='container !px-0 md:!px-8 relative'>
                <img
                    src={globes}
                    alt='globes'
                    className='absolute right-0 md:right-8 top-0 bottom-0 h-full mt-0 object-cover opacity-30 md:opacity-100'
                />
                <div className='flex flex-row items-stretch relative'>
                    <div className='flex flex-col justify-center flex-1 !pr-12 !py-3 xl:!py-8 relative z-10 text-center md:!text-left'>
                        <h1 className='font-bold text-[40px] leading-[50px] mt-10 mb-2'>{Title}</h1>
                        <p className='font-bold my-4'>{Description}</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
);
