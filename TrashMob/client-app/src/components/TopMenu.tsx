import * as React from 'react'
import { RouteComponentProps, useHistory, withRouter } from 'react-router-dom';
import { msalClient } from '../store/AuthStore';
import UserData from './Models/UserData';
import logo from './assets/logo.svg'
import { Button, Navbar, Nav, NavDropdown } from 'react-bootstrap';

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
        { name: "What is TrashMob?", url: "/aboutus" },
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

    function viewUserProfile(e: any) {
        e.preventDefault();
        history.push("/userprofile");
    }

    return (
        <header className="tm-header">
            <div className="container-fluid bg-light tm-mainNav">
                <Navbar className="container navbar navbar-expand-lg navbar-light">

                    <div className="collapse navbar-collapse" id="navbarsExample09">
                        <Navbar.Brand href="/"><img src={logo} alt="TrashMob Logo" /></Navbar.Brand>
                        <Navbar.Toggle aria-controls="basic-navbar-nav" />
                        <Navbar.Collapse id="basic-navbar-nav">
                            <Nav className="mr-auto">
                                {mainNavItems.map(item => (
                                    <Nav.Link className="nav-link" href={item.url} key={item.name}>{item.name}</Nav.Link>
                                ))}
                                <NavDropdown title="Learn More" id="basic-nav-dropdown">
                                    <NavDropdown.Item className="dropdown-item" href="/mediagallery">Media Gallery</NavDropdown.Item>
                                    <NavDropdown.Item className="dropdown-item" href="/partners">Partners</NavDropdown.Item>
                                    <NavDropdown.Item className="dropdown-item" href="/contactus">Contact Us</NavDropdown.Item>
                                    <NavDropdown.Item className="dropdown-item" href="/faq">FAQ</NavDropdown.Item>
                                </NavDropdown>
                            </Nav>
                            <Button hidden={!isUserLoaded} className="btn btn-link" style={{ color: "#ffffff" }} onClick={(e) => viewUserProfile(e)}>Welcome{userName ? ", " + userName : ""}!</Button>
                            <Button hidden={isUserLoaded} className="btn btn-primary" onClick={(e) => signIn(e)}>Sign Up/Log In</Button>
                            <Button hidden={!isUserLoaded} className="btn btn-outline-primary" style={{ color: "#ffffff" }} onClick={(e) => signOut(e)}>Log Out</Button>
                        </Navbar.Collapse>
                    </div>
                </Navbar>
            </div>
        </header>
    )
}

export default withRouter(TopMenu);