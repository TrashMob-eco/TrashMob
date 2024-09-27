import * as React from 'react';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Button, Dropdown, Nav } from 'react-bootstrap';
import { getApiConfig, getB2CPolicies, msalClient } from '../store/AuthStore';
import UserData from './Models/UserData';
import logo from './assets/logo.svg';
import './assets/styles/header.css';
import {
    BoxArrowLeft,
    Map,
    Person,
    PersonX,
    PersonBadge,
    PersonCircle,
    PlusLg,
    Speedometer2,
    QuestionCircle,
} from 'react-bootstrap-icons';

interface TopMenuProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const TopMenu: React.FC<TopMenuProps> = (props) => {
    const [userName, setUserName] = React.useState<string>('');
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);

    React.useEffect(() => {
        if (props.currentUser && props.isUserLoaded) {
            setUserName(props.currentUser.userName);
        }

        setIsUserLoaded(props.isUserLoaded);
    }, [props.currentUser, props.isUserLoaded]);

    const mainNavItems = [
        { name: 'Getting Started', url: '/gettingstarted' },
        ...(isUserLoaded ? [{ name: 'My Dashboard', url: '/mydashboard' }] : []),
        { name: 'Events', url: '/#events' },
        { name: 'Donate', url: 'https://donate.stripe.com/14k9DN2EnfAog9O3cc' },
        { name: 'Shop', url: '/shop' },
    ];

    function signOut(e: any) {
        e.preventDefault();
        const logoutRequest = {
            account: msalClient.getActiveAccount(),
        };

        msalClient.logout(logoutRequest);
    }

    function profileEdit(e: any) {
        e.preventDefault();

        const account = msalClient.getAllAccounts()[0];
        const policy = getB2CPolicies();
        const scopes = getApiConfig();

        const request = {
            account,
            authority: policy.authorities.profileEdit.authority,
            scopes: scopes.b2cScopes,
        };

        msalClient.acquireTokenRedirect(request);
    }

    function signIn(e: any) {
        e.preventDefault();
        const apiConfig = getApiConfig();

        msalClient.loginRedirect({
            scopes: apiConfig.b2cScopes,
        });
    }

    return (
        <header className='tm-header'>
            <div className='container bg-light tm-mainNav'>
                <div className='navbar navbar-expand-lg navbar-light navbar-static-top px-0' id='navbar'>
                    <a className='navbar-brand' href='/' id='navbarBrand'>
                        <img src={logo} alt='TrashMob Logo' className='logo m-0' />
                    </a>
                    <button
                        className='navbar-toggler'
                        type='button'
                        data-toggle='collapse'
                        data-target='#navbarNav'
                        aria-controls='navbarNav'
                        aria-expanded='false'
                        aria-label='Toggle navigation'
                    >
                        <span className='navbar-toggler-icon' />
                    </button>
                    <div className='navbar-collapse collapse' id='navbarNav'>
                        {/* <Navbar.Toggle aria-controls="basic-navbar-nav" /> */}
                        <ul className='nav navbar-nav'>
                            {mainNavItems.map((item) => (
                                <li key={item.name}>
                                    <Nav.Link href={item.url}>{item.name}</Nav.Link>
                                </li>
                            ))}
                        </ul>
                        <Button
                            hidden={isUserLoaded}
                            className='btn btn-primary'
                            onClick={(e) => signIn(e)}
                            id='loginBtn'
                        >
                            Sign in
                        </Button>
                        <Dropdown hidden={!isUserLoaded}>
                            <Dropdown.Toggle id='userBtn' variant='light'>
                                <PersonCircle className='mr-3' size={32} color='#96ba00' aria-labelledby='userName' />
                                {userName || 'Welcome'}
                            </Dropdown.Toggle>
                            <Dropdown.Menu className='shadow border-0'>
                                <Dropdown.Item eventKey='1' href='/mydashboard'>
                                    <Speedometer2 aria-hidden='true' />
                                    My dashboard
                                </Dropdown.Item>
                                <Dropdown.Divider />
                                <Dropdown.Item eventKey='2' href='/manageeventdashboard'>
                                    <PlusLg aria-hidden='true' />
                                    Add event
                                </Dropdown.Item>
                                <Dropdown.Divider />
                                <Dropdown.Item eventKey='3' onClick={(e) => profileEdit(e)}>
                                    <Person aria-hidden='true' />
                                    Update my profile
                                </Dropdown.Item>
                                <Dropdown.Divider />
                                <Dropdown.Item eventKey='4' href='/locationpreference'>
                                    <Map aria-hidden='true' />
                                    My location preference
                                </Dropdown.Item>
                                <Dropdown.Divider />
                                {props.currentUser.isSiteAdmin ? (
                                    <>
                                        {' '}
                                        <Dropdown.Item
                                            eventKey='5'
                                            href='/siteadmin'
                                            disabled={!props.currentUser.isSiteAdmin}
                                        >
                                            <PersonBadge aria-hidden='true' />
                                            Site administration
                                        </Dropdown.Item>
                                        <Dropdown.Divider />
                                    </>
                                ) : (
                                    ''
                                )}
                                <Dropdown.Item eventKey='6' href='/deletemydata'>
                                    <PersonX aria-hidden='true' />
                                    Delete my account
                                </Dropdown.Item>
                                <Dropdown.Divider />
                                <Dropdown.Item eventKey='7' onClick={(e) => signOut(e)}>
                                    <BoxArrowLeft aria-hidden='true' />
                                    Sign out
                                </Dropdown.Item>
                            </Dropdown.Menu>
                        </Dropdown>
                        <Button className='btn' href='/help' id='helpBtn'>
                            <QuestionCircle />
                        </Button>
                    </div>
                </div>
            </div>
        </header>
    );
};

export default withRouter(TopMenu);
