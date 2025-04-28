import { FC, FormEvent } from 'react';
import { getApiConfig, getB2CPolicies, msalClient } from '@/store/AuthStore';
import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';

interface DeleteMyDataProps {}

export const DeleteMyData: FC<DeleteMyDataProps> = (props) => {
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

    return (
        <div className='tailwind'>
            <HeroSection
                Description='TrashMob members are making the world a better place!'
                Title='Delete your account?'
            />
            <div className='container mx-auto py-5'>
                <Card>
                    <CardHeader>
                        <CardTitle>Delete my data</CardTitle>
                        <CardDescription>
                            <p>
                                If you no longer wish to be a member of the TrashMob.eco community, clicking the delete
                                button below will delete your account and anonymize any event-related data for events
                                you may have participated in. Warning: Deleting an account cannot be undone.
                            </p>
                            <p>We are sorry to see you go!</p>
                            <p>The Team at TrashMob.eco</p>
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <Button onClick={handleDelete} variant='destructive'>
                            Delete Account
                        </Button>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
};
