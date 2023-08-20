import { FC } from 'react'
import { Col, Container, Image, Row } from 'react-bootstrap';
import globes from '../assets/gettingStarted/globes.png';

interface HeroSectionProps {
    Title: string;
    Description: string;
};

export const HeroSection: FC<HeroSectionProps> = ({ Title, Description }) => {
    return (
        <Container fluid className='bg-grass'>
            <Row className="text-center pt-0">
                <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                    <h1 className="font-weight-bold">Title</h1>
                    <p className="font-weight-bold">Description</p>
                </Col>
                <Col md={5}>
                    <Image src={globes} alt="globes" className="h-100 mt-0" />
                </Col>
            </Row>
        </Container>
    )
}