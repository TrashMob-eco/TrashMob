import * as React from 'react'

import { Link } from 'react-router-dom';

import '../custom.css';

export const Footer: React.FC = () => {
    return (
        <div >
            <hr className="rounded" />
            <p>
                <Link to="/privacypolicy" style={{ display: 'inline-block', padding: '4px 20px 4px 0px' }}>Privacy Policy</Link>
                <Link to="/termsofservice" style={{ display: 'inline-block', padding: '4px 20px 4px 0px' }}>Terms Of Service</Link>
                <a href="https://www.twitter.com/trashmobe" style={{ display: 'inline-block', padding: '4px 20px 4px 0px' }}>Follow us on Twitter</a>
            </p>
        </div>
    );
}
