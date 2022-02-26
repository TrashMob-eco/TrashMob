import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger } from 'react-bootstrap';
import WaiverData from '../Models/WaiverData';
import { Guid } from 'guid-typescript';

interface AdminWaiversPropsType extends RouteComponentProps {
    waivers: WaiverData[];
    isWaiverDataLoaded: boolean;
    onWaiverListChanged: any;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const AdminWaivers: React.FC<AdminWaiversPropsType> = (props) => {

    const [waiverId, setWaiverId] = React.useState<string>(Guid.createEmpty().toString());
    const [name, setName] = React.useState<string>("");
    const [version, setVersion] = React.useState<string>("");
    const [effectiveDate, setEffectiveDate] = React.useState<Date>();
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [versionErrors, setVersionErrors] = React.useState<string>("");
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [isEditOrAdd, setIsEditOrAdd] = React.useState<boolean>(false);
    const [isWaiverDataLoaded, setIsWaiverDataLoaded] = React.useState<boolean>(false);

    function handleSave(event: any) {
        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        var waiverData = new WaiverData();
        waiverData.id = waiverId;
        waiverData.name = name ?? "";
        waiverData.version = version ?? "";
        waiverData.effectiveDate = effectiveDate;
        waiverData.createdByUserId = createdByUserId ?? props.currentUser.id;
        waiverData.createdDate = createdDate;
        waiverData.lastUpdatedByUserId = props.currentUser.id;

        var data = JSON.stringify(waiverData);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            var method = "PUT";

            if (waiverData.id === Guid.EMPTY) {
                method = "POST";
            }

            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/waivers', {
                method: method,
                body: data,
                headers: headers,
            })
                .then(() => {
                    props.onWaiverListChanged()
                    setIsEditOrAdd(false);
                });
        });
    }

    function handleNameChanged(value: string) {

        if (value === "") {
            setNameErrors("Name cannot be empty.")
        }
        else {
            setName(value);
            setNameErrors("");
        }

        validateForm();
    }

    function handleVersionChanged(value: string) {

        if (value === "") {
            setVersionErrors("Version cannot be empty.")
        }
        else {
            setVersion(value);
            setVersionErrors("");
        }

        validateForm();
    }

    function validateForm() {
        if (name === "" ||
            nameErrors !== "" ||
            version === "" ||
            versionErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    function editWaiver(waiverId: string) {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/waivers/' + waiverId, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<WaiverData>)
                .then(data => {
                    setWaiverId(data.id);
                    setName(data.name);
                    setVersion(data.version);
                    setEffectiveDate(data.effectiveDate)
                    setIsWaiverDataLoaded(true);
                    setIsEditOrAdd(true);
                });
        });
    }

    function addWaiver() {
        setIsWaiverDataLoaded(true);
        setIsEditOrAdd(true);
    }



    function renderWaiversTable(waivers: WaiverData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Version</th>
                            <th>Effective Date</th>
                            <th>Waiver Duration Type</th>
                        </tr>
                    </thead>
                    <tbody>
                        {waivers.map(waiver => {
                            return (
                                <tr key={waiver.id.toString()}>
                                    <td>{waiver.name}</td>
                                    <td>{waiver.effectiveDate}</td>
                                    <td>{waiver.waiverDurationTypeId}</td>
                                    <td>
                                        <Button className="action" onClick={() => addWaiver()}>Add Waiver</Button>
                                    </td>
                                </tr>)
                        }
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderEditWaiver() {
        return (
            <div>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <input type="hidden" name="Id" value={waiverId.toString()} />
                    </Form.Row>
                    <Button disabled={!isSaveEnabled} className="action" onClick={(e) => handleSave(e)}>Save</Button>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderWaiverNameToolTip}>
                                    <Form.Label className="control-label" htmlFor="Name">Name:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="name" defaultValue={name} onChange={val => handleNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                                <span style={{ color: "red" }}>{nameErrors}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderWaiverVersionToolTip}>
                                    <Form.Label className="control-label">Version:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={version} maxLength={parseInt('64')} onChange={(val) => handleVersionChanged(val.target.value)} required />
                                <span style={{ color: "red" }}>{versionErrors}</span>
                            </Form.Group >
                        </Col>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    let contents = props.isWaiverDataLoaded
        ? renderWaiversTable(props.waivers)
        : <p><em>Loading...</em></p>;

    return (
        <div>
            {!props.isWaiverDataLoaded && <p><em>Loading...</em></p>}
            {props.isWaiverDataLoaded && props.waivers && renderWaiversTable(props.waivers)}
            {isEditOrAdd && renderEditWaiver()}
        </div>
    );
}

