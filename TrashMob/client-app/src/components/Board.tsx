import * as React from 'react';
import linkedIn from './assets/card/linkedin.svg';
import JoeBeernink from './assets/boardOfDirectors/JoeBeernink.jpg';
import DarylBarber from './assets/boardOfDirectors/darylbarber.jpg';
import JacquelineHayot from './assets/boardOfDirectors/JacquelineHayot.jpg';
import JakeDiliberto from './assets/boardOfDirectors/JakeDiliberto.jpg';
import KevinGleason from './assets/boardOfDirectors/KevinGleason.jpg';
import SandraMau from './assets/boardOfDirectors/SandraMau.jpg';
import CynthiaMitchell from './assets/boardOfDirectors/CynthiaMitchell.jpg';
import ValerieWilden from './assets/boardOfDirectors/ValerieWilden.jpg';
import { HeroSection } from './Customization/HeroSection';

const boards = [
    {
        name: 'Joe Beernink',
        img: JoeBeernink,
        linkedin: 'https://www.linkedin.com/in/joebeernink/',
        position: 'Board President',
        detail: [
            'Joe Beernink is a software developer with over 25 years of industry experience developing mission-critical software.',
            'Joe grew up on a small farm in Southern Ontario, Canada, working and playing in the great outdoors, graduated with a degree in Space Science from York University in Toronto in 1994, and moved to the US in 1996. He previously lived in Michigan and Colorado before making Washington State his home in 1999.',
            'In 2021, Joe was inspired by Edgar McGregor, a climate activist in California, to get out and start cleaning up his community. After seeing just how much work needed to be done, Joe envisioned a website that enabled like-minded people to get out and start cleaning the environment together, and the idea for TrashMob.eco was born.',
            'Joe resides in Issaquah, WA with his 2 kids.',
        ],
    },
    {
        name: 'Cynthia Mitchell',
        img: CynthiaMitchell,
        linkedin: 'https://www.linkedin.com/in/cynthia-mitchell/',
        position: 'Board Vice-President',
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
        name: 'Valerie Wilden',
        img: ValerieWilden,
        linkedin: 'https://www.linkedin.com/in/valerie-day-wilden-283a13b5/',
        position: 'Board Secretary',
        detail: [
            'Valerie Wilden is principal consultant for Vivid Communication, the public relations and marketing agency she founded after 35+ years of media and pr, operations, government affairs, crisis management and fundraising for Pennsylvania’s largest nonprofit healthcare organization of its kind. Its uniquely diverse nature also required Mrs. Wilden to spearhead growth-related communication, planning and volunteer relations for entities that supported its core medical and charitable mission: a performing arts center, a multitude of outreach programs, special events including renowned VIPS, an auto-repair service, resale shops and the award winning five-state PRESENTS FOR PATIENTS® program, of which Valerie was a television spokesperson.',
            'Now at Vivid Communication, she consults with charity and for-profit organizations by writing marketing plans, boosting social media, creating promotions and guiding efforts toward highest net revenue potential. She is a three-term trustee of Westminster College, where she earned her Bachelor of Arts in English. Upon graduating with a Master of Arts in Journalism and Mass Communication from Point Park University, she taught corporate writing there.',
            'She and her husband, Greg, live in Wexford, a northern suburb of Pittsburgh, Pennsylvania and are parents of Alyssa, Scott, and Dayne.',
        ],
    },
    {
        name: 'Jake Diliberto',
        img: JakeDiliberto,
        linkedin: 'https://www.linkedin.com/in/jakediliberto/',
        position: 'Member at large',
        detail: [
            'Jake Diliberto is a senior operations professional having managed a (NYSE) fortune 500 portfolio. He has proven expertise in transformational management with comprehensive experience managing multiple operating units, driving change management, project management, process reengineering, resource optimization, and systems implementations.',
            'Jake also has veteran/military front-line field leadership expertise, having directed and motivated teams in high pressure settings with real-time consequences.',
        ],
    },
    {
        name: 'Kevin Gleason',
        img: KevinGleason,
        linkedin: 'https://www.linkedin.com/in/kevin-gleason-78a9236/',
        position: 'Member at large',
        detail: [
            'Kevin Gleason currently is Vice President New York Life and Chief Compliance Officer at MainStay Funds and Index IQ ETFs. He is a seasoned legal and compliance professional with over 25 years of experience working for 5 Fortune Five Hundred diversified financial services organizations.',
            'Mr. Gleason has advised and transacted business globally on 5 continents including across Europe, the Middle East, Asia, and South America. He has counseled C-suite executives and boards of directors on the creation of compliance and ethics programs; the development of controls, training, testing, conflicts identification, and risk assessments; and the structuring of governance frameworks.',
            'Mr. Gleason has a law degree and a masters in financial services law. He has earned an MBA from The University of Chicago and BA from University of Notre Dame. He is or has been a board member at Arizona Science Center, National Society of Compliance Professionals, and Journal of Financial Compliance. He is a frequent speaker at and contributor to industry events and publications.',
        ],
    },
    {
        name: 'Jacqueline Hayot',
        img: JacquelineHayot,
        linkedin: 'https://www.linkedin.com/in/jacqueline-hayot-ba-1aa29b1/',
        position: 'Member at large',
        detail: [
            'Jacqueline has over 25 years of experience in capital markets and financial product marketing. In the last several years, she has leveraged her experience and skills to raise capital for investment funds and founders, namely in the climate tech space. She is an angel investor in several climate start-ups and funds. Most recently, she led business development at Mercator Partners, a climate-focused investment fund in Princeton, NJ. She has held COO and Business Development roles at several funds including AltraVue Capital in Bellevue, WA. She began her post-MBA career in debt and equity capital markets at Morgan Stanley and led equity derivatives marketing at BNP Paribas. She earned her MBA from the Wharton School at the University of Pennsylvania, an MA from the School of Advanced International Studies (SAIS) at Johns Hopkins University, and her BA from Dartmouth College.',
        ],
    },
    {
        name: 'Sandra Mau',
        img: SandraMau,
        linkedin: 'https://www.linkedin.com/in/sandramau/',
        position: '',
        detail: [
            'Sandra is VP of Product for Cloud Solutions at Clarivate (NYSE:CLVT). Prior to joining Clarivate via acquisition, she was the CEO and Founder of TrademarkVision, an award-winning AI/Computer Vision startup doing visual brand protection.',
            "Sandra is very active in supporting tech and startup communities. She was the Founding Chair of IEEE QLD Women in Engineering, and listed as one of Australia's Top 50 Female Programmers by Pollenizer 2014, and one of Australia's Top 100 Most Influential Engineers by Engineer's Australia 2015. She's also a regular participant in hackathons including past GovHacks and International Women's Day. She was recognised in 2018 by Pittsburgh Business Times with the Pittsburgh Innovator Award and by QUT with the Innovation and Entrepreneurship Outstanding Alumni Award.",
            'She holds a Masters in Robotics from Carnegie Mellon University, a Bachelors in Engineering Science (Aerospace) from University of Toronto, and an MBA from Queensland University of Technology (QUT).',
        ],
    },
];

export const Board: React.FC = () => {
    return (
        <div className='tailwind'>
            <HeroSection Title='Board of Directors' Description='Meet our team!' />
            <div className='container my-5 pb-5'>
                {boards.map((board) => (
                    <div className='p-4 mt-24 bg-card shadow-lg !rounded-lg'>
                        <div className='flex flex-col md:flex-row gap-8'>
                            <div className='basis-full md:basis-6/12 lg:basis-5/12'>
                                <img
                                    src={board.img}
                                    alt={board.name}
                                    className='mx-auto md:mx-0 mt-0 h-100 object-cover !rounded-md'
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
