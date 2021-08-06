import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import EventData from './Models/EventData';
import { withRouter } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import UserData from './Models/UserData';
import { Button, Col, Form } from 'react-bootstrap';
import DisplayPartner from './Models/DisplayPartner';
import EventPartnerData from './Models/EventPartnerData';
import PartnerLocationData from './Models/PartnerLocationData';
import PartnerData from './Models/PartnerData';

export interface EventPartnersMatchParams {
    eventId: string;
}

export interface EventPartnersProps extends RouteComponentProps<EventPartnersMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventPartners: React.FC<EventPartnersProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [eventId, setEventId] = React.useState<string>(props.match.params["eventId"]);
    const [partners, setPartners] = React.useState<PartnerData[]>([]);
    const [partnerLocations, setEventPartnerLocations] = React.useState<PartnerLocationData[]>([]);
    const [eventPartners, setEventPartners] = React.useState<EventPartnerData[]>([]);
    const [title, setTitle] = React.useState<string>("Event Partners");

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');

        
        fetch('/api/EventPartnersPotentialMatches/' + eventId, {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<EventPartnerData[]>)
            .then(data => {
                setEventPartners(data);
            })
        .then(data => )



    }, [eventId])

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push("/mydashboard");
    }

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        var eventData = new EventData();
        var method = "POST";

        var evtdata = JSON.stringify(eventData);

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/EventPartners', {
                method: method,
                headers: headers,
                body: evtdata,
            }).then(() => {
                props.history.push("/mydashboard");
            });
        })
    }

    React.useEffect(() => {
        if (props.isPartnerDataLoaded && props.partnerList) {
            const list = props.partnerList.map((partner) => {
                var dispPartner = new DisplayPartner()
                dispPartner.id = partner.id;
                dispPartner.name = partner.name;
                return dispPartner;
            });
            setDisplayPartners(list);
        }
    }, [props.isPartnerDataLoaded, props.partnerList, props.isUserLoaded])

    function getPartnerId(e: any) {
        props.onSelectedPartnerChanged(e);
    }

    function renderPartnersTable(partners: DisplayPartner[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        {partners.map(partner =>
                            <tr key={partner.id.toString()}>
                                <td>{partner.name}</td>
                                <td>
                                    <Button className="action" onClick={() => getPartnerId(partner.id)}>View Details / Edit</Button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    return (
        <>
            <div>
                {!props.isPartnerDataLoaded && <p><em>Loading...</em></p>}
                {props.isPartnerDataLoaded && renderPartnersTable(displayPartners)}
            </div>
        </>
    );
}

export default withRouter(EventPartners);