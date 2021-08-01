import * as React from 'react'

import { RouteComponentProps, withRouter } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';
import UserData from './Models/UserData';
import PartnerData from './Models/PartnerData';
import { PartnerList } from './PartnerList';
import { PartnerEdit } from './PartnerEdit';
import PartnerStatusData from './Models/PartnerStatusData';
import { PartnerUsers } from './PartnerUsers';
import PartnerUserData from './Models/PartnerUserData';
import { PartnerLocations } from './PartnerLocations';
import PartnerLocationData from './Models/PartnerLocationData';

interface PartnerDashboardProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const PartnerDashboard: React.FC<PartnerDashboardProps> = (props) => {
    const [partnerList, setPartnerList] = React.useState<PartnerData[]>([]);
    const [userList, setUserList] = React.useState<UserData[]>([]);
    const [partnerLocationList, setPartnerLocationList] = React.useState<PartnerLocationData[]>([]);
    const [partnerStatusList, setPartnerStatusList] = React.useState<PartnerStatusData[]>([]);
    const [isPartnerDataLoaded, setIsPartnerDataLoaded] = React.useState<boolean>(false);
    const [isUserDataLoaded, setIsUserDataLoaded] = React.useState<boolean>(false);
    const [isPartnerLocationDataLoaded, setIsPartnerLocationDataLoaded] = React.useState<boolean>(false);
    const [currentUser, setCurrentUser] = React.useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);
    const [message, setMessage] = React.useState<string>("Loading...");
    const [selectedPartnerId, setSelectedPartnerId] = React.useState<string>("");
    const [selectPartnerMessage, setSelectPartnerMessage] = React.useState<string>("");
    const [isSelectedPartnerDataLoaded, setIsSelectedPartnerDataLoaded] = React.useState<boolean>(false);
    const [selectedPartner, setSelectedPartner] = React.useState<PartnerData>(new PartnerData());

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

                fetch('/api/partnerstatuses', {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<PartnerStatusData[]>)
                    .then(data => {
                        setPartnerStatusList(data)
                    })
                    .then(() => {
                        fetch('/api/partnerusers/getpartnersforuser/' + props.currentUser.id, {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => {
                                if (response.ok) {
                                    return response.json() as Promise<PartnerData[]>
                                }
                                else {
                                    throw new Error("No Partners found for this user");
                                }
                            })
                            .then(data => {
                                setPartnerList(data);
                                setIsPartnerDataLoaded(true);
                                setSelectedPartnerId("");
                                setSelectPartnerMessage("Select a partner to view more details");
                                return;
                            });
                    })
                    .catch(_ => {
                        setMessage("Your id does not map to an existing partner.")
                        setIsPartnerDataLoaded(false);
                        setSelectedPartnerId("");
                        setSelectPartnerMessage("");
                    });
            });
        }
    }, [props.currentUser, props.isUserLoaded]);

    function loadPartner(partnerId: string) {
        if (props.isUserLoaded) {
            setIsSelectedPartnerDataLoaded(false);
            setSelectedPartnerId(partnerId);
            setSelectPartnerMessage("Loading");

            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partners/' + partnerId, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<PartnerData>)
                    .then(data => {
                        setSelectedPartner(data);
                        setIsSelectedPartnerDataLoaded(true);
                    })
                    .then(() => {
                        fetch('/api/partnerusers/users/' + partnerId, {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => {
                                if (response.ok) {
                                    return response.json() as Promise<UserData[]>
                                }
                                else {
                                    throw new Error("No Partner Users found for this partner");
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
                    })
                    .then(() => {
                        fetch('/api/partnerlocations/' + partnerId, {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => {
                                if (response.ok) {
                                    return response.json() as Promise<PartnerLocationData[]>
                                }
                                else {
                                    throw new Error("No Partner Locations found for this partner");
                                }
                            })
                            .then(data => {
                                setPartnerLocationList(data);
                                setIsPartnerLocationDataLoaded(true);
                                return;
                            })
                            .catch(_ => {
                                setIsPartnerLocationDataLoaded(false);
                                setPartnerLocationList([]);
                            });
                    });
            })
        }
    };

    function handlePartnerUpdated() {
        loadPartner(selectedPartnerId);
    }

    function handlePartnerUsersUpdated() {
        loadPartner(selectedPartnerId);
    }

    function handlePartnerLocationsUpdated() {
        loadPartner(selectedPartnerId);
    }

    function handleLocationChange() {
        // do nothing
    }

    function handlePartnerEditCanceled() {
        setSelectedPartnerId("");
        setIsSelectedPartnerDataLoaded(false);
        setSelectedPartner(new PartnerData())
    }

    function renderPartners() {

        return (
            <div className="card pop">
                <div>
                    <div>
                        <PartnerList history={props.history} location={props.location} match={props.match} partnerList={partnerList} isPartnerDataLoaded={isPartnerDataLoaded} onSelectedPartnerChanged={loadPartner} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                    </div>
                </div>
            </div>);
    }

    function renderPartner(partner: PartnerData) {

        return (
            <div className="card pop">
                <div>
                    <PartnerEdit history={props.history} location={props.location} match={props.match} partner={partner} isPartnerDataLoaded={isPartnerDataLoaded} onPartnerUpdated={handlePartnerUpdated} onEditCanceled={handlePartnerEditCanceled} currentUser={currentUser} isUserLoaded={isUserLoaded} partnerStatusList={partnerStatusList} />
                </div>
                <div>
                    <PartnerUsers history={props.history} location={props.location} match={props.match} users={userList} partnerId={partner.id} isUserDataLoaded={isUserDataLoaded} onPartnerUsersUpdated={handlePartnerUsersUpdated} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                </div>
                <div>
                    <PartnerLocations history={props.history} location={props.location} match={props.match} partnerLocations={partnerLocationList} partnerId={partner.id} isPartnerLocationDataLoaded={isPartnerLocationDataLoaded} onPartnerLocationsUpdated={handlePartnerLocationsUpdated} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                </div>
            </div>);
    }

    let partnersRender = isPartnerDataLoaded ? renderPartners() : <p><em>{message}</em></p>;
    let selectedPartnerRender = selectedPartnerId !== "" && isSelectedPartnerDataLoaded ? renderPartner(selectedPartner) : <p><em>{selectPartnerMessage}</em></p>;

    return <div>
        <h3>Partners</h3>
        <hr />
        {partnersRender}
        {selectedPartnerRender}
    </div>;
}

export default withRouter(PartnerDashboard);