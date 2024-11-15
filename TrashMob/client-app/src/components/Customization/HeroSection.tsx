import { FC } from 'react';
import { Col, Image, Row } from 'react-bootstrap';
import globes from '../assets/gettingStarted/globes.png';

interface HeroSectionProps {
    Title: string;
    Description: string;
}

export const HeroSection: FC<HeroSectionProps> = ({ Title, Description }) => (
    <div className='bg-grass '>
        <div className='container-lg mx-auto'>
            <Row className='text-center mx-auto text-md-left pt-0 position-relative'>
                <Col className='d-flex flex-column justify-content-center pr-sm-5 m-auto' style={{ zIndex: 5 }}>
                    <h1 className='font-weight-bold'>{Title}</h1>
                    <p className='font-weight-bold'>{Description}</p>
                </Col>
                <div className='d-md-none position-absolute h-100 w-100' style={{ zIndex: '1', opacity: '30%' }}>
                    <Image src={globes} alt='globes' className='h-100 w-100 object-fit-cover mt-0' />
                </div>
                <Col className='d-none d-md-block'>
                    <Image src={globes} alt='globes' className='h-100 mt-0' />
                </Col>
            </Row>
        </div>
    </div>
);
