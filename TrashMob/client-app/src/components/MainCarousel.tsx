import * as React from 'react'
// import "react-responsive-carousel/lib/styles/carousel.min.css"; // requires a loader
import litter from "./assets/litter.jpg";
import beachlitter from "./assets/brian-yurasits-nwS3b_s-IRs-unsplash.jpg";
import styrofoam from "./assets/brian-yurasits-0G2jF-c704s-unsplash.jpg"
import woman from "./assets/paige-cody-G8VOA-BrnHo-unsplash.jpg"

import { Carousel } from 'react-bootstrap';

export const MainCarousel: React.FC = () => {
    return (
        <Carousel className="carousel slide carousel-fade">
            <Carousel.Item className="carousel-inner" style={{ backgroundImage: 'url(' + woman + ')' }}>
                <Carousel.Caption className="carousel-caption d-none d-md-block">
                    <h5>It's easy to get started</h5>
                    <p>Get out and start with your neighborhood and expand from there. Every bit matters!</p>
                </Carousel.Caption>
            </Carousel.Item>
            <Carousel.Item className="carousel-inner" style={{backgroundImage: 'url(' + styrofoam + ')'}}  >
                <Carousel.Caption className="carousel-caption d-none d-md-block">
                    <h5>TrashMob.eco</h5>
                    <p>Cleaning up Planet Earth, one bucket of trash at a time.</p>
                </Carousel.Caption>
            </Carousel.Item>
            <Carousel.Item className="carousel-inner" style={{backgroundImage: 'url(' + litter + ')'}} >
                <Carousel.Caption className="carousel-caption d-none d-md-block">
                    <h5>One hour at a local park</h5>
                    <p>It's amazing how much is out there, even in "clean" parks. Get started today!</p>
                </Carousel.Caption>
            </Carousel.Item>
            <Carousel.Item className="carousel-inner" style={{backgroundImage: 'url(' + beachlitter + ')'}} >
                <Carousel.Caption className="carousel-caption d-none d-md-block">
                    <h5>Protecting the oceans starts in your own neighborhood</h5>
                    <p>Every piece of litter picked up before it hits the watershed solves a problem before it starts</p>
                </Carousel.Caption>
            </Carousel.Item>
        </Carousel>
    );
}

