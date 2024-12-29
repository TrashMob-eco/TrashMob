import React, { useEffect, useRef, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import _ from 'lodash';
import { AzureMapSearchAddress } from '@/services/maps';
import { Command, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command';
import { useDebounce } from '@/hooks/useDebounce';
import * as MapStore from '@/store/MapStore';
import { cn } from '@/lib/utils';

export type SearchLocationOption = {
    id: string;
    displayAddress: string;
    address: {
        municipality: string;
        country: string;
        countrySubdivisionName: string;
    };
    position: {
        lat: number;
        lon: number;
    };
};

type SearchLocationGroup = {
    groupName: string;
    items: SearchLocationOption[];
};

export type RenderInputProps = {
    placeholder: string;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    value: string;
    onFocus: () => void;
    className: string;
};

export type AzureSearchLocationInputProps = {
    azureKey: string;
    onSelectLocation: (position: SearchLocationOption) => void;
    placeholder?: string
    className?: string;
    inputClassName?: string;
    listClassName?: string;
    renderInput?: (props: RenderInputProps) => React.ReactNode;
};

export function AzureSearchLocationInput(props: AzureSearchLocationInputProps) {
    const { azureKey, onSelectLocation, placeholder, className = '', inputClassName = '', listClassName = '', renderInput } = props;
    const [showSuggestion, setShowSuggestion] = React.useState<boolean>(false);
    const [query, setQuery] = React.useState<string>('');
    const debouncedQuery = useDebounce<string>(query, 200); // delay 200ms

    const commandRef = useRef<HTMLDivElement>(null);

    const handleClickOutside = (event: MouseEvent) => {
        if (commandRef.current && !commandRef.current.contains(event.target as Node)) {
            setShowSuggestion(false); // Close suggestions if clicked outside
        }
    };

    useEffect(() => {
        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    const {
        data: searchResult,
        isLoading,
        isFetching,
    } = useQuery<{ options: SearchLocationOption[]; totalResults: number }>({
        queryKey: AzureMapSearchAddress().key(debouncedQuery),
        queryFn: async () => {
            const response = await AzureMapSearchAddress().service({ azureKey, query: debouncedQuery });
            const { data } = response;
            return {
                options: data.results.map((item) => ({
                    id: item.id,
                    displayAddress: item.address.freeformAddress,
                    address: item.address,
                    position: item.position,
                })),
                totalResults: data.summary.totalResults,
            };
        },
        initialData: () => ({ options: [], totalResults: 0 }),
        enabled: debouncedQuery.length >= 2,
    });

    const handleSelectSuggestion = (suggestionId: string) => {
        const suggestion = suggestions.find((s) => s.id === suggestionId);
        if (!suggestion) return;

        onSelectLocation(suggestion);
        setShowSuggestion(false);
    };

    const suggestions = (searchResult?.options || []) as SearchLocationOption[];
    // Group result by Country
    const suggestionGroups: SearchLocationGroup[] = _(suggestions)
        .groupBy((sgt: SearchLocationOption) => `${sgt.address.country}`)
        .map((items: SearchLocationOption[], groupName: string) => {
            return { groupName, items };
        })
        .value();

    const handleFocus = () => {
        setShowSuggestion(true); // Open suggestions on focus
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setQuery(e.target.value);
    };

    const customInputProps: RenderInputProps = {
        placeholder: placeholder ?? 'Location...',
        onChange: handleInputChange,
        value: query,
        onFocus: handleFocus,
        className: cn(inputClassName),
    };

    return (
        <div className='tailwind'>
            <Command
                ref={commandRef}
                shouldFilter={false}
                className={cn('!rounded-lg border md:min-w-[200px] relative', { 'shadow-md': open }, className)}
            >
                {renderInput ? (
                    renderInput(customInputProps)
                ) : (
                    <CommandInput
                        placeholder={placeholder ?? 'Location...'}
                        value={query}
                        onValueChange={(value) => setQuery(value)}
                        onFocus={handleFocus}
                        className={cn('!mb-0', inputClassName)}
                    />
                )}
                <CommandList
                    className={cn('max-w-[300px] hidden', { flex: showSuggestion && query.length > 1 }, listClassName)}
                >
                    {isLoading || isFetching ? (
                        <div className='p-2 text-sm text-gray-500'>Loading...</div>
                    ) : suggestionGroups.length ? (
                        suggestionGroups.map((group) => (
                            <CommandGroup heading={group.groupName} key={group.groupName} className='border-b'>
                                {group.items.map((item) => (
                                    <CommandItem
                                        key={item.id}
                                        value={item.id}
                                        className='font-light !py-3 cursor-pointer'
                                        onSelect={handleSelectSuggestion}
                                    >
                                        {item.displayAddress}
                                    </CommandItem>
                                ))}
                            </CommandGroup>
                        ))
                    ) : query.length > 1 ? (
                        <div className='p-2 text-sm text-gray-500'>No results found.</div>
                    ) : null}
                </CommandList>
            </Command>
        </div>
    );
}

export const AzureSearchLocationInputWithKey = (props: Omit<AzureSearchLocationInputProps, 'azureKey'>) => {
    const [azureKey, setAzureKey] = useState<string>('');
    
    useEffect(() => {
        MapStore.getOption().then((opts) => {
            setAzureKey(opts.subscriptionKey);
        });
    }, [])  

    return (
        <AzureSearchLocationInput {...props} azureKey={azureKey} />
    );
}