import { Trash2 } from 'lucide-react';

interface LitterReportPinProps {
    color: string;
    size?: number;
}

export const LitterReportPin = ({ color, size = 32 }: LitterReportPinProps) => {
    return (
        <div
            className='flex items-center justify-center rounded-full border-2 border-white shadow-lg'
            style={{
                backgroundColor: color,
                width: size,
                height: size,
            }}
        >
            <Trash2 className='text-white' style={{ width: size * 0.5, height: size * 0.5 }} />
        </div>
    );
};

export const litterReportColors = {
    new: '#DC2626', // Red - New/Unassigned
    assigned: '#F59E0B', // Yellow/Amber - Assigned to event
    cleaned: '#16A34A', // Green - Cleaned/Resolved
    cancelled: '#6B7280', // Gray - Cancelled
};
