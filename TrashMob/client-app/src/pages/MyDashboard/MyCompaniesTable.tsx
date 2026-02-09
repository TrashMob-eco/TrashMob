import { Link } from 'react-router';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import ProfessionalCompanyData from '@/components/Models/ProfessionalCompanyData';
import { Ellipsis, Pencil } from 'lucide-react';

type MyCompaniesTableProps = {
    items: ProfessionalCompanyData[];
};

export const MyCompaniesTable = ({ items }: MyCompaniesTableProps) => {
    const headerTitles = ['Company Name', 'Actions'];
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
                    .map((company) => (
                        <TableRow key={company.id}>
                            <TableCell>{company.name}</TableCell>
                            <TableCell className='py-0'>
                                <DropdownMenu>
                                    <DropdownMenuTrigger asChild>
                                        <Button variant='ghost' size='icon'>
                                            <Ellipsis />
                                        </Button>
                                    </DropdownMenuTrigger>
                                    <DropdownMenuContent className='w-56'>
                                        <DropdownMenuItem asChild>
                                            <Link to={`/companydashboard/${company.id}`}>
                                                <Pencil />
                                                <span>Manage company</span>
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
