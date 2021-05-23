import * as React from 'react'
import "react-responsive-carousel/lib/styles/carousel.min.css"; // requires a loader
import litter from "./assets/litter.jpg";
import beachlitter from "./assets/beachlitter.jpg";
import styrofoam from "./assets/brian-yurasits-0G2jF-c704s-unsplash.jpg"
import man from "./assets/brian-yurasits-euyUAlyZxTo-unsplash.jpg"

export const MainCarousel: React.FC = () => {
    return (
        <div>
            <div id="carouselExampleCaptions" className="carousel slide carousel-fade" data-ride="carousel">
                <ol className="carousel-indicators">
                    <li data-target="#carouselExampleCaptions" data-slide-to="0" className="active"></li>
                    <li data-target="#carouselExampleCaptions" data-slide-to="1"></li>
                    <li data-target="#carouselExampleCaptions" data-slide-to="2"></li>
                    <li data-target="#carouselExampleCaptions" data-slide-to="3"></li>
                </ol>
                <div className="carousel-inner">
                    <div className="carousel-item active" style={{
                        backgroundImage: 'url(' + styrofoam + ')'
                    }}
                    >
                        <div className="carousel-caption d-none d-md-block">
                            <h5>Using the power of the flash mob to clean up the planet</h5>                            
                        </div>
                    </div>
                    <div className="carousel-item" style={{
                        backgroundImage: 'url(' + litter + ')'
                    }}
                    >
                        <div className="carousel-caption d-none d-md-block">
                            <h5>An hour at a local park</h5>
                            <p>It's amazing how much is out there, even in "clean" parks. Get started today!</p>
                        </div>
                    </div>
                    <div className="carousel-item" style={{
                        backgroundImage: 'url(' + beachlitter + ')'
                    }}
                    >
                        <div className="carousel-caption d-none d-md-block">
                            <h5>Beach Litter</h5>
                            <p>Found next to one of the key salmon spawning streams of the Pacific Northwest. The problem starts close to home.</p>
                        </div>
                    </div>
                    <div className="carousel-item" style={{
                        backgroundImage: 'url(' + man + ')'
                    }}
                    >
                        <div className="carousel-caption d-none d-md-block">
                            <h5>It's easy to get started</h5>
                            <p>Get out and start with you neighborhood and expand from there. Every bit matters!</p>
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

