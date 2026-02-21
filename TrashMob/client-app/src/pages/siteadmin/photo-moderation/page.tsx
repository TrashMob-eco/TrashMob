import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { DataTable } from '@/components/ui/data-table';
import {
    GetPendingPhotos,
    GetFlaggedPhotos,
    GetModeratedPhotos,
    ApprovePhoto,
    RejectPhoto,
    DismissFlag,
    PhotoModerationItem,
    PhotoType,
} from '@/services/photo-moderation';
import { getColumns } from './columns';
import { PhotoDetailModal } from './photo-detail-modal';

type TabValue = 'pending' | 'flagged' | 'moderated';

const tabLabels: Record<TabValue, string> = {
    pending: 'Pending Review',
    flagged: 'Flagged by Users',
    moderated: 'Recently Moderated',
};

export const SiteAdminPhotoModeration = () => {
    const queryClient = useQueryClient();
    const [activeTab, setActiveTab] = useState<TabValue>('pending');
    const [selectedPhoto, setSelectedPhoto] = useState<PhotoModerationItem | null>(null);
    const [detailModalOpen, setDetailModalOpen] = useState(false);

    // Queries for each tab
    const pendingQuery = useQuery({
        queryKey: GetPendingPhotos().key,
        queryFn: GetPendingPhotos().service,
        select: (res) => res.data,
        enabled: activeTab === 'pending',
    });

    const flaggedQuery = useQuery({
        queryKey: GetFlaggedPhotos().key,
        queryFn: GetFlaggedPhotos().service,
        select: (res) => res.data,
        enabled: activeTab === 'flagged',
    });

    const moderatedQuery = useQuery({
        queryKey: GetModeratedPhotos().key,
        queryFn: GetModeratedPhotos().service,
        select: (res) => res.data,
        enabled: activeTab === 'moderated',
    });

    const invalidateQueries = () => {
        queryClient.invalidateQueries({ queryKey: GetPendingPhotos().key });
        queryClient.invalidateQueries({ queryKey: GetFlaggedPhotos().key });
        queryClient.invalidateQueries({ queryKey: GetModeratedPhotos().key });
    };

    // Mutations
    const approveMutation = useMutation({
        mutationKey: ApprovePhoto().key,
        mutationFn: ApprovePhoto().service,
        onSuccess: invalidateQueries,
    });

    const rejectMutation = useMutation({
        mutationKey: RejectPhoto().key,
        mutationFn: (params: { photoType: PhotoType; id: string }) =>
            RejectPhoto().service(params, { reason: 'Rejected via quick action' }),
        onSuccess: invalidateQueries,
    });

    const dismissMutation = useMutation({
        mutationKey: DismissFlag().key,
        mutationFn: DismissFlag().service,
        onSuccess: invalidateQueries,
    });

    // Handlers
    const handleApprove = (photoType: PhotoType, id: string) => {
        approveMutation.mutate({ photoType, id });
    };

    const handleReject = (photoType: PhotoType, id: string) => {
        // Open modal for rejection reason selection
        const photo = getCurrentData()?.find((p) => p.photoId === id);
        if (photo) {
            setSelectedPhoto(photo);
            setDetailModalOpen(true);
        }
    };

    const handleDismiss = (photoType: PhotoType, id: string) => {
        dismissMutation.mutate({ photoType, id });
    };

    const handleViewDetails = (photo: PhotoModerationItem) => {
        setSelectedPhoto(photo);
        setDetailModalOpen(true);
    };

    // Get current data based on active tab
    const getCurrentData = (): PhotoModerationItem[] | undefined => {
        switch (activeTab) {
            case 'pending':
                return pendingQuery.data;
            case 'flagged':
                return flaggedQuery.data;
            case 'moderated':
                return moderatedQuery.data;
        }
    };

    const isLoading = () => {
        switch (activeTab) {
            case 'pending':
                return pendingQuery.isLoading;
            case 'flagged':
                return flaggedQuery.isLoading;
            case 'moderated':
                return moderatedQuery.isLoading;
        }
    };

    const columns = getColumns({
        onApprove: handleApprove,
        onReject: handleReject,
        onDismiss: handleDismiss,
        onViewDetails: handleViewDetails,
        tab: activeTab,
    });

    const data = getCurrentData() || [];
    const len = data.length;

    return (
        <>
            <Card>
                <CardHeader>
                    <div className='flex items-center justify-between'>
                        <CardTitle>Photo Moderation ({len})</CardTitle>
                        <Tabs value={activeTab} onValueChange={(value) => setActiveTab(value as TabValue)}>
                            <TabsList>
                                {(Object.keys(tabLabels) as TabValue[]).map((tab) => (
                                    <TabsTrigger key={tab} value={tab}>
                                        {tabLabels[tab]}
                                    </TabsTrigger>
                                ))}
                            </TabsList>
                        </Tabs>
                    </div>
                </CardHeader>
                <CardContent>
                    {isLoading() ? (
                        <div className='text-center py-8'>Loading photos...</div>
                    ) : data.length === 0 ? (
                        <div className='text-center py-8 text-muted-foreground'>
                            {activeTab === 'pending' && 'No photos pending review'}
                            {activeTab === 'flagged' && 'No flagged photos to review'}
                            {activeTab === 'moderated' && 'No recently moderated photos'}
                        </div>
                    ) : (
                        <DataTable columns={columns} data={data} />
                    )}
                </CardContent>
            </Card>

            <PhotoDetailModal
                photo={selectedPhoto}
                open={detailModalOpen}
                onOpenChange={setDetailModalOpen}
                tab={activeTab}
            />
        </>
    );
};
