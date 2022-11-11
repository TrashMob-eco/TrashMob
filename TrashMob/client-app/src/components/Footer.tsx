import * as React from 'react'
import logo from './assets/logowhite.svg'
//import twitter from './assets/twitter.svg'
//import facebook from './assets/facebook.svg'
import { Link } from 'react-router-dom';

export const Footer: React.FC = () => {
    return (
        <footer id="pageFooter">
            <div className="container" id="footerWrapper">
                <div className="my-5">
                    <img src={logo} alt="TrashMob Logo" id="logo_footer" />
                    <div className="row">
                        <div className="col-md-4">
                            <Link to="/aboutus">About us</Link>
                            <Link to="/board">Board</Link>
                            <Link to="/partnerships">Partnerships</Link>
                            <Link to="/faq">FAQ</Link>
                        </div>

                        <div className="col-md-4">
                            <Link to="/volunteeropportunities">Volunteer opportunities</Link>
                            <Link to="/contactus">Contact us</Link>
                            <a href={"https://donate.stripe.com/14k9DN2EnfAog9O3cc"}>Donate</a>
                        </div>

                        <div className="col-md-4">
                            <Link to="/privacypolicy">Privacy policy</Link>
                            <Link to="/termsofservice">Terms of service</Link>
                            <Link to="/waivers">Waivers</Link>
                            <Link to="/eventsummaries">Event summaries</Link>
                        </div>
                    </div>
                    <hr className="horizontalLine mt-5" />
                    <div className="row" id="copyright">
                        <div className="col-md-8">
                            <p>Copyright &copy; 2022 TRASHMOB.ECO - All rights reserved.</p>
                        </div>
                        <div className="col-md-4">
                            <div className="row" id="iconsOrigWrapper">
                                <div className="d-flex justify-content-between">
                                    <div className="iconWrapper" id="firstWrapper">
                                        <a href={"https://www.facebook.com/trashmob.eco/"} target="_blank" rel="noreferrer noopener">
                                            <i className="fab fa-facebook-f"></i>
                                        </a>
                                    </div>
                                    <div className="iconWrapper">
                                        <a href={"https://twitter.com/TrashMobEco"} target="_blank" rel="noreferrer noopener">
                                            <i className="fab fa-twitter"></i>
                                        </a>
                                    </div>
                                    <div className="iconWrapper">
                                        <a href={"https://profiles.eco/trashmob?ref=tm"} target="_blank" rel="noreferrer noopener">
                                            <img className="eco-trustmark" alt=".eco profile for trashmob.eco" src="https://trust.profiles.eco/trashmob/eco-button.svg?color=%2396BA00" />
                                        </a>
                                    </div>
                                    <div className="iconWrapper">
                                        <a href={"https://www.instagram.com/trashmobinfo"} target="_blank" rel="noreferrer noopener">
                                            <i className="fa-brands fa-instagram"></i>
                                        </a>
                                    </div>
                                    <div className="iconWrapper">
                                        <a href={"https://www.youtube.com/channel/UC2LgFmXFCA8kdkxd4IJ51BA"} target="_blank" rel="noreferrer noopener">
                                            <i className="fa-brands fa-youtube"></i>
                                        </a>
                                    </div>
                                    <div className="iconWrapper">
                                        <a href={"https://www.linkedin.com/company/76188984"} target="_blank" rel="noreferrer noopener">
                                            <i className="fa-brands fa-linkedin"></i>
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div >
                </div >
            </div >
        </footer >
    );
}
