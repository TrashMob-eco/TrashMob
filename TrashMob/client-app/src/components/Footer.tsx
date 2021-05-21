import * as React from 'react'

import { Link } from 'react-router-dom';

import '../custom.css';

export const Footer: React.FC = () => {
    return (
        <div>
            <hr className="rounded" />
            <p><Link to="/privacypolicy">Privacy Policy</Link></p>
            <p><Link to="/termsofservice">Terms Of Service</Link></p>
        </div>
    );
}
