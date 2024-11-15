import * as React from 'react';
import { Guid } from 'guid-typescript';
import { Container, Dropdown } from 'react-bootstrap';
import { ThreeDots } from 'react-bootstrap-icons';
import { useQuery } from '@tanstack/react-query';
import UserData from '../Models/UserData';
import { GetEventAttendees } from '../../services/events';
import { Services } from '../../config/services.config';

export interface ManageEventAttendeesProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const ManageEventAttendees: React.FC<ManageEventAttendeesProps> = (props) => {
    // const [isOpen, setIsOpen] = React.useState(false);
    // const [modalTitle, setModalTitle] = React.useState("")
    // const [modalDescription, setModalDescription] = React.useState("")
    const [eventAttendees, setEventAttendees] = React.useState<UserData[]>([]);
    const [isEventAttendeeDataLoaded, setIsEventAttendeeDataLoaded] = React.useState<boolean>(false);

    // const togglemodal = () => {
    //    setIsOpen(!isOpen);
    // }
    // const messageToAttendee = () => {
    //    togglemodal()
    //    setModalTitle("Message to [username here]")
    //    setModalDescription("Maybe I want to send a message to a specific person about something idk. Well that message would be sent here!")
    // }

    // const removeToAttendee = () => {
    //    togglemodal()
    //    setModalTitle("Remove [username]?")
    //    setModalDescription("Are you sure you want to remove attendee [username]? They will be removed from the attendees list for this event and will not receive any event updates or emails. This action cannot be undone.")
    // }
    // const messageToAll = () => {
    //    togglemodal()
    //    setModalTitle("Message all attendees")
    //    setModalDescription("Maybe I want to send a specific message out to all the attendees. Maybe it’s reminding them of what clothes to wear, that the event time changed, etc. ")
    // }

    const getEventAttendees = useQuery({
        queryKey: GetEventAttendees({ eventId: props.eventId }).key,
        queryFn: GetEventAttendees({ eventId: props.eventId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    React.useEffect(() => {
        if (props.isUserLoaded && props.eventId && props.eventId !== Guid.EMPTY) {
            getEventAttendees.refetch().then((res) => {
                setEventAttendees(res.data?.data || []);
                setIsEventAttendeeDataLoaded(true);
            });
        }
    }, [props.eventId, props.isUserLoaded]);

    function renderEventAttendeesTable(users: UserData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby='tableLabel'>
                    <thead>
                        <tr>
                            <th className='h5 py-4'>User Name</th>
                            <th className='h5 py-4'>City</th>
                            <th className='h5 py-4'>Country</th>
                            <th className='h5 py-4'>Join date</th>
                            <th className='h5 py-4'>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map((user) => (
                            <tr key={user.id.toString()}>
                                <td className='color-grey p-18 py-3'>{user.userName}</td>
                                <td className='color-grey p-18 py-3'>{user.city}</td>
                                <td className='color-grey p-18 py-3'>{user.country}</td>
                                <td className='color-grey p-18 py-3'>
                                    {new Date(user.memberSince).toLocaleDateString()}
                                </td>
                                <td className='color-grey p-18 py-3'>
                                    <Dropdown>
                                        <Dropdown.Toggle id='userBtn' variant='light' className='remove-drop-icon'>
                                            <ThreeDots size={24} color='#696B72' />
                                        </Dropdown.Toggle>
                                        {/*    <Dropdown.Menu className="shadow border-0"> */}
                                        {/*        <Dropdown.Item eventKey="1" onClick={() => messageToAttendee()}><Envelope aria-hidden="true" color='#96ba00' size={24} className="mr-2" /><span className='color-grey p-18'>Message attendee</span></Dropdown.Item> */}
                                        {/*        <Dropdown.Divider /> */}
                                        {/*        <Dropdown.Item eventKey="2" onClick={() => removeToAttendee()}><PersonX aria-hidden="true" color='#96ba00' size={24} className="mr-2" /><span className='color-grey p-18'>Remove attendee</span></Dropdown.Item> */}
                                        {/*    </Dropdown.Menu> */}
                                    </Dropdown>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        );
    }

    return (
        <Container className='p-4 bg-white rounded my-5'>
            <div className='d-flex align-items-center justify-content-between'>
                <h4 className='fw-600 color-primary my-4'>Attendees ({eventAttendees?.length})</h4>
                {/*    <div className='d-flex align-items-center' onClick={() => messageToAll()} role="button"> */}
                {/*        <Envelope aria-hidden="true" size={24} color="#96ba00" /> */}
                {/*        <div className='p-18 color-primary ml-2'>Message all</div> */}
                {/*    </div> */}
            </div>
            {props.eventId === Guid.EMPTY && (
                <p>
                    {' '}
                    <em>Event must be created first.</em>
                </p>
            )}
            {!isEventAttendeeDataLoaded && props.eventId !== Guid.EMPTY && (
                <p>
                    <em>Loading...</em>
                </p>
            )}
            {isEventAttendeeDataLoaded ? renderEventAttendeesTable(eventAttendees) : null}
        </Container>
    );
};
