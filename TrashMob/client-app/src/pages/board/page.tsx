import * as React from 'react';
import linkedIn from '@/components/assets/card/linkedin.svg';
import JoeBeernink from '@/components/assets/boardOfDirectors/JoeBeernink.jpg';
import DarylBarber from '@/components/assets/boardOfDirectors/darylbarber.jpg';
import JacquelineHayot from '@/components/assets/boardOfDirectors/JacquelineHayot.jpg';
import CynthiaMitchell from '@/components/assets/boardOfDirectors/CynthiaMitchell.jpg';
import { HeroSection } from '@/components/Customization/HeroSection';

const boards = [
    {
        name: 'Cynthia Mitchell',
        img: CynthiaMitchell,
        linkedin: 'https://www.linkedin.com/in/cynthia-mitchell/',
        position: 'Board President',
        detail: [
            'Cynthia Mitchell is a serial tech and media entrepreneur, C-Suite advisor to startups and board member. Mitchell has worked for more than 50 different companies across an array of industries including technology, environmental, energy, construction, health, media, fashion and entertainment. Brands include American Broadcasting Company, Time Warner, Meredith Corporation, Maclean Hunter, Times Mirror, Kaiser Permanente, Mutual of Omaha, and The Summer Olympic Games among others. As a strategist, she has created programs for leading global brands such as Toyota, Nissan, Honda, Coors, Nike, Rolex, and American Express. Over the past several years, she has worked with the Government of India in partnership with The Netherlands to develop biomass plants that recycle agricultural waste into green energy and biofertilizers. She has also led private enterprise initiatives to develop biomass innovation for construction, packaging, displays and other applications.',
            'Mitchell lives in Southern California. She is the mother of two daughters – both also entrepreneurs. She is a lifelong equestrian and breeder, a passionate gardener and believes in the collective power of personal service and stewardship to make the world a better, healthier place for all its inhabitants.',
        ],
    },
    {
        name: 'Daryl Barber',
        img: DarylBarber,
        linkedin: 'https://www.linkedin.com/in/daryl-r-barber-9abb8123/',
        position: 'Board Treasurer',
        detail: [
            'Daryl R. Barber is a Finance Professional with extensive experience at highly complex public and private-equity owned companies within a broad range of businesses, including industrials, chemicals, and software technology and services. Daryl specializes in finance strategies, included in treasury, investor relations, M&A, financial planning & analysis, audit, and corporate and business controllership. Daryl provides consulting services, currently acting as interim Chief Financial Officer and interim Controller and Treasurer for two technology driven companies.',
            'In addition to this finance background, Daryl has experience with several 501(c)(3) organizations, all of which provide assistance to the needy and disadvantaged through education, health, and other human services.',
            'Having completed his undergraduate studies at the University of Hartford and his graduate studies at Fairleigh Dickinson University, Daryl now resides, with his wife, three children, and a beagle, in Malvern, Pennsylvania.',
        ],
    },
    {
        name: 'Jacqueline Hayot',
        img: JacquelineHayot,
        linkedin: 'https://www.linkedin.com/in/jacqueline-hayot-ba-1aa29b1/',
        position: 'Board Secretary',
        detail: [
            'Jacqueline has over 25 years of experience in capital markets and financial product marketing. In the last several years, she has leveraged her experience and skills to raise capital for investment funds and founders, namely in the climate tech space. She is an angel investor in several climate start-ups and funds. Most recently, she led business development at Mercator Partners, a climate-focused investment fund in Princeton, NJ. She has held COO and Business Development roles at several funds including AltraVue Capital in Bellevue, WA. She began her post-MBA career in debt and equity capital markets at Morgan Stanley and led equity derivatives marketing at BNP Paribas. She earned her MBA from the Wharton School at the University of Pennsylvania, an MA from the School of Advanced International Studies (SAIS) at Johns Hopkins University, and her BA from Dartmouth College.',
        ],
    },
    {
        name: 'Joe Beernink',
        img: JoeBeernink,
        linkedin: 'https://www.linkedin.com/in/joebeernink/',
        position: 'Founder, Director of Product and Engineering',
        detail: [
            'Joe Beernink is a software developer with over 25 years of industry experience developing mission-critical software.',
            'Joe grew up on a small farm in Southern Ontario, Canada, working and playing in the great outdoors, graduated with a degree in Space Science from York University in Toronto in 1994, and moved to the US in 1996. He previously lived in Michigan and Colorado before making Washington State his home in 1999.',
            'In 2021, Joe was inspired by Edgar McGregor, a climate activist in California, to get out and start cleaning up his community. After seeing just how much work needed to be done, Joe envisioned a website that enabled like-minded people to get out and start cleaning the environment together, and the idea for TrashMob.eco was born.',
            'Joe resides in Issaquah, WA with his 2 kids.',
        ],
    },
];

export const Board: React.FC = () => {
    return (
        <div className='tailwind'>
            <HeroSection Title='Board of Directors' Description='Meet our team!' />
            <div className='container my-5 pb-5'>
                {boards.map((board) => (
                    <div className='p-4 mt-24 bg-card shadow-lg rounded-lg!'>
                        <div className='flex flex-col md:flex-row gap-8'>
                            <div className='basis-full md:basis-6/12 lg:basis-5/12'>
                                <img
                                    src={board.img}
                                    alt={board.name}
                                    className='mx-auto md:mx-0 mt-0 h-100 object-cover rounded-md!'
                                />
                            </div>
                            <div className='basis-full md:basis-6/12 lg:basis-7/12'>
                                <div className='flex justify-between items-center'>
                                    <h2 className='m-0 font-medium text-3xl color-primary'>{board.name}</h2>
                                    <a href={board.linkedin}>
                                        <img src={linkedIn} alt='linkedin icon' className='mt-0 object-cover' />
                                    </a>
                                </div>
                                <h5 className='my-3 font-medium text-[#696b72]'>{board.position}</h5>
                                {board.detail.map((paragraph, i) => (
                                    <p className='text-sm font-normal text-[#696b72]' key={i}>
                                        {paragraph}
                                    </p>
                                ))}
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};
