import * as React from 'react'

import "react-responsive-carousel/lib/styles/carousel.min.css"; // requires a loader
import sign from "./assets/sign.jpg";
import litter from "./assets/litter.jpg";
import beachlitter from "./assets/beachlitter.jpg";
import trash1000miles from "./assets/trash1000miles.jpg";
import stoplittering from "./assets/stoplittering.jpg";

export const MainCarousel: React.FC = () => {
    return (
        <div>
            <div id="carouselExampleCaptions" className="carousel slide" data-ride="carousel">
                <ol className="carousel-indicators">
                    <li data-target="#carouselExampleCaptions" data-slide-to="0" className="active"></li>
                    <li data-target="#carouselExampleCaptions" data-slide-to="1"></li>
                    <li data-target="#carouselExampleCaptions" data-slide-to="2"></li>
                </ol>
                <div className="carousel-inner">
                    <div className="carousel-item active" style={{
                        backgroundImage: 'url(https://www.salesforce.org/wp-content/uploads/2019/06/Clean-up.jpg)'
                    }}
                    >
                        <div className="carousel-caption d-none d-md-block">
                            <h5>Using the power of the Flash Mob to clean up the planet</h5>
                            <a href="" className="btn btn-primary">Join today</a>
                        </div>
                    </div>
                    <div className="carousel-item" style={{
                        backgroundImage: 'url(' + litter + ')'
                    }}
                    >
                        <div className="carousel-caption d-none d-md-block">
                            <h5>It's Everywhere</h5>
                            <p>Some representative placeholder content for the second slide.</p>
                        </div>
                    </div>
                    <div className="carousel-item" style={{
                        backgroundImage: 'url(' + beachlitter + ')'
                    }}
                    >
                        <div className="carousel-caption d-none d-md-block">
                            <h5>Beach Litter</h5>
                            <p>Some representative placeholder content for the third slide.</p>
                        </div>
                    </div>
                </div>
                <a className="carousel-control-prev" href="#carouselExampleCaptions" role="button" data-slide="prev">
                    <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                    <span className="sr-only">Previous</span>
                </a>
                <a className="carousel-control-next" href="#carouselExampleCaptions" role="button" data-slide="next">
                    <span className="carousel-control-next-icon" aria-hidden="true"></span>
                    <span className="sr-only">Next</span>
                </a>
            </div>
        </div>
    );
}

