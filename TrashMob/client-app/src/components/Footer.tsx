import * as React from 'react'

import { Link } from 'react-router-dom';

import '../custom.css';

export const Footer: React.FC = () => {
    return (
        <footer>
            <p>
                <Link to="/privacypolicy">Privacy Policy</Link>
                <Link to="/termsofservice">Terms of Service</Link>
                <a href="https://www.twitter.com/trashmobe">Twitter</a>
                <a href="https://www.instagram.com/trashmob">Instagram</a>
            </p>
        </footer>
    );
}
