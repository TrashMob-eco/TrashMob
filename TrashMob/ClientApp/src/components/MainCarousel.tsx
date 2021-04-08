import { Component } from 'react';
import * as React from 'react'

import "react-responsive-carousel/lib/styles/carousel.min.css"; // requires a loader
import { Carousel } from 'react-responsive-carousel';
import sign from "./assets/sign.jpg";
import litter from "./assets/litter.jpg";
import streetlitter from "./assets/streetlitter.jpg";
import beachlitter from "./assets/beachlitter.jpg";
import trash1000miles from "./assets/trash1000miles.jpg";
import stoplittering from "./assets/stoplittering.jpg";

export class MainCarousel extends Component {
    render() {
        return (
            <Carousel>
                <div>
                    <img src={sign} alt="Litter pickup ahead" />
                    <p className="legend">Trash Mob At Work</p>
                </div>
                <div>
                    <img src={litter} alt="Litter"/>
                    <p className="legend">It's Everywhere</p>
                </div>
                <div>
                    <img src={streetlitter} alt="Litter on a Street" />
                    <p className="legend">Street Litter</p>
                </div>
                <div>
                    <img src={beachlitter} alt="Litter on a beach" />
                    <p className="legend">Beach Litter</p>
                </div>
                <div>
                    <img src={trash1000miles} alt="Trash Next 1000 Miles" />
                    <p className="legend">Beach Litter</p>
                </div>
                <div>
                    <img src={stoplittering} alt="Stop Littering" />
                    <p className="legend">Stop Littering</p>
                </div>
            </Carousel>
        );
    }
}
