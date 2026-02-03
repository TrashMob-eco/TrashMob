interface LocationPinProps {
    color: string;
    size?: number;
}

export const LocationPin = ({ color, size = 36 }: LocationPinProps) => {
    return (
        <svg width={size} height={size} viewBox='0 0 96 96' fill='none'>
            <path
                fill={color}
                d='M48 8C32 8 20 20.5 20 36C20 57 48 88 48 88C48 88 76 57 76 36C76 20.5 64 8 48 8ZM48 46C42.5 46 38 41.5 38 36C38 30.5 42.5 26 48 26C53.5 26 58 30.5 58 36C58 41.5 53.5 46 48 46Z'
            />
            <path
                stroke='black'
                strokeWidth='2'
                d='M48 8C32 8 20 20.5 20 36C20 57 48 88 48 88C48 88 76 57 76 36C76 20.5 64 8 48 8ZM48 46C42.5 46 38 41.5 38 36C38 30.5 42.5 26 48 26C53.5 26 58 30.5 58 36C58 41.5 53.5 46 48 46Z'
            />
            <circle cx='48' cy='36' r='6' fill='white' />
        </svg>
    );
};

export const locationPinColors = {
    active: '#16A34A', // green-600
    inactive: '#9CA3AF', // gray-400
    primary: '#2563EB', // blue-600
};
