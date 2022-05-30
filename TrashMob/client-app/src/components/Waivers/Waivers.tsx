import React from 'react';
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Jumbotron from 'react-bootstrap/Jumbotron';
import UserData from '../Models/UserData';
import { useEffect } from 'react';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import EnvelopeResponse from '../Models/EnvelopeResponse';

export interface WaiversProps {
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const CurrentTrashMobWaiverVersion = {
    versionId: "0.1",
    versionDate: new Date(2022, 5, 28, 0, 0, 0, 0)
}

export class TrashMobWaiverVersion {
    versionId: string = "0.1";
    versionDate: Date = new Date(2022, 5, 28, 0, 0, 0, 0);
}

const Waivers: React.FC<WaiversProps> = (props) => {

    const [name, setName] = React.useState<string | undefined>();
    const [email, setEmail] = React.useState<string | undefined>();

    useEffect(() => {
        if (props.currentUser) {
            setName(props.currentUser.givenName + " " + props.currentUser.surName)
            setEmail(props.currentUser.email)
        }
    }, [props.currentUser])

    async function signWaiver() {

        const account = msalClient.getAllAccounts()[0];

        const request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {

            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            const envelopeRequest = {
                signerEmail: email,
                signerName: name,
                signerClientId: props.currentUser.id,
                returnUrl: "http://localhost:44332/waiversreturn",
            };
            fetch('/api/docusign', {
                method: 'POST',
                headers: headers,
                body: JSON.stringify(envelopeRequest),
            }).then(response => response.json() as Promise<EnvelopeResponse>)
                .then(data => {
                    // Save the envelope Id to state
                    sessionStorage.setItem('envelopeId', JSON.stringify(data.envelopeId));
                    window.location.href = data.redirectUrl;
                })
        });
    }

    return (
        <Container className='bodyMargin'>
            <Row>
                <Col>
                    <Jumbotron>
                        <h1>TrashMob.eco Waiver</h1>
                        <p>
                            In order to participate in TrashMob.eco events, you must agree to a liability waiver. Please click the
                            Sign Waiver button below. This will take you to a screen which will ask you to view and sign the waiver.
                            Once that is done, you will be redirected back to TrashMob.eco.
                        </p>
                        <p>
                            You will only need to sign this waiver once (unless we have to change the legalese).
                        </p>
                        <p>
                            Thank you!
                        </p>
                    </Jumbotron>
                </Col>
            </Row>
            <Row>
                <Col>
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

export default Waivers;
