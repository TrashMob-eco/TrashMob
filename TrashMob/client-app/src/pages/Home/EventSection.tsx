import * as MapStore from '@/store/MapStore';
import { AzureSearchLocationInput, SearchLocationOption } from "@/components/Map/AzureSearchLocationInput"
import { EventsMap } from '@/components/Map';
import { Button } from '@/components/ui/button';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import { APIProvider } from '@vis.gl/react-google-maps';
import { useCallback, useEffect, useState } from "react"
import UserData from '@/components/Models/UserData';
import { useQuery } from '@tanstack/react-query';
import { GetAllActiveEvents } from '@/services/events';

interface EventSectionProps {
  isUserLoaded: boolean
  currentUser: UserData
}

export const EventSection = (props: EventSectionProps) => {
  const [azureSubscriptionKey, setAzureSubscriptionKey] = useState<string>('')
  useEffect(() => {
    MapStore.getOption().then((opts) => {
      setAzureSubscriptionKey(opts.subscriptionKey);
    });
  })

  /** Event List */
  const [events, setEvents] = useState([])
  const { refetch } = useQuery({
    queryKey: GetAllActiveEvents().key,
    queryFn: GetAllActiveEvents().service,
    staleTime: 100,
    enabled: false,
  });

  useEffect(() => {
    refetch().then(res => {
      setEvents(res.data?.data || []);
    })
  }, [])

  const handleSelectLocation = useCallback(
    async (location: SearchLocationOption) => {
        const { lat, lon } = location.position;
        console.log({ lat, lon })       
    },
    []
  );

  return (
    <div className="bg-card">
      <div className="container !py-20">
        <div className="flex flex-col gap-2">
          <div className="flex flex-row items-center gap-4">
            <h3 className="my-0">Upcoming Events near</h3>
            <AzureSearchLocationInput
              azureKey={azureSubscriptionKey}
              renderInput={({ inputRef, referenceElementRef, ...inputProps }) => (
                <div className="w-fit">
                  <input
                    type="text"
                    {...inputProps}
                    className="w-72 text-primary font-semibold text-4xl mb-0 !py-2 border-b-4 border-primary"
                    ref={(input) => {
                      inputRef(input);
                      referenceElementRef(input);
                    }}
                  />
                  <Button variant="ghost" className="w-12 h-12 text-primary">
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="!w-8 !h-8">
                      <path strokeLinecap="round" strokeLinejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0 1 15.75 21H5.25A2.25 2.25 0 0 1 3 18.75V8.25A2.25 2.25 0 0 1 5.25 6H10" />
                    </svg>
                  </Button>
                </div>
              )}
              onSelectLocation={handleSelectLocation}
            />
          </div>
          <EventsMap
            events={events}
            isUserLoaded={props.isUserLoaded}
            currentUser={props.currentUser}
          />
        </div>
      </div>
    </div>
  )
}

const EventSectionWrapper = (props: EventSectionProps) => {
  const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

  if (isLoading) return null;

  return (
      <APIProvider apiKey={googleApiKey || ''}>
          <EventSection {...props} />
      </APIProvider>
  );
};

export default EventSectionWrapper