import * as React from 'react';
import { DayPicker, getDefaultClassNames } from 'react-day-picker';

import { cn } from '@/lib/utils';
import { buttonVariants } from '@/components/ui/button';
import { ChevronLeftIcon, ChevronRightIcon } from '@radix-ui/react-icons';

export type CalendarProps = React.ComponentProps<typeof DayPicker>;

function Calendar({ className, classNames, showOutsideDays = true, ...props }: CalendarProps) {
    const defaultClassNames = getDefaultClassNames();

    return (
        <DayPicker
            showOutsideDays={showOutsideDays}
            className={cn('p-3', className)}
            classNames={{
                months: cn('relative flex flex-col sm:flex-row gap-4', defaultClassNames.months),
                month: cn('flex flex-col gap-4', defaultClassNames.month),
                month_caption: cn('flex justify-center pt-1 relative items-center', defaultClassNames.month_caption),
                caption_label: cn('text-sm font-medium', defaultClassNames.caption_label),
                dropdowns: cn('flex items-center gap-2', defaultClassNames.dropdowns),
                dropdown: cn(
                    'appearance-none rounded-md border border-input bg-background px-2 py-1 text-sm font-medium shadow-xs focus:outline-none focus:ring-1 focus:ring-ring',
                    defaultClassNames.dropdown,
                ),
                months_dropdown: cn('[&>span]:sr-only', defaultClassNames.months_dropdown),
                years_dropdown: cn('[&>span]:sr-only', defaultClassNames.years_dropdown),
                nav: cn(
                    'absolute inset-x-0 top-0 z-10 flex w-full items-center justify-between',
                    defaultClassNames.nav,
                ),
                button_previous: cn(
                    buttonVariants({ variant: 'outline' }),
                    'h-7 w-7 bg-transparent p-0 opacity-50 hover:opacity-100',
                    defaultClassNames.button_previous,
                ),
                button_next: cn(
                    buttonVariants({ variant: 'outline' }),
                    'h-7 w-7 bg-transparent p-0 opacity-50 hover:opacity-100',
                    defaultClassNames.button_next,
                ),
                month_grid: cn('w-full border-collapse', defaultClassNames.month_grid),
                weekdays: cn('flex', defaultClassNames.weekdays),
                weekday: cn(
                    'text-muted-foreground rounded-md w-8 flex-1 font-normal text-[0.8rem]',
                    defaultClassNames.weekday,
                ),
                week: cn('flex w-full mt-2', defaultClassNames.week),
                day: cn(
                    'relative p-0 text-center text-sm flex-1 focus-within:relative focus-within:z-20 [&:has([aria-selected])]:bg-accent [&:has([aria-selected].day-outside)]:bg-accent/50 [&:has([aria-selected].day-range-end)]:rounded-r-md',
                    props.mode === 'range'
                        ? '[&:has(>.day-range-end)]:rounded-r-md [&:has(>.day-range-start)]:rounded-l-md first:[&:has([aria-selected])]:rounded-l-md last:[&:has([aria-selected])]:rounded-r-md'
                        : '[&:has([aria-selected])]:rounded-md',
                    defaultClassNames.day,
                ),
                day_button: cn(
                    buttonVariants({ variant: 'ghost' }),
                    'h-8 w-8 p-0 font-normal aria-selected:opacity-100',
                    defaultClassNames.day_button,
                ),
                range_start: cn('day-range-start', defaultClassNames.range_start),
                range_end: cn('day-range-end', defaultClassNames.range_end),
                selected: cn(
                    'bg-primary text-primary-foreground hover:bg-primary hover:text-primary-foreground focus:bg-primary focus:text-primary-foreground',
                    defaultClassNames.selected,
                ),
                today: cn('bg-accent text-accent-foreground', defaultClassNames.today),
                outside: cn(
                    'day-outside text-muted-foreground aria-selected:bg-accent/50 aria-selected:text-muted-foreground',
                    defaultClassNames.outside,
                ),
                disabled: cn('text-muted-foreground opacity-50', defaultClassNames.disabled),
                range_middle: cn(
                    'aria-selected:bg-accent aria-selected:text-accent-foreground',
                    defaultClassNames.range_middle,
                ),
                hidden: cn('invisible', defaultClassNames.hidden),
                ...classNames,
            }}
            components={{
                Chevron: ({ orientation, className, ...rest }) => {
                    const Icon = orientation === 'left' ? ChevronLeftIcon : ChevronRightIcon;
                    return <Icon className={cn('h-4 w-4', className)} {...rest} />;
                },
            }}
            {...props}
        />
    );
}
Calendar.displayName = 'Calendar';

export { Calendar };
