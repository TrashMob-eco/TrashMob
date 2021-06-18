import * as React from 'react'

import { Link } from 'react-router-dom';

import '../custom.css';

export const Footer: React.FC = () => {
    return (
        <footer>
            <p>
                <table>
                    <tr>
                        <td />
                        <td>
                            <a href="https://twitter.com/TrashMobE?ref_src=twsrc%5Etfw" className="twitter-follow-button" data-show-count="false">Follow @TrashMobE</a><script async src="https://platform.twitter.com/widgets.js" charSet="utf-8"></script>
                            <div className="fb-like" data-href="https://www.trashmob.eco" data-width="" data-layout="standard" data-action="like" data-size="small" data-share="true"></div>
                            <a href="https://profiles.eco/trashmob?ref=tm" rel="noopener"><img className="eco-trustmark" alt=".eco profile for trashmob.eco" src="https://trust.profiles.eco/trashmob/eco-button.svg?color=%2396BA00" /></a>
                            <a href="https://www.instagram.com/trashmob">Instagram</a>
                        </td>
                        <td>
                            <Link to="/privacypolicy">Privacy Policy</Link>
                            <Link to="/termsofservice">Terms of Service</Link>
                        </td>
                        <td />
                    </tr>
                </table>
            </p>
        </footer>
    );
}
