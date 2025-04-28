import { Link } from 'react-router';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import { GetEventPickupLocationsByUser, PickupLocationMarkAsPickedUp } from '@/services/locations';

import { Ellipsis, FileCheck, SquareCheck } from 'lucide-react';
import PickupLocationData from '@/components/Models/PickupLocationData';
import { useMutation, useQueryClient } from '@tanstack/react-query';

type MyPicupRequestsTableProps = {
    readonly userId: string;
    readonly items: PickupLocationData[];
};

export const MyPickupRequestsTable = ({ items, userId }: MyPicupRequestsTableProps) => {
    const queryClient = useQueryClient();

    const { mutate } = useMutation({
        mutationKey: PickupLocationMarkAsPickedUp().key,
        mutationFn: PickupLocationMarkAsPickedUp().service,
        onSuccess: () => {
            queryClient.invalidateQueries([GetEventPickupLocationsByUser({ userId }).key]);
        },
    });

    const headerTitles = ['Street Address', 'City', 'Notes', 'Actions'];

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
                {(items || []).map((displayPickup) => (
                    <TableRow key={displayPickup.id.toString()}>
                        <TableCell>{displayPickup.name}</TableCell>
                        <TableCell>{displayPickup.streetAddress}</TableCell>
                        <TableCell>{displayPickup.city}</TableCell>
                        <TableCell>{displayPickup.notes}</TableCell>
                        <TableCell className='py-0'>
                            <DropdownMenu>
                                <DropdownMenuTrigger asChild>
                                    <Button size='icon' variant='ghost'>
                                        <Ellipsis />
                                    </Button>
                                </DropdownMenuTrigger>
                                <DropdownMenuContent className='w-56'>
                                    <DropdownMenuItem asChild>
                                        <Link to={`/eventsummary/${displayPickup.eventId}`}>
                                            <FileCheck />
                                            <span>Event Summary</span>
                                        </Link>
                                    </DropdownMenuItem>
                                    <DropdownMenuItem onClick={() => mutate({ locationId: displayPickup.id })}>
                                        <SquareCheck />
                                        <span>Marked picked up</span>
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
