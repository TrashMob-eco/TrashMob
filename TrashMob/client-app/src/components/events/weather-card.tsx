import { useQuery } from '@tanstack/react-query';
import {
    Cloud,
    CloudDrizzle,
    CloudLightning,
    CloudRain,
    CloudSnow,
    Droplets,
    Sun,
    Thermometer,
    Wind,
} from 'lucide-react';
import { GetWeatherForecast, WeatherForecast } from '@/services/weather';

interface WeatherCardProps {
    latitude: number;
    longitude: number;
    eventDate: string;
    durationHours: number;
}

function getWeatherIcon(conditionCode: number | undefined) {
    if (conditionCode == null) return <Cloud className='h-8 w-8 text-muted-foreground' />;

    if (conditionCode === 0) return <Sun className='h-8 w-8 text-yellow-500' />;
    if (conditionCode <= 3) return <Cloud className='h-8 w-8 text-gray-400' />;
    if (conditionCode <= 48) return <Cloud className='h-8 w-8 text-gray-500' />;
    if (conditionCode <= 55) return <CloudDrizzle className='h-8 w-8 text-blue-400' />;
    if (conditionCode <= 65) return <CloudRain className='h-8 w-8 text-blue-500' />;
    if (conditionCode <= 77) return <CloudSnow className='h-8 w-8 text-blue-200' />;
    if (conditionCode <= 82) return <CloudRain className='h-8 w-8 text-blue-600' />;
    if (conditionCode <= 86) return <CloudSnow className='h-8 w-8 text-blue-300' />;
    return <CloudLightning className='h-8 w-8 text-yellow-600' />;
}

export function WeatherCard({ latitude, longitude, eventDate, durationHours }: WeatherCardProps) {
    const { data: forecast, isLoading } = useQuery({
        queryKey: GetWeatherForecast({ lat: latitude, lng: longitude, date: eventDate, durationHours }).key,
        queryFn: GetWeatherForecast({ lat: latitude, lng: longitude, date: eventDate, durationHours }).service,
        select: (res) => res.data,
        staleTime: 60 * 60 * 1000,
        enabled: !!latitude && !!longitude && !!eventDate,
    });

    if (isLoading) return null;
    if (!forecast?.isAvailable) {
        return (
            <div className='flex items-center gap-2 text-sm text-muted-foreground'>
                <Cloud className='h-4 w-4' />
                <span>Weather forecast not yet available for this date</span>
            </div>
        );
    }

    return <WeatherDisplay forecast={forecast} />;
}

function WeatherDisplay({ forecast }: { forecast: WeatherForecast }) {
    return (
        <div className='flex flex-wrap items-center gap-x-6 gap-y-2 rounded-lg border bg-muted/30 px-4 py-3'>
            <div className='flex items-center gap-3'>
                {getWeatherIcon(forecast.conditionCode)}
                <div>
                    <div className='text-lg font-semibold'>{Math.round(forecast.temperature ?? 0)}°F</div>
                    <div className='text-sm text-muted-foreground'>{forecast.conditionText}</div>
                </div>
            </div>
            {forecast.highTemperature != null && forecast.lowTemperature != null && (
                <div className='flex items-center gap-1.5 text-sm text-muted-foreground'>
                    <Thermometer className='h-4 w-4' />
                    <span>
                        {Math.round(forecast.lowTemperature)}° / {Math.round(forecast.highTemperature)}°
                    </span>
                </div>
            )}
            {forecast.precipitationChance != null && forecast.precipitationChance > 0 && (
                <div className='flex items-center gap-1.5 text-sm text-muted-foreground'>
                    <Droplets className='h-4 w-4' />
                    <span>{forecast.precipitationChance}% rain</span>
                </div>
            )}
            {forecast.windSpeed != null && (
                <div className='flex items-center gap-1.5 text-sm text-muted-foreground'>
                    <Wind className='h-4 w-4' />
                    <span>{Math.round(forecast.windSpeed)} mph</span>
                </div>
            )}
        </div>
    );
}
