import * as React from 'react'

import { RouteComponentProps, withRouter } from 'react-router-dom';
import UserData from '../Models/UserData';
import { ButtonGroup, ToggleButton } from 'react-bootstrap';
import { EditEvent } from './EditEvent';
import { ManageEventSummary } from './ManageEventSummary';
import { ManageEventPartners } from './ManageEventPartners';
import { ManageEventMedia } from './ManageEventMedia';
import { ManageEventAttendees } from './ManageEventAttendees';
import { Guid } from 'guid-typescript';
import { Button } from 'reactstrap';

export interface ManageEventDashboardMatchParams {
    eventId?: string;
}

export interface ManageEventDashboardProps extends RouteComponentProps<ManageEventDashboardMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const ManageEventDashboard: React.FC<ManageEventDashboardProps> = (props) => {
    const [radioValue, setRadioValue] = React.useState('1');
    const [eventId, setEventId] = React.useState<string>("");
    const [isEventIdReady, setIsEventIdReady] = React.useState<boolean>();
    const [loadedEventId, setLoadedEventId] = React.useState<string | undefined>(props.match?.params["eventId"]);

    const radios = [
        { name: 'Manage Event', value: '1' },
        { name: 'Manage Event Partners', value: '2' },
        { name: 'Manage Event Attendees', value: '3' },
    //    { name: 'Manage Event Media', value: '4' },
    //    { name: 'Manage Event Summary', value: '5' },
    ];

    React.useEffect(() => {
        var evId = loadedEventId;
        if (!evId) {
            setEventId(Guid.createEmpty().toString());
            setLoadedEventId(Guid.createEmpty().toString())
        }
        else {
            setEventId(evId);
        }

        setIsEventIdReady(true);
    }, [loadedEventId]);

    function handleEditCancel() {
        props.history.push("/mydashboard");
    }

    function handleEditSave() {
        props.history.push("/mydashboard");
    }

    function handleBackToDashboard() {
        props.history.push("/mydashboard");
    }

    function renderManageEvent() {

        return (
            <div className="card pop">
                <div>
                    <h2>Event Details</h2>
                    <div>
                        <EditEvent eventId={eventId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} onEditCancel={handleEditCancel} onEditSave={handleEditSave} />
                    </div>
                </div>
            </div>);
    }

    function renderManageEventAttendees() {
        return (
            <div>
                <h2>Event Attendees</h2>
                <ManageEventAttendees eventId={eventId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderManageEventPartners() {
        return (
            <div>
                <h2>Event Partners</h2>
                <ManageEventPartners eventId={eventId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderManageEventMedia() {
        return (
            <div>
                <h2>Event Media</h2>
                <ManageEventMedia eventId={eventId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div>
        )
    }

    function renderManageEventSummary() {
        return (
            <div>
                <h2>Post Event Summary</h2>
                <ManageEventSummary eventId={eventId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderEventDashboard() {
        return (
            <div className="card pop">
                <ButtonGroup>
                    {radios.map((radio, idx) => (
                        <ToggleButton
                            key={idx}
                            id={`radio-${idx}`}
                            type="radio"
                            variant={idx % 2 ? 'outline-success' : 'outline-danger'}
                            name="radio"
                            value={radio.value}
                            checked={radioValue === radio.value}
                            onChange={(e) => setRadioValue(e.currentTarget.value)}
                        >
                            {radio.name}
                        </ToggleButton>
                    ))}
                </ButtonGroup>
                <Button className="action" onClick={() => handleBackToDashboard()}>Return to My Dashboard</Button>

                { radioValue === '1' && renderManageEvent()}
                { radioValue === '2' && renderManageEventPartners()}
                { radioValue === '3' && renderManageEventAttendees()}
                { radioValue === '4' && renderManageEventMedia()}
                { radioValue === '5' && renderManageEventSummary()}
            </div>);
    }

    let contents = isEventIdReady
        ? renderEventDashboard()
        : <p><em>Loading...</em></p>;

    return <div>
        <h3>Manage Event</h3>
        <hr />
        {contents}
    </div>;
}

export default withRouter(ManageEventDashboard);