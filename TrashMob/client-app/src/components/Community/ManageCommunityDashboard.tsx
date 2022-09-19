import * as React from 'react'
import { RouteComponentProps, withRouter } from 'react-router-dom';
import UserData from '../Models/UserData';
import { CommunityEdit } from './CommunityEdit';
import { CommunityUsers } from './CommunityUsers';
import { Button, ButtonGroup, ToggleButton } from 'react-bootstrap';
import { Guid } from 'guid-typescript';
import { CommunitySocialMediaAccounts } from './CommunitySocialMediaAccounts';
import { CommunityDocuments } from './CommunityDocuments';

export interface ManageCommunityDashboardMatchParams {
    communityId?: string;
}

export interface ManageCommunityDashboardProps extends RouteComponentProps<ManageCommunityDashboardMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const ManageCommunityDashboard: React.FC<ManageCommunityDashboardProps> = (props) => {
    const [communityId, setCommunityId] = React.useState<string>("");
    const [loadedCommunityId, setLoadedCommunityId] = React.useState<string | undefined>(props.match?.params["communityId"]);
    const [radioValue, setRadioValue] = React.useState('1');
    const [isCommunityIdReady, setIsCommunityIdReady] = React.useState<boolean>();

    const radios = [
        { name: 'Manage Community', value: '1' },
        { name: 'Manage Community Users', value: '2' },
        { name: 'Manage Social Media Accounts', value: '3' },
        { name: 'Manage Documents', value: '4' },
    ];

    React.useEffect(() => {
        var commId = loadedCommunityId;
        if (!commId) {
            setCommunityId(Guid.createEmpty().toString());
            setLoadedCommunityId(Guid.createEmpty().toString())
        }
        else {
            setCommunityId(commId);
        }

        setIsCommunityIdReady(true);
    }, [loadedCommunityId]);

    function handleEditCancel() {
        props.history.push("/managecommunitydashboard");
    }

    function handleEditSave() {
        props.history.push("/managecommunitydashboard");
    }

    function handleCommunityUsersSave() {
        props.history.push("/managecommunitydashboard");
    }

    function handleBackToDashboard() {
        props.history.push("/managecommunitydashboard");
    }

    function renderManageCommunity() {

        return (
            <div className="card pop">
                <div>
                    <h2>Community Details</h2>
                    <div>
                        <CommunityEdit communityId={communityId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} onEditCanceled={handleEditCancel} onEditSave={handleEditSave} />
                    </div>
                </div>
            </div>);
    }

    function renderCommunityUsers() {
        return (
            <div>
                <h2>Community Users</h2>
                <CommunityUsers communityId={communityId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} onCommunityUsersUpdated={handleCommunityUsersSave} />
            </div >
        )
    }

    function renderSocialMediaAccounts() {
        return (
            <div>
                <h2>Social Media Accounts</h2>
                <CommunitySocialMediaAccounts communityId={communityId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderDocuments() {
        return (
            <div>
                <h2>Documents</h2>
                <CommunityDocuments communityId={communityId} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} />
            </div >
        )
    }

    function renderCommunityDashboard() {
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

                {radioValue === '1' && renderManageCommunity()}
                {radioValue === '2' && renderCommunityUsers()}
                {radioValue === '3' && renderSocialMediaAccounts()}
                {radioValue === '4' && renderDocuments()}
            </div>);
    }

    let contents = isCommunityIdReady
        ? renderCommunityDashboard()
        : <p><em>Loading...</em></p>;

    return <div>
        <h3>Manage Community</h3>
        <hr />
        {contents}
    </div>;
}

export default withRouter(ManageCommunityDashboard);