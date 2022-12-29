import { ChangeEvent, FC, FormEvent, useEffect, useState } from 'react';
import UserData from '../Models/UserData';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../../store/ToolTips";
import { getApiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Button, Col, Container, Form, Image, ModalBody, Row } from 'react-bootstrap';
import { Modal } from 'reactstrap';
import * as MapStore from '../../store/MapStore';
import { getKey } from '../../store/MapStore';
import AddressData from '../Models/AddressData';
import { data } from 'azure-maps-control';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import * as Constants from '../Models/Constants';
import MapControllerSinglePoint from '../MapControllerSinglePoint';
import globes from '../assets/gettingStarted/globes.png';
import infoCycle from '../assets/info-circle.svg';

interface DeleteMyDataProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const DeleteMyData: FC<DeleteMyDataProps> = (props) => {
    const userId = props.currentUser.id;
    const [isDataLoaded, setIsDataLoaded] = useState<boolean>(false);
    const [userName, setUserName] = useState<string>("");
    const [givenName, setGivenName] = useState<string>("");
    const [surname, setSurname] = useState<string>("");
    const [email, setEmail] = useState<string>();
    const [city, setCity] = useState<string>();
    const [radiusType, setRadiusType] = useState<string>("");
    const [country, setCountry] = useState<string>();
    const [region, setRegion] = useState<string>();
    const [postalCode, setPostalCode] = useState<string>();
    const [dateAgreedToTrashMobWaiver, setDateAgreedToTrashMobWaiver] = useState<Date>(new Date());
    const [trashMobWaiverVersion, setTrashMobWaiverVersion] = useState<string>("");
    const [memberSince, setMemberSince] = useState<Date>(new Date());
    const [maxEventsRadiusErrors, setMaxEventsRadiusErrors] = useState<string>("");
    const [userNameErrors, setUserNameErrors] = useState<string>("");
    const [givenNameErrors, setGivenNameErrors] = useState<string>("");
    const [surNameErrors, setSurNameErrors] = useState<string>("");
    const [longitude, setLongitude] = useState<number>(0);
    const [latitude, setLatitude] = useState<number>(0);
    const [prefersMetric, setPrefersMetric] = useState<boolean>(false);
    const [travelLimitForLocalEvents, setTravelLimitForLocalEvents] = useState<number>(10);
    const [isOpen, setIsOpen] = useState(false);
    const [travelLimitForLocalEventsErrors, setTravelLimitForLocalEventsErrors] = useState<string>("");
    const [center, setCenter] = useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = useState<boolean>(false);
    const [mapOptions, setMapOptions] = useState<IAzureMapOptions>();
    const [eventName, setEventName] = useState<string>("User's Base Location");
    const [isSaveEnabled, setIsSaveEnabled] = useState<boolean>(false);
    const [formSubmitted, setFormSubmitted] = useState<boolean>(false);
    const [formSubmitErrors, setFormSubmitErrors] = useState<string>("");
    const [units, setUnits] = useState<string[]>([]);

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])

    const togglemodal = () => {
        setIsOpen(!isOpen);
    }

    // This will handle Cancel button click event.  
    const handleCancel = (event: FormEvent<HTMLElement>) => {
        event.preventDefault();
        props.history.push("/");
    }

    const handleDelete = (event: FormEvent<HTMLElement>) => {
        event.preventDefault();
        setIsOpen(true);
    }

    // This will handle the delete account
    const deleteAccount = () => {

        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

        const request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {

            const headers = getDefaultHeaders('DELETE');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/users/' + userId, {
                method: 'DELETE',
                headers: headers
            }).then(() => {
                msalClient.logoutRedirect();
                props.history.push("/");
            })
        })
    }

    return (
        <div>
            <Container fluid className='bg-grass shadow'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className='font-weight-bold'>About TrashMob</h1>
                        <p className="font-weight-bold">Ideas Inspired by simple action.</p>
                    </Col>
                    <Col md={5}>
                        <Image src={globes} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>
            <Modal isOpen={isOpen} centered onrequestclose={togglemodal} contentlabel="Delete Account?" fade={true} size={"lg"}>
                <ModalBody>
                    <h2 className='fw-500'>Delete your account?</h2>
                    <p className='p-18'>
                        Are you sure you want to delete your account? This action cannot be undone and you will not be able to reactivate your account, view your past events, or continue building your stats.
                    </p>
                    <div className='d-flex justify-content-end'>
                        <Button className="action h-49 p-18" onClick={() => {
                            togglemodal();
                        }
                        }>
                            Cancel
                        </Button>
                        <Button variant="outline" className='ml-2 border-danger text-danger h-49' onClick={() => {
                            togglemodal();
                            deleteAccount();
                        }
                        }>
                            Delete
                        </Button>
                    </div>
                </ModalBody>
            </Modal>

            <Container className='bodyMargin'>
                <h2 className='fw-500 font-size-xl'>Delete my data</h2>
                <p className="p-18">
                    If you no longer wish to be a member of the TrashMob.eco community, clicking the delete button below will delete your account and anonymize any event-related data for events you may have participated in.
                    Warning: Deleting an account cannot be undone.
                </p>
                <p>
                    We are sorry to see you go!
                </p>
                <p>
                    The Team at TrashMob.eco
                </p>
                <Button className='mx-0 my-5 border border-danger text-danger h-49 p-18' variant="outline" onClick={(e) => handleDelete(e)}>Delete Account</Button>
            </Container>
        </div >
    );
}

export default withRouter(DeleteMyData);