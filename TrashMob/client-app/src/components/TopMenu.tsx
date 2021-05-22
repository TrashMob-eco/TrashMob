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
                <nav className="container navbar navbar-expand-lg navbar-light" style={{ display: 'block', flexWrap: 'unset', height: 'auto', width: 'auto' }}>

                    <div className="collapse navbar-collapse" id="navbarsExample09" style={{ display: 'inline-block', width: 100 + '%' }}>
                        <a className="navbar-brand" href="/" style={{ display: 'inline-block', height: 'auto', left: '0', width: '240px', margin: '1px 20px 0px -8px', position: 'relative', flexWrap: 'unset' }}><img src={logo} alt="TrashMob Logo" /></a>
                        <button className="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarsExample09" aria-controls="navbarsExample09" aria-expanded="false" aria-label="Toggle navigation">
                            <span className="navbar-toggler-icon"></span>
                        </button>
                        <ul className="navbar-nav mr-auto" style={{ display: 'inline-block', width: 100 +'%' }}>
                            {mainNavItems.map(item => (
                                <li className="nav-item" key={item.name} style={{ display: 'inline-block', padding: '16px 20px 8px 0px' }}>
                                    <a className="nav-link" style={{ verticalAlign: 'middle', margin: '0px', padding: '0px'}} href={item.url}>{item.name}</a>
                                </li>
                            ))}
                            <li className="nav-item dropdown" style={{ display: 'inline-block', padding: '8px 20px 8px 0px' }}>
                                <button className="btn btn-link dropdown-toggle" id="dropdown09" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style={{ verticalAlign: 'middle', margin: '0px', padding: '0px', display: 'block'}}>Learn More</button>
                                <div className="dropdown-menu" aria-labelledby="dropdown09">
                                    <a className="dropdown-item" href="/partners">Partners</a>
                                    <a className="dropdown-item" href="/sponsors">Sponsors</a>
                                    <a className="dropdown-item" href="/contactus">Contact Us</a>
                                    <a className="dropdown-item" href="/faq">FAQ</a>
                                </div>
                            </li>
                        </ul>
                        <button hidden={!isUserLoaded} className="btn btn-link" onClick={(e) => viewUserProfile(e)}>Welcome{userName ? ", " + userName : ""}!</button>
                        <button hidden={isUserLoaded} className="btn btn-primary" onClick={(e) => signIn(e)} style={{ display: 'block', width: 'auto', margin: '8px 0px 0px', whiteSpace: 'nowrap' }}>Sign Up/Log In</button>
                        <button hidden={!isUserLoaded} className="btn btn-outline-primary" onClick={(e) => signOut(e)}>Log Out</button>
                    </div>
                </nav>
            </div>
        </header>
    )
}

export default withRouter(TopMenu);