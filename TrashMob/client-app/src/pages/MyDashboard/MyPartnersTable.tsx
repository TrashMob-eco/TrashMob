import { Link } from 'react-router';
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
import { Ellipsis, Pencil, SquareArrowRight } from 'lucide-react';
import { PartnerStatusActive } from '@/components/Models/Constants';

type MyPartnersTableProps = {
    items: DisplayPartnershipData[];
};

export const MyPartnersTable = ({ items }: MyPartnersTableProps) => {
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
                                        {displayPartner.partnerStatusId === PartnerStatusActive ? (
                                            <DropdownMenuItem asChild>
                                                <Link to={`/partnerdashboard/${displayPartner.id}`}>
                                                    <Pencil />
                                                    <span>Manage partnership</span>
                                                </Link>
                                            </DropdownMenuItem>
                                        ) : (
                                            <DropdownMenuItem asChild>
                                                <Link to={`/partnerdashboard/${displayPartner.id}/edit`}>
                                                    <SquareArrowRight />
                                                    <span>Activate Partnership</span>
                                                </Link>
                                            </DropdownMenuItem>
                                        )}
                                    </DropdownMenuContent>
                                </DropdownMenu>
                            </TableCell>
                        </TableRow>
                    ))}
            </TableBody>
        </Table>
    );
};
