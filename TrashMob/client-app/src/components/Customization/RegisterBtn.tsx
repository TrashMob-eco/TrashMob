
import { FC, useState } from 'react';
import { Button } from 'react-bootstrap';
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from '../../store/AuthStore';
import EventAttendeeData from '../Models/EventAttendeeData';
import UserData from '../Models/UserData';
import { DisplayEvent } from '../MainEvents';
import { RouteComponentProps } from 'react-router-dom';
import { CurrentTrashMobWaiverVersion } from '../Waivers/Waivers';
import React from 'react';
import WaiverData from '../Models/WaiverData';

interface RegisterBtnProps extends RouteComponentProps {
    currentUser: UserData;
    eventId: DisplayEvent["id"];
    isAttending: DisplayEvent["isAttending"];
    isUserLoaded: boolean;
    isEventCompleted: boolean;
    onAttendanceChanged: any;
};

export const RegisterBtn: FC<RegisterBtnProps> = ({ currentUser, eventId, isAttending, isUserLoaded, isEventCompleted, onAttendanceChanged, history }) => {
    const [registered, setRegistered] = useState<boolean>(false);
    const [waiver, setWaiver] = useState<WaiverData>();

    React.useEffect(() => {
        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {

            if (!validateToken(tokenResponse.idTokenClaims)) {
                return;
            }

            var method = "GET";
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/waivers/trashmob', {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<WaiverData>)
                .then(data => {
                    setWaiver(data);
                })
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

            if (!validateToken(tokenResponse.idTokenClaims)) {
                return;
            }

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
            }).then(() => onAttendanceChanged(eventId))
                .then(() => setRegistered(true))
                    .then(() => {
                        // re-direct user to event details page once they are registered
                        history.push(`/eventdetails/${eventId}`)
                    })
        })
    }

    const handleAttend = (eventId: string) => {

        // Have user sign waiver if needed
        const isTrashMobWaiverOutOfDate = (new Date(currentUser.dateAgreedToTrashMobWaiver)).toISOString() < CurrentTrashMobWaiverVersion.versionDate.toISOString();
        if (waiver?.isWaiverEnabled && (isTrashMobWaiverOutOfDate || (currentUser.trashMobWaiverVersion === ""))) {
            sessionStorage.setItem('targetUrl', window.location.pathname);
            history.push("/waivers");
        }

        const accounts = msalClient.getAllAccounts();

        if (accounts === null || accounts.length === 0) {
            var apiConfig = getApiConfig();
            msalClient.loginRedirect({
                scopes: apiConfig.b2cScopes
            }).then(() => {
                addAttendee(eventId);
            })
        }
        else {
            addAttendee(eventId);
        }
    }

    return (
        <Button id="addAttendee" className="btn btn-primary action btn-128" hidden={!isUserLoaded || isAttending === "Yes" || registered || isEventCompleted}
            onClick={() => handleAttend(eventId)}>{registered ? 'Attended!' : 'Attend'}</Button>
    )
}
