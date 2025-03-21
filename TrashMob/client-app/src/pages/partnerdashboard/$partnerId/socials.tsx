import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, Outlet, useMatch, useNavigate, useParams } from 'react-router';
import { AxiosResponse } from 'axios';
import { Ellipsis, Pencil, Plus, SquareX } from 'lucide-react';

import { SidebarLayout } from '../../layouts/_layout.sidebar';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';

import { DeletePartnerDocumentByDocuemntId, GetPartnerDocumentsByPartnerId } from '@/services/documents';
import {
    DeletePartnerSocialMediaAccountById,
    GetPartnerSocialMediaAccountsByPartnerId,
    GetSocialMediaAccountTypes,
} from '@/services/social-media';
import PartnerSocialMediaAccountData from '@/components/Models/PartnerSocialMediaAccountData';
import { Badge } from '@/components/ui/badge';
import SocialMediaAccountTypeData from '@/components/Models/SocialMediaAccountTypeData';

export const PartnerSocialMediaAccounts = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const isEdit = useMatch(`/partnerdashboard/:partnerId/socials/:accountId/edit`);
    const isCreate = useMatch(`/partnerdashboard/:partnerId/socials/create`);

    const { data: accountTypes } = useQuery({
        queryKey: GetSocialMediaAccountTypes().key,
        queryFn: GetSocialMediaAccountTypes().service,
        select: (res) => res.data,
    });

    function getSocialMediaAccountType(id: number): SocialMediaAccountTypeData {
        const socialMediaAccountType = (accountTypes || []).find((accountType) => accountType.id === id);
        return socialMediaAccountType || ({ name: 'Unknown' } as SocialMediaAccountTypeData);
    }

    const { data: socialAccounts } = useQuery<
        AxiosResponse<PartnerSocialMediaAccountData[]>,
        unknown,
        PartnerSocialMediaAccountData[]
    >({
        queryKey: GetPartnerSocialMediaAccountsByPartnerId({ partnerId }).key,
        queryFn: GetPartnerSocialMediaAccountsByPartnerId({ partnerId }).service,
        select: (res) => res.data,
    });

    const deletePartnerSocialMediaAccountById = useMutation({
        mutationKey: DeletePartnerSocialMediaAccountById().key,
        mutationFn: DeletePartnerSocialMediaAccountById().service,
    });

    const [isDeletingId, setIsDeletingId] = useState<string | null>(null);
    const removeItem = (accountId: string, accountName: string) => {
        setIsDeletingId(accountId);
        if (
            !window.confirm(
                `Please confirm that you want to remove social media account with name: '${
                    accountName
                }' as a social media account from this Partner?`,
            )
        )
            return;

        deletePartnerSocialMediaAccountById
            .mutateAsync({ accountId })
            .then(async () => {
                return queryClient.invalidateQueries({
                    queryKey: GetPartnerSocialMediaAccountsByPartnerId({ partnerId }).key,
                    refetchType: 'all',
                });
            })
            .then(() => {
                setIsDeletingId(null);
            });
    };

    return (
        <SidebarLayout
            title='Edit Partner Social Media Accounts'
            description='This page allows you to add a list of social media accounts you would like to have tagged when you approve a partnership request to both help spread the word about what TrashMob.eco users are doing within your community, and how your organization is helping your community. This feature is still in development, but adding the information when you set things up now will help when this feature fully launches.'
        >
            <div>
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Account Type</TableHead>
                            <TableHead>Account Name</TableHead>
                            <TableHead>Is Active</TableHead>
                            <TableHead>Actions</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(socialAccounts || []).map((row) => (
                            <TableRow key={row.id} className={isDeletingId === row.id ? 'opacity-20' : ''}>
                                <TableCell>{getSocialMediaAccountType(row.socialMediaAccountTypeId).name}</TableCell>
                                <TableCell>{row.accountIdentifier}</TableCell>
                                <TableCell>
                                    {row.isActive ? (
                                        <Badge variant='success'>Active</Badge>
                                    ) : (
                                        <Badge variant='secondary'>Inactive</Badge>
                                    )}
                                </TableCell>

                                <TableCell>
                                    <DropdownMenu>
                                        <DropdownMenuTrigger asChild>
                                            <Button variant='ghost' size='icon'>
                                                <Ellipsis />
                                            </Button>
                                        </DropdownMenuTrigger>
                                        <DropdownMenuContent className='w-56'>
                                            <DropdownMenuItem asChild>
                                                <Link to={`${row.id}/edit`}>
                                                    <Pencil />
                                                    Edit
                                                </Link>
                                            </DropdownMenuItem>
                                            <DropdownMenuItem onClick={() => removeItem(row.id, row.accountIdentifier)}>
                                                <SquareX />
                                                Remove
                                            </DropdownMenuItem>
                                        </DropdownMenuContent>
                                    </DropdownMenu>
                                </TableCell>
                            </TableRow>
                        ))}
                        <TableRow>
                            <TableCell colSpan={4}>
                                <Button variant='ghost' className='w-full' asChild>
                                    <Link to='create'>
                                        <Plus /> Add Social Media Account
                                    </Link>
                                </Button>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
                <Dialog open={!!isEdit} onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/locations`)}>
                    <DialogContent
                        className='sm:max-w-[600px] overflow-y-scroll max-h-screen'
                        onOpenAutoFocus={(e) => e.preventDefault()}
                    >
                        <DialogHeader>
                            <DialogTitle>Edit Account</DialogTitle>
                        </DialogHeader>
                        <div>
                            <Outlet />
                        </div>
                    </DialogContent>
                </Dialog>
                <Dialog open={!!isCreate} onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/locations`)}>
                    <DialogContent
                        className='sm:max-w-[600px] overflow-y-scroll max-h-screen'
                        onOpenAutoFocus={(e) => e.preventDefault()}
                    >
                        <DialogHeader>
                            <DialogTitle>Add Account</DialogTitle>
                        </DialogHeader>
                        <div>
                            <Outlet />
                        </div>
                    </DialogContent>
                </Dialog>
            </div>
        </SidebarLayout>
    );
};
