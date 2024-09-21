import React, { FC, useState } from 'react';
import { Button } from 'react-bootstrap';
import { RouteComponentProps } from 'react-router-dom';
import { getApiConfig, msalClient } from '../../store/AuthStore';
import EventAttendeeData from '../Models/EventAttendeeData';
import UserData from '../Models/UserData';
import { DisplayEvent } from '../MainEvents';
import { CurrentTrashMobWaiverVersion } from '../Waivers/Waivers';

import WaiverData from '../Models/WaiverData';

interface RegisterBtnProps extends RouteComponentProps {
    currentUser: UserData;
    eventId: DisplayEvent['id'];
    isAttending: DisplayEvent['isAttending'];
    isUserLoaded: boolean;
    isEventCompleted: boolean;
    onAttendanceChanged: any;
    addEventAttendee: any;
    waiverData: any;
}

export const RegisterBtn: FC<RegisterBtnProps> = ({
    currentUser,
    eventId,
    isAttending,
    isUserLoaded,
    isEventCompleted,
    onAttendanceChanged,
    history,
    waiverData,
    addEventAttendee,
}) => {
    const [registered, setRegistered] = useState<boolean>(false);
    const [waiver] = useState<WaiverData>(waiverData);

    const addAttendee = async (eventId: string) => {
        const body = new EventAttendeeData();
        body.userId = currentUser.id;
        body.eventId = eventId;

        await addEventAttendee.mutateAsync(body);
        await onAttendanceChanged(eventId);
        setRegistered(true);
        history.push(`/eventdetails/${eventId}`); // re-direct user to event details page once they are registered
    };

    const handleAttend = (eventId: string) => {
        // Have user sign waiver if needed
        const isTrashMobWaiverOutOfDate =
            new Date(currentUser.dateAgreedToTrashMobWaiver).toISOString() <
            CurrentTrashMobWaiverVersion.versionDate.toISOString();
        if (waiver?.isWaiverEnabled && (isTrashMobWaiverOutOfDate || currentUser.trashMobWaiverVersion === '')) {
            sessionStorage.setItem('targetUrl', window.location.pathname);
            history.push('/waivers');
        }

        const accounts = msalClient.getAllAccounts();

        if (accounts === null || accounts.length === 0) {
            const apiConfig = getApiConfig();
            msalClient
                .loginRedirect({
                    scopes: apiConfig.b2cScopes,
                })
                .then(() => {
                    addAttendee(eventId);
                });
        } else {
            addAttendee(eventId);
        }
    };

    return (
        <Button
            id='addAttendee'
            className='btn btn-primary action btn-128'
            hidden={!isUserLoaded || isAttending === 'Yes' || registered || isEventCompleted}
            onClick={() => handleAttend(eventId)}
        >
            {registered ? 'Attended!' : 'Attend'}
        </Button>
    );
};
