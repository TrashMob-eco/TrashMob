import * as React from 'react'
import { RouteComponentProps, withRouter } from 'react-router-dom';
import UserData from '../Models/UserData';
import { PartnerEdit } from './PartnerEdit';
import { PartnerAdmins } from './PartnerAdmins';
import { PartnerLocations } from './PartnerLocations';
import { ButtonGroup, ToggleButton } from 'react-bootstrap';
import { Guid } from 'guid-typescript';
import { PartnerDocuments } from './PartnerDocuments';
import { PartnerSocialMediaAccounts } from './PartnerSocialMediaAccounts';
import { PartnerContacts } from './PartnerContacts';

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
        { name: 'Manage Partner Locations', value: '2' },
        { name: 'Manage Partner Contacts', value: '3' },
        { name: 'Manage Partner Admins', value: '4' },
        { name: 'Manage Partner Documents', value: '5' },
        { name: 'Manage Partner Social Media Accounts', value: '6' },
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

    function renderPartnerAdmins() {
        return (
            <div>
                <PartnerAdmins partnerId={partnerId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
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

    function renderPartnerContacts() {
        return (
            <div>
                <PartnerContacts partnerId={partnerId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderPartnerDocuments() {
        return (
            <div>
                <PartnerDocuments partnerId={partnerId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderPartnerSocialMediaAccounts() {
        return (
            <div>
                <PartnerSocialMediaAccounts partnerId={partnerId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
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
                { radioValue === '2' && renderPartnerLocations()}
                { radioValue === '3' && renderPartnerContacts()}
                { radioValue === '4' && renderPartnerAdmins()}
                { radioValue === '5' && renderPartnerDocuments()}
                { radioValue === '6' && renderPartnerSocialMediaAccounts()}
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