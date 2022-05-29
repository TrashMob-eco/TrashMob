import React from 'react';
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Jumbotron from 'react-bootstrap/Jumbotron';
import Navbar from 'react-bootstrap/Navbar';
import Nav from 'react-bootstrap/Nav';
import { ToastContainer, toast } from 'react-toastify';
import OAuthImplicit from './OAuthImplicit';
import UserData from '../Models/UserData';
import { useEffect } from 'react';
import * as DS from '../../store/Docusign';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import EnvelopeResponse from '../Models/EnvelopeResponse';

export interface WaiversProps {
    isUserLoaded: boolean;
    currentUser: UserData;
};

const Waivers: React.FC<WaiversProps> = (props) => {

    const oAuthImplicit = new OAuthImplicit(oAuthResults);
    const [accessToken, setAccessToken] = React.useState<string | undefined>();
    const [expires, setExpires] = React.useState<Date | undefined>();
    const [name, setName] = React.useState<string | undefined>();
    const [email, setEmail] = React.useState<string | undefined>();
    const [externalAccountId, setExternalAccountId] = React.useState<string | undefined>();
    const [accountName, setAccountName] = React.useState<string | undefined>();
    const [accountId, setAccountId] = React.useState<string | undefined>();
    const [page, setPage] = React.useState<string>('welcome');
    const [working, setWorking] = React.useState<boolean>(false);
    const [workingMessage, setWorkingMessage] = React.useState<string>('');

    useEffect(() => {
        /**
         * Starting up--if our URL includes a hash, check it to see if
         * it's the OAuth response
         */
        // if the url has a query parameter of ?error=logout_request (from a logout operation) 
        // then remove it
        if (window.location.search && window.location.search === '?error=logout_request') {
            window.history.replaceState(null, '', DS.DS_APP_URL);
        }

        setName(props.currentUser.givenName + " " + props.currentUser.surName)
        setEmail(props.currentUser.email)

        if (DS.DS_REDIRECT_AUTHENTICATION) {
            const hash = window.location.hash;
            if (!hash) { return }
            // possible OAuth response
            setWorking(true);
            setWorkingMessage('Logging in');
            oAuthImplicit.receiveHash(hash)
                .then(() => {
                    setWorking(false);
                });
        } else {
            // await authentication via the new tab
            window.addEventListener("message", receiveMessage, false);
        }
    }, []);

    useEffect(() => {
        if (props.currentUser) {
            setName(props.currentUser.givenName + " " + props.currentUser.surName)
            setEmail(props.currentUser.email)
        }
    }, [props.currentUser])

    /**
     * Receive message from a child .
     * This method is only used if authentication is done
     * in a new tab. See file public/oauthResponse.html
     * @param {object} e 
     */
    async function receiveMessage(e: any) {
        const rawSource = e && e.data && e.data.source
            , ignore = {
                'react-devtools-inject-backend': true,
                'react-devtools-content-script': true,
                'react-devtools-detector': true,
                'react-devtools-bridge': true
            }
            , source = (rawSource && !ignore[rawSource]) ? rawSource : false
            ;
        if (!source) { return }; // Ignore if no source field
        if (source === 'oauthResponse') {
            setWorking(true);
            const hash = e.data && e.data.hash;
            await oAuthImplicit.receiveHash(hash);
            setWorking(false);
        }
    }

    function startAuthentication() {
        oAuthImplicit.startLogin();
    }

    /**
     * Is the accessToken ok to use?
     * @returns boolean accessTokenIsGood
     */
    function checkToken() {
        if (
            !accessToken ||
            expires === undefined ||
            new Date() > expires
        ) {
            // Need new login. Only clear auth, don't clear the state (leave form contents);
            clearAuth();
            setPage('welcome');
            setWorking(false);
            toast.error('Your Docusign login session has ended.\nPlease login to Docusign again', {
                autoClose: 8000,
            });
            return false;
        }
        return true;
    }

    /**
     * This method clears this app's authentication information.
     * But there may still be an active login session cookie
     * from the IdP. Your IdP may have an API method for clearing
     * the login session.
     */
    function logout() {
        clearAuth();
        clearState();
        setPage('welcome');
        toast.success('You have logged out from docusign.', { autoClose: 5000 });
        oAuthImplicit.logout();
    }

    /**
     * Clear authentication-related state
     */
    function clearAuth() {
        setAccessToken(undefined);
        setExpires(undefined);
        setAccountId(undefined);
        setExternalAccountId(undefined);
        setAccountName(undefined);
    }

    /**
     * Clear the app's form and related state
     */
    function clearState() {
        setWorking(false);
    }

    /**
     * Process the oauth results.
     * This method is called by the OAuthImplicit class
     * @param results
     */
    function oAuthResults(results: any) {
        setAccessToken(results.accessToken);
        setExpires(results.expires);
        setExternalAccountId(results.externalAccountId);
        setAccountId(results.accountId);
        setAccountName(results.accountName);
        setPage('loggedIn');

        toast.success(`Welcome ${results.name}, you are now logged in to Docusign`);
    }

    async function signWaiver() {

        if (!checkToken()) {
            return; // Problem! The user needs to login
        }

        const account = msalClient.getAllAccounts()[0];

        const request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {

            setWorking(true);

            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            const envelopeRequest = {
                accountId: accountId,
                signerEmail: email,
                signerName: name,
                signerClientId: props.currentUser.id,
                accessToken: accessToken,
                basePath: "https://demo.docusign.net/restapi",
                returnUrl: "http://localhost:44332/waiversreturn",
            };

            fetch('/api/docusign', {
                method: 'POST',
                headers: headers,
                body: JSON.stringify(envelopeRequest),
            }).then(response => response.json() as Promise<EnvelopeResponse>)
                .then(data => {
                    window.location.href = data.redirectUrl;
                })
        });
    }

    function LoggedIn() {
        return (
            <Container className='bodyMargin'>
                <Row>
                    <Col className="col-md-4">
                        <h2>Sign Waiver</h2>
                        <Form>
                            <Button variant="primary" onClick={signWaiver}>
                                Sign Waiver
                            </Button>
                        </Form>
                    </Col>
                </Row>
            </Container>
        )
    }

    function Welcome() {
        return (
            <Container fluid className='welcomeMargin'>
                <Row>
                    <Col>
                        <Jumbotron>
                            <h1>TrashMob.eco Waiver</h1>
                            <p>
                                In order to participate in TrashMob.eco events, you must agree to a liability waiver. Please click the
                                Login button below. This will take you to a screen which will either allow you to sign in
                                to an existing Docusign account, or allow you to create a new Docusign Account. Once you have signed in, you will
                                be asked to view and sign the waiver. Once that is done, you will be redirected back to TrashMob.eco.
                            </p>
                            <p>
                                You will only need to sign this waiver once (unless we have to change the legalese).
                            </p>
                            <p>
                                Thank you!
                            </p>
                            <p>
                                Login with your DocuSign credentials.
                            </p>
                            <p>
                                <Button variant="primary" onClick={startAuthentication}>Login</Button>
                            </p>
                        </Jumbotron>
                    </Col>
                </Row>
            </Container>
        )
    }

    // Just two pages with a common header. 
    // Choose the body of the page:
    let pagebody;
    switch (page) {
        case 'welcome': // not logged in
            pagebody = Welcome();
            break;
        case 'loggedIn':
            pagebody = LoggedIn();
            break;
        default:
            pagebody = Welcome();
    };

    // Compute the name block for the top nav section
    let nameBlock;
    if (accessToken) {
        nameBlock = (
            <Navbar.Text>
                {name}<br />
                {accountName} ({externalAccountId})
                <Nav>
                    <Nav.Link href="#" onClick={() => logout()}>Logout</Nav.Link>
                </Nav>
            </Navbar.Text>
        )
    } else {
        nameBlock = null;
    }

    // The spinner
    const spinner = (
        <Container fluid className='bodyMargin'
            style={{ display: working ? 'block' : 'none' }}>
            <Row className='justify-content-center'>
                <div className="spinner" />
            </Row>
            <Row className='justify-content-center'>
                <h3>{workingMessage}…</h3>
            </Row>
        </Container>
    )

    // The complete page:
    return (
        <>
            <Navbar fixed="top" bg="primary" variant="dark" >
                <Navbar.Brand>DocuSign Code Example</Navbar.Brand>
                <Navbar.Toggle />
                <Navbar.Collapse className="justify-content-end">{nameBlock}</Navbar.Collapse>
            </Navbar>
            <ToastContainer />
            {spinner}
            {pagebody}
        </>
    );
}

export default Waivers;
