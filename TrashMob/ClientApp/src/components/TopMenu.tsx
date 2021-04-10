import { FunctionComponent } from 'react';
import * as React from 'react'

import logo from './assets/Logo1.png';
import { msalClient } from '../store/AuthStore';
import { getUserFromCache } from '../store/accountHandler'
import UserData from './Models/UserData';

export const TopMenu: FunctionComponent = () => {
    //const [userName, setUserName] = React.useState('username');

    //window.addEventListener('storage', () => {
    //    var user = getUserFromCache() as UserData;
    //    if (user == null) {
    //        setUserName("Not Signed In");
    //    }
    //    else {
    //        if (user.userName === "") {
    //            setUserName("Username not set");
    //        }
    //        else {
    //            setUserName("Welcome " + user.userName);
    //        }
    //    }
    //});
    function signOut(e : any) {
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
        <nav className="navbar navbar-expand-sm navbar-light bg-light bb-primary">
            <img src={logo} className="App-logo" alt="logo" />
            <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbar5">
                <span className="navbar-toggler-icon"></span>
            </button>
            <div className="navbar-collapse collapse justify-content-stretch" id="navbar">

                <ul className="navbar-nav">
                    <li className="nav-item">
                        <a className="nav-link" href="/">Home</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/about">About</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/gettingstarted">Getting Started</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/mydashboard">My Dashboard</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/partners">Partners</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/sponsors">Sponsors</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/faq">FAQ</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/contactus">Contact Us</a>
                    </li>
                    <li className="nav-item">
                        <button className="btn" onClick={signIn}>Login</button>
                    </li>
                    <li className="nav-item">
                        <button className="btn" onClick={signOut}>Logout</button>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/profile">Profile</a>
                    </li>
                {/*    <li className="nav-item">*/}
                {/*        <label className="lbl">{userName}</label>*/}
                {/*    </li>*/}
                </ul>
            </div>
        </nav>
    )
}