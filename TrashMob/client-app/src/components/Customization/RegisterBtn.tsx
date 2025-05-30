import { FC, useState } from 'react';
import { Button } from '@/components/ui/button';
import { useNavigate } from 'react-router';
import { getApiConfig, msalClient } from '../../store/AuthStore';
import EventAttendeeData from '../Models/EventAttendeeData';
import UserData from '../Models/UserData';
import { CurrentTrashMobWaiverVersion } from '../../pages/waivers/page';

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { GetTrashMobWaivers } from '../../services/waivers';
import { AddEventAttendee, GetAllEventsBeingAttendedByUser } from '../../services/events';
import { cn } from '@/lib/utils';

interface RegisterBtnProps {
    currentUser: UserData;
    eventId: string;
    isAttending: string;
    isUserLoaded: boolean;
    isEventCompleted: boolean;
}

export const RegisterBtn: FC<RegisterBtnProps> = ({
    currentUser,
    eventId,
    isAttending,
    isUserLoaded,
    isEventCompleted,
}) => {
    const userId = currentUser.id;
    const navigate = useNavigate();
    const [registered, setRegistered] = useState<boolean>(false);
    const queryClient = useQueryClient();

    const { data: waiver } = useQuery({
        queryKey: GetTrashMobWaivers().key,
        queryFn: GetTrashMobWaivers().service,
        select: (res) => res.data,
    });

    const addEventAttendee = useMutation({
        mutationKey: AddEventAttendee().key,
        mutationFn: AddEventAttendee().service,
        onSuccess: () => {
            // Invalidate user's list of attended events, triggerring refetch
            queryClient.invalidateQueries(GetAllEventsBeingAttendedByUser({ userId }).key);

            // re-direct user to event details page once they are registered
            navigate(`/eventdetails/${eventId}`);
        },
    });

    const addAttendee = async (eventId: string) => {
        const body = new EventAttendeeData();
        body.userId = currentUser.id;
        body.eventId = eventId;

        await addEventAttendee.mutateAsync(body);
        setRegistered(true);
    };

    const handleAttend = (eventId: string) => {
        // Have user sign waiver if needed
        const isTrashMobWaiverOutOfDate =
            new Date(currentUser.dateAgreedToTrashMobWaiver).toISOString() <
            CurrentTrashMobWaiverVersion.versionDate.toISOString();
        if (waiver?.isWaiverEnabled && (isTrashMobWaiverOutOfDate || currentUser.trashMobWaiverVersion === '')) {
            sessionStorage.setItem('targetUrl', window.location.pathname);
            navigate('/waivers');
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
            className={cn({
                hidden: !isUserLoaded || isAttending === 'Yes' || registered || isEventCompleted,
            })}
            onClick={() => handleAttend(eventId)}
        >
            {registered ? 'Attended!' : 'Attend'}
        </Button>
    );
};
