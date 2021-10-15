import * as React from 'react'
import logo from './assets/logo.svg'
import twitter from './assets/twitter.svg'
import facebook from './assets/facebook.svg'
import { Link } from 'react-router-dom';

import '../custom.css';

export const Footer: React.FC = () => {
    return (
        <footer id="pageFooter">
            <div className="container" id="footerWrapper">
                 {/* logo needs to be changed for a white variant ! */}
                <img src={logo} alt="TrashMob Logo" id="logo_footer"/>
                <div className="row">
                    <div className="col-md-4">
                        <Link to="/aboutus">About us</Link>
                        <Link to="/partners">Partners</Link>
                        <Link to="/sponsors">Sponsors</Link>
                        <Link to="/contactus">Contact us</Link>
                    </div>

                    <div className="col-md-4">
                        <Link to="/faq">FAQ</Link>
                        <Link to="/privacypolicy">Privacy Policy</Link>
                        <Link to="/termsofservice">Terms of Service</Link>
                    </div>
                </div>
                <hr className="horizontalLine"/>
                <div className="row" id="copyright">
                    <div className="col-md-8">
                        <p>Copyright 2021 TRASHMOB.ECO - All Rights Reserved.</p>
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
        </footer>
    );
}
