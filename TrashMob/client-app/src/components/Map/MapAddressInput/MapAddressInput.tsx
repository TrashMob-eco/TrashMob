import { ReactChild, useCallback, useEffect, useState } from "react";
import { MapMouseEvent, Marker, MarkerProps, useMap } from '@vis.gl/react-google-maps';
import { GoogleMap } from "../GoogleMap"
import * as MapStore from '../../../store/MapStore';
import { useAzureMapSearchAddressReverse } from "../../../hooks/useAzureMapSearchAddressReverse";
import { AzureMapSearchAddressReverseResultItem } from "../../Models/AzureMapSearchAddressReverse";
import { AzureSearchLocationInput, SearchLocationOption } from "../AzureSearchLocationInput";

export type RenderMarkerProps = {
	position: google.maps.LatLngLiteral
	draggable: boolean
	onDragEnd: MarkerProps["onDragEnd"]
}

export type MapAddressInputValue = {
	lat: number
	lng: number
	address?: AzureMapSearchAddressReverseResultItem["address"]
}

export type MapAddressInputProps = {
	value: MapAddressInputValue
	onChange: (value: MapAddressInputValue) => void
	disableSearch: boolean
	customRenderMarker: (markerProps: RenderMarkerProps) => ReactChild
}

export const MapAddressInput = (props: MapAddressInputProps) => {
	const {
		value,
		onChange,
		disableSearch = false,
		customRenderMarker
	} = props
	const [azureSubscriptionKey, setAzureSubscriptionKey] = useState<string>();
	const [latitude, setLatitude] = useState<number>(0)
	const [longitude, setLongitude] = useState<number>(0)

	const map = useMap()

	useEffect(() => {
		MapStore.getOption().then((opts) => {
            setAzureSubscriptionKey(opts.subscriptionKey);
        });
	}, [])

	useEffect(() => {
		setLatitude(value.lat)
		setLongitude(value.lng)
	}, [value])

	const { refetch: refetchAddressReverse } = useAzureMapSearchAddressReverse(
        {
            lat: latitude,
            long: longitude,
            azureKey: azureSubscriptionKey || '',
        },
        { enabled: false },
    );

	const handleSelectSearchLocation = useCallback(
        async (location: SearchLocationOption) => {
            const { lat, lon } = location.position;
            setLatitude(lat);
            setLongitude(lon);

            // side effect: Move Map Center
            if (map) map.panTo({ lat, lng: lon });
        },
        [map],
    );

	const handleClickMap = useCallback((e: MapMouseEvent) => {
        if (e.detail.latLng) {
            const lat = e.detail.latLng.lat;
            const lng = e.detail.latLng.lng;
            setLatitude(lat);
            setLongitude(lng);
        }
    }, [])

    const handleMarkerDragEnd = useCallback((e: google.maps.MapMouseEvent) => {
        if (e.latLng) {
            const lat = e.latLng.lat();
            const lng = e.latLng.lng();
            setLatitude(lat);
            setLongitude(lng);
        }
    }, [])

    // on Marker moved (latitude + longitude changed), do reverse search lat,lng to address
    useEffect(() => {
        const searchAddressReverse = async () => {
            const { data } = await refetchAddressReverse();

            const firstResult = data?.addresses[0];
            if (firstResult) {
				onChange({
					lat: latitude,
					lng: longitude,
					address: firstResult.address
				})
            }
        };
        if (latitude && longitude) searchAddressReverse();
    }, [latitude, longitude]);
	
	return (
		<div style={{ position: 'relative', width: '100%' }}>
			<GoogleMap onClick={handleClickMap}>
				{(latitude && longitude) && (
					<>
						{typeof customRenderMarker === 'function' ? 
							customRenderMarker({ 
								position: { lat: latitude, lng: longitude },
								draggable: true,
								onDragEnd: handleMarkerDragEnd
							}) : (
								<Marker
									position={{ lat: latitude, lng: longitude }}
									draggable
									onDragEnd={handleMarkerDragEnd}
								/>
							)
						}
					</>
				)}				
			</GoogleMap>
			{!disableSearch && azureSubscriptionKey ? (
				<div style={{ position: 'absolute', top: 8, left: 8 }}>
					<AzureSearchLocationInput
						azureKey={azureSubscriptionKey}
						onSelectLocation={handleSelectSearchLocation}
					/>
				</div>
			) : null}
		</div>
	)
}
