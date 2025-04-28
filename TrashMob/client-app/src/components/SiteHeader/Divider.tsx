import { cn } from '@/lib/utils';

export const Divider = ({ className, ...props }: React.HTMLAttributes<HTMLElement>) => (
    <div className={cn('w-[2px] bg-[#B0CCCB] h-0 lg:h-6', className)} />
);
