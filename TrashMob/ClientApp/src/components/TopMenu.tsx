import { FunctionComponent } from 'react';
import * as React from 'react'

import logo from './assets/Logo1.png';
import { msalClient } from '../store/AuthStore';
import { getUserFromCache } from '../store/accountHandler'
import UserData from './Models/UserData';

export const TopMenu: FunctionComponent = () => {

    const mainNavItems = [
      { name: "Home", url: "/" },
      // { name: "About Us", url: "/about" },
      // { name: "Contact Us", url: "/contactus" },
      { name: "Getting Started", url: "/gettingstarted" },
      { name: "My Dashboard", url: "/mydashboard" }
    ];

    // const secondaryNavItems = [
    //   { name: "Partners", url: "/partners" },
    //   { name: "Sponsors", url: "/sponsors" },
    //   { name: "FAQ", url: "/faq" }
    // ];
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

    // return (
    //     <nav className="navbar navbar-expand-sm navbar-light bg-light bb-primary">
    //         <img src={logo} className="App-logo" alt="logo" />
    //         <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbar5">
    //             <span className="navbar-toggler-icon"></span>
    //         </button>
    //         <div className="navbar-collapse collapse justify-content-stretch" id="navbar">

    //             <ul className="navbar-nav">
    //                 <li className="nav-item">
    //                     <a className="nav-link" href="/">Home</a>
    //                 </li>
    //                 <li className="nav-item">
    //                     <a className="nav-link" href="/about">About</a>
    //                 </li>
    //                 <li className="nav-item">
    //                     <a className="nav-link" href="/gettingstarted">Getting Started</a>
    //                 </li>
    //                 <li className="nav-item">
    //                     <a className="nav-link" href="/mydashboard">My Dashboard</a>
    //                 </li>
    //                 <li className="nav-item">
    //                     <a className="nav-link" href="/partners">Partners</a>
    //                 </li>
    //                 <li className="nav-item">
    //                     <a className="nav-link" href="/sponsors">Sponsors</a>
    //                 </li>
    //                 <li className="nav-item">
    //                     <a className="nav-link" href="/faq">FAQ</a>
    //                 </li>
    //                 <li className="nav-item">
    //                     <a className="nav-link" href="/contactus">Contact Us</a>
    //                 </li>
    //                 <li className="nav-item">
    //                     <button className="btn" onClick={signIn}>Login</button>
    //                 </li>
    //                 <li className="nav-item">
    //                     <button className="btn" onClick={signOut}>Logout</button>
    //                 </li>
    //                 <li className="nav-item">
    //                     <a className="nav-link" href="/profile">Profile</a>
    //                 </li>
    //                 {/* <li className="nav-item">
    //                     <label className="lbl">{userName}</label>
    //                 </li> */}
    //             </ul>
    //         </div>
    //     </nav>
    // )

    return (
    <header className="tm-header">
      {/* <div className="container-fluid tm-secondaryNav">
        <div className="container">
          <ul className="tm-secondaryNav-list">
              {secondaryNavItems.map(item => (
                <li className="tm-secondaryNav-item">
                  <a className="tm-secondaryNav-link" href={item.url}>{item.name}</a>
                </li>
              ))}
          </ul>
        </div>
      </div> */}
      <div className="container-fluid bg-light tm-mainNav">
        <nav className="container navbar navbar-expand-lg navbar-light">
          <a className="navbar-brand" href="/">TrashMob</a>
          <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarsExample09" aria-controls="navbarsExample09" aria-expanded="false" aria-label="Toggle navigation">
            <span className="navbar-toggler-icon"></span>
          </button>

          <div className="collapse navbar-collapse" id="navbarsExample09">
            <ul className="navbar-nav mr-auto">
              {mainNavItems.map(item => (
                <li className="nav-item">
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
            <button className="btn btn-primary" onClick={(e) => signIn(e)}>Sign Up/Log In</button>
            <button className="btn btn-outline-primary" onClick={(e) => signOut(e)}>Log Out</button>
          </div>
        </nav>
    </div>
  </header>
    )
}