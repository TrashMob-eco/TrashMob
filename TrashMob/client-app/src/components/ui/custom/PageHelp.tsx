import * as React from 'react';
import { CircleHelp } from 'lucide-react';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Button } from '@/components/ui/button';

interface PageHelpProps {
    children: React.ReactNode;
    side?: 'top' | 'right' | 'bottom' | 'left';
}

export const PageHelp: React.FC<PageHelpProps> = ({ children, side = 'bottom' }) => {
    return (
        <Popover>
            <PopoverTrigger asChild>
                <Button variant='ghost' size='icon' className='h-8 w-8' aria-label='Show page help'>
                    <CircleHelp className='h-5 w-5 text-muted-foreground' />
                </Button>
            </PopoverTrigger>
            <PopoverContent side={side} align='end' className='w-80 max-w-sm text-sm'>
                {children}
            </PopoverContent>
        </Popover>
    );
};
