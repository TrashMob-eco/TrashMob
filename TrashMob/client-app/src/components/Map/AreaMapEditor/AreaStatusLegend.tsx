export const AreaStatusLegend = () => (
    <div className='flex gap-4 text-xs text-muted-foreground'>
        <span className='flex items-center gap-1.5'>
            <span className='inline-block w-2.5 h-2.5 rounded-full bg-green-600' />
            <span className='inline-block w-4 h-0.5 bg-current opacity-70' />
            Available
        </span>
        <span className='flex items-center gap-1.5'>
            <span className='inline-block w-2.5 h-2.5 rounded-full bg-blue-600' />
            <span className='inline-block w-4 h-1 bg-current opacity-90' />
            Adopted
        </span>
        <span className='flex items-center gap-1.5'>
            <span className='inline-block w-2.5 h-2.5 rounded-full bg-gray-500' />
            <span className='inline-block w-4 h-px bg-current opacity-40' />
            Unavailable
        </span>
    </div>
);
