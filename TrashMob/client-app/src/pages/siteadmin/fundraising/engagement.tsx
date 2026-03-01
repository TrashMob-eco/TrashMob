import { useQuery } from '@tanstack/react-query';
import { useSearchParams } from 'react-router';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { DataTable } from '@/components/ui/data-table';
import { GetEngagementScores, GetLybuntContacts, GetVolunteerPipeline } from '@/services/contacts';
import { engagementColumns } from './engagement-columns';

export const SiteAdminEngagement = () => {
    const [searchParams] = useSearchParams();
    const defaultTab = searchParams.get('tab') || 'all';

    const { data: allScores } = useQuery({
        queryKey: GetEngagementScores().key,
        queryFn: GetEngagementScores().service,
        select: (res) => res.data,
    });

    const { data: volunteerPipeline } = useQuery({
        queryKey: GetVolunteerPipeline().key,
        queryFn: GetVolunteerPipeline().service,
        select: (res) => res.data,
    });

    const { data: lybuntContacts } = useQuery({
        queryKey: GetLybuntContacts().key,
        queryFn: GetLybuntContacts().service,
        select: (res) => res.data,
    });

    return (
        <div className='space-y-6'>
            <Tabs defaultValue={defaultTab}>
                <TabsList>
                    <TabsTrigger value='all'>All Contacts ({(allScores || []).length})</TabsTrigger>
                    <TabsTrigger value='pipeline'>Volunteer Pipeline ({(volunteerPipeline || []).length})</TabsTrigger>
                    <TabsTrigger value='lybunt'>LYBUNT ({(lybuntContacts || []).length})</TabsTrigger>
                </TabsList>

                <TabsContent value='all'>
                    <Card>
                        <CardHeader>
                            <CardTitle>Engagement Scores</CardTitle>
                            <CardDescription>
                                All active contacts ranked by engagement score (donations, volunteer activity,
                                interactions)
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <DataTable
                                columns={engagementColumns}
                                data={allScores || []}
                                enableSearch
                                searchPlaceholder='Search contacts...'
                                searchColumns={['contactName', 'email']}
                            />
                        </CardContent>
                    </Card>
                </TabsContent>

                <TabsContent value='pipeline'>
                    <Card>
                        <CardHeader>
                            <CardTitle>Volunteer-to-Donor Pipeline</CardTitle>
                            <CardDescription>
                                Highly engaged volunteers who haven&apos;t made a donation yet — prime candidates for
                                fundraising outreach
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <DataTable
                                columns={engagementColumns}
                                data={volunteerPipeline || []}
                                enableSearch
                                searchPlaceholder='Search contacts...'
                                searchColumns={['contactName', 'email']}
                            />
                        </CardContent>
                    </Card>
                </TabsContent>

                <TabsContent value='lybunt'>
                    <Card>
                        <CardHeader>
                            <CardTitle>LYBUNT — Last Year But Unfortunately Not This Year</CardTitle>
                            <CardDescription>
                                Donors who gave in the previous calendar year but have not donated this year. Re-engage
                                these contacts before they lapse permanently.
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <DataTable
                                columns={engagementColumns}
                                data={lybuntContacts || []}
                                enableSearch
                                searchPlaceholder='Search contacts...'
                                searchColumns={['contactName', 'email']}
                            />
                        </CardContent>
                    </Card>
                </TabsContent>
            </Tabs>
        </div>
    );
};
