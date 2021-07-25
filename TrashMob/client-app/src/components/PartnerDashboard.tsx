import * as React from 'react'

import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';
import UserData from './Models/UserData';
import PartnerData from './Models/PartnerData';
import { PartnerList } from './PartnerList';

interface PartnerDashboardProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const PartnerDashboard: React.FC<PartnerDashboardProps> = (props) => {
    const [myPartnerList, setPartnerList] = React.useState<PartnerData[]>([]);
    const [isPartnerDataLoaded, setIsPartnerDataLoaded] = React.useState<boolean>(false);
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [currentUser, setCurrentUser] = React.useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);

    React.useEffect(() => {
        setCurrentUser(props.currentUser);
        setIsUserLoaded(props.isUserLoaded);

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })

        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                var point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point)
            });
        } else {
            console.log("Not Available");
        }

        if (props.isUserLoaded) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partners/userpartners/' + props.currentUser.id, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<PartnerData[]>)
                    .then(data => {
                        setPartnerList(data);
                        setIsPartnerDataLoaded(true);
                    });
            });
        }
    }, [props.currentUser, props.isUserLoaded]);

    function loadPartnerLocations() {
        if (props.isUserLoaded) {
            setIsPartnerDataLoaded(false);
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partner/userpartners/' + currentUser.id, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<PartnerData[]>)
                    .then(data => {
                        setPartnerList(data);
                        setIsPartnerDataLoaded(true);
                    });
            });
        }
    };

    function handleLocationChange(point: data.Position) {
        // do nothing
    }

    return (
        <div className="card pop">
            <div>
                <Link to="/addpartner">Create a New Partner</Link>
            </div>
            <div>
                <div>
                    <PartnerList history={props.history} location={props.location} match={props.match} partnerList={myPartnerList} isPartnerDataLoaded={isPartnerDataLoaded} onPartnerLocationsChanged={loadPartnerLocations} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                </div>
            </div>
            <div>
                <AzureMapsProvider>
                    <>
                        <MapController center={center} multipleEvents={[]} isEventDataLoaded={isPartnerDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                    </>
                </AzureMapsProvider>
            </div>
        </div>
    );
}

export default withRouter(PartnerDashboard);