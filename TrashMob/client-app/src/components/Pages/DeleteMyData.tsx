import { FC, FormEvent, useEffect, useState } from 'react';
import UserData from '../Models/UserData';
import { getApiConfig, getB2CPolicies, msalClient } from '../../store/AuthStore';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Button, Col, Container, Image, ModalBody, Row } from 'react-bootstrap';
import { Modal } from 'reactstrap';
import globes from '../assets/gettingStarted/globes.png';

interface DeleteMyDataProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const DeleteMyData: FC<DeleteMyDataProps> = (props) => {
    const [isOpen, setIsOpen] = useState(false);

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])

    const togglemodal = () => {
        setIsOpen(!isOpen);
    }

    const handleDelete = (event: FormEvent<HTMLElement>) => {
        event.preventDefault();
        setIsOpen(true);
    }

    // This will handle the delete account
    const deleteAccount = () => {

        var policy = getB2CPolicies();
        var scopes = getApiConfig();

        var request = {
            authority: policy.authorities.deleteUser.authority,
            scopes: scopes.b2cScopes,
            
        };
        msalClient.acquireTokenRedirect(request)
            .then(() => props.history.push("/"));
    }

    return (
        <div>
            <Container fluid className='bg-grass shadow'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className='font-weight-bold'>Delete your account?</h1>
                        <p className="font-weight-bold">TrashMob members are making the world a better place!</p>
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