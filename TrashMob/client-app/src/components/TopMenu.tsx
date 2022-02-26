import * as React from 'react'
import { RouteComponentProps, useHistory, withRouter } from 'react-router-dom';
import { msalClient } from '../store/AuthStore';
import UserData from './Models/UserData';
import logo from './assets/logo.svg'
import { Button, Dropdown, Nav } from 'react-bootstrap';
import './assets/styles/header.css';
import { Person, PersonCircle, PlusLg, Power, Speedometer2 } from 'react-bootstrap-icons';

interface TopMenuProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const TopMenu: React.FC<TopMenuProps> = (props) => {
    const [userName, setUserName] = React.useState<string>("");
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);
    const history = useHistory();

    React.useEffect(() => {
        if (props.currentUser && props.isUserLoaded) {
            setUserName(props.currentUser.userName);
        }

        setIsUserLoaded(props.isUserLoaded);
    }, [props.currentUser, props.isUserLoaded])

    const mainNavItems = [
        { name: "Home", url: '/' },
        // { name: "What is TrashMob?", url: "/aboutus" },
        { name: "Get Started", url: "/gettingstarted" },
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
            <div className="container bg-light tm-mainNav">
                <div className="navbar navbar-expand-lg navbar-light navbar-static-top" id="navbar">
                    <a className="navbar-brand" href="/" id="navbarBrand"><img src={logo} alt="TrashMob Logo" id="logo" /></a>
                    <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                        <span className="navbar-toggler-icon"></span>
                    </button>
                    <div className="navbar-collapse collapse" id="navbarNav">
                        {/* <Navbar.Toggle aria-controls="basic-navbar-nav" /> */}
                        <ul className="nav navbar-nav">
                            {mainNavItems.map(item => (
                                <li><Nav.Link href={item.url} key={item.name}>{item.name}</Nav.Link></li>
                            ))}
                        </ul>
                        {/* <NavDropdown title="Learn More" id="basic-nav-dropdown">
                            <NavDropdown.Item className="dropdown-item" href="/mediagallery">Media Gallery</NavDropdown.Item>
                            <NavDropdown.Item className="dropdown-item" href="/partners">Partners</NavDropdown.Item>
                            <NavDropdown.Item className="dropdown-item" href="/becomeapartner">Become a Partner</NavDropdown.Item>
                            <NavDropdown.Item className="dropdown-item" hidden={!isUserLoaded} href="/partnerdashboard">Partner Dashboard</NavDropdown.Item>
                            <NavDropdown.Item className="dropdown-item" href="/contactus">Contact Us</NavDropdown.Item>
                            <NavDropdown.Item className="dropdown-item" href="/faq">FAQ</NavDropdown.Item>
                            <NavDropdown.Item className="dropdown-item" hidden={!isUserLoaded || !props.currentUser.isSiteAdmin} href="/siteadmin">SiteAdmin</NavDropdown.Item>
                        </NavDropdown> */}
                        <Button hidden={isUserLoaded} className="btn btn-primary" onClick={(e) => signIn(e)} id="loginBtn">Sign in</Button>
                        <Button hidden={isUserLoaded} className="btn btn-primary" onClick={(e) => signIn(e)} id="registerBtn">Sign up</Button>
                        <Dropdown hidden={!isUserLoaded}>
                            <Dropdown.Toggle id="userBtn" variant="light">
                                <PersonCircle className="mr-3" size={32} color="#96ba00" aria-labelledby="userName" />
                                {userName ? userName : 'Welcome'}
                            </Dropdown.Toggle>
                            <Dropdown.Menu className="shadow border-0">
                                <Dropdown.Item eventKey="1" href="/mydashboard"><Speedometer2 aria-hidden="true" />Dashboard</Dropdown.Item>
                                <Dropdown.Divider />
                                <Dropdown.Item eventKey="2" href="/userprofile"><Person aria-hidden="true" />My profile</Dropdown.Item>
                                <Dropdown.Divider />
                                <Dropdown.Item eventKey="3" href="/notificationpreferences"><Person aria-hidden="true" />Notification Preferences</Dropdown.Item>
                                <Dropdown.Divider />
                                <Dropdown.Item eventKey="4"><PlusLg aria-hidden="true" />Add event</Dropdown.Item>
                                <Dropdown.Divider />
                                <Dropdown.Item eventKey="5" onClick={(e) => signOut(e)}><Power aria-hidden="true" />Sign out</Dropdown.Item>
                            </Dropdown.Menu>
                        </Dropdown>
                    </div>
                </div>
            </div>
        </header>
    )
}

export default withRouter(TopMenu);