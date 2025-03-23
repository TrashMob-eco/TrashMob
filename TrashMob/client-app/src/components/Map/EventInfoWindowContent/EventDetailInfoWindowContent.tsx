import { useGetEventType } from '../../../hooks/useGetEventType';
import { RegisterBtn } from '../../Customization/RegisterBtn';
import EventData from '../../Models/EventData';
import UserData from '../../Models/UserData';

type EventDetailInfoWindowContentProps = {
    event: EventData & {
        isAttending?: boolean;
    };
    hideTitle?: boolean;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const EventDetailInfoWindowHeader = (props: EventData) => (
    <h5 className='mt-1 font-bold' style={{ fontFamily: 'Poppins' }}>
        {props.name}
    </h5>
);

export const EventDetailInfoWindowContent = (props: EventDetailInfoWindowContentProps) => {
    const { event, hideTitle, isUserLoaded, currentUser } = props;
    const {
        id,
        name,
        eventTypeId,
        eventDate,
        city,
        region,
        country,
        postalCode,
        createdByUserName,
        isAttending = '',
    } = event;

    const { data: eventType } = useGetEventType(eventTypeId);

    const date = new Date(eventDate).toLocaleDateString([], {
        month: 'long',
        day: '2-digit',
        year: 'numeric',
    });
    const time = new Date(eventDate).toLocaleTimeString([], {
        timeZoneName: 'short',
    });

    return (
        <div style={{ width: 500, overflowX: 'auto' }}>
            <div>
                {!hideTitle && <h5 className='mt-1 font-bold'>{name}</h5>}
                <p className='my-3 event-list-event-type p-2 rounded'>{eventType?.name}</p>
                <p className='m-0'>
                    {date},{time}
                </p>
                <p className='m-0'>
                    {city ? `${city},` : ''} {region ? `${region},` : ''} {country ? `${country},` : ''}{' '}
                    {postalCode || ''}
                </p>
            </div>
            <div className='d-flex justify-content-between mt-2 tailwind'>
                <span className='align-self-end'>
                    Created by
                    {createdByUserName}
                </span>
                <button className='btn btn-outline mr-0'>
                    <a href={`/eventdetails/${id}`}>View Details</a>
                </button>
                <RegisterBtn
                    eventId={id}
                    isAttending={isAttending ? 'Yes' : 'No'}
                    isEventCompleted={new Date(eventDate) < new Date()}
                    currentUser={currentUser}
                    isUserLoaded={isUserLoaded}
                />
            </div>
        </div>
    );
};
