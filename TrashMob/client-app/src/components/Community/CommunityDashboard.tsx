import * as React from 'react'
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import CommunityData from '../Models/CommunityData';
import { CommunityList } from './CommunityList';
import { CommunityEdit } from './CommunityEdit';
import CommunityStatusData from '../Models/CommunityStatusData';
import { CommunityUsers } from './CommunityUsers';
import { ButtonGroup, ToggleButton } from 'react-bootstrap';

interface CommunityDashboardProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const CommunityDashboard: React.FC<CommunityDashboardProps> = (props) => {
    const [communityList, setCommunityList] = React.useState<CommunityData[]>([]);
    const [radioValue, setRadioValue] = React.useState('1');
    const [userList, setUserList] = React.useState<UserData[]>([]);
    const [communityLocationList, setCommunityLocationList] = React.useState<CommunityLocationData[]>([]);
    const [communityStatusList, setCommunityStatusList] = React.useState<CommunityStatusData[]>([]);
    const [isCommunityDataLoaded, setIsCommunityDataLoaded] = React.useState<boolean>(false);
    const [isUserDataLoaded, setIsUserDataLoaded] = React.useState<boolean>(false);
    const [isCommunityLocationDataLoaded, setIsCommunityLocationDataLoaded] = React.useState<boolean>(false);
    const [currentUser, setCurrentUser] = React.useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);
    const [message, setMessage] = React.useState<string>("Loading...");
    const [selectedCommunityId, setSelectedCommunityId] = React.useState<string>("");
    const [selectCommunityMessage, setSelectCommunityMessage] = React.useState<string>("");
    const [isSelectedCommunityDataLoaded, setIsSelectedCommunityDataLoaded] = React.useState<boolean>(false);
    const [selectedCommunity, setSelectedCommunity] = React.useState<CommunityData>(new CommunityData());

    const radios = [
        { name: 'Manage Community', value: '1' },
        { name: 'Manage Community Users', value: '2' },
        { name: 'Manage Community Locations', value: '3' },
        { name: 'Manage Event Requests', value: '4' },
    //    { name: 'View Executive Summary', value: '5' },
    ];

    React.useEffect(() => {
        setCurrentUser(props.currentUser);
        setIsUserLoaded(props.isUserLoaded);

        if (props.isUserLoaded) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/communitystatuses', {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<CommunityStatusData[]>)
                    .then(data => {
                        setCommunityStatusList(data)
                    })
                    .then(() => {
                        fetch('/api/communityusers/getcommunitiesforuser/' + props.currentUser.id, {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => {
                                if (response.ok) {
                                    return response.json() as Promise<CommunityData[]>
                                }
                                else {
                                    throw new Error("No Communitys found for this user");
                                }
                            })
                            .then(data => {
                                setCommunityList(data);
                                setIsCommunityDataLoaded(true);
                                setSelectedCommunityId("");

                                if (data.length === 0) {
                                    setSelectCommunityMessage("No Communitys found for this user")
                                }
                                else {
                                    setSelectCommunityMessage("Select a community to view / edit details");
                                }
                                return;
                            });
                    })
                    .catch(_ => {
                        setMessage("Your id does not map to an existing community.")
                        setIsCommunityDataLoaded(false);
                        setSelectedCommunityId("");
                        setSelectCommunityMessage("");
                    });
            });
        }
    }, [props.currentUser, props.isUserLoaded]);

    function loadCommunity(communityId: string) {
        if (props.isUserLoaded) {
            setIsSelectedCommunityDataLoaded(false);
            setSelectedCommunityId(communityId);
            setSelectCommunityMessage("Loading");

            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/communities/' + communityId, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<CommunityData>)
                    .then(data => {
                        setSelectedCommunity(data);
                        setIsSelectedCommunityDataLoaded(true);
                    })
                    .then(() => {
                        fetch('/api/communityusers/users/' + communityId, {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => {
                                if (response.ok) {
                                    return response.json() as Promise<UserData[]>
                                }
                                else {
                                    throw new Error("No Community Users found for this community");
                                }
                            })
                            .then(data => {
                                setUserList(data);
                                setIsUserDataLoaded(true);
                                return;
                            })
                            .catch(_ => {
                                setIsUserDataLoaded(false);
                                setUserList([]);
                            });
                    });
            })
        }
    };

    function handleCommunityUpdated() {

        setIsCommunityDataLoaded(false);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/communityusers/getcommunitesforuser/' + props.currentUser.id, {
                method: 'GET',
                headers: headers
            })
                .then(response => {
                    if (response.ok) {
                        return response.json() as Promise<CommunityData[]>
                    }
                    else {
                        throw new Error("No Communitys found for this user");
                    }
                })
                .then(data => {
                    setCommunityList(data);
                    setIsCommunityDataLoaded(true);
                    setSelectedCommunityId("");
                    setSelectCommunityMessage("Select a community to view more details");
                    return;
                });
        })
            .catch(_ => {
                setMessage("Your id does not map to an existing community.")
                setIsCommunityDataLoaded(false);
                setSelectedCommunityId("");
                setSelectCommunityMessage("");
            });
    }

    function handleCommunityUsersUpdated() {
        loadCommunity(selectedCommunityId);
    }

    function handleCommunityLocationsUpdated() {
        loadCommunity(selectedCommunityId);
    }

    function handleCommunityEditCanceled() {
        setSelectedCommunityId("");
        setIsSelectedCommunityDataLoaded(false);
        setSelectedCommunity(new CommunityData())
        setSelectCommunityMessage("");
    }

    function renderCommunitys() {

        return (
            <div className="card pop">
                <div>
                    <h2>Your Communities</h2>
                    <div>
                        <CommunityList communityList={communityList} isCommunityDataLoaded={isCommunityDataLoaded} onSelectedCommunityChanged={loadCommunity} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                    </div>
                </div>
            </div>);
    }

    function renderEditCommunity(community: CommunityData) {
        return (
            <div>
                <h2>Community Metadata for {community.name}</h2>
                <CommunityEdit community={community} isCommunityDataLoaded={isCommunityDataLoaded} onCommunityUpdated={handleCommunityUpdated} onEditCanceled={handleCommunityEditCanceled} currentUser={currentUser} isUserLoaded={isUserLoaded} communityStatusList={communityStatusList} />
            </div >
        )
    }

    function renderCommunityUsers(community: CommunityData) {
        return (
            <div>
                <h2>Users for {community.name}</h2>
                <CommunityUsers users={userList} communityId={community.id} isUserDataLoaded={isUserDataLoaded} onCommunityUsersUpdated={handleCommunityUsersUpdated} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </div>
        )
    }

    function renderExecutiveSummary(community: CommunityData) {
        return (
            <div>
                <h2>Executive Summary for {community.name}</h2>
            </div >
        )
    }

    function renderCommunity(community: CommunityData) {
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

                { radioValue === '1' && renderEditCommunity(community)}
                { radioValue === '2' && renderCommunityUsers(community)}
                { radioValue === '3' && renderExecutiveSummary(community)}
            </div>);
    }

    let communitiesRender = isCommunityDataLoaded ? renderCommunitys() : <p><em>{message}</em></p>;
    let selectedCommunityRender = selectedCommunityId !== "" && isSelectedCommunityDataLoaded ? renderCommunity(selectedCommunity) : <p><em>{selectCommunityMessage}</em></p>;

    return <div>
        <h3>Communities</h3>
        <hr />
        {communitiesRender}
        {selectedCommunityRender}
    </div>;
}

export default withRouter(CommunityDashboard);