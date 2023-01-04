import React from 'react';
import { useEffect } from 'react';
import Container from 'react-bootstrap/Container';
import Button from 'react-bootstrap/Button';
import UserData from '../Models/UserData';
import { Col, Form, Image, OverlayTrigger, Row, Tooltip } from 'react-bootstrap';
import { getApiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import EnvelopeResponse from '../Models/EnvelopeResponse';
import logo from "../assets/logo.svg";
import globes from '../assets/gettingStarted/globes.png';
import * as ToolTips from "../../store/ToolTips";

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

    const [fullName, setFullName] = React.useState<string>("");
    const [fullNameErrors, setFullNameErrors] = React.useState<string>("");
    const [email, setEmail] = React.useState<string | undefined>();
    const [isSignWaiverEnabled, setIsSignWaiverEnabled] = React.useState<boolean>(false);

    useEffect(() => {
        if (props.currentUser) {
            setEmail(props.currentUser.email)
        }
    }, [props.currentUser])

    async function signWaiver() {

        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

        const request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {

            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);
            var hostname = window.location.hostname;

            if (hostname === "localhost") {
                hostname = hostname + ":" + window.location.port
            }

            const envelopeRequest = {
                signerEmail: email,
                signerName: fullName,
                createdByUserId: props.currentUser.id,
                returnUrl: "https://" + hostname + "/waiversreturn",
            };
            fetch('/api/docusign', {
                method: 'POST',
                headers: headers,
                body: JSON.stringify(envelopeRequest),
            }).then(response => response.json() as Promise<EnvelopeResponse>)
                .then(data => {
                    // Save the envelope Id to state
                    sessionStorage.setItem('envelopeId', data.envelopeId);
                    window.location.href = data.redirectUrl;
                })
        });
    }

    React.useEffect(() => {
        if (fullNameErrors !== "") {
            setIsSignWaiverEnabled(false);
        }
        else {
            setIsSignWaiverEnabled(true);
        }
    }, [fullNameErrors])

    const handleFullNameChanged = (val: string) => {
        if (!val || val === "") {
            setFullNameErrors("Full Name is required.")
            setFullName("");
        }
        else {
            setFullNameErrors("")
            setFullName(val);
        }
        setFullName(val);
    }

    const renderFullNameToolTip = (props: any) => {
        return <Tooltip {...props}>{ToolTips.WaiverFullName}</Tooltip>
    }

    return (
        <>
            <Container fluid className='bg-grass'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className='font-weight-bold'>Waiver</h1>
                        <h4 className="font-weight-bold">
                            Safety first!
                        </h4>
                    </Col>
                    <Col md={5}>
                        <Image src={globes} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>

            <Container className='bodyMargin'>
                <h2 className='fw-500 font-size-xl'>TrashMob.eco Waiver</h2>
                <p className="p-18">
                    In order to participate in TrashMob.eco events, you must agree to a liability waiver. Please click the
                    <span className='color-primary'> Sign Waiver</span> button below. This will take you to a screen which will ask you to view and sign the waiver.
                    Once that is done, you will be redirected back to TrashMob.eco.
                </p>
                <p className="p-18">
                    You will only need to sign this waiver once unless we have to change the legalese.
                </p>
                <Form>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderFullNameToolTip}>
                                <Form.Label className="control-label font-weight-bold h5">Full Name</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={fullName} maxLength={parseInt('100')} onChange={(val) => handleFullNameChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{fullNameErrors}</span>
                        </Form.Group>
                    </Col>
                    <Col>
                    <Button disabled={!isSignWaiverEnabled} variant="primary" onClick={signWaiver} className="h-49 fw-600">
                        Sign Waiver
                        </Button>
                    </Col>
                </Form>
                <p className="p-18 mb-5">
                    Thank you!
                </p>
                <p className="p-18">The team at TrashMob.eco.</p>
                <Row className='mb-5'>
                    <Col lg={3} sm={6} md={4} xs={6} className="p-0">
                        <img src={logo} className="p-0 m-0 pl-2 mb-5" alt="TrashMob Logo" id="logo" />
                    </Col>
                </Row>
            </Container>
        </>
    )
}

export default Waivers;
