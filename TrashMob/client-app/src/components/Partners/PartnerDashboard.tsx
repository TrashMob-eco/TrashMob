import * as React from 'react'
import { RouteComponentProps, withRouter } from 'react-router-dom';
import UserData from '../Models/UserData';
import { PartnerEdit } from './PartnerEdit';
import { PartnerUsers } from './PartnerUsers';
import { PartnerLocations } from './PartnerLocations';
import { ButtonGroup, ToggleButton } from 'react-bootstrap';
import { PartnerEventRequests } from './PartnerEventRequests';
import { Guid } from 'guid-typescript';

export interface PartnerDashboardMatchParams {
    partnerId?: string;
}

export interface PartnerDashboardProps extends RouteComponentProps<PartnerDashboardMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const PartnerDashboard: React.FC<PartnerDashboardProps> = (props) => {
    const [radioValue, setRadioValue] = React.useState('1');
    const [partnerId, setPartnerId] = React.useState<string>("");
    const [isPartnerIdReady, setIsPartnerIdReady] = React.useState<boolean>();
    const [loadedPartnerId, setLoadedPartnerId] = React.useState<string | undefined>(props.match?.params["partnerId"]);

    const radios = [
        { name: 'Manage Partner', value: '1' },
        { name: 'Manage Partner Users', value: '2' },
        { name: 'Manage Partner Locations', value: '3' },
        { name: 'Manage Event Requests', value: '4' }
    ];

    React.useEffect(() => {
        var partId = loadedPartnerId;
        if (!partId) {
            setPartnerId(Guid.createEmpty().toString());
            setLoadedPartnerId(Guid.createEmpty().toString())
        }
        else {
            setPartnerId(partId);
        }

        setIsPartnerIdReady(true);

    }, [loadedPartnerId]);


    function renderEditPartner() {
        return (
            <div>
                <PartnerEdit partnerId={partnerId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderPartnerUsers() {
        return (
            <div>
                <PartnerUsers partnerId={partnerId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div>
        )
    }

    function renderPartnerLocations() {
        return (
            <div>
                <PartnerLocations partnerId={partnerId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderEventRequests() {
        return (
            <div>
                <PartnerEventRequests partnerId={partnerId}  currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderPartnerDashboard() {
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

                { radioValue === '1' && renderEditPartner()}
                { radioValue === '2' && renderPartnerUsers()}
                { radioValue === '3' && renderPartnerLocations()}
                { radioValue === '4' && renderEventRequests()}
            </div>);
    }

    let contents = isPartnerIdReady
        ? renderPartnerDashboard()
        : <p><em>Loading...</em></p>;

    return <div>
        <hr />
        {contents}
    </div>;
}

export default withRouter(PartnerDashboard);