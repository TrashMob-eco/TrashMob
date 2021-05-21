import * as React from 'react'
import { RouteComponentProps, useHistory, withRouter } from 'react-router-dom';
import { msalClient } from '../store/AuthStore';
import UserData from './Models/UserData';
import logo from './assets/logo.svg'

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

    function viewUserProfile(e: any) {
        e.preventDefault();
        history.push("/userprofile");
    }

    return (
        <header className="tm-header">
            <div className="container-fluid bg-light tm-mainNav">
                <nav className="container navbar navbar-expand-lg navbar-light">
                    <a className="navbar-brand" href="/"><img src={logo} alt="TrashMob Logo" /></a>
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
                                <button className="btn btn-link dropdown-toggle" id="dropdown09" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Learn More</button>
                                <div className="dropdown-menu" aria-labelledby="dropdown09">
                                    <a className="dropdown-item" href="/aboutus">About TrashMob</a>
                                    <a className="dropdown-item" href="/partners">Partners</a>
                                    <a className="dropdown-item" href="/sponsors">Sponsors</a>
                                    <a className="dropdown-item" href="/contactus">Contact Us</a>
                                    <a className="dropdown-item" href="/faq">FAQ</a>
                                </div>
                            </li>
                        </ul>
                        <button hidden={!isUserLoaded} className="btn btn-link" onClick={(e) => viewUserProfile(e)}>Welcome{userName ? ", " + userName : ""}!</button>
                        <button hidden={isUserLoaded} className="btn btn-primary" onClick={(e) => signIn(e)}>Sign Up/Log In</button>
                        <button hidden={!isUserLoaded}className="btn btn-outline-primary" onClick={(e) => signOut(e)}>Log Out</button>
                    </div>
                </nav>
            </div>
        </header>
    )
}

export default withRouter(TopMenu);