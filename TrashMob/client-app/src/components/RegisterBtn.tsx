
import { FC } from 'react';
import { Button } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import EventAttendeeData from './Models/EventAttendeeData';
import UserData from './Models/UserData';
import { DisplayEvent } from './MainEvents';

interface RegisterBtnProps {
    currentUser: UserData;
    eventId: DisplayEvent["id"];
    isAttending: DisplayEvent["isAttending"];
    isUserLoaded: boolean;
    onAttendanceChanged: any;
};

export const RegisterBtn: FC<RegisterBtnProps> = ({ currentUser, eventId, isAttending, isUserLoaded, onAttendanceChanged }) => {
    console.log('cur', currentUser, eventId, 'isAt', isAttending, 'isUsr', isUserLoaded, onAttendanceChanged)
    const addAttendee = (eventId: string) => {
        const account = msalClient.getAllAccounts()[0];

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
                .then(onAttendanceChanged(eventId))
        })
    }

    const handleAttend = (eventId: string) => {
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
        <Button className="btn btn-primary action btn-128" hidden={!isUserLoaded || isAttending === "Yes"}
            onClick={() => handleAttend(eventId)}>Register</Button>
    )
}
