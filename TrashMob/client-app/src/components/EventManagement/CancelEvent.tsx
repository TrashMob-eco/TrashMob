import * as React from 'react';
import OverlayTrigger from 'react-bootstrap/OverlayTrigger';
import Tooltip from 'react-bootstrap/Tooltip';
import { Button, Col, Container, Form } from 'react-bootstrap';
import { RouteComponentProps } from 'react-router-dom';
import { useMutation, useQuery } from '@tanstack/react-query';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import * as ToolTips from '../../store/ToolTips';
import { SocialsModal } from './ShareToSocialsModal';
import * as SharingMessages from '../../store/SharingMessages';
import { DeleteEvent, GetEventById } from '../../services/events';
import { Services } from '../../config/services.config';

export interface CancelEventMatchParams {
    eventId: string;
}

export interface CancelEventProps extends RouteComponentProps<CancelEventMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const CancelEvent: React.FC<CancelEventProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [eventId, setEventId] = React.useState<string>(props.match.params.eventId);
    const [eventName, setEventName] = React.useState<string>('New Event');
    const [cancellationReason, setCancellationReason] = React.useState<string>('');
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [showModal, setShowSocialsModal] = React.useState<boolean>(false);
    const [eventToShare, setEventToShare] = React.useState<EventData>();

    const getEventById = useQuery({
        queryKey: GetEventById({ eventId }).key,
        queryFn: GetEventById({ eventId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const deleteEvent = useMutation({
        mutationKey: DeleteEvent().key,
        mutationFn: DeleteEvent().service,
    });

    React.useEffect(() => {
        getEventById.refetch().then((res) => {
            if (res.data === undefined) return;
            setEventId(res.data.data.id);
            setEventName(res.data.data.name);
            setEventToShare(res.data.data);
            setIsDataLoaded(true);
        });
    }, [eventId]);

    React.useEffect(() => {
        if (cancellationReason === '') {
            setIsSaveEnabled(false);
        } else {
            setIsSaveEnabled(true);
        }
    }, [cancellationReason]);

    function handleCancellationReasonChanged(val: string) {
        setCancellationReason(val);
    }

    function renderCancellationReasonToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.EventCancellationReason}</Tooltip>;
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.goBack();
    }

    // This will handle the submit form event.
    function handleSave(event: any) {
        event.preventDefault();
        if (!isSaveEnabled) return;
        setIsSaveEnabled(false);
        return deleteEvent.mutateAsync({ eventId, cancellationReason }).then((res) => {
            if (eventToShare) handleShowModal(true);
        });
    }

    // Returns the HTML Form to the render() method.
    function renderCancelForm() {
        return (
            <div className='container-fluid'>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <input type='hidden' name='Id' value={eventId.toString()} />
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group className='required'>
                                <OverlayTrigger placement='top' overlay={renderCancellationReasonToolTip}>
                                    <Form.Label className='control-label font-weight-bold h5' htmlFor='Name'>
                                        Reason for Cancelling the Event?
                                    </Form.Label>
                                </OverlayTrigger>
                                <Form.Control
                                    type='text'
                                    name='cancellationReason'
                                    defaultValue={cancellationReason}
                                    onChange={(val) => handleCancellationReasonChanged(val.target.value)}
                                    maxLength={parseInt('200')}
                                    required
                                />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <h4>Are you sure you want to cancel the Event {eventName}?</h4>
                    </Form.Row>
                    <Form.Row>
                        <Form.Group>
                            <Button disabled={!isSaveEnabled} type='submit' className='btn btn-default'>
                                Yes
                            </Button>
                            <Button className='action' onClick={(e: any) => handleCancel(e)}>
                                No
                            </Button>
                        </Form.Group>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    const handleShowModal = (showModal: boolean) => {
        // if modal is being dismissed, route user back to previous page
        if (!showModal) props.history.goBack();
        else setShowSocialsModal(showModal);
    };

    const contents =
        isDataLoaded && eventId ? (
            renderCancelForm()
        ) : (
            <p>
                <em>Loading...</em>
            </p>
        );

    return (
        <div>
            <Container className='p-4 bg-white rounded my-5'>
                {isDataLoaded ? (
                    <SocialsModal
                        eventToShare={eventToShare}
                        show={showModal}
                        handleShow={handleShowModal}
                        modalTitle='Share Event Cancellation'
                        message={SharingMessages.getCancellationMessage(eventToShare, cancellationReason)}
                        eventLink='https://www.trashmob.eco'
                        emailSubject='TrashMob Event Cancellation'
                    />
                ) : null}
                <h2>Cancel Event</h2>
                <hr />
                {contents}
            </Container>
        </div>
    );
};
