import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, ButtonGroup, ToggleButton } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import PartnerLocationData from '../Models/PartnerLocationData';
import { PartnerLocationEdit } from './PartnerLocationEdit';
import { PartnerLocationServices } from './PartnerLocationServices';
import { PartnerLocationContacts } from './PartnerLocationContacts';
import { PartnerLocationEventRequests } from './PartnerLocationEventRequests';
import { Guid } from 'guid-typescript';

export interface PartnerLocationsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerLocations: React.FC<PartnerLocationsDataProps> = (props) => {

    const [radioValue, setRadioValue] = React.useState('1');
    const [partnerLocations, setPartnerLocations] = React.useState<PartnerLocationData[]>([]);
    const [isPartnerLocationDataLoaded, setIsPartnerLocationDataLoaded] = React.useState<boolean>(false);
    const [partnerLocationId, setPartnerLocationId] = React.useState<string>("");
    const [isEdit, setIsEdit] = React.useState<boolean>(false);
    const [isAdd, setIsAdd] = React.useState<boolean>(false);
    const [isAddEnabled, setIsAddEnabled] = React.useState<boolean>(true);

    const radios = [
        { name: 'Manage Partner Location', value: '1' },
        { name: 'Manage Partner Location Contacts', value: '2' },
        { name: 'Manage Partner Location Services', value: '3' },
        { name: 'Manage Event Requests', value: '4' }
    ];

    React.useEffect(() => {

        if (props.isUserLoaded) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnerlocations/getbypartner/' + props.partnerId, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<PartnerLocationData[]>)
                    .then(data => {
                        setPartnerLocations(data);
                        setIsPartnerLocationDataLoaded(true);
                    });
            });
        }
    }, [props.currentUser, props.isUserLoaded, props.partnerId]);

    function removeLocation(locationId: string, name: string) {
        if (!window.confirm("Please confirm that you want to remove Location with name: '" + name + "' as a location from this Partner?"))
            return;
        else {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('DELETE');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnerlocations/' + props.partnerId + '/' + locationId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        setIsPartnerLocationDataLoaded(false);

                        fetch('/api/partnerlocations/getbypartner/' + props.partnerId, {
                            method: 'GET',
                            headers: headers,
                        })
                            .then(response => response.json() as Promise<PartnerLocationData[]>)
                            .then(data => {
                                setPartnerLocations(data);
                                setIsPartnerLocationDataLoaded(true);
                            });
                    })
            });
        }
    }

    function addLocation() {
        setPartnerLocationId(Guid.EMPTY);
        setIsAdd(true);
        setIsAddEnabled(false);
    }

    // This will handle Cancel button click event.
    function handleCancel() {
        setPartnerLocationId(Guid.EMPTY);
        setIsAdd(false);
        setIsEdit(false);
        setIsAddEnabled(true);
    }

    // This will handle Save button click event.
    function handleSave() {
        setPartnerLocationId(Guid.EMPTY);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerlocations/getbypartner/' + props.partnerId, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<PartnerLocationData[]>)
                .then(data => {
                    setPartnerLocations(data);
                    setIsPartnerLocationDataLoaded(true);
                    setIsAdd(false);
                    setIsEdit(false);
                    setIsAddEnabled(true);
                });
        });
    }

    function editLocation(partnerLocationId: string) {
        setPartnerLocationId(partnerLocationId);
        setIsEdit(true);
        setIsAddEnabled(false);
    }

    function renderPartnerLocationsTable(locations: PartnerLocationData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                        </tr>
                    </thead>
                    <tbody>
                        {locations.map(location =>
                            <tr key={location.id.toString()}>
                                <td>{location.name}</td>
                                <td>{location.city}</td>
                                <td>{location.region}</td>
                                <td>{location.country}</td>
                                <td>
                                    <Button className="action" onClick={() => editLocation(location.id)}>Edit Location</Button>
                                    <Button className="action" onClick={() => removeLocation(location.id, location.name)}>Remove Location</Button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button disabled={!isAddEnabled} className="action" onClick={() => addLocation()}>Add Location</Button>
            </div>
        );
    }

    function renderEditPartnerLocation() {
        return (
            <div>
                <PartnerLocationEdit partnerId={props.partnerId} partnerLocationId={partnerLocationId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} onCancel={handleCancel} onSave={handleSave} />
            </div >
        )
    }

    function renderPartnerLocationContacts() {
        return (
            <div>
                <PartnerLocationContacts partnerLocationId={partnerLocationId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderPartnerLocationServices() {
        return (
            <div>
                <PartnerLocationServices partnerLocationId={partnerLocationId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderPartnerLocationEventRequests() {
        return (
            <div>
                <PartnerLocationEventRequests partnerLocationId={partnerLocationId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderPartnerLocationDashboard() {
        return (
            <div className="card pop">
                <ButtonGroup>
                    {radios.map((radio, idx) => (
                        <ToggleButton
                            key={idx}
                            id={`radio-${idx}`}
                            type="radio"
                            variant={idx % 2 ? 'outline-success' : 'outline-danger'}
                            name="radio"
                            value={radio.value}
                            checked={radioValue === radio.value}
                            onChange={(e) => setRadioValue(e.currentTarget.value)}
                        >
                            {radio.name}
                        </ToggleButton>
                    ))}
                </ButtonGroup>

                {radioValue === '1' && renderEditPartnerLocation()}
                {radioValue === '2' && renderPartnerLocationContacts()}
                {radioValue === '3' && renderPartnerLocationServices()}
                {radioValue === '4' && renderPartnerLocationEventRequests()}
            </div>);
    }

    return (
        <>
            <div>
                {!isPartnerLocationDataLoaded && <p><em>Loading...</em></p>}
                {isPartnerLocationDataLoaded && partnerLocations && renderPartnerLocationsTable(partnerLocations)}
                {(isEdit || isAdd) && renderPartnerLocationDashboard() }
            </div>
        </>
    );
}