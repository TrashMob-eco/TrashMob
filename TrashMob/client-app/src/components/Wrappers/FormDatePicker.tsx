import { format } from 'date-fns';
import { CalendarIcon } from 'lucide-react';

import { cn } from '@/lib/utils';
import { Button } from '@/components/ui/button';
import { Calendar } from '@/components/ui/calendar';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';

interface DatePickerWrapperProps {
    control: any;
    name: string;
    label?: string;
    description?: string;
    placeholder?: string;
}

const FormDatePicker = ({ control, name, label, description, placeholder }: DatePickerWrapperProps) => {
    return (
        <TooltipProvider>
            <FormField
                control={control}
                name={name}
                render={({ field }) => (
                    <FormItem className='flex flex-col'>
                        <Tooltip>
                            <TooltipTrigger asChild>
                                <FormLabel>{label}</FormLabel>
                            </TooltipTrigger>
                            {description && (
                                <TooltipContent>
                                    <p>{description}</p>
                                </TooltipContent>
                            )}
                        </Tooltip>
                        <Popover>
                            <PopoverTrigger asChild>
                                <FormControl>
                                    <Button
                                        variant={'outline'}
                                        className={cn(
                                            'w-[240px] pl-3 text-left font-normal',
                                            !field.value && 'text-muted-foreground',
                                        )}
                                    >
                                        {field.value ? format(field.value, 'PPP') : <span>Pick a date</span>}
                                        <CalendarIcon className='ml-auto h-4 w-4 opacity-50' />
                                    </Button>
                                </FormControl>
                            </PopoverTrigger>
                            <PopoverContent className='w-auto p-0' align='start'>
                                <Calendar
                                    mode='single'
                                    selected={field.value}
                                    onSelect={field.onChange}
                                    disabled={(date) => date > new Date() || date < new Date('1900-01-01')}
                                    initialFocus
                                />
                            </PopoverContent>
                        </Popover>
                        <FormMessage />
                    </FormItem>
                )}
            />
        </TooltipProvider>
    );
};

export default FormDatePicker;
