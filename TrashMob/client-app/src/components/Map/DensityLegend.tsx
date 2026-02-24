const DENSITY_SCALE = [
    { color: '#9E9E9E', label: 'No data' },
    { color: '#4CAF50', label: '< 5 g/m' },
    { color: '#8BC34A', label: '5-15 g/m' },
    { color: '#FFC107', label: '15-30 g/m' },
    { color: '#FF9800', label: '30-60 g/m' },
    { color: '#FF5722', label: '60-120 g/m' },
    { color: '#F44336', label: '120+ g/m' },
];

export const DensityLegend = () => (
    <div className='absolute bottom-4 left-4 bg-background/90 backdrop-blur-sm rounded-lg shadow-md p-3 text-xs'>
        <p className='font-semibold mb-1.5'>Litter Density</p>
        <div className='flex flex-col gap-1'>
            {DENSITY_SCALE.map(({ color, label }) => (
                <div key={color} className='flex items-center gap-2'>
                    <span className='inline-block w-4 h-1 rounded-full' style={{ backgroundColor: color }} />
                    <span className='text-muted-foreground'>{label}</span>
                </div>
            ))}
        </div>
    </div>
);
