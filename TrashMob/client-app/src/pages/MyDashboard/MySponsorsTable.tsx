import { Link } from 'react-router';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import SponsorData from '@/components/Models/SponsorData';
import { Ellipsis, Eye } from 'lucide-react';

type MySponsorsTableProps = {
    items: SponsorData[];
};

export const MySponsorsTable = ({ items }: MySponsorsTableProps) => {
    const headerTitles = ['Sponsor Name', 'Actions'];
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
                    .sort((a, b) => a.name.localeCompare(b.name))
                    .map((sponsor) => (
                        <TableRow key={sponsor.id}>
                            <TableCell>{sponsor.name}</TableCell>
                            <TableCell className='py-0'>
                                <DropdownMenu>
                                    <DropdownMenuTrigger asChild>
                                        <Button variant='ghost' size='icon'>
                                            <Ellipsis />
                                        </Button>
                                    </DropdownMenuTrigger>
                                    <DropdownMenuContent className='w-56'>
                                        <DropdownMenuItem asChild>
                                            <Link to={`/sponsordashboard/${sponsor.id}`}>
                                                <Eye />
                                                <span>View sponsor</span>
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
