import { FC } from 'react';
import { Col, Container, Image, Row } from 'react-bootstrap';
import globes from '../assets/gettingStarted/globes.png';

interface HeroSectionProps {
    Title: string;
    Description: string;
}

export const HeroSection: FC<HeroSectionProps> = ({ Title, Description }) => (
    <Container fluid className='bg-grass'>
        <div className='container mx-auto'>
            <Row className='text-left pt-0'>
                <Col className='d-flex flex-column justify-content-center pr-5'>
                    <h1 className='font-weight-bold'>{Title}</h1>
                    <p className='font-weight-bold'>{Description}</p>
                </Col>
                <Col>
                    <Image src={globes} alt='globes' className='h-100 mt-0' />
                </Col>
            </Row>
        </div>
    </Container>
);
