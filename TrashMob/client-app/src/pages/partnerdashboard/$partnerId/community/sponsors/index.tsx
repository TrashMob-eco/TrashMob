import { useCallback } from 'react';
import { useParams, Link, useNavigate } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Loader2, Plus, Pencil, Trash2, HandCoins } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogTrigger,
} from '@/components/ui/alert-dialog';
import { useToast } from '@/hooks/use-toast';
import SponsorData from '@/components/Models/SponsorData';
import { GetSponsors, DeactivateSponsor } from '@/services/sponsors';

export const PartnerCommunitySponsors = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();

    const { data: sponsors, isLoading } = useQuery<AxiosResponse<SponsorData[]>, unknown, SponsorData[]>({
        queryKey: GetSponsors({ partnerId }).key,
        queryFn: GetSponsors({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { mutate: deactivateSponsor, isPending: isDeactivating } = useMutation({
        mutationKey: DeactivateSponsor().key,
        mutationFn: DeactivateSponsor().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetSponsors({ partnerId }).key,
            });
            toast({
                variant: 'primary',
                title: 'Sponsor deactivated',
                description: 'The sponsor has been deactivated.',
            });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to deactivate sponsor. Please try again.',
            });
        },
    });

    const handleDeactivate = useCallback(
        (sponsorId: string) => {
            if (!partnerId) return;
            deactivateSponsor({ partnerId, sponsorId });
        },
        [partnerId, deactivateSponsor],
    );

    if (isLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='py-8'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div>
                        <CardTitle className='flex items-center gap-2'>
                            <HandCoins className='h-5 w-5' />
                            Sponsors
                        </CardTitle>
                        <CardDescription>
                            Manage sponsors who fund professional cleanup of adopted areas.
                        </CardDescription>
                    </div>
                    <Button onClick={() => navigate(`/partnerdashboard/${partnerId}/community/sponsors/create`)}>
                        <Plus className='h-4 w-4 mr-2' />
                        Add Sponsor
                    </Button>
                </CardHeader>
                <CardContent>
                    {sponsors && sponsors.length > 0 ? (
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Name</TableHead>
                                    <TableHead>Email</TableHead>
                                    <TableHead>Phone</TableHead>
                                    <TableHead>On Map</TableHead>
                                    <TableHead>Status</TableHead>
                                    <TableHead className='text-right'>Actions</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {sponsors.map((sponsor) => (
                                    <TableRow key={sponsor.id}>
                                        <TableCell className='font-medium'>{sponsor.name}</TableCell>
                                        <TableCell>{sponsor.contactEmail}</TableCell>
                                        <TableCell>{sponsor.contactPhone}</TableCell>
                                        <TableCell>
                                            <Badge
                                                className={
                                                    sponsor.showOnPublicMap
                                                        ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                                                        : 'bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-200'
                                                }
                                            >
                                                {sponsor.showOnPublicMap ? 'Visible' : 'Hidden'}
                                            </Badge>
                                        </TableCell>
                                        <TableCell>
                                            <Badge
                                                className={
                                                    sponsor.isActive
                                                        ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                                                        : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
                                                }
                                            >
                                                {sponsor.isActive ? 'Active' : 'Inactive'}
                                            </Badge>
                                        </TableCell>
                                        <TableCell className='text-right'>
                                            <div className='flex justify-end gap-2'>
                                                <Button variant='outline' size='sm' asChild>
                                                    <Link
                                                        to={`/partnerdashboard/${partnerId}/community/sponsors/${sponsor.id}/edit`}
                                                    >
                                                        <Pencil className='h-4 w-4' />
                                                    </Link>
                                                </Button>
                                                {sponsor.isActive ? (
                                                    <AlertDialog>
                                                        <AlertDialogTrigger asChild>
                                                            <Button
                                                                variant='outline'
                                                                size='sm'
                                                                disabled={isDeactivating}
                                                            >
                                                                <Trash2 className='h-4 w-4' />
                                                            </Button>
                                                        </AlertDialogTrigger>
                                                        <AlertDialogContent>
                                                            <AlertDialogHeader>
                                                                <AlertDialogTitle>Deactivate Sponsor</AlertDialogTitle>
                                                                <AlertDialogDescription>
                                                                    Are you sure you want to deactivate &quot;
                                                                    {sponsor.name}&quot;? This will not delete the
                                                                    sponsor record.
                                                                </AlertDialogDescription>
                                                            </AlertDialogHeader>
                                                            <AlertDialogFooter>
                                                                <AlertDialogCancel>Cancel</AlertDialogCancel>
                                                                <AlertDialogAction
                                                                    onClick={() => handleDeactivate(sponsor.id)}
                                                                >
                                                                    Deactivate
                                                                </AlertDialogAction>
                                                            </AlertDialogFooter>
                                                        </AlertDialogContent>
                                                    </AlertDialog>
                                                ) : null}
                                            </div>
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    ) : (
                        <div className='text-center py-12'>
                            <HandCoins className='h-12 w-12 mx-auto text-muted-foreground mb-4' />
                            <h3 className='text-lg font-medium mb-2'>No sponsors yet</h3>
                            <p className='text-muted-foreground mb-4'>
                                Add your first sponsor to start tracking funded adoption programs.
                            </p>
                            <Button
                                onClick={() => navigate(`/partnerdashboard/${partnerId}/community/sponsors/create`)}
                            >
                                <Plus className='h-4 w-4 mr-2' />
                                Add First Sponsor
                            </Button>
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
};

export default PartnerCommunitySponsors;
