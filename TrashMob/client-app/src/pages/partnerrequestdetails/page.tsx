import * as React from 'react';
import { useNavigate, useParams } from 'react-router';
import moment from 'moment';
import { useQuery } from '@tanstack/react-query';
import * as ToolTips from '@/store/ToolTips';
import { getPartnerRequestStatus } from '@/store/partnerRequestStatusHelper';
import { getPartnerType } from '@/store/partnerTypeHelper';
import { GetPartnerRequestById, GetPartnerRequestStatuses, GetPartnerTypes } from '@/services/partners';
import { Marker } from '@vis.gl/react-google-maps';
import { GoogleMapWithKey as GoogleMap } from '@/components/Map/GoogleMap';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';

export interface PartnerRequestDetailsMatchParams {
    partnerRequestId: string;
}

export const PartnerRequestDetails: React.FC = () => {
    const { partnerRequestId } = useParams<'partnerRequestId'>();
    const navigate = useNavigate();

    // Get PartnerRequest
    const { data, isLoading } = useQuery({
        queryKey: GetPartnerRequestById({ id: partnerRequestId! }).key,
        queryFn: GetPartnerRequestById({ id: partnerRequestId! }).service,
        select: (res) => res.data,
    });

    // Get PartnerRequestStatus
    const { data: partnerRequestStatus, isLoading: isStatusesLoading } = useQuery({
        queryKey: GetPartnerRequestStatuses().key,
        queryFn: GetPartnerRequestStatuses().service,
        select: (res) => {
            const statuses = res.data;
            return getPartnerRequestStatus(statuses, data?.partnerRequestStatusId);
        },
        enabled: !!data?.partnerRequestStatusId,
    });

    // Get PartnerRequestType
    const { data: partnerType, isLoading: isTypeLoading } = useQuery({
        queryKey: GetPartnerTypes().key,
        queryFn: GetPartnerTypes().service,
        select: (res) => {
            const types = res.data;
            return getPartnerType(types, data?.partnerTypeId);
        },
        enabled: !!data?.partnerTypeId,
    });

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        navigate('/mydashboard');
    }

    if (!data || isLoading || isStatusesLoading || isTypeLoading) {
        return (
            <p>
                <em>Loading...</em>
            </p>
        );
    }

    return (
        <div className='tailwind'>
            <div className='container grid grid-cols-12 space-x-4 my-8'>
                <div className='col-span-4'>
                    <Card className='h-full'>
                        <CardHeader>
                            <CardTitle className='text-2xl text-primary'>Partner request sent</CardTitle>
                            <CardDescription>This partner request has been sent!</CardDescription>
                        </CardHeader>
                    </Card>
                </div>
                <div className='col-span-8'>
                    <Card>
                        <CardHeader>
                            <CardTitle className='text-2xl text-primary'>Partner request</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <TooltipProvider delayDuration={0}>
                                <div className='grid grid-cols-12 gap-4'>
                                    <div className='space-y-2 col-span-4'>
                                        <Tooltip>
                                            <TooltipTrigger>
                                                <Label>Partner Name</Label>
                                            </TooltipTrigger>
                                            <TooltipContent className='max-w-96'>
                                                {ToolTips.PartnerRequestName}
                                            </TooltipContent>
                                        </Tooltip>
                                        <Input type='text' disabled value={data.name} />
                                    </div>
                                    <div className='space-y-2 col-span-4'>
                                        <Tooltip>
                                            <TooltipTrigger>
                                                <Label>Partner Type</Label>
                                            </TooltipTrigger>
                                            <TooltipContent className='max-w-96'>{ToolTips.PartnerType}</TooltipContent>
                                        </Tooltip>
                                        <Input type='text' disabled value={partnerType || ''} />
                                    </div>
                                    <div className='space-y-2 col-span-4'>
                                        <Tooltip>
                                            <TooltipTrigger>
                                                <Label>Request Status</Label>
                                            </TooltipTrigger>
                                            <TooltipContent className='max-w-96'>
                                                {ToolTips.PartnerRequestStatus}
                                            </TooltipContent>
                                        </Tooltip>
                                        <Input type='text' disabled value={partnerRequestStatus || ''} />
                                    </div>
                                    <div className='space-y-2 col-span-5'>
                                        <Tooltip>
                                            <TooltipTrigger>
                                                <Label>Email</Label>
                                            </TooltipTrigger>
                                            <TooltipContent className='max-w-96'>
                                                {ToolTips.PartnerRequestEmail}
                                            </TooltipContent>
                                        </Tooltip>
                                        <Input type='text' disabled value={data.email} />
                                    </div>
                                    <div className='space-y-2 col-span-7'>
                                        <Tooltip>
                                            <TooltipTrigger>
                                                <Label>Phone</Label>
                                            </TooltipTrigger>
                                            <TooltipContent className='max-w-96'>
                                                {ToolTips.PartnerRequestPhone}
                                            </TooltipContent>
                                        </Tooltip>
                                        <Input type='text' disabled value={data.phone} />
                                    </div>
                                    <div className='space-y-2 col-span-12'>
                                        <Tooltip>
                                            <TooltipTrigger>
                                                <Label>Website</Label>
                                            </TooltipTrigger>
                                            <TooltipContent className='max-w-96'>
                                                {ToolTips.PartnerRequestWebsite}
                                            </TooltipContent>
                                        </Tooltip>
                                        <Input type='text' disabled value={data.website} />
                                    </div>
                                    <div className='space-y-2 col-span-12'>
                                        <Tooltip>
                                            <TooltipTrigger>
                                                <Label>Notes</Label>
                                            </TooltipTrigger>
                                            <TooltipContent className='max-w-96'>
                                                {ToolTips.PartnerRequestNotes}
                                            </TooltipContent>
                                        </Tooltip>
                                        <Input type='text' disabled value={data.notes} />
                                    </div>
                                    <div className='space-y-2 col-span-12'>
                                        <Label>Street Address</Label>
                                        <Input type='text' disabled value={data.streetAddress} />
                                    </div>
                                    <div className='space-y-2 col-span-6'>
                                        <Label>City</Label>
                                        <Input type='text' disabled value={data.city} />
                                    </div>
                                    <div className='space-y-2 col-span-6'>
                                        <Label>Postal Code</Label>
                                        <Input type='text' disabled value={data.postalCode} />
                                    </div>
                                    <div className='space-y-2 col-span-6'>
                                        <Tooltip>
                                            <TooltipTrigger>
                                                <Label>Region</Label>
                                            </TooltipTrigger>
                                            <TooltipContent className='max-w-96'>
                                                {ToolTips.PartnerRequestRegion}
                                            </TooltipContent>
                                        </Tooltip>
                                        <Input type='text' disabled value={data.region} />
                                    </div>
                                    <div className='space-y-2 col-span-6'>
                                        <Tooltip>
                                            <TooltipTrigger>
                                                <Label>Country</Label>
                                            </TooltipTrigger>
                                            <TooltipContent className='max-w-96'>
                                                {ToolTips.PartnerRequestCountry}
                                            </TooltipContent>
                                        </Tooltip>
                                        <Input type='text' disabled value={data.country} />
                                    </div>
                                    <div className='col-span-12'>
                                        <GoogleMap
                                            defaultCenter={{ lat: data.latitude, lng: data.longitude }}
                                            defaultZoom={11}
                                        >
                                            <Marker
                                                position={{ lat: data.latitude, lng: data.longitude }}
                                                draggable={false}
                                            />
                                        </GoogleMap>
                                    </div>
                                    <div className='space-y-2 col-span-6'>
                                        <Tooltip>
                                            <TooltipTrigger>
                                                <Label>Created Date</Label>
                                            </TooltipTrigger>
                                            <TooltipContent className='max-w-96'>
                                                {ToolTips.PartnerRequestCreatedDate}
                                            </TooltipContent>
                                        </Tooltip>
                                        <Input type='text' disabled value={moment(data.createdDate).toString()} />
                                    </div>
                                    <div className='space-y-2 col-span-6'>
                                        <Tooltip>
                                            <TooltipTrigger>
                                                <Label>Last Updated Date</Label>
                                            </TooltipTrigger>
                                            <TooltipContent className='max-w-96'>
                                                {ToolTips.PartnerRequestLastUpdatedDate}
                                            </TooltipContent>
                                        </Tooltip>
                                        <Input type='text' disabled value={moment(data.lastUpdatedDate).toString()} />
                                    </div>
                                    <div className='col-span-12'>
                                        <Button onClick={handleCancel}>Cancel</Button>
                                    </div>
                                </div>
                            </TooltipProvider>
                        </CardContent>
                    </Card>
                </div>
            </div>
        </div>
    );
};
