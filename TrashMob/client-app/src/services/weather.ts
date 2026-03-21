import { ApiService } from '.';

export interface WeatherForecast {
    isAvailable: boolean;
    temperature?: number;
    conditionCode?: number;
    conditionText?: string;
    precipitationChance?: number;
    windSpeed?: number;
    highTemperature?: number;
    lowTemperature?: number;
}

export type GetWeatherForecast_Params = {
    lat: number;
    lng: number;
    date: string;
    durationHours: number;
};

export const GetWeatherForecast = (params: GetWeatherForecast_Params) => ({
    key: ['/weather/forecast', params.lat, params.lng, params.date],
    service: async () =>
        ApiService('public').fetchData<WeatherForecast>({
            url: `/v2/weather/forecast?lat=${params.lat}&lng=${params.lng}&date=${encodeURIComponent(params.date)}&durationHours=${params.durationHours}`,
            method: 'get',
        }),
});
