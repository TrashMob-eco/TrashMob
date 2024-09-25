import { FC, FormEvent, useEffect } from 'react';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Button, Container } from 'react-bootstrap';
import UserData from '../Models/UserData';
import { getApiConfig, getB2CPolicies, msalClient } from '../../store/AuthStore';
import { HeroSection } from '../Customization/HeroSection';

interface DeleteMyDataProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const DeleteMyData: FC<DeleteMyDataProps> = (props) => {
    useEffect(() => {
        window.scrollTo(0, 0);
    }, []);

    const handleDelete = (event: FormEvent<HTMLElement>) => {
        event.preventDefault();

        const account = msalClient.getAllAccounts()[0];
        const policy = getB2CPolicies();
        const scopes = getApiConfig();

        const request = {
            account,
            authority: policy.authorities.deleteUser.authority,
            scopes: scopes.b2cScopes,
        };
        msalClient.acquireTokenPopup(request).then(() => {
            const logoutRequest = {
                account: msalClient.getActiveAccount(),
            };
            msalClient.logoutRedirect(logoutRequest);
        });
    };

    const renderDeleteYourAccount = () => (
        <Container className='bodyMargin'>
            <h2 className='fw-500 font-size-xl'>Delete my data</h2>
            <p className='p-18'>
                If you no longer wish to be a member of the TrashMob.eco community, clicking the delete button below
                will delete your account and anonymize any event-related data for events you may have participated in.
                Warning: Deleting an account cannot be undone.
            </p>
            <p>We are sorry to see you go!</p>
            <p>The Team at TrashMob.eco</p>
            <Button
                className='mx-0 my-5 border border-danger text-danger h-49 p-18'
                variant='outline'
                onClick={(e) => handleDelete(e)}
            >
                Delete Account
            </Button>
        </Container>
    );

    const contents = renderDeleteYourAccount();

    return (
        <div>
            <HeroSection
                Title='Delete your account?'
                Description='TrashMob members are making the world a better place!'
            />
            {contents}
        </div>
    );
};

export default withRouter(DeleteMyData);
