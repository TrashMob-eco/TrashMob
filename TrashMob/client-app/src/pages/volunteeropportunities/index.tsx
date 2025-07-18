import * as React from 'react';
import { Link } from 'react-router';
import { Button } from '@/components/ui/button';
import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible';
import { useQuery } from '@tanstack/react-query';
import { GetAllJobOpportunities } from '@/services/job-opportunities';

export const VolunteerOpportunities: React.FC = () => {
    const { data: opportunities } = useQuery({
        queryKey: GetAllJobOpportunities().key,
        queryFn: GetAllJobOpportunities().service,
        select: (res) => res.data,
        enabled: true,
    });

    return (
        <div className='tailwind'>
            <HeroSection Title='Recruiting' Description='We’d love to have you join us.' />
            <div className='container mx-auto'>
                <div className='grid grid-cols-12 gap-4'>
                    <div className='col-span-12'>
                        <div className='flex justify-between items-center my-4'>
                            <h1 className='m-0'>Open volunteer positions ({(opportunities || []).length})</h1>
                            <Button asChild>
                                <Link to='/contactus'>Contact us</Link>
                            </Button>
                        </div>
                    </div>
                    <div className='col-span-12 lg:col-span-4 mb-8'>
                        <Card>
                            <CardHeader>
                                <CardTitle>Looking to contribute to the growth of TrashMob.eco?</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <p className='p-18'>
                                    There are many ways to get involved in the growth of TrashMob.eco besides picking
                                    litter.
                                </p>
                                <p className='p-18'>
                                    On this page are a few ways you can contribute from the comfort of your own home! We
                                    encourage you to reach out even if you don't have all the preferred skills.
                                </p>
                                <p className='p-18'>
                                    If you are interested in any of these opportunities, contact us at{' '}
                                </p>
                                <p className='p-18 color-primary'>info@trashmob.eco.</p>
                            </CardContent>
                        </Card>
                    </div>
                    <div className='col-span-12 lg:col-span-8'>
                        {opportunities && opportunities.length > 0 ? (
                            <>
                                {opportunities.map((opp, index) => (
                                    <Card className='mb-4' key={`opportunity-${index}`}>
                                        <CardHeader>
                                            <CardTitle>{opp.title}</CardTitle>
                                        </CardHeader>
                                        <CardContent>
                                            <p>{opp.tagLine}</p>
                                            <Collapsible className='group'>
                                                <CollapsibleContent>{opp.fullDescription}</CollapsibleContent>
                                                <CollapsibleTrigger>
                                                    <span className='text-primary group-data-[state=open]:hidden'>
                                                        See more
                                                    </span>
                                                    <span className='text-primary group-data-[state=closed]:hidden'>
                                                        See less
                                                    </span>
                                                </CollapsibleTrigger>
                                            </Collapsible>
                                        </CardContent>
                                    </Card>
                                ))}
                            </>
                        ) : (
                            <div className='flex justify-center py-24'>There is no opening right now.</div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};
