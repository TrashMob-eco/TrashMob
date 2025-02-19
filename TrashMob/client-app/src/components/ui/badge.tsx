import * as React from 'react';
import { cva, type VariantProps } from 'class-variance-authority';

import { cn } from '@/lib/utils';

const badgeVariants = cva(
    'inline-flex items-center rounded-md px-2.5 py-1 text-xs font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 gap-2 [&_svg]:size-4 [&_svg]:shrink-0',
    {
        variants: {
            variant: {
                default: 'bg-newprimary/20 text-black hover:bg-newprimary/80',
                secondary: 'bg-secondary text-secondary-foreground hover:bg-secondary/80',
                destructive: 'bg-destructive text-destructive-foreground hover:bg-destructive/80',
                success: 'bg-[#ECFCCB] text-black',
                verified: 'bg-[#E7D8EA] text-black [&_svg]:text-[#610777]',
                outline: 'text-foreground',
            },
        },
        defaultVariants: {
            variant: 'default',
        },
    },
);

export interface BadgeProps extends React.HTMLAttributes<HTMLDivElement>, VariantProps<typeof badgeVariants> {}

function Badge({ className, variant, ...props }: BadgeProps) {
    return <div className={cn(badgeVariants({ variant }), className)} {...props} />;
}

function AttendingBadge(props: BadgeProps) {
    return (
        <Badge variant='success' {...props}>
            <svg width='14' height='14' viewBox='0 0 14 14' fill='none' xmlns='http://www.w3.org/2000/svg'>
                <path
                    fill-rule='evenodd'
                    clip-rule='evenodd'
                    d='M6.99961 13.3996C10.5342 13.3996 13.3996 10.5342 13.3996 6.99961C13.3996 3.46499 10.5342 0.599609 6.99961 0.599609C3.46499 0.599609 0.599609 3.46499 0.599609 6.99961C0.599609 10.5342 3.46499 13.3996 6.99961 13.3996ZM10.0849 5.55251C10.2798 5.28452 10.2205 4.90927 9.95251 4.71437C9.68452 4.51946 9.30927 4.57871 9.11437 4.84671L6.32736 8.67884L4.82387 7.17534C4.58956 6.94103 4.20966 6.94103 3.97535 7.17534C3.74103 7.40966 3.74103 7.78956 3.97535 8.02387L5.97535 10.0239C6.09942 10.148 6.2716 10.2115 6.44654 10.1978C6.62148 10.184 6.78164 10.0944 6.88485 9.95251L10.0849 5.55251Z'
                    fill='#005B4C'
                />
            </svg>
            Attending
        </Badge>
    );
}

function HostingBadge(props: BadgeProps) {
    return (
        <Badge variant='verified' {...props}>
            <svg width='14' height='14' viewBox='0 0 14 14' fill='none' xmlns='http://www.w3.org/2000/svg'>
                <path
                    fill-rule='evenodd'
                    clip-rule='evenodd'
                    d='M6.72848 0.789059C6.88528 0.658855 7.11393 0.658855 7.27074 0.789059C8.82176 2.07694 10.7848 2.88578 12.9329 2.98861C13.126 2.99785 13.2909 3.13731 13.3162 3.32895C13.3712 3.74483 13.3996 4.16912 13.3996 4.60003C13.3996 8.72997 10.7917 12.2508 7.13285 13.6056C7.04706 13.6373 6.95243 13.6373 6.86664 13.6056C3.20764 12.2509 0.599609 8.72998 0.599609 4.59994C0.599609 4.16906 0.627996 3.74481 0.683002 3.32895C0.70835 3.13732 0.873222 2.99785 1.06631 2.98861C3.21441 2.88579 5.17745 2.07695 6.72848 0.789059ZM10.0849 5.55271C10.2798 5.28472 10.2205 4.90947 9.95251 4.71456C9.68452 4.51966 9.30927 4.57891 9.11437 4.8469L6.32736 8.67903L4.82387 7.17554C4.58956 6.94123 4.20966 6.94123 3.97535 7.17554C3.74103 7.40986 3.74103 7.78975 3.97535 8.02407L5.97535 10.0241C6.09942 10.1481 6.2716 10.2117 6.44654 10.198C6.62148 10.1842 6.78164 10.0946 6.88485 9.95271L10.0849 5.55271Z'
                    fill='#610777'
                />
            </svg>
            Hosting
        </Badge>
    );
}

export { Badge, AttendingBadge, HostingBadge, badgeVariants };
