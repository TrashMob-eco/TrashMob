import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import UserData from '../Models/UserData';
import EventData from '../Models/EventData';

interface EventAttendeeTableProps {
    users: UserData[];
    event: EventData;
}

export const EventAttendeeTable = (props: EventAttendeeTableProps) => {
    const { users, event } = props;
    return (
        <div className='overflow-auto'>
            <Table className='table table-striped' aria-labelledby='tableLabel'>
                <TableHeader>
                    <TableRow className='bg-ice'>
                        <TableHead>User Name</TableHead>
                        <TableHead>City</TableHead>
                        <TableHead>Country</TableHead>
                        <TableHead>Member Since</TableHead>
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {(users || []).map((user) => (
                        <TableRow key={user.id.toString()}>
                            <TableCell>
                                {user.id === event.createdByUserId ? `${user.userName} (Lead)` : `${user.userName}`}
                            </TableCell>
                            <TableCell>{user.city}</TableCell>
                            <TableCell>{user.country}</TableCell>
                            <TableCell>{new Date(user.memberSince).toLocaleDateString()}</TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </div>
    );
};
