import * as React from 'react';
import moment from 'moment';
import { CalendarIcon } from 'lucide-react';

import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { Calendar, type CalendarProps } from '@/components/ui/calendar';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';

interface DatePickerProps {
    value: Date;
    onChange: (value?: Date) => void;
    placeholder?: string;
    calendarProps?: Omit<CalendarProps, 'mode' | 'selected' | 'onSelect'>;
}

export function DatePicker({ value, onChange, placeholder = 'Pick a date', calendarProps }: DatePickerProps) {
    return (
        <Popover>
            <PopoverTrigger asChild>
                <Button
                    variant='outline'
                    className={cn(
                        'w-full justify-start text-left font-normal border-input shadow-xs',
                        !value && 'text-muted-foreground',
                    )}
                >
                    <CalendarIcon />
                    {value ? moment(value).format('L') : <span>{placeholder}</span>}
                </Button>
            </PopoverTrigger>
            <PopoverContent className='w-auto p-0'>
                <Calendar mode='single' selected={value} onSelect={onChange} initialFocus {...calendarProps} />
            </PopoverContent>
        </Popover>
    );
}
