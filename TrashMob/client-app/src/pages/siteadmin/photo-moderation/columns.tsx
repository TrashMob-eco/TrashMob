import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, CheckCircle, XCircle, Flag, Eye, Image } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import { PhotoModerationItem, PhotoType } from '@/services/photo-moderation';

interface GetColumnsProps {
    onApprove: (photoType: PhotoType, id: string) => void;
    onReject: (photoType: PhotoType, id: string) => void;
    onDismiss: (photoType: PhotoType, id: string) => void;
    onViewDetails: (photo: PhotoModerationItem) => void;
    tab: 'pending' | 'flagged' | 'moderated';
}

const statusColors: Record<number, { label: string; color: string }> = {
    0: { label: 'Pending', color: 'bg-yellow-500' },
    1: { label: 'Approved', color: 'bg-green-500' },
    2: { label: 'Rejected', color: 'bg-red-500' },
};

const photoTypeColors: Record<string, string> = {
    LitterImage: 'bg-blue-500',
    TeamPhoto: 'bg-purple-500',
    EventPhoto: 'bg-green-600',
    PartnerPhoto: 'bg-orange-500',
};

const photoTypeLabels: Record<string, string> = {
    LitterImage: 'Litter',
    TeamPhoto: 'Team',
    EventPhoto: 'Event',
    PartnerPhoto: 'Community',
};

export const getColumns = ({
    onApprove,
    onReject,
    onDismiss,
    onViewDetails,
    tab,
}: GetColumnsProps): ColumnDef<PhotoModerationItem>[] => [
    {
        accessorKey: 'imageUrl',
        header: 'Photo',
        cell: ({ row }) => {
            const imageUrl = row.getValue('imageUrl') as string;
            return imageUrl ? (
                <button
                    type='button'
                    className='w-12 h-12 p-0 border-0 bg-transparent cursor-pointer'
                    onClick={() => onViewDetails(row.original)}
                >
                    <img
                        src={imageUrl}
                        alt='Thumbnail for moderation'
                        className='w-12 h-12 object-cover rounded hover:opacity-80'
                    />
                </button>
            ) : (
                <div className='w-12 h-12 bg-gray-200 rounded flex items-center justify-center'>
                    <Image className='h-6 w-6 text-gray-400' />
                </div>
            );
        },
    },
    {
        accessorKey: 'photoType',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Type' />,
        cell: ({ row }) => {
            const photoType = row.getValue('photoType') as string;
            return (
                <Badge className={photoTypeColors[photoType] || 'bg-gray-500'}>
                    {photoTypeLabels[photoType] || photoType}
                </Badge>
            );
        },
    },
    {
        accessorKey: 'context',
        header: 'Context',
        cell: ({ row }) => {
            const photo = row.original;
            const contextName = photo.litterReportName || photo.teamName || photo.eventName || photo.partnerName;
            if (contextName) {
                return (
                    <div className='max-w-xs truncate' title={contextName}>
                        {contextName}
                    </div>
                );
            }
            return <span className='text-muted-foreground'>-</span>;
        },
    },
    {
        accessorKey: 'uploaderName',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Uploaded By' />,
        cell: ({ row }) => {
            const name = row.getValue('uploaderName') as string;
            return name || <span className='text-muted-foreground'>Unknown</span>;
        },
    },
    {
        accessorKey: 'uploadedDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Uploaded' />,
        cell: ({ row }) => {
            const date = row.getValue('uploadedDate') as string;
            return date ? new Date(date).toLocaleDateString() : '-';
        },
    },
    ...(tab === 'flagged'
        ? [
              {
                  accessorKey: 'flagReason',
                  header: 'Flag Reason',
                  cell: ({ row }: { row: { getValue: (key: string) => unknown } }) => {
                      const reason = row.getValue('flagReason') as string;
                      return reason ? (
                          <div className='max-w-xs truncate' title={reason}>
                              {reason}
                          </div>
                      ) : (
                          <span className='text-muted-foreground'>-</span>
                      );
                  },
              } as ColumnDef<PhotoModerationItem>,
          ]
        : []),
    ...(tab === 'moderated'
        ? [
              {
                  accessorKey: 'moderationStatus',
                  header: 'Status',
                  cell: ({ row }: { row: { getValue: (key: string) => unknown } }) => {
                      const status = row.getValue('moderationStatus') as number;
                      const statusInfo = statusColors[status] || { label: 'Unknown', color: 'bg-gray-500' };
                      return <Badge className={statusInfo.color}>{statusInfo.label}</Badge>;
                  },
              } as ColumnDef<PhotoModerationItem>,
              {
                  accessorKey: 'moderatedDate',
                  header: 'Moderated',
                  cell: ({ row }: { row: { getValue: (key: string) => unknown } }) => {
                      const date = row.getValue('moderatedDate') as string;
                      return date ? new Date(date).toLocaleDateString() : '-';
                  },
              } as ColumnDef<PhotoModerationItem>,
          ]
        : []),
    {
        accessorKey: 'actions',
        header: 'Actions',
        cell: ({ row }) => {
            const photo = row.original;
            const photoType = photo.photoType as PhotoType;

            return (
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant='ghost' size='icon'>
                            <Ellipsis />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent className='w-56'>
                        <DropdownMenuItem onClick={() => onViewDetails(photo)}>
                            <Eye className='mr-2 h-4 w-4' />
                            View Details
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        {(tab === 'pending' || tab === 'flagged') && (
                            <>
                                <DropdownMenuItem
                                    onClick={() => onApprove(photoType, photo.photoId)}
                                    className='text-green-600 focus:text-green-600'
                                >
                                    <CheckCircle className='mr-2 h-4 w-4' />
                                    Approve
                                </DropdownMenuItem>
                                <DropdownMenuItem
                                    onClick={() => onReject(photoType, photo.photoId)}
                                    className='text-destructive focus:text-destructive'
                                >
                                    <XCircle className='mr-2 h-4 w-4' />
                                    Reject
                                </DropdownMenuItem>
                            </>
                        )}
                        {tab === 'flagged' && (
                            <DropdownMenuItem onClick={() => onDismiss(photoType, photo.photoId)}>
                                <Flag className='mr-2 h-4 w-4' />
                                Dismiss Flag
                            </DropdownMenuItem>
                        )}
                    </DropdownMenuContent>
                </DropdownMenu>
            );
        },
    },
];
