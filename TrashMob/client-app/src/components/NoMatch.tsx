import * as React from 'react'
import { Link } from 'react-router-dom';

export const NoMatch: React.FC = () => {
    return (
        <div>
            <h1>Page Not Found</h1>
            <p>We're sorry, we cannot find the page you have requested. If you think you have found a bug, please <Link to="./contactus">let us know</Link>!</p>
        </div>
    );
}

