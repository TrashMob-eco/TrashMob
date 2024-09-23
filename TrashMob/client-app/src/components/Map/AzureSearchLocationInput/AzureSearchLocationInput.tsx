import React from 'react'
import { useQuery } from '@tanstack/react-query';
import { AsyncTypeahead } from 'react-bootstrap-typeahead';
import { Option } from 'react-bootstrap-typeahead/types/types';
import { data } from 'azure-maps-control';
import { AzureMapSearchAddress } from '../../../services/maps';

const PER_PAGE = 50

export type AzureSearchLocationInputProps = {
  azureKey: string
  onSelectLocation: (position: data.Position) => void
}
 
type SearchLocationOption = {
  id: string
  displayAddress: string
  position: {
    lat: number
    lon: number
  }
}

export const AzureSearchLocationInput = (props: AzureSearchLocationInputProps) => {
  const { azureKey, onSelectLocation } = props
  const [query, setQuery] = React.useState('');

  const handleInputChange = (q: string) => {
    setQuery(q);
  }

  const { data: searchResult, isLoading, refetch } = useQuery<{ options: SearchLocationOption[], totalResults: number }>({
    queryKey: AzureMapSearchAddress().key(query),
    queryFn: async () => {
      const response = await AzureMapSearchAddress().service({ azureKey, query })
      const data = response.data
      return {
        options: data.results.map((item) => ({
          id: item.id,
          displayAddress: item.address.freeformAddress,
          position: item.position
        })),
        totalResults: data.summary.totalResults
      }
    },
    initialData: () => ({ options: [], totalResults: 0 }),
    enabled: false
  })

  const handleSearch = React.useCallback((q) => {
    refetch()
  }, []);

  function handleSelectedChanged(selected: Option[]) {
    if (selected && selected.length > 0) {
        const item = selected[0] as SearchLocationOption
        var position = item.position;
        var point = new data.Position(position.lon, position.lat)
        onSelectLocation(point);
    }
  }

  return (
    <AsyncTypeahead
      id="search-location"
      isLoading={isLoading}
      labelKey="displayAddress"
      maxResults={PER_PAGE - 1}
      minLength={2}
      onInputChange={handleInputChange}
      onSearch={handleSearch}
      onChange={handleSelectedChanged}
      options={searchResult.options}
      placeholder="Search for a location..."
      renderMenuItemChildren={(option: Option) => {
        const locationOption = option as SearchLocationOption
        return (
          <div key={locationOption.id}>
            <span>{locationOption.displayAddress}</span>
          </div>
        )
      }}
      useCache={false}
    />
  )
}
