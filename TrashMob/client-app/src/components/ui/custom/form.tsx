import * as React from 'react';
import * as LabelPrimitive from '@radix-ui/react-label';

import { cn } from '@/lib/utils';
import { useFormField } from '@/components/ui/form';
import { Label } from '@/components/ui/label';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';

/**
 * Enhanced FormLabel
 * ==================================
 * - Can show tooltip
 * - Can show required asterisk
 */
type EnhancedFormLabelProps = React.ComponentPropsWithoutRef<typeof LabelPrimitive.Root> & {
    tooltip?: string;
    required?: boolean;
};

const EnhancedFormLabel = React.forwardRef<React.ElementRef<typeof LabelPrimitive.Root>, EnhancedFormLabelProps>(
    ({ className, ...props }, ref) => {
        const { error, formItemId } = useFormField();
        const { required = false, tooltip } = props;
        if (tooltip) {
            return (
                <TooltipProvider>
                    <Tooltip>
                        <TooltipTrigger>
                            <Label
                                ref={ref}
                                className={cn(error && 'text-destructive', className)}
                                htmlFor={formItemId}
                                {...props}
                            />
                            {required ? <span className='text-destructive'>*</span> : ''}
                        </TooltipTrigger>
                        <TooltipContent className='max-w-64'>{tooltip}</TooltipContent>
                    </Tooltip>
                </TooltipProvider>
            );
        }

        return (
            <Label ref={ref} className={cn(error && 'text-destructive', className)} htmlFor={formItemId} {...props} />
        );
    },
);
EnhancedFormLabel.displayName = 'EnhancedFormLabel';

export { EnhancedFormLabel };
