import { FC, FormEvent, useEffect } from 'react';
import UserData from '../Models/UserData';
import { getApiConfig, getB2CPolicies, msalClient, msalClientNoRedirect } from '../../store/AuthStore';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Button, Col, Container, Image, Row } from 'react-bootstrap';
import globes from '../assets/gettingStarted/globes.png';

interface DeleteMyDataProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
    onUserDeleted: any;
}

const DeleteMyData: FC<DeleteMyDataProps> = (props) => {

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [])

    const handleDelete = (event: FormEvent<HTMLElement>) => {
        event.preventDefault();

        const account = msalClient.getAllAccounts()[0];
        var policy = getB2CPolicies();
        var scopes = getApiConfig();

        var request = {
            account: account,
            authority: policy.authorities.deleteUser.authority,
            scopes: scopes.b2cScopes,
        };
        msalClientNoRedirect.acquireTokenRedirect(request)
            .then(() => {
                props.onUserDeleted();
                props.history.push("/");
            });
    }

    const renderDeleteYourAccount = () => {
        return (
            <>
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
            </>
        )
    }

    const contents = renderDeleteYourAccount();

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
            {contents}
        </div>
    );
}

export default withRouter(DeleteMyData);