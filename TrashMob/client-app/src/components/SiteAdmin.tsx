import * as React from 'react';
import { ButtonGroup, ToggleButton } from 'react-bootstrap';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import UserData from './Models/UserData';

interface SiteAdminProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const SiteAdmin: React.FC<SiteAdminProps> = (props) => {

    const [isSiteAdmin, setIsSiteAdmin] = React.useState<boolean>(false);
    const [radioValue, setRadioValue] = React.useState('1');

    const radios = [
        { name: 'Manage Users', value: '1' },
        { name: 'Manage Events', value: '2' },
        { name: 'Manage Partners', value: '3' },
        { name: 'Manage Partner Requests', value: '4' },
        { name: 'View Executive Summary', value: '5' },
    ];

    React.useEffect(() => {
        setIsSiteAdmin(props.currentUser.isSiteAdmin);
    })

    function renderManageEvents() {
        return (
            <div>
                Events
            </div>
        )
    }

    function renderManageUsers() {
        return (
            <div>
                UserList
            </div>
        )
    }

    function renderManagePartners() {
        return (
            <div>
                Partner List
            </div>
        )
    }

    function renderManagePartnerRequests() {
        return (
            <div>
                Partner Requests
            </div>
        )
    }

    function renderExecutiveSummary() {
        return (
            <div>
                Executive Summary
            </div>
        )
    }

    function renderAdminTable() {
        return (
            <div>
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

                {radioValue === '1' && renderManageUsers()}
                {radioValue === '2' && renderManageEvents()}
                {radioValue === '3' && renderManagePartners()}
                {radioValue === '4' && renderManagePartnerRequests()}
                {radioValue === '5' && renderExecutiveSummary()}

            </div>
        );
    }

    return (
        <>
            <h1>Site Administration</h1>
            <div>
                {!isSiteAdmin && <p><em>Access Denied</em></p>}
                {isSiteAdmin && renderAdminTable()}

            </div>
        </>
    )
}

export default withRouter(SiteAdmin);