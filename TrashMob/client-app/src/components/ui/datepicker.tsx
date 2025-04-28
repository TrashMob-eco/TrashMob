import * as React from 'react';
import moment from 'moment';
import { CalendarIcon } from 'lucide-react';

import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { Calendar } from '@/components/ui/calendar';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';

interface DatePickerProps {
    readonly value: Date;
    readonly onChange: (value?: Date) => void;
}

export function DatePicker(props: DatePickerProps) {
    const date = props.value;
    return (
        <Popover>
            <PopoverTrigger asChild>
                <Button
                    className={cn(
                        'w-full justify-start text-left font-normal border-input shadow-sm',
                        !date && 'text-muted-foreground',
                    )}
                    variant='outline'
                >
                    <CalendarIcon />
                    {date ? moment(date).format('L') : <span>Pick a date</span>}
                </Button>
            </PopoverTrigger>
            <PopoverContent className='w-auto p-0'>
                <Calendar initialFocus mode='single' onSelect={props.onChange} selected={date} />
            </PopoverContent>
        </Popover>
    );
}
