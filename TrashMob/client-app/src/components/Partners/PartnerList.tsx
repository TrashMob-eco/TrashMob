import * as React from 'react';
import { Button } from 'react-bootstrap';
import UserData from '../Models/UserData';
import PartnerData from '../Models/PartnerData';
import DisplayPartner from '../Models/DisplayPartner';

export interface PartnerListDataProps {
    partnerList: PartnerData[];
    isPartnerDataLoaded: boolean;
    isUserLoaded: boolean;
    currentUser: UserData;
    onSelectedPartnerChanged: any;
}

export const PartnerList: React.FC<PartnerListDataProps> = (props) => {
    const [displayPartners, setDisplayPartners] = React.useState<DisplayPartner[]>([]);

    React.useEffect(() => {
        if (props.isPartnerDataLoaded && props.partnerList) {
            const list = props.partnerList.map((partner) => {
                const dispPartner = new DisplayPartner();
                dispPartner.id = partner.id;
                dispPartner.name = partner.name;
                return dispPartner;
            });
            setDisplayPartners(list);
        }
    }, [props.isPartnerDataLoaded, props.partnerList, props.isUserLoaded]);

    function getPartnerId(e: any) {
        props.onSelectedPartnerChanged(e);
    }

    function renderPartnersTable(partners: DisplayPartner[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby='tableLabel' width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        {partners.map((partner) => (
                            <tr key={partner.id.toString()}>
                                <td>{partner.name}</td>
                                <td>
                                    <Button className='action' onClick={() => getPartnerId(partner.id)}>
                                        View Details / Edit
                                    </Button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        );
    }

    return (
        <div>
            {!props.isPartnerDataLoaded && (
                <p>
                    <em>Loading...</em>
                </p>
            )}
            {props.isPartnerDataLoaded ? renderPartnersTable(displayPartners) : null}
        </div>
    );
};
