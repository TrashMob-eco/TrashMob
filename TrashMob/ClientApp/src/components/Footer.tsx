import React from 'react';
import { Link } from 'react-router-dom';

export class Footer extends React.Component {
    render() {
        return (
            <div>
                
                <p><Link to="/privacypolicy">Privacy Policy</Link></p>
                <p><Link to="/termsofservice">Terms Of Service</Link></p>
            </div>
        );
    }
}
