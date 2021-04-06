import { FunctionComponent } from 'react';
import * as React from 'react'

import logo from './assets/Logo1.png';
import { msalClient } from '../store/AuthStore';

export const TopMenu: FunctionComponent = () => {

    function signOut(e : any) {
        e.preventDefault();
        const logoutRequest = {
            account: msalClient.getActiveAccount(),
            
        }

        msalClient.logout(logoutRequest);
    }

    function signIn(e: any) {
        e.preventDefault();
        msalClient.loginPopup();
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
                        <a className="nav-link" href="/Home">Home</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/About">About</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/GettingStarted">Getting Started</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/MyDashboard">My Dashboard</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/UserStories">User Stories</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/Partners">Partners</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/Sponsors">Sponsors</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/Faq">FAQ</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/ContactUs">Contact Us</a>
                    </li>
                    <li className="nav-item">
                        <button className="btn" onClick={signIn}>Login</button>
                    </li>
                    <li className="nav-item">
                        <button className="btn" onClick={signOut}>Logout</button>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="/Profile">Profile</a>
                    </li>
                </ul>
            </div>
        </nav>
    )
}