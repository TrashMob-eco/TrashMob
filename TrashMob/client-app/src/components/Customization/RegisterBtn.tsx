
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
import { useMutation, useQuery } from '@tanstack/react-query';
import { GetTrashMobWaivers } from '../../services/waivers';
import { Services } from '../../config/services.config';
import { AddEventAttendee } from '../../services/events';

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

    const getTrashMobWaivers = useQuery({ 
        queryKey: GetTrashMobWaivers().key,
        queryFn: GetTrashMobWaivers().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false
    });

    const addEventAttendee = useMutation({
        mutationKey: AddEventAttendee().key,
        mutationFn: AddEventAttendee().service
    })

    React.useEffect(() => {
        getTrashMobWaivers.refetch().then((res) => {
            setWaiver(res.data?.data)
        })
    }, [])

    const addAttendee = async (eventId: string) => {
        const body = new EventAttendeeData();
        body.userId = currentUser.id;
        body.eventId = eventId;

        await addEventAttendee.mutateAsync(body);
        await onAttendanceChanged(eventId);
        setRegistered(true)
        history.push(`/eventdetails/${eventId}`);   // re-direct user to event details page once they are registered
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
