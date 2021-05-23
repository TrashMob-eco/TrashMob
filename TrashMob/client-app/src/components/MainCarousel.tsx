import * as React from 'react'
// import "react-responsive-carousel/lib/styles/carousel.min.css"; // requires a loader
import litter from "./assets/litter.jpg";
import beachlitter from "./assets/beachlitter.jpg";
import styrofoam from "./assets/brian-yurasits-0G2jF-c704s-unsplash.jpg"
import man from "./assets/brian-yurasits-euyUAlyZxTo-unsplash.jpg"
import { Carousel } from 'react-bootstrap';

export const MainCarousel: React.FC = () => {
    return (
        <Carousel className="carousel slide carousel-fade">
            <Carousel.Item className="carousel-inner">
                <img className="carousel-item active" src={styrofoam} alt="Flash mobs" />
                <Carousel.Caption className="carousel-caption d-none d-md-block">
                    <h5>Using the power of the flash mob to clean up the planet</h5>
                </Carousel.Caption>
            </Carousel.Item>
            <Carousel.Item className="carousel-inner">
                <img className="carousel-item active" src={litter} alt="Litter" />
                <Carousel.Caption className="carousel-caption d-none d-md-block">
                    <h5>An hour at a local park</h5>
                    <p>It's amazing how much is out there, even in "clean" parks. Get started today!</p>
                </Carousel.Caption>
            </Carousel.Item>
            <Carousel.Item className="carousel-inner">
                <img className="carousel-item active" src={beachlitter} alt="Beach Litter" />
                <Carousel.Caption className="carousel-caption d-none d-md-block">
                    <h5>Beach Litter</h5>
                    <p>Found next to one of the key salmon spawning streams of the Pacific Northwest. The problem starts close to home.</p>
                </Carousel.Caption>
            </Carousel.Item>
            <Carousel.Item className="carousel-inner">
                <img className="carousel-item active" src={man} alt="Man picking trash" />
                <Carousel.Caption className="carousel-caption d-none d-md-block">
                    <h5>It's easy to get started</h5>
                    <p>Get out and start with you neighborhood and expand from there. Every bit matters!</p>
                </Carousel.Caption>
            </Carousel.Item>
        </Carousel>
    );
}

