import * as React from 'react'
import { RouteComponentProps, useHistory, withRouter } from 'react-router-dom';
import { msalClient } from '../store/AuthStore';
import UserData from './Models/UserData';
import logo from './assets/logo.svg'
import { Dropdown } from 'react-bootstrap';

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
        { name: "Getting Started", url: "/gettingstarted" },
        { name: "My Dashboard", url: "/mydashboard" },
        { name: "About TrashMob", url: "/aboutus" }
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
                <nav className="container navbar navbar-expand-lg navbar-light">

                    <div className="collapse navbar-collapse" id="navbarsExample09">
                        <a className="navbar-brand" href="/"><img src={logo} alt="TrashMob Logo" /></a>
                        <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarsExample09" aria-controls="navbarsExample09" aria-expanded="false" aria-label="Toggle navigation">
                            <span className="navbar-toggler-icon"></span>
                        </button>
                        <ul className="navbar-nav mr-auto">
                            {mainNavItems.map(item => (
                                <li className="nav-item" key={item.name}>
                                    <a className="nav-link" href={item.url}>{item.name}</a>
                                </li>
                            ))}
                            <li className="nav-item dropdown">
                                <Dropdown>
                                    <Dropdown.Toggle className="btn btn-link dropdown-toggle" href="#" role="button" id="dropdown09" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        Learn More
                                    </Dropdown.Toggle>
                                    <Dropdown.Menu className="dropdown-menu" aria-labelledby="dropdown09">
                                        <Dropdown.Item className="dropdown-item" href="/partners">Partners</Dropdown.Item>
                                        <Dropdown.Item className="dropdown-item" href="/sponsors">Sponsors</Dropdown.Item>
                                        <Dropdown.Item className="dropdown-item" href="/contactus">Contact Us</Dropdown.Item>
                                        <Dropdown.Item className="dropdown-item" href="/faq">FAQ</Dropdown.Item>
                                    </Dropdown.Menu>
                                </Dropdown>
                            </li>
                        </ul>
                        <button hidden={!isUserLoaded} className="btn btn-link" onClick={(e) => viewUserProfile(e)}>Welcome{userName ? ", " + userName : ""}!</button>
                        <button hidden={isUserLoaded} className="btn btn-primary" onClick={(e) => signIn(e)}>Sign Up/Log In</button>
                        <button hidden={!isUserLoaded} className="btn btn-outline-primary" onClick={(e) => signOut(e)}>Log Out</button>
                    </div>
                </nav>
            </div>
        </header>
    )
}

export default withRouter(TopMenu);