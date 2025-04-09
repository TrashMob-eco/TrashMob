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
import PartnerDocumentData from '@/components/Models/PartnerDocumentData';

export const PartnerDocuments = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const isEdit = useMatch(`/partnerdashboard/:partnerId/documents/:documentId/edit`);
    const isCreate = useMatch(`/partnerdashboard/:partnerId/documents/create`);

    const { data: documents } = useQuery<AxiosResponse<PartnerDocumentData[]>, unknown, PartnerDocumentData[]>({
        queryKey: GetPartnerDocumentsByPartnerId({ partnerId }).key,
        queryFn: GetPartnerDocumentsByPartnerId({ partnerId }).service,
        select: (res) => res.data,
    });

    const deletePartnerDocumentByDocuemntId = useMutation({
        mutationKey: DeletePartnerDocumentByDocuemntId().key,
        mutationFn: DeletePartnerDocumentByDocuemntId().service,
    });

    const [isDeletingId, setIsDeletingId] = useState<string | null>(null);
    const removeDocument = (documentId: string, documentName: string) => {
        setIsDeletingId(documentId);
        if (
            !window.confirm(
                `Please confirm that you want to remove document with name: '${
                    documentName
                }' as a document from this Partner?`,
            )
        )
            return;

        deletePartnerDocumentByDocuemntId
            .mutateAsync({ documentId })
            .then(async () => {
                return queryClient.invalidateQueries({
                    queryKey: GetPartnerDocumentsByPartnerId({ partnerId }).key,
                    refetchType: 'all',
                });
            })
            .then(() => {
                setIsDeletingId(null);
            });
    };

    return (
        <SidebarLayout
            title='Edit Partner Documents'
            description='This page allows you and the TrashMob administrators to track documents relevant to the partnership. i.e. Volunteer Organizational Agreements or special waivers if needed. Note that this page will have more functionality added in the future to allow uploading filled out documents.'
        >
            <div>
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Name</TableHead>
                            <TableHead>Url</TableHead>
                            <TableHead>Actions</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(documents || []).map((row) => (
                            <TableRow key={row.id} className={isDeletingId === row.id ? 'opacity-20' : ''}>
                                <TableCell>{row.name}</TableCell>
                                <TableCell>{row.url}</TableCell>
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
                                                    Edit Document
                                                </Link>
                                            </DropdownMenuItem>
                                            <DropdownMenuItem onClick={() => removeDocument(row.id, row.name)}>
                                                <SquareX />
                                                Remove Document
                                            </DropdownMenuItem>
                                        </DropdownMenuContent>
                                    </DropdownMenu>
                                </TableCell>
                            </TableRow>
                        ))}
                        <TableRow>
                            <TableCell colSpan={3}>
                                <Button variant='ghost' className='w-full' asChild>
                                    <Link to='create'>
                                        <Plus /> Add Document
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
                            <DialogTitle>Edit Document</DialogTitle>
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
                            <DialogTitle>Add Document</DialogTitle>
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
