import * as React from 'react';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import UserData from './Models/UserData';

interface SiteAdminProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
    isSiteAdmin: boolean;
}

const SiteAdmin: React.FC<SiteAdminProps> = (props) => {

    const [isManageUsersSelected, setIsManageUsersSelected] = React.useState<boolean>(false);
    const [isManagePartnersSelected, setIsManagePartnersSelected] = React.useState<boolean>(false);
    const [isManagePartnerRequestsSelected, setManagePartnerRequestsSelected] = React.useState<boolean>(false);
    const [isManageEventsSelected, setIsManageEventsSelected] = React.useState<boolean>(false);
    const [isSeeExecutiveSummarySelected, setSeeExecutiveSummarySelected] = React.useState<boolean>(false);

    React.useEffect(() => {
        if (props.isSiteAdmin) {

        }
    })

    function renderManageEvents() {
        return (
            <div>
            </div>
        )
    }

    function renderManageUsers() {
        return (
            <div>
            </div>
        )
    }

    function renderManagePartners() {
        return (
            <div>
            </div>
        )
    }

    function renderManagePartnerRequests() {
        return (
            <div>
            </div>
        )
    }

    function renderExecutiveSummary() {
        return (
            <div>
            </div>
        )
    }

    function renderAdminTable() {
        return (
            <div>
                <table>
                    <tr>
                        <td>
                            Manage Users
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Manage Existing Partners
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Manage Partner Requests
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Manage Events
                        </td>
                    </tr>
                    <tr>
                        <td>
                            See Executive Summary
                        </td>
                    </tr>
                </table>

                {isManageEventsSelected && renderManageEvents()}
                {isManageUsersSelected && renderManageUsers()}
                {isManagePartnersSelected && renderManagePartners()}
                {isSeeExecutiveSummarySelected && renderExecutiveSummary()}
                {isManagePartnerRequestsSelected && renderManagePartnerRequests()}

            </div>
        );
    }

    return (
        <>
            <div>
                {!props.isSiteAdmin && <p><em>Access Denied</em></p>}
                {props.isSiteAdmin && renderAdminTable()}

            </div>
        </>
    )
}

export default withRouter(SiteAdmin);