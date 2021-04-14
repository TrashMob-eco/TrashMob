import { Component } from 'react';
import * as React from 'react'

import { Link } from 'react-router-dom';

import '../custom.css';

export class Footer extends Component {
    render() {
        return (
            <div>    
                <hr className="rounded" />
                <p><Link to="/privacypolicy">Privacy Policy</Link></p>
                <p><Link to="/termsofservice">Terms Of Service</Link></p>
            </div>
        );
    }
}
