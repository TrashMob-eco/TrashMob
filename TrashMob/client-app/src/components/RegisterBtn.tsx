
import { FC, useState } from 'react';
import { Button } from 'react-bootstrap';
import { getApiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import EventAttendeeData from './Models/EventAttendeeData';
import UserData from './Models/UserData';
import { DisplayEvent } from './MainEvents';
import { RouteComponentProps } from 'react-router-dom';
import { CurrentTrashMobWaiverVersion } from './Waivers/Waivers';
import React from 'react';
import WaiverData from './Models/WaiverData';

interface RegisterBtnProps extends RouteComponentProps {
    currentUser: UserData;
    eventId: DisplayEvent["id"];
    isAttending: DisplayEvent["isAttending"];
    isUserLoaded: boolean;
    onAttendanceChanged: any;
};

export const RegisterBtn: FC<RegisterBtnProps> = ({ currentUser, eventId, isAttending, isUserLoaded, onAttendanceChanged, history }) => {
    const [registered, setRegistered] = useState<boolean>(false);
    const [waiver, setWaiver] = useState<WaiverData>();

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');

        fetch('/api/waivers/trashmob', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<WaiverData>)
            .then(data => {
                setWaiver(data);
            })
    }, [])

    const addAttendee = (eventId: string) => {
        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

        const request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {

            const eventAttendee = new EventAttendeeData();
            eventAttendee.userId = currentUser.id;
            eventAttendee.eventId = eventId;

            const data = JSON.stringify(eventAttendee);

            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            // POST request for Add EventAttendee.  
            fetch('/api/eventattendees', {
                method: 'POST',
                body: data,
                headers: headers,
            }).then((response) => response.json())
                .then(onAttendanceChanged(eventId)).then(() => setRegistered(true))
        })
    }

    const handleAttend = (eventId: string) => {

        // Have user sign waiver if needed
        const isTrashMobWaiverOutOfDate = currentUser.dateAgreedToTrashMobWaiver < CurrentTrashMobWaiverVersion.versionDate;
        if (waiver?.isWaiverEnabled && (isTrashMobWaiverOutOfDate || (currentUser.trashMobWaiverVersion === ""))) {
            sessionStorage.setItem('targetUrl', window.location.pathname);
            history.push("/waivers");
        }

        const accounts = msalClient.getAllAccounts();

        if (accounts === null || accounts.length === 0) {
            msalClient.loginRedirect().then(() => {
                addAttendee(eventId);
            })
        }
        else {
            addAttendee(eventId);
        }
    }

    return (
        <Button className="btn btn-primary action btn-128" hidden={!isUserLoaded || isAttending === "Yes" || registered}
            onClick={() => handleAttend(eventId)}>{registered ? 'Registered!' : 'Register'}</Button>
    )
}
