import { Link } from 'react-router-dom';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import DisplayPartnershipData from '@/components/Models/DisplayPartnershipData';
import { getDisplayPartnershipStatus } from '@/store/displayPartnershipStatusHelper';
import { useGetPartnerStatuses } from '@/hooks/useGetPartnerStatuses';
import { useGetPartnerRequestStatuses } from '@/hooks/useGetPartnerRequestStatuses';
import { Ellipsis, Eye } from 'lucide-react';

type MyPartnersRequestTableProps = {
    items: DisplayPartnershipData[];
};
export const MyPartnersRequestTable = ({ items }: MyPartnersRequestTableProps) => {
    const { data: partnerStatusList } = useGetPartnerStatuses();
    const { data: partnerRequestStatusList } = useGetPartnerRequestStatuses();

    const headerTitles = ['Name', 'Status', 'Actions'];
    return (
        <Table>
            <TableHeader>
                <TableRow>
                    {headerTitles.map((header) => (
                        <TableHead key={header}>{header}</TableHead>
                    ))}
                </TableRow>
            </TableHeader>
            <TableBody>
                {(items || [])
                    .sort((a, b) => (a.name < b.name ? 1 : -1))
                    .map((displayPartner) => (
                        <TableRow key={displayPartner.id.toString()}>
                            <TableCell>{displayPartner.name}</TableCell>
                            <TableCell>
                                {getDisplayPartnershipStatus(
                                    partnerStatusList || [],
                                    partnerRequestStatusList || [],
                                    displayPartner.partnerStatusId,
                                    displayPartner.partnerRequestStatusId,
                                )}
                            </TableCell>
                            <TableCell className='py-0'>
                                <DropdownMenu>
                                    <DropdownMenuTrigger asChild>
                                        <Button variant='ghost' size='icon'>
                                            <Ellipsis />
                                        </Button>
                                    </DropdownMenuTrigger>
                                    <DropdownMenuContent className='w-56'>
                                        <DropdownMenuItem asChild>
                                            <Link to={`/partnerRequestDetails/${displayPartner.id}`}>
                                                <Eye />
                                                View request form
                                            </Link>
                                        </DropdownMenuItem>
                                    </DropdownMenuContent>
                                </DropdownMenu>
                            </TableCell>
                        </TableRow>
                    ))}
            </TableBody>
        </Table>
    );
};
