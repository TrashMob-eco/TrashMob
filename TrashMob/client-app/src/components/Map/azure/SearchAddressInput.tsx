import React from 'react'
import { useQuery } from '@tanstack/react-query';
import { data } from 'azure-maps-control';
import { AsyncTypeahead } from 'react-bootstrap-typeahead';
import { AzureMapSearchAddress } from '../../../services/maps';
import { SearchAddressResultItem } from './types';

const PER_PAGE = 50;

export type SearchAddressInputProps = {
  azureKey: string
  onSelectLocation: (position: data.Position) => void
}

export const SearchAddressInput = (props: SearchAddressInputProps) => {

  const { azureKey, onSelectLocation } = props

  const [query, setQuery] = React.useState<string>('');
  const { data: options, isLoading, refetch } = useQuery({
    queryKey: ["AzureMapSearchAddress", query],
    queryFn: async () => {
      const response = await AzureMapSearchAddress().service({ azureKey, query })
      return response.data.results as SearchAddressResultItem[]
    },
    initialData: () => [],
    enabled: false
  })

  const handleSearch = React.useCallback((q) => {
    refetch()
  }, []);

  function handleSelectedChanged(val: any) {
    if (val && val.length > 0) {
        var position = val[0].position;
        var point = new data.Position(position.lon, position.lat)
        onSelectLocation(point);
    }
  }

  return (
    <AsyncTypeahead
        id="search-address"
        isLoading={isLoading}
        labelKey="displayAddress"
        maxResults={PER_PAGE - 1}
        minLength={2}
        onInputChange={setQuery}
        onSearch={handleSearch}
        onChange={(selected) => handleSelectedChanged(selected)}
        options={options}
        paginate
        placeholder="Search for a location..."
        renderMenuItemChildren={(option: any) => (
            <div key={option.id}>
                <span>{option.displayAddress}</span>
            </div>
        )}
        useCache={false}
    />
  )
}