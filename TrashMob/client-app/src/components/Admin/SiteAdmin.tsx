import * as React from 'react';
import { ButtonGroup, Col, Container, Row, ToggleButton } from 'react-bootstrap';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import UserData from '../Models/UserData';
import { AdminEmailTemplates } from './AdminEmailTemplates';
import { AdminEvents } from './AdminEvents';
import { AdminPartnerRequests } from './AdminPartnerRequests';
import { AdminPartners } from './AdminPartners';
import { AdminSendNotifications } from './AdminSendNotifications';
import { AdminUsers } from './AdminUsers';

interface SiteAdminProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const SiteAdmin: React.FC<SiteAdminProps> = (props) => {

    const [currentUser, setCurrentUser] = React.useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);
    const [isSiteAdmin, setIsSiteAdmin] = React.useState<boolean>(false);
    const [radioValue, setRadioValue] = React.useState('1');

    const radios = [
        { name: 'Manage Users', value: '1' },
        { name: 'Manage Events', value: '2' },
        { name: 'Manage Partners', value: '3' },
        { name: 'Manage Partner Requests', value: '4' },
        { name: 'View Executive Summary', value: '5' },
        { name: 'Send Notifications', value: '6' },
        { name: 'View Email Templates', value: '7' },
    ];

    React.useEffect(() => {
        setIsSiteAdmin(props.currentUser.isSiteAdmin);
        setCurrentUser(props.currentUser);
        setIsUserLoaded(props.isUserLoaded);
    }, [props.currentUser, props.currentUser.isSiteAdmin, props.isUserLoaded])

    function renderManageEvents() {
        return (
            <div>
                <AdminEvents history={props.history} location={props.location} match={props.match} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </div>
        )
    }

    function renderManageUsers() {
        return (
            <div>
                <AdminUsers history={props.history} location={props.location} match={props.match} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </div>
        )
    }

    function renderManagePartners() {
        return (
            <div>
                <AdminPartners history={props.history} location={props.location} match={props.match} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </div>
        )
    }

    function renderManagePartnerRequests() {
        return (
            <div>
                <AdminPartnerRequests history={props.history} location={props.location} match={props.match} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </div>
        )
    }

    function renderSendNotifications() {
        return (
            <div>
                <AdminSendNotifications history={props.history} location={props.location} match={props.match} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </div>
        )
    }

    function renderEmailTemplates() {
        return (
            <div>
                <AdminEmailTemplates history={props.history} location={props.location} match={props.match} currentUser={currentUser} isUserLoaded={isUserLoaded} />
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
                {radioValue === '6' && renderSendNotifications()}
                {radioValue === '7' && renderEmailTemplates()}

            </div>
        );
    }

    return (
        <Container>
            <h1 className='font-weight-bold'>Site Administration</h1>
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={12}>
                    <div>
                        {!isSiteAdmin && <p><em>Access Denied</em></p>}
                        {isSiteAdmin && renderAdminTable()}
                    </div>
                </Col>
            </Row>
        </Container >
    )
}

export default withRouter(SiteAdmin);