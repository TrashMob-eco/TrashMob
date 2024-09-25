import { useQuery } from '@tanstack/react-query';
import { APIProvider, Map, MapProps } from '@vis.gl/react-google-maps';
import { PropsWithChildren } from 'react';
import { GetGoogleMapApiKey } from '../../../services/maps';

export const GoogleMap = (props: PropsWithChildren<MapProps>) => {

  const { data: apiKey, isSuccess } = useQuery({
    queryKey: GetGoogleMapApiKey().key,
    queryFn: GetGoogleMapApiKey().service,
    select: (res) => res.data,
  })

  if (!isSuccess) return <div>Fail to get key</div>

  return (
    <APIProvider apiKey={apiKey}>
      <Map
        mapId="6f295631d841c617"
        gestureHandling={'greedy'}
        disableDefaultUI={true}
        {...props}
      />
    </APIProvider>
  );
}
