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
                            <Link to="/partners">Partners</Link>
                        </div>

                        <div className="col-md-4">
                            <Link to="/faq">FAQ</Link>
                            <Link to="/volunteeropportunities">Volunteer opportunities</Link>
                            <Link to="/contactus">Contact us</Link>
                        </div>

                        <div className="col-md-4">
                            <Link to="/privacypolicy">Privacy policy</Link>
                            <Link to="/termsofservice">Terms of service</Link>
                            <Link to="/waiver">Waiver</Link>
                            <Link to="/eventsummaries">Event summaries</Link>
                        </div>
                    </div>
                    <hr className="horizontalLine" />
                    <div className="row" id="copyright">
                        <div className="col-md-8">
                            <p>Copyright &copy; 2021 TRASHMOB.ECO - All rights reserved.</p>
                        </div>
                        <div className="col-md-4">
                            <div className="row" id="iconsOrigWrapper">
                                <div className="iconWrapper" id="firstWrapper">
                                    <Link to="https://www.facebook.com/trashmob.eco/">
                                        <i className="fab fa-facebook-f"></i>
                                    </Link>
                                </div>
                                <div className="iconWrapper">
                                    <Link to="https://twitter.com/TrashMobEco">
                                        <i className="fab fa-twitter"></i>
                                    </Link>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </footer>
    );
}
