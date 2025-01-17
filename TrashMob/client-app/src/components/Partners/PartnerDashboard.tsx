import * as React from 'react';
import { RouteComponentProps, useParams, withRouter } from 'react-router-dom';
import UserData from '../Models/UserData';
import { PartnerEdit } from './PartnerEdit';
import { PartnerAdmins } from './PartnerAdmins';
import { PartnerLocations } from './PartnerLocations';
import { PartnerDocuments } from './PartnerDocuments';
import { PartnerSocialMediaAccounts } from './PartnerSocialMediaAccounts';
import { PartnerContacts } from './PartnerContacts';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';

export interface PartnerDashboardMatchParams {
    partnerId: string;
}

export interface PartnerDashboardProps extends RouteComponentProps<PartnerDashboardMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const PartnerDashboard: React.FC<PartnerDashboardProps> = (props) => {
    const [radioValue, setRadioValue] = React.useState('1');

    const { partnerId } = useParams<PartnerDashboardMatchParams>();

    const radios = [
        { name: 'Manage Partner', value: '1' },
        { name: 'Manage Partner Locations', value: '2' },
        { name: 'Manage Partner Contacts', value: '3' },
        { name: 'Manage Partner Admins', value: '4' },
        { name: 'Manage Partner Documents', value: '5' },
        { name: 'Manage Partner Social Media Accounts', value: '6' },
    ];

    return (
        <div className='tailwind'>
            <div className='container my-8'>
                <div>
                    <Tabs value={radioValue} onValueChange={setRadioValue}>
                        <TabsList className='w-full h-14'>
                            {radios.map((radio, idx) => (
                                <TabsTrigger className='whitespace-normal' value={radio.value} key={`tab-${idx}`}>
                                    {radio.name}
                                </TabsTrigger>
                            ))}
                        </TabsList>
                        <TabsContent value='1'>
                            <PartnerEdit
                                partnerId={partnerId}
                                currentUser={props.currentUser}
                                isUserLoaded={props.isUserLoaded}
                            />
                        </TabsContent>
                        <TabsContent value='2'>
                            <PartnerLocations
                                partnerId={partnerId}
                                currentUser={props.currentUser}
                                isUserLoaded={props.isUserLoaded}
                            />
                        </TabsContent>
                        <TabsContent value='3'>
                            <PartnerContacts
                                partnerId={partnerId}
                                currentUser={props.currentUser}
                                isUserLoaded={props.isUserLoaded}
                            />
                        </TabsContent>
                        <TabsContent value='4'>
                            <PartnerAdmins
                                partnerId={partnerId}
                                currentUser={props.currentUser}
                                isUserLoaded={props.isUserLoaded}
                            />
                        </TabsContent>
                        <TabsContent value='5'>
                            <PartnerDocuments
                                partnerId={partnerId}
                                currentUser={props.currentUser}
                                isUserLoaded={props.isUserLoaded}
                            />
                        </TabsContent>
                        <TabsContent value='6'>
                            <PartnerSocialMediaAccounts
                                partnerId={partnerId}
                                currentUser={props.currentUser}
                                isUserLoaded={props.isUserLoaded}
                            />
                        </TabsContent>
                    </Tabs>
                </div>
            </div>
        </div>
    );
};

export default withRouter(PartnerDashboard);
