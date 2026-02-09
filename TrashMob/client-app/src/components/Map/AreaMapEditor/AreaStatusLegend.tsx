export const AreaStatusLegend = () => (
    <div className='flex gap-3 text-xs text-muted-foreground'>
        <span className='flex items-center gap-1'>
            <span className='inline-block w-2.5 h-2.5 rounded-full bg-green-600' /> Available
        </span>
        <span className='flex items-center gap-1'>
            <span className='inline-block w-2.5 h-2.5 rounded-full bg-blue-600' /> Adopted
        </span>
        <span className='flex items-center gap-1'>
            <span className='inline-block w-2.5 h-2.5 rounded-full bg-gray-500' /> Unavailable
        </span>
    </div>
);
