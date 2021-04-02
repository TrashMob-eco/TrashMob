import React from 'react';
import "react-responsive-carousel/lib/styles/carousel.min.css"; // requires a loader
import { Carousel } from 'react-responsive-carousel';

export class MainCarousel extends React.Component {
    render() {
        return (
            <Carousel>
                <div>
                    <img src="assets/sign.jpeg" />
                    <p className="legend">Trash Mob At Work</p>
                </div>
                <div>
                    <img src="assets/litter.jpeg" />
                    <p className="legend">It's Everywhere</p>
                </div>
                <div>
                    <img src="assets/streetlitter.jpeg" />
                    <p className="legend">Street Litter</p>
                </div>
                <div>
                    <img src="assets/beachlitter.jpeg" />
                    <p className="legend">Beach Litter</p>
                </div>
            </Carousel>
        );
    }
}
