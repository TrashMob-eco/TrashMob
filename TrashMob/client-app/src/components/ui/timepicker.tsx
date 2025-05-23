import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import moment from 'moment';

interface TimePickerProps {
    value: string;
    onChange: (value: string) => void;
}

const generateTimeRange = (start: number, end: number): string[] => {
    const times = [];
    for (let hour = start; hour <= end; hour++) {
        times.push(`${hour}:00`);
        times.push(`${hour}:30`);
    }
    return times;
};

const timeRange = generateTimeRange(7, 20);

export const TimePicker = (props: TimePickerProps) => {
    return (
        <Select value={props.value} onValueChange={props.onChange}>
            <SelectTrigger className='w-full'>
                <SelectValue placeholder='When' />
            </SelectTrigger>
            <SelectContent>
                {timeRange.map((time) => (
                    <SelectItem key={time} value={time}>
                        {moment(time, ['HH:mm']).format('h:mm A')}
                    </SelectItem>
                ))}
            </SelectContent>
        </Select>
    );
};
