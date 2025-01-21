import * as React from 'react';
import { useParams } from 'react-router-dom';
import UserData from '@/components/Models/UserData';
import { PartnerEdit } from '@/components/Partners/PartnerEdit';
import { PartnerAdmins } from '@/components/Partners/PartnerAdmins';
import { PartnerLocations } from '@/components/Partners/PartnerLocations';
import { PartnerDocuments } from '@/components/Partners/PartnerDocuments';
import { PartnerSocialMediaAccounts } from '@/components/Partners/PartnerSocialMediaAccounts';
import { PartnerContacts } from '@/components/Partners/PartnerContacts';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';

export interface PartnerDashboardMatchParams {
    partnerId: string;
}

export interface PartnerDashboardProps {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const PartnerDashboard: React.FC<PartnerDashboardProps> = (props) => {
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
                        <TabsList className='w-full h-14 mb-4'>
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
