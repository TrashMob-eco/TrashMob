import { FC, useState } from 'react';
import { Button } from '@/components/ui/button';
import { useNavigate } from 'react-router';
import { getApiConfig, getMsalClientInstance } from '../../store/AuthStore';
import EventAttendeeData from '../Models/EventAttendeeData';
import UserData from '../Models/UserData';
import { WaiverSigningFlow } from '@/components/Waivers';

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { GetRequiredWaiversForEvent } from '../../services/user-waivers';
import { AddEventAttendee, GetAllEventsBeingAttendedByUser } from '../../services/events';
import { cn } from '@/lib/utils';
import { useFeatureMetrics } from '@/hooks/useFeatureMetrics';

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
    const [showWaiverFlow, setShowWaiverFlow] = useState<boolean>(false);
    const queryClient = useQueryClient();
    const { trackAttendance } = useFeatureMetrics();

    // Fetch required waivers for this event
    const { data: requiredWaivers, refetch: refetchWaivers } = useQuery({
        queryKey: GetRequiredWaiversForEvent({ eventId }).key,
        queryFn: GetRequiredWaiversForEvent({ eventId }).service,
        select: (res) => res.data,
        enabled: isUserLoaded && !!userId,
    });

    const addEventAttendee = useMutation({
        mutationKey: AddEventAttendee().key,
        mutationFn: AddEventAttendee().service,
        onSuccess: () => {
            // Track attendance registration
            trackAttendance('Register', eventId);

            // Invalidate user's list of attended events, triggering refetch
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
        const accounts = getMsalClientInstance().getAllAccounts();

        // Check if user is logged in
        if (accounts === null || accounts.length === 0) {
            const apiConfig = getApiConfig();
            getMsalClientInstance()
                .loginRedirect({
                    scopes: apiConfig.b2cScopes,
                })
                .then(() => {
                    // After login, this component will re-render with updated data
                    // The user can then click the button again
                });
            return;
        }

        // Check if there are waivers to sign
        if (requiredWaivers && requiredWaivers.length > 0) {
            setShowWaiverFlow(true);
        } else {
            // No waivers needed, proceed with registration
            addAttendee(eventId);
        }
    };

    const handleWaiverFlowComplete = (allSigned: boolean) => {
        setShowWaiverFlow(false);
        if (allSigned) {
            // Refetch waivers to confirm they're all signed
            refetchWaivers().then(() => {
                // Proceed with registration
                addAttendee(eventId);
            });
        }
        // If not all signed, user cancelled - do nothing
    };

    return (
        <>
            <Button
                className={cn({
                    hidden: !isUserLoaded || isAttending === 'Yes' || registered || isEventCompleted,
                })}
                onClick={() => handleAttend(eventId)}
            >
                {registered ? 'Attended!' : 'Attend'}
            </Button>

            {requiredWaivers && (
                <WaiverSigningFlow
                    waivers={requiredWaivers}
                    open={showWaiverFlow}
                    onComplete={handleWaiverFlowComplete}
                />
            )}
        </>
    );
};
