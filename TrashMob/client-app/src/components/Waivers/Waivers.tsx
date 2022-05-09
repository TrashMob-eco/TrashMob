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
import DocuSign from './Docusign';
import configuration from '../../store/config.json';

export interface WaiversProps {
    isUserLoaded: boolean;
    currentUser: UserData;
};

const Waivers: React.FC<WaiversProps> = (props) => {

    const oAuthImplicit = new OAuthImplicit(this);
    const docusign = new DocuSign(this);
    const [accessToken, setAccessToken] = React.useState<string | undefined>();
    const [expires, setExpires] = React.useState<Date | undefined>();
    const [name, setName] = React.useState<string | undefined>();
    const [email, setEmail] = React.useState<string | undefined>();
    const [externalAccountId, setExternalAccountId] = React.useState<string | undefined>();
    const [accountName, setAccountName] = React.useState<string | undefined>();
    const [accountId, setAccountId] = React.useState<string | undefined>();
    const [baseUri, setBaseUri] = React.useState<string | undefined>();
    const [page, setPage] = React.useState<string>('welcome');
    const [working, setWorking] = React.useState<boolean>(false);
    const [workingMessage, setWorkingMessage] = React.useState<string>('');
    const [responseErrorMsg, setResponseErrorMsg] = React.useState<string | undefined>();
    const [responseEnvelopeId, setResponseEnvelopeId] = React.useState<string | undefined>();
    const [responseAvailableApiRequests, setResponseAvailableApiRequests] = React.useState<number | undefined>();
    const [responseApiRequestsReset, setResponseApiRequestsReset] = React.useState<Date | undefined>();
    const [responseSuccess, setResponseSuccess] = React.useState<boolean | undefined>();
    const [responseTraceId, setResponseTraceId] = React.useState<string | undefined>();
    const [resultsEnvelopeJson, setResultsEnvelopeJson] = React.useState<string | undefined>();
    const [formName, setFormName] = React.useState<string>('');
    const [formEmail, setFormEmail] = React.useState<string>('');

    useEffect(() => {
        /**
         * Starting up--if our URL includes a hash, check it to see if
         * it's the OAuth response
         */
        const config = configuration;
        // if the url has a query parameter of ?error=logout_request (from a logout operation) 
        // then remove it
        if (window.location.search && window.location.search === '?error=logout_request') {
            window.history.replaceState(null, '', config.DS_APP_URL);
        }

        if (config?.DS_REDIRECT_AUTHENTICATION) {
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
    });

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
            setWorkingMessage('Logging in');
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
            toast.error('Your login session has ended.\nPlease login again', {
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
        toast.success('You have logged out.', { autoClose: 5000 });
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
        setBaseUri(undefined);
        setName(undefined);
        setEmail(undefined);
    }

    /**
     * Clear the app's form and related state
     */
    function clearState() {
        setFormName('');
        setFormEmail('');
        setWorking(false);
        setResponseErrorMsg(undefined);
        setResponseEnvelopeId(undefined);
        setResponseAvailableApiRequests(undefined);
        setResponseApiRequestsReset(undefined)
        setResponseSuccess(undefined);
        setResponseTraceId(undefined);
        setResultsEnvelopeJson(undefined);
    }

    /**
     * Process the oauth results.
     * This method is called by the OAuthImplicit class
     * @param results
     */
    function oAuthResults(results: any) {
        setAccessToken(results.accessToken);
        setExpires(results.expires);
        setName(results.name);
        setExternalAccountId(results.externalAccountId);
        setEmail(results.email);
        setAccountId(results.accountId);
        setAccountName(results.accountName);
        setBaseUri(results.baseUri);
        setPage('loggedIn');
        setFormName(results.name);
        setFormEmail(results.email);

        toast.success(`Welcome ${results.name}, you are now logged in`);
    }

    function formNameChange(event: any) {
        setFormName(event.target.value);
    }

    function formEmailChange(event: any) {
        setFormEmail(event.target.value);
    }

    async function sendEnvelope() {
        setResponseErrorMsg(undefined);
        setResponseEnvelopeId(undefined);
        setResponseAvailableApiRequests(undefined);
        setResponseApiRequestsReset(undefined);
        setResponseSuccess(undefined);
        setResponseTraceId(undefined);
        setResultsEnvelopeJson(undefined);

        if (!checkToken()) {
            return; // Problem! The user needs to login
        }

        if (!formEmail || formEmail.length < 5) {
            toast.error("Problem: Enter the signer's email address");
            return;
        }

        if (!formName || formName.length < 5) {
            toast.error("Problem: Enter the signer's name");
            return;
        }

        setWorking(true);
        setWorkingMessage("Sending envelope");
        const results = await docusign.sendEnvelope();

        if (!results) {
            return;
        }

        const { apiRequestsReset } = results;
        const responseApiRequestsReset = apiRequestsReset ? new Date(apiRequestsReset) : undefined;
        setWorking(false);
        setResponseSuccess(results.success);
        setResponseErrorMsg(results.errorMsg);
        setResponseEnvelopeId(results.envelopeId);
        setResponseAvailableApiRequests(results.availableApiRequests);
        setResponseTraceId(results.traceId);
        setResponseApiRequestsReset(responseApiRequestsReset);
    }

    async function getEnvelope() {
        setResponseErrorMsg(undefined);
        setResponseEnvelopeId(undefined);
        setResponseAvailableApiRequests(undefined);
        setResponseApiRequestsReset(undefined);
        setResponseSuccess(undefined);
        setResponseTraceId(undefined);

        if (!checkToken()) {
            return; // Problem! The user needs to login
        }
        if (!responseEnvelopeId) {
            toast.error("Problem: First send an envelope");
            return;
        }

        setWorking(true);
        setWorkingMessage("Fetching the envelope's status");

        const results = await docusign.getEnvelope();
        const { apiRequestsReset } = results;
        const responseApiRequestsReset = apiRequestsReset ? new Date(apiRequestsReset) : undefined;

        setWorking(false);
        setResponseSuccess(results.success);
        setResponseErrorMsg(results.errorMsg);
        setResponseAvailableApiRequests(results.availableApiRequests);
        setResponseTraceId(results.traceId);
        setResponseApiRequestsReset(responseApiRequestsReset);
        setResultsEnvelopeJson(results.resultsEnvelopeJson);
        setResponseApiRequestsReset(responseApiRequestsReset);
    }

    function LoggedIn() {
        const resetTime = responseApiRequestsReset;
        const resetTimeString = resetTime
            ? new Intl.DateTimeFormat('en-US', {
                dateStyle: 'medium',
                timeStyle: 'full',
            }).format(resetTime)
            : undefined;
        return (
            <Container className='bodyMargin'>
                <Row>
                    <Col className="col-md-4">
                        <h2>Send an Envelope</h2>
                        <Form>
                            <Form.Group controlId="formName">
                                <Form.Label>Name</Form.Label>
                                <Form.Control type="text" placeholder="Name"
                                    value={formName}
                                    onChange={formNameChange}

                                />
                            </Form.Group>
                            <Form.Group controlId="formEmail">
                                <Form.Label>Email</Form.Label>
                                <Form.Control type="email" placeholder="Email"
                                    value={formEmail}
                                    onChange={formEmailChange}
                                />
                            </Form.Group>

                            <Button variant="primary" onClick={sendEnvelope}>
                                Send Envelope
                            </Button>
                            <Button variant="primary" className='ml-4' onClick={getEnvelope}>
                                Get Envelope Status
                            </Button>
                        </Form>
                    </Col>
                </Row>
                <Row className='mt-4'>
                    <Col>
                        <h2>Results</h2>
                        <h2>
                            {responseSuccess !== undefined ? (
                                responseSuccess ? (
                                    <>✅ Success!</>
                                ) : (
                                    <>❌ Problem!</>
                                )
                            ) : null}
                        </h2>
                        {responseErrorMsg ? (
                            <p>Error message: {responseErrorMsg}</p>
                        ) : null}
                        {responseEnvelopeId ? (
                            <p>Envelope ID: {responseEnvelopeId}</p>
                        ) : null}
                        {resultsEnvelopeJson ? (
                            <p><pre>Response: {JSON.stringify(resultsEnvelopeJson, null, 4)}</pre></p>
                        ) : null}
                        {responseAvailableApiRequests ? (
                            <p>
                                Available API requests: {responseAvailableApiRequests}
                            </p>
                        ) : null}
                        {resetTimeString ? (
                            <p>API requests reset time: {resetTimeString}</p>
                        ) : null}
                        {responseTraceId ? (
                            <p>
                                Trace ID: {responseTraceId}. Please include with all
                                customer service questions.
                            </p>
                        ) : null}
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
                            <h1>React Example with OAuth Authentication</h1>
                            <p>
                                In this example the user authenticates with DocuSign via the OAuth Implicit grant flow.
                                Since the app will then have an access token for the user, the app can call any
                                DocuSign eSignature REST API method.
                            </p>
                            <p>
                                Use this example for apps used by the staff of your organization who have
                                DocuSign accounts. For example, an application could pull data from multiple
                                sources and then send an envelope that includes the data.
                            </p>
                            <p>
                                Login with your DocuSign Developer (Demo) credentials.
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
