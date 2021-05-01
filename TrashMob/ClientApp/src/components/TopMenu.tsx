import * as React from 'react'
import { RouteComponentProps } from 'react-router-dom';
import { msalClient } from '../store/AuthStore';
import UserData from './Models/UserData';

interface TopMenuProps extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const TopMenu: React.FC<TopMenuProps> = (props) => {

    const mainNavItems = [
        { name: "Home", url: "/" },
        { name: "Getting Started", url: "/gettingstarted" },
        { name: "My Dashboard", url: "/mydashboard" }
    ];

    function signOut(e: any) {
        e.preventDefault();
        const logoutRequest = {
            account: msalClient.getActiveAccount(),

        }

        msalClient.logout(logoutRequest);
    }

    function signIn(e: any) {
        e.preventDefault();
        msalClient.loginRedirect();
    }

    return (
        <header className="tm-header">
            <div className="container-fluid bg-light tm-mainNav">
                <nav className="container navbar navbar-expand-lg navbar-light">
                    <a className="navbar-brand" href="/">TrashMob</a>
                    <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarsExample09" aria-controls="navbarsExample09" aria-expanded="false" aria-label="Toggle navigation">
                        <span className="navbar-toggler-icon"></span>
                    </button>

                    <div className="collapse navbar-collapse" id="navbarsExample09">
                        <ul className="navbar-nav mr-auto">
                            {mainNavItems.map(item => (
                                <li className="nav-item" key={item.name}>
                                    <a className="nav-link" href={item.url}>{item.name}</a>
                                </li>
                            ))}
                            <li className="nav-item dropdown">
                                <a className="nav-link dropdown-toggle" href="#" id="dropdown09" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Learn More</a>
                                <div className="dropdown-menu" aria-labelledby="dropdown09">
                                    <a className="dropdown-item" href="/aboutus">About TrashMob</a>
                                    <a className="dropdown-item" href="/partners">Partners</a>
                                    <a className="dropdown-item" href="/sponsors">Sponsors</a>
                                    <a className="dropdown-item" href="/contactus">Contact Us</a>
                                    <a className="dropdown-item" href="/faq">FAQ</a>
                                </div>
                            </li>
                        </ul>
                        <label hidden={!props.isUserLoaded}>Welcome, {props.currentUser.givenName}</label>
                        <button hidden={props.isUserLoaded} className="btn btn-primary" onClick={(e) => signIn(e)}>Sign Up/Log In</button>
                        <button hidden={!props.isUserLoaded}className="btn btn-outline-primary" onClick={(e) => signOut(e)}>Log Out</button>
                    </div>
                </nav>
            </div>
        </header>
    )
}